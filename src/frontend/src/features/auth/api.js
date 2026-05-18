// ─── AUTH API LAYER ─────────────────────────────────────────────────────────
// Pure API functions for the authentication feature.
// These functions handle HTTP communication only — no state management,
// no side-effects, no routing. That responsibility belongs to useAuth.js.
//
// API Contract (ASP.NET Core Identity):
//   POST /auth/login          → { accessToken, user: { id, email, role } }
//   POST /auth/refresh-token  → { accessToken }  (reads HttpOnly cookie)
//   POST /auth/logout         → 204 No Content   (clears HttpOnly cookie)
// ─────────────────────────────────────────────────────────────────────────────

import apiClient from '../../core/axios'

/**
 * Authenticate an admin user with email & password.
 *
 * On success, the backend will:
 *   1. Return an accessToken + user object in the response body.
 *   2. Set a new Refresh Token in an HttpOnly cookie (handled by browser).
 *
 * @param {{ email: string, password: string }} credentials
 * @returns {Promise<{ accessToken: string, user: { id: string, email: string, role: string } }>}
 */
export async function loginAdmin(credentials) {
  const { data } = await apiClient.post('/auth/login', {
    email: credentials.email,
    password: credentials.password,
  })
  return data
}

/**
 * Attempt to restore a session after a page refresh.
 *
 * The browser will automatically send the HttpOnly Refresh Token cookie.
 * If valid, the backend issues a fresh accessToken.
 *
 * NOTE: This function is safe to call through `apiClient` because the
 * response interceptor's guard (`isAuthEndpoint`) skips retry logic
 * for any URL containing '/auth/'.
 *
 * @returns {Promise<{ accessToken: string }>}
 */
export async function refreshSession() {
  const { data } = await apiClient.post('/auth/refresh-token')
  return data
}

/**
 * Invalidate the current session on the server.
 *
 * This tells the backend to clear/revoke the Refresh Token cookie.
 * If the backend doesn't implement this endpoint yet, the call will
 * fail silently (handled by the composable's try/catch).
 *
 * @returns {Promise<void>}
 */
export async function logoutAdmin() {
  await apiClient.post('/auth/logout')
}
