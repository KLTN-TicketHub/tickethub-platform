<template>
  <div class="flex flex-col h-screen bg-[#0A0F0D] text-white overflow-hidden">
    <header class="flex items-center justify-between gap-3 px-4 py-3 border-b border-white/5 bg-[#0A0F0D]/95 backdrop-blur-xl z-10 flex-shrink-0">
      <div class="flex items-center gap-2.5">
        <div class="w-9 h-9 bg-primary text-black rounded-xl flex items-center justify-center font-bold text-lg shadow-[0_0_15px_rgba(0,200,83,0.3)]">
          <PhQrCode weight="fill" />
        </div>
        <div class="flex flex-col leading-none">
          <span class="font-heading font-black text-[15px] tracking-tight text-white">TicketHub</span>
          <span class="text-[9px] font-bold text-primary uppercase tracking-[0.2em] mt-0.5">Soát vé</span>
        </div>
      </div>

      <div class="flex items-center gap-2.5">
        <NotificationBell />

        <button @click="handleLogout" class="w-9 h-9 flex items-center justify-center rounded-full bg-white/5 border border-white/10 text-white/60 hover:text-danger hover:border-danger/30 hover:bg-danger/10 transition-all">
          <PhSignOut class="text-lg" weight="bold" />
        </button>
      </div>
    </header>

    <main class="flex-1 min-h-0 overflow-y-auto">
      <router-view />
    </main>

    <ToastNotification />
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { logout as authLogout } from '../services/auth/auth.service'
import { resetNotifications } from '../stores/notificationStore'
import NotificationBell from '../components/layout/NotificationBell.vue'
import ToastNotification from '../components/admin/ToastNotification.vue'
import { PhQrCode, PhSignOut } from '@phosphor-icons/vue'

const router = useRouter()

const handleLogout = async () => {
  await authLogout()
  await resetNotifications()
  router.replace('/staff/login')
}
</script>
