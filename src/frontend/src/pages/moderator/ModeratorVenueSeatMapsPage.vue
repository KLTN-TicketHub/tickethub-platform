<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12 px-2 md:px-0">
    <!-- Header & Actions -->
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-4">
      <div class="flex flex-col gap-2">
        <div class="flex items-center gap-2 text-white/50 text-sm font-medium mb-1">
          <router-link to="/moderator/venues" class="hover:text-primary transition-colors flex items-center gap-1">
            <PhArrowLeft weight="bold" class="text-xs" />
            Quản lý địa điểm
          </router-link>
          <span>/</span>
          <span class="text-white/30">Sơ đồ ghế</span>
        </div>
        <h1 class="font-heading text-3xl md:text-4xl font-black text-white tracking-tight">Sơ đồ ghế</h1>
        <p v-if="venue" class="text-white/50 font-medium text-lg flex items-center gap-2">
          <span>Danh sách sơ đồ chỗ ngồi của địa điểm:</span>
          <span class="text-primary font-bold">{{ venue.venueName }}</span>
          <span class="text-xs px-2 py-0.5 rounded bg-white/10 text-white/70">{{ venue.venueCode }}</span>
        </p>
        <p v-else class="text-white/30 font-medium text-lg">Đang tải thông tin địa điểm...</p>
      </div>
      <div class="flex items-center gap-3">
        <router-link
          :to="`/moderator/venues/${venueId}/seat-maps/create`"
          class="inline-flex items-center justify-center gap-2 px-5 py-2.5 text-[13px] font-bold text-black bg-primary rounded-xl hover:scale-[1.02] active:scale-95 transition-transform"
        >
          <PhPlus weight="bold" />
          Tạo sơ đồ mới
        </router-link>
        <router-link
          to="/moderator/venues"
          class="inline-flex items-center justify-center gap-2 px-5 py-2.5 text-[13px] font-bold text-white bg-white/5 border border-white/10 rounded-xl hover:bg-white/10 transition-colors"
        >
          <PhArrowLeft weight="bold" />
          Quay lại
        </router-link>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] overflow-hidden flex flex-col">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="border-b border-white/5 bg-white/5">
              <th class="px-6 py-4 text-[11px] font-bold uppercase tracking-wider text-white/50">Mã sơ đồ ghế</th>
              <th class="px-6 py-4 text-[11px] font-bold uppercase tracking-wider text-white/50">Tên sơ đồ ghế</th>
              <th class="px-6 py-4 text-[11px] font-bold uppercase tracking-wider text-white/50 text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-white/5">
            <tr v-if="isLoading" class="animate-pulse">
              <td colspan="3" class="px-6 py-8 text-center text-white/40 font-medium">Đang tải dữ liệu sơ đồ ghế...</td>
            </tr>
            <tr v-else-if="seatMaps.length === 0">
              <td colspan="3" class="px-6 py-12 text-center flex flex-col items-center justify-center gap-3 w-full">
                <div class="w-12 h-12 rounded-full bg-white/5 flex items-center justify-center text-white/30">
                  <PhSquaresFour class="w-6 h-6" />
                </div>
                <span class="text-white/40 font-medium text-[14px]">Địa điểm này chưa có sơ đồ ghế nào được tạo.</span>
              </td>
            </tr>
            <tr v-else v-for="sm in seatMaps" :key="sm.id"
              class="hover:bg-white/[0.02] transition-colors group cursor-pointer"
              @click="$router.push(`/moderator/venues/${venueId}/seat-maps/${sm.id}`)"
            >
              <td class="px-6 py-4 text-[13px] font-medium text-white/60 font-mono">{{ sm.seatMapCode }}</td>
              <td class="px-6 py-4">
                <span class="font-bold text-white group-hover:text-primary transition-colors">{{ sm.seatMapName }}</span>
              </td>
              <td class="px-6 py-4 text-right">
                <div class="flex items-center justify-end gap-2">
                  <router-link
                    :to="`/moderator/venues/${venueId}/seat-maps/${sm.id}`"
                    class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary/10 text-primary text-[12px] font-bold hover:bg-primary/20 transition-colors"
                    @click.stop
                  >
                    <PhArrowSquareOut weight="bold" />
                    Xem chi tiết
                  </router-link>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="!isLoading && totalPages > 0" class="px-6 py-4 border-t border-white/5 flex items-center justify-between">
        <span class="text-[13px] text-white/50 font-medium">Trang {{ filters.PageNumber }} / {{ totalPages }}</span>
        <div class="flex items-center gap-2">
          <button 
            @click="prevPage" 
            :disabled="!hasPreviousPage"
            class="px-4 py-2 rounded-lg bg-white/5 text-white/80 text-[13px] font-bold hover:bg-white/10 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
          >
            Trước
          </button>
          <button 
            @click="nextPage" 
            :disabled="!hasNextPage"
            class="px-4 py-2 rounded-lg bg-white/5 text-white/80 text-[13px] font-bold hover:bg-white/10 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
          >
            Sau
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { getModeratorVenueById, getModeratorVenueSeatMaps } from '../../services/venue.service'
import { addToast } from '../../stores/adminStore'
import { getErrorMessage } from '../../utils/apiError'
import { PhArrowLeft, PhSquaresFour, PhPlus, PhArrowSquareOut } from '@phosphor-icons/vue'

const route = useRoute()
const venueId = route.params.id

const venue = ref(null)
const seatMaps = ref([])
const totalPages = ref(0)
const hasNextPage = ref(false)
const hasPreviousPage = ref(false)
const isLoading = ref(false)

const filters = reactive({
  PageNumber: 1,
  PageSize: 12
})

onMounted(async () => {
  await fetchVenueInfo()
  await fetchSeatMaps()
})

const fetchVenueInfo = async () => {
  try {
    const res = await getModeratorVenueById(venueId)
    if (res && res.success) {
      venue.value = res.data
    }
  } catch (err) {
    console.error('Lỗi khi tải thông tin địa điểm:', err)
    addToast(getErrorMessage(err, 'Không thể tải thông tin địa điểm.'), 'error')
  }
}

const fetchSeatMaps = async () => {
  isLoading.value = true
  try {
    const res = await getModeratorVenueSeatMaps(venueId, filters)
    if (res && res.success && res.data) {
      seatMaps.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      hasNextPage.value = res.data.hasNextPage || false
      hasPreviousPage.value = res.data.hasPreviousPage || false
    }
  } catch (err) {
    console.error('Lỗi khi tải danh sách sơ đồ chỗ ngồi:', err)
    addToast(getErrorMessage(err, 'Không thể tải danh sách sơ đồ chỗ ngồi.'), 'error')
  } finally {
    isLoading.value = false
  }
}

// Pagination
const prevPage = () => {
  if (hasPreviousPage.value) {
    filters.PageNumber--
    fetchSeatMaps()
  }
}

const nextPage = () => {
  if (hasNextPage.value) {
    filters.PageNumber++
    fetchSeatMaps()
  }
}
</script>
