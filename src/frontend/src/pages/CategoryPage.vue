<template>
  <div class="py-8 max-w-[1440px] mx-auto px-6 md:px-10">
    <CategoryFilter
      v-model="activeFilter"
      v-model:city="city"
      :availableFilters="timeFilters"
      :availableCities="availableCities"
    />

    <!-- Loading State -->
    <div v-if="isLoading" class="py-20 flex flex-col items-center justify-center text-white/50">
      <div class="w-10 h-10 border-4 border-primary/20 border-t-primary rounded-full animate-spin mb-4"></div>
      <p class="font-bold text-sm animate-pulse">Đang tải sự kiện...</p>
    </div>

    <template v-else>
      <!-- Results count -->
      <div class="flex items-center gap-2 mb-4 px-1" v-if="events.length > 0">
        <span class="font-heading text-[15px] font-bold text-primary">{{ totalCount }} sự kiện</span>
      </div>

      <CategoryEventGrid
        :events="events"
        @reset="resetFilters"
      />

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="mt-4 flex justify-center">
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
    </template>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { getPublicEvents, getPublicEventCategories } from '../services/eventService'
import { getProvinces } from '../services/location.service'
import { store } from '../stores/eventStore'
import { getErrorMessage } from '../utils/apiError'
import CategoryFilter from '../components/category/CategoryFilter.vue'
import CategoryEventGrid from '../components/category/CategoryEventGrid.vue'
import { PhCaretLeft, PhCaretRight } from '@phosphor-icons/vue'

const route = useRoute()

// Filter state
const activeFilter = ref('Tất cả')
const city = ref('')
const timeFilters = ['Tất cả', 'Hôm nay', 'Tuần này', 'Tháng này']

// Data state
const isLoading = ref(true)
const events = ref([])
const categories = ref([])
const availableCities = ref([])
const totalCount = ref(0)
const pageNumber = ref(1)
const totalPages = ref(1)
const hasPreviousPage = ref(false)
const hasNextPage = ref(false)

// Danh mục thật khớp với slug trên URL (BE trả về slug tự sinh từ tên danh mục)
const currentCategory = computed(() => {
  const slug = route.params.type
  return categories.value.find(c => c.slug === slug) || null
})

const fetchCategories = async () => {
  try {
    const res = await getPublicEventCategories()
    categories.value = res?.data?.data ?? res?.data ?? (Array.isArray(res) ? res : [])
  } catch (error) {
    console.error('Failed to fetch categories', error)
  }
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

// Quy đổi filter thời gian (pill) thành FromDate/ToDate thật gửi lên BE
const dateRangeForFilter = (filter) => {
  const now = new Date()
  const toISO = (d) => d.toISOString()

  if (filter === 'Hôm nay') {
    const from = new Date(now); from.setHours(0, 0, 0, 0)
    const to = new Date(now); to.setHours(23, 59, 59, 999)
    return { from: toISO(from), to: toISO(to) }
  }
  if (filter === 'Tuần này') {
    const from = new Date(now); from.setHours(0, 0, 0, 0)
    const to = new Date(now)
    const dayOfWeek = to.getDay() || 7
    to.setDate(to.getDate() + (7 - dayOfWeek))
    to.setHours(23, 59, 59, 999)
    return { from: toISO(from), to: toISO(to) }
  }
  if (filter === 'Tháng này') {
    const from = new Date(now); from.setHours(0, 0, 0, 0)
    const to = new Date(now.getFullYear(), now.getMonth() + 1, 0)
    to.setHours(23, 59, 59, 999)
    return { from: toISO(from), to: toISO(to) }
  }
  return { from: '', to: '' }
}

const loadEvents = async () => {
  isLoading.value = true
  try {
    if (!currentCategory.value) {
      events.value = []
      totalCount.value = 0
      totalPages.value = 1
      return
    }

    const params = {
      CategoryId: currentCategory.value.id,
      PageNumber: pageNumber.value,
      PageSize: 12
    }
    if (city.value) params.ProvinceCity = city.value
    const { from, to } = dateRangeForFilter(activeFilter.value)
    if (from) params.FromDate = from
    if (to) params.ToDate = to

    const response = await getPublicEvents(params)

    if (response && response.data && Array.isArray(response.data.data)) {
      events.value = response.data.data
      totalCount.value = response.data.totalCount || 0
      pageNumber.value = response.data.pageNumber || 1
      totalPages.value = response.data.totalPages || 1
      hasPreviousPage.value = response.data.hasPreviousPage || false
      hasNextPage.value = response.data.hasNextPage || false
    } else {
      events.value = []
      totalCount.value = 0
      totalPages.value = 1
    }
  } catch (error) {
    console.error('Failed to load category events:', error)
    store.toast = { message: getErrorMessage(error, 'Không thể tải danh sách sự kiện.'), icon: '❌' }
    events.value = []
  } finally {
    isLoading.value = false
  }
}

const resetFilters = () => {
  activeFilter.value = 'Tất cả'
  city.value = ''
  pageNumber.value = 1
}

const goToPage = (p) => {
  if (p < 1 || p > totalPages.value) return
  pageNumber.value = p
  loadEvents()
}

onMounted(async () => {
  await Promise.all([fetchCategories(), fetchCities()])
  loadEvents()
})

watch(() => route.params.type, () => {
  pageNumber.value = 1
  loadEvents()
})

watch([activeFilter, city], () => {
  pageNumber.value = 1
  loadEvents()
})
</script>
