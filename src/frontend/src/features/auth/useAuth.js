// ─── AUTH COMPOSABLE ────────────────────────────────────────────────────────
// Central authentication composable for the TicketHub admin panel.
//
// Architecture:
//   ┌─────────────┐     ┌──────────────┐     ┌──────────────┐
//   │  Component   │────▶│  useAuth()   │────▶│   api.js     │
//   │  (UI only)   │     │  (state +    │     │  (HTTP only) │
//   │              │◀────│   logic)     │────▶│              │
//   └─────────────┘     └──────┬───────┘     └──────────────┘
//                              │
//                              ▼
//                     ┌──────────────┐
//                     │  axios.js    │
//                     │  (intercept  │
//                     │   + token)   │
//                     └──────────────┘
//
// State Strategy:
//   - `accessToken` is stored in module-scope memory inside axios.js
//     (via setAccessToken/clearAccessToken). Never in localStorage.
//   - `user` object is stored in a module-level reactive() singleton
//     below, shared across all components that call useAuth().
//   - On page refresh, both are lost. Call `tryRestoreSession()` on
//     app init to recover the session from the HttpOnly cookie.
// ─────────────────────────────────────────────────────────────────────────────

import { reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import {
  setAccessToken,
  clearAccessToken,
  onAuthFailure,
} from '../../core/axios'
import { loginAdmin, logoutAdmin, refreshSession } from './api'

// ─── SINGLETON AUTH STATE ───────────────────────────────────────────────────
// Module-level reactive object. Every component calling useAuth() gets
// the SAME reactive references, ensuring a single source of truth.
const state = reactive({
  /** @type {{ id: string, email: string, role: string } | null} */
  user: null,

  /** True while a login or session restore is in progress */
  isLoading: false,

  /** True once tryRestoreSession has been called (prevents double-init) */
  isInitialized: false,

  /** Human-readable error message from the last failed operation */
  error: null,
})

// ─── AUTH FAILURE REGISTRATION ──────────────────────────────────────────────
// Track whether we've already registered the onAuthFailure callback
// from axios.js, so we don't overwrite it on every useAuth() call.
let _failureCallbackRegistered = false

// ─── INTERNAL HELPERS ───────────────────────────────────────────────────────

/**
 * Decode the payload section of a JWT without a library.
 * Used to extract user info from the accessToken when the refresh
 * endpoint doesn't return a user object.
 *
 * @param {string} token - A JWT string (header.payload.signature)
 * @returns {object|null} - The decoded payload, or null if invalid
 */
function decodeJwtPayload(token) {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch {
    return null
  }
}

/**
 * Extract a normalized user object from a JWT payload.
 * ASP.NET Core Identity uses claim types like:
 *   - "sub" or "nameid" for user ID
 *   - "email"
 *   - "role"
 *
 * @param {object} payload - Decoded JWT payload
 * @returns {{ id: string, email: string, role: string }}
 */
function extractUserFromJwt(payload) {
  return {
    id: payload.sub || payload.nameid || payload.Id || '',
    email:
      payload.email ||
      payload[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
      ] ||
      '',
    role:
      payload.role ||
      payload[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ] ||
      '',
  }
}

/**
 * Hard-reset all client-side auth state.
 * Called on explicit logout AND on auth failure (token expired).
 */
function resetAuthState() {
  state.user = null
  state.error = null
  clearAccessToken()
}

// ─── MODULE-LEVEL EXPORTS (for Router Guard & non-component contexts) ───────
// These are exported separately from the composable so that the Vue Router
// navigation guard (which runs outside of setup()) can access auth state
// and trigger session restoration without calling useAuth().

/** Readonly reference to the singleton auth state. */
export const authState = state

/**
 * Promise deduplication: if tryRestoreSession is called concurrently
 * (e.g. by the router guard AND a component), only one HTTP request
 * is made. All callers await the same promise.
 */
let _sessionRestorePromise = null

/**
 * Attempt to restore a session after a full page refresh.
 *
 * How it works:
 *   1. Calls POST /auth/refresh-token (the browser sends the
 *      HttpOnly cookie automatically).
 *   2. If the server returns a new accessToken, we decode the JWT
 *      to recover the user object (since refresh doesn't return it).
 *   3. If the cookie is expired/invalid, we silently fail and the
 *      user remains unauthenticated.
 *
 * Safe to call from anywhere (components, guards, app init).
 *
 * @returns {Promise<boolean>} true if session was restored successfully
 */
export async function tryRestoreSession() {
  // Already initialized — return cached result
  if (state.isInitialized) return !!state.user

  // If a restore is already in-flight, piggyback on the same promise
  // instead of firing a duplicate HTTP request.
  if (_sessionRestorePromise) return _sessionRestorePromise

  _sessionRestorePromise = _performSessionRestore()

  try {
    return await _sessionRestorePromise
  } finally {
    _sessionRestorePromise = null
  }
}

/** @private */
async function _performSessionRestore() {
  state.isLoading = true

  try {
    const data = await refreshSession()

    setAccessToken(data.accessToken)

    // If the refresh response includes user data, use it directly.
    // Otherwise, decode the JWT to extract user claims.
    if (data.user) {
      state.user = data.user
    } else {
      const payload = decodeJwtPayload(data.accessToken)
      if (payload) {
        state.user = extractUserFromJwt(payload)
      }
    }

    return true
  } catch {
    // Session restoration failed (cookie expired / no cookie).
    // This is expected for first-time visitors — not an error.
    clearAccessToken()
    state.user = null
    return false
  } finally {
    state.isLoading = false
    state.isInitialized = true
  }
}

// ─── COMPOSABLE ─────────────────────────────────────────────────────────────

/**
 * Vue 3 Composable for authentication.
 *
 * Usage in a component's <script setup>:
 * ```js
 * import { useAuth } from '@/features/auth/useAuth'
 *
 * const { user, isAuthenticated, isLoading, login, logout } = useAuth()
 * ```
 *
 * @returns {object} Reactive auth state and action methods
 */
export function useAuth() {
  const router = useRouter()

  // ── Register the global auth failure handler (once) ─────────────────
  // When axios.js detects that a token refresh has failed (e.g. refresh
  // token expired), this callback fires to clean up and redirect.
  if (!_failureCallbackRegistered) {
    onAuthFailure(() => {
      resetAuthState()
      router.replace('/admin/login')
    })
    _failureCallbackRegistered = true
  }

  // ── Computed properties ─────────────────────────────────────────────
  const isAuthenticated = computed(() => !!state.user)
  const isAdmin = computed(
    () => state.user?.role?.toLowerCase() === 'admin'
  )

  // ── Actions ─────────────────────────────────────────────────────────

  /**
   * Authenticate with email & password.
   * On success: stores the token in memory, saves the user object,
   * and navigates to the admin dashboard.
   *
   * @param {string} email
   * @param {string} password
   */
  async function login(email, password) {
    state.isLoading = true
    state.error = null

    try {
      const data = await loginAdmin({ email, password })

      // 1. Store access token in memory (axios.js module scope)
      setAccessToken(data.accessToken)

      // 2. Store user object in reactive state
      state.user = data.user

      // 3. Navigate to admin dashboard
      await router.push('/admin/dashboard')
    } catch (err) {
      // Surface a user-friendly error message
      state.error =
        err.response?.data?.message ||
        err.response?.data?.title ||
        'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.'

      throw err
    } finally {
      state.isLoading = false
    }
  }

  /**
   * Log out the current user.
   * Clears all client state and notifies the server to revoke the
   * refresh token cookie. Redirects to the login page.
   */
  async function logout() {
    try {
      // Attempt server-side session invalidation.
      // If the backend hasn't implemented /auth/logout yet,
      // this will fail silently — client-side cleanup still runs.
      await logoutAdmin()
    } catch {
      // Swallow — server-side logout is best-effort
    }

    resetAuthState()
    await router.replace('/admin/login')
  }

  // ── Return public API ───────────────────────────────────────────────
  // Note: tryRestoreSession is defined at module scope (above) so it
  // can be used by the router guard outside of component setup().
  return {
    // Reactive state (readonly to prevent external mutation)
    user: computed(() => state.user),
    error: computed(() => state.error),
    isLoading: computed(() => state.isLoading),
    isAuthenticated,
    isAdmin,
    isInitialized: computed(() => state.isInitialized),

    // Actions
    login,
    logout,
    tryRestoreSession,
  }
}
