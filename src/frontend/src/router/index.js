import { createRouter, createWebHistory } from 'vue-router'
import { isLoggedIn } from '../services/auth/token.service'
import { tryRefresh } from '../services/auth/auth.service'
import { store } from '../stores/eventStore'

import HomePage from '../pages/HomePage.vue'
import SearchPage from '../pages/SearchPage.vue'
import CategoryPage from '../pages/CategoryPage.vue'
import EventDetailPage from '../pages/EventDetailPage.vue'
import MyTicketsPage from '../pages/MyTicketsPage.vue'
import ProfilePage from '../pages/ProfilePage.vue'
import OrganizerPage from '../pages/OrganizerPage.vue'
import OrganizerCreateEventPage from '../pages/organizer/OrganizerCreateEventPage.vue'
import EarlyBirdPage from '../pages/EarlyBirdPage.vue'
import StarsPage from '../pages/StarsPage.vue'
import DestinationsPage from '../pages/DestinationsPage.vue'
import AuthCallback from '../pages/AuthCallback.vue'
import ForbiddenPage from '../pages/ForbiddenPage.vue'
import ActivateAccountPage from '../pages/ActivateAccountPage.vue'
import NotFoundPage from '../pages/NotFoundPage.vue'
import PaymentPage from '../pages/PaymentPage.vue'
import PaymentResultPage from '../pages/PaymentResultPage.vue'

// Admin
import AdminLayout from '../layouts/AdminLayout.vue'
import AdminDashboard from '../pages/admin/AdminDashboard.vue'
import EventsAdmin from '../pages/admin/EventsAdmin.vue'
import UsersAdmin from '../pages/admin/UsersAdmin.vue'
import OrdersAdmin from '../pages/admin/OrdersAdmin.vue'
import AdminLoginPage from '../pages/admin/AdminLoginPage.vue'
import ModeratorsAdmin from '../pages/admin/ModeratorsAdmin.vue'

// Moderator
import ModeratorLayout from '../layouts/ModeratorLayout.vue'
import ModeratorDashboard from '../pages/moderator/ModeratorDashboard.vue'
import ModeratorLoginPage from '../pages/moderator/ModeratorLoginPage.vue'
import ModeratorCreateVenuePage from '../pages/moderator/ModeratorCreateVenuePage.vue'
import ModeratorVenuesPage from '../pages/moderator/ModeratorVenuesPage.vue'
import ModeratorEditVenuePage from '../pages/moderator/ModeratorEditVenuePage.vue'
import ModeratorVenueSeatMapsPage from '../pages/moderator/ModeratorVenueSeatMapsPage.vue'
import ModeratorCreateSeatMapPage from '../pages/moderator/ModeratorCreateSeatMapPage.vue'
import ModeratorSeatMapDetailPage from '../pages/moderator/ModeratorSeatMapDetailPage.vue'
import ModeratorEventsPage from '../pages/moderator/ModeratorEventsPage.vue'
import ModeratorEventDetailPage from '../pages/moderator/ModeratorEventDetailPage.vue'

// Organizer
import OrganizerLayout from '../layouts/OrganizerLayout.vue'
import OrganizerLoginPage from '../pages/organizer/OrganizerLoginPage.vue'
import OrganizerRegisterPage from '../pages/organizer/OrganizerRegisterPage.vue'

const routes = [
  { path: '/', name: 'home', component: HomePage },
  { path: '/search', name: 'search', component: SearchPage },
  { path: '/:type(concerts|arts|sports|experiences|workshops|others)', name: 'category', component: CategoryPage },
  { path: '/event/:id', name: 'event-detail', component: EventDetailPage },
  { path: '/my-tickets', name: 'my-tickets', component: MyTicketsPage },
  { path: '/profile', name: 'profile', component: ProfilePage },
  { path: '/create-event', redirect: '/organizer/create-event' },
  { path: '/early-bird', name: 'early-bird', component: EarlyBirdPage },
  { path: '/stars', name: 'stars', component: StarsPage },
  { path: '/destinations', name: 'destinations', component: DestinationsPage },
  { path: '/auth/callback', name: 'auth-callback', component: AuthCallback },
  { path: '/admin/login', name: 'admin-login', component: AdminLoginPage },
  { path: '/moderator/login', name: 'moderator-login', component: ModeratorLoginPage },
  { path: '/organizer/login', name: 'organizer-login', component: OrganizerLoginPage },
  { path: '/organizer/register', name: 'organizer-register', component: OrganizerRegisterPage },
  { path: '/activate-account', name: 'activate-account', component: ActivateAccountPage },
  { path: '/403', name: 'forbidden', component: ForbiddenPage },
  // Payment flow
  { path: '/payment', name: 'payment', component: PaymentPage },
  { path: '/payment/result', name: 'payment-result', component: PaymentResultPage },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: NotFoundPage },
  
  // Admin Routes
  {
    path: '/admin',
    component: AdminLayout,
    meta: { requiresAuth: true, role: 'admin' },
    children: [
      { path: '', redirect: '/admin/dashboard' },
      { path: 'dashboard', name: 'admin-dashboard', component: AdminDashboard },
      { path: 'events', name: 'admin-events', component: EventsAdmin },
      { path: 'users', name: 'admin-users', component: UsersAdmin },
      { path: 'moderators', name: 'admin-moderators', component: ModeratorsAdmin },
      { path: 'orders', name: 'admin-orders', component: OrdersAdmin },
    ]
  },

  // Moderator Routes
  {
    path: '/moderator',
    component: ModeratorLayout,
    meta: { requiresAuth: true, role: 'moderator' },
    children: [
      { path: '', redirect: '/moderator/dashboard' },
      { path: 'dashboard', name: 'moderator-dashboard', component: ModeratorDashboard },
      { path: 'venues', name: 'moderator-venues', component: ModeratorVenuesPage },
      { path: 'venues/create', name: 'moderator-venues-create', component: ModeratorCreateVenuePage },
      { path: 'venues/:id/edit', name: 'moderator-venues-edit', component: ModeratorEditVenuePage },
      { path: 'venues/:id/seat-maps', name: 'moderator-venues-seatmaps', component: ModeratorVenueSeatMapsPage },
      { path: 'venues/:id/seat-maps/create', name: 'moderator-venues-seatmaps-create', component: ModeratorCreateSeatMapPage },
      { path: 'venues/:id/seat-maps/:seatMapId', name: 'moderator-venues-seatmaps-detail', component: ModeratorSeatMapDetailPage },
      { path: 'events', name: 'moderator-events', component: ModeratorEventsPage },
      { path: 'events/:id', name: 'moderator-event-detail', component: ModeratorEventDetailPage }
    ]
  },

  // Organizer Routes
  {
    path: '/organizer',
    component: OrganizerLayout,
    meta: { requiresAuth: true, role: 'organizer' },
    children: [
      { path: '', name: 'organizer', component: OrganizerPage },
      { path: 'create-event', name: 'organizer-create-event', component: OrganizerCreateEventPage },
      { path: 'events/:id', name: 'organizer-event-detail', component: () => import('../pages/organizer/OrganizerEventDetailPage.vue') }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  },
})

/** Đảm bảo store.user được hydrate từ refresh token nếu chưa có */
async function ensureUser() {
  if (!store.user) {
    await tryRefresh().catch(() => null)
  }
  return store.user
}

const DASHBOARD_PATHS = {
  admin: '/admin/dashboard',
  organizer: '/organizer',
  staff: '/staff/dashboard',
  moderator: '/moderator/dashboard',
  user: '/'
}

const LOGIN_PATHS = {
  admin: '/admin/login',
  organizer: '/organizer/login',
  staff: '/staff/login',
  moderator: '/moderator/login',
  user: '/'
}

function getExpectedRole(path) {
  if (path.startsWith('/admin')) return 'admin'
  if (path.startsWith('/organizer')) return 'organizer'
  if (path.startsWith('/staff')) return 'staff'
  if (path.startsWith('/moderator')) return 'moderator'
  return 'user'
}

function getUserRole(user) {
  if (!user) return 'user'
  const roles = user.roles || []
  if (roles.some(r => r.toLowerCase() === 'admin')) return 'admin'
  if (roles.some(r => r.toLowerCase() === 'moderator')) return 'moderator'
  if (roles.some(r => r.toLowerCase() === 'organizer')) return 'organizer'
  if (roles.some(r => r.toLowerCase() === 'staff')) return 'staff'
  return 'user'
}

router.beforeEach(async (to, from, next) => {
  // Bỏ qua kiểm tra đối với route callback auth và trang 403
  if (to.path === '/auth/callback' || to.path === '/403') {
    return next()
  }

  const expectedRole = getExpectedRole(to.path)
  const isLoginPage = to.path === LOGIN_PATHS[expectedRole] || to.path === '/organizer/register'

  // 1. Nếu chưa đăng nhập
  if (!isLoggedIn()) {
    if (expectedRole !== 'user' && !isLoginPage) {
      return next(LOGIN_PATHS[expectedRole])
    }
    return next()
  }

  // 2. Đã đăng nhập -> Đảm bảo có thông tin user
  const user = await ensureUser()
  if (!user) {
    if (expectedRole !== 'user' && !isLoginPage) {
      return next(LOGIN_PATHS[expectedRole])
    }
    return next()
  }

  const userRole = getUserRole(user)

  // 3. Nếu đang vào trang login khi đã đăng nhập
  const isAnyLoginPage = (Object.values(LOGIN_PATHS).includes(to.path) || to.path === '/organizer/register') && to.path !== '/'
  if (isAnyLoginPage) {
    return next(DASHBOARD_PATHS[userRole])
  }

  // 4. Kiểm tra sự phù hợp giữa quyền của user và phân vùng route
  if (userRole !== expectedRole) {
    console.warn(`Chuyển hướng: Người dùng quyền '${userRole}' không được truy cập tuyến đường thuộc phân vùng '${expectedRole}'`)
    return next(DASHBOARD_PATHS[userRole])
  }

  next()
})

export default router
