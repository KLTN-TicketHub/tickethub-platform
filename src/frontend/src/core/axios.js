// ─── GLOBAL AXIOS INSTANCE + JWT INTERCEPTORS ───────────────────────────────
// This module creates a single, pre-configured Axios instance used by all
// feature API modules. It handles:
//   1. Attaching the in-memory accessToken to every outgoing request.
//   2. Intercepting 401 responses to silently refresh the token.
//   3. A Queue/Mutex pattern to prevent "Token Race Conditions" —
//      if N requests fail with 401 at the same time, only ONE refresh
//      call is made; the other N-1 requests wait in a queue and retry
//      automatically once the new token arrives.
//
// SECURITY NOTE:
//   - The accessToken is stored ONLY in module-scope memory (never in
//     localStorage or sessionStorage) to minimize XSS attack surface.
//   - The Refresh Token is managed entirely by the backend via an
//     HttpOnly + Secure + SameSite=Strict cookie. The frontend never
//     reads or writes it directly.
// ─────────────────────────────────────────────────────────────────────────────

import axios from 'axios'

// ─── IN-MEMORY TOKEN STORE ──────────────────────────────────────────────────
// Module-private variable. Only accessible via the exported getter/setter.
let _accessToken = null

/** @param {string|null} token */
export const setAccessToken = (token) => {
  _accessToken = token
}

/** @returns {string|null} */
export const getAccessToken = () => _accessToken

export const clearAccessToken = () => {
  _accessToken = null
}

// ─── AUTH FAILURE CALLBACK ──────────────────────────────────────────────────
// Allows the auth composable (useAuth) to register a handler that fires
// when a token refresh fails (e.g. refresh token expired).
// This avoids importing vue-router directly in this infrastructure module,
// keeping the dependency graph clean.
let _onAuthFailure = null

/** @param {Function} callback */
export const onAuthFailure = (callback) => {
  _onAuthFailure = callback
}

// ─── AXIOS INSTANCE ─────────────────────────────────────────────────────────
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  timeout: 15000,
  withCredentials: true, // Required: sends HttpOnly cookies on every request
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
})

// ─── QUEUE / MUTEX STATE ────────────────────────────────────────────────────
//
// isRefreshing : acts as a mutex lock. When true, a refresh is in-flight.
// failedQueue  : stores { resolve, reject } pairs for requests waiting
//                on the refresh to complete.
//
// Flow diagram:
//
//   Request A (401) ──▶ isRefreshing=false ──▶ Lock mutex, call /refresh
//   Request B (401) ──▶ isRefreshing=true  ──▶ Push to queue, wait…
//   Request C (401) ──▶ isRefreshing=true  ──▶ Push to queue, wait…
//                                    │
//               ┌─── refresh succeeds ───┐
//               │                        │
//               ▼                        ▼
//   processQueue(null, newToken)     Unlock mutex
//       B retries with newToken
//       C retries with newToken
//       A retries with newToken (returned directly)
//
let isRefreshing = false
let failedQueue = []

/**
 * Resolve or reject every request sitting in the queue.
 * @param {Error|null} error  – pass null on success
 * @param {string|null} token – the fresh accessToken
 */
const processQueue = (error, token = null) => {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) {
      reject(error)
    } else {
      resolve(token)
    }
  })
  failedQueue = []
}

// ─── REQUEST INTERCEPTOR ────────────────────────────────────────────────────
// Runs before every outgoing request. If an accessToken exists in memory,
// attach it as a Bearer token.
apiClient.interceptors.request.use(
  (config) => {
    if (_accessToken) {
      config.headers.Authorization = `Bearer ${_accessToken}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// ─── RESPONSE INTERCEPTOR ───────────────────────────────────────────────────
// Intercepts every response. On 401, attempts a silent token refresh
// using the Queue/Mutex pattern described above.
apiClient.interceptors.response.use(
  // Success path — pass through untouched
  (response) => response,

  // Error path — handle 401
  async (error) => {
    const originalRequest = error.config

    // ── Guard clauses ────────────────────────────────────────────────────
    // 1. Only intercept 401 Unauthorized
    // 2. Don't retry a request that has already been retried (prevents
    //    infinite loops if the refreshed token is also invalid)
    // 3. Don't intercept the login request itself (it returns 401 for
    //    bad credentials, not for expired tokens)
    const isUnauthorized = error.response?.status === 401
    const isAlreadyRetried = originalRequest._retry
    const isAuthEndpoint = originalRequest.url?.includes('/auth/')

    if (!isUnauthorized || isAlreadyRetried || isAuthEndpoint) {
      return Promise.reject(error)
    }

    // Mark as retried BEFORE the queue check so queued requests also
    // won't enter an infinite retry loop if the new token fails too.
    originalRequest._retry = true

    // ── If a refresh is already in-flight, queue this request ────────
    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject })
      }).then((newToken) => {
        originalRequest.headers.Authorization = `Bearer ${newToken}`
        return apiClient(originalRequest)
      })
    }

    // ── Acquire the mutex lock and perform the refresh ───────────────
    isRefreshing = true

    try {
      // IMPORTANT: Use a raw `axios.post` (not `apiClient`) to call the
      // refresh endpoint. This bypasses our own interceptors and avoids
      // an infinite loop if the refresh itself returns 401.
      const { data } = await axios.post(
        `${apiClient.defaults.baseURL}/auth/refresh-token`,
        null, // No body — backend reads Refresh Token from HttpOnly cookie
        { withCredentials: true }
      )

      const newToken = data.accessToken

      // Update in-memory token
      setAccessToken(newToken)

      // Unblock all queued requests with the fresh token
      processQueue(null, newToken)

      // Retry the original request that triggered the refresh
      originalRequest.headers.Authorization = `Bearer ${newToken}`
      return apiClient(originalRequest)
    } catch (refreshError) {
      // Refresh failed (e.g. refresh token expired / revoked).
      // Reject all queued requests so their callers can handle the error.
      clearAccessToken()
      processQueue(refreshError, null)

      // Notify the auth layer (e.g. to redirect to /admin/login)
      if (_onAuthFailure) {
        _onAuthFailure(refreshError)
      }

      return Promise.reject(refreshError)
    } finally {
      // Always release the mutex lock, even if the refresh failed
      isRefreshing = false
    }
  }
)

export default apiClient
