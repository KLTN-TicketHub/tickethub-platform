import axios from 'axios'
import api from '../api/axios'
import { GOOGLE_REDIRECT, AUTH_REFRESH, AUTH_PROFILE, AUTH_LOGOUT } from '../api/endpoints'
import * as tokenService from './token.service'
import { store } from '../../stores/eventStore'

// in-memory single-tab refresh guard
let ongoingRefresh = null

function decodeJwt(token) {
  try {
    const payload = token.split('.')[1]
    const json = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
    return json
  } catch (e) {
    return null
  }
}

function applyUserFromToken(token) {
  if (!token) return
  const payload = decodeJwt(token)
  if (!payload) return
  // try common claim names
  const name = payload.name || payload.unique_name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload['sub'] || null
  if (name) {
    store.user = {
      name: String(name).split('@')[0] || String(name),
      email: payload.email || null,
      initial: String(name).charAt(0).toUpperCase()
    }
  }
}

function applyUserFromProfile(profile) {
  if (!profile) return

  const displayName = profile.fullName || profile.userName || profile.name || profile.email || ''
  const fallbackName = profile.email ? String(profile.email).split('@')[0] : 'User'
  const resolvedName = String(displayName || fallbackName).trim() || 'User'

  store.user = {
    id: profile.id || null,
    name: resolvedName,
    email: profile.email || null,
    imageUrl: profile.imageUrl || null,
    roles: Array.isArray(profile.roles) ? profile.roles : [],
    initial: resolvedName.charAt(0).toUpperCase()
  }
}

async function fetchProfile(accessToken) {
  const headers = accessToken ? { Authorization: `Bearer ${accessToken}` } : {}
  const response = await axios.get(buildApiUrl(AUTH_PROFILE), {
    withCredentials: true,
    headers
  })
  return response.data?.data || response.data || null
}

const ROLE_STORAGE_KEY = 'ticket-hub-current-role'
const ROLE_LOGIN_PATHS = {
  admin: '/admin/login',
  organizer: '/organizer/login',
  staff: '/staff/login',
  user: '/login'
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

function buildApiUrl(path) {
  const base = (import.meta.env.VITE_API_URL || '').replace(/\/$/, '')
  return `${base}${path}`
}

function buildAppUrl(path) {
  return new URL(path, window.location.origin).toString()
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms))
}

export function redirectToGoogle(returnUrl) {
  const targetReturnUrl = returnUrl || window.location.href
  window.location.href = `${buildApiUrl(GOOGLE_REDIRECT)}?returnUrl=${encodeURIComponent(targetReturnUrl)}`
}

export function openGooglePopup(returnUrl) {
  const callbackUrl = buildAppUrl(`/auth/callback?next=${encodeURIComponent(returnUrl || window.location.href)}`)
  const popupUrl = `${buildApiUrl(GOOGLE_REDIRECT)}?returnUrl=${encodeURIComponent(callbackUrl)}`
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
    'noreferrer=no'
  ].join(',')

  const popup = window.open(popupUrl, 'ticketHubGoogleLogin', features)
  if (popup) popup.focus()
  return popup
}

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

  try {
    const profile = await fetchProfile(token)
    applyUserFromProfile(profile)
  } catch (error) {
    applyUserFromToken(token)
  }

  if (window.opener && window.opener !== window) {
    window.opener.postMessage({ type: 'ticket-hub:auth-success', token }, window.location.origin)
  }
  return token
}

export async function tryRefresh() {
  // serialize same-tab refreshes
  if (ongoingRefresh) return ongoingRefresh

  const LOCK_KEY = 'ticket-hub:refresh-lock'
  const LOCK_TIMEOUT = 8000 // ms
  const LOCK_POLL = 120 // ms

  const acquireLock = async () => {
    const id = `${Date.now()}_${Math.random().toString(36).slice(2)}`
    const payload = JSON.stringify({ id, expiresAt: Date.now() + LOCK_TIMEOUT })
    const start = Date.now()

    while (Date.now() - start < LOCK_TIMEOUT) {
      const existing = localStorage.getItem(LOCK_KEY)
      if (existing) {
        try {
          const parsed = JSON.parse(existing)
          if (!parsed.expiresAt || Date.now() > parsed.expiresAt) {
            localStorage.removeItem(LOCK_KEY)
          } else {
            await sleep(LOCK_POLL)
            continue
          }
        } catch (err) {
          localStorage.removeItem(LOCK_KEY)
        }
      }

      try {
        localStorage.setItem(LOCK_KEY, payload)
      } catch (err) {
        await sleep(LOCK_POLL)
        continue
      }

      const confirm = localStorage.getItem(LOCK_KEY)
      if (confirm === payload) return { acquired: true, id }
      await sleep(LOCK_POLL)
    }
    return { acquired: false }
  }

  const releaseLock = (id) => {
    try {
      const raw = localStorage.getItem(LOCK_KEY)
      if (!raw) return
      const parsed = JSON.parse(raw)
      if (parsed.id === id) localStorage.removeItem(LOCK_KEY)
    } catch (err) {
      try { localStorage.removeItem(LOCK_KEY) } catch (e) {}
    }
  }

  const fallbackAttemptRefresh = async (attempts = 3) => {
    let lastErr = null
    for (let i = 0; i < attempts; i += 1) {
      try {
        const resp = await axios.post(buildApiUrl(AUTH_REFRESH), {}, { withCredentials: true })
        return resp
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
      // Another tab is likely refreshing. Wait a short time and then attempt a fallback refresh.
      await sleep(300 + Math.random() * 400)
      const resp = await fallbackAttemptRefresh()
      const newToken = resp.data?.accessToken || null
      tokenService.setToken(newToken)
      try {
        const profile = await fetchProfile(newToken)
        applyUserFromProfile(profile)
      } catch (error) {
        applyUserFromToken(newToken)
      }
      return newToken
    }

      // We acquired the lock, perform the canonical refresh
      const resp = await axios.post(buildApiUrl(AUTH_REFRESH), {}, { withCredentials: true })
      const newToken = resp.data?.accessToken || null
      tokenService.setToken(newToken)
      try {
        const profile = await fetchProfile(newToken)
        applyUserFromProfile(profile)
      } catch (error) {
        applyUserFromToken(newToken)
      }
      return newToken
    } catch (e) {
      tokenService.setToken(null)
      throw e
    } finally {
      if (lockInfo && lockInfo.acquired) releaseLock(lockInfo.id)
      ongoingRefresh = null
    }
  })()

  return ongoingRefresh
}

export async function logout() {
  try {
    // Only call backend logout if we have an access token (prevents 401 for anonymous clients)
    const currentToken = tokenService.getToken()
    if (currentToken) {
      // Use the api axios instance so the Authorization header (accessToken) is attached by interceptors
      await api.post(AUTH_LOGOUT, {}, { withCredentials: true })
    }
  } finally {
    tokenService.clearLogin()
    store.user = null
    clearCurrentRole()
    // After logout, always redirect to homepage
    window.location.replace('/')
  }
}

export default {
  redirectToGoogle,
  openGooglePopup,
  handleGooglePopupCallback,
  tryRefresh,
  logout,
  setCurrentRole,
  getCurrentRole,
  clearCurrentRole,
  redirectToRoleLogin
  ,shouldRedirectToLogin
}
