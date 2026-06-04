/**
 * TicketHub — Application Router
 *
 * Three distinct route groups with meta-driven portal detection:
 *   1. Public   — No portal meta, wrapped by AppHeader/AppFooter
 *   2. Organizer — meta.portal = 'organizer', rendered inside OrganizerLayout
 *   3. Admin     — meta.portal = 'admin', rendered inside AdminLayout
 *
 * Lazy-loaded feature pages keep the initial bundle slim.
 */
import { createRouter, createWebHistory } from 'vue-router'

/* ── Lazy-loaded layouts ───────────────────────────────────────────────────── */
const OrganizerLayout = () => import('@/shared/layouts/OrganizerLayout.vue')
const AdminLayout = () => import('@/shared/layouts/AdminLayout.vue')

/* ── Route Definitions ─────────────────────────────────────────────────────── */
const routes = [
  /* ╔══════════════════════════════════════════════╗
     ║  PUBLIC ROUTES                               ║
     ╚══════════════════════════════════════════════╝ */
  {
    path: '/',
    name: 'home',
    component: () => import('@/features/events/pages/HomePage.vue'),
  },
  {
    path: '/my-tickets',
    name: 'my-tickets',
    component: () => import('@/features/customer/pages/MyTicketsPage.vue'),
  },
  {
    path: '/event/:id',
    name: 'event-detail',
    component: () => import('@/features/events/pages/EventDetailPage.vue'),
  },
  {
    path: '/event/:id/booking',
    name: 'event-booking',
    component: () => import('@/features/booking/pages/EventBookingPage.vue'),
  },

  /* ╔══════════════════════════════════════════════╗
     ║  ORGANIZER PORTAL                            ║
     ╚══════════════════════════════════════════════╝ */
  {
    path: '/organizer',
    component: OrganizerLayout,
    meta: { portal: 'organizer' },
    children: [
      {
        path: '',
        name: 'organizer-dashboard',
        component: () => import('@/features/organizer/pages/OrganizerDashboard.vue'),
        meta: { portal: 'organizer' },
      },
      {
        path: 'create',
        name: 'organizer-create-event',
        component: () => import('@/features/organizer/pages/CreateEventPage.vue'),
        meta: { portal: 'organizer' },
      },
    ],
  },

  /* ╔══════════════════════════════════════════════╗
     ║  ADMIN PORTAL                                ║
     ╚══════════════════════════════════════════════╝ */
  {
    path: '/admin',
    component: AdminLayout,
    meta: { portal: 'admin' },
    children: [
      {
        path: '',
        name: 'admin-dashboard',
        component: () => import('@/features/admin/pages/AdminDashboard.vue'),
        meta: { portal: 'admin' },
      },
    ],
  },

  /* ── Catch-All (redirect to home) ────────────────────────────────────────── */
  {
    path: '/:pathMatch(.*)*',
    redirect: '/',
  },
]

/* ── Router Instance ───────────────────────────────────────────────────────── */
const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(_to, _from, savedPosition) {
    if (savedPosition) return savedPosition
    return { top: 0, behavior: 'smooth' }
  },
})

export default router
