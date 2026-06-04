import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'
import { delay } from '@/shared/composables/useMockApi'

/**
 * Auth Store — Handles mock authentication, token persistence, and user state.
 *
 * Uses Pinia Setup Store syntax (Composition API) for maximum flexibility.
 *
 * Race-condition prevention:
 *   If `login()` is called while a previous login is in-flight,
 *   the duplicate call is silently rejected to prevent token corruption.
 *
 * Token persistence:
 *   A mock JWT and the associated user email are stored in localStorage
 *   so `initAuth()` can rehydrate state on page refresh.
 */

/* ── Mock User Database ────────────────────────────────────────────────────── */
const MOCK_USERS = {
  'admin@tickethub.local': {
    id: 'usr_admin_001',
    email: 'admin@tickethub.local',
    name: 'Admin User',
    avatar: null,
    role: 'admin',
  },
  'organizer@tickethub.local': {
    id: 'usr_org_001',
    email: 'organizer@tickethub.local',
    name: 'Organizer User',
    avatar: null,
    role: 'organizer',
  },
  'customer@tickethub.local': {
    id: 'usr_cust_001',
    email: 'customer@tickethub.local',
    name: 'Customer User',
    avatar: null,
    role: 'customer',
  },
}

const STORAGE_KEY_TOKEN = 'tickethub:token'
const STORAGE_KEY_EMAIL = 'tickethub:email'

/**
 * Generates a deterministic mock JWT-like string for a given email.
 * Not cryptographically meaningful — purely for realistic UX.
 */
function generateMockToken(email) {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
  const payload = btoa(
    JSON.stringify({
      sub: email,
      iat: Math.floor(Date.now() / 1000),
      exp: Math.floor(Date.now() / 1000) + 86400, // 24h
    }),
  )
  const signature = btoa(email.split('').reverse().join(''))
  return `${header}.${payload}.${signature}`
}

/* ── Store Definition ──────────────────────────────────────────────────────── */
export const useAuthStore = defineStore('auth', () => {
  const router = useRouter()

  /* ── State ───────────────────────────────────────────────────────────────── */
  const user = ref(null)
  const token = ref(null)
  const isLoading = ref(false)
  const error = ref(null)

  /* ── Getters ─────────────────────────────────────────────────────────────── */
  const isAuthenticated = computed(() => !!token.value && !!user.value)

  const userRole = computed(() => {
    if (!user.value) return null
    return user.value.role // 'customer' | 'organizer' | 'admin'
  })

  const userDisplayName = computed(() => {
    if (!user.value) return 'Guest'
    return user.value.name
  })

  const userInitial = computed(() => {
    if (!user.value?.name) return '?'
    return user.value.name.charAt(0).toUpperCase()
  })

  /* ── Actions ─────────────────────────────────────────────────────────────── */

  /**
   * Attempts login with the given credentials against mock user database.
   *
   * Race-condition guard: if `isLoading` is already true, the call is
   * rejected immediately so only one login flow runs at a time.
   *
   * @param {string} email
   * @param {string} password — Accepted value is any non-empty string.
   * @returns {Promise<void>}
   */
  async function login(email, password) {
    // ── Race-condition prevention ──
    if (isLoading.value) {
      console.warn('[AuthStore] Login already in-flight — ignoring duplicate call.')
      return Promise.reject(new Error('Login already in progress'))
    }

    // ── Reset & start ──
    error.value = null
    isLoading.value = true

    try {
      // Simulate network roundtrip
      await delay(1200)

      // Validate inputs
      if (!email || !password) {
        throw new Error('Email and password are required.')
      }

      const normalizedEmail = email.trim().toLowerCase()
      const matchedUser = MOCK_USERS[normalizedEmail]

      if (!matchedUser) {
        throw new Error('Invalid email or password. Try a demo account below.')
      }

      // ── Success path ──
      const mockToken = generateMockToken(normalizedEmail)

      user.value = { ...matchedUser }
      token.value = mockToken

      // Persist to localStorage
      localStorage.setItem(STORAGE_KEY_TOKEN, mockToken)
      localStorage.setItem(STORAGE_KEY_EMAIL, normalizedEmail)
    } catch (err) {
      error.value = err.message
      user.value = null
      token.value = null
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Logs the user out by clearing all state and storage,
   * then navigates to the home page.
   */
  function logout() {
    user.value = null
    token.value = null
    error.value = null

    localStorage.removeItem(STORAGE_KEY_TOKEN)
    localStorage.removeItem(STORAGE_KEY_EMAIL)

    router.push('/')
  }

  /**
   * Rehydrates auth state from localStorage on app startup.
   * Called once during app initialisation (main.js or App.vue).
   *
   * If a stored token exists but the email doesn't match any mock user,
   * the stale token is silently cleared (simulates token expiry).
   */
  function initAuth() {
    const storedToken = localStorage.getItem(STORAGE_KEY_TOKEN)
    const storedEmail = localStorage.getItem(STORAGE_KEY_EMAIL)

    if (!storedToken || !storedEmail) return

    const matchedUser = MOCK_USERS[storedEmail]

    if (matchedUser) {
      user.value = { ...matchedUser }
      token.value = storedToken
    } else {
      // Stale/invalid token — clean up
      localStorage.removeItem(STORAGE_KEY_TOKEN)
      localStorage.removeItem(STORAGE_KEY_EMAIL)
    }
  }

  /**
   * Clears any lingering error message.
   */
  function clearError() {
    error.value = null
  }

  return {
    // State
    user,
    token,
    isLoading,
    error,
    // Getters
    isAuthenticated,
    userRole,
    userDisplayName,
    userInitial,
    // Actions
    login,
    logout,
    initAuth,
    clearError,
  }
})
