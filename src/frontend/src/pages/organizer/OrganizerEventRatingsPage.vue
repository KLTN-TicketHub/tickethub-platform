<template>
  <div v-if="isLoading" class="flex flex-col pb-20 bg-[#0A0F0D] min-h-[80vh] items-center justify-center">
    <div class="flex flex-col items-center gap-4">
      <PhSpinner class="animate-spin text-primary text-5xl" weight="bold" />
      <span class="text-white/50 text-sm font-bold uppercase tracking-widest">Đang tải đánh giá sự kiện...</span>
    </div>
  </div>

  <div v-else-if="error" class="flex flex-col items-center justify-center py-32 px-6 text-center min-h-[80vh] bg-[#0A0F0D]">
    <div class="w-24 h-24 bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 mb-6 shadow-inner">
      <PhWarningCircle weight="duotone" />
    </div>
    <h2 class="font-heading text-3xl font-black text-white mb-3">Lỗi tải đánh giá</h2>
    <p class="text-white/50 max-w-md mx-auto mb-10 font-medium">{{ error }}</p>
    <BaseButton variant="primary" @click="router.push('/organizer')">Quay lại Tổng quan</BaseButton>
  </div>

  <div v-else class="flex flex-col pb-20 bg-[#0A0F0D] min-h-screen">
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 pt-8 w-full">
      <button
        @click="router.back()"
        class="inline-flex items-center gap-2 text-white/60 hover:text-primary transition-colors font-bold text-[14px] mb-6 cursor-pointer group"
      >
        <PhArrowLeft class="group-hover:-translate-x-1 transition-transform" weight="bold" />
        Quay lại
      </button>

      <!-- Event Header Card -->
      <div class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 flex flex-col md:flex-row gap-6 items-center justify-between shadow-2xl mb-12">
        <div class="flex items-center gap-6">
          <div class="w-20 h-20 md:w-24 md:h-24 rounded-2xl overflow-hidden border border-white/5 bg-[#0A0F0D] flex-shrink-0">
            <img
              :src="eventInfo?.coverImageUrl || 'https://picsum.photos/seed/event-placeholder/200/200'"
              class="w-full h-full object-cover"
              @error="handleImageError"
            />
          </div>
          <div class="space-y-1">
            <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">Đánh giá từ khán giả</span>
            <h1 class="text-2xl md:text-3xl font-black font-heading text-white tracking-tight line-clamp-1 uppercase">
              {{ eventInfo?.title }}
            </h1>
          </div>
        </div>

        <div class="flex flex-col items-end gap-1 shrink-0">
          <span class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Tổng số đánh giá</span>
          <span class="text-3xl font-black text-primary font-heading">{{ totalCount }}</span>
        </div>
      </div>

      <!-- Ratings Table -->
      <section class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 shadow-2xl">
        <h2 class="font-heading text-2xl font-black text-white uppercase tracking-wider mb-6">Danh sách đánh giá</h2>

        <div v-if="isLoadingRatings" class="py-12 flex flex-col items-center justify-center gap-3">
          <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
          <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách đánh giá...</span>
        </div>

        <div v-else-if="ratings.length === 0" class="py-12 text-center text-white/30 font-bold text-[14px]">
          Sự kiện này chưa có đánh giá nào.
        </div>

        <div v-else class="overflow-x-auto">
          <BaseTable :columns="columns" :data="ratings">
            <template #reviewerName="{ row }">
              <span class="font-bold text-white/90">{{ row.reviewerName }}</span>
            </template>
            <template #createdAt="{ row }">
              <span class="text-white/60 text-[13px]">{{ formatDate(row.createdAt) }}</span>
            </template>
            <template #overallRating="{ row }">
              <span class="font-black text-primary flex items-center gap-1">
                <PhStar weight="fill" /> {{ row.overallRating.toFixed(1) }}
              </span>
            </template>
            <template #comment="{ row }">
              <span class="text-white/70 text-[13px] line-clamp-2 max-w-xs block">{{ row.comment || '—' }}</span>
            </template>
          </BaseTable>
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="flex items-center justify-between mt-8 pt-6 border-t border-white/5">
          <div class="text-[12px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ currentPage }}</span> / <span class="text-white font-bold">{{ totalPages }}</span>
          </div>

          <div class="flex items-center gap-2">
            <button
              @click="changePage(currentPage - 1)"
              :disabled="currentPage === 1"
              class="group flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <PhCaretLeft weight="bold" class="text-[14px] text-white/50 group-hover:text-white group-hover:-translate-x-0.5 transition-all" />
              <span class="text-[12px] font-bold text-white/50 group-hover:text-white transition-colors">Trước</span>
            </button>

            <div class="flex items-center gap-1">
              <template v-for="(page, index) in computedVisiblePages" :key="index">
                <span v-if="page === '...'" class="w-8 text-center text-white/30 font-bold tracking-widest text-[12px]">...</span>
                <button
                  v-else
                  @click="changePage(page)"
                  class="w-8 h-8 flex items-center justify-center rounded-lg font-bold text-[12px] transition-all cursor-pointer"
                  :class="currentPage === page ? 'text-black bg-primary shadow-lg' : 'text-white/60 hover:text-white hover:bg-white/5'"
                >
                  {{ page }}
                </button>
              </template>
            </div>

            <button
              @click="changePage(currentPage + 1)"
              :disabled="currentPage === totalPages"
              class="group flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <span class="text-[12px] font-bold text-white/50 group-hover:text-white transition-colors">Sau</span>
              <PhCaretRight weight="bold" class="text-[14px] text-white/50 group-hover:text-white group-hover:translate-x-0.5 transition-all" />
            </button>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getOrganizerEventDetail } from '../../services/eventService'
import { getOrganizerEventRatings } from '../../services/rating.service'
import { addToast } from '../../stores/adminStore'
import { getErrorMessage } from '../../utils/apiError'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import {
  PhSpinner, PhWarningCircle, PhArrowLeft,
  PhCaretLeft, PhCaretRight, PhStar
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const eventInfo = ref(null)
const isLoading = ref(true)
const error = ref('')

const ratings = ref([])
const isLoadingRatings = ref(false)
const currentPage = ref(1)
const totalPages = ref(1)
const totalCount = ref(0)
const pageSize = 10

const columns = [
  { key: 'reviewerName', label: 'Người đánh giá' },
  { key: 'createdAt', label: 'Ngày' },
  { key: 'soundRating', label: 'Âm thanh' },
  { key: 'visualRating', label: 'Hình ảnh' },
  { key: 'organizationRating', label: 'Tổ chức' },
  { key: 'facilityRating', label: 'Cơ sở vật chất' },
  { key: 'serviceRating', label: 'Dịch vụ' },
  { key: 'performanceRating', label: 'Biểu diễn' },
  { key: 'overallRating', label: 'Trung bình' },
  { key: 'comment', label: 'Bình luận' }
]

const fetchRatings = async () => {
  isLoadingRatings.value = true
  try {
    const res = await getOrganizerEventRatings(route.params.id, { pageNumber: currentPage.value, pageSize })
    if (res && res.success && res.data) {
      ratings.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      totalCount.value = res.data.totalCount || 0
      currentPage.value = res.data.pageNumber || 1
    } else {
      ratings.value = []
      totalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching event ratings:', err)
    const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra khi tải danh sách đánh giá.'
    addToast(errorMsg, 'error')
    ratings.value = []
  } finally {
    isLoadingRatings.value = false
  }
}

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  fetchRatings()
}

const computedVisiblePages = computed(() => {
  const current = currentPage.value
  const total = totalPages.value
  if (total <= 5) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }
  if (current <= 3) {
    return [1, 2, 3, '...', total]
  }
  if (current >= total - 2) {
    return [1, '...', total - 2, total - 1, total]
  }
  return [1, '...', current, '...', total]
})

onMounted(async () => {
  try {
    const res = await getOrganizerEventDetail(route.params.id)
    if (res && res.success) {
      eventInfo.value = res.data
    } else {
      error.value = res?.message || 'Không thể tải thông tin sự kiện.'
    }
  } catch (err) {
    console.error('Error fetching event detail:', err)
    error.value = getErrorMessage(err, 'Lỗi kết nối khi tải dữ liệu sự kiện.')
  } finally {
    isLoading.value = false
  }

  if (!error.value) {
    await fetchRatings()
  }
})

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
  } catch (e) {
    return dateStr
  }
}

const handleImageError = (e) => {
  e.target.src = 'https://picsum.photos/seed/event-placeholder/200/200'
}
</script>

<style scoped>
</style>
