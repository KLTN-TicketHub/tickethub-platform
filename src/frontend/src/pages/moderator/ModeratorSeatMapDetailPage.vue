<template>
  <div class="flex flex-col gap-6 animate-fade-up pb-12 px-2 md:px-0">
    <!-- Loading -->
    <div v-if="isLoading" class="flex flex-col gap-6">
      <div class="h-10 w-80 bg-white/5 rounded-xl animate-pulse"></div>
      <div class="h-[500px] bg-white/5 rounded-[2rem] animate-pulse"></div>
    </div>

    <template v-else-if="seatMap">
      <!-- Header -->
      <div class="flex flex-col md:flex-row md:items-start justify-between gap-4">
        <div class="flex flex-col gap-2">
          <div class="flex items-center gap-2 text-white/50 text-sm font-medium mb-1">
            <router-link to="/moderator/venues" class="hover:text-primary transition-colors">Địa điểm</router-link>
            <span>/</span>
            <router-link :to="`/moderator/venues/${venueId}/seat-maps`" class="hover:text-primary transition-colors flex items-center gap-1">
              <PhArrowLeft weight="bold" class="text-xs" />
              Sơ đồ ghế
            </router-link>
            <span>/</span>
            <span class="text-white/30 truncate max-w-[200px]">{{ seatMap.seatMapName }}</span>
          </div>
          <h1 class="font-heading text-2xl md:text-3xl font-black text-white tracking-tight leading-tight">
            {{ seatMap.seatMapName }}
          </h1>
          <div class="flex flex-wrap items-center gap-3 mt-1">
            <span class="text-xs font-mono font-bold text-white/40 bg-white/5 px-3 py-1 rounded-full">
              {{ seatMap.seatMapCode }}
            </span>
            <span class="text-xs font-bold text-white/50">
              {{ seatMap.width }}×{{ seatMap.height }}px
            </span>
            <span class="text-xs font-bold text-primary bg-primary/10 px-3 py-1 rounded-full">
              {{ totalSeats }} ghế ngồi
            </span>
            <span v-if="gaCapacity > 0" class="text-xs font-bold text-emerald-400 bg-emerald-500/10 px-3 py-1 rounded-full">
              {{ gaCapacity }} sức chứa GA
            </span>
          </div>
        </div>

        <div class="flex items-center gap-2 flex-shrink-0">
          <router-link
            :to="`/moderator/venues/${venueId}/seat-maps`"
            class="inline-flex items-center gap-2 px-4 py-2.5 text-[13px] font-bold text-white/70 bg-white/5 border border-white/10 rounded-xl hover:bg-white/10 transition-colors"
          >
            <PhArrowLeft weight="bold" />
            Quay lại
          </router-link>
        </div>
      </div>

      <!-- Main layout: Canvas + Sidebar -->
      <div class="flex flex-col xl:flex-row gap-6">
        <!-- Canvas -->
        <div class="flex-1 min-w-0">
          <div class="bg-[#080D0B] border border-white/10 rounded-[2rem] overflow-hidden flex flex-col">
            <!-- Canvas toolbar -->
            <div class="flex items-center justify-between px-6 py-4 border-b border-white/5">
              <div class="flex items-center gap-3">
                <PhMapPin weight="fill" class="text-primary" />
                <span class="text-white font-bold text-[14px]">Sơ đồ chỗ ngồi</span>
              </div>
              <div class="flex items-center gap-2 text-[12px] text-white/40 font-medium">
                <span v-if="hoveredSeat" class="text-white/70 transition-all">
                  {{ hoveredSeat.zoneName }} · Hàng {{ hoveredSeat.rowName }} · Ghế {{ hoveredSeat.seatName }}
                </span>
                <span v-else>Di chuột vào ghế để xem thông tin</span>
              </div>
            </div>

            <!-- Konva Stage -->
            <div ref="konvaContainer" class="w-full overflow-auto">
              <v-stage :config="stageConfig">
                <v-layer>
                  <!-- Background -->
                  <v-rect :config="{ x: 0, y: 0, width: stageConfig.width, height: stageConfig.height, fill: '#0b0f19' }" />

                  <!-- Zones -->
                  <template v-for="zone in seatMap.zones" :key="zone.id">
                    <!-- SVG paths and texts -->
                    <template v-for="(el, eli) in zone.svgElements" :key="`${zone.id}-el-${eli}`">
                      <v-path v-if="el.type === 'path' && el.data"
                        :config="buildPathConfig(el)" />
                      <v-text v-if="el.type === 'text' && el.text"
                        :config="buildTextConfig(el)" />
                    </template>

                    <!-- Reserved seats (circles) -->
                    <template v-for="row in zone.rows" :key="`${zone.id}-row-${row.id}`">
                      <v-circle
                        v-for="seat in row.seats"
                        :key="seat.id"
                        :config="buildSeatConfig(seat, zone)"
                        @mouseenter="onSeatEnter(seat, zone, row)"
                        @mouseleave="onSeatLeave"
                      />
                    </template>

                    <!-- GA zone label overlay -->
                    <template v-if="!zone.isStage && !zone.isReservingSeat && zone.isSalable">
                      <v-rect :config="{
                        x: zone.x + zone.width / 2 - 60,
                        y: zone.y + zone.height / 2 - 14,
                        width: 120, height: 26,
                        fill: 'rgba(0,0,0,0.35)', cornerRadius: 6, listening: false
                      }" />
                      <v-text :config="{
                        x: zone.x + zone.width / 2 - 58,
                        y: zone.y + zone.height / 2 - 8,
                        text: `GA · ${zone.capacity} người`,
                        fontSize: 11, fill: zone.color, fontStyle: 'bold', listening: false
                      }" />
                    </template>
                  </template>
                </v-layer>

                <!-- Tooltip -->
                <v-layer v-if="hoveredSeat">
                  <v-rect :config="tooltipBg" />
                  <v-text :config="tooltipText" />
                </v-layer>
              </v-stage>
            </div>

            <!-- Legend -->
            <div class="px-6 py-4 border-t border-white/5 flex flex-wrap gap-3">
              <div v-for="zone in seatMap.zones" :key="zone.id + '-legend'"
                class="flex items-center gap-1.5 text-[11px] font-bold text-white/60">
                <div class="w-2.5 h-2.5 rounded-full flex-shrink-0" :style="{ background: zone.color }"></div>
                {{ zone.zoneName }}
              </div>
              <div class="ml-auto flex items-center gap-4 text-[11px] text-white/40">
                <div class="flex items-center gap-1.5">
                  <div class="w-3 h-3 rounded-full bg-primary/20 border border-primary/60"></div>
                  Có sẵn
                </div>
                <div class="flex items-center gap-1.5">
                  <div class="w-3 h-3 rounded-full bg-red-500/20 border border-red-500/60"></div>
                  Đã đặt
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar: Zone stats -->
        <div class="xl:w-80 flex flex-col gap-4 flex-shrink-0">
          <!-- Total stats card -->
          <div class="bg-[#111916]/60 border border-white/5 rounded-2xl p-5 grid grid-cols-2 gap-4">
            <div class="flex flex-col gap-1 text-center">
              <span class="text-2xl font-black text-primary">{{ seatMap.zones.length }}</span>
              <span class="text-[11px] text-white/50 font-medium">Phân khu</span>
            </div>
            <div class="flex flex-col gap-1 text-center">
              <span class="text-2xl font-black text-white">{{ salableZones }}</span>
              <span class="text-[11px] text-white/50 font-medium">Có bán vé</span>
            </div>
            <div class="flex flex-col gap-1 text-center">
              <span class="text-2xl font-black text-[#818cf8]">{{ totalRows }}</span>
              <span class="text-[11px] text-white/50 font-medium">Tổng hàng</span>
            </div>
            <div class="flex flex-col gap-1 text-center">
              <span class="text-2xl font-black text-emerald-400">{{ totalSeats }}</span>
              <span class="text-[11px] text-white/50 font-medium">Ghế ngồi</span>
            </div>
          </div>

          <!-- Zone cards -->
          <div
            v-for="zone in seatMap.zones"
            :key="zone.id + '-card'"
            class="bg-[#111916]/60 border border-white/5 rounded-2xl p-5 flex flex-col gap-3 hover:border-white/10 transition-colors cursor-default"
          >
            <!-- Zone header -->
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <div class="w-3 h-3 rounded-full flex-shrink-0" :style="{ background: zone.color }"></div>
                <span class="font-bold text-white text-[13px] truncate">{{ zone.zoneName }}</span>
              </div>
              <span class="text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full flex-shrink-0"
                :class="zone.isStage ? 'bg-sky-500/10 text-sky-400' : zone.isReservingSeat ? 'bg-primary/10 text-primary' : 'bg-emerald-500/10 text-emerald-400'">
                {{ zone.isStage ? 'Sân khấu' : zone.isReservingSeat ? 'Ghế cố định' : 'GA' }}
              </span>
            </div>

            <!-- Zone info -->
            <div class="flex flex-col gap-1.5 text-[12px] text-white/60">
              <div v-if="zone.isReservingSeat" class="flex items-center justify-between">
                <span>Số hàng:</span>
                <span class="font-semibold text-white/80">{{ zone.rows.length }}</span>
              </div>
              <div v-if="zone.isReservingSeat" class="flex items-center justify-between">
                <span>Tổng ghế:</span>
                <span class="font-bold text-primary">{{ zone.capacity }}</span>
              </div>
              <div v-if="!zone.isReservingSeat && zone.isSalable" class="flex items-center justify-between">
                <span>Sức chứa:</span>
                <span class="font-bold text-emerald-400">{{ zone.capacity }}</span>
              </div>
              <div v-if="zone.isSalable" class="flex items-center justify-between">
                <span>Giá vé:</span>
                <span class="font-bold text-yellow-400">{{ formatPrice(zone.basePrice) }}</span>
              </div>
              <div v-if="zone.isSalable" class="flex items-center justify-between">
                <span>Thứ tự:</span>
                <span class="text-white/60">{{ zone.displayOrder }}</span>
              </div>
            </div>

            <!-- Row breakdown (for reserved zones) -->
            <div v-if="zone.isReservingSeat && zone.rows.length > 0" class="pt-2 border-t border-white/5">
              <div class="flex flex-wrap gap-1.5">
                <div
                  v-for="row in zone.rows"
                  :key="row.id"
                  class="flex items-center gap-1 px-2 py-1 bg-white/5 rounded-lg text-[11px]"
                >
                  <span class="font-black text-white/80">{{ row.rowName }}</span>
                  <span class="text-white/40">{{ row.seats.length }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Error state -->
    <div v-else-if="error" class="flex flex-col items-center justify-center py-24 gap-4">
      <div class="w-16 h-16 rounded-2xl bg-red-500/10 flex items-center justify-center text-red-400">
        <PhWarningCircle class="text-3xl" weight="duotone" />
      </div>
      <p class="text-white/60 font-medium">{{ error }}</p>
      <router-link :to="`/moderator/venues/${venueId}/seat-maps`"
        class="px-5 py-2.5 text-[13px] font-bold text-white bg-white/5 rounded-xl hover:bg-white/10 transition-colors flex items-center gap-2">
        <PhArrowLeft weight="bold" /> Quay lại
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, nextTick } from 'vue'
import { useRoute } from 'vue-router'
import { Stage as VStage, Layer as VLayer, Rect as VRect, Path as VPath, Text as VText, Circle as VCircle } from 'vue-konva'
import { getSeatMapDetail } from '../../services/venue.service.js'
import { PhArrowLeft, PhWarningCircle, PhMapPin } from '@phosphor-icons/vue'

const route = useRoute()
const venueId = route.params.id
const seatMapId = route.params.seatMapId

const seatMap = ref(null)
const isLoading = ref(true)
const error = ref('')
const hoveredSeat = ref(null)
const konvaContainer = ref(null)

// ── Computed ──
const totalSeats = computed(() =>
  seatMap.value?.zones.reduce((sum, z) => sum + (z.isReservingSeat ? z.capacity : 0), 0) ?? 0
)
const gaCapacity = computed(() =>
  seatMap.value?.zones.reduce((sum, z) => sum + (!z.isReservingSeat && z.isSalable ? z.capacity : 0), 0) ?? 0
)
const salableZones = computed(() =>
  seatMap.value?.zones.filter(z => z.isSalable).length ?? 0
)
const totalRows = computed(() =>
  seatMap.value?.zones.reduce((sum, z) => sum + z.rows.length, 0) ?? 0
)

const stageConfig = computed(() => {
  if (!seatMap.value) return { width: 800, height: 500 }
  const containerW = konvaContainer.value?.clientWidth || 900
  const svgW = seatMap.value.width || 999
  const svgH = seatMap.value.height || 666
  const scale = Math.min(containerW / svgW, 1)
  return {
    width: Math.round(svgW * scale),
    height: Math.round(svgH * scale),
    scaleX: scale,
    scaleY: scale
  }
})

const tooltipBg = computed(() => {
  if (!hoveredSeat.value) return { visible: false }
  const { x, y } = hoveredSeat.value
  return {
    x: x + 14, y: y - 34,
    width: 180, height: 26,
    fill: 'rgba(0,0,0,0.85)', cornerRadius: 6, visible: true
  }
})

const tooltipText = computed(() => {
  if (!hoveredSeat.value) return { visible: false }
  const { x, y, zoneName, rowName, seatName } = hoveredSeat.value
  return {
    x: x + 20, y: y - 26,
    text: `${zoneName} · ${rowName}-${seatName}`,
    fontSize: 12, fill: '#ffffff', fontStyle: 'bold', visible: true
  }
})

// ── Konva builders ──
function buildPathConfig(el) {
  return {
    data: el.data || '',
    fill: el.fill || 'transparent',
    stroke: el.stroke || undefined,
    strokeWidth: el.strokeWidth || 0,
    listening: false
  }
}

function buildTextConfig(el) {
  return {
    x: el.x,
    y: el.y,
    text: el.text || '',
    fontSize: el.fontSize || 12,
    fontFamily: el.fontFamily || 'sans-serif',
    fill: el.fill || '#ffffff',
    fontStyle: 'bold',
    align: 'center',
    listening: false
  }
}

// Map seat status to color
function seatStatusColor(layoutStatus, zoneColor) {
  if (layoutStatus === 'Đã bán' || layoutStatus === 'Đã đặt') return '#ef4444'
  if (layoutStatus === 'Đang giữ') return '#f97316'
  return '#0b0f19' // available fill (dark)
}

function buildSeatConfig(seat, zone) {
  return {
    x: seat.x,
    y: seat.y,
    radius: seat.radius || 10,
    fill: seatStatusColor(seat.layoutStatus, zone.color),
    stroke: zone.color,
    strokeWidth: 2,
    id: seat.svgElementId
  }
}

function onSeatEnter(seat, zone, row) {
  hoveredSeat.value = {
    x: seat.x,
    y: seat.y,
    zoneName: zone.zoneName,
    rowName: row.rowName,
    seatName: seat.seatName,
    status: seat.layoutStatus
  }
}

function onSeatLeave() {
  hoveredSeat.value = null
}

function formatPrice(val) {
  if (!val || isNaN(val)) return 'Miễn phí'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

// ── Fetch ──
onMounted(async () => {
  try {
    const res = await getSeatMapDetail(venueId, seatMapId)
    if (res?.success && res?.data) {
      seatMap.value = res.data
      await nextTick()
      // Force stage to re-read container width after DOM is ready
      konvaContainer.value?.dispatchEvent(new Event('resize'))
    } else {
      error.value = res?.message || 'Không thể tải thông tin sơ đồ ghế.'
    }
  } catch (err) {
    error.value = 'Lỗi kết nối máy chủ. Vui lòng thử lại.'
    console.error(err)
  } finally {
    isLoading.value = false
  }
})
</script>
