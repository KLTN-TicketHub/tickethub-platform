<template>
  <div class="flex flex-col pb-20 bg-[#0A0F0D] min-h-screen">
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 pt-8 w-full">
      <header class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-10">
        <div class="space-y-1">
          <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">Tổng quan</span>
          <h1 class="text-3xl md:text-4xl font-black font-heading text-white tracking-tight uppercase">Thống kê xu hướng</h1>
          <p class="text-white/40 text-[13px] font-medium">Xu hướng quan tâm của người dùng trên toàn bộ sự kiện của bạn</p>
        </div>

        <div class="flex bg-[#111916] p-1.5 rounded-xl border border-white/5 self-start">
          <button
            @click="setRange('7d')"
            class="px-4 py-2 rounded-lg text-xs font-bold transition-all cursor-pointer"
            :class="range === '7d' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
          >
            7 Ngày Qua
          </button>
          <button
            @click="setRange('30d')"
            class="px-4 py-2 rounded-lg text-xs font-bold transition-all cursor-pointer"
            :class="range === '30d' ? 'bg-primary text-black shadow-lg font-black' : 'text-white/50 hover:text-white'"
          >
            30 Ngày Qua
          </button>
        </div>
      </header>

      <div v-if="error" class="py-20 text-center text-white/40 font-bold">{{ error }}</div>

      <template v-else>
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
          <TrendMiniChart
            title="Lượt xem sự kiện"
            color="#00C853"
            :data="trend"
            value-key="viewCount"
            value-label="lượt xem"
            :label-fn="formatDateLabel"
            :is-loading="isLoading"
          />
          <TrendMiniChart
            title="Lượt có ý định mua vé"
            color="#818cf8"
            :data="trend"
            value-key="purchaseIntentCount"
            value-label="lượt"
            :label-fn="formatDateLabel"
            :is-loading="isLoading"
          />
        </div>

        <section class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 shadow-2xl">
          <h2 class="font-heading text-2xl font-black text-white uppercase tracking-wider mb-6">Top sự kiện được quan tâm nhất</h2>

          <div v-if="isLoading" class="py-12 flex justify-center">
            <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
          </div>
          <div v-else-if="topEvents.length === 0" class="py-12 text-center text-white/30 font-bold text-[14px]">
            Chưa có dữ liệu tương tác nào trong khoảng thời gian này.
          </div>
          <div v-else class="overflow-x-auto">
            <table class="w-full border-collapse text-left">
              <thead>
                <tr class="border-b border-white/10 text-white/40 text-[12px] font-bold uppercase tracking-wider">
                  <th class="pb-4 pr-4">Sự kiện</th>
                  <th class="pb-4 px-4 text-right">Lượt xem</th>
                  <th class="pb-4 px-4 text-right">Có ý định mua</th>
                  <th class="pb-4 pl-4 text-right">Tỷ lệ chuyển đổi</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-white/5">
                <tr v-for="ev in topEvents" :key="ev.eventId" class="text-[14px] text-white/80 hover:bg-white/[0.02] transition-colors">
                  <td class="py-4 pr-4 font-bold text-white">{{ ev.eventTitle }}</td>
                  <td class="py-4 px-4 text-right font-black text-primary">{{ ev.viewCount.toLocaleString('vi-VN') }}</td>
                  <td class="py-4 px-4 text-right font-bold text-[#818cf8]">{{ ev.purchaseIntentCount.toLocaleString('vi-VN') }}</td>
                  <td class="py-4 pl-4 text-right font-bold text-warning">{{ conversionRate(ev) }}%</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-8">
          <AiSuggestionSection
            title="Gợi ý định hướng thị trường"
            :fetcher="(r) => getMarketDirectionSuggestions({ range: r })"
            :range="range"
          />
          <AiSuggestionSection
            title="Gợi ý cho sự kiện của bạn"
            :fetcher="(r) => getPortfolioSuggestions({ range: r })"
            :range="range"
          />
        </div>
      </template>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { PhSpinner } from '@phosphor-icons/vue'
import { getOrganizerInsights } from '../../services/insights.service'
import { getMarketDirectionSuggestions, getPortfolioSuggestions } from '../../services/ai-insights.service'
import { getErrorMessage } from '../../utils/apiError'
import TrendMiniChart from '../../components/organizer/TrendMiniChart.vue'
import AiSuggestionSection from '../../components/organizer/AiSuggestionSection.vue'

const range = ref('30d')
const isLoading = ref(true)
const error = ref('')
const trend = ref([])
const topEvents = ref([])

const fetchInsights = async () => {
  isLoading.value = true
  try {
    const res = await getOrganizerInsights({ range: range.value })
    if (res && res.success && res.data) {
      trend.value = res.data.trend || []
      topEvents.value = res.data.topEvents || []
    } else {
      error.value = res?.message || 'Không thể tải thống kê.'
    }
  } catch (err) {
    console.error('Error fetching organizer insights:', err)
    error.value = getErrorMessage(err, 'Lỗi kết nối khi tải dữ liệu thống kê.')
  } finally {
    isLoading.value = false
  }
}

const setRange = (r) => {
  if (range.value === r) return
  range.value = r
  fetchInsights()
}

const conversionRate = (ev) => {
  if (!ev.viewCount) return '0.0'
  return ((ev.purchaseIntentCount / ev.viewCount) * 100).toFixed(1)
}

const formatDateLabel = (point) => {
  if (!point?.date) return ''
  const d = new Date(point.date)
  return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}`
}

onMounted(() => {
  fetchInsights()
})
</script>

<style scoped>
</style>
