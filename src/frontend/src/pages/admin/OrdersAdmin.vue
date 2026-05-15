<template>
  <div class="flex flex-col gap-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6">
      <div>
        <h1 class="font-heading text-3xl font-bold text-main mb-2">Quản lý đơn hàng</h1>
        <p class="text-muted font-medium">Theo dõi và xử lý tất cả các giao dịch mua vé</p>
      </div>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
      <div v-for="stat in orderStats" :key="stat.label" class="bg-card border border-border-main p-4 rounded-2xl flex flex-col items-center text-center group hover:border-primary/30 transition-all">
        <span class="text-2xl font-bold mb-1 group-hover:scale-110 transition-transform" :style="{ color: stat.color }">{{ stat.value }}</span>
        <span class="text-[10px] font-bold text-muted uppercase tracking-widest">{{ stat.label }}</span>
      </div>
    </div>

    <!-- Filter Bar -->
    <div class="bg-card border border-border-main p-6 rounded-[24px] flex flex-col lg:flex-row gap-4 items-center">
      <div class="flex-1 flex items-center gap-3 bg-surface border border-border-main rounded-xl px-4 w-full group focus-within:border-primary transition-all">
        <svg class="w-4 h-4 text-muted group-focus-within:text-primary transition-colors" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
        <input 
          type="text" 
          v-model="localSearch" 
          placeholder="Tìm theo mã đơn hoặc tên khách hàng..." 
          class="flex-1 bg-transparent border-none py-3 text-[14px] text-main outline-none"
        />
      </div>
      <BaseSelect 
        :options="statusOptions" 
        v-model="statusFilter"
        class="min-w-[200px] w-full lg:w-auto"
      />
    </div>

    <!-- Table Container -->
    <div class="bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl">
      <BaseTable :columns="columns" :data="filteredOrders">
        <!-- ID Slot -->
        <template #id="{ row }">
          <span class="font-mono text-primary font-bold text-[13px]">{{ row.id }}</span>
        </template>

        <!-- User Slot -->
        <template #user="{ row }">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-lg bg-surface border border-border-main flex items-center justify-center text-[11px] font-bold text-main uppercase shadow-inner">
              {{ row.user.name.charAt(0) }}
            </div>
            <div class="flex flex-col">
              <span class="text-[14px] font-bold text-main leading-tight">{{ row.user.name }}</span>
              <span class="text-[11px] text-muted">{{ row.user.email }}</span>
            </div>
          </div>
        </template>

        <!-- Event Slot -->
        <template #event="{ row }">
          <span class="text-[14px] font-medium text-main group-hover:text-primary transition-colors line-clamp-1">{{ row.event.title }}</span>
        </template>

        <!-- Amount Slot -->
        <template #amount="{ row }">
          <span class="text-[14px] font-bold text-main">{{ formatPrice(row.totalPrice) }}</span>
        </template>

        <!-- Status Slot -->
        <template #status="{ row }">
          <BaseBadge :variant="getStatusVariant(row.status)" size="sm">
            {{ getStatusLabel(row.status) }}
          </BaseBadge>
        </template>

        <!-- Date Slot -->
        <template #date="{ row }">
          <span class="text-[13px] text-muted font-medium">{{ formatDate(row.createdAt) }}</span>
        </template>

        <!-- Actions Slot -->
        <template #actions="{ row }">
          <div class="flex justify-end gap-2">
            <BaseButton v-if="row.status !== 'confirmed'" variant="ghost" size="sm" class="!p-2 hover:!text-primary" @click="updateStatus(row, 'confirmed')">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><polyline points="20 6 9 17 4 12"/></svg>
            </BaseButton>
            <BaseButton v-if="row.status !== 'cancelled'" variant="ghost" size="sm" class="!p-2 hover:!text-warning" @click="updateStatus(row, 'cancelled')">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-danger" @click="confirmDelete(row)">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M3 6h18"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6"/></svg>
            </BaseButton>
          </div>
        </template>
      </BaseTable>

      <!-- Empty State -->
      <div v-if="filteredOrders.length === 0" class="py-20 flex flex-col items-center text-center">
        <div class="w-20 h-20 bg-surface rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner">🛒</div>
        <h3 class="text-xl font-bold text-main mb-2">Không tìm thấy đơn hàng</h3>
        <p class="text-muted max-w-xs mb-8">Hãy thử thay đổi từ khóa tìm kiếm hoặc bộ lọc của bạn.</p>
        <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa bộ lọc</BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { ordersData, adminSearch, openConfirm, addToast } from '../../stores/adminStore'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import BaseSelect from '../../components/ui/BaseSelect.vue'

const localSearch = ref('')
const statusFilter = ref('all')

const statusOptions = [
  { value: 'all', label: 'Tất cả trạng thái' },
  { value: 'confirmed', label: 'Đã xác nhận' },
  { value: 'pending', label: 'Đang chờ' },
  { value: 'cancelled', label: 'Đã hủy' },
]

const columns = [
  { key: 'id', label: 'Mã đơn' },
  { key: 'user', label: 'Khách hàng' },
  { key: 'event', label: 'Sự kiện' },
  { key: 'amount', label: 'Số tiền' },
  { key: 'status', label: 'Trạng thái' },
  { key: 'date', label: 'Ngày đặt' },
  { key: 'actions', label: '', class: 'w-32' },
]

watch(adminSearch, (val) => { localSearch.value = val })

const filteredOrders = computed(() => {
  const q = localSearch.value.toLowerCase()
  return ordersData.filter(o => {
    const matchSearch = !q || o.id.toLowerCase().includes(q) || o.user.name.toLowerCase().includes(q)
    const matchStatus = statusFilter.value === 'all' || o.status === statusFilter.value
    return matchSearch && matchStatus
  })
})

const orderStats = computed(() => [
  { label: 'Tổng đơn', value: ordersData.length, color: 'var(--color-main)' },
  { label: 'Thành công', value: ordersData.filter(o => o.status === 'confirmed').length, color: 'var(--color-primary)' },
  { label: 'Đang chờ',   value: ordersData.filter(o => o.status === 'pending').length, color: 'var(--color-warning)' },
  { label: 'Đã hủy', value: ordersData.filter(o => o.status === 'cancelled').length, color: 'var(--color-danger)' },
  { label: 'Doanh thu', value: new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(
      ordersData.filter(o => o.status === 'confirmed').reduce((s, o) => s + o.totalPrice, 0)
    ).replace('₫', 'đ'), color: '#f472b6' },
])

const getStatusVariant = (status) => {
  if (status === 'confirmed') return 'primary'
  if (status === 'pending') return 'warning'
  return 'secondary'
}

const getStatusLabel = (status) => {
  const map = {
    'confirmed': 'Thành công',
    'pending': 'Đang chờ',
    'cancelled': 'Đã hủy'
  }
  return map[status] || status
}

const updateStatus = (order, status) => {
  const idx = ordersData.findIndex(o => o.id === order.id)
  if (idx !== -1) {
    ordersData[idx].status = status
    addToast(`Đã cập nhật đơn hàng ${order.id} sang ${getStatusLabel(status)}`, status === 'confirmed' ? 'success' : 'warning')
  }
}

const confirmDelete = (order) => {
  openConfirm(
    'Xóa đơn hàng',
    `Bạn có chắc chắn muốn xóa đơn hàng "${order.id}"? Hành động này không thể hoàn tác.`,
    () => {
      const idx = ordersData.findIndex(o => o.id === order.id)
      if (idx !== -1) {
        ordersData.splice(idx, 1)
        addToast(`Đơn hàng ${order.id} đã bị xóa`, 'error')
      }
    }
  )
}

const formatDate = (d) => {
  if (!d) return '-'
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

const formatPrice = (n) => {
  if (!n) return '0đ'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(n).replace('₫', 'đ')
}

const resetFilters = () => {
  localSearch.value = ''
  statusFilter.value = 'all'
}
</script>

