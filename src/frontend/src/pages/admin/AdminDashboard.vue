<template>
  <div class="flex flex-col gap-8 animate-in fade-in slide-in-from-bottom-4 duration-500 pb-12">
    <!-- Header -->
    <div>
      <h1 class="font-heading text-3xl font-bold text-main mb-2">Tổng quan</h1>
      <p class="text-muted font-medium italic">Chào mừng trở lại! Đây là tóm tắt hoạt động của hệ thống hôm nay.</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      <div v-for="stat in mainStats" :key="stat.label" 
        class="bg-card border border-border-main p-6 rounded-[24px] flex items-center gap-5 group hover:border-primary/30 hover:shadow-2xl hover:shadow-primary/5 transition-all duration-300"
      >
        <div 
          class="w-14 h-14 rounded-2xl flex items-center justify-center text-2xl group-hover:scale-110 transition-transform duration-300 shadow-inner" 
          :style="{ backgroundColor: stat.bg, color: stat.color }"
        >
          <component :is="stat.icon" class="w-6 h-6" />
        </div>
        <div class="flex flex-col">
          <span class="text-[11px] font-bold text-muted uppercase tracking-widest mb-1">{{ stat.label }}</span>
          <span class="text-2xl font-bold text-main">{{ stat.value }}</span>
          <span class="text-[12px] text-muted font-medium mt-1">{{ stat.sub }}</span>
        </div>
      </div>
    </div>

    <!-- Dashboard Content -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Recent Events Table -->
      <div class="lg:col-span-2 bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl flex flex-col">
        <div class="p-6 border-b border-border-main flex justify-between items-center bg-card/50">
          <h3 class="text-lg font-bold text-main">Sự kiện mới đăng</h3>
          <router-link to="/admin/events" class="text-primary text-[13px] font-bold hover:underline transition-all">Tất cả →</router-link>
        </div>
        
        <BaseTable :columns="eventColumns" :data="recentEvents" variant="ghost">
          <template #event="{ row }">
            <div class="flex items-center gap-3">
              <img :src="row.image" class="w-10 h-7 rounded-md object-cover border border-border-main shadow-sm" />
              <span class="font-bold text-main group-hover:text-primary transition-colors line-clamp-1">{{ row.title }}</span>
            </div>
          </template>
          
          <template #category="{ row }">
            <span class="text-[13px] text-muted font-medium capitalize">{{ row.category }}</span>
          </template>
          
          <template #date="{ row }">
            <span class="text-[13px] text-muted font-medium">{{ formatDate(row.dateStart) }}</span>
          </template>
          
          <template #status="{ row }">
            <BaseBadge :variant="getStatusVariant(row.status)" size="xs">{{ getStatusLabel(row.status) }}</BaseBadge>
          </template>
        </BaseTable>
      </div>

      <!-- Right Column -->
      <div class="flex flex-col gap-8">
        <!-- Recent Orders -->
        <div class="bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl">
          <div class="p-6 border-b border-border-main flex justify-between items-center bg-card/50">
            <h3 class="text-lg font-bold text-main">Đơn hàng mới</h3>
            <router-link to="/admin/orders" class="text-primary text-[13px] font-bold hover:underline transition-all">Tất cả →</router-link>
          </div>
          <div class="divide-y divide-border-main/30">
            <div v-for="order in recentOrders" :key="order.id" class="p-5 flex items-center justify-between hover:bg-surface/50 transition-all group">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-surface border border-border-main flex items-center justify-center text-[13px] font-bold text-primary shadow-inner group-hover:border-primary/30 transition-colors">
                  {{ order.user.name.charAt(0) }}
                </div>
                <div class="flex flex-col">
                  <span class="text-[14px] font-bold text-main leading-tight group-hover:text-primary transition-colors">{{ order.user.name }}</span>
                  <span class="text-[11px] text-muted line-clamp-1 max-w-[140px]">{{ order.event.title }}</span>
                </div>
              </div>
              <div class="flex flex-col items-end gap-1">
                <span class="text-[14px] font-bold text-primary">{{ formatPrice(order.totalPrice) }}</span>
                <BaseBadge :variant="order.status === 'confirmed' ? 'primary' : 'secondary'" size="xs" class="!px-1.5 !py-0.5 !text-[9px]">
                  {{ order.status === 'confirmed' ? 'Xong' : 'Chờ' }}
                </BaseBadge>
              </div>
            </div>
          </div>
        </div>

        <!-- Revenue by category -->
        <div class="bg-card border border-border-main rounded-[32px] p-8 shadow-xl">
          <h3 class="text-lg font-bold text-main mb-6">Doanh thu theo loại</h3>
          <div class="flex flex-col gap-6">
            <div v-for="cat in revenueByCategory" :key="cat.name" class="flex flex-col gap-2.5">
              <div class="flex justify-between items-center">
                <span class="text-[13px] font-bold text-main flex items-center gap-2">
                  <span class="w-6 h-6 flex items-center justify-center rounded-lg bg-surface border border-border-main text-[12px]">{{ cat.icon }}</span>
                  {{ cat.name }}
                </span>
                <span class="text-[12px] font-bold text-muted">{{ formatPrice(cat.revenue) }}</span>
              </div>
              <div class="h-2 bg-surface rounded-full overflow-hidden border border-border-main/50 relative">
                <div 
                  class="h-full bg-gradient-to-r from-primary/40 to-primary rounded-full transition-all duration-1000 shadow-[0_0_10px_rgba(0,200,83,0.3)]"
                  :style="{ width: cat.percent + '%' }"
                ></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, h } from 'vue'
import { getEvents } from '../../stores/eventStore'
import { usersData, ordersData } from '../../stores/adminStore'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import BaseTable from '../../components/ui/BaseTable.vue'

const allEvents = computed(() => getEvents())

// Column definition for summary table
const eventColumns = [
  { key: 'event', label: 'Sự kiện' },
  { key: 'category', label: 'Thể loại' },
  { key: 'date', label: 'Thời gian' },
  { key: 'status', label: 'Trạng thái', class: 'text-right' },
]

// Icons
const EventIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('rect', { x: '3', y: '4', width: '18', height: '18', rx: '2' }), h('line', { x1: '16', y1: '2', x2: '16', y2: '6' }), h('line', { x1: '8', y1: '2', x2: '8', y2: '6' }), h('line', { x1: '3', y1: '10', x2: '21', y2: '10' })])
const UserIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('path', { d: 'M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2' }), h('circle', { cx: '9', cy: '7', r: '4' })])
const OrderIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('circle', { cx: '9', cy: '21', r: '1' }), h('circle', { cx: '20', cy: '21', r: '1' }), h('path', { d: 'M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6' })])
const MoneyIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('line', { x1: '12', y1: '1', x2: '12', y2: '23' }), h('path', { d: 'M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6' })])

const mainStats = computed(() => [
  { 
    label: 'Tổng sự kiện', 
    value: allEvents.value.length, 
    sub: `${allEvents.value.filter(e => e.status === 'upcoming').length} sắp diễn ra`,
    icon: EventIcon,
    bg: 'var(--color-primary-dim)',
    color: 'var(--color-primary)'
  },
  { 
    label: 'Người dùng', 
    value: usersData.length, 
    sub: `${usersData.filter(u => u.status === 'active').length} đang hoạt động`,
    icon: UserIcon,
    bg: 'rgba(99, 102, 241, 0.1)',
    color: '#818cf8'
  },
  { 
    label: 'Đơn hàng', 
    value: ordersData.length, 
    sub: `${ordersData.filter(o => o.status === 'confirmed').length} đã xác nhận`,
    icon: OrderIcon,
    bg: 'var(--color-warning-dim)',
    color: 'var(--color-warning)'
  },
  { 
    label: 'Doanh thu', 
    value: formatPrice(ordersData.filter(o => o.status === 'confirmed').reduce((sum, o) => sum + o.totalPrice, 0)), 
    sub: 'Từ các đơn thành công',
    icon: MoneyIcon,
    bg: 'rgba(236, 72, 153, 0.1)',
    color: '#f472b6'
  }
])

const recentEvents = computed(() => allEvents.value.slice(0, 6))
const recentOrders = computed(() => [...ordersData].sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)).slice(0, 5))

const revenueByCategory = computed(() => {
  const cats = { concerts: 0, sports: 0, arts: 0, experiences: 0, workshops: 0 }
  const labels = { concerts: 'Concert', sports: 'Thể thao', arts: 'Sân khấu', experiences: 'Trải nghiệm', workshops: 'Workshop' }
  const icons = { concerts: '🎵', sports: '⚽', arts: '🎭', experiences: '🧭', workshops: '📚' }
  
  ordersData.filter(o => o.status === 'confirmed').forEach(o => {
    const ev = allEvents.value.find(e => e.title === o.event.title)
    if (ev && cats[ev.category] !== undefined) cats[ev.category] += o.totalPrice
  })
  const max = Math.max(...Object.values(cats), 1)
  return Object.entries(cats).map(([key, val]) => ({
    name: labels[key],
    icon: icons[key],
    revenue: val,
    percent: Math.round((val / max) * 100)
  }))
})

const getStatusVariant = (status) => {
  if (status === 'upcoming') return 'primary'
  if (status === 'sold-out') return 'danger'
  return 'secondary'
}

const getStatusLabel = (status) => {
  const map = {
    'upcoming': 'Sắp tới',
    'sold-out': 'Hết vé',
    'ended': 'Kết thúc'
  }
  return map[status] || status
}

const formatDate = (d) => {
  if (!d) return 'TBA'
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })
}

const formatPrice = (n) => {
  if (!n || n === 0) return '0đ'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(n).replace('₫', 'đ')
}
</script>

