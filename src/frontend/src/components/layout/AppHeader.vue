<template>
  <header 
    class="fixed top-0 left-0 right-0 z-[1000] transition-all duration-500 ease-[cubic-bezier(0.23,1,0.32,1)] backdrop-blur-2xl"
    :class="[isScrolled ? 'bg-[#0A0F0D]/90 border-b border-white/10 shadow-[0_20px_50px_rgba(0,0,0,0.5)] shadow-[inset_0_1px_0_rgba(255,255,255,0.05)] py-3' : 'bg-transparent py-5']"
  >
    <div class="max-w-[1440px] mx-auto px-6 md:px-10 flex items-center justify-between gap-6">
      
      <!-- Logo & Navigation -->
      <div class="flex items-center gap-10">
        <router-link to="/" class="flex items-center gap-3 group">
          <div class="w-10 h-10 bg-primary text-black rounded-xl flex items-center justify-center font-bold text-xl shadow-[0_0_20px_rgba(0,200,83,0.3)] group-hover:scale-110 transition-transform">
            <PhTicket weight="fill" />
          </div>
          <span class="font-heading font-black text-2xl tracking-tight text-white hidden xl:block uppercase">TicketHub</span>
        </router-link>

        <!-- Main Nav (Desktop) -->
        <nav class="hidden lg:flex items-center gap-1 bg-white/5 border border-white/10 rounded-full p-1">
          <router-link 
            v-for="item in NAV_LINKS" 
            :key="item.label" 
            :to="item.path"
            class="px-4 py-2 rounded-full text-[13px] font-bold text-white/70 hover:text-white hover:bg-white/10 transition-all flex items-center gap-2 group"
            active-class="!text-black !bg-primary"
          >
            <component :is="item.icon" weight="bold" class="text-lg opacity-80 group-hover:opacity-100 transition-opacity" />
            {{ item.label }}
          </router-link>
        </nav>
      </div>

      <!-- Search Bar -->
      <div class="flex-1 max-w-md relative hidden md:block" ref="searchRef">
        <div class="relative group/search">
          <PhMagnifyingGlass class="absolute left-4 top-1/2 -translate-y-1/2 text-white/40 text-lg transition-colors group-focus-within/search:text-primary" weight="bold" />
          <input
            type="text"
            placeholder="Tìm sự kiện, nghệ sĩ..."
            v-model="searchQuery"
            @input="handleSearch"
            @focus="handleSearch(); showSearch = true"
            @keyup.enter="handleSearchEnter"
            class="w-full bg-[#111916]/80 backdrop-blur-md border border-white/10 rounded-full py-2.5 pl-12 pr-6 text-[13px] text-white font-medium outline-none focus:border-primary/50 focus:bg-white/10 transition-all placeholder:text-white/40"
          />
        </div>
        
        <!-- Search Dropdown -->
        <Transition
          enter-active-class="transition duration-300 ease-out"
          enter-from-class="opacity-0 translate-y-4"
          enter-to-class="opacity-100 translate-y-0"
        >
          <div 
            v-if="showSearch && (searchResults.length > 0 || searchQuery)" 
            class="absolute top-[calc(100%+12px)] left-0 right-0 bg-[#0A0F0D]/98 backdrop-blur-3xl border border-white/10 rounded-2xl shadow-[0_40px_80px_rgba(0,0,0,0.8)] overflow-hidden z-[1001]"
          >
            <div v-if="searchResults.length > 0" class="p-2 space-y-1">
              <div 
                v-for="e in searchResults" 
                :key="e.id" 
                class="flex items-center gap-4 p-2 rounded-xl cursor-pointer hover:bg-white/5 transition-all group/item"
                @click="goToEvent(e)"
              >
                <div class="w-12 h-12 rounded-lg overflow-hidden flex-shrink-0 border border-white/10 group-hover/item:border-primary/30 transition-colors">
                  <img :src="e.image" :alt="e.title" class="w-full h-full object-cover group-hover/item:scale-110 transition-transform" />
                </div>
                <div class="flex-1 min-w-0">
                  <div class="text-[14px] font-bold text-white group-hover/item:text-primary transition-colors truncate">{{ e.title }}</div>
                  <div class="text-[12px] text-muted truncate mt-0.5">{{ formatDate(e.dateStart) }} • {{ e.location?.name }}</div>
                </div>
              </div>
            </div>
            <div v-else class="p-8 text-center text-muted italic text-[13px]">Không tìm thấy kết quả cho "{{ searchQuery }}"</div>
          </div>
        </Transition>
      </div>

      <!-- Actions -->
      <div class="flex items-center gap-3">
        
        <div class="relative hidden lg:block">
          <button 
            @click="showCitySelector = !showCitySelector"
            class="flex items-center gap-2 px-4 py-2.5 rounded-full bg-white/5 border border-white/10 text-white/70 hover:text-white hover:border-primary/50 transition-all text-[13px] font-bold"
          >
            <PhMapPin weight="fill" class="text-primary" /> {{ selectedCity }}
            <PhCaretDown weight="bold" class="text-[10px] opacity-60 transition-transform" :class="{ 'rotate-180': showCitySelector }" />
          </button>
          
          <Transition
            enter-active-class="transition duration-200 ease-out"
            enter-from-class="opacity-0 translate-y-2"
            enter-to-class="opacity-100 translate-y-0"
          >
            <div v-if="showCitySelector" class="absolute top-[calc(100%+8px)] right-0 bg-[#0A0F0D]/95 backdrop-blur-3xl border border-white/10 rounded-xl p-1.5 min-w-[160px] shadow-2xl z-[1002]">
              <button 
                v-for="city in CITIES" 
                :key="city"
                @click="selectedCity = city; showCitySelector = false"
                class="w-full text-left px-3 py-2.5 rounded-lg text-[13px] font-bold transition-all flex items-center justify-between"
                :class="[selectedCity === city ? 'bg-primary text-black' : 'text-white/60 hover:bg-white/5 hover:text-white']"
              >
                {{ city }}
                <PhCheck v-if="selectedCity === city" weight="bold" />
              </button>
            </div>
          </Transition>
        </div>

        <NotificationBell v-if="store.user" />

        <button
          @click="router.push('/my-tickets')"
          class="relative w-10 h-10 flex items-center justify-center rounded-full bg-white/5 border border-white/10 text-white hover:bg-white/10 hover:border-primary/40 transition-all group active:scale-95"
        >
          <PhTicket class="text-lg group-hover:scale-110 transition-transform" weight="fill" />
          <span v-if="store.tickets.length > 0" class="absolute -top-1 -right-1 w-4 h-4 bg-primary text-black text-[10px] font-black rounded-full flex items-center justify-center border-2 border-[#0A0F0D]">
            {{ store.tickets.length }}
          </span>
        </button>

        <div v-if="store.user" class="relative">
          <button 
            @click.stop="showDropdown = !showDropdown"
            class="flex items-center gap-2 p-1 pr-3 rounded-full bg-white/5 border border-white/10 hover:bg-white/10 hover:border-primary/40 transition-all group active:scale-95"
          >
            <div class="w-8 h-8 rounded-full bg-gradient-to-tr from-primary to-[#B9FF6A] text-black font-black text-sm flex items-center justify-center">
              {{ store.user.initial }}
            </div>
            <span class="text-[13px] font-bold text-white hidden sm:block">{{ store.user.name }}</span>
          </button>
          <Transition
            enter-active-class="transition duration-300 ease-out"
            enter-from-class="opacity-0 translate-y-4"
            enter-to-class="opacity-100 translate-y-0"
          >
            <div v-if="showDropdown" class="absolute top-[calc(100%+12px)] right-0 bg-[#0A0F0D]/95 backdrop-blur-3xl border border-white/10 rounded-2xl min-w-[220px] p-2 shadow-2xl z-[1001]">
              <button 
                v-for="item in dropdownItems" 
                :key="item.label" 
                @click="router.push(item.path); showDropdown = false"
                class="w-full flex items-center gap-3 px-4 py-2.5 rounded-xl text-[13px] text-white/70 font-bold hover:bg-white/5 hover:text-white transition-all group/dd"
              >
                <component :is="item.icon" weight="bold" class="text-lg opacity-50 group-hover/dd:opacity-100 group-hover/dd:text-primary transition-colors" /> 
                {{ item.label }}
              </button>
              <div class="h-px bg-white/10 my-1"></div>
              <button @click="handleLogout" class="w-full flex items-center gap-3 px-4 py-2.5 rounded-xl text-[13px] text-danger/80 font-bold hover:bg-danger/10 transition-all">
                <PhSignOut weight="bold" class="text-lg" /> Đăng xuất
              </button>
            </div>
          </Transition>
        </div>
        <div v-else class="flex items-center gap-2">
          <button class="px-5 py-2.5 rounded-full text-[13px] font-bold text-white hover:bg-white/10 transition-all active:scale-95" @click="openAuth('login')">
            Đăng nhập
          </button>
          <button class="px-5 py-2.5 rounded-full text-[13px] font-bold text-black bg-primary hover:bg-primary-dark transition-all active:scale-95 shadow-[0_0_20px_rgba(0,200,83,0.2)]" @click="openAuth('register')">
            Đăng ký
          </button>
        </div>
      </div>
    </div>
  </header>
  
  <div class="h-24 md:h-28"></div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { store, selectEvent, openAuth, logout } from '../../stores/eventStore'
import { getEvents } from '../../stores/eventStore'
import { logout as authLogout } from '../../services/auth/auth.service'
import { resetNotifications } from '../../stores/notificationStore'
import NotificationBell from './NotificationBell.vue'
import { 
  PhTicket, PhMagnifyingGlass, PhMapPin, PhCheck, PhCaretDown, 
  PhSignOut, PhHeart, PhUser, PhMicrophoneStage, PhMaskHappy, 
  PhTrophy, PhBooks, PhPlus 
} from '@phosphor-icons/vue'

const router = useRouter()

const showDropdown = ref(false)
const showCitySelector = ref(false)
const selectedCity = ref('Toàn quốc')
const searchQuery = ref('')
const searchResults = ref([])
const showSearch = ref(false)
const searchRef = ref(null)
const isScrolled = ref(false)

const CITIES = ['Toàn quốc', 'Hồ Chí Minh', 'Hà Nội', 'Đà Nẵng', 'Đà Lạt']

const NAV_LINKS = [
  { icon: PhMicrophoneStage, label: 'Concerts', path: '/concerts' },
  { icon: PhMaskHappy, label: 'Sân khấu', path: '/arts' },
  { icon: PhTrophy, label: 'Thể thao', path: '/sports' },
  { icon: PhBooks, label: 'Workshop', path: '/workshops' },
]

const dropdownItems = [
  { icon: PhTicket, label: 'Vé của tôi', path: '/my-tickets' },
  { icon: PhHeart, label: 'Yêu thích', path: '/wishlist' },
  { icon: PhUser, label: 'Hồ sơ', path: '/profile' },
  { icon: PhPlus, label: 'Tạo sự kiện', path: '/organizer' }
]

const goToEvent = (event) => {
  selectEvent(event)
  showSearch.value = false
  searchQuery.value = ''
  router.push('/event/' + event.id)
}

const handleLogout = async () => {
  await authLogout()
  logout()
  await resetNotifications()
  showDropdown.value = false
}

const handleSearch = () => {
  const q = searchQuery.value.toLowerCase().trim()
  if (!q) { 
    searchResults.value = []; 
    showSearch.value = false; 
    return 
  }
  
  const allEvents = getEvents()
  searchResults.value = allEvents.filter(e =>
    e.title.toLowerCase().includes(q) ||
    (e.performers?.some(p => p.name.toLowerCase().includes(q))) ||
    (e.location?.name?.toLowerCase().includes(q))
  ).slice(0, 6)
  showSearch.value = true
}

const handleSearchEnter = () => {
  if (searchQuery.value.trim()) {
    showSearch.value = false
    router.push({ path: '/search', query: { Search: searchQuery.value.trim() } })
  }
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  try {
    const d = new Date(dateStr)
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })
  } catch(e) { return dateStr }
}

const handleClickOutside = (e) => {
  if (!searchRef.value?.contains(e.target)) showSearch.value = false
  if (!e.target.closest('button')) {
    showDropdown.value = false
    showCitySelector.value = false
  }
}

const handleScroll = () => {
  isScrolled.value = window.scrollY > 20
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  window.addEventListener('scroll', handleScroll, { passive: true })
  handleScroll()
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  window.removeEventListener('scroll', handleScroll)
})
</script>
