<template>
  <header
    class="sticky top-0 z-50 glass-modal border-b border-border-light/50"
  >
    <nav
      class="max-w-7xl mx-auto flex items-center justify-between h-16 px-4 sm:px-6 lg:px-8"
    >
      <!-- ── Logo ───────────────────────────────────────────────────────── -->
      <router-link
        to="/"
        class="flex items-center gap-2.5 group"
      >
        <div
          class="w-9 h-9 rounded-lg bg-primary/15 border border-primary/25 flex items-center justify-center
                 transition-all duration-300 group-hover:bg-primary/20 group-hover:shadow-glow"
        >
          <svg class="w-5 h-5 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z" />
            <path d="M13 5v2" /><path d="M13 17v2" /><path d="M13 11v2" />
          </svg>
        </div>
        <span class="font-heading font-bold text-lg text-main tracking-tight">
          Ticket<span class="text-primary">Hub</span>
        </span>
      </router-link>

      <!-- ── Desktop Navigation ─────────────────────────────────────────── -->
      <div class="hidden md:flex items-center gap-1">
        <router-link
          v-for="link in navLinks"
          :key="link.to"
          :to="link.to"
          class="nav-link"
          active-class="!text-primary !bg-primary-ghost"
        >
          {{ link.label }}
        </router-link>
      </div>

      <!-- ── Actions ────────────────────────────────────────────────────── -->
      <div class="flex items-center gap-3">
        <BaseButton
          variant="ghost"
          size="sm"
          class="hidden sm:inline-flex"
          @click="$router.push('/organizer')"
        >
          <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <circle cx="12" cy="12" r="10" /><path d="M8 12h8" /><path d="M12 8v8" />
          </svg>
          Create Event
        </BaseButton>

        <!-- Authenticated: User Menu -->
        <template v-if="authStore.isAuthenticated">
          <div class="relative" ref="userMenuRef">
            <button
              class="flex items-center gap-2.5 px-2.5 py-1.5 rounded-xl cursor-pointer
                     transition-all duration-200 hover:bg-white/[0.04]"
              @click="isUserMenuOpen = !isUserMenuOpen"
              :aria-expanded="isUserMenuOpen"
              aria-haspopup="true"
            >
              <div
                class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold
                       transition-all duration-200"
                :class="avatarClasses"
              >
                {{ authStore.userInitial }}
              </div>
              <span class="hidden sm:block text-sm font-medium text-main max-w-[120px] truncate">
                {{ authStore.userDisplayName }}
              </span>
              <svg
                class="w-3.5 h-3.5 text-muted transition-transform duration-200"
                :class="{ 'rotate-180': isUserMenuOpen }"
                viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"
              >
                <polyline points="6 9 12 15 18 9" />
              </svg>
            </button>

            <!-- Dropdown -->
            <Transition
              enter-active-class="transition-all duration-200 ease-out"
              enter-from-class="opacity-0 scale-95 -translate-y-1"
              enter-to-class="opacity-100 scale-100 translate-y-0"
              leave-active-class="transition-all duration-150 ease-in"
              leave-from-class="opacity-100 scale-100"
              leave-to-class="opacity-0 scale-95 -translate-y-1"
            >
              <div
                v-if="isUserMenuOpen"
                class="absolute right-0 top-full mt-2 w-56 glass-panel p-1.5 z-50"
              >
                <!-- User Info -->
                <div class="px-3 py-2.5 mb-1">
                  <p class="text-sm font-medium text-main truncate">
                    {{ authStore.userDisplayName }}
                  </p>
                  <p class="text-xs text-dimmed truncate">
                    {{ authStore.user?.email }}
                  </p>
                  <span
                    class="inline-block mt-1.5 text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full"
                    :class="roleBadgeClasses"
                  >
                    {{ authStore.userRole }}
                  </span>
                </div>

                <div class="h-px bg-border-main mx-1.5 my-1" />

                <!-- Menu Items -->
                <button
                  class="dropdown-item"
                  @click="handleMenuAction('my-tickets')"
                >
                  <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z" />
                    <path d="M13 5v2" /><path d="M13 17v2" /><path d="M13 11v2" />
                  </svg>
                  My Tickets
                </button>

                <button
                  v-if="authStore.userRole === 'organizer' || authStore.userRole === 'admin'"
                  class="dropdown-item"
                  @click="handleMenuAction('organizer')"
                >
                  <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <rect width="7" height="9" x="3" y="3" rx="1" />
                    <rect width="7" height="5" x="14" y="3" rx="1" />
                    <rect width="7" height="9" x="14" y="12" rx="1" />
                    <rect width="7" height="5" x="3" y="16" rx="1" />
                  </svg>
                  Organizer Portal
                </button>

                <button
                  v-if="authStore.userRole === 'admin'"
                  class="dropdown-item"
                  @click="handleMenuAction('admin')"
                >
                  <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10" />
                  </svg>
                  Admin Portal
                </button>

                <div class="h-px bg-border-main mx-1.5 my-1" />

                <button
                  class="dropdown-item !text-danger"
                  @click="handleLogout"
                >
                  <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                    <polyline points="16 17 21 12 16 7" />
                    <line x1="21" y1="12" x2="9" y2="12" />
                  </svg>
                  Log out
                </button>
              </div>
            </Transition>
          </div>
        </template>

        <!-- Not authenticated: Sign In -->
        <template v-else>
          <BaseButton
            variant="primary"
            size="sm"
            @click="$emit('openAuth')"
          >
            Sign In
          </BaseButton>
        </template>

        <!-- Mobile Menu Toggle -->
        <BaseButton
          variant="icon"
          size="sm"
          class="md:hidden"
          aria-label="Toggle mobile menu"
          @click="isMobileMenuOpen = !isMobileMenuOpen"
        >
          <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <template v-if="!isMobileMenuOpen">
              <line x1="4" y1="6" x2="20" y2="6" />
              <line x1="4" y1="12" x2="20" y2="12" />
              <line x1="4" y1="18" x2="20" y2="18" />
            </template>
            <template v-else>
              <line x1="18" y1="6" x2="6" y2="18" />
              <line x1="6" y1="6" x2="18" y2="18" />
            </template>
          </svg>
        </BaseButton>
      </div>
    </nav>

    <!-- ── Mobile Dropdown ──────────────────────────────────────────────── -->
    <Transition
      enter-active-class="transition-all duration-300 ease-out"
      enter-from-class="opacity-0 -translate-y-2 max-h-0"
      enter-to-class="opacity-100 translate-y-0 max-h-64"
      leave-active-class="transition-all duration-200 ease-in"
      leave-from-class="opacity-100 translate-y-0 max-h-64"
      leave-to-class="opacity-0 -translate-y-2 max-h-0"
    >
      <div
        v-if="isMobileMenuOpen"
        class="md:hidden overflow-hidden border-t border-border-light/30 px-4 pb-4"
      >
        <div class="flex flex-col gap-1 pt-3">
          <router-link
            v-for="link in navLinks"
            :key="link.to"
            :to="link.to"
            class="nav-link"
            active-class="!text-primary !bg-primary-ghost"
            @click="isMobileMenuOpen = false"
          >
            {{ link.label }}
          </router-link>
        </div>
      </div>
    </Transition>
  </header>
</template>

<script setup>
/**
 * AppHeader — Public navigation bar.
 *
 * Shows "Sign In" when unauthenticated (emits `open-auth` to parent).
 * When authenticated, shows the user avatar + dropdown with role-based
 * portal links and a logout action.
 */
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store'
import BaseButton from '@/shared/components/BaseButton.vue'

defineEmits(['openAuth'])

const router = useRouter()
const authStore = useAuthStore()

const isMobileMenuOpen = ref(false)
const isUserMenuOpen = ref(false)
const userMenuRef = ref(null)

const navLinks = [
  { to: '/', label: 'Discover' },
  { to: '/my-tickets', label: 'My Tickets' },
]

/* ── Role-based styling ────────────────────────────────────────────────────── */
const avatarClasses = computed(() => {
  switch (authStore.userRole) {
    case 'admin':
      return 'bg-danger/15 text-danger border border-danger/25'
    case 'organizer':
      return 'bg-primary/15 text-primary border border-primary/25'
    default:
      return 'bg-info/15 text-info border border-info/25'
  }
})

const roleBadgeClasses = computed(() => {
  switch (authStore.userRole) {
    case 'admin':
      return 'bg-danger/15 text-danger'
    case 'organizer':
      return 'bg-primary/15 text-primary'
    default:
      return 'bg-info/15 text-info'
  }
})

/* ── Dropdown handlers ─────────────────────────────────────────────────────── */
function handleMenuAction(target) {
  isUserMenuOpen.value = false
  switch (target) {
    case 'my-tickets':
      router.push('/my-tickets')
      break
    case 'organizer':
      router.push('/organizer')
      break
    case 'admin':
      router.push('/admin')
      break
  }
}

function handleLogout() {
  isUserMenuOpen.value = false
  authStore.logout()
}

/* ── Click-outside to close dropdown ───────────────────────────────────────── */
function handleClickOutside(event) {
  if (userMenuRef.value && !userMenuRef.value.contains(event.target)) {
    isUserMenuOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside, true)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside, true)
})
</script>

<style scoped>
@reference "@/app.css";

.nav-link {
  @apply px-3.5 py-2 text-sm font-medium text-muted rounded-lg
         transition-all duration-200 ease-out
         hover:text-main hover:bg-white/[0.04];
}

.dropdown-item {
  @apply flex items-center gap-2.5 w-full px-3 py-2 text-sm text-muted
         rounded-lg cursor-pointer bg-transparent border-0
         transition-all duration-150 ease-out
         hover:text-main hover:bg-white/[0.05];
}
</style>
