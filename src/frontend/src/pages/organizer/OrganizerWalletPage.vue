<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Ví của tôi</h1>
      <p class="text-white/50 font-medium text-lg">Theo dõi số dư, lịch sử giao dịch và các đề xuất giải ngân</p>
    </div>

    <!-- Balance Card -->
    <div class="bg-gradient-to-br from-primary/20 via-[#111916] to-[#111916] border border-primary/20 rounded-[2.5rem] p-8 md:p-10 relative overflow-hidden">
      <div class="absolute -right-10 -top-10 w-40 h-40 bg-primary/10 rounded-full blur-3xl"></div>
      <div class="flex items-center gap-3 mb-3">
        <div class="w-12 h-12 rounded-2xl bg-primary/10 flex items-center justify-center text-primary text-2xl">
          <PhWallet weight="duotone" />
        </div>
        <span class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Số dư khả dụng</span>
      </div>
      <div v-if="isLoadingWallet" class="h-12 w-48 bg-white/5 animate-pulse rounded-lg"></div>
      <span v-else class="text-4xl md:text-5xl font-black text-white font-heading">{{ formatCurrency(wallet?.balance) }}</span>
    </div>

    <!-- Tabs -->
    <div class="flex bg-[#0A0F0D] p-1.5 rounded-xl border border-white/5 self-start">
      <button
        @click="activeTab = 'transactions'"
        class="px-5 py-2.5 rounded-lg text-sm font-bold transition-all cursor-pointer"
        :class="activeTab === 'transactions' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
      >
        Lịch sử giao dịch
      </button>
      <button
        @click="activeTab = 'proposed'"
        class="px-5 py-2.5 rounded-lg text-sm font-bold transition-all cursor-pointer"
        :class="activeTab === 'proposed' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
      >
        Đề xuất chờ xác nhận
      </button>
    </div>

    <!-- Transactions Tab -->
    <div v-if="activeTab === 'transactions'" class="flex flex-col gap-4">
      <div v-if="isLoadingTx" class="py-20 flex flex-col items-center justify-center gap-3">
        <PhSpinner class="animate-spin text-primary text-4xl" weight="bold" />
        <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải lịch sử giao dịch...</span>
      </div>

      <template v-else>
        <BaseTable :columns="txColumns" :data="txList">
          <template #eventTitle="{ row }">
            <span class="font-bold text-white line-clamp-1">{{ row.eventTitle || '--' }}</span>
          </template>
          <template #type="{ row }">
            <span class="px-2.5 py-1 rounded-full text-[10px] font-black uppercase tracking-wider" :class="getTypeBadgeClass(row.type)">
              {{ getTypeText(row.type) }}
            </span>
          </template>
          <template #amount="{ row }">
            <span class="font-black" :class="row.type === 'Fee' ? 'text-danger' : 'text-primary'">
              {{ row.type === 'Fee' ? '-' : '+' }}{{ formatCurrency(row.amount) }}
            </span>
          </template>
          <template #status="{ row }">
            <span class="px-2.5 py-1 rounded-full text-[10px] font-black uppercase tracking-wider" :class="getStatusBadgeClass(row.status)">
              {{ getStatusText(row.status) }}
            </span>
          </template>
          <template #createdAt="{ row }">
            <span class="text-white/50 text-[13px] font-medium">{{ formatDateString(row.createdAt) }}</span>
          </template>
        </BaseTable>

        <div v-if="txList.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
            <PhReceipt weight="duotone" />
          </div>
          <h3 class="text-xl font-bold font-heading text-white mb-2">Chưa có giao dịch nào</h3>
        </div>

        <div v-if="txTotalPages > 1" class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
          <div class="text-[12px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ txPage }}</span> / <span class="text-white font-bold">{{ txTotalPages }}</span>
          </div>
          <div class="flex items-center gap-2">
            <button @click="changeTxPage(txPage - 1)" :disabled="txPage === 1" class="px-3 py-1.5 rounded-lg text-[12px] font-bold text-white/50 hover:text-white hover:bg-white/5 disabled:opacity-30 disabled:hover:bg-transparent transition-all cursor-pointer">Trước</button>
            <button @click="changeTxPage(txPage + 1)" :disabled="txPage === txTotalPages" class="px-3 py-1.5 rounded-lg text-[12px] font-bold text-white/50 hover:text-white hover:bg-white/5 disabled:opacity-30 disabled:hover:bg-transparent transition-all cursor-pointer">Sau</button>
          </div>
        </div>
      </template>
    </div>

    <!-- Proposed Payouts Tab -->
    <div v-else class="flex flex-col gap-4">
      <div v-if="isLoadingProposed" class="py-20 flex flex-col items-center justify-center gap-3">
        <PhSpinner class="animate-spin text-primary text-4xl" weight="bold" />
        <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách đề xuất...</span>
      </div>

      <template v-else>
        <div v-if="proposedList.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
            <PhHandCoins weight="duotone" />
          </div>
          <h3 class="text-xl font-bold font-heading text-white mb-2">Không có đề xuất nào</h3>
          <p class="text-white/50 max-w-xs">Khi Moderator đề xuất giải ngân, nó sẽ hiện ở đây để bạn xác nhận.</p>
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-5">
          <div v-for="item in proposedList" :key="item.id" class="bg-[#111916] border border-white/5 rounded-[2rem] p-6 flex flex-col gap-4 hover:border-primary/20 transition-all">
            <div class="flex items-start justify-between gap-3">
              <h3 class="font-bold text-white text-lg line-clamp-2">{{ item.eventTitle }}</h3>
              <span class="shrink-0 px-2.5 py-1 rounded-full bg-warning/20 border border-warning/30 text-warning text-[10px] font-black uppercase tracking-wider">Chờ xác nhận</span>
            </div>

            <div class="space-y-2 text-sm">
              <div class="flex items-center justify-between">
                <span class="text-white/50">Doanh thu gộp</span>
                <span class="font-bold text-white">{{ formatCurrency(item.grossAmount) }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-white/50">% hoa hồng áp dụng</span>
                <span class="font-bold text-white">{{ item.appliedRate }}%</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-white/50">Phí hoa hồng</span>
                <span class="font-bold text-danger">-{{ formatCurrency(item.feeAmount) }}</span>
              </div>
              <div class="flex items-center justify-between pt-2 border-t border-white/5">
                <span class="text-white/70 font-bold">Thực nhận</span>
                <span class="font-black text-primary text-lg">{{ formatCurrency(item.netAmount) }}</span>
              </div>
            </div>

            <div class="flex gap-3 mt-2">
              <BaseButton variant="outline" class="flex-1 !rounded-2xl !border-danger/30 !text-danger hover:!bg-danger/10" :disabled="acceptingId === item.id" @click="openRejectModal(item)">
                Từ chối
              </BaseButton>
              <BaseButton variant="primary" class="flex-1 !rounded-2xl" :disabled="acceptingId === item.id" @click="confirmAccept(item)">
                <PhSpinner v-if="acceptingId === item.id" class="animate-spin text-lg" />
                <span v-else>Chấp nhận</span>
              </BaseButton>
            </div>
          </div>
        </div>

        <div v-if="proposedTotalPages > 1" class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
          <div class="text-[12px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ proposedPage }}</span> / <span class="text-white font-bold">{{ proposedTotalPages }}</span>
          </div>
          <div class="flex items-center gap-2">
            <button @click="changeProposedPage(proposedPage - 1)" :disabled="proposedPage === 1" class="px-3 py-1.5 rounded-lg text-[12px] font-bold text-white/50 hover:text-white hover:bg-white/5 disabled:opacity-30 disabled:hover:bg-transparent transition-all cursor-pointer">Trước</button>
            <button @click="changeProposedPage(proposedPage + 1)" :disabled="proposedPage === proposedTotalPages" class="px-3 py-1.5 rounded-lg text-[12px] font-bold text-white/50 hover:text-white hover:bg-white/5 disabled:opacity-30 disabled:hover:bg-transparent transition-all cursor-pointer">Sau</button>
          </div>
        </div>
      </template>
    </div>

    <!-- Reject Modal -->
    <div v-if="isRejectModalOpen" class="fixed inset-0 z-[10000] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="closeRejectModal"></div>

      <div class="relative bg-card/90 backdrop-blur-2xl border border-border-main rounded-[32px] w-full max-w-lg overflow-hidden shadow-2xl shadow-black/60 p-8 animate-in zoom-in-95 fade-in duration-300">
        <h3 class="text-2xl font-bold text-main mb-1 font-heading">Từ chối đề xuất giải ngân</h3>
        <p class="text-white/50 text-sm mb-6 line-clamp-1">{{ rejectingItem?.eventTitle }}</p>

        <form @submit.prevent="handleSubmitReject" class="space-y-6">
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Lý do từ chối (không bắt buộc)</label>
            <textarea
              v-model="rejectReason"
              placeholder="VD: % hoa hồng áp dụng chưa hợp lý, cần Moderator xem xét lại..."
              rows="3"
              class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-danger/50 transition-all placeholder:text-white/20 resize-none"
            ></textarea>
            <p class="text-white/40 text-xs">Yêu cầu sẽ được chuyển lại cho Moderator xem xét lại từ đầu.</p>
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
import { ref, onMounted, watch } from 'vue'
import { addToast, openConfirm } from '../../stores/adminStore'
import { getWallet, getWalletTransactions, getProposedPayouts, acceptPayout, rejectPayout } from '../../services/organizer-wallet.service'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import { PhSpinner, PhWallet, PhReceipt, PhHandCoins } from '@phosphor-icons/vue'

const activeTab = ref('transactions')

// Wallet balance
const wallet = ref(null)
const isLoadingWallet = ref(true)

// Transactions
const isLoadingTx = ref(true)
const txList = ref([])
const txPage = ref(1)
const txTotalPages = ref(1)
const txPageSize = 10

const txColumns = [
  { key: 'eventTitle', label: 'Sự kiện' },
  { key: 'type', label: 'Loại' },
  { key: 'amount', label: 'Số tiền' },
  { key: 'status', label: 'Trạng thái' },
  { key: 'createdAt', label: 'Thời gian' },
]

// Proposed payouts
const isLoadingProposed = ref(true)
const proposedList = ref([])
const proposedPage = ref(1)
const proposedTotalPages = ref(1)
const proposedPageSize = 6
const acceptingId = ref(null)

// Reject modal
const isRejectModalOpen = ref(false)
const isRejecting = ref(false)
const rejectingItem = ref(null)
const rejectReason = ref('')

const fetchWallet = async () => {
  isLoadingWallet.value = true
  try {
    const res = await getWallet()
    if (res && res.success) {
      wallet.value = res.data
    }
  } catch (err) {
    console.error('Error fetching wallet:', err)
    addToast('Không thể tải thông tin ví.', 'error')
  } finally {
    isLoadingWallet.value = false
  }
}

const fetchTransactions = async () => {
  isLoadingTx.value = true
  try {
    const res = await getWalletTransactions({ pageNumber: txPage.value, pageSize: txPageSize })
    if (res && res.success && res.data) {
      txList.value = res.data.data || []
      txTotalPages.value = res.data.totalPages || 1
      txPage.value = res.data.pageNumber || 1
    } else {
      txList.value = []
      txTotalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching wallet transactions:', err)
    txList.value = []
    addToast('Không thể tải lịch sử giao dịch.', 'error')
  } finally {
    isLoadingTx.value = false
  }
}

const fetchProposedPayouts = async () => {
  isLoadingProposed.value = true
  try {
    const res = await getProposedPayouts({ pageNumber: proposedPage.value, pageSize: proposedPageSize })
    if (res && res.success && res.data) {
      proposedList.value = res.data.data || []
      proposedTotalPages.value = res.data.totalPages || 1
      proposedPage.value = res.data.pageNumber || 1
    } else {
      proposedList.value = []
      proposedTotalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching proposed payouts:', err)
    proposedList.value = []
    addToast('Không thể tải danh sách đề xuất giải ngân.', 'error')
  } finally {
    isLoadingProposed.value = false
  }
}

onMounted(() => {
  fetchWallet()
  fetchTransactions()
  fetchProposedPayouts()
})

watch(activeTab, (tab) => {
  if (tab === 'proposed' && proposedList.value.length === 0) {
    fetchProposedPayouts()
  }
})

const changeTxPage = (page) => {
  if (page < 1 || page > txTotalPages.value) return
  txPage.value = page
  fetchTransactions()
}

const changeProposedPage = (page) => {
  if (page < 1 || page > proposedTotalPages.value) return
  proposedPage.value = page
  fetchProposedPayouts()
}

const confirmAccept = (item) => {
  openConfirm(
    'Chấp nhận giải ngân',
    `Xác nhận chấp nhận đề xuất giải ngân ${formatCurrency(item.netAmount)} cho sự kiện "${item.eventTitle}"? Số tiền sẽ được cộng ngay vào ví của bạn.`,
    async () => {
      acceptingId.value = item.id
      try {
        const res = await acceptPayout(item.id)
        if (res && res.success) {
          addToast('Đã chấp nhận giải ngân thành công.', 'success')
          await Promise.all([fetchWallet(), fetchProposedPayouts(), fetchTransactions()])
        } else {
          addToast(res?.message || 'Chấp nhận giải ngân thất bại.', 'error')
        }
      } catch (err) {
        console.error('Error accepting payout:', err)
        const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra khi chấp nhận giải ngân.'
        addToast(errorMsg, 'error')
      } finally {
        acceptingId.value = null
      }
    }
  )
}

const openRejectModal = (item) => {
  rejectingItem.value = item
  rejectReason.value = ''
  isRejectModalOpen.value = true
}

const closeRejectModal = () => {
  isRejectModalOpen.value = false
  rejectingItem.value = null
}

const handleSubmitReject = async () => {
  isRejecting.value = true
  try {
    const res = await rejectPayout(rejectingItem.value.id, rejectReason.value.trim() || undefined)
    if (res && res.success) {
      addToast('Đã từ chối đề xuất giải ngân.', 'success')
      closeRejectModal()
      await fetchProposedPayouts()
    } else {
      addToast(res?.message || 'Từ chối đề xuất giải ngân thất bại.', 'error')
    }
  } catch (err) {
    console.error('Error rejecting payout:', err)
    const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra khi từ chối đề xuất giải ngân.'
    addToast(errorMsg, 'error')
  } finally {
    isRejecting.value = false
  }
}

const getTypeBadgeClass = (type) => {
  if (type === 'Revenue') return 'bg-primary/20 border border-primary/30 text-primary'
  if (type === 'Fee') return 'bg-danger/20 border border-danger/30 text-danger'
  return 'bg-white/5 border border-white/10 text-white/55'
}

const getTypeText = (type) => {
  if (type === 'Revenue') return 'Doanh thu'
  if (type === 'Fee') return 'Phí hoa hồng'
  if (type === 'Payout') return 'Giải ngân'
  return type || 'Không rõ'
}

const getStatusBadgeClass = (status) => {
  if (status === 'Success') return 'bg-primary/20 border border-primary/30 text-primary'
  if (status === 'Pending') return 'bg-warning/20 border border-warning/30 text-warning'
  if (status === 'Failed') return 'bg-danger/20 border border-danger/30 text-danger'
  return 'bg-white/5 border border-white/10 text-white/55'
}

const getStatusText = (status) => {
  if (status === 'Success') return 'Thành công'
  if (status === 'Pending') return 'Chờ xử lý'
  if (status === 'Failed') return 'Thất bại'
  return status || 'Không rõ'
}

const formatCurrency = (val) => {
  if (val === undefined || val === null) return '0 ₫'
  return val.toLocaleString('vi-VN') + ' ₫'
}

const formatDateString = (dateStr) => {
  if (!dateStr) return '--'
  try {
    const date = new Date(dateStr)
    const time = date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false })
    const day = date.getDate().toString().padStart(2, '0')
    const month = (date.getMonth() + 1).toString().padStart(2, '0')
    const year = date.getFullYear()
    return `${time}, ${day}/${month}/${year}`
  } catch (e) {
    return dateStr
  }
}
</script>

<style scoped>
</style>
