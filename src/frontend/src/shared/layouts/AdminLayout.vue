<template>
  <div class="flex h-screen overflow-hidden bg-bg">
    <!-- ── Fixed Sidebar ──────────────────────────────────────────────── -->
    <aside
      class="hidden lg:flex lg:flex-col w-64 shrink-0 border-r border-border-main/60
             bg-surface/80 backdrop-blur-xl"
    >
      <!-- Sidebar Header -->
      <div class="flex items-center gap-3 px-6 h-16 border-b border-border-main/40">
        <div
          class="w-8 h-8 rounded-lg bg-danger/15 border border-danger/25
                 flex items-center justify-center"
        >
          <svg class="w-4 h-4 text-danger" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10" />
          </svg>
        </div>
        <div class="flex flex-col">
          <span class="font-heading font-bold text-sm text-main leading-tight">
            TicketHub
          </span>
          <span class="text-xs text-danger font-medium">Admin</span>
        </div>
      </div>

      <!-- Navigation Links -->
      <nav class="flex-1 px-3 py-4 space-y-1 overflow-y-auto no-scrollbar">
        <router-link
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          class="sidebar-link"
          active-class="sidebar-link--active"
          :class="{ 'sidebar-link--active': isExactActive(item.to) }"
        >
          <component :is="item.icon" class="w-[18px] h-[18px] shrink-0" />
          <span>{{ item.label }}</span>
        </router-link>
      </nav>

      <!-- Sidebar Footer -->
      <div class="px-3 pb-4 pt-2 border-t border-border-main/40 mt-auto">
        <div class="flex items-center gap-3 px-3 py-2.5 mb-3">
          <div
            class="w-8 h-8 rounded-full bg-danger/15 border border-danger/25
                   flex items-center justify-center text-xs font-bold text-danger"
          >
            A
          </div>
          <div class="flex flex-col min-w-0">
            <span class="text-sm font-medium text-main truncate">Admin</span>
            <span class="text-xs text-dimmed truncate">admin@tickethub.vn</span>
          </div>
        </div>

        <BaseButton
          variant="ghost"
          size="sm"
          class="w-full justify-start"
          @click="handleLogout"
        >
          <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
            <polyline points="16 17 21 12 16 7" />
            <line x1="21" y1="12" x2="9" y2="12" />
          </svg>
          <span class="text-danger/80">Log out</span>
        </BaseButton>
      </div>
    </aside>

    <!-- ── Mobile Sidebar Overlay ─────────────────────────────────────── -->
    <Transition
      enter-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-200"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="isMobileSidebarOpen"
        class="fixed inset-0 z-40 bg-black/60 backdrop-blur-sm lg:hidden"
        @click="isMobileSidebarOpen = false"
      />
    </Transition>

    <Transition
      enter-active-class="transition-transform duration-300 ease-out"
      enter-from-class="-translate-x-full"
      enter-to-class="translate-x-0"
      leave-active-class="transition-transform duration-200 ease-in"
      leave-from-class="translate-x-0"
      leave-to-class="-translate-x-full"
    >
      <aside
        v-if="isMobileSidebarOpen"
        class="fixed inset-y-0 left-0 z-50 flex flex-col w-64
               bg-surface border-r border-border-main lg:hidden"
      >
        <div class="flex items-center justify-between px-6 h-16 border-b border-border-main/40">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-lg bg-danger/15 border border-danger/25 flex items-center justify-center">
              <svg class="w-4 h-4 text-danger" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10" />
              </svg>
            </div>
            <span class="font-heading font-bold text-sm text-main">
              Ticket<span class="text-danger">Hub</span>
            </span>
          </div>

          <BaseButton
            variant="icon"
            size="sm"
            aria-label="Close sidebar"
            @click="isMobileSidebarOpen = false"
          >
            <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
              <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
            </svg>
          </BaseButton>
        </div>

        <nav class="flex-1 px-3 py-4 space-y-1 overflow-y-auto no-scrollbar">
          <router-link
            v-for="item in navItems"
            :key="item.to"
            :to="item.to"
            class="sidebar-link"
            active-class="sidebar-link--active"
            @click="isMobileSidebarOpen = false"
          >
            <component :is="item.icon" class="w-[18px] h-[18px] shrink-0" />
            <span>{{ item.label }}</span>
          </router-link>
        </nav>
      </aside>
    </Transition>

    <!-- ── Main Content Area ──────────────────────────────────────────── -->
    <div class="flex-1 flex flex-col min-w-0">
      <!-- Top Bar -->
      <header
        class="sticky top-0 z-30 flex items-center h-16 px-6 border-b border-border-main/40
               bg-bg/80 backdrop-blur-xl"
      >
        <BaseButton
          variant="icon"
          size="sm"
          class="lg:hidden mr-4"
          aria-label="Open sidebar"
          @click="isMobileSidebarOpen = true"
        >
          <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <line x1="4" y1="6" x2="20" y2="6" />
            <line x1="4" y1="12" x2="20" y2="12" />
            <line x1="4" y1="18" x2="20" y2="18" />
          </svg>
        </BaseButton>

        <div class="flex items-center gap-2 text-sm">
          <span class="text-dimmed">Admin</span>
          <svg class="w-3.5 h-3.5 text-dimmed" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <polyline points="9 18 15 12 9 6" />
          </svg>
          <span class="text-main font-medium">{{ currentPageLabel }}</span>
        </div>

        <div class="flex-1" />

        <BaseButton variant="ghost" size="sm" @click="$router.push('/')">
          <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
            <path d="m3 9 9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
            <polyline points="9 22 9 12 15 12 15 22" />
          </svg>
          Back to Site
        </BaseButton>
      </header>

      <!-- Scrollable Page Content -->
      <main class="flex-1 overflow-y-auto p-6 lg:p-8">
        <router-view v-slot="{ Component, route: viewRoute }">
          <Transition name="portal-page" mode="out-in">
            <component :is="Component" :key="viewRoute.path" />
          </Transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<script setup>
/**
 * AdminLayout — Self-contained admin portal shell.
 *
 * Identical structure to OrganizerLayout but with a danger/red accent
 * to visually distinguish the admin context. Fixed left sidebar,
 * responsive mobile drawer, sticky top bar, scrollable main content.
 */
import { ref, computed, h } from 'vue'
import { useRoute } from 'vue-router'
import BaseButton from '@/shared/components/BaseButton.vue'

const route = useRoute()
const isMobileSidebarOpen = ref(false)

/* ── Navigation Items ──────────────────────────────────────────────────────── */
const IconDashboard = (_, { attrs }) =>
  h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
    h('rect', { width: '7', height: '9', x: '3', y: '3', rx: '1' }),
    h('rect', { width: '7', height: '5', x: '14', y: '3', rx: '1' }),
    h('rect', { width: '7', height: '9', x: '14', y: '12', rx: '1' }),
    h('rect', { width: '7', height: '5', x: '3', y: '16', rx: '1' }),
  ])

const IconUsers = (_, { attrs }) =>
  h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
    h('path', { d: 'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2' }),
    h('circle', { cx: '9', cy: '7', r: '4' }),
    h('path', { d: 'M22 21v-2a4 4 0 0 0-3-3.87' }),
    h('path', { d: 'M16 3.13a4 4 0 0 1 0 7.75' }),
  ])

const IconCalendar = (_, { attrs }) =>
  h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
    h('path', { d: 'M8 2v4' }), h('path', { d: 'M16 2v4' }),
    h('rect', { width: '18', height: '18', x: '3', y: '4', rx: '2' }),
    h('path', { d: 'M3 10h18' }),
  ])

const IconSettings = (_, { attrs }) =>
  h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
    h('path', { d: 'M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z' }),
    h('circle', { cx: '12', cy: '12', r: '3' }),
  ])

const navItems = [
  { to: '/admin', label: 'Dashboard', icon: IconDashboard },
  { to: '/admin', label: 'Users', icon: IconUsers },
  { to: '/admin', label: 'Events', icon: IconCalendar },
  { to: '/admin', label: 'Settings', icon: IconSettings },
]

/* ── Helpers ───────────────────────────────────────────────────────────────── */
const isExactActive = (path) => route.path === path

const currentPageLabel = computed(() => {
  const matched = navItems.find((item) => item.to === route.path)
  return matched?.label ?? 'Dashboard'
})

const handleLogout = () => {
  console.info('[TicketHub] Admin logout clicked — Auth feature pending')
}
</script>

<style scoped>
@reference "@/app.css";
.sidebar-link {
  @apply flex items-center gap-3 px-3 py-2.5 text-sm font-medium text-muted
         rounded-lg transition-all duration-200 ease-out
         hover:text-main hover:bg-white/[0.04];
}

.sidebar-link--active {
  @apply !text-danger !bg-danger-dim
         shadow-[inset_2px_0_0_var(--color-danger)];
}

/* ── Portal Page Transition ────────────────────────────────────────────────── */
.portal-page-enter-active {
  transition: opacity 0.25s ease, transform 0.25s cubic-bezier(0.16, 1, 0.3, 1);
}
.portal-page-leave-active {
  transition: opacity 0.15s ease;
}

.portal-page-enter-from {
  opacity: 0;
  transform: translateY(12px);
}
.portal-page-leave-to {
  opacity: 0;
}
</style>
