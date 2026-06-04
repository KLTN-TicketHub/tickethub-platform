<template>
  <!-- Public shell: Header + content + Footer -->
  <template v-if="!isPortalRoute">
    <AppHeader @open-auth="showAuthModal = true" />

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pb-20 pt-6 min-h-[calc(100vh-140px)]">
      <router-view v-slot="{ Component, route: viewRoute }">
        <Transition name="page" mode="out-in">
          <component :is="Component" :key="viewRoute.path" />
        </Transition>
      </router-view>
    </main>

    <AppFooter />
  </template>

  <!-- Portal shell: Full-screen layout with its own chrome -->
  <template v-else>
    <router-view v-slot="{ Component, route: viewRoute }">
      <Transition name="page" mode="out-in">
        <component :is="Component" :key="viewRoute.path" />
      </Transition>
    </router-view>
  </template>

  <!-- Auth Modal (global, available on any public page) -->
  <AuthModal
    :visible="showAuthModal"
    @close="showAuthModal = false"
    @success="handleAuthSuccess"
  />
  <!-- Global Toast Notifications -->
  <AppToast />
</template>

<script setup>
/**
 * App.vue — Root Orchestrator
 *
 * Uses route.meta.portal to switch between the public shell
 * (AppHeader + content + AppFooter) and the portal shell
 * (OrganizerLayout / AdminLayout handle their own chrome).
 *
 * Hosts the global AuthModal and initialises auth state on mount.
 */
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/features/auth/store'
import AppHeader from '@/shared/layouts/AppHeader.vue'
import AppFooter from '@/shared/layouts/AppFooter.vue'
import AuthModal from '@/features/auth/components/AuthModal.vue'
import AppToast from '@/shared/components/AppToast.vue'

const route = useRoute()
const authStore = useAuthStore()

/** True when the user is inside /admin or /organizer portals */
const isPortalRoute = computed(() => !!route.meta?.portal)

/** Auth modal visibility */
const showAuthModal = ref(false)

/** Rehydrate auth state from localStorage on app startup */
onMounted(() => {
  authStore.initAuth()
})

function handleAuthSuccess() {
  console.info(`[TicketHub] Logged in as ${authStore.userDisplayName} (${authStore.userRole})`)
}
</script>

<style scoped>
/* ── Page Transition ───────────────────────────────────────────────────────── */
.page-enter-active {
  transition: opacity 0.35s ease, transform 0.35s cubic-bezier(0.16, 1, 0.3, 1);
}
.page-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}

.page-enter-from {
  opacity: 0;
  transform: translateY(16px);
}
.page-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
