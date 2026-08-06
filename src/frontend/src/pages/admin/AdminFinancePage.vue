<template>
  <div class="flex flex-col gap-10 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-6">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Báo cáo tài chính</h1>
        <p class="text-white/50 font-medium text-lg max-w-2xl">Doanh thu, phí sàn và tình hình thanh toán của toàn nền tảng.</p>
      </div>

      <!-- Date range filter -->
      <div class="flex items-end gap-3 bg-[#111916] border border-white/5 rounded-2xl p-4">
        <div class="flex flex-col gap-1.5">
          <label class="text-[11px] font-bold text-white/40 uppercase tracking-wider">Từ ngày</label>
          <input
            v-model="dateFrom"
            type="date"
            class="bg-[#0A0F0D] border border-white/10 focus:border-primary/50 text-white rounded-lg py-2 px-3 text-[13px] font-medium outline-none transition-all"
          />
        </div>
        <div class="flex flex-col gap-1.5">
          <label class="text-[11px] font-bold text-white/40 uppercase tracking-wider">Đến ngày</label>
          <input
            v-model="dateTo"
            type="date"
            class="bg-[#0A0F0D] border border-white/10 focus:border-primary/50 text-white rounded-lg py-2 px-3 text-[13px] font-medium outline-none transition-all"
          />
        </div>
        <BaseButton variant="primary" class="!py-2.5 !px-5" :disabled="isLoadingSummary" @click="loadAll">
          Áp dụng
        </BaseButton>
      </div>
    </div>

    <!-- KPI Bento Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-primary/20 transition-all duration-300">
        <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-primary/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
        <div class="w-12 h-12 rounded-2xl bg-primary/10 flex items-center justify-center text-primary text-2xl mb-4">
          <PhCoins weight="duotone" />
        </div>
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Tổng doanh thu gộp</span>
        <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ formatCurrency(summary.grossRevenue) }}</span>
      </div>

      <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-[#818cf8]/20 transition-all duration-300">
        <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-[#818cf8]/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
        <div class="w-12 h-12 rounded-2xl bg-[#818cf8]/10 flex items-center justify-center text-[#818cf8] text-2xl mb-4">
          <PhPercent weight="duotone" />
        </div>
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Tổng phí sàn</span>
        <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ formatCurrency(summary.platformFee) }}</span>
      </div>

      <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-[#f43f5e]/20 transition-all duration-300">
        <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-[#f43f5e]/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
        <div class="w-12 h-12 rounded-2xl bg-[#f43f5e]/10 flex items-center justify-center text-[#f43f5e] text-2xl mb-4">
          <PhHandCoins weight="duotone" />
        </div>
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Đã trả Organizer</span>
        <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ formatCurrency(summary.netPaidToOrganizers) }}</span>
      </div>

      <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-warning/20 transition-all duration-300">
        <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-warning/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
        <div class="w-12 h-12 rounded-2xl bg-warning/10 flex items-center justify-center text-warning text-2xl mb-4">
          <PhWallet weight="duotone" />
        </div>
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Số dư ví hệ thống hiện tại</span>
        <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ formatCurrency(summary.currentTotalWalletBalance) }}</span>
      </div>
    </div>

    <!-- Trend Chart -->
    <section class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 shadow-2xl">
      <div class="mb-8">
        <h2 class="font-heading text-2xl font-black text-white uppercase tracking-wider">Xu hướng doanh thu &amp; phí sàn</h2>
        <p class="text-white/40 text-[13px] font-medium mt-1">Theo từng ngày trong khoảng thời gian đã chọn</p>
      </div>

      <div v-if="isLoadingTrend" class="h-80 flex flex-col items-center justify-center gap-3">
        <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
        <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải dữ liệu...</span>
      </div>
      <div v-else-if="trend.length === 0" class="h-80 flex items-center justify-center text-white/30 font-bold text-[14px]">
        Không có dữ liệu cho khoảng thời gian này.
      </div>
      <div v-else class="relative w-full h-80">
        <Line :data="trendChartJsData" :options="trendChartOptions" />
      </div>
    </section>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Revenue by category -->
      <div class="bg-[#111916] border border-white/5 rounded-[2rem] p-8 flex flex-col gap-8">
        <h3 class="text-xl font-bold font-heading text-white">Doanh thu theo danh mục</h3>

        <div v-if="isLoadingByCategory" class="py-8 flex justify-center">
          <PhSpinner class="animate-spin text-primary text-2xl" weight="bold" />
        </div>
        <div v-else-if="byCategory.length === 0" class="py-8 text-center text-white/30 font-bold text-[13px]">
          Không có dữ liệu cho khoảng thời gian này.
        </div>
        <div v-else class="flex flex-col gap-6">
          <div v-for="cat in byCategoryWithPercent" :key="cat.categoryId" class="flex flex-col gap-3 group">
            <div class="flex justify-between items-center">
              <span class="text-[14px] font-bold text-white/80">{{ cat.categoryName }} <span class="text-white/30 font-medium">· {{ cat.eventCount }} sự kiện</span></span>
              <span class="text-[13px] font-bold text-white">{{ formatCurrency(cat.grossRevenue) }}</span>
            </div>
            <div class="h-1.5 bg-white/5 rounded-full overflow-hidden relative">
              <div class="absolute top-0 left-0 h-full bg-primary rounded-full transition-all duration-1000" :style="{ width: cat.percent + '%' }"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Payment gateway stats -->
      <div class="bg-[#111916] border border-white/5 rounded-[2rem] p-8 flex flex-col gap-8">
        <h3 class="text-xl font-bold font-heading text-white">Thống kê theo cổng thanh toán</h3>

        <div v-if="isLoadingGatewayStats" class="py-8 flex justify-center">
          <PhSpinner class="animate-spin text-primary text-2xl" weight="bold" />
        </div>
        <div v-else-if="gatewayStats.length === 0" class="py-8 text-center text-white/30 font-bold text-[13px]">
          Không có giao dịch nào trong khoảng thời gian này.
        </div>
        <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div v-for="gw in gatewayStats" :key="gw.gateway" class="bg-[#0A0F0D] border border-white/5 rounded-2xl p-5 flex flex-col gap-3">
            <div class="flex items-center justify-between">
              <span class="font-black text-white uppercase tracking-wider text-[14px]">{{ gw.gateway }}</span>
              <span class="text-primary font-black text-[13px]">{{ gw.successRate }}%</span>
            </div>
            <span class="text-xl font-black text-white font-heading">{{ formatCurrency(gw.successAmount) }}</span>
            <div class="flex items-center gap-4 text-[12px] font-bold">
              <span class="text-primary">{{ gw.successCount }} thành công</span>
              <span class="text-danger">{{ gw.failedCount }} thất bại</span>
              <span class="text-white/40">{{ gw.pendingCount }} chờ</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { Line } from 'vue-chartjs'
import { PhCoins, PhPercent, PhHandCoins, PhWallet, PhSpinner } from '@phosphor-icons/vue'
import { getFinanceSummary, getFinanceTrend, getFinanceByCategory, getPaymentGatewayStats } from '../../services/admin-finance.service'
import { createAreaGradient } from '../../lib/chartSetup'
import BaseButton from '../../components/ui/BaseButton.vue'

const toDateInputValue = (d) => d.toISOString().slice(0, 10)

const today = new Date()
const thirtyDaysAgo = new Date()
thirtyDaysAgo.setDate(today.getDate() - 30)

const dateFrom = ref(toDateInputValue(thirtyDaysAgo))
const dateTo = ref(toDateInputValue(today))

const isLoadingSummary = ref(false)
const isLoadingTrend = ref(false)
const isLoadingByCategory = ref(false)
const isLoadingGatewayStats = ref(false)

const summary = ref({
  grossRevenue: 0,
  platformFee: 0,
  netPaidToOrganizers: 0,
  eventsSettledCount: 0,
  pendingPayoutRequestsCount: 0,
  currentTotalWalletBalance: 0
})
const trend = ref([])
const byCategory = ref([])
const gatewayStats = ref([])

const dateRangeParams = computed(() => ({ dateFrom: dateFrom.value, dateTo: dateTo.value }))

const loadSummary = async () => {
  isLoadingSummary.value = true
  try {
    const res = await getFinanceSummary(dateRangeParams.value)
    if (res && res.success) summary.value = res.data
  } catch (err) {
    console.error('Error fetching finance summary:', err)
  } finally {
    isLoadingSummary.value = false
  }
}

const loadTrend = async () => {
  isLoadingTrend.value = true
  try {
    const res = await getFinanceTrend(dateRangeParams.value)
    trend.value = (res && res.success) ? (res.data || []) : []
  } catch (err) {
    console.error('Error fetching finance trend:', err)
    trend.value = []
  } finally {
    isLoadingTrend.value = false
  }
}

const loadByCategory = async () => {
  isLoadingByCategory.value = true
  try {
    const res = await getFinanceByCategory(dateRangeParams.value)
    byCategory.value = (res && res.success) ? (res.data || []) : []
  } catch (err) {
    console.error('Error fetching finance by category:', err)
    byCategory.value = []
  } finally {
    isLoadingByCategory.value = false
  }
}

const loadGatewayStats = async () => {
  isLoadingGatewayStats.value = true
  try {
    const res = await getPaymentGatewayStats(dateRangeParams.value)
    gatewayStats.value = (res && res.success) ? (res.data || []) : []
  } catch (err) {
    console.error('Error fetching payment gateway stats:', err)
    gatewayStats.value = []
  } finally {
    isLoadingGatewayStats.value = false
  }
}

const loadAll = () => {
  loadSummary()
  loadTrend()
  loadByCategory()
  loadGatewayStats()
}

const byCategoryWithPercent = computed(() => {
  const max = Math.max(...byCategory.value.map(c => c.grossRevenue), 1)
  return byCategory.value.map(c => ({ ...c, percent: Math.round((c.grossRevenue / max) * 100) }))
})

const trendChartJsData = computed(() => ({
  labels: trend.value.map(d => formatDateLabel(d.date)),
  datasets: [
    {
      label: 'Doanh thu gộp',
      data: trend.value.map(d => d.grossRevenue),
      borderColor: '#00C853',
      borderWidth: 3,
      pointRadius: 0,
      pointHoverRadius: 5,
      tension: 0.35,
      fill: true,
      backgroundColor: (context) => {
        const { ctx, chartArea } = context.chart
        if (!chartArea) return '#00C85320'
        return createAreaGradient(ctx, chartArea, '#00C853')
      }
    },
    {
      label: 'Phí sàn',
      data: trend.value.map(d => d.platformFee),
      borderColor: '#818cf8',
      borderWidth: 3,
      pointRadius: 0,
      pointHoverRadius: 5,
      tension: 0.35,
      fill: false
    }
  ]
}))

const trendChartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  interaction: { mode: 'index', intersect: false },
  scales: {
    x: {
      grid: { display: false },
      border: { display: false },
      ticks: { color: 'rgba(255,255,255,0.3)', font: { size: 9, weight: 'bold' }, maxRotation: 0, autoSkip: true, maxTicksLimit: 10 }
    },
    y: {
      grid: { color: 'rgba(255,255,255,0.03)' },
      border: { display: false },
      beginAtZero: true,
      ticks: { color: 'rgba(255,255,255,0.2)', font: { size: 9, weight: 'bold' }, maxTicksLimit: 5, callback: (value) => formatCurrencyCompact(value) }
    }
  },
  plugins: {
    legend: {
      display: true,
      position: 'top',
      align: 'end',
      labels: { color: 'rgba(255,255,255,0.6)', font: { size: 11, weight: 'bold' }, boxWidth: 10, boxHeight: 10, usePointStyle: true, pointStyle: 'circle' }
    },
    tooltip: {
      backgroundColor: '#182019',
      borderColor: 'rgba(0,200,83,0.2)',
      borderWidth: 1,
      titleColor: '#00C853',
      titleFont: { size: 11, weight: 'bold' },
      bodyColor: '#ffffff',
      bodyFont: { size: 12, weight: 'bold' },
      padding: 12,
      cornerRadius: 12,
      callbacks: {
        label: (item) => `${item.dataset.label}: ${formatCurrency(item.raw)}`
      }
    }
  }
}))

const formatDateLabel = (dateStr) => {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}`
}

const formatCurrency = (val) => {
  if (val === undefined || val === null) return '0 ₫'
  return val.toLocaleString('vi-VN') + ' ₫'
}

const formatCurrencyCompact = (val) => {
  if (val === undefined || val === null) return '0 ₫'
  if (val >= 1000000) return (val / 1000000).toLocaleString('vi-VN', { maximumFractionDigits: 1 }) + 'M ₫'
  if (val >= 1000) return (val / 1000).toLocaleString('vi-VN', { maximumFractionDigits: 0 }) + 'K ₫'
  return val.toLocaleString('vi-VN') + ' ₫'
}

onMounted(() => {
  loadAll()
})
</script>
