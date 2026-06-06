import { createRouter, createWebHistory } from 'vue-router'
import { isLoggedIn } from '../services/auth/token.service'
import { tryRefresh } from '../services/auth/auth.service'
import { store } from '../stores/eventStore'

import HomePage from '../pages/HomePage.vue'
import CategoryPage from '../pages/CategoryPage.vue'
import EventDetailPage from '../pages/EventDetailPage.vue'
import MyTicketsPage from '../pages/MyTicketsPage.vue'
import ProfilePage from '../pages/ProfilePage.vue'
import OrganizerPage from '../pages/OrganizerPage.vue'
import CreateEventPage from '../pages/CreateEventPage.vue'
import EarlyBirdPage from '../pages/EarlyBirdPage.vue'
import StarsPage from '../pages/StarsPage.vue'
import DestinationsPage from '../pages/DestinationsPage.vue'
import AuthCallback from '../pages/AuthCallback.vue'
import ForbiddenPage from '../pages/ForbiddenPage.vue'

// Admin
import AdminLayout from '../layouts/AdminLayout.vue'
import AdminDashboard from '../pages/admin/AdminDashboard.vue'
import EventsAdmin from '../pages/admin/EventsAdmin.vue'
import UsersAdmin from '../pages/admin/UsersAdmin.vue'
import OrdersAdmin from '../pages/admin/OrdersAdmin.vue'
import AdminLoginPage from '../pages/admin/AdminLoginPage.vue'

const routes = [
  { path: '/', name: 'home', component: HomePage },
  { path: '/:type(concerts|arts|sports|experiences|workshops|others)', name: 'category', component: CategoryPage },
  { path: '/event/:id', name: 'event-detail', component: EventDetailPage },
  { path: '/my-tickets', name: 'my-tickets', component: MyTicketsPage },
  { path: '/profile', name: 'profile', component: ProfilePage },
  { path: '/organizer', name: 'organizer', component: OrganizerPage },
  { path: '/create-event', name: 'create-event', component: CreateEventPage },
  { path: '/early-bird', name: 'early-bird', component: EarlyBirdPage },
  { path: '/stars', name: 'stars', component: StarsPage },
  { path: '/destinations', name: 'destinations', component: DestinationsPage },
  { path: '/auth/callback', name: 'auth-callback', component: AuthCallback },
  { path: '/admin/login', name: 'admin-login', component: AdminLoginPage },
  { path: '/403', name: 'forbidden', component: ForbiddenPage },
  
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
      { path: 'orders', name: 'admin-orders', component: OrdersAdmin },
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
  user: '/'
}

const LOGIN_PATHS = {
  admin: '/admin/login',
  organizer: '/organizer/login',
  staff: '/staff/login',
  user: '/'
}

function getExpectedRole(path) {
  if (path.startsWith('/admin')) return 'admin'
  if (path.startsWith('/organizer')) return 'organizer'
  if (path.startsWith('/staff')) return 'staff'
  return 'user'
}

function getUserRole(user) {
  if (!user) return 'user'
  const roles = user.roles || []
  if (roles.some(r => r.toLowerCase() === 'admin')) return 'admin'
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
  const isLoginPage = to.path === LOGIN_PATHS[expectedRole]

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
  const isAnyLoginPage = Object.values(LOGIN_PATHS).includes(to.path) && to.path !== '/'
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
