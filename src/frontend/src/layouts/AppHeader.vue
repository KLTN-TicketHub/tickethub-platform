<template>
  <header 
    class="fixed top-0 left-0 right-0 z-[1000] transition-all duration-500 ease-[cubic-bezier(0.23,1,0.32,1)] bg-[#0A0F0D]/80 backdrop-blur-3xl border-b border-white/5"
    :class="[isScrolled ? 'py-2 shadow-[0_20px_50px_rgba(0,0,0,0.5)]' : 'py-4']"
  >
    <div class="max-w-[1440px] mx-auto px-6 md:px-10 space-y-4">
      <!-- TOP ROW: Logo, Search, Actions -->
      <div class="flex items-center justify-between gap-8 md:gap-12">
        <!-- Logo & City Selector -->
        <div class="flex items-center gap-6 flex-shrink-0">
          <router-link to="/" class="flex items-center gap-3 group">
            <div class="w-10 h-10 bg-primary text-black rounded-xl flex items-center justify-center font-bold text-xl shadow-[0_0_20px_rgba(0,200,83,0.3)] group-hover:scale-110 transition-all">
              ◉
            </div>
            <span class="font-heading font-black text-xl tracking-tighter text-white hidden sm:block uppercase">TicketHub</span>
          </router-link>

          <!-- City Selector -->
          <div class="relative hidden lg:block" ref="cityRef">
            <button 
              @click="showCitySelector = !showCitySelector"
              class="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-white/5 border border-white/10 text-white/60 hover:text-white hover:border-primary/50 transition-all text-[12px] font-bold"
            >
              📍 {{ selectedCity }}
              <span class="text-[8px] opacity-40 transition-transform" :class="{ 'rotate-180': showCitySelector }">▼</span>
            </button>
            
            <Transition
              enter-active-class="transition duration-200 ease-out"
              enter-from-class="opacity-0 translate-y-2"
              enter-to-class="opacity-100 translate-y-0"
            >
              <div v-if="showCitySelector" class="absolute top-[calc(100%+8px)] left-0 bg-[#0A0F0D]/95 backdrop-blur-3xl border border-white/10 rounded-xl p-1.5 min-w-[140px] shadow-2xl z-[1002]">
                <button 
                  v-for="city in CITIES" 
                  :key="city"
                  @click="selectedCity = city; showCitySelector = false"
                  class="w-full text-left px-3 py-2 rounded-lg text-[12px] font-bold transition-all"
                  :class="[selectedCity === city ? 'bg-primary text-black' : 'text-white/60 hover:bg-white/5 hover:text-white']"
                >
                  {{ city }}
                </button>
              </div>
            </Transition>
          </div>
        </div>

        <!-- Premium Search Bar -->
        <div class="flex-1 max-w-2xl relative" ref="searchRef">
          <div class="relative group/search">
            <span class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 text-lg pointer-events-none transition-all group-focus-within/search:text-primary">🔍</span>
            <input
              type="text"
              placeholder="Tìm sự kiện, nghệ sĩ, địa điểm..."
              v-model="searchQuery"
              @input="handleSearch"
              @focus="handleSearch(); showSearch = true"
              class="w-full bg-white/5 border border-white/10 rounded-xl py-3 pl-12 pr-6 text-[14px] text-white font-medium outline-none focus:border-primary/40 focus:bg-white/10 transition-all placeholder:text-white/20"
            />
          </div>
          
          <!-- Search Dropdown (Restyled for 2nd row) -->
          <Transition
            enter-active-class="transition duration-300 ease-out"
            enter-from-class="opacity-0 translate-y-4"
            enter-to-class="opacity-100 translate-y-0"
          >
            <div 
              v-if="showSearch && (searchResults.length > 0 || searchQuery)" 
              class="absolute top-[calc(100%+12px)] left-0 right-0 bg-[#0A0F0D]/98 backdrop-blur-3xl border border-white/10 rounded-2xl shadow-[0_40px_80px_rgba(0,0,0,0.8)] overflow-hidden z-[1001]"
            >
              <div v-if="searchResults.length > 0" class="p-3 space-y-1">
                <div 
                  v-for="e in searchResults" 
                  :key="e.id" 
                  class="flex items-center gap-4 p-3 rounded-xl cursor-pointer hover:bg-white/5 transition-all group/item"
                  @click="goToEvent(e)"
                >
                  <div class="w-12 h-12 rounded-lg overflow-hidden flex-shrink-0">
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
        <div class="flex items-center gap-3 flex-shrink-0">
          <!-- Create Event CTA -->
          <BaseButton 
            variant="ghost" 
            size="sm" 
            class="!rounded-xl !border-dashed !border-white/10 !px-4 hidden xl:flex hover:!border-primary/50 text-[13px]"
            @click="router.push('/organizer')"
          >
            ➕ Tạo sự kiện
          </BaseButton>

          <!-- Ticket Counter -->
          <button 
            @click="router.push('/my-tickets')"
            class="relative w-10 h-10 flex items-center justify-center rounded-xl bg-white/5 border border-white/10 text-white hover:bg-white/10 transition-all group"
          >
            <span class="text-lg group-hover:scale-110 transition-transform">🎟️</span>
            <span v-if="store.tickets.length > 0" class="absolute -top-1.5 -right-1.5 w-5 h-5 bg-primary text-black text-[10px] font-black rounded-full flex items-center justify-center border-2 border-[#0A0F0D]">
              {{ store.tickets.length }}
            </span>
          </button>

          <!-- User Profile / Auth -->
          <div v-if="store.user" class="relative" ref="dropdownRef">
            <button 
              @click.stop="showDropdown = !showDropdown"
              class="flex items-center gap-2 p-1 rounded-xl bg-white/5 border border-white/10 hover:bg-white/10 transition-all group"
            >
              <div class="w-8 h-8 rounded-lg bg-primary text-black font-black text-sm flex items-center justify-center">
                {{ store.user.initial }}
              </div>
              <span class="text-[13px] font-bold text-white pr-2 hidden md:block">{{ store.user.name }}</span>
            </button>
            <!-- Dropdown content same as before -->
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
                  class="w-full flex items-center gap-3 px-4 py-2.5 rounded-xl text-[13px] text-white/70 font-bold hover:bg-white/5 hover:text-primary transition-all"
                >
                  <span class="text-lg opacity-50">{{ item.icon }}</span> {{ item.label }}
                </button>
                <div class="h-px bg-white/5 my-1"></div>
                <button @click="handleLogout" class="w-full flex items-center gap-3 px-4 py-2.5 rounded-xl text-[13px] text-danger/80 font-bold hover:bg-danger/10 transition-all">
                  🚪 Đăng xuất
                </button>
              </div>
            </Transition>
          </div>
          <div v-else class="flex items-center gap-2">
            <BaseButton variant="ghost" size="sm" class="!px-4" @click="openAuth('login')">Đăng nhập</BaseButton>
            <BaseButton variant="primary" size="sm" class="!px-5" @click="openAuth('register')">Đăng ký</BaseButton>
          </div>
        </div>
      </div>

      <!-- BOTTOM ROW: Navigation -->
      <nav class="flex items-center gap-1 transition-all duration-500 overflow-hidden" :class="[isScrolled ? 'h-0 opacity-0' : 'h-10 opacity-100']">
        <router-link 
          v-for="item in NAV_LINKS" 
          :key="item.label" 
          :to="item.path"
          class="px-5 py-2 rounded-lg text-[13px] font-bold text-muted hover:text-white hover:bg-white/5 transition-all flex items-center gap-3 group"
          active-class="!text-primary bg-primary/5"
        >
          <span class="text-lg opacity-60 group-hover:opacity-100 group-hover:scale-110 transition-all">{{ item.icon }}</span>
          {{ item.label }}
        </router-link>
      </nav>
    </div>
  </header>
  
  <!-- Content Spacer -->
  <div :class="[isScrolled ? 'h-24' : 'h-36']" class="transition-all duration-500"></div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { store, selectEvent, openAuth, logout } from '../features/events/store'
import BaseButton from '../shared/components/BaseButton.vue'

// ─── Composables ────────────────────────────────────────────────────────────
import { useScroll } from '../shared/composables/useScroll'
import { useClickOutside } from '../shared/composables/useClickOutside'
import { useEventSearch } from '../features/events/composables/useEventSearch'

const router = useRouter()

// ─── Scroll behaviour ──────────────────────────────────────────────────────
const { isScrolled } = useScroll({ threshold: 20 })

// ─── Search (feature composable) ───────────────────────────────────────────
const { searchQuery, searchResults, showSearch, handleSearch, clearSearch } = useEventSearch()

// ─── Template refs for click-outside targets ───────────────────────────────
const searchRef = ref(null)
const cityRef = ref(null)
const dropdownRef = ref(null)

// ─── Click-outside handlers ────────────────────────────────────────────────
useClickOutside(searchRef, () => {
  showSearch.value = false
})

const showCitySelector = ref(false)
useClickOutside(cityRef, () => {
  showCitySelector.value = false
})

const showDropdown = ref(false)
useClickOutside(dropdownRef, () => {
  showDropdown.value = false
})

// ─── Local UI state ────────────────────────────────────────────────────────
const selectedCity = ref('Toàn quốc')

const CITIES = ['Toàn quốc', 'Hồ Chí Minh', 'Hà Nội', 'Đà Nẵng', 'Đà Lạt']

const NAV_LINKS = [
  { icon: '🎤', label: 'Concerts', path: '/concerts' },
  { icon: '🎭', label: 'Nghệ thuật & Sân khấu', path: '/arts' },
  { icon: '🏆', label: 'Thể thao', path: '/sports' },
  { icon: '📚', label: 'Workshop & Khóa học', path: '/workshops' },
]

const dropdownItems = [
  { icon: '🎟️', label: 'Vé của tôi', path: '/my-tickets' },
  { icon: '❤️', label: 'Yêu thích', path: '/wishlist' },
  { icon: '👤', label: 'Hồ sơ', path: '/profile' },
]

// ─── Actions ───────────────────────────────────────────────────────────────
const goToEvent = (event) => {
  selectEvent(event)
  clearSearch()
  router.push('/event/' + event.id)
}

const handleLogout = () => {
  logout()
  showDropdown.value = false
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  try {
    const d = new Date(dateStr)
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })
  } catch(e) { return dateStr }
}
</script>

<style scoped>
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
