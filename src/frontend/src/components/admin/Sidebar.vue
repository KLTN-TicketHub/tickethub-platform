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
      <router-link 
        to="/" 
        class="flex items-center gap-3 px-4 py-3 rounded-xl font-bold text-muted hover:bg-danger/10 hover:text-danger transition-all group"
      >
        <svg class="w-5 h-5 opacity-70 group-hover:opacity-100" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
        </svg>
        <span class="text-[14px]">Thoát Admin</span>
      </router-link>
    </div>
  </aside>
</template>

<script setup>
import { h } from 'vue'
import { isSidebarOpen, toggleSidebar } from '../../stores/adminStore'

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
</script>

