<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12 px-2 md:px-0">
    <!-- Header & Actions -->
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-4">
      <div class="flex flex-col gap-2">
        <div class="flex items-center gap-2 text-white/50 text-sm font-medium mb-1">
          <router-link to="/moderator/dashboard" class="hover:text-primary transition-colors flex items-center gap-1">
            <PhArrowLeft weight="bold" class="text-xs" />
            Tổng quan
          </router-link>
          <span>/</span>
          <span class="text-white/30">Duyệt sự kiện</span>
        </div>
        <h1 class="font-heading text-3xl md:text-4xl font-black text-white tracking-tight">Duyệt sự kiện</h1>
        <p class="text-white/50 font-medium text-lg">Quản lý và xét duyệt các sự kiện từ nhà tổ chức.</p>
      </div>
    </div>

    <!-- Filters & Search -->
    <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-6 md:p-8 flex flex-col gap-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        
        <!-- Search -->
        <div class="flex flex-col gap-2 relative">
          <label class="text-[13px] font-bold text-white/80">Tìm kiếm</label>
          <div class="relative w-full group">
            <PhMagnifyingGlass class="absolute left-4 top-1/2 -translate-y-1/2 text-white/40 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
            <input
              v-model="filters.Search"
              @input="onSearchInput"
              type="text"
              placeholder="Tên sự kiện..."
              class="w-full bg-white/5 border border-white/10 rounded-xl pl-12 pr-4 py-3 text-white placeholder-white/30 focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none"
            />
          </div>
        </div>

        <!-- Status Filter -->
        <div class="flex flex-col gap-2 relative">
          <label class="text-[13px] font-bold text-white/80">Trạng thái</label>
          <div class="relative w-full group">
            <select
              v-model="filters.Status"
              @change="onFilterChange"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none appearance-none"
            >
              <option value="" class="text-black">Tất cả</option>
              <option v-for="status in eventStatuses" :key="status.id" :value="status.name" class="text-black">
                {{ getStatusLabel(status.name) }}
              </option>
            </select>
            <PhCaretDown class="absolute right-4 top-1/2 -translate-y-1/2 text-white/40 pointer-events-none" weight="bold" />
          </div>
        </div>

      </div>
    </div>

    <!-- Data Grid (Bento Style) -->
    <div class="w-full">
      <!-- Loading State -->
      <div v-if="isLoading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <div v-for="i in 8" :key="i" class="bg-white/5 rounded-3xl h-[380px] animate-pulse border border-white/5 flex flex-col overflow-hidden">
          <div class="h-48 bg-white/10 w-full"></div>
          <div class="p-5 flex flex-col gap-3 flex-1">
            <div class="h-6 bg-white/10 rounded-lg w-3/4"></div>
            <div class="h-4 bg-white/10 rounded-lg w-1/2"></div>
            <div class="mt-auto flex justify-between">
               <div class="h-8 bg-white/10 rounded-full w-24"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="events.length === 0" class="flex flex-col items-center justify-center py-20 bg-[#111916]/50 border border-white/5 rounded-[2rem]">
        <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center mb-6">
          <PhTicket class="text-4xl text-white/20" weight="duotone" />
        </div>
        <h3 class="text-xl font-bold text-white mb-2">Không có sự kiện nào</h3>
        <p class="text-white/50 text-center max-w-md">Không tìm thấy sự kiện nào phù hợp với bộ lọc hiện tại.</p>
        <button @click="resetFilters" class="mt-6 px-6 py-2.5 rounded-full bg-white/10 text-white font-medium hover:bg-white/20 transition-colors">
          Xóa bộ lọc
        </button>
      </div>

      <!-- Events Grid -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <router-link
          v-for="event in events"
          :key="event.id"
          :to="`/moderator/events/${event.id}`"
          class="group bg-[#111916]/80 backdrop-blur-sm border border-white/5 rounded-[24px] overflow-hidden hover:border-primary/50 hover:shadow-[0_8px_30px_rgba(0,200,83,0.1)] transition-all duration-500 flex flex-col"
        >
          <div class="relative h-48 overflow-hidden bg-black">
            <img :src="event.coverImageUrl || 'https://picsum.photos/seed/' + event.id + '/600/400'" :alt="event.title" class="w-full h-full object-cover group-hover:scale-105 group-hover:opacity-80 transition-all duration-700" />
            <div class="absolute inset-0 bg-gradient-to-t from-[#111916] via-transparent to-transparent opacity-80"></div>
            <div class="absolute top-4 right-4">
              <span :class="getStatusBadgeClass(event.status)" class="px-3 py-1.5 rounded-full text-[11px] font-bold uppercase tracking-wider backdrop-blur-md shadow-lg border">
                {{ getStatusLabel(event.status) }}
              </span>
            </div>
          </div>
          
          <div class="p-6 flex flex-col flex-1 gap-4">
            <div class="flex flex-col gap-1">
              <h3 class="font-heading font-black text-xl text-white group-hover:text-primary transition-colors line-clamp-2 leading-tight">
                {{ event.title }}
              </h3>
            </div>
            
            <div class="flex flex-col gap-3 mt-auto pt-4 border-t border-white/5">
              <div class="flex items-center gap-2 text-[13px] text-white/60">
                <PhCalendarBlank class="text-white/40" weight="bold" />
                <span class="truncate">{{ formatDateString(event.startAt) }}</span>
              </div>
              <div class="flex items-center gap-2 text-[13px] text-white/60">
                <PhMapPin class="text-white/40" weight="bold" />
                <span class="truncate">{{ event.location?.provinceCity || 'Chưa cập nhật' }}</span>
              </div>
            </div>
          </div>
        </router-link>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="!isLoading && totalPages > 0" class="px-6 py-4 flex items-center justify-between">
      <span class="text-[13px] text-white/50 font-medium">Trang {{ filters.PageNumber }} / {{ totalPages }}</span>
      <div class="flex items-center gap-2">
        <button 
          @click="prevPage" 
          :disabled="!hasPreviousPage"
          class="px-5 py-2.5 rounded-xl bg-white/5 border border-white/10 text-white/80 text-[13px] font-bold hover:bg-white/10 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-all"
        >
          Trước
        </button>
        <button 
          @click="nextPage" 
          :disabled="!hasNextPage"
          class="px-5 py-2.5 rounded-xl bg-white/5 border border-white/10 text-white/80 text-[13px] font-bold hover:bg-white/10 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-all"
        >
          Sau
        </button>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { getModeratorEvents, getEventStatusesForModerator } from '../../services/eventService'
import { PhArrowLeft, PhMagnifyingGlass, PhCaretDown, PhTicket, PhCalendarBlank, PhMapPin } from '@phosphor-icons/vue'

const events = ref([])
const totalPages = ref(0)
const hasNextPage = ref(false)
const hasPreviousPage = ref(false)
const isLoading = ref(false)
const eventStatuses = ref([])

const filters = reactive({
  PageNumber: 1,
  PageSize: 12,
  Search: '',
  Status: 'PendingApproval' // Default to pending approval to match requirement
})

let searchTimeout = null

onMounted(async () => {
  await loadStatuses()
  await loadEvents()
})

const loadStatuses = async () => {
  try {
    const res = await getEventStatusesForModerator()
    if (res.success) {
      eventStatuses.value = res.data || []
    }
  } catch (error) {
    console.error("Failed to load statuses", error)
  }
}

const loadEvents = async () => {
  isLoading.value = true
  try {
    // Clean up empty params
    const activeFilters = { ...filters }
    if (!activeFilters.Search) delete activeFilters.Search
    if (!activeFilters.Status) delete activeFilters.Status

    const res = await getModeratorEvents(activeFilters)
    if (res.success && res.data) {
      events.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      hasNextPage.value = res.data.hasNextPage || false
      hasPreviousPage.value = res.data.hasPreviousPage || false
    }
  } catch (error) {
    console.error("Failed to load events", error)
  } finally {
    isLoading.value = false
  }
}

const onSearchInput = () => {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    filters.PageNumber = 1
    loadEvents()
  }, 500)
}

const onFilterChange = () => {
  filters.PageNumber = 1
  loadEvents()
}

const resetFilters = () => {
  filters.Search = ''
  filters.Status = ''
  filters.PageNumber = 1
  loadEvents()
}

const prevPage = () => {
  if (hasPreviousPage.value) {
    filters.PageNumber--
    loadEvents()
  }
}

const nextPage = () => {
  if (hasNextPage.value) {
    filters.PageNumber++
    loadEvents()
  }
}

// Format Date string
const formatDateString = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  }).format(date)
}

const getStatusLabel = (status) => {
  const map = {
    'PendingApproval': 'Chờ duyệt',
    'Published': 'Đã xuất bản',
    'Rejected': 'Từ chối',
    'Draft': 'Bản nháp',
    'Bị từ chối': 'Bị từ chối',
    'Chờ duyệt': 'Chờ duyệt'
  }
  return map[status] || status
}

const getStatusBadgeClass = (status) => {
  if (status === 'Published' || status === 'Đã xuất bản') return 'bg-[#00c853]/10 text-[#00c853] border-[#00c853]/20'
  if (status === 'PendingApproval' || status === 'Chờ duyệt') return 'bg-[#ff9100]/10 text-[#ff9100] border-[#ff9100]/20'
  if (status === 'Rejected' || status === 'Bị từ chối') return 'bg-danger/10 text-danger border-danger/20'
  return 'bg-white/10 text-white/70 border-white/20'
}

</script>
