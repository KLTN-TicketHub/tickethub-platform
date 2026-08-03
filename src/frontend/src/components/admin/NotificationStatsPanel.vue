<template>
  <section class="flex flex-col gap-6">
    <!-- Range filter -->
    <div class="flex flex-wrap items-center justify-between gap-4">
      <h2 class="font-heading text-2xl font-black text-white tracking-tight">Thống kê thông báo</h2>
      <div class="flex items-center gap-1 bg-white/5 border border-white/10 rounded-full p-1">
        <button
          v-for="range in ranges"
          :key="range.days"
          type="button"
          @click="selectRange(range.days)"
          class="px-4 py-1.5 rounded-full text-[12px] font-bold transition-all"
          :class="activeDays === range.days ? 'bg-primary text-black' : 'text-white/60 hover:text-white hover:bg-white/10'"
        >
          {{ range.label }}
        </button>
      </div>
    </div>

    <!-- KPI tiles -->
    <div class="grid grid-cols-2 xl:grid-cols-4 gap-4">
      <div v-for="tile in tiles" :key="tile.label" class="bg-[#141B16] border border-white/5 rounded-2xl p-5">
        <span class="text-[11px] font-bold text-white/40 uppercase tracking-widest">{{ tile.label }}</span>
        <div class="flex items-baseline gap-1.5 mt-2">
          <span class="text-3xl font-black font-heading text-white tabular-nums">{{ tile.value }}</span>
          <span v-if="tile.unit" class="text-[15px] font-bold text-white/40">{{ tile.unit }}</span>
        </div>
        <span class="text-[12px] text-muted font-medium mt-1 block">{{ tile.hint }}</span>
      </div>
    </div>

    <!-- Daily trend -->
    <div class="bg-[#141B16] border border-white/5 rounded-[2rem] p-6">
      <div class="flex flex-wrap items-center justify-between gap-4 mb-6">
        <h3 class="text-[15px] font-bold text-white">Thông báo gửi và lượt đọc theo ngày</h3>
        <div class="flex items-center gap-4">
          <span v-for="series in seriesLegend" :key="series.key" class="flex items-center gap-2">
            <span class="w-3 h-3 rounded-full" :style="{ backgroundColor: series.color }"></span>
            <span class="text-[12px] font-bold text-white/60">{{ series.label }}</span>
          </span>
        </div>
      </div>

      <div v-if="isLoading" class="h-64 flex items-center justify-center text-[13px] text-muted font-medium">
        Đang tải số liệu...
      </div>

      <div v-else-if="daily.length === 0" class="h-64 flex items-center justify-center text-[13px] text-muted italic">
        Chưa có thông báo nào trong khoảng thời gian này.
      </div>

      <div v-else class="relative w-full select-none">
        <svg class="w-full h-64" viewBox="0 0 1000 300" preserveAspectRatio="none" role="img" aria-label="Biểu đồ thông báo gửi và lượt đọc theo ngày">
          <line v-for="tick in gridTicks" :key="tick.value" x1="50" :y1="tick.y" x2="960" :y2="tick.y" stroke="rgba(255,255,255,0.05)" stroke-width="1" />
          <line x1="50" y1="250" x2="960" y2="250" stroke="rgba(255,255,255,0.12)" stroke-width="1" />

          <path v-if="daily.length > 1" :d="sentPath" fill="none" :stroke="SENT_COLOR" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" vector-effect="non-scaling-stroke" />
          <path v-if="daily.length > 1" :d="readPath" fill="none" :stroke="READ_COLOR" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" vector-effect="non-scaling-stroke" />

          <!-- Chấm luôn hiển thị ở mỗi điểm, kể cả khi chỉ có 1 ngày dữ liệu (path một điểm không vẽ được nét nào) -->
          <circle
            v-for="(point, index) in daily"
            :key="`sent-dot-${index}`"
            :cx="pointX(index)" :cy="pointY(point.sent)" r="4"
            :fill="SENT_COLOR" stroke="#141B16" stroke-width="1.5"
          />
          <circle
            v-for="(point, index) in daily"
            :key="`read-dot-${index}`"
            :cx="pointX(index)" :cy="pointY(point.readCount)" r="4"
            :fill="READ_COLOR" stroke="#141B16" stroke-width="1.5"
          />

          <line
            v-if="hoveredIndex !== null"
            :x1="pointX(hoveredIndex)" y1="40" :x2="pointX(hoveredIndex)" y2="250"
            stroke="rgba(255,255,255,0.2)" stroke-width="1" stroke-dasharray="4,4" vector-effect="non-scaling-stroke"
          />

          <template v-if="hoveredIndex !== null">
            <circle :cx="pointX(hoveredIndex)" :cy="pointY(daily[hoveredIndex].sent)" r="6" :fill="SENT_COLOR" stroke="#141B16" stroke-width="2" />
            <circle :cx="pointX(hoveredIndex)" :cy="pointY(daily[hoveredIndex].readCount)" r="6" :fill="READ_COLOR" stroke="#141B16" stroke-width="2" />
          </template>

          <rect
            v-for="(point, index) in daily"
            :key="index"
            :x="pointX(index) - hitWidth / 2"
            y="40"
            :width="hitWidth"
            height="210"
            fill="transparent"
            @mouseenter="hoveredIndex = index"
            @mouseleave="hoveredIndex = null"
          />
        </svg>

        <div class="flex justify-between mt-3 px-[5%]">
          <span v-for="label in axisLabels" :key="label.text" class="text-[11px] font-medium text-white/30">{{ label.text }}</span>
        </div>

        <div
          v-if="hoveredIndex !== null"
          class="absolute pointer-events-none bg-[#0A0F0D] border border-white/10 rounded-xl px-3.5 py-2.5 shadow-2xl min-w-[150px] z-10"
          :style="tooltipStyle"
        >
          <div class="text-[11px] font-bold text-white/40 uppercase tracking-widest mb-1.5">
            {{ formatDayLabel(daily[hoveredIndex].date, true) }}
          </div>
          <div v-for="series in seriesLegend" :key="series.key" class="flex items-center justify-between gap-4 py-0.5">
            <span class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: series.color }"></span>
              <span class="text-[12px] font-medium text-white/60">{{ series.label }}</span>
            </span>
            <span class="text-[13px] font-black text-white tabular-nums">{{ daily[hoveredIndex][series.key] }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Breakdown by type -->
    <div class="bg-[#141B16] border border-white/5 rounded-[2rem] p-6">
      <h3 class="text-[15px] font-bold text-white mb-6">Phân bố theo loại thông báo</h3>

      <div v-if="byType.length === 0" class="py-8 text-center text-[13px] text-muted italic">
        Chưa có dữ liệu.
      </div>

      <div v-else class="flex flex-col gap-5">
        <div v-for="row in byType" :key="row.type" class="flex flex-col gap-2">
          <div class="flex items-center justify-between gap-4">
            <span class="text-[13px] font-bold text-white/80">{{ describeType(row.type) }}</span>
            <span class="text-[12px] font-medium text-white/40 tabular-nums">
              {{ row.sent }} gửi · {{ row.readCount }} đọc
            </span>
          </div>
          <div class="flex flex-col gap-[2px]">
            <div class="h-2.5 rounded-r-[4px]" :style="{ width: barWidth(row.sent), backgroundColor: SENT_COLOR, minWidth: '2px' }"></div>
            <div class="h-2.5 rounded-r-[4px]" :style="{ width: barWidth(row.readCount), backgroundColor: READ_COLOR, minWidth: '2px' }"></div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { notificationService } from '../../services/notification.service'

const SENT_COLOR = '#00A040'
const READ_COLOR = '#0284C7'

const TYPE_LABELS = {
  Announcement: 'Thông báo từ Admin',
  OrderPaid: 'Đặt vé thành công',
  OrderCancelled: 'Đơn hàng bị huỷ',
  OrderRefunded: 'Hoàn tiền',
  TicketsIssued: 'Phát hành vé',
  EventPendingReview: 'Sự kiện chờ duyệt',
  EventApproved: 'Sự kiện được duyệt',
  EventRejected: 'Sự kiện bị từ chối',
  EventCancelled: 'Sự kiện bị huỷ',
  EventCancellationRequested: 'Yêu cầu huỷ sự kiện',
  EventCancellationApproved: 'Chấp thuận huỷ sự kiện',
  EventCancellationRejected: 'Từ chối huỷ sự kiện',
  PayoutRequested: 'Yêu cầu giải ngân',
  PayoutProposed: 'Đề xuất giải ngân',
  General: 'Khác'
}

const ranges = [
  { days: 7, label: '7 ngày' },
  { days: 30, label: '30 ngày' },
  { days: 90, label: '90 ngày' }
]

const seriesLegend = [
  { key: 'sent', label: 'Đã gửi', color: SENT_COLOR },
  { key: 'readCount', label: 'Lượt đọc', color: READ_COLOR }
]

const stats = ref(null)
const isLoading = ref(false)
const activeDays = ref(30)
const hoveredIndex = ref(null)

const daily = computed(() => stats.value?.daily ?? [])
const byType = computed(() => stats.value?.byType ?? [])

const maxValue = computed(() => {
  const values = daily.value.flatMap(point => [point.sent, point.readCount])
  const max = values.length > 0 ? Math.max(...values) : 0
  return max > 0 ? max : 1
})

const maxTypeValue = computed(() => {
  const values = byType.value.flatMap(row => [row.sent, row.readCount])
  const max = values.length > 0 ? Math.max(...values) : 0
  return max > 0 ? max : 1
})

const hitWidth = computed(() => (daily.value.length > 1 ? 910 / (daily.value.length - 1) : 910))

const tiles = computed(() => [
  {
    label: 'Tổng đã gửi',
    value: (stats.value?.totalSent ?? 0).toLocaleString('vi-VN'),
    hint: `${stats.value?.directSent ?? 0} đích danh · ${stats.value?.broadcastSent ?? 0} bản tin chung`
  },
  {
    label: 'Tỉ lệ đọc đích danh',
    value: stats.value?.directReadRate ?? 0,
    unit: '%',
    hint: `${stats.value?.directRead ?? 0}/${stats.value?.directSent ?? 0} thông báo đã đọc`
  },
  {
    label: 'Lượt đọc bản tin chung',
    value: (stats.value?.broadcastReadTotal ?? 0).toLocaleString('vi-VN'),
    hint: 'Không tính được tỉ lệ do không biết tổng người nhận'
  },
  {
    label: 'Người đọc',
    value: (stats.value?.distinctReaders ?? 0).toLocaleString('vi-VN'),
    hint: 'Số người đã đọc ít nhất một bản tin chung'
  }
])

const gridTicks = computed(() => {
  const max = maxValue.value
  return [0, 0.5, 1].map(ratio => ({
    value: Math.round(max * ratio),
    y: 250 - ratio * 210
  }))
})

const axisLabels = computed(() => {
  const points = daily.value
  if (points.length === 0) return []

  const step = points.length > 20 ? 5 : points.length > 10 ? 3 : 1
  return points
    .filter((_, index) => index === 0 || index === points.length - 1 || index % step === 0)
    .map(point => ({ text: formatDayLabel(point.date) }))
})

const pointX = (index) => {
  const total = daily.value.length
  return total === 1 ? 505 : 50 + (index / (total - 1)) * 910
}

const pointY = (value) => 250 - (value / maxValue.value) * 210

const sentPath = computed(() => buildPath(point => point.sent))
const readPath = computed(() => buildPath(point => point.readCount))

const buildPath = (accessor) => {
  return daily.value
    .map((point, index) => `${index === 0 ? 'M' : 'L'} ${pointX(index)} ${pointY(accessor(point))}`)
    .join(' ')
}

const tooltipStyle = computed(() => {
  const total = daily.value.length
  const ratio = total === 1 ? 0.5 : hoveredIndex.value / (total - 1)

  return {
    left: `${5 + ratio * 91}%`,
    top: '8px',
    transform: ratio > 0.6 ? 'translateX(-100%)' : 'none'
  }
})

const barWidth = (value) => `${Math.max((value / maxTypeValue.value) * 100, 0.5)}%`

const describeType = (type) => TYPE_LABELS[type] || type

const formatDayLabel = (value, withYear = false) => {
  const date = new Date(value)
  return date.toLocaleDateString('vi-VN', withYear
    ? { day: '2-digit', month: '2-digit', year: 'numeric' }
    : { day: '2-digit', month: '2-digit' })
}

const loadStats = async () => {
  isLoading.value = true
  try {
    const to = new Date()
    const from = new Date(to.getTime() - activeDays.value * 24 * 60 * 60 * 1000)

    stats.value = await notificationService.getStats({
      from: from.toISOString(),
      to: to.toISOString()
    })
  } catch (err) {
    console.error('[Notification] Không thể tải thống kê:', err)
  } finally {
    isLoading.value = false
  }
}

const selectRange = async (days) => {
  if (activeDays.value === days) return
  activeDays.value = days
  hoveredIndex.value = null
  await loadStats()
}

defineExpose({ loadStats })

onMounted(loadStats)
</script>
