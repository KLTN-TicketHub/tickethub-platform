<template>
  <div class="flex flex-col min-h-screen bg-[#0A0F0D] text-white relative overflow-x-hidden">
    <Topbar role="organizer" />
    
    <main class="flex-1 w-full mx-auto p-6 md:p-10">
      <router-view v-slot="{ Component }">
        <transition 
          name="fade" 
          mode="out-in"
          enter-active-class="transition-all duration-500 ease-[cubic-bezier(0.23,1,0.32,1)]"
          enter-from-class="opacity-0 translate-y-4"
          enter-to-class="opacity-100 translate-y-0"
          leave-active-class="transition-all duration-300 ease-in"
          leave-from-class="opacity-100 translate-y-0"
          leave-to-class="opacity-0 -translate-y-4"
        >
          <component :is="Component" />
        </transition>
      </router-view>
    </main>

    <!-- System Overlays -->
    <ToastNotification />
    <ConfirmModal />
  </div>
</template>

<script setup>
import Topbar from '../components/admin/Topbar.vue'
import ToastNotification from '../components/admin/ToastNotification.vue'
import ConfirmModal from '../components/admin/ConfirmModal.vue'
</script>
