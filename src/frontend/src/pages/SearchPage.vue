<template>
  <div class="min-h-screen pt-6 pb-20">
    <div class="max-w-[1440px] mx-auto px-6 md:px-10">
      
      <!-- Header Row: Title & Filters (Title on Left, Filters & Search on Right) -->
      <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-10 pb-6 border-b border-white/5">
        
        <!-- Left: Title -->
        <h1 class="text-white/50 font-bold text-xs tracking-wide flex items-center gap-1.5">
          Kết quả tìm kiếm: 
          <span v-if="filters.Search" class="text-white text-base font-black font-heading">"{{ filters.Search }}"</span>
          <span v-else class="text-white text-base font-black font-heading">Tất cả sự kiện</span>
        </h1>

        <!-- Right: Search & Filters Group -->
        <div class="flex flex-wrap items-center gap-3">
          
          <!-- Search Input -->
          <div class="relative">
            <input 
              type="text" 
              v-model="searchInput"
              placeholder="Tìm kiếm trong kết quả..." 
              class="w-44 lg:w-56 bg-surface border border-white/5 hover:border-white/20 rounded-full py-2.5 pl-4 pr-10 text-xs text-white focus:border-primary focus:bg-card transition-all outline-none"
              @keyup.enter="applySearch"
            />
            <button @click="applySearch" class="absolute right-3 top-1/2 -translate-y-1/2 text-white/50 hover:text-white transition-colors cursor-pointer">
              <PhMagnifyingGlass class="text-xs" weight="bold" />
            </button>
          </div>

          <!-- Date Filter (Tất cả các ngày) -->
          <div class="relative" ref="dateFilterRef">
            <button 
              @click="showDateFilter = !showDateFilter"
              class="flex items-center gap-2 px-4 py-2.5 bg-surface hover:bg-card border border-white/5 hover:border-white/10 rounded-full text-white/90 text-xs font-bold transition-all cursor-pointer"
            >
              <PhCalendarBlank weight="bold" /> 
              {{ dateLabel }}
              <PhCaretDown weight="bold" class="text-xs ml-1" />
            </button>

            <!-- Dropdown Date -->
            <Transition
              enter-active-class="transition duration-200 ease-out"
              enter-from-class="opacity-0 translate-y-2"
              enter-to-class="opacity-100 translate-y-0"
              leave-active-class="transition duration-150 ease-in"
              leave-from-class="opacity-100 translate-y-0"
              leave-to-class="opacity-0 translate-y-2"
            >
              <div v-if="showDateFilter" class="absolute right-0 top-full mt-2 w-72 bg-card border border-border-main rounded-xl p-4 shadow-2xl z-50">
                <div class="space-y-4">
                  <div class="space-y-1">
                    <label class="text-[11px] text-white/50 uppercase font-bold tracking-wider">Từ ngày</label>
                    <input type="date" v-model="tempDate.FromDate" class="w-full bg-card-2 border border-white/5 rounded-lg px-3 py-2 text-white text-sm outline-none focus:border-primary focus:bg-surface transition-colors" />
                  </div>
                  <div class="space-y-1">
                    <label class="text-[11px] text-white/50 uppercase font-bold tracking-wider">Đến ngày</label>
                    <input type="date" v-model="tempDate.ToDate" class="w-full bg-card-2 border border-white/5 rounded-lg px-3 py-2 text-white text-sm outline-none focus:border-primary focus:bg-surface transition-colors" />
                  </div>
                  <div class="flex gap-2 pt-2">
                    <button @click="clearDates" class="flex-1 py-2 rounded-lg text-white/70 hover:text-white bg-card-2 hover:bg-surface text-sm font-bold transition-colors">Xóa</button>
                    <button @click="applyDates" class="flex-1 py-2 rounded-lg bg-primary text-black text-sm font-bold hover:brightness-110 transition-colors">Áp dụng</button>
                  </div>
                </div>
              </div>
            </Transition>
          </div>

          <!-- General Filter (Bộ lọc) -->
          <div class="relative" ref="generalFilterRef">
            <button 
              @click="showGeneralFilter = !showGeneralFilter"
              class="flex items-center gap-2 px-4 py-2.5 bg-primary hover:brightness-115 rounded-full text-black text-xs font-bold transition-all cursor-pointer shadow-glow"
            >
              <PhFunnel weight="fill" /> 
              Bộ lọc
              <PhCaretDown weight="bold" class="text-[10px] ml-1" />
            </button>

            <!-- Dropdown Filter -->
            <Transition
              enter-active-class="transition duration-200 ease-out"
              enter-from-class="opacity-0 translate-y-2"
              enter-to-class="opacity-100 translate-y-0"
              leave-active-class="transition duration-150 ease-in"
              leave-from-class="opacity-100 translate-y-0"
              leave-to-class="opacity-0 translate-y-2"
            >
              <div v-if="showGeneralFilter" class="absolute right-0 top-full mt-2 w-64 bg-card border border-border-main rounded-xl p-4 shadow-2xl z-50">
                <div class="space-y-4">
                  <!-- Category -->
                  <div class="space-y-1">
                    <label class="text-[11px] text-white/50 uppercase font-bold tracking-wider">Danh mục</label>
                    <select v-model="tempFilter.CategoryId" class="w-full bg-card-2 border border-white/5 rounded-lg px-3 py-2 text-white text-sm outline-none focus:border-primary focus:bg-surface transition-colors">
                      <option value="" class="bg-card text-white">Tất cả danh mục</option>
                      <option v-for="cat in categories" :key="cat.id" :value="cat.id" class="bg-card text-white">{{ cat.categoryName }}</option>
                    </select>
                  </div>
                  <!-- Location -->
                  <div class="space-y-1">
                    <label class="text-[11px] text-white/50 uppercase font-bold tracking-wider">Địa điểm</label>
                    <select v-model="tempFilter.ProvinceCity" class="w-full bg-card-2 border border-white/5 rounded-lg px-3 py-2 text-white text-sm outline-none focus:border-primary focus:bg-surface transition-colors">
                      <option value="" class="bg-card text-white">Toàn quốc</option>
                      <option v-for="city in availableCities" :key="city.code" :value="city.name" class="bg-card text-white">{{ city.name }}</option>
                    </select>
                  </div>
                  <div class="flex gap-2 pt-2">
                    <button @click="clearGeneral" class="flex-1 py-2 rounded-lg text-white/70 hover:text-white bg-card-2 hover:bg-surface text-sm font-bold transition-colors">Xóa</button>
                    <button @click="applyGeneral" class="flex-1 py-2 rounded-lg bg-primary text-black text-sm font-bold hover:brightness-110 transition-colors">Áp dụng</button>
                  </div>
                </div>
              </div>
            </Transition>
          </div>

          <!-- Active Category Tag -->
          <div v-if="activeCategoryName" class="flex items-center gap-2 px-4 py-2.5 bg-primary/10 text-primary border border-primary/20 rounded-full text-xs font-bold transition-all">
            <PhXCircle weight="fill" class="cursor-pointer text-sm hover:text-white transition-colors" @click="clearCategory" />
            {{ activeCategoryName }}
          </div>

        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="py-20 flex flex-col items-center justify-center text-white/50">
        <div class="w-10 h-10 border-4 border-primary/20 border-t-primary rounded-full animate-spin mb-4"></div>
        <p class="font-bold text-sm animate-pulse">Đang tìm kiếm...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="events.length === 0" class="py-20 flex flex-col items-center justify-center text-center">
        <PhMagnifyingGlass class="text-6xl text-white/20 mb-4" weight="duotone" />
        <h3 class="text-xl font-bold text-white mb-2">Không tìm thấy sự kiện</h3>
        <p class="text-sm text-white/50 max-w-sm">Không có sự kiện nào khớp với tiêu chí tìm kiếm của bạn. Hãy thử thay đổi bộ lọc.</p>
      </div>

      <!-- Results Grid -->
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-x-6 gap-y-10">
        <SearchEventCard 
          v-for="event in events" 
          :key="event.id" 
          :event="event" 
        />
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1 && !isLoading" class="mt-16 flex justify-center">
        <div class="flex items-center gap-2 bg-surface rounded-full p-2 border border-white/5">
          <button 
            :disabled="!hasPreviousPage"
            @click="goToPage(pageNumber - 1)"
            class="w-10 h-10 rounded-full flex items-center justify-center text-white hover:bg-white/10 transition-colors disabled:opacity-30 disabled:pointer-events-none"
          >
            <PhCaretLeft weight="bold" />
          </button>
          
          <div class="flex items-center gap-1 px-2">
            <button 
              v-for="p in totalPages" 
              :key="p"
              @click="goToPage(p)"
              class="w-10 h-10 rounded-full text-sm font-bold transition-all flex items-center justify-center"
              :class="[p === pageNumber ? 'bg-primary text-black' : 'text-white hover:bg-white/10']"
            >
              {{ p }}
            </button>
          </div>

          <button 
            :disabled="!hasNextPage"
            @click="goToPage(pageNumber + 1)"
            class="w-10 h-10 rounded-full flex items-center justify-center text-white hover:bg-white/10 transition-colors disabled:opacity-30 disabled:pointer-events-none"
          >
            <PhCaretRight weight="bold" />
          </button>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getPublicEvents, getPublicEventCategories } from '../services/eventService'
import { getProvinces } from '../services/location.service'
import { store } from '../stores/eventStore'
import { getErrorMessage } from '../utils/apiError'
import SearchEventCard from '../components/SearchEventCard.vue'
import { PhMagnifyingGlass, PhCalendarBlank, PhCaretDown, PhFunnel, PhXCircle, PhCaretLeft, PhCaretRight } from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

// UI State
const isLoading = ref(true)
const showDateFilter = ref(false)
const showGeneralFilter = ref(false)
const searchInput = ref('')
const dateFilterRef = ref(null)
const generalFilterRef = ref(null)

// Data State
const events = ref([])
const categories = ref([])
const availableCities = ref([])
const totalCount = ref(0)
const pageNumber = ref(1)
const totalPages = ref(1)
const hasPreviousPage = ref(false)
const hasNextPage = ref(false)

// Filter State (applied to URL)
const filters = ref({
  Search: '',
  CategoryId: '',
  ProvinceCity: '',
  FromDate: '',
  ToDate: ''
})

// Temp State (for dropdowns before applying)
const tempDate = ref({ FromDate: '', ToDate: '' })
const tempFilter = ref({ CategoryId: '', ProvinceCity: '' })

const dateLabel = computed(() => {
  if (filters.value.FromDate || filters.value.ToDate) {
    return 'Ngày tùy chỉnh'
  }
  return 'Tất cả các ngày'
})

const activeCategoryName = computed(() => {
  if (!filters.value.CategoryId || categories.value.length === 0) return ''
  const cat = categories.value.find(c => c.id === filters.value.CategoryId)
  return cat ? cat.categoryName : 'Danh mục đã chọn'
})

// Click outside handler
const handleClickOutside = (e) => {
  if (dateFilterRef.value && !dateFilterRef.value.contains(e.target)) {
    showDateFilter.value = false
  }
  if (generalFilterRef.value && !generalFilterRef.value.contains(e.target)) {
    showGeneralFilter.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  initFiltersFromQuery()
  fetchCities()
  fetchCategories()
  loadEvents()
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

const initFiltersFromQuery = () => {
  filters.value.Search = route.query.Search || ''
  filters.value.CategoryId = route.query.CategoryId || ''
  filters.value.ProvinceCity = route.query.ProvinceCity || ''
  filters.value.FromDate = route.query.FromDate || ''
  filters.value.ToDate = route.query.ToDate || ''
  
  searchInput.value = filters.value.Search
  tempDate.value.FromDate = filters.value.FromDate
  tempDate.value.ToDate = filters.value.ToDate
  tempFilter.value.CategoryId = filters.value.CategoryId
  tempFilter.value.ProvinceCity = filters.value.ProvinceCity
  
  const page = parseInt(route.query.PageNumber)
  pageNumber.value = !isNaN(page) && page > 0 ? page : 1
}

const fetchCities = async () => {
  try {
    const data = await getProvinces()
    if (data && Array.isArray(data) && data.length > 0) {
      availableCities.value = data
    } else {
      throw new Error('Empty data')
    }
  } catch (error) {
    console.error('Failed to fetch provinces', error)
    // Fallback in case the external API is down (502 Bad Gateway)
    availableCities.value = [
      { code: '01', name: 'Thành phố Hà Nội' },
      { code: '79', name: 'Thành phố Hồ Chí Minh' },
      { code: '48', name: 'Thành phố Đà Nẵng' },
      { code: '92', name: 'Thành phố Cần Thơ' },
      { code: '31', name: 'Thành phố Hải Phòng' },
      { code: '24', name: 'Tỉnh Bắc Giang' }
    ]
  }
}

const fetchCategories = async () => {
  try {
    const res = await getPublicEventCategories()
    // handle PaginatedResult wrapper if present
    if (res && res.data && res.data.data) {
      categories.value = res.data.data
    } else if (res && res.data) {
      categories.value = res.data
    } else if (Array.isArray(res)) {
      categories.value = res
    }
  } catch (error) {
    console.error('Failed to fetch categories', error)
    store.toast = { message: getErrorMessage(error, 'Không thể tải danh mục sự kiện.'), icon: '❌' }
  }
}

const loadEvents = async () => {
  isLoading.value = true
  try {
    const params = {
      PageNumber: pageNumber.value,
      PageSize: 12
    }
    
    if (filters.value.Search) params.Search = filters.value.Search
    if (filters.value.CategoryId) params.CategoryId = filters.value.CategoryId
    if (filters.value.ProvinceCity) params.ProvinceCity = filters.value.ProvinceCity
    if (filters.value.FromDate) params.FromDate = filters.value.FromDate
    if (filters.value.ToDate) params.ToDate = filters.value.ToDate

    const response = await getPublicEvents(params)
    
    if (response && response.data && Array.isArray(response.data.data)) {
      events.value = response.data.data
      totalCount.value = response.data.totalCount || 0
      pageNumber.value = response.data.pageNumber || 1
      totalPages.value = response.data.totalPages || 1
      hasPreviousPage.value = response.data.hasPreviousPage || false
      hasNextPage.value = response.data.hasNextPage || false
    } else if (response && Array.isArray(response.data)) {
      events.value = response.data
      totalCount.value = response.data.length
      totalPages.value = 1
    } else if (Array.isArray(response)) {
      events.value = response
      totalCount.value = response.length
      totalPages.value = 1
    } else {
      events.value = []
      totalCount.value = 0
    }
  } catch (error) {
    console.error('Failed to load events:', error)
    store.toast = { message: getErrorMessage(error, 'Không thể tải danh sách sự kiện.'), icon: '❌' }
    events.value = []
  } finally {
    isLoading.value = false
  }
}

const updateRoute = () => {
  const query = {}
  if (filters.value.Search) query.Search = filters.value.Search
  if (filters.value.CategoryId) query.CategoryId = filters.value.CategoryId
  if (filters.value.ProvinceCity) query.ProvinceCity = filters.value.ProvinceCity
  if (filters.value.FromDate) query.FromDate = filters.value.FromDate
  if (filters.value.ToDate) query.ToDate = filters.value.ToDate
  query.PageNumber = pageNumber.value
  
  router.push({ path: '/search', query })
}

const applySearch = () => {
  filters.value.Search = searchInput.value.trim()
  pageNumber.value = 1
  updateRoute()
}

const applyDates = () => {
  filters.value.FromDate = tempDate.value.FromDate
  filters.value.ToDate = tempDate.value.ToDate
  pageNumber.value = 1
  showDateFilter.value = false
  updateRoute()
}

const clearDates = () => {
  tempDate.value.FromDate = ''
  tempDate.value.ToDate = ''
  applyDates()
}

const applyGeneral = () => {
  filters.value.CategoryId = tempFilter.value.CategoryId
  filters.value.ProvinceCity = tempFilter.value.ProvinceCity
  pageNumber.value = 1
  showGeneralFilter.value = false
  updateRoute()
}

const clearGeneral = () => {
  tempFilter.value.CategoryId = ''
  tempFilter.value.ProvinceCity = ''
  applyGeneral()
}

const clearCategory = () => {
  tempFilter.value.CategoryId = ''
  filters.value.CategoryId = ''
  pageNumber.value = 1
  updateRoute()
}

const goToPage = (p) => {
  if (p < 1 || p > totalPages.value) return
  pageNumber.value = p
  updateRoute()
}

watch(() => route.query, () => {
  initFiltersFromQuery()
  loadEvents()
})

</script>
