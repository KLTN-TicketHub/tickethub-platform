// ─── VUE ROUTER NAVIGATION GUARD ────────────────────────────────────────────
// Protects admin routes by enforcing authentication and role-based access.
//
// How it works:
//
//   ┌──────────────┐     ┌──────────────────┐     ┌──────────────────┐
//   │  Navigation  │────▶│  beforeEach()    │────▶│  tryRestore      │
//   │  triggered   │     │  guard           │     │  Session()       │
//   └──────────────┘     └──────┬───────────┘     └──────────────────┘
//                               │
//                        ┌──────▼───────┐
//                        │ Route meta?  │
//                        └──────┬───────┘
//                               │
//               ┌───────────────┼───────────────┐
//               │               │               │
//        requiresAuth?    role check?     already on
//        No user → /login  Wrong role     login page?
//                          → /403 or /    Has user → /dashboard
//
// Integration:
//   Called once from main.js after creating the router:
//
//   ```js
//   import { setupRouterGuards } from './core/router/guard'
//   setupRouterGuards(router)
//   ```
// ─────────────────────────────────────────────────────────────────────────────

import { authState, tryRestoreSession } from '../../features/auth/useAuth'

/**
 * Install all navigation guards onto the given router instance.
 *
 * @param {import('vue-router').Router} router
 */
export function setupRouterGuards(router) {
  router.beforeEach(async (to, from) => {
    // ── Step 1: Wait for session initialization ───────────────────────
    // On first navigation (including page refresh), we must wait for
    // tryRestoreSession() to attempt recovering the user's session
    // from the HttpOnly refresh token cookie.
    //
    // Without this step, a page refresh would always redirect to
    // /admin/login because the in-memory accessToken is lost and
    // authState.user is null until the refresh completes.
    //
    // The `isInitialized` flag + promise deduplication inside
    // tryRestoreSession() ensure this only runs ONCE, no matter
    // how many guard invocations occur during startup.
    if (!authState.isInitialized) {
      await tryRestoreSession()
    }

    // ── Step 2: Determine what the target route requires ──────────────
    // We use `to.matched.some(...)` instead of `to.meta.requiresAuth`
    // directly, so that child routes inherit their parent's meta.
    // Example: /admin/dashboard inherits requiresAuth from /admin.
    const requiresAuth = to.matched.some(
      (record) => record.meta.requiresAuth
    )
    const requiredRole = to.matched.find(
      (record) => record.meta.role
    )?.meta.role

    // ── Step 3: Redirect unauthenticated users ────────────────────────
    if (requiresAuth && !authState.user) {
      return {
        path: '/admin/login',
        // Preserve the intended destination so we can redirect back
        // after a successful login (handled by the login page).
        query: { redirect: to.fullPath },
      }
    }

    // ── Step 4: Enforce role-based access ──────────────────────────────
    if (requiredRole && authState.user) {
      const userRole = authState.user.role?.toLowerCase()
      const needed = requiredRole.toLowerCase()

      if (userRole !== needed) {
        // User is authenticated but lacks the required role.
        // Redirect to home page (or a /403 page if you create one).
        return { path: '/' }
      }
    }

    // ── Step 5: Prevent authenticated users from seeing login page ─────
    // If a logged-in admin manually navigates to /admin/login,
    // redirect them to the dashboard instead.
    if (to.path === '/admin/login' && authState.user) {
      return { path: '/admin/dashboard' }
    }

    // ── Step 6: Allow navigation ──────────────────────────────────────
    // Returning undefined (or nothing) lets the navigation proceed.
  })
}
