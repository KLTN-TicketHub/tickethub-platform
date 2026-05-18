<template>
  <div class="py-8">
    <CategoryHero 
      :title="categoryData.title"
      :subtitle="categoryData.subtitle"
      :icon="categoryData.icon"
      :bgGradient="categoryData.bgGradient"
    />

    <CategoryFilter 
      v-model="activeFilter"
      v-model:sortBy="sortBy"
      v-model:city="city"
      v-model:status="statusFilter"
      :availableFilters="timeFilters"
      :colorTheme="categoryData.bgGradient"
    />

    <!-- Results count -->
    <div class="flex items-center gap-2 mb-4 px-1" v-if="filteredEvents.length > 0">
      <span class="font-heading text-[15px] font-bold text-primary">{{ filteredEvents.length }} sự kiện</span>
      <span v-if="totalEvents !== filteredEvents.length" class="text-[13px] text-muted">
        (trong tổng số {{ totalEvents }})
      </span>
    </div>

    <CategoryEventGrid 
      :events="filteredEvents"
      :categoryLabel="categoryData.title"
      :categoryIcon="categoryData.icon"
      @reset="resetFilters"
    />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { getEvents } from '../features/events/store'
import CategoryHero from '../components/category/CategoryHero.vue'
import CategoryFilter from '../components/category/CategoryFilter.vue'
import CategoryEventGrid from '../components/category/CategoryEventGrid.vue'

const route = useRoute()

// Filter state
const activeFilter = ref('Tất cả')
const sortBy = ref('newest')
const city = ref('all')
const statusFilter = ref('all')

const timeFilters = ['Tất cả', 'Hôm nay', 'Tuần này', 'Tháng này', 'Sắp tới']

const categoryConfigs = {
  concerts: {
    title: 'Nhạc & Concert',
    subtitle: 'Hòa mình vào không gian âm nhạc sống động với những nghệ sĩ hàng đầu.',
    icon: '🎵',
    bgGradient: 'linear-gradient(135deg, #1A0A15 0%, #4A0E35 50%, #1A0A15 100%)'
  },
  arts: {
    title: 'Sân Khấu & Nghệ Thuật',
    subtitle: 'Khám phá những tác phẩm kịch nghệ và triển lãm nghệ thuật đặc sắc.',
    icon: '🎭',
    bgGradient: 'linear-gradient(135deg, #0A1525 0%, #1A3A6A 50%, #0A1525 100%)'
  },
  sports: {
    title: 'Thể Thao',
    subtitle: 'Cháy hết mình với những trận cầu đỉnh cao và sự kiện thể thao kịch tính.',
    icon: '🏆',
    bgGradient: 'linear-gradient(135deg, #0A2515 0%, #155530 50%, #0A2515 100%)'
  },
  experiences: {
    title: 'Trải Nghiệm',
    subtitle: 'Những chuyến đi và hoạt động trải nghiệm thực tế đầy thú vị.',
    icon: '🧭',
    bgGradient: 'linear-gradient(135deg, #251005 0%, #6A2505 50%, #251005 100%)'
  },
  workshops: {
    title: 'Hội Thảo & Khóa Học',
    subtitle: 'Nâng cao kỹ năng và kiến thức với những chuyên gia hàng đầu.',
    icon: '📚',
    bgGradient: 'linear-gradient(135deg, #051A25 0%, #104A6A 50%, #051A25 100%)'
  },
  others: {
    title: 'Khác',
    subtitle: 'Khám phá những sự kiện đa dạng và độc đáo khác.',
    icon: '✨',
    bgGradient: 'linear-gradient(135deg, #1A1A1A 0%, #333333 50%, #1A1A1A 100%)'
  }
}

const categoryData = computed(() => {
  const type = route.params.type || 'others'
  return categoryConfigs[type] || categoryConfigs.others
})

// ─── HELPER: date boundaries ────────────────────────────────────────────────
const now = new Date()

const startOfToday = computed(() => {
  const d = new Date(now)
  d.setHours(0, 0, 0, 0)
  return d
})

const endOfToday = computed(() => {
  const d = new Date(now)
  d.setHours(23, 59, 59, 999)
  return d
})

const endOfWeek = computed(() => {
  const d = new Date(now)
  const dayOfWeek = d.getDay() || 7 // Monday = 1, Sunday = 7
  d.setDate(d.getDate() + (7 - dayOfWeek))
  d.setHours(23, 59, 59, 999)
  return d
})

const endOfMonth = computed(() => {
  const d = new Date(now.getFullYear(), now.getMonth() + 1, 0)
  d.setHours(23, 59, 59, 999)
  return d
})

// ─── MAIN FILTER + SORT ─────────────────────────────────────────────────────
const categoryEvents = computed(() => {
  const allEvents = getEvents()
  const type = route.params.type || 'others'
  if (type === 'others') return allEvents
  return allEvents.filter(e => e.category === type)
})

const totalEvents = computed(() => categoryEvents.value.length)

const filteredEvents = computed(() => {
  let result = [...categoryEvents.value]

  // 1. Time filter
  if (activeFilter.value !== 'Tất cả') {
    result = result.filter(e => {
      const eventDate = new Date(e.dateStart)
      if (isNaN(eventDate.getTime())) return false

      switch (activeFilter.value) {
        case 'Hôm nay':
          return eventDate >= startOfToday.value && eventDate <= endOfToday.value
        case 'Tuần này':
          return eventDate >= startOfToday.value && eventDate <= endOfWeek.value
        case 'Tháng này':
          return eventDate >= startOfToday.value && eventDate <= endOfMonth.value
        case 'Sắp tới':
          return eventDate > now
        default:
          return true
      }
    })
  }

  // 2. City filter
  if (city.value !== 'all') {
    result = result.filter(e => {
      const eventCity = e.location?.city || ''
      return eventCity.toLowerCase() === city.value.toLowerCase()
    })
  }

  // 3. Status filter
  if (statusFilter.value !== 'all') {
    result = result.filter(e => e.status === statusFilter.value)
  }

  // 4. Sort
  switch (sortBy.value) {
    case 'newest':
      result.sort((a, b) => new Date(b.dateStart) - new Date(a.dateStart))
      break
    case 'oldest':
      result.sort((a, b) => new Date(a.dateStart) - new Date(b.dateStart))
      break
    case 'price-asc':
      result.sort((a, b) => (a.priceRange?.min || 0) - (b.priceRange?.min || 0))
      break
    case 'price-desc':
      result.sort((a, b) => (b.priceRange?.min || 0) - (a.priceRange?.min || 0))
      break
    case 'name-asc':
      result.sort((a, b) => a.title.localeCompare(b.title, 'vi'))
      break
    case 'name-desc':
      result.sort((a, b) => b.title.localeCompare(a.title, 'vi'))
      break
  }

  return result
})

const resetFilters = () => {
  activeFilter.value = 'Tất cả'
  sortBy.value = 'newest'
  city.value = 'all'
  statusFilter.value = 'all'
}
</script>
