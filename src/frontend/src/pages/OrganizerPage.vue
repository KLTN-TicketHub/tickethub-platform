<template>
  <div class="max-w-[1400px] mx-auto py-12 px-6 lg:px-10 min-h-[80vh]">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-8 mb-12 animate-fade-up">
      <div class="space-y-4">
        <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-black tracking-widest uppercase shadow-[0_0_20px_rgba(0,200,83,0.15)]">
          <PhSuitcase weight="fill" /> Organizer Dashboard
        </div>
        <h1 class="font-heading text-4xl lg:text-5xl font-black text-white tracking-tight uppercase">Trung tâm Tổ chức</h1>
        <p class="text-white/50 font-medium text-lg max-w-xl">Quản lý các sự kiện và theo dõi hiệu suất bán vé của bạn một cách trực quan nhất.</p>
      </div>
      <router-link to="/organizer/create-event">
        <BaseButton variant="primary" size="lg" class="!px-8 !rounded-2xl shadow-[0_0_30px_rgba(0,200,83,0.3)] hover:scale-105 transition-transform flex items-center gap-2">
          <PhPlus weight="bold" /> Tạo sự kiện mới
        </BaseButton>
      </router-link>
    </div>

    <!-- Stats Grid (Bento) -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-16 animate-fade-up [animation-delay:100ms]">
      <div v-for="stat in computedStats" :key="stat.label" class="bg-[#111916] border border-white/5 p-8 rounded-[2.5rem] hover:border-primary/30 transition-all group shadow-2xl relative overflow-hidden">
        <div class="absolute -right-8 -top-8 w-24 h-24 bg-white/5 rounded-full blur-2xl group-hover:bg-primary/10 transition-colors"></div>
        <div class="flex items-center justify-between mb-8 relative z-10">
          <div class="w-14 h-14 rounded-[1.25rem] bg-[#0A0F0D] border border-white/10 flex items-center justify-center text-3xl group-hover:scale-110 transition-transform shadow-inner" :class="stat.color">
            <component :is="stat.icon" weight="duotone" />
          </div>
          <span class="text-[12px] font-black bg-primary/10 border border-primary/20 text-primary px-3 py-1.5 rounded-xl flex items-center gap-1 shadow-sm">
            <PhTrendUp weight="bold" /> {{ stat.trend }}%
          </span>
        </div>
        <div class="relative z-10">
          <div class="text-white/40 text-[12px] font-bold uppercase tracking-widest mb-1.5">{{ stat.label }}</div>
          <div class="text-4xl lg:text-5xl font-heading font-black text-white tracking-tighter">{{ stat.value }}</div>
        </div>
      </div>
    </div>

    <!-- Events Section -->
    <div class="flex flex-col gap-6 animate-fade-up [animation-delay:200ms]">
      <!-- Filter and Search Bar -->
      <div class="flex flex-col lg:flex-row lg:items-center justify-between gap-6">
        <!-- Search Input -->
        <div class="flex items-center gap-3 w-full lg:max-w-md">
          <div class="flex-1 flex items-center gap-3 bg-[#111916] border border-white/5 focus-within:border-primary/30 rounded-2xl px-4 transition-all">
            <PhMagnifyingGlass class="text-white/30 text-lg" weight="bold" />
            <input 
              type="text" 
              v-model="searchKeyword" 
              @keyup.enter="handleSearch"
              placeholder="Tìm kiếm sự kiện" 
              class="flex-1 bg-transparent border-none py-3.5 text-[14px] text-white outline-none placeholder:text-white/20 font-medium"
            />
          </div>
          <BaseButton 
            variant="primary" 
            @click="handleSearch"
            class="!py-3.5 !px-6 !rounded-2xl font-bold text-[14px] cursor-pointer"
          >
            Tìm kiếm
          </BaseButton>
        </div>

        <!-- Status Tabs -->
        <div class="flex items-center bg-[#111916] p-1.5 rounded-2xl border border-white/5 overflow-x-auto hide-scroll">
          <button 
            v-for="status in eventStatuses" 
            :key="status.id"
            @click="selectStatus(status.id)"
            class="px-6 py-2.5 rounded-xl font-bold text-[13px] transition-all whitespace-nowrap cursor-pointer"
            :class="activeStatusId === status.id ? 'bg-primary text-black shadow-md' : 'text-white/50 hover:text-white hover:bg-white/5'"
          >
            {{ getTabLabel(status) }}
          </button>
        </div>
      </div>

      <!-- Warning Banner -->
      <Transition
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div 
          v-if="isPendingApprovalActive" 
          class="flex items-start gap-3 p-4 bg-[#FFC107]/10 border border-[#FFC107]/20 rounded-2xl text-[#FFC107] text-[13px] font-bold leading-relaxed shadow-sm"
        >
          <PhWarningCircle class="text-[#FFC107] text-lg flex-shrink-0 mt-0.5" weight="fill" />
          <span>Lưu ý: Sự kiện đang chờ duyệt. Để đảm bảo tính bảo mật cho sự kiện của bạn, quyền truy cập vào trang chỉ dành cho chủ sở hữu và quản trị viên được ủy quyền</span>
        </div>
      </Transition>

      <!-- Events List (Card Layout) -->
      <div v-if="isLoading" class="flex flex-col gap-6">
        <!-- Card Skeleton Loader -->
        <div v-for="n in 3" :key="n" class="bg-[#111916] border border-white/5 rounded-3xl p-6 animate-pulse flex flex-col md:flex-row gap-6">
          <div class="w-full md:w-[240px] aspect-[16/9] rounded-2xl bg-white/5 flex-shrink-0"></div>
          <div class="flex-1 flex flex-col justify-between py-1 gap-4">
            <div class="space-y-3">
              <div class="h-6 bg-white/5 rounded-md w-3/4"></div>
              <div class="h-4 bg-white/5 rounded-md w-1/2"></div>
              <div class="h-4 bg-white/5 rounded-md w-2/3"></div>
            </div>
            <div class="h-10 bg-white/5 rounded-xl w-full"></div>
          </div>
        </div>
      </div>

      <div v-else-if="events.length === 0" class="py-24 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[3rem]">
        <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
          <PhMagnifyingGlass weight="duotone" />
        </div>
        <h3 class="text-xl font-bold font-heading text-white mb-2">Không tìm thấy sự kiện</h3>
        <p class="text-white/50 max-w-xs mb-8">Bạn chưa có sự kiện nào ở trạng thái này hoặc từ khóa tìm kiếm không khớp.</p>
      </div>

      <div v-else class="flex flex-col gap-6">
        <!-- Event Cards -->
        <div 
          v-for="event in events" 
          :key="event.id" 
          class="bg-[#111916] border border-white/5 hover:border-primary/20 rounded-[2.5rem] overflow-hidden shadow-2xl transition-all duration-300 group hover:-translate-y-0.5"
        >
          <div class="p-6 md:p-8 flex flex-col md:flex-row gap-6 md:gap-8">
            <!-- Event Cover Image -->
            <div class="w-full md:w-[240px] aspect-[16/9] rounded-2xl overflow-hidden border border-white/5 bg-[#0A0F0D] flex-shrink-0 relative">
              <img 
                :src="event.coverImageUrl || 'https://picsum.photos/seed/event-placeholder/400/225'" 
                class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                @error="handleImageError"
              />
            </div>

            <!-- Event Details -->
            <div class="flex-1 flex flex-col justify-between py-1">
              <div>
                <h3 
                  @click="router.push(`/organizer/events/${event.id}`)"
                  class="font-heading text-2xl font-black text-white group-hover:text-primary transition-colors line-clamp-1 mb-4 cursor-pointer hover:underline"
                >
                  {{ event.title }}
                </h3>
                
                <div class="flex flex-col gap-3">
                  <!-- Date -->
                  <div class="flex items-center gap-2.5 text-white/70 text-[14px] font-bold">
                    <PhCalendarBlank class="text-primary text-lg" weight="bold" />
                    <span>{{ formatEventDate(event.startAt) }}</span>
                  </div>

                  <!-- Location -->
                  <div class="flex items-start gap-2.5 text-white/50 text-[13px] font-medium leading-normal">
                    <PhMapPin class="text-primary text-lg mt-0.5 flex-shrink-0" weight="fill" />
                    <div class="flex flex-col">
                      <span class="text-white/80 font-bold text-[14px]">{{ event.location?.venueName || 'Địa điểm tự chọn' }}</span>
                      <span class="mt-0.5 text-white/40">{{ formatLocation(event.location) }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Bottom Actions -->
          <div class="bg-[#182019] px-6 py-4 border-t border-white/5 flex flex-wrap items-center justify-center md:justify-start gap-y-3">
            <template v-for="(action, aIdx) in eventActions" :key="action.label">
              <button 
                @click="handleAction(action, event)"
                class="px-5 py-1 text-white/50 hover:text-white transition-colors text-[13px] font-bold flex items-center gap-2 cursor-pointer"
              >
                <component :is="action.icon" class="text-lg" weight="bold" />
                {{ action.label }}
              </button>
              <div 
                v-if="aIdx < eventActions.length - 1" 
                class="hidden sm:block h-4 w-px bg-white/10 mx-2"
              ></div>
            </template>
          </div>
        </div>

        <!-- Premium Pagination Component -->
        <div v-if="totalPages > 1" class="flex items-center justify-between mt-12 px-6 py-4 bg-[#111916]/80 backdrop-blur-md border border-white/5 rounded-[2rem] shadow-2xl">
          <div class="text-[13px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ currentPage }}</span> / <span class="text-white font-bold">{{ totalPages }}</span>
          </div>
          
          <div class="flex items-center gap-2">
            <!-- Previous Page Button -->
            <button 
              @click="changePage(currentPage - 1)"
              :disabled="currentPage === 1"
              class="group flex items-center gap-2 px-4 py-2.5 rounded-xl bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <PhCaretLeft weight="bold" class="text-[16px] text-white/50 group-hover:text-white group-hover:-translate-x-0.5 transition-all" />
              <span class="text-[13px] font-bold text-white/50 group-hover:text-white transition-colors">Trước</span>
            </button>

            <!-- Page Numbers -->
            <div class="flex items-center gap-1.5">
              <template v-for="(page, index) in computedVisiblePages" :key="index">
                <span v-if="page === '...'" class="w-10 text-center text-white/30 font-bold tracking-widest">...</span>
                <button 
                  v-else
                  @click="changePage(page)"
                  class="w-10 h-10 flex items-center justify-center rounded-xl font-bold text-[14px] transition-all cursor-pointer relative"
                  :class="currentPage === page ? 'text-black bg-primary shadow-[0_0_20px_rgba(0,200,83,0.3)] scale-110' : 'text-white/60 hover:text-white hover:bg-white/10'"
                >
                  {{ page }}
                </button>
              </template>
            </div>

            <!-- Next Page Button -->
            <button 
              @click="changePage(currentPage + 1)"
              :disabled="currentPage === totalPages"
              class="group flex items-center gap-2 px-4 py-2.5 rounded-xl bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <span class="text-[13px] font-bold text-white/50 group-hover:text-white transition-colors">Sau</span>
              <PhCaretRight weight="bold" class="text-[16px] text-white/50 group-hover:text-white group-hover:translate-x-0.5 transition-all" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, markRaw } from 'vue'
import { useRouter } from 'vue-router'
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { getOrganizerEvents, getEventStatuses } from '../services/organizer.service'
import { 
  PhSuitcase, PhPlus, PhTicket, PhTrendUp, PhCurrencyCircleDollar, 
  PhEye, PhCalendarBlank, PhPencilSimple, PhMagnifyingGlass, PhMapPin, 
  PhChartPie, PhUsers, PhReceipt, PhArmchair, PhCaretLeft, PhCaretRight, 
  PhWarningCircle 
} from '@phosphor-icons/vue'

const router = useRouter()
const isLoading = ref(true)
const events = ref([])
const eventStatuses = ref([])
const activeStatusId = ref(1) // Default to 'Chờ duyệt' (ID 1)
const searchKeyword = ref('')
const currentPage = ref(1)
const totalPages = ref(1)
const totalCount = ref(0)
const pageSize = 12

// Dynamically fetch event statuses and initial events
onMounted(async () => {
  try {
    const statusesRes = await getEventStatuses()
    if (statusesRes && statusesRes.success && statusesRes.data) {
      eventStatuses.value = statusesRes.data
      if (eventStatuses.value.length > 0) {
        const pendingStatus = eventStatuses.value.find(s => s.id === 1)
        activeStatusId.value = pendingStatus ? pendingStatus.id : eventStatuses.value[0].id
      }
    } else {
      // Fallback local status enums if API response is invalid or empty
      eventStatuses.value = [
        { id: 1, name: 'Chờ duyệt' },
        { id: 2, name: 'Đã xuất bản' },
        { id: 3, name: 'Đã qua' }
      ]
    }
  } catch (err) {
    console.error('Error fetching event statuses:', err)
    eventStatuses.value = [
      { id: 1, name: 'Chờ duyệt' },
      { id: 2, name: 'Đã xuất bản' },
      { id: 3, name: 'Đã qua' }
    ]
  }
  
  await fetchData()
})

const fetchData = async () => {
  isLoading.value = true
  try {
    const params = {
      Status: activeStatusId.value,
      PageNumber: currentPage.value,
      PageSize: pageSize
    }
    if (searchKeyword.value.trim()) {
      params.Search = searchKeyword.value.trim()
    }
    
    const res = await getOrganizerEvents(params)
    if (res && res.success && res.data) {
      events.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      totalCount.value = res.data.totalCount || 0
      currentPage.value = res.data.pageNumber || 1
    } else {
      events.value = []
      totalPages.value = 1
      totalCount.value = 0
    }
  } catch (err) {
    console.error('Error fetching events:', err)
    events.value = []
    totalPages.value = 1
    totalCount.value = 0
  } finally {
    isLoading.value = false
  }
}

// Stats computed dynamically
const computedStats = computed(() => {
  return [
    { label: 'Tổng sự kiện', value: totalCount.value.toString(), icon: markRaw(PhTicket), trend: '12', color: 'text-primary' },
    { label: 'Vé đã bán', value: '450', icon: markRaw(PhTrendUp), trend: '8', color: 'text-info' },
    { label: 'Doanh thu', value: '67.5M', icon: markRaw(PhCurrencyCircleDollar), trend: '15', color: 'text-warning' },
    { label: 'Lượt xem', value: '2.4K', icon: markRaw(PhEye), trend: '24', color: 'text-[#f43f5e]' },
  ]
})

// Tab label mapper (maps API response status names to preferred UI names)
const getTabLabel = (status) => {
  if (status.id === 2) return 'Sắp tới' // Maps 'Đã xuất bản' to 'Sắp tới' to match the design
  return status.name
}

const isPendingApprovalActive = computed(() => {
  return activeStatusId.value === 1
})

const handleSearch = () => {
  currentPage.value = 1
  fetchData()
}

const selectStatus = (id) => {
  activeStatusId.value = id
  currentPage.value = 1
  fetchData()
}

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  fetchData()
}

// Visible pages for pagination with ellipsis
const computedVisiblePages = computed(() => {
  const current = currentPage.value
  const total = totalPages.value
  if (total <= 7) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }
  
  if (current <= 4) {
    return [1, 2, 3, 4, 5, '...', total]
  }
  if (current >= total - 3) {
    return [1, '...', total - 4, total - 3, total - 2, total - 1, total]
  }
  return [1, '...', current - 1, current, current + 1, '...', total]
})

// Format Date: e.g. "00:00, Thứ 6, 15 tháng 05 2026"
const formatEventDate = (dateStr) => {
  if (!dateStr) return 'TBA'
  try {
    const date = new Date(dateStr)
    const time = date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false })
    
    // Custom weekday formatting to match Vietnamese pattern
    const dayOfWeek = date.getDay()
    const weekdayMap = {
      0: 'Chủ nhật',
      1: 'Thứ 2',
      2: 'Thứ 3',
      3: 'Thứ 4',
      4: 'Thứ 5',
      5: 'Thứ 6',
      6: 'Thứ 7'
    }
    const weekday = weekdayMap[dayOfWeek]
    
    const day = date.getDate().toString().padStart(2, '0')
    const month = (date.getMonth() + 1).toString().padStart(2, '0')
    const year = date.getFullYear()
    
    return `${time}, ${weekday}, ${day} tháng ${month} ${year}`
  } catch (e) {
    return dateStr
  }
}

// Join location info
const formatLocation = (loc) => {
  if (!loc) return 'Chưa xác định'
  const parts = []
  if (loc.addressLine) parts.push(loc.addressLine)
  if (loc.ward) parts.push(loc.ward)
  if (loc.district) parts.push(loc.district)
  if (loc.provinceCity) parts.push(loc.provinceCity)
  return parts.join(', ')
}

const handleImageError = (e) => {
  e.target.src = 'https://picsum.photos/seed/event-placeholder/400/225'
}

// Card bottom actions
const eventActions = [
  { label: 'Tổng quan', icon: markRaw(PhChartPie) },
  { label: 'Đơn hàng', icon: markRaw(PhReceipt) }
]

const handleAction = (action, event) => {
  store.toast = { 
    message: `Chức năng "${action.label}" cho sự kiện "${event.title}" đang được phát triển!`, 
    icon: '✨' 
  }
}
</script>

<style scoped>
.hide-scroll::-webkit-scrollbar {
  display: none;
}
.hide-scroll {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
