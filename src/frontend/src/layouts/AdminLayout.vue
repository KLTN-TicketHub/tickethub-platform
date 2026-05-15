<template>
  <div class="flex min-h-screen bg-surface text-main relative overflow-x-hidden">
    <!-- Mobile Sidebar Overlay -->
    <div 
      v-if="isSidebarOpen" 
      class="fixed inset-0 bg-black/60 backdrop-blur-sm z-[90] md:hidden transition-all duration-300" 
      @click="toggleSidebar"
    ></div>

    <!-- Sidebar -->
    <Sidebar />

    <!-- Main Content -->
    <div class="flex-1 flex flex-col min-width-0 transition-all duration-300">
      <Topbar />
      
      <main class="flex-1 p-4 md:p-8 overflow-y-auto bg-surface">
        <router-view v-slot="{ Component }">
          <transition 
            name="fade" 
            mode="out-in"
            enter-active-class="animate-in fade-in slide-in-from-bottom-2 duration-300"
            leave-active-class="animate-out fade-out slide-out-to-top-2 duration-200"
          >
            <component :is="Component" />
          </transition>
        </router-view>
      </main>
    </div>

    <!-- System Overlays -->
    <ToastNotification />
    <ConfirmModal />
  </div>
</template>

<script setup>
import { isSidebarOpen, toggleSidebar } from '../stores/adminStore'
import Sidebar from '../components/admin/Sidebar.vue'
import Topbar from '../components/admin/Topbar.vue'
import ToastNotification from '../components/admin/ToastNotification.vue'
import ConfirmModal from '../components/admin/ConfirmModal.vue'
</script>
