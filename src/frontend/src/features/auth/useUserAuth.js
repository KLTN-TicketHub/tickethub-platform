// ─── USER AUTH COMPOSABLE ───────────────────────────────────────────────────
// Manages user-facing authentication state (login, register, Google OAuth).
//
// Architecture:
//   ┌─────────────┐     ┌──────────────────┐     ┌──────────────┐
//   │  Component   │────▶│  useUserAuth()   │────▶│  apiClient   │
//   │  (UI only)   │     │  (state + logic) │     │  (core/axios)│
//   │              │◀────│                  │     └──────────────┘
//   └─────────────┘     └──────────────────┘
//
// Token Strategy:
//   - accessToken is stored in module-scope memory inside core/axios.js
//     via setAccessToken() / getAccessToken(). Never in localStorage.
//   - A lightweight boolean flag in localStorage ("ticket-hub:is-logged-in")
//     is used ONLY to decide whether to attempt a silent token refresh
//     on page load (prevents unnecessary /refresh-token calls for first-
//     time visitors who never logged in).
//   - The Refresh Token lives in an HttpOnly cookie managed by the backend.
// ────────────────────────────────────────────────────────────────────────────

import { reactive, ref, computed } from 'vue'
import axios from 'axios'
import apiClient, {
  setAccessToken,
  getAccessToken,
  clearAccessToken,
} from '../../core/axios'

// ─── CONSTANTS ──────────────────────────────────────────────────────────────

const LOGIN_FLAG_KEY = 'ticket-hub:is-logged-in'
const REFRESH_LOCK_KEY = 'ticket-hub:refresh-lock'
const LOCK_TIMEOUT = 8000   // ms
const LOCK_POLL = 120       // ms

// ─── SINGLETON STATE ────────────────────────────────────────────────────────
// Module-level reactive state shared by every component calling useUserAuth().

const state = reactive({
  /** @type {{ id?: string, name: string, email: string|null, imageUrl?: string, roles?: string[], initial: string } | null} */
  user: null,

  /** Whether the auth modal is visible */
  showAuth: false,

  /** Current modal mode: 'login' | 'register' */
  authMode: 'login',
})

const isLoading = ref(false)

// ─── IN-MEMORY REFRESH GUARD ────────────────────────────────────────────────
let ongoingRefresh = null

// ─── HELPERS ────────────────────────────────────────────────────────────────

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms))
}

/**
 * Decode the payload section of a JWT without a library.
 */
function decodeJwt(token) {
  try {
    const payload = token.split('.')[1]
    const json = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
    return json
  } catch {
    return null
  }
}

/**
 * Extract a user object from JWT claims and apply it to state.
 */
function applyUserFromToken(token) {
  if (!token) return
  const payload = decodeJwt(token)
  if (!payload) return

  const name =
    payload.name ||
    payload.unique_name ||
    payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
    payload.sub ||
    null

  if (name) {
    state.user = {
      name: String(name).split('@')[0] || String(name),
      email: payload.email || null,
      initial: String(name).charAt(0).toUpperCase(),
    }
  }
}

/**
 * Apply a richer user object from a /auth/profile API response.
 */
function applyUserFromProfile(profile) {
  if (!profile) return

  const displayName = profile.fullName || profile.userName || profile.name || profile.email || ''
  const fallbackName = profile.email ? String(profile.email).split('@')[0] : 'User'
  const resolvedName = String(displayName || fallbackName).trim() || 'User'

  state.user = {
    id: profile.id || null,
    name: resolvedName,
    email: profile.email || null,
    imageUrl: profile.imageUrl || null,
    roles: Array.isArray(profile.roles) ? profile.roles : [],
    initial: resolvedName.charAt(0).toUpperCase(),
  }
}

/**
 * Set the login flag in localStorage so we know to try refresh on next page load.
 */
function setLoginFlag() {
  try { localStorage.setItem(LOGIN_FLAG_KEY, 'true') } catch { /* noop */ }
}

function clearLoginFlag() {
  try { localStorage.removeItem(LOGIN_FLAG_KEY) } catch { /* noop */ }
}

// ─── EXPORTED UTILITIES (non-composable, for use in main.js / guards) ───────

/**
 * Check if the user has logged in before (lightweight localStorage check).
 * Used by main.js to decide whether to attempt a silent refresh on startup.
 */
export function isLoggedIn() {
  try {
    return localStorage.getItem(LOGIN_FLAG_KEY) === 'true'
  } catch {
    return false
  }
}

// ─── TOKEN REFRESH ──────────────────────────────────────────────────────────

/**
 * Cross-tab-safe token refresh with localStorage-based locking.
 *
 * Uses the global apiClient's baseURL to construct the refresh endpoint.
 * Makes a raw axios call (not through apiClient) to avoid triggering
 * the response interceptor's 401 retry logic in an infinite loop.
 *
 * @returns {Promise<string>} The new access token
 */
export async function tryRefresh() {
  // Serialize same-tab refreshes
  if (ongoingRefresh) return ongoingRefresh

  const acquireLock = async () => {
    const id = `${Date.now()}_${Math.random().toString(36).slice(2)}`
    const payload = JSON.stringify({ id, expiresAt: Date.now() + LOCK_TIMEOUT })
    const start = Date.now()

    while (Date.now() - start < LOCK_TIMEOUT) {
      const existing = localStorage.getItem(REFRESH_LOCK_KEY)
      if (existing) {
        try {
          const parsed = JSON.parse(existing)
          if (!parsed.expiresAt || Date.now() > parsed.expiresAt) {
            localStorage.removeItem(REFRESH_LOCK_KEY)
          } else {
            await sleep(LOCK_POLL)
            continue
          }
        } catch {
          localStorage.removeItem(REFRESH_LOCK_KEY)
        }
      }

      try {
        localStorage.setItem(REFRESH_LOCK_KEY, payload)
      } catch {
        await sleep(LOCK_POLL)
        continue
      }

      const confirm = localStorage.getItem(REFRESH_LOCK_KEY)
      if (confirm === payload) return { acquired: true, id }
      await sleep(LOCK_POLL)
    }
    return { acquired: false }
  }

  const releaseLock = (id) => {
    try {
      const raw = localStorage.getItem(REFRESH_LOCK_KEY)
      if (!raw) return
      const parsed = JSON.parse(raw)
      if (parsed.id === id) localStorage.removeItem(REFRESH_LOCK_KEY)
    } catch {
      try { localStorage.removeItem(REFRESH_LOCK_KEY) } catch { /* noop */ }
    }
  }

  const refreshUrl = `${apiClient.defaults.baseURL}/auth/refresh-token`

  const fallbackAttemptRefresh = async (attempts = 3) => {
    let lastErr = null
    for (let i = 0; i < attempts; i += 1) {
      try {
        return await axios.post(refreshUrl, {}, { withCredentials: true })
      } catch (err) {
        lastErr = err
        await sleep(150 + Math.random() * 200)
      }
    }
    throw lastErr
  }

  let lockInfo = null
  ongoingRefresh = (async () => {
    try {
      lockInfo = await acquireLock()

      if (!lockInfo.acquired) {
        // Another tab is likely refreshing. Wait and then attempt a fallback.
        await sleep(300 + Math.random() * 400)
        const resp = await fallbackAttemptRefresh()
        const newToken = resp.data?.accessToken || null
        setAccessToken(newToken)
        setLoginFlag()
        await _loadProfile(newToken)
        return newToken
      }

      // We acquired the lock — perform the canonical refresh
      const resp = await axios.post(refreshUrl, {}, { withCredentials: true })
      const newToken = resp.data?.accessToken || null
      setAccessToken(newToken)
      setLoginFlag()
      await _loadProfile(newToken)
      return newToken
    } catch (e) {
      clearAccessToken()
      clearLoginFlag()
      state.user = null
      throw e
    } finally {
      if (lockInfo && lockInfo.acquired) releaseLock(lockInfo.id)
      ongoingRefresh = null
    }
  })()

  return ongoingRefresh
}

/**
 * Attempt to load user profile; fallback to JWT decode.
 * @private
 */
async function _loadProfile(token) {
  try {
    const resp = await apiClient.get('/auth/profile')
    applyUserFromProfile(resp.data?.data || resp.data || null)
  } catch {
    applyUserFromToken(token)
  }
}

// ─── GOOGLE OAUTH ───────────────────────────────────────────────────────────

/**
 * Redirect the browser to the Google OAuth consent screen.
 */
export function redirectToGoogle(returnUrl) {
  const targetReturnUrl = returnUrl || window.location.href
  const redirectEndpoint = `${apiClient.defaults.baseURL}/auth/google/redirect`
  window.location.href = `${redirectEndpoint}?returnUrl=${encodeURIComponent(targetReturnUrl)}`
}

/**
 * Open a popup window for Google OAuth login.
 */
export function openGooglePopup(returnUrl) {
  const callbackUrl = new URL(
    `/auth/callback?next=${encodeURIComponent(returnUrl || window.location.href)}`,
    window.location.origin
  ).toString()

  const redirectEndpoint = `${apiClient.defaults.baseURL}/auth/google/redirect`
  const popupUrl = `${redirectEndpoint}?returnUrl=${encodeURIComponent(callbackUrl)}`

  const width = 520
  const height = 700
  const left = window.screenX + Math.max(0, (window.outerWidth - width) / 2)
  const top = window.screenY + Math.max(0, (window.outerHeight - height) / 2)
  const features = [
    `width=${width}`,
    `height=${height}`,
    `left=${Math.round(left)}`,
    `top=${Math.round(top)}`,
    'resizable=yes',
    'scrollbars=yes',
    'noopener=no',
    'noreferrer=no',
  ].join(',')

  const popup = window.open(popupUrl, 'ticketHubGoogleLogin', features)
  if (popup) popup.focus()
  return popup
}

/**
 * Handle the Google OAuth callback inside the popup window.
 * Called from AuthCallback.vue after the backend redirects back.
 */
export async function handleGooglePopupCallback() {
  let token = null
  let lastError = null

  for (let attempt = 0; attempt < 5; attempt += 1) {
    try {
      if (attempt > 0) await sleep(250)
      token = await tryRefresh()
      break
    } catch (error) {
      lastError = error
    }
  }

  if (!token) throw lastError || new Error('Failed to refresh after Google login')

  // Profile was already loaded inside tryRefresh → _loadProfile

  if (window.opener && window.opener !== window) {
    window.opener.postMessage({ type: 'ticket-hub:auth-success', token }, window.location.origin)
  }
  return token
}

// ─── ROLE HELPERS ───────────────────────────────────────────────────────────

const ROLE_STORAGE_KEY = 'ticket-hub-current-role'
const ROLE_LOGIN_PATHS = {
  admin: '/admin/login',
  organizer: '/organizer/login',
  staff: '/staff/login',
  user: '/login',
}

function normalizeRole(role) {
  if (!role) return 'user'
  const value = String(role).toLowerCase()
  return ROLE_LOGIN_PATHS[value] ? value : 'user'
}

function getRoleFromPath(pathname = window.location.pathname) {
  if (pathname.startsWith('/admin')) return 'admin'
  if (pathname.startsWith('/organizer')) return 'organizer'
  if (pathname.startsWith('/staff')) return 'staff'
  return 'user'
}

export function setCurrentRole(role) {
  const normalized = normalizeRole(role)
  sessionStorage.setItem(ROLE_STORAGE_KEY, normalized)
  return normalized
}

export function getCurrentRole() {
  const storedRole = sessionStorage.getItem(ROLE_STORAGE_KEY)
  if (storedRole) return normalizeRole(storedRole)
  const roleFromPath = getRoleFromPath()
  sessionStorage.setItem(ROLE_STORAGE_KEY, roleFromPath)
  return roleFromPath
}

export function clearCurrentRole() {
  sessionStorage.removeItem(ROLE_STORAGE_KEY)
}

export function redirectToRoleLogin(role = getCurrentRole()) {
  const targetRole = normalizeRole(role)
  const targetPath = ROLE_LOGIN_PATHS[targetRole] || ROLE_LOGIN_PATHS.user
  window.location.replace(targetPath)
}

export function shouldRedirectToLogin(error) {
  const status = error?.response?.status
  return status === 400 || status === 401 || status === 404
}

// ─── COMPOSABLE ─────────────────────────────────────────────────────────────

/**
 * Vue 3 Composable for user-facing authentication.
 *
 * Usage:
 * ```js
 * const { user, isAuthenticated, loginUser, logoutUser, openAuth } = useUserAuth()
 * ```
 */
export function useUserAuth() {
  // ── Computed ──────────────────────────────────────────────────────────
  const user = computed(() => state.user)
  const isAuthenticated = computed(() => !!state.user)
  const showAuth = computed(() => state.showAuth)
  const authMode = computed(() => state.authMode)

  // ── Modal Actions ────────────────────────────────────────────────────
  function openAuth(mode = 'login') {
    state.authMode = mode
    state.showAuth = true
  }

  function closeAuth() {
    state.showAuth = false
  }

  function setAuthMode(mode) {
    state.authMode = mode
  }

  // ── Login (real API call) ────────────────────────────────────────────
  async function loginUser(email, password) {
    isLoading.value = true
    try {
      const { data } = await apiClient.post('/auth/login', { email, password })

      // Store token in memory (core/axios module scope)
      setAccessToken(data.accessToken)
      setLoginFlag()

      // Apply user from response or decode JWT
      if (data.user) {
        const u = data.user
        const name = u.fullName || u.userName || u.name || u.email?.split('@')[0] || 'User'
        state.user = {
          id: u.id || null,
          name,
          email: u.email || email,
          imageUrl: u.imageUrl || null,
          roles: Array.isArray(u.roles) ? u.roles : [],
          initial: name.charAt(0).toUpperCase(),
        }
      } else {
        applyUserFromToken(data.accessToken)
      }

      state.showAuth = false
      return true
    } finally {
      isLoading.value = false
    }
  }

  // ── Register (real API call) ─────────────────────────────────────────
  async function registerUser(name, email, password) {
    isLoading.value = true
    try {
      const { data } = await apiClient.post('/auth/register', { name, email, password })

      // If the backend returns a token on registration, auto-login
      if (data.accessToken) {
        setAccessToken(data.accessToken)
        setLoginFlag()

        if (data.user) {
          const u = data.user
          const displayName = u.fullName || u.userName || u.name || name
          state.user = {
            id: u.id || null,
            name: displayName,
            email: u.email || email,
            imageUrl: u.imageUrl || null,
            roles: Array.isArray(u.roles) ? u.roles : [],
            initial: displayName.charAt(0).toUpperCase(),
          }
        } else {
          state.user = {
            name,
            email,
            initial: name.charAt(0).toUpperCase(),
          }
        }
      } else {
        // Backend doesn't auto-login on register — set minimal user object
        state.user = {
          name,
          email,
          initial: name.charAt(0).toUpperCase(),
        }
      }

      state.showAuth = false
      return true
    } finally {
      isLoading.value = false
    }
  }

  // ── Logout ───────────────────────────────────────────────────────────
  async function logoutUser() {
    try {
      const currentToken = getAccessToken()
      if (currentToken) {
        await apiClient.post('/auth/logout').catch(() => null)
      }
    } finally {
      clearAccessToken()
      clearLoginFlag()
      state.user = null
      clearCurrentRole()
      window.location.replace('/')
    }
  }

  // ── Return public API ────────────────────────────────────────────────
  return {
    // State (readonly computed)
    user,
    isAuthenticated,
    isLoading: computed(() => isLoading.value),
    showAuth,
    authMode,

    // Modal actions
    openAuth,
    closeAuth,
    setAuthMode,

    // Auth actions
    loginUser,
    registerUser,
    logoutUser,

    // Google OAuth
    openGooglePopup,
    redirectToGoogle,
    handleGooglePopupCallback,

    // Session
    tryRefresh,
    isLoggedIn,
  }
}

// ─── DIRECT STATE EXPORT (for non-composable contexts) ──────────────────────
// Used by components that need to read user state outside of setup(),
// e.g. the App.vue handleAuthMessage listener.
export const userAuthState = state
export { isLoading as userAuthLoading }
