import { createRouter, createWebHistory } from 'vue-router'
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

// Admin
import AdminLayout from '../layouts/AdminLayout.vue'
import AdminDashboard from '../pages/admin/AdminDashboard.vue'
import EventsAdmin from '../pages/admin/EventsAdmin.vue'
import UsersAdmin from '../pages/admin/UsersAdmin.vue'
import OrdersAdmin from '../pages/admin/OrdersAdmin.vue'

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
  
  // Admin Routes
  {
    path: '/admin',
    component: AdminLayout,
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

export default router
