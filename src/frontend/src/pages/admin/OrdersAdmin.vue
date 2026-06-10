<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Quản lý đơn hàng</h1>
      <p class="text-white/50 font-medium text-lg">Theo dõi và xử lý tất cả các giao dịch mua vé</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
      <div v-for="stat in orderStats" :key="stat.label" class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] flex flex-col items-center text-center group hover:-translate-y-1 transition-all">
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
          placeholder="Tìm theo mã đơn hoặc khách hàng..." 
          class="flex-1 bg-transparent border-none py-2.5 text-[14px] text-white outline-none placeholder:text-white/30"
        />
      </div>
      <BaseSelect :options="statusOptions" v-model="statusFilter" class="w-[200px]" />
    </div>

    <!-- Table Container -->
    <div class="flex flex-col gap-4">
      <BaseTable :columns="columns" :data="filteredOrders">
        <template #id="{ row }">
          <span class="font-mono text-primary font-bold text-[13px]">{{ row.id }}</span>
        </template>
        <template #user="{ row }">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-[11px] font-bold text-white uppercase shadow-inner">
              {{ row.user.name.charAt(0) }}
            </div>
            <div class="flex flex-col">
              <span class="text-[14px] font-bold text-white leading-tight">{{ row.user.name }}</span>
              <span class="text-[11px] text-white/50">{{ row.user.email }}</span>
            </div>
          </div>
        </template>
        <template #event="{ row }">
          <span class="text-[14px] font-medium text-white/90 group-hover:text-primary transition-colors line-clamp-1">{{ row.event.title }}</span>
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
        <template #actions="{ row }">
          <div class="flex justify-end gap-2">
            <BaseButton v-if="row.status !== 'confirmed'" variant="ghost" size="sm" class="!px-3 hover:!bg-primary/10 hover:!text-primary" @click="updateStatus(row, 'confirmed')">
              <PhCheckCircle weight="bold" class="text-white/70 hover:text-primary" />
            </BaseButton>
            <BaseButton v-if="row.status !== 'cancelled'" variant="ghost" size="sm" class="!px-3 hover:!bg-warning/10 hover:!text-warning" @click="updateStatus(row, 'cancelled')">
              <PhXCircle weight="bold" class="text-white/70 hover:text-warning" />
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-danger/10 hover:!text-danger" @click="confirmDelete(row)">
              <PhTrash weight="bold" class="text-white/70 hover:text-danger" />
            </BaseButton>
          </div>
        </template>
      </BaseTable>

      <div v-if="filteredOrders.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
        <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
          <PhReceipt weight="duotone" />
        </div>
        <h3 class="text-xl font-bold font-heading text-white mb-2">Không tìm thấy đơn hàng</h3>
        <p class="text-white/50 max-w-xs mb-8">Thử thay đổi từ khóa hoặc xóa các bộ lọc để tìm lại.</p>
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
import { PhMagnifyingGlass, PhCheckCircle, PhXCircle, PhTrash, PhReceipt } from '@phosphor-icons/vue'

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
  { label: 'Tổng đơn', value: ordersData.length, color: 'var(--color-white)' },
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
  return 'neutral'
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
