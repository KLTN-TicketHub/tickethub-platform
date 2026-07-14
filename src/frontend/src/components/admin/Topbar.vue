<template>
  <header class="sticky top-0 z-[100] bg-[#0A0F0D]/90 backdrop-blur-2xl border-b border-white/5 py-4">
    <div class="px-6 md:px-10 flex items-center justify-between gap-6">
      
      <!-- Brand & Nav -->
      <div class="flex items-center gap-10">
        <router-link :to="role === 'admin' ? '/admin/dashboard' : role === 'moderator' ? '/moderator/dashboard' : '/organizer'" class="flex items-center gap-3 group">
          <div class="w-10 h-10 bg-primary text-black rounded-xl flex items-center justify-center font-bold text-xl shadow-[0_0_20px_rgba(0,200,83,0.3)] group-hover:scale-110 transition-transform">
            <PhCrown weight="fill" v-if="role === 'admin'" />
            <PhShieldCheck weight="fill" v-else-if="role === 'moderator'" />
            <PhSuitcase weight="fill" v-else />
          </div>
          <div class="hidden xl:flex flex-col">
            <span class="font-heading font-black text-xl tracking-tight text-white uppercase leading-none">TicketHub</span>
            <span class="text-[10px] font-bold text-primary uppercase tracking-[0.3em] mt-1">{{ role === 'admin' ? 'Admin Space' : role === 'moderator' ? 'Moderator' : 'Partner Space' }}</span>
          </div>
        </router-link>

        <nav class="hidden lg:flex items-center gap-1 bg-white/5 border border-white/10 rounded-full p-1">
          <router-link 
            v-for="item in activeMenuItems" 
            :key="item.path" 
            :to="item.path"
            class="px-4 py-2 rounded-full text-[13px] font-bold transition-all flex items-center gap-2 group"
            :class="[
              isRouteActive(item.path) 
                ? 'text-black bg-primary shadow-[0_0_15px_rgba(0,200,83,0.3)] font-black' 
                : 'text-white/70 hover:text-white hover:bg-white/10'
            ]"
          >
            <component :is="item.icon" weight="bold" class="text-lg opacity-80 group-hover:opacity-100" />
            {{ item.label }}
          </router-link>
        </nav>
      </div>

      <!-- Right Side -->
      <div class="flex items-center gap-4">
        <div class="hidden sm:flex items-center gap-3 bg-[#111916] border border-white/10 rounded-full px-4 py-2 w-64 group focus-within:border-primary/50 focus-within:bg-white/5 transition-all">
          <PhMagnifyingGlass class="text-white/40 group-focus-within:text-primary text-lg" weight="bold" />
          <input 
            type="text" 
            :placeholder="searchPlaceholder" 
            class="flex-1 bg-transparent border-none text-[13px] text-white outline-none placeholder:text-white/30 font-medium" 
            v-model="localSearch"
            @input="handleInput"
          />
        </div>

        <button @click="handleLogout" class="w-10 h-10 flex items-center justify-center rounded-full bg-white/5 border border-white/10 text-white/60 hover:text-danger hover:border-danger/30 hover:bg-danger/10 transition-all group">
          <PhSignOut class="text-lg group-hover:scale-110 transition-transform" weight="bold" />
        </button>

        <!-- Hamburger Menu for Mobile -->
        <button @click="showMobileMenu = !showMobileMenu" class="lg:hidden w-10 h-10 flex items-center justify-center rounded-full bg-white/5 border border-white/10 text-white/60 hover:text-white hover:bg-white/10 transition-all">
          <PhList v-if="!showMobileMenu" class="text-xl" weight="bold" />
          <PhX v-else class="text-xl" weight="bold" />
        </button>
      </div>

    </div>

    <!-- Mobile Menu Overlay -->
    <div v-if="showMobileMenu" class="lg:hidden absolute top-full left-0 right-0 bg-[#0A0F0D]/95 backdrop-blur-3xl border-b border-white/5 py-4 px-6 flex flex-col gap-3 shadow-2xl z-[99]">
      <router-link 
        v-for="item in activeMenuItems" 
        :key="item.path" 
        :to="item.path"
        @click="showMobileMenu = false"
        class="px-4 py-3 rounded-xl text-[14px] font-bold transition-all flex items-center gap-3"
        :class="[
          isRouteActive(item.path) 
            ? 'text-black bg-primary font-black shadow-[0_0_15px_rgba(0,200,83,0.3)]' 
            : 'text-white/70 hover:text-white hover:bg-white/5'
        ]"
      >
        <component :is="item.icon" weight="bold" class="text-lg" />
        {{ item.label }}
      </router-link>
    </div>
  </header>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { adminSearch } from '../../stores/adminStore'
import { logout as authLogout } from '../../services/auth/auth.service'
import { PhCrown, PhSquaresFour, PhTicket, PhUsers, PhShieldCheck, PhReceipt, PhMagnifyingGlass, PhSignOut, PhCheckSquareOffset, PhMapPin, PhList, PhX, PhSuitcase, PhPlus, PhFolderOpen, PhHandCoins, PhWallet } from '@phosphor-icons/vue'

const props = defineProps({
  role: { type: String, default: 'admin' }
})

const route = useRoute()
const router = useRouter()
const localSearch = ref('')
const showMobileMenu = ref(false)
let debounceTimer = null

const adminMenuItems = [
  { label: 'Tổng quan', path: '/admin/dashboard', icon: PhSquaresFour },
  { label: 'Sự kiện', path: '/admin/events', icon: PhTicket },
  { label: 'Danh mục', path: '/admin/event-categories', icon: PhFolderOpen },
  { label: 'Người dùng', path: '/admin/users', icon: PhUsers },
  { label: 'Kiểm duyệt', path: '/admin/moderators', icon: PhShieldCheck },
  { label: 'Đơn hàng', path: '/admin/orders', icon: PhReceipt },
]

const modMenuItems = [
  { label: 'Tổng quan', path: '/moderator/dashboard', icon: PhSquaresFour },
  { label: 'Duyệt sự kiện', path: '/moderator/events', icon: PhCheckSquareOffset },
  { label: 'Địa điểm', path: '/moderator/venues', icon: PhMapPin },
  { label: 'Giải ngân', path: '/moderator/payouts', icon: PhHandCoins },
]

const orgMenuItems = [
  { label: 'Tổng quan', path: '/organizer', icon: PhSquaresFour },
  { label: 'Tạo sự kiện', path: '/organizer/create-event', icon: PhPlus },
  { label: 'Ví của tôi', path: '/organizer/wallet', icon: PhWallet },
]

const activeMenuItems = computed(() => {
  if (props.role === 'admin') return adminMenuItems
  if (props.role === 'moderator') return modMenuItems
  return orgMenuItems
})

const isRouteActive = (path) => {
  if (path.endsWith('/dashboard') || path === '/' || path === '/admin' || path === '/moderator') {
    return route.path === path
  }
  return route.path.startsWith(path)
}

const searchPlaceholder = computed(() => {
  if (route.path.includes('/events')) return 'Tìm sự kiện...'
  if (route.path.includes('/event-categories')) return 'Tìm danh mục...'
  if (route.path.includes('/users')) return 'Tìm người dùng...'
  if (route.path.includes('/orders')) return 'Tìm đơn hàng...'
  return 'Tìm kiếm...'
})

const handleInput = () => {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => { adminSearch.value = localSearch.value }, 300)
}

const handleLogout = async () => {
  await authLogout()
  router.replace(props.role === 'admin' ? '/admin/login' : props.role === 'moderator' ? '/moderator/login' : '/organizer/login')
}
</script>
