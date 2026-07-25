<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Yêu cầu hủy sự kiện</h1>
      <p class="text-white/50 font-medium text-lg">Xem xét các yêu cầu hủy sự kiện do Organizer gửi lên</p>
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
            <span class="font-bold text-white line-clamp-1">{{ row.eventTitle }}</span>
          </template>
          <template #organizerName="{ row }">
            <span class="text-white/80 font-medium">{{ row.organizerName || 'Không rõ' }}</span>
          </template>
          <template #reason="{ row }">
            <span class="text-white/60 text-[13px] line-clamp-2 max-w-xs" :title="row.reason">{{ row.reason }}</span>
          </template>
          <template #createdAt="{ row }">
            <span class="text-white/50 text-[13px] font-medium">{{ formatDateString(row.createdAt) }}</span>
          </template>
          <template #actions="{ row }">
            <div class="flex justify-end gap-2">
              <button
                @click="openRejectModal(row)"
                class="px-4 py-2 rounded-xl bg-danger/10 text-danger border border-danger/20 text-[13px] font-bold hover:bg-danger hover:text-white transition-all cursor-pointer"
              >
                Từ chối
              </button>
              <BaseButton variant="primary" size="sm" class="!rounded-xl" @click="confirmApprove(row)">
                Duyệt
              </BaseButton>
            </div>
          </template>
        </BaseTable>

        <div v-if="requestsList.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
            <PhProhibit weight="duotone" />
          </div>
          <h3 class="text-xl font-bold font-heading text-white mb-2">Không có yêu cầu nào</h3>
          <p class="text-white/50 max-w-xs">Hiện chưa có Organizer nào gửi yêu cầu hủy sự kiện.</p>
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

    <!-- Reject Modal -->
    <div v-if="isRejectModalOpen" class="fixed inset-0 z-[10000] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="closeRejectModal"></div>

      <div class="relative bg-card/90 backdrop-blur-2xl border border-border-main rounded-[32px] w-full max-w-lg overflow-hidden shadow-2xl shadow-black/60 p-8 animate-in zoom-in-95 fade-in duration-300">
        <h3 class="text-2xl font-bold text-main mb-1 font-heading">Từ chối yêu cầu hủy sự kiện</h3>
        <p class="text-white/50 text-sm mb-6 line-clamp-1">{{ rejectingItem?.eventTitle }}</p>

        <form @submit.prevent="submitReject" class="space-y-6">
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Lý do từ chối <span class="text-danger">*</span></label>
            <textarea
              v-model="rejectReason"
              placeholder="VD: Sự kiện vẫn có thể tổ chức bình thường, chưa đủ căn cứ để hủy..."
              rows="4"
              class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-danger/50 transition-all placeholder:text-white/20 resize-none"
            ></textarea>
            <span v-if="rejectError" class="text-danger text-xs font-bold">{{ rejectError }}</span>
          </div>

          <div class="flex gap-3 pt-2">
            <BaseButton type="button" variant="outline" class="flex-1 !rounded-2xl" @click="closeRejectModal">
              Hủy bỏ
            </BaseButton>
            <BaseButton type="submit" variant="primary" class="flex-1 !rounded-2xl !bg-danger hover:!bg-danger/80" :disabled="isRejecting">
              <PhSpinner v-if="isRejecting" class="animate-spin text-lg" />
              <span v-else>Xác nhận từ chối</span>
            </BaseButton>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { addToast, openConfirm } from '../../stores/adminStore'
import { getEventCancellationRequests, reviewEventCancellationRequest } from '../../services/eventService'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import { PhSpinner, PhCaretLeft, PhCaretRight, PhProhibit } from '@phosphor-icons/vue'

const isLoading = ref(true)
const requestsList = ref([])

const currentPage = ref(1)
const totalPages = ref(1)
const pageSize = 10

const columns = [
  { key: 'eventTitle', label: 'Sự kiện' },
  { key: 'organizerName', label: 'Organizer' },
  { key: 'reason', label: 'Lý do' },
  { key: 'createdAt', label: 'Ngày yêu cầu' },
  { key: 'actions', label: '', class: 'w-48' },
]

const isRejectModalOpen = ref(false)
const isRejecting = ref(false)
const rejectingItem = ref(null)
const rejectReason = ref('')
const rejectError = ref('')

const fetchRequests = async () => {
  isLoading.value = true
  try {
    const res = await getEventCancellationRequests({ pageNumber: currentPage.value, pageSize })
    if (res && res.success && res.data) {
      requestsList.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      currentPage.value = res.data.pageNumber || 1
    } else {
      requestsList.value = []
      totalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching event cancellation requests:', err)
    requestsList.value = []
    addToast('Không thể tải danh sách yêu cầu hủy sự kiện.', 'error')
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

const confirmApprove = (row) => {
  openConfirm(
    'Duyệt yêu cầu hủy sự kiện',
    `Xác nhận duyệt yêu cầu hủy sự kiện "${row.eventTitle}"? Sự kiện sẽ bị hủy ngay và toàn bộ đơn hàng chưa check-in sẽ được tự động hoàn tiền.`,
    async () => {
      try {
        const res = await reviewEventCancellationRequest(row.id, { isApproved: true })
        if (res && res.success) {
          addToast('Đã duyệt yêu cầu hủy sự kiện thành công.', 'success')
          await fetchRequests()
        } else {
          addToast(res?.message || 'Duyệt yêu cầu thất bại.', 'error')
        }
      } catch (err) {
        console.error('Error approving cancellation request:', err)
        addToast(err.response?.data?.message || 'Có lỗi xảy ra khi duyệt yêu cầu.', 'error')
      }
    }
  )
}

const openRejectModal = (row) => {
  rejectingItem.value = row
  rejectReason.value = ''
  rejectError.value = ''
  isRejectModalOpen.value = true
}

const closeRejectModal = () => {
  isRejectModalOpen.value = false
  rejectingItem.value = null
}

const submitReject = async () => {
  if (!rejectReason.value.trim()) {
    rejectError.value = 'Lý do từ chối không được để trống.'
    return
  }

  isRejecting.value = true
  try {
    const res = await reviewEventCancellationRequest(rejectingItem.value.id, { isApproved: false, reason: rejectReason.value.trim() })
    if (res && res.success) {
      addToast('Đã từ chối yêu cầu hủy sự kiện.', 'success')
      closeRejectModal()
      await fetchRequests()
    } else {
      addToast(res?.message || 'Từ chối yêu cầu thất bại.', 'error')
    }
  } catch (err) {
    console.error('Error rejecting cancellation request:', err)
    addToast(err.response?.data?.message || 'Có lỗi xảy ra khi từ chối yêu cầu.', 'error')
  } finally {
    isRejecting.value = false
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
