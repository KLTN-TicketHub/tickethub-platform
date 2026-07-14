<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Yêu cầu giải ngân</h1>
      <p class="text-white/50 font-medium text-lg">Xem xét và đề xuất % hoa hồng áp dụng cho các sự kiện đã kết thúc</p>
    </div>

    <!-- Table Container -->
    <div class="flex flex-col gap-4">
      <div v-if="isLoading" class="py-20 flex flex-col items-center justify-center gap-3">
        <PhSpinner class="animate-spin text-primary text-4xl" weight="bold" />
        <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách yêu cầu...</span>
      </div>

      <template v-else>
        <BaseTable :columns="columns" :data="requestsList">
          <template #eventTitle="{ row }">
            <div class="flex flex-col gap-1">
              <div class="flex items-center gap-2">
                <span class="font-bold text-white line-clamp-1">{{ row.eventTitle }}</span>
                <span v-if="row.isResubmitted" class="shrink-0 px-2 py-0.5 rounded-full bg-warning/20 border border-warning/30 text-warning text-[10px] font-black uppercase tracking-wider">
                  Gửi lại
                </span>
              </div>
              <span v-if="row.isResubmitted && row.lastRejectionReason" class="text-[12px] text-danger/80 font-medium line-clamp-1" :title="row.lastRejectionReason">
                Lý do từ chối trước: {{ row.lastRejectionReason }}
              </span>
            </div>
          </template>
          <template #categoryName="{ row }">
            <span class="px-2.5 py-1 rounded-full bg-white/5 text-xs font-bold text-white/60">{{ row.categoryName || 'Không rõ' }}</span>
          </template>
          <template #organizerName="{ row }">
            <span class="text-white/80 font-medium">{{ row.organizerName || 'Không rõ' }}</span>
          </template>
          <template #grossAmount="{ row }">
            <span class="font-black text-white">{{ formatCurrency(row.grossAmount) }}</span>
          </template>
          <template #orderCount="{ row }">
            <span class="text-white/70 font-bold">{{ row.orderCount }}</span>
          </template>
          <template #recommendedRate="{ row }">
            <span class="font-bold text-primary">{{ row.recommendedRate }}%</span>
          </template>
          <template #requestedAt="{ row }">
            <span class="text-white/50 text-[13px] font-medium">{{ formatDateString(row.requestedAt) }}</span>
          </template>
          <template #actions="{ row }">
            <div class="flex justify-end">
              <BaseButton variant="primary" size="sm" class="!rounded-xl" @click="openProposeModal(row)">
                Đề xuất giải ngân
              </BaseButton>
            </div>
          </template>
        </BaseTable>

        <div v-if="requestsList.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
            <PhHandCoins weight="duotone" />
          </div>
          <h3 class="text-xl font-bold font-heading text-white mb-2">Không có yêu cầu nào</h3>
          <p class="text-white/50 max-w-xs">Hiện chưa có Organizer nào gửi yêu cầu giải ngân.</p>
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
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
      </template>
    </div>

    <!-- Propose Modal -->
    <div v-if="isModalOpen" class="fixed inset-0 z-[10000] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="closeModal"></div>

      <div class="relative bg-card/90 backdrop-blur-2xl border border-border-main rounded-[32px] w-full max-w-lg overflow-hidden shadow-2xl shadow-black/60 p-8 animate-in zoom-in-95 fade-in duration-300">
        <h3 class="text-2xl font-bold text-main mb-1 font-heading">Đề xuất giải ngân</h3>
        <p class="text-white/50 text-sm mb-6 line-clamp-1">{{ selectedRequest?.eventTitle }}</p>

        <div v-if="selectedRequest?.isResubmitted" class="mb-6 p-4 rounded-2xl bg-warning/10 border border-warning/20 flex gap-3">
          <PhWarningCircle class="text-warning text-xl shrink-0" weight="fill" />
          <div class="flex flex-col gap-1">
            <span class="font-bold text-warning text-sm">Organizer đã từ chối đề xuất trước đó</span>
            <p v-if="selectedRequest.lastRejectionReason" class="text-white/70 text-sm leading-relaxed">{{ selectedRequest.lastRejectionReason }}</p>
            <p v-else class="text-white/50 text-sm italic">Không có lý do cụ thể.</p>
          </div>
        </div>

        <div class="space-y-4 mb-6">
          <div class="flex items-center justify-between py-3 border-b border-white/5">
            <span class="text-white/50 text-sm font-medium">Doanh thu gộp</span>
            <span class="font-black text-white">{{ formatCurrency(selectedRequest?.grossAmount) }}</span>
          </div>
          <div class="flex items-center justify-between py-3 border-b border-white/5">
            <span class="text-white/50 text-sm font-medium">% hoa hồng tham khảo</span>
            <span class="font-bold text-primary">{{ selectedRequest?.recommendedRate }}%</span>
          </div>
        </div>

        <form @submit.prevent="handleSubmitPropose" class="space-y-6">
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">% hoa hồng áp dụng</label>
            <input
              v-model.number="appliedRate"
              type="number"
              min="0"
              max="100"
              step="0.01"
              required
              class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20"
            />
          </div>

          <div class="space-y-2 p-4 rounded-2xl bg-white/5 border border-white/10">
            <div class="flex items-center justify-between text-sm">
              <span class="text-white/50">Phí hoa hồng</span>
              <span class="font-bold text-danger">-{{ formatCurrency(previewFeeAmount) }}</span>
            </div>
            <div class="flex items-center justify-between text-sm pt-2 border-t border-white/10">
              <span class="text-white/70 font-bold">Organizer thực nhận</span>
              <span class="font-black text-primary">{{ formatCurrency(previewNetAmount) }}</span>
            </div>
          </div>

          <div class="flex gap-3 pt-2">
            <BaseButton type="button" variant="outline" class="flex-1 !rounded-2xl" @click="closeModal">
              Hủy bỏ
            </BaseButton>
            <BaseButton type="submit" variant="primary" class="flex-1 !rounded-2xl" :disabled="isSubmitting">
              <PhSpinner v-if="isSubmitting" class="animate-spin text-lg" />
              <span v-else>Xác nhận đề xuất</span>
            </BaseButton>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { addToast } from '../../stores/adminStore'
import { getPayoutRequests, proposePayout } from '../../services/moderator-payout.service'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import { PhSpinner, PhCaretLeft, PhCaretRight, PhHandCoins, PhWarningCircle } from '@phosphor-icons/vue'

const isLoading = ref(true)
const requestsList = ref([])

const currentPage = ref(1)
const totalPages = ref(1)
const pageSize = 10

const columns = [
  { key: 'eventTitle', label: 'Sự kiện' },
  { key: 'categoryName', label: 'Danh mục' },
  { key: 'organizerName', label: 'Organizer' },
  { key: 'grossAmount', label: 'Doanh thu gộp' },
  { key: 'orderCount', label: 'Số đơn' },
  { key: 'recommendedRate', label: '% tham khảo' },
  { key: 'requestedAt', label: 'Ngày yêu cầu' },
  { key: 'actions', label: '', class: 'w-48' },
]

const isModalOpen = ref(false)
const isSubmitting = ref(false)
const selectedRequest = ref(null)
const appliedRate = ref(0)

const fetchRequests = async () => {
  isLoading.value = true
  try {
    const res = await getPayoutRequests({ pageNumber: currentPage.value, pageSize })
    if (res && res.success && res.data) {
      requestsList.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      currentPage.value = res.data.pageNumber || 1
    } else {
      requestsList.value = []
      totalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching payout requests:', err)
    requestsList.value = []
    addToast('Không thể tải danh sách yêu cầu giải ngân.', 'error')
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchRequests()
})

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  fetchRequests()
}

const openProposeModal = (row) => {
  selectedRequest.value = row
  appliedRate.value = row.recommendedRate
  isModalOpen.value = true
}

const closeModal = () => {
  isModalOpen.value = false
  selectedRequest.value = null
}

const previewFeeAmount = computed(() => {
  if (!selectedRequest.value) return 0
  return Math.round(selectedRequest.value.grossAmount * (appliedRate.value || 0) / 100)
})

const previewNetAmount = computed(() => {
  if (!selectedRequest.value) return 0
  return selectedRequest.value.grossAmount - previewFeeAmount.value
})

const handleSubmitPropose = async () => {
  if (appliedRate.value < 0 || appliedRate.value > 100) {
    addToast('Phần trăm hoa hồng áp dụng phải nằm trong khoảng từ 0 đến 100.', 'error')
    return
  }

  isSubmitting.value = true
  try {
    const res = await proposePayout(selectedRequest.value.payoutRequestId, Number(appliedRate.value))
    if (res && res.success) {
      addToast('Đã đề xuất giải ngân thành công.', 'success')
      closeModal()
      await fetchRequests()
    } else {
      addToast(res?.message || 'Đề xuất giải ngân thất bại.', 'error')
    }
  } catch (err) {
    console.error('Error proposing payout:', err)
    const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra khi đề xuất giải ngân.'
    addToast(errorMsg, 'error')
  } finally {
    isSubmitting.value = false
  }
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

const formatCurrency = (val) => {
  if (val === undefined || val === null) return '0 ₫'
  return val.toLocaleString('vi-VN') + ' ₫'
}

const formatDateString = (dateStr) => {
  if (!dateStr) return '--'
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
  } catch (e) {
    return dateStr
  }
}
</script>

<style scoped>
</style>
