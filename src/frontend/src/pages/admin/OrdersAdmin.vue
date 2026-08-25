<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Quản lý đơn hàng</h1>
      <p class="text-white/50 font-medium text-lg">Theo dõi và xử lý tất cả các giao dịch mua vé</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
      <div v-for="stat in orderStatCards" :key="stat.label" class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] flex flex-col items-center text-center group hover:-translate-y-1 transition-all">
        <span class="text-3xl font-heading font-black mb-1" :style="{ color: stat.color }">{{ stat.value }}</span>
        <span class="text-[10px] font-bold text-white/50 uppercase tracking-widest">{{ stat.label }}</span>
      </div>
    </div>

    <!-- Filter Bar -->
    <div class="flex flex-col xl:flex-row xl:items-center justify-between gap-6 bg-[#111916] border border-white/5 rounded-[2rem] p-4 lg:px-6">
      <div class="flex-1 flex items-center gap-3 bg-white/5 border border-white/10 rounded-full px-4 group focus-within:border-primary/50 transition-all w-full max-w-md">
        <PhMagnifyingGlass class="text-white/40 group-focus-within:text-primary text-lg transition-colors" weight="bold" />
        <input
          type="text"
          v-model="localSearch"
          @input="onSearchInput"
          placeholder="Tìm theo mã đơn, khách hàng, sự kiện..."
          class="flex-1 bg-transparent border-none py-2.5 text-[14px] text-white outline-none placeholder:text-white/30"
        />
      </div>
      <BaseSelect :options="statusOptions" v-model="statusFilter" class="w-[200px]" />
    </div>

    <!-- Table Container -->
    <div class="flex flex-col gap-4">
      <div v-if="isLoadingOrders" class="py-20 flex flex-col items-center justify-center gap-3">
        <PhSpinner class="animate-spin text-primary text-4xl" weight="bold" />
        <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách đơn hàng...</span>
      </div>

      <template v-else>
        <BaseTable :columns="columns" :data="ordersList">
          <template #id="{ row }">
            <span class="font-mono text-primary font-bold text-[13px]">{{ row.orderId.slice(0, 8) }}</span>
          </template>
          <template #user="{ row }">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-[11px] font-bold text-white uppercase shadow-inner">
                {{ (row.customerName || '?').charAt(0) }}
              </div>
              <div class="flex flex-col">
                <span class="text-[14px] font-bold text-white leading-tight">{{ row.customerName }}</span>
                <span class="text-[11px] text-white/50">{{ row.customerEmail }}</span>
              </div>
            </div>
          </template>
          <template #event="{ row }">
            <div class="flex flex-col">
              <span class="text-[14px] font-medium text-white/90 group-hover:text-primary transition-colors line-clamp-1">{{ row.eventTitle }}</span>
              <span class="text-[11px] text-white/40">{{ row.organizerName }}</span>
            </div>
          </template>
          <template #amount="{ row }">
            <span class="text-[14px] font-bold text-white">{{ formatPrice(row.totalPrice) }}</span>
          </template>
          <template #status="{ row }">
            <BaseBadge :variant="getStatusVariant(row.status)">{{ getStatusLabel(row.status) }}</BaseBadge>
          </template>
          <template #date="{ row }">
            <span class="text-[13px] text-white/60 font-medium">{{ formatDate(row.createdAt) }}</span>
          </template>
        </BaseTable>

        <div v-if="ordersList.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
            <PhReceipt weight="duotone" />
          </div>
          <h3 class="text-xl font-bold font-heading text-white mb-2">Không tìm thấy đơn hàng</h3>
          <p class="text-white/50 max-w-xs mb-8">Thử thay đổi từ khóa hoặc xóa các bộ lọc để tìm lại.</p>
          <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa bộ lọc</BaseButton>
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
          <div class="text-[12px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ currentPage }}</span> / <span class="text-white font-bold">{{ totalPages }}</span>
          </div>
          <div class="flex items-center gap-2">
            <BaseButton variant="outline" size="sm" :disabled="currentPage <= 1" @click="changePage(currentPage - 1)">Trước</BaseButton>
            <BaseButton variant="outline" size="sm" :disabled="currentPage >= totalPages" @click="changePage(currentPage + 1)">Sau</BaseButton>
          </div>
        </div>
      </template>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { adminSearch, addToast } from '../../stores/adminStore'
import { getAdminOrders } from '../../services/order.service'
import { getFinanceSummary } from '../../services/admin-finance.service'
import { getErrorMessage } from '../../utils/apiError'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import BaseSelect from '../../components/ui/BaseSelect.vue'
import { PhMagnifyingGlass, PhReceipt, PhSpinner } from '@phosphor-icons/vue'

const localSearch = ref('')
const statusFilter = ref('all')

const statusOptions = [
  { value: 'all', label: 'Tất cả trạng thái' },
  { value: 'Pending', label: 'Đang chờ' },
  { value: 'Paid', label: 'Đã thanh toán' },
  { value: 'Completed', label: 'Hoàn tất' },
  { value: 'Cancelled', label: 'Đã hủy' },
  { value: 'Refunded', label: 'Đã hoàn tiền' },
]

const columns = [
  { key: 'id', label: 'Mã đơn' },
  { key: 'user', label: 'Khách hàng' },
  { key: 'event', label: 'Sự kiện' },
  { key: 'amount', label: 'Số tiền' },
  { key: 'status', label: 'Trạng thái' },
  { key: 'date', label: 'Ngày đặt' },
]

const ordersList = ref([])
const isLoadingOrders = ref(false)
const currentPage = ref(1)
const totalPages = ref(1)
const totalCount = ref(0)
const pageSize = 10

const orderStats = ref({
  totalCount: 0,
  pendingCount: 0,
  paidCount: 0,
  cancelledCount: 0,
  grossRevenue: 0
})

const loadOrders = async () => {
  isLoadingOrders.value = true
  try {
    const params = { pageNumber: currentPage.value, pageSize }
    if (localSearch.value.trim()) params.search = localSearch.value.trim()
    if (statusFilter.value !== 'all') params.status = statusFilter.value

    const res = await getAdminOrders(params)
    if (res && res.success && res.data) {
      ordersList.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      totalCount.value = res.data.totalCount || 0
      currentPage.value = res.data.pageNumber || 1
    } else {
      ordersList.value = []
      totalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching admin orders:', err)
    addToast(getErrorMessage(err, 'Không thể tải danh sách đơn hàng.'), 'error')
    ordersList.value = []
  } finally {
    isLoadingOrders.value = false
  }
}

const loadOrderStats = async () => {
  try {
    const [totalRes, pendingRes, paidRes, cancelledRes, financeRes] = await Promise.all([
      getAdminOrders({ pageNumber: 1, pageSize: 1 }),
      getAdminOrders({ pageNumber: 1, pageSize: 1, status: 'Pending' }),
      getAdminOrders({ pageNumber: 1, pageSize: 1, status: 'Paid' }),
      getAdminOrders({ pageNumber: 1, pageSize: 1, status: 'Cancelled' }),
      getFinanceSummary({})
    ])
    orderStats.value = {
      totalCount: totalRes?.data?.totalCount || 0,
      pendingCount: pendingRes?.data?.totalCount || 0,
      paidCount: paidRes?.data?.totalCount || 0,
      cancelledCount: cancelledRes?.data?.totalCount || 0,
      grossRevenue: financeRes?.data?.grossRevenue || 0
    }
  } catch (err) {
    console.error('Error fetching admin order stats:', err)
  }
}

onMounted(() => {
  loadOrders()
  loadOrderStats()
})

watch(adminSearch, (val) => { localSearch.value = val })

let searchDebounce = null
const onSearchInput = () => {
  clearTimeout(searchDebounce)
  searchDebounce = setTimeout(() => {
    currentPage.value = 1
    loadOrders()
  }, 400)
}

watch(statusFilter, () => {
  currentPage.value = 1
  loadOrders()
})

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  loadOrders()
}

const getStatusVariant = (status) => {
  if (status === 'Paid' || status === 'Completed') return 'primary'
  if (status === 'Pending') return 'warning'
  if (status === 'Cancelled' || status === 'Refunded') return 'danger'
  return 'neutral'
}

const getStatusLabel = (status) => {
  const map = {
    Pending: 'Đang chờ',
    Paid: 'Đã thanh toán',
    Completed: 'Hoàn tất',
    Cancelled: 'Đã hủy',
    Refunded: 'Đã hoàn tiền'
  }
  return map[status] || status
}

const formatDate = (d) => {
  if (!d) return '-'
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

const formatPrice = (n) => {
  if (!n) return '0đ'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(n).replace('₫', 'đ')
}

const orderStatCards = computed(() => [
  { label: 'Tổng đơn', value: orderStats.value.totalCount, color: 'var(--color-white)' },
  { label: 'Đã thanh toán', value: orderStats.value.paidCount, color: 'var(--color-primary)' },
  { label: 'Đang chờ', value: orderStats.value.pendingCount, color: 'var(--color-warning)' },
  { label: 'Đã hủy', value: orderStats.value.cancelledCount, color: 'var(--color-danger)' },
  { label: 'Doanh thu (30 ngày)', value: formatPrice(orderStats.value.grossRevenue), color: '#f472b6' },
])

const resetFilters = () => {
  localSearch.value = ''
  statusFilter.value = 'all'
  currentPage.value = 1
  loadOrders()
}
</script>
