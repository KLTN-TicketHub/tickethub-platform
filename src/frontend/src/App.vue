<template>
  <AppHeader v-if="!isAdminRoute" />

  <main :class="{ 'max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pb-20': !isAdminRoute }">
    <RouterView />
  </main>

  <AppFooter v-if="!isAdminRoute" />

  <BookingModal v-if="!isAdminRoute && store.showBooking && store.bookingEvent" :event="store.bookingEvent" @close="closeBooking" @success="handleBookingSuccess" />
  <AuthModal v-if="!isAdminRoute && store.showAuth" @close="closeAuth" />
  <ToastNotification v-if="!isAdminRoute && store.toast" :message="store.toast.message" :icon="store.toast.icon" @close="clearToast" />
</template>

<script setup>
import { computed, onMounted, onBeforeUnmount } from 'vue'
import { useRoute } from 'vue-router'
import { store, closeBooking, addTicket, clearToast, closeAuth } from './stores/eventStore'
import { tryRefresh } from './services/auth/auth.service'
import AppHeader from './components/layout/AppHeader.vue'
import AppFooter from './components/layout/AppFooter.vue'
import BookingModal from './components/BookingModal.vue'
import AuthModal from './components/AuthModal.vue'
import ToastNotification from './components/ToastNotification.vue'

const route = useRoute()

const isAdminRoute = computed(() => route.path.startsWith('/admin'))

const handleBookingSuccess = (bookedTicket) => {
  addTicket(bookedTicket)
}

const handleAuthMessage = async (event) => {
  if (event.origin !== window.location.origin) return

  if (event.data?.type === 'ticket-hub:auth-success') {
    try {
      await tryRefresh()
      store.toast = { message: 'Đăng nhập thành công!', icon: '👋' }
    } catch (err) {
      store.toast = { message: 'Đăng nhập không hoàn tất. Vui lòng thử lại.', icon: '⚠️' }
    } finally {
      closeAuth()
    }
  }

  if (event.data?.type === 'ticket-hub:auth-error') {
    const err = event.data?.error
    const msg = (err && (err.message || err.error || JSON.stringify(err))) || 'Đăng nhập Google thất bại.'
    store.toast = { message: msg, icon: '⚠️' }
    closeAuth()
  }
}

onMounted(() => {
  window.addEventListener('message', handleAuthMessage)
})

onBeforeUnmount(() => {
  window.removeEventListener('message', handleAuthMessage)
})
</script>
