<template>
  <div v-if="isLoading" class="flex flex-col pb-20 bg-[#0A0F0D] min-h-[80vh] items-center justify-center">
    <div class="flex flex-col items-center gap-4">
      <PhSpinner class="animate-spin text-primary text-5xl" weight="bold" />
      <span class="text-white/50 text-sm font-bold uppercase tracking-widest">Đang tải báo cáo sự kiện...</span>
    </div>
  </div>

  <div v-else-if="error" class="flex flex-col items-center justify-center py-32 px-6 text-center min-h-[80vh] bg-[#0A0F0D]">
    <div class="w-24 h-24 bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 mb-6 shadow-inner">
      <PhWarningCircle weight="duotone" />
    </div>
    <h2 class="font-heading text-3xl font-black text-white mb-3">Lỗi tải báo cáo</h2>
    <p class="text-white/50 max-w-md mx-auto mb-10 font-medium">{{ error }}</p>
    <BaseButton variant="primary" @click="router.push('/organizer')">Quay lại Tổng quan</BaseButton>
  </div>

  <div v-else-if="reportData" class="flex flex-col pb-20 bg-[#0A0F0D] min-h-screen">
    <!-- Header Area -->
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
              :src="reportData.eventImage || 'https://picsum.photos/seed/event-placeholder/200/200'"
              class="w-full h-full object-cover"
              @error="handleImageError"
            />
          </div>
          <div class="space-y-1">
            <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">Báo cáo doanh thu</span>
            <h1 class="text-2xl md:text-3xl font-black font-heading text-white tracking-tight line-clamp-1 uppercase">
              {{ reportData.eventTitle }}
            </h1>
          </div>
        </div>

        <BaseButton
          v-if="payoutStatus && payoutStatus.hasProposedPayout"
          variant="primary"
          class="!rounded-2xl !py-3.5 !px-6 shrink-0 flex items-center gap-2"
          @click="router.push('/organizer/wallet')"
        >
          <PhHandCoins weight="bold" />
          Xem đề xuất giải ngân
        </BaseButton>
        <span
          v-else-if="payoutStatus && payoutStatus.hasAcceptedPayout"
          class="shrink-0 px-6 py-3.5 rounded-2xl bg-primary/10 border border-primary/20 text-primary text-[13px] font-black uppercase tracking-widest flex items-center gap-2"
        >
          <PhCheckCircle weight="fill" />
          Đã giải ngân
        </span>
        <span
          v-else-if="payoutStatus && payoutStatus.hasPendingRequest"
          class="shrink-0 px-6 py-3.5 rounded-2xl bg-warning/10 border border-warning/20 text-warning text-[13px] font-black uppercase tracking-widest flex items-center gap-2"
        >
          <PhClock weight="bold" />
          Đã yêu cầu giải ngân
        </span>
        <BaseButton
          v-else-if="payoutStatus"
          variant="primary"
          class="!rounded-2xl !py-3.5 !px-6 shrink-0 flex items-center gap-2"
          :disabled="isRequestingPayout"
          @click="handleRequestPayout"
        >
          <PhSpinner v-if="isRequestingPayout" class="animate-spin text-lg" />
          <template v-else>
            <PhHandCoins weight="bold" />
            Yêu cầu giải ngân
          </template>
        </BaseButton>
      </div>

      <!-- Bento Grid KPIs -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <!-- Revenue Card -->
        <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-primary/20 transition-all duration-300">
          <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-primary/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
          <div class="w-12 h-12 rounded-2xl bg-primary/10 flex items-center justify-center text-primary text-2xl mb-4">
            <PhCoins weight="duotone" />
          </div>
          <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Tổng doanh thu</span>
          <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ formatCurrency(reportData.totalRevenue) }}</span>
        </div>

        <!-- Tickets Sold Card -->
        <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-[#818cf8]/20 transition-all duration-300">
          <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-[#818cf8]/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
          <div class="w-12 h-12 rounded-2xl bg-[#818cf8]/10 flex items-center justify-center text-[#818cf8] text-2xl mb-4">
            <PhTicket weight="duotone" />
          </div>
          <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Vé đã bán</span>
          <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ reportData.totalTicketsSold }} vé</span>
        </div>

        <!-- Total Capacity Card -->
        <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-[#f43f5e]/20 transition-all duration-300">
          <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-[#f43f5e]/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
          <div class="w-12 h-12 rounded-2xl bg-[#f43f5e]/10 flex items-center justify-center text-[#f43f5e] text-2xl mb-4">
            <PhUsers weight="duotone" />
          </div>
          <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Tổng sức chứa</span>
          <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ reportData.totalCapacity }} chỗ</span>
        </div>

        <!-- Fill Rate Card -->
        <div class="bg-[#111916] border border-white/5 p-6 rounded-[2rem] shadow-xl relative overflow-hidden group hover:border-warning/20 transition-all duration-300">
          <div class="absolute -right-8 -bottom-8 w-24 h-24 bg-warning/10 rounded-full blur-2xl group-hover:scale-125 transition-transform"></div>
          <div class="w-12 h-12 rounded-2xl bg-warning/10 flex items-center justify-center text-warning text-2xl mb-4">
            <PhChartPie weight="duotone" />
          </div>
          <span class="text-[12px] font-bold text-white/40 uppercase tracking-wider block mb-1">Tỷ lệ lấp đầy</span>
          <span class="text-2xl md:text-3xl font-black text-white font-heading leading-tight">{{ reportData.fillRate }}%</span>
        </div>
      </div>

      <!-- Chart Section -->
      <section class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 shadow-2xl mb-12">
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
          <div>
            <h2 class="font-heading text-2xl font-black text-white uppercase tracking-wider">Xu hướng bán vé</h2>
            <p class="text-white/40 text-[13px] font-medium mt-1">Biểu đồ doanh thu và số lượng vé bán qua thời gian</p>
          </div>
          
          <!-- Range toggle buttons -->
          <div class="flex bg-[#0A0F0D] p-1.5 rounded-xl border border-white/5 self-start">
            <button 
              @click="setChartRange('24h')"
              class="px-4 py-2 rounded-lg text-xs font-bold transition-all cursor-pointer"
              :class="chartRange === '24h' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
            >
              24 Giờ Qua
            </button>
            <button 
              @click="setChartRange('30d')"
              class="px-4 py-2 rounded-lg text-xs font-bold transition-all cursor-pointer"
              :class="chartRange === '30d' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
            >
              30 Ngày Qua
            </button>
          </div>
        </div>

        <div v-if="isLoadingChart" class="h-80 flex flex-col items-center justify-center gap-3">
          <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
          <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải dữ liệu biểu đồ...</span>
        </div>

        <div v-else-if="chartData.length === 0" class="h-80 flex items-center justify-center text-white/30 font-bold text-[14px]">
          Không có dữ liệu thống kê cho khoảng thời gian này.
        </div>

        <div v-else class="relative w-full h-80">
          <Line :data="revenueChartJsData" :options="revenueChartOptions" />
        </div>
      </section>

      <!-- Engagement Trend Section -->
      <section class="mb-12">
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
          <div>
            <h2 class="font-heading text-2xl font-black text-white uppercase tracking-wider">Xu hướng tương tác</h2>
            <p class="text-white/40 text-[13px] font-medium mt-1">Lượt xem và lượt có ý định mua vé theo thời gian</p>
          </div>

          <div class="flex bg-[#0A0F0D] p-1.5 rounded-xl border border-white/5 self-start">
            <button
              @click="setClickTrendRange('7d')"
              class="px-4 py-2 rounded-lg text-xs font-bold transition-all cursor-pointer"
              :class="clickTrendRange === '7d' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
            >
              7 Ngày Qua
            </button>
            <button
              @click="setClickTrendRange('30d')"
              class="px-4 py-2 rounded-lg text-xs font-bold transition-all cursor-pointer"
              :class="clickTrendRange === '30d' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
            >
              30 Ngày Qua
            </button>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <TrendMiniChart
            title="Lượt xem sự kiện"
            color="#00C853"
            :data="clickTrend"
            value-key="viewCount"
            value-label="lượt xem"
            :label-fn="formatClickTrendLabel"
            :is-loading="isLoadingClickTrend"
          />
          <TrendMiniChart
            title="Lượt có ý định mua vé"
            color="#818cf8"
            :data="clickTrend"
            value-key="purchaseIntentCount"
            value-label="lượt"
            :label-fn="formatClickTrendLabel"
            :is-loading="isLoadingClickTrend"
          />
        </div>
      </section>

      <!-- AI Suggestion Section -->
      <div class="mb-12">
        <AiSuggestionSection
          title="Gợi ý cải thiện sự kiện"
          :fetcher="(r) => getEventSuggestions(route.params.id, { range: r })"
          :range="clickTrendRange"
        />
      </div>

      <!-- Showtimes Detailed Breakdown -->
      <section class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 shadow-2xl mb-12">
        <h2 class="font-heading text-2xl font-black text-white uppercase tracking-wider mb-6">Chi tiết theo suất diễn</h2>
        <div class="overflow-x-auto">
          <table class="w-full border-collapse text-left">
            <thead>
              <tr class="border-b border-white/10 text-white/40 text-[12px] font-bold uppercase tracking-wider">
                <th class="pb-4 pr-4">Suất diễn</th>
                <th class="pb-4 px-4">Thời gian diễn ra</th>
                <th class="pb-4 px-4 text-right">Doanh thu</th>
                <th class="pb-4 px-4 text-right">Đã bán</th>
                <th class="pb-4 px-4 text-right">Sức chứa</th>
                <th class="pb-4 pl-4 text-right">Lấp đầy</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-white/5">
              <tr v-for="(st, idx) in reportData.showtimes" :key="st.showtimeId" class="text-[14px] text-white/80 hover:bg-white/[0.02] transition-colors">
                <td class="py-4 pr-4 font-bold text-primary">Suất {{ idx + 1 }}</td>
                <td class="py-4 px-4">
                  <div class="flex flex-col gap-0.5">
                    <span class="font-bold text-white/90">{{ formatEventDate(st.startAt) }}</span>
                    <span class="text-xs text-white/40">{{ formatTimeOnly(st.startAt) }} - {{ formatTimeOnly(st.endAt) }}</span>
                  </div>
                </td>
                <td class="py-4 px-4 text-right font-black text-white">{{ formatCurrency(st.revenue) }}</td>
                <td class="py-4 px-4 text-right font-bold">{{ st.ticketsSold }}</td>
                <td class="py-4 px-4 text-right text-white/50">{{ st.capacity }}</td>
                <td class="py-4 pl-4 text-right">
                  <div class="flex items-center justify-end gap-3">
                    <span class="font-black text-warning">{{ st.fillRate }}%</span>
                    <div class="hidden sm:block w-16 h-1.5 bg-white/5 rounded-full overflow-hidden">
                      <div class="h-full bg-warning" :style="{ width: st.fillRate + '%' }"></div>
                    </div>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Orders List Section -->
      <section class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 shadow-2xl">
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
          <h2 class="font-heading text-2xl font-black text-white uppercase tracking-wider">Danh sách đơn hàng</h2>
          
          <!-- Search input -->
          <div class="relative w-full sm:w-80">
            <input 
              v-model="searchKeyword" 
              @input="handleSearch"
              type="text" 
              placeholder="Tìm theo tên, email, SĐT, mã đơn..." 
              class="w-full bg-[#0A0F0D] border border-white/5 focus:border-primary/50 text-white rounded-xl py-2.5 pl-10 pr-4 text-[13px] font-medium outline-none transition-all"
            />
            <PhMagnifyingGlass class="absolute left-3.5 top-1/2 -translate-y-1/2 text-white/30 text-lg" />
          </div>
        </div>

        <div v-if="isLoadingOrders" class="py-12 flex flex-col items-center justify-center gap-3">
          <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
          <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách đơn hàng...</span>
        </div>

        <div v-else-if="ordersList.length === 0" class="py-12 text-center text-white/30 font-bold text-[14px]">
          Không tìm thấy đơn hàng nào.
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full border-collapse text-left">
            <thead>
              <tr class="border-b border-white/10 text-white/40 text-[12px] font-bold uppercase tracking-wider">
                <th class="pb-4 pr-4">Mã đơn hàng</th>
                <th class="pb-4 px-4">Khách hàng</th>
                <th class="pb-4 px-4">Thông tin vé</th>
                <th class="pb-4 px-4 text-right">Tổng tiền</th>
                <th class="pb-4 px-4">Trạng thái</th>
                <th class="pb-4 pl-4 text-center">Chi tiết</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-white/5">
              <template v-for="order in ordersList" :key="order.orderId">
                <tr class="text-[13px] text-white/80 hover:bg-white/[0.01] transition-colors align-middle">
                  <td class="py-4 pr-4 font-bold text-white/60 font-mono text-xs">{{ order.orderId.substring(0, 8) }}...</td>
                  <td class="py-4 px-4">
                    <div class="flex flex-col">
                      <span class="font-bold text-white/90">{{ order.customerName }}</span>
                      <span class="text-xs text-white/40">{{ order.customerEmail }} · {{ order.customerPhone }}</span>
                    </div>
                  </td>
                  <td class="py-4 px-4">
                    <div class="flex flex-col gap-0.5">
                      <span class="font-medium text-white/70">{{ formatEventDate(order.showtimeStartAt) }}</span>
                      <span class="text-[11px] text-white/40 font-bold uppercase tracking-wider">{{ order.paymentMethod }}</span>
                    </div>
                  </td>
                  <td class="py-4 px-4 text-right font-black text-white">{{ formatCurrency(order.totalPrice) }}</td>
                  <td class="py-4 px-4">
                    <span 
                      class="px-2.5 py-1 rounded-full text-[10px] font-black uppercase tracking-wider whitespace-nowrap"
                      :class="getStatusBadgeClass(order.status)"
                    >
                      {{ getStatusText(order.status) }}
                    </span>
                  </td>
                  <td class="py-4 pl-4 text-center">
                    <button 
                      @click="toggleOrderExpand(order.orderId)"
                      class="text-primary hover:text-white transition-colors font-bold text-[12px] cursor-pointer"
                    >
                      {{ expandedOrders.includes(order.orderId) ? 'Thu gọn' : 'Xem chi tiết' }}
                    </button>
                  </td>
                </tr>

                <!-- Expanded Items Row -->
                <tr v-if="expandedOrders.includes(order.orderId)">
                  <td colspan="6" class="bg-[#0A0F0D]/60 p-4 border-l-2 border-primary">
                    <div class="space-y-3">
                      <div class="text-[11px] font-black text-white/40 uppercase tracking-widest">Chi tiết các loại vé mua:</div>
                      <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                        <div v-for="(item, idx) in order.items" :key="idx" class="p-3 bg-[#141B16] border border-white/5 rounded-xl flex flex-col justify-between">
                          <div>
                            <span class="text-[11px] font-black text-primary uppercase tracking-widest">Vé {{ idx + 1 }}</span>
                            <div class="font-bold text-white text-[13px] mt-1">{{ item.ticketTypeName }}</div>
                            <div v-if="item.seatName" class="text-xs text-white/50 mt-0.5">Vị trí: Hàng {{ item.rowName }} - Ghế {{ item.seatName }}</div>
                          </div>
                          <div class="flex items-center justify-between mt-3 pt-2 border-t border-white/5">
                            <span class="text-xs text-white/40 font-medium">Số lượng: <strong class="text-white">{{ item.quantity }}</strong></span>
                            <span class="font-black text-white text-[13px]">{{ formatCurrency(item.price) }}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  </td>
                </tr>
              </template>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="flex items-center justify-between mt-8 pt-6 border-t border-white/5">
          <div class="text-[12px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ currentPage }}</span> / <span class="text-white font-bold">{{ totalPages }}</span>
          </div>
          
          <div class="flex items-center gap-2">
            <!-- Prev -->
            <button 
              @click="changePage(currentPage - 1)"
              :disabled="currentPage === 1"
              class="group flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <PhCaretLeft weight="bold" class="text-[14px] text-white/50 group-hover:text-white group-hover:-translate-x-0.5 transition-all" />
              <span class="text-[12px] font-bold text-white/50 group-hover:text-white transition-colors">Trước</span>
            </button>

            <!-- Page Numbers -->
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

            <!-- Next -->
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
import { Line } from 'vue-chartjs'
import { getEventReport, getEventOrders, getEventChartData } from '../../services/order.service'
import { requestPayout, getEventPayoutStatus } from '../../services/organizer-wallet.service'
import { getEventClickTrend } from '../../services/insights.service'
import { getEventSuggestions } from '../../services/ai-insights.service'
import { addToast } from '../../stores/adminStore'
import { getErrorMessage } from '../../utils/apiError'
import { createAreaGradient } from '../../lib/chartSetup'
import BaseButton from '../../components/ui/BaseButton.vue'
import TrendMiniChart from '../../components/organizer/TrendMiniChart.vue'
import AiSuggestionSection from '../../components/organizer/AiSuggestionSection.vue'
import {
  PhSpinner, PhWarningCircle, PhArrowLeft, PhCoins,
  PhTicket, PhUsers, PhChartPie, PhMagnifyingGlass,
  PhCaretLeft, PhCaretRight, PhHandCoins, PhCheckCircle, PhClock
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const reportData = ref(null)
const isLoading = ref(true)
const error = ref('')
const isRequestingPayout = ref(false)
const payoutStatus = ref(null)

// Orders list properties
const searchKeyword = ref('')
const currentPage = ref(1)
const totalPages = ref(1)
const pageSize = 10
const isLoadingOrders = ref(false)
const ordersList = ref([])
const expandedOrders = ref([])

// Chart properties
const chartRange = ref('30d')
const isLoadingChart = ref(false)
const chartData = ref([])

const fetchChart = async () => {
  isLoadingChart.value = true
  try {
    const res = await getEventChartData(route.params.id, chartRange.value)
    if (res && res.success) {
      chartData.value = res.data || []
    } else {
      chartData.value = []
    }
  } catch (err) {
    console.error('Error fetching chart data:', err)
    addToast(getErrorMessage(err, 'Không thể tải dữ liệu biểu đồ.'), 'error')
    chartData.value = []
  } finally {
    isLoadingChart.value = false
  }
}

const setChartRange = async (range) => {
  if (chartRange.value === range) return
  chartRange.value = range
  await fetchChart()
}

// Engagement (click trend) properties
const clickTrendRange = ref('30d')
const isLoadingClickTrend = ref(false)
const clickTrend = ref([])

const fetchClickTrend = async () => {
  isLoadingClickTrend.value = true
  try {
    const res = await getEventClickTrend(route.params.id, { range: clickTrendRange.value })
    if (res && res.success) {
      clickTrend.value = res.data || []
    } else {
      clickTrend.value = []
    }
  } catch (err) {
    console.error('Error fetching event click trend:', err)
    addToast(getErrorMessage(err, 'Không thể tải xu hướng lượt xem.'), 'error')
    clickTrend.value = []
  } finally {
    isLoadingClickTrend.value = false
  }
}

const setClickTrendRange = async (range) => {
  if (clickTrendRange.value === range) return
  clickTrendRange.value = range
  await fetchClickTrend()
}

const formatClickTrendLabel = (point) => {
  if (!point?.date) return ''
  const d = new Date(point.date)
  return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}`
}

const revenueChartJsData = computed(() => ({
  labels: chartData.value.map(d => d.legend),
  datasets: [
    {
      data: chartData.value.map(d => d.totalAmount),
      borderColor: '#00C853',
      borderWidth: 3,
      pointRadius: 0,
      pointHoverRadius: 5,
      pointHoverBackgroundColor: '#00C853',
      pointHoverBorderColor: '#ffffff',
      pointHoverBorderWidth: 2,
      tension: 0.35,
      fill: true,
      backgroundColor: (context) => {
        const { ctx, chartArea } = context.chart
        if (!chartArea) return '#00C85320'
        return createAreaGradient(ctx, chartArea, '#00C853')
      }
    }
  ]
}))

const revenueChartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  interaction: { mode: 'index', intersect: false },
  scales: {
    x: {
      grid: { display: false },
      border: { display: false },
      ticks: {
        color: 'rgba(255,255,255,0.3)',
        font: { size: 9, weight: 'bold' },
        maxRotation: 0,
        autoSkip: true,
        maxTicksLimit: 10,
        callback: function (value) {
          const label = this.getLabelForValue(value)
          return chartRange.value === '24h' ? label.split(' ')[0] : label
        }
      }
    },
    y: {
      grid: { color: 'rgba(255,255,255,0.03)' },
      border: { display: false },
      beginAtZero: true,
      ticks: {
        color: 'rgba(255,255,255,0.2)',
        font: { size: 9, weight: 'bold' },
        maxTicksLimit: 5,
        callback: (value) => formatCurrencyCompact(value)
      }
    }
  },
  plugins: {
    legend: { display: false },
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
      displayColors: false,
      callbacks: {
        title: (items) => chartData.value[items[0].dataIndex]?.legend || '',
        label: (item) => {
          const point = chartData.value[item.dataIndex]
          return [
            `Doanh thu: ${formatCurrency(point.totalAmount)}`,
            `Vé đã bán: ${point.ticketSold} vé`
          ]
        }
      }
    }
  }
}))

const fetchOrders = async () => {
  isLoadingOrders.value = true
  try {
    const params = {
      pageNumber: currentPage.value,
      pageSize: pageSize
    }
    if (searchKeyword.value.trim()) {
      params.search = searchKeyword.value.trim()
    }
    const res = await getEventOrders(route.params.id, params)
    if (res && res.success && res.data) {
      ordersList.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      currentPage.value = res.data.pageNumber || 1
    } else {
      ordersList.value = []
      totalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching event orders:', err)
    addToast(getErrorMessage(err, 'Không thể tải danh sách đơn hàng.'), 'error')
    ordersList.value = []
  } finally {
    isLoadingOrders.value = false
  }
}

const handleRequestPayout = async () => {
  isRequestingPayout.value = true
  try {
    const res = await requestPayout(route.params.id)
    if (res && res.success) {
      addToast(res.message || 'Đã gửi yêu cầu giải ngân thành công.', 'success')
      await fetchPayoutStatus()
    } else {
      addToast(res?.message || 'Không thể gửi yêu cầu giải ngân.', 'error')
    }
  } catch (err) {
    console.error('Error requesting payout:', err)
    const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra khi gửi yêu cầu giải ngân.'
    addToast(errorMsg, 'error')
  } finally {
    isRequestingPayout.value = false
  }
}

const fetchPayoutStatus = async () => {
  try {
    const res = await getEventPayoutStatus(route.params.id)
    if (res && res.success && res.data) {
      payoutStatus.value = res.data
    } else {
      payoutStatus.value = { hasPendingRequest: false, hasProposedPayout: false, hasAcceptedPayout: false }
    }
  } catch (err) {
    console.error('Error fetching event payout status:', err)
    payoutStatus.value = { hasPendingRequest: false, hasProposedPayout: false, hasAcceptedPayout: false }
  }
}

onMounted(async () => {
  try {
    const res = await getEventReport(route.params.id)
    if (res && res.success) {
      reportData.value = res.data
    } else {
      error.value = res?.message || 'Không thể tải báo cáo sự kiện.'
    }
  } catch (err) {
    console.error('Error fetching event report:', err)
    error.value = getErrorMessage(err, 'Lỗi kết nối khi tải dữ liệu báo cáo.')
  } finally {
    isLoading.value = false
  }

  // Also fetch chart, orders list, engagement trend, and payout status
  await Promise.all([
    fetchChart(),
    fetchOrders(),
    fetchClickTrend(),
    fetchPayoutStatus()
  ])
})

const handleSearch = () => {
  currentPage.value = 1
  fetchOrders()
}

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  fetchOrders()
}

const toggleOrderExpand = (orderId) => {
  const index = expandedOrders.value.indexOf(orderId)
  if (index > -1) {
    expandedOrders.value.splice(index, 1)
  } else {
    expandedOrders.value.push(orderId)
  }
}

const getStatusBadgeClass = (status) => {
  const s = status?.toLowerCase()
  if (s === 'paid' || s === 'completed' || s === '2' || s === '3') return 'bg-primary/20 border border-primary/30 text-primary'
  if (s === 'pending' || s === '1') return 'bg-warning/20 border border-warning/30 text-warning'
  if (s === 'cancelled' || s === 'refunded' || s === '4' || s === '5') return 'bg-danger/20 border border-danger/30 text-danger'
  return 'bg-white/5 border border-white/10 text-white/55'
}

const getStatusText = (status) => {
  const s = status?.toLowerCase()
  if (s === 'paid' || s === '2') return 'Đã thanh toán'
  if (s === 'completed' || s === '3') return 'Hoàn thành'
  if (s === 'pending' || s === '1') return 'Chờ thanh toán'
  if (s === 'cancelled' || s === '4') return 'Đã hủy'
  if (s === 'refunded' || s === '5') return 'Đã hoàn tiền'
  return status || 'Không rõ'
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

const formatCurrencyCompact = (val) => {
  if (val === undefined || val === null) return '0 ₫'
  if (val >= 1000000) {
    return (val / 1000000).toLocaleString('vi-VN', { maximumFractionDigits: 1 }) + 'M ₫'
  }
  if (val >= 1000) {
    return (val / 1000).toLocaleString('vi-VN', { maximumFractionDigits: 0 }) + 'K ₫'
  }
  return val.toLocaleString('vi-VN') + ' ₫'
}

const formatEventDate = (dateStr) => {
  if (!dateStr) return 'TBA'
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

const formatTimeOnly = (dateStr) => {
  if (!dateStr) return '--:--'
  try {
    const date = new Date(dateStr)
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false })
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
