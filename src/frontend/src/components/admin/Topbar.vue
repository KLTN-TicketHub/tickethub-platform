<template>
  <header class="h-[72px] bg-surface border-b border-border-main flex items-center justify-between px-6 md:px-10 sticky top-0 z-[50]">
    <!-- Left: Search & Mobile Menu -->
    <div class="flex items-center gap-4 flex-1">
      <button class="md:hidden w-10 h-10 flex items-center justify-center rounded-xl bg-card border border-border-main text-muted hover:text-primary transition-all" @click="toggleSidebar">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16" /></svg>
      </button>

      <div class="hidden sm:flex items-center gap-3 bg-card border border-border-main rounded-xl px-4 w-full max-w-[400px] group focus-within:border-primary transition-all">
        <svg class="w-4 h-4 text-muted group-focus-within:text-primary transition-colors" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
        <input 
          type="text" 
          :placeholder="searchPlaceholder" 
          class="flex-1 bg-transparent border-none py-3 text-[14px] text-main outline-none placeholder:text-muted/50" 
          v-model="localSearch"
          @input="handleInput"
        />
      </div>
    </div>
    
    <!-- Right: Actions & Profile -->
    <div class="flex items-center gap-6">
      <button class="relative w-10 h-10 flex items-center justify-center rounded-xl bg-card border border-border-main text-muted hover:text-primary transition-all group">
        <svg class="w-5 h-5 group-hover:animate-bounce" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/></svg>
        <span class="absolute top-2 right-2 w-2 h-2 bg-primary rounded-full ring-2 ring-surface"></span>
      </button>

      <div class="w-[1px] h-6 bg-border-main hidden sm:block"></div>

      <div class="flex items-center gap-3 cursor-pointer group">
        <div class="flex flex-col text-right hidden sm:flex">
          <span class="text-[14px] font-bold text-main group-hover:text-primary transition-colors">Admin User</span>
          <span class="text-[11px] font-bold text-muted uppercase tracking-wider">Superuser</span>
        </div>
        <div class="w-10 h-10 rounded-xl overflow-hidden border-2 border-border-main group-hover:border-primary transition-all shadow-lg">
          <img src="https://ui-avatars.com/api/?name=Admin+User&background=00C853&color=fff" alt="Admin" class="w-full h-full object-cover" />
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { toggleSidebar, adminSearch } from '../../stores/adminStore'

const route = useRoute()
const localSearch = ref('')
let debounceTimer = null

const searchPlaceholder = computed(() => {
  if (route.path.includes('/events')) return 'Tìm sự kiện...'
  if (route.path.includes('/users')) return 'Tìm người dùng...'
  if (route.path.includes('/orders')) return 'Tìm đơn hàng...'
  return 'Tìm kiếm...'
})

const handleInput = () => {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    adminSearch.value = localSearch.value
  }, 300)
}
</script>

