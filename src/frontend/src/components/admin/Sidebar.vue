<template>
  <aside 
    class="w-[260px] bg-card border-r border-border-main flex flex-col h-screen sticky top-0 z-[100] transition-transform duration-300 md:translate-x-0"
    :class="isSidebarOpen ? 'fixed translate-x-0' : 'fixed -translate-x-full'"
  >
    <!-- Header -->
    <div class="p-6 px-8 border-b border-border-main flex items-center justify-between">
      <router-link to="/" class="font-heading text-xl font-bold text-main tracking-tight">
        Event<span class="text-primary">Sphere</span> <span class="text-[10px] uppercase tracking-widest text-muted ml-1 opacity-50">Admin</span>
      </router-link>
      <button @click="toggleSidebar" class="md:hidden text-muted hover:text-main">✕</button>
    </div>

    <!-- Nav -->
    <nav class="flex-1 p-4 flex flex-col gap-1.5 overflow-y-auto mt-4">
      <router-link 
        v-for="item in menuItems" 
        :key="item.path"
        :to="item.path" 
        class="flex items-center gap-3 px-4 py-3 rounded-xl font-bold transition-all group"
        active-class="bg-primary/10 text-primary"
        inactive-class="text-muted hover:bg-surface hover:text-main"
        @click="closeSidebarMobile"
      >
        <component :is="item.icon" class="w-5 h-5 opacity-70 group-hover:opacity-100 transition-opacity" />
        <span class="text-[14px]">{{ item.label }}</span>
      </router-link>
    </nav>

    <!-- Footer -->
    <div class="p-4 border-t border-border-main">
      <button 
        type="button"
        @click="handleLogout"
        class="flex items-center gap-3 px-4 py-3 rounded-xl font-bold text-muted hover:bg-danger/10 hover:text-danger transition-all group"
      >
        <svg class="w-5 h-5 opacity-70 group-hover:opacity-100" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
        </svg>
        <span class="text-[14px]">Thoát Admin</span>
      </button>
    </div>
  </aside>
</template>

<script setup>
import { h } from 'vue'
import { useRouter } from 'vue-router'
import { isSidebarOpen, toggleSidebar } from '../../stores/adminStore'
import { logout as authLogout } from '../../services/auth/auth.service'

const router = useRouter()

const menuItems = [
  { 
    label: 'Bảng điều khiển', 
    path: '/admin/dashboard', 
    icon: () => h('svg', { fill: 'none', stroke: 'currentColor', 'stroke-width': '2', viewBox: '0 0 24 24' }, [
      h('rect', { x: '3', y: '3', width: '7', height: '9', rx: '1' }),
      h('rect', { x: '14', y: '3', width: '7', height: '5', rx: '1' }),
      h('rect', { x: '14', y: '12', width: '7', height: '9', rx: '1' }),
      h('rect', { x: '3', y: '16', width: '7', height: '5', rx: '1' }),
    ])
  },
  { 
    label: 'Sự kiện', 
    path: '/admin/events', 
    icon: () => h('svg', { fill: 'none', stroke: 'currentColor', 'stroke-width': '2', viewBox: '0 0 24 24' }, [
      h('rect', { x: '3', y: '4', width: '18', height: '18', rx: '2', ry: '2' }),
      h('line', { x1: '16', y1: '2', x2: '16', y2: '6' }),
      h('line', { x1: '8', y1: '2', x2: '8', y2: '6' }),
      h('line', { x1: '3', y1: '10', x2: '21', y2: '10' }),
    ])
  },
  { 
    label: 'Người dùng', 
    path: '/admin/users', 
    icon: () => h('svg', { fill: 'none', stroke: 'currentColor', 'stroke-width': '2', viewBox: '0 0 24 24' }, [
      h('path', { d: 'M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2' }),
      h('circle', { cx: '9', cy: '7', r: '4' }),
      h('path', { d: 'M23 21v-2a4 4 0 0 0-3-3.87' }),
      h('path', { d: 'M16 3.13a4 4 0 0 1 0 7.75' }),
    ])
  },
  { 
    label: 'Danh mục', 
    path: '/admin/event-categories', 
    icon: () => h('svg', { fill: 'none', stroke: 'currentColor', 'stroke-width': '2', viewBox: '0 0 24 24' }, [
      h('path', { d: 'M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10' })
    ])
  },
  { 
    label: 'Kiểm duyệt viên', 
    path: '/admin/moderators', 
    icon: () => h('svg', { fill: 'none', stroke: 'currentColor', 'stroke-width': '2', viewBox: '0 0 24 24' }, [
      h('path', { d: 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.953 11.953 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z' })
    ])
  },
  { 
    label: 'Đơn hàng', 
    path: '/admin/orders', 
    icon: () => h('svg', { fill: 'none', stroke: 'currentColor', 'stroke-width': '2', viewBox: '0 0 24 24' }, [
      h('circle', { cx: '9', cy: '21', r: '1' }),
      h('circle', { cx: '20', cy: '21', r: '1' }),
      h('path', { d: 'M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6' }),
    ])
  },
]

const closeSidebarMobile = () => {
  if (window.innerWidth <= 768 && isSidebarOpen.value) {
    toggleSidebar()
  }
}

const handleLogout = async () => {
  await authLogout()
  if (router.currentRoute.value.path.startsWith('/admin')) {
    router.replace('/admin/login')
  }
}
</script>

