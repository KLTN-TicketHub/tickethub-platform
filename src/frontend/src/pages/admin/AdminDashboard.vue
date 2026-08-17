<template>
  <div class="flex flex-col gap-10 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Tổng quan</h1>
      <p class="text-white/50 font-medium text-lg max-w-2xl">Theo dõi hiệu suất nền tảng, doanh thu và các hoạt động mới nhất trong ngày.</p>
    </div>

    <!-- Stats Bento Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <router-link v-for="(stat, idx) in mainStats" :key="stat.label" 
        :to="stat.link || '/admin/dashboard'"
        class="relative overflow-hidden rounded-[2rem] p-8 flex flex-col gap-6 group transition-transform duration-500 hover:-translate-y-1 hover:border-primary/20 border border-white/5"
        :class="stat.bgClass"
      >
        <div class="flex justify-between items-start relative z-10">
          <div class="w-12 h-12 rounded-full flex items-center justify-center text-xl shadow-lg" :class="stat.iconBgClass">
            <component :is="stat.icon" weight="fill" :class="stat.iconColorClass" />
          </div>
          <span class="text-[10px] font-bold uppercase tracking-[0.2em]" :class="stat.iconColorClass">{{ stat.sub }}</span>
        </div>
        <div class="flex flex-col relative z-10 mt-4">
          <span class="text-[13px] font-bold text-white/50 mb-1">{{ stat.label }}</span>
          <span class="text-4xl font-heading font-black text-white tracking-tight">{{ stat.value }}</span>
        </div>
        <!-- Decorative Glow -->
        <div class="absolute -bottom-10 -right-10 w-40 h-40 rounded-full blur-[50px] opacity-20 pointer-events-none transition-opacity duration-500 group-hover:opacity-40" :class="stat.glowClass"></div>
      </router-link>
    </div>

    <!-- Finance Summary Widget -->
    <div class="bg-surface/50 border border-white/5 rounded-4xl p-8 flex flex-col gap-6">
      <div class="flex items-center justify-between">
        <h3 class="text-xl font-bold font-heading text-white">Tổng quan tài chính <span class="text-white/40 font-medium text-sm">(30 ngày qua)</span></h3>
        <router-link to="/admin/finance" class="text-[13px] font-bold text-primary hover:text-white transition-colors flex items-center gap-1 group">
          Xem báo cáo chi tiết <PhArrowRight weight="bold" class="group-hover:translate-x-1 transition-transform" />
        </router-link>
      </div>

      <div v-if="isLoadingFinance" class="py-6 flex justify-center">
        <PhSpinner class="animate-spin text-primary text-2xl" weight="bold" />
      </div>
      <div v-else class="grid grid-cols-1 sm:grid-cols-3 gap-6">
        <div class="flex flex-col gap-1">
          <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider">Doanh thu gộp</span>
          <span class="text-2xl font-black text-white font-heading">{{ formatPrice(financeSummary.grossRevenue) }}</span>
        </div>
        <div class="flex flex-col gap-1">
          <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider">Phí sàn</span>
          <span class="text-2xl font-black text-white font-heading">{{ formatPrice(financeSummary.platformFee) }}</span>
        </div>
        <div class="flex flex-col gap-1">
          <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider">Đã trả Organizer</span>
          <span class="text-2xl font-black text-white font-heading">{{ formatPrice(financeSummary.netPaidToOrganizers) }}</span>
        </div>
      </div>
    </div>

    <!-- Main Content Area -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
      
      <!-- Recent Events Table (Takes 8 cols) -->
      <div class="lg:col-span-8 flex flex-col gap-6">
        <div class="flex justify-between items-end px-2">
          <h3 class="text-2xl font-bold font-heading text-white">Sự kiện mới đăng</h3>
          <router-link to="/admin/events" class="text-[13px] font-bold text-primary hover:text-white transition-colors flex items-center gap-1 group">
            Xem tất cả <PhArrowRight weight="bold" class="group-hover:translate-x-1 transition-transform" />
          </router-link>
        </div>
        <BaseTable :columns="eventColumns" :data="recentEvents">
          <template #event="{ row }">
            <div class="flex items-center gap-4">
              <div class="w-12 h-12 rounded-xl overflow-hidden border border-white/10 flex-shrink-0">
                <img :src="row.image" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              </div>
              <span class="font-bold text-white group-hover:text-primary transition-colors line-clamp-1">{{ row.title }}</span>
            </div>
          </template>
          <template #category="{ row }">
            <span class="text-[13px] text-white/60 font-bold uppercase tracking-wider">{{ row.category }}</span>
          </template>
          <template #date="{ row }">
            <span class="text-[14px] text-white/80 font-medium">{{ formatDate(row.dateStart) }}</span>
          </template>
          <template #status="{ row }">
            <BaseBadge :variant="getStatusVariant(row.status)">{{ getStatusLabel(row.status) }}</BaseBadge>
          </template>
        </BaseTable>
      </div>

      <!-- Right Column (Takes 4 cols) -->
      <div class="lg:col-span-4 flex flex-col gap-8">
        
        <!-- Revenue by category -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-8 flex flex-col gap-8">
          <h3 class="text-xl font-bold font-heading text-white">Doanh thu thể loại</h3>
          <div class="flex flex-col gap-6">
            <div v-for="cat in revenueByCategory" :key="cat.name" class="flex flex-col gap-3 group">
              <div class="flex justify-between items-center">
                <span class="text-[14px] font-bold text-white/80 flex items-center gap-3">
                  <component :is="cat.icon" weight="fill" class="text-white/40 group-hover:text-primary transition-colors" />
                  {{ cat.name }}
                </span>
                <span class="text-[13px] font-bold text-white">{{ formatPrice(cat.revenue) }}</span>
              </div>
              <div class="h-1.5 bg-white/5 rounded-full overflow-hidden relative">
                <div class="absolute top-0 left-0 h-full bg-primary rounded-full transition-all duration-1000" :style="{ width: cat.percent + '%' }"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Orders -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-8 flex flex-col gap-6">
          <div class="flex justify-between items-center">
            <h3 class="text-xl font-bold font-heading text-white">Đơn hàng mới</h3>
          </div>
          <div class="flex flex-col gap-5">
            <div v-for="order in recentOrders" :key="order.id" class="flex items-center justify-between group cursor-pointer">
              <div class="flex items-center gap-4">
                <div class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-[13px] font-bold text-white/60 group-hover:bg-primary/10 group-hover:text-primary transition-colors">
                  {{ order.user.name.charAt(0) }}
                </div>
                <div class="flex flex-col">
                  <span class="text-[14px] font-bold text-white leading-tight group-hover:text-primary transition-colors">{{ order.user.name }}</span>
                  <span class="text-[12px] text-white/40 line-clamp-1 max-w-[120px] mt-0.5">{{ order.event.title }}</span>
                </div>
              </div>
              <div class="flex flex-col items-end gap-1">
                <span class="text-[14px] font-bold text-primary">{{ formatPrice(order.totalPrice) }}</span>
                <BaseBadge :variant="order.status === 'confirmed' ? 'primary' : 'neutral'" size="sm">
                  {{ order.status === 'confirmed' ? 'Xong' : 'Chờ' }}
                </BaseBadge>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { getEvents } from '../../stores/eventStore'
import { usersData, ordersData, addToast } from '../../stores/adminStore'
import { getFinanceSummary } from '../../services/admin-finance.service'
import { getErrorMessage } from '../../utils/apiError'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import {
  PhTicket, PhUsers, PhReceipt, PhCoins, PhArrowRight, PhSpinner,
  PhMicrophoneStage, PhTrophy, PhMaskHappy, PhCompass, PhBooks
} from '@phosphor-icons/vue'

const allEvents = computed(() => getEvents())

const isLoadingFinance = ref(true)
const financeSummary = ref({ grossRevenue: 0, platformFee: 0, netPaidToOrganizers: 0 })

const loadFinanceSummary = async () => {
  isLoadingFinance.value = true
  try {
    const today = new Date()
    const thirtyDaysAgo = new Date()
    thirtyDaysAgo.setDate(today.getDate() - 30)
    const res = await getFinanceSummary({
      dateFrom: thirtyDaysAgo.toISOString().slice(0, 10),
      dateTo: today.toISOString().slice(0, 10)
    })
    if (res && res.success) financeSummary.value = res.data
  } catch (err) {
    console.error('Error fetching finance summary:', err)
    addToast(getErrorMessage(err, 'Không thể tải tổng quan doanh thu.'), 'error')
  } finally {
    isLoadingFinance.value = false
  }
}

onMounted(() => {
  loadFinanceSummary()
})

const eventColumns = [
  { key: 'event', label: 'Sự kiện' },
  { key: 'category', label: 'Thể loại' },
  { key: 'date', label: 'Thời gian' },
  { key: 'status', label: 'Trạng thái', class: 'text-right' },
]

const mainStats = computed(() => [
  { 
    label: 'Tổng sự kiện', 
    value: allEvents.value.length, 
    sub: 'Đang mở',
    icon: PhTicket,
    link: '/admin/events',
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-primary/10',
    iconColorClass: 'text-primary',
    glowClass: 'bg-primary'
  },
  { 
    label: 'Người dùng', 
    value: usersData.length, 
    sub: 'Hoạt động',
    icon: PhUsers,
    link: '/admin/users',
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-[#818cf8]/10',
    iconColorClass: 'text-[#818cf8]',
    glowClass: 'bg-[#818cf8]'
  },
  { 
    label: 'Đơn hàng', 
    value: ordersData.length, 
    sub: 'Thành công',
    icon: PhReceipt,
    link: '/admin/orders',
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-yellow-500/10',
    iconColorClass: 'text-yellow-500',
    glowClass: 'bg-yellow-500'
  },
  { 
    label: 'Doanh thu', 
    value: formatPrice(ordersData.filter(o => o.status === 'confirmed').reduce((sum, o) => sum + o.totalPrice, 0)), 
    sub: 'Tuần này',
    icon: PhCoins,
    link: '/admin/orders',
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-[#f472b6]/10',
    iconColorClass: 'text-[#f472b6]',
    glowClass: 'bg-[#f472b6]'
  }
])

const recentEvents = computed(() => allEvents.value.slice(0, 5))
const recentOrders = computed(() => [...ordersData].sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)).slice(0, 5))

const revenueByCategory = computed(() => {
  const cats = { concerts: 0, sports: 0, arts: 0, experiences: 0, workshops: 0 }
  const labels = { concerts: 'Concert', sports: 'Thể thao', arts: 'Sân khấu', experiences: 'Trải nghiệm', workshops: 'Workshop' }
  const icons = { concerts: PhMicrophoneStage, sports: PhTrophy, arts: PhMaskHappy, experiences: PhCompass, workshops: PhBooks }
  
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
  })).sort((a,b) => b.revenue - a.revenue)
})

const getStatusVariant = (status) => {
  if (status === 'upcoming') return 'primary'
  if (status === 'sold-out') return 'danger'
  return 'neutral'
}

const getStatusLabel = (status) => {
  const map = { 'upcoming': 'Sắp diễn ra', 'sold-out': 'Hết vé', 'ended': 'Đã kết thúc' }
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
