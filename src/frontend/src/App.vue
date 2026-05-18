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
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { store, closeBooking, addTicket, clearToast, closeAuth } from './features/events/store'
import AppHeader from './layouts/AppHeader.vue'
import AppFooter from './layouts/AppFooter.vue'
import BookingModal from './features/booking/components/BookingModal.vue'
import AuthModal from './components/AuthModal.vue'
import ToastNotification from './components/ToastNotification.vue'

const route = useRoute()

const isAdminRoute = computed(() => route.path.startsWith('/admin'))

const handleBookingSuccess = (bookedTicket) => {
  addTicket(bookedTicket)
}
</script>
