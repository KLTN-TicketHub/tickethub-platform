<template>
  <div v-if="isLoading" class="flex flex-col pb-20 bg-[#050807] min-h-[80vh] items-center justify-center">
    <div class="flex flex-col items-center gap-4">
      <PhSpinner class="animate-spin text-primary text-5xl" weight="bold" />
      <span class="text-white/50 text-sm font-bold uppercase tracking-widest">Đang tải thông tin sự kiện...</span>
    </div>
  </div>

  <div v-else-if="event" class="flex flex-col pb-20 bg-[#050807] min-h-screen">
    <!-- Hero Section -->
    <div class="relative w-full h-[50vh] md:h-[60vh] overflow-hidden">
      <div class="absolute inset-0 z-0">
        <img 
          :src="event.coverImageUrl || 'https://picsum.photos/seed/event-hero/1200/800'" 
          :alt="event.title" 
          class="w-full h-full object-cover scale-105"
        />
        <div class="absolute inset-0 bg-gradient-to-t from-[#050807] via-[#050807]/80 to-transparent"></div>
        <div class="absolute inset-0 bg-gradient-to-r from-[#050807]/90 via-[#050807]/50 to-transparent"></div>
      </div>

      <div class="relative z-10 max-w-[1400px] mx-auto px-6 md:px-10 h-full flex flex-col justify-end pb-12">
        <div class="max-w-4xl space-y-6 animate-fade-up">
          <div class="flex items-center gap-3">
            <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/20 border border-primary/30 backdrop-blur-md shadow-[0_0_20px_rgba(0,200,83,0.15)]">
              <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">{{ event.categoryName || 'Sự kiện' }}</span>
            </div>
            <div 
              class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full backdrop-blur-md transition-colors"
              :class="event.status === 'Bị từ chối' ? 'bg-[#ef4444]/20 border border-[#ef4444]/30 shadow-[0_0_20px_rgba(239,68,68,0.15)]' : 'bg-white/10 border border-white/20'"
            >
              <span 
                class="text-[11px] font-black uppercase tracking-[0.2em]"
                :class="event.status === 'Bị từ chối' ? 'text-[#ef4444]' : 'text-white'"
              >
                {{ event.status || 'Chờ duyệt' }}
              </span>
            </div>
          </div>

          <h1 class="text-4xl md:text-6xl font-black font-heading text-white leading-[1.1] tracking-tight uppercase">
            {{ event.title }}
          </h1>

          <div class="flex flex-wrap items-center gap-4 text-[14px]">
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10">
              <PhCalendarBlank weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">{{ formatEventDate(event.startAt) }}</span>
            </div>
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10">
              <PhMapPin weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">{{ event.location?.venueName || 'Địa điểm chưa cập nhật' }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-20 mt-8 w-full">
      <div class="grid grid-cols-1 lg:grid-cols-[1fr_440px] gap-12 xl:gap-16 items-start">
        
        <!-- Left Column: Details & Seatmap -->
        <div class="space-y-16 min-w-0">
          
          <!-- Rejection Banner -->
          <section v-if="event.status === 'Bị từ chối' && event.reasonForRejection" class="animate-fade-up [animation-delay:100ms] -mb-4">
            <div class="relative overflow-hidden rounded-[2.5rem] p-8 border border-[#ef4444]/30 bg-[#ef4444]/5 shadow-[0_20px_40px_-15px_rgba(239,68,68,0.1)]">
              <div class="absolute -right-12 -top-12 w-64 h-64 bg-[#ef4444]/20 blur-[80px] pointer-events-none rounded-full"></div>
              <div class="flex flex-col sm:flex-row gap-6 items-start relative z-10">
                <div class="w-14 h-14 rounded-2xl bg-[#ef4444]/10 flex items-center justify-center text-[#ef4444] shrink-0 border border-[#ef4444]/20 shadow-inner">
                  <PhWarningCircle weight="fill" class="text-3xl" />
                </div>
                <div class="flex-1">
                  <h3 class="font-heading text-2xl font-black text-[#ef4444] mb-2 uppercase tracking-tight">Sự kiện bị từ chối phê duyệt</h3>
                  <p class="text-white/70 text-[15px] leading-relaxed mb-6">
                    Rất tiếc, sự kiện của bạn đã không được đội ngũ kiểm duyệt chấp thuận. Vui lòng xem lại lý do chi tiết bên dưới, chỉnh sửa lại thông tin và gửi yêu cầu phê duyệt lại.
                  </p>
                  
                  <div class="relative p-5 rounded-2xl bg-[#0A0F0D]/80 border border-[#ef4444]/10 shadow-inner overflow-hidden">
                    <div class="absolute left-0 top-0 bottom-0 w-1 bg-[#ef4444]/50"></div>
                    <div class="flex items-center gap-2 mb-2">
                      <span class="text-[11px] font-black text-[#ef4444] uppercase tracking-widest">Lý do từ chối</span>
                    </div>
                    <p class="text-white/90 text-[15px] font-medium leading-relaxed italic">
                      "{{ event.reasonForRejection }}"
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </section>

          <!-- About Section -->
          <section class="animate-fade-up [animation-delay:200ms]">
            <div class="flex items-center gap-6 mb-8">
              <h2 class="font-heading text-3xl font-black text-white uppercase tracking-widest whitespace-nowrap">Giới thiệu</h2>
              <div class="h-px flex-1 bg-gradient-to-r from-white/20 to-transparent"></div>
            </div>
            <div class="prose prose-invert max-w-none">
              <div v-html="event.description || 'Chưa có thông tin giới thiệu.'" class="text-[16px] leading-[1.8] text-white/70 font-medium whitespace-pre-wrap"></div>
            </div>
          </section>

          <!-- SeatMap Section -->
          <section v-if="event.seatMapId" class="animate-fade-up [animation-delay:300ms]">
            <div class="flex items-center gap-6 mb-8">
              <h2 class="font-heading text-3xl font-black text-white uppercase tracking-widest whitespace-nowrap">Sơ đồ ghế ngồi</h2>
              <div class="h-px flex-1 bg-gradient-to-r from-white/20 to-transparent"></div>
            </div>

            <div class="bg-[#111916] border border-white/5 rounded-[3rem] overflow-hidden flex flex-col shadow-2xl">
              <!-- Toolbar -->
              <div class="flex items-center justify-between px-6 py-4 border-b border-white/5 bg-[#141B16]">
                <div class="flex items-center gap-3">
                  <PhMapPinLine weight="fill" class="text-primary text-xl" />
                  <span class="text-white font-bold text-[14px]">{{ seatMapData?.seatMapName || 'Đang tải sơ đồ...' }}</span>
                </div>
                <div class="flex items-center gap-2 text-[12px] text-white/40 font-medium">
                  <span v-if="hoveredSeat" class="text-white/70 transition-all">
                    {{ hoveredSeat.zoneName }} · Hàng {{ hoveredSeat.rowName }} · Ghế {{ hoveredSeat.seatName }}
                  </span>
                  <span v-else>Chế độ xem trước (Chỉ xem)</span>
                </div>
              </div>

              <!-- Konva Interactive Canvas -->
              <div v-if="isLoadingSeatMap" class="h-[450px] flex flex-col items-center justify-center gap-3 bg-[#0A0F0D]">
                <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
                <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tạo sơ đồ...</span>
              </div>

              <div v-else-if="seatMapError" class="h-[450px] flex flex-col items-center justify-center p-6 text-center gap-4 bg-[#0A0F0D]">
                <div class="w-12 h-12 rounded-xl bg-danger/10 flex items-center justify-center text-danger">
                  <PhWarningCircle class="text-2xl" weight="fill" />
                </div>
                <p class="text-white/50 text-[14px] max-w-sm">{{ seatMapError }}</p>
              </div>

              <div v-else-if="seatMapData" ref="konvaContainer" class="w-full h-[450px] cursor-grab active:cursor-grabbing bg-[#080D0B] overflow-hidden relative">
                <v-stage :config="stageConfig" @wheel="handleWheel" @dragend="handleDragEnd">
                  <v-layer>
                    <!-- Background -->
                    <v-rect :config="{ x: 0, y: 0, width: realDimensions.width, height: realDimensions.height, fill: '#080d0a' }" />

                    <!-- Zones -->
                    <template v-for="zone in seatMapData.zones" :key="zone.id">
                      <!-- SVG Elements -->
                      <template v-for="(el, eli) in zone.svgElements" :key="`${zone.id}-el-${eli}`">
                        <v-path v-if="el.type === 'path' && el.data" :config="buildPathConfig(el)" />
                        <v-text v-if="el.type === 'text' && el.text" :config="buildTextConfig(el)" />
                      </template>

                      <!-- Seats -->
                      <template v-for="row in zone.rows" :key="`${zone.id}-row-${row.id}`">
                        <v-circle
                          v-for="seat in row.seats"
                          :key="seat.id"
                          :config="buildSeatConfig(seat, zone)"
                          @mouseenter="onSeatEnter(seat, zone, row)"
                          @mouseleave="onSeatLeave"
                        />
                      </template>

                      <!-- GA text tag -->
                      <template v-if="!zone.isStage && !zone.isReservingSeat && zone.isSalable">
                        <v-rect :config="{
                          x: zone.x + zone.width / 2 - 50,
                          y: zone.y + zone.height / 2 - 12,
                          width: 100, height: 24,
                          fill: 'rgba(0,0,0,0.45)', cornerRadius: 4, listening: false
                        }" />
                        <v-text :config="{
                          x: zone.x + zone.width / 2 - 48,
                          y: zone.y + zone.height / 2 - 6,
                          text: `GA · ${zone.zoneName}`,
                          fontSize: 10, fill: zone.color, fontStyle: 'bold', align: 'center', listening: false
                        }" />
                      </template>
                    </template>
                  </v-layer>

                  <!-- Tooltip Layer -->
                  <v-layer v-if="hoveredSeat">
                    <v-rect :config="tooltipBg" />
                    <v-text :config="tooltipText" />
                  </v-layer>
                </v-stage>
              </div>

              <!-- Legend -->
              <div class="px-6 py-4 border-t border-white/5 flex flex-wrap gap-4 bg-[#141B16] text-[11px] font-bold text-white/50">
                <div class="flex items-center gap-1.5">
                  <div class="w-3 h-3 rounded-full bg-[#0b0f19] border-2 border-primary"></div>
                  <span>Ghế trống</span>
                </div>
                <div class="flex items-center gap-1.5">
                  <div class="w-3 h-3 rounded-full bg-red-500/30 border border-red-500/60"></div>
                  <span>Đã bán</span>
                </div>
                <div class="ml-auto flex items-center gap-3">
                  <span class="text-white/30">Hạng vé:</span>
                  <div v-for="zone in seatMapData?.zones?.filter(z => z.isSalable && !z.isStage)" :key="zone.id" class="flex items-center gap-1">
                    <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: zone.color }"></span>
                    <span class="text-white/70">{{ zone.zoneName }}</span>
                  </div>
                </div>
              </div>
            </div>
          </section>
        </div>

        <!-- Right Column: Sidebar -->
        <aside class="relative">
          <div class="sticky top-32 space-y-6">
            <div class="bg-[#0A0F0D]/90 backdrop-blur-3xl border border-white/10 rounded-[3rem] p-8 shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] overflow-hidden relative">
              <div class="absolute -top-32 -right-32 w-64 h-64 bg-primary/10 blur-[100px] pointer-events-none rounded-full"></div>
              
              <div class="flex items-center justify-between mb-8 relative z-10">
                <h3 class="font-heading text-2xl font-black text-white uppercase tracking-tight">Cấu hình vé</h3>
                <div class="px-3 py-1 bg-white/5 border border-white/10 rounded-full text-white/50 text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5">
                  <PhTicket weight="fill" /> {{ event.ticketTypes?.length || 0 }} loại vé
                </div>
              </div>

              <!-- Ticket Types List -->
              <div class="space-y-4 relative z-10 max-h-[50vh] overflow-y-auto pr-2 custom-scrollbar">
                <div v-if="!event.ticketTypes || event.ticketTypes.length === 0" class="py-8 text-center border border-dashed border-white/10 rounded-2xl text-white/30 text-[13px] font-medium">
                  Chưa có thông tin hạng vé.
                </div>
                
                <div v-for="tier in event.ticketTypes" :key="tier.id" class="p-5 rounded-2xl bg-white/5 border border-white/5 flex flex-col gap-3 hover:border-primary/30 transition-colors">
                  <div class="flex justify-between items-start">
                    <div class="flex flex-col">
                      <span class="font-bold text-[15px] text-white">{{ tier.ticketTypeName }}</span>
                      <span class="text-[11px] font-bold text-white/40 uppercase tracking-wider mt-1">{{ isReservedTier(tier) ? 'Bản đồ ghế' : 'Vé Tự do (GA)' }}</span>
                    </div>
                    <span class="font-heading font-black text-lg text-primary">{{ formatCurrency(tier.price) }}</span>
                  </div>
                  
                  <div class="h-px w-full bg-white/5 my-1"></div>
                  
                  <div class="flex items-center justify-between">
                    <span class="text-white/50 text-[12px] font-bold">Số lượng phát hành:</span>
                    <span class="text-white font-black text-[14px]">{{ tier.publishedQuota }} vé</span>
                  </div>
                  <div class="flex items-center justify-between">
                    <span class="text-white/50 text-[12px] font-bold">Giới hạn mỗi đơn:</span>
                    <span class="text-white font-black text-[14px]">{{ tier.maxQtyQuota }} vé</span>
                  </div>
                </div>
              </div>

              <!-- Actions -->
              <div class="mt-8 pt-6 border-t border-white/10 relative z-10 flex flex-col gap-3">
                <BaseButton variant="outline" class="w-full !rounded-xl !py-3 flex items-center justify-center gap-2" @click="handleEdit">
                  <PhPencilSimple weight="bold" />
                  Chỉnh sửa sự kiện
                </BaseButton>
                <BaseButton variant="primary" class="w-full !rounded-xl !py-3 flex items-center justify-center gap-2" @click="handleOrders">
                  <PhReceipt weight="bold" />
                  Xem đơn hàng
                </BaseButton>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </div>
  </div>

  <!-- Error State -->
  <div v-else class="flex flex-col items-center justify-center py-32 px-6 text-center min-h-[80vh] bg-[#050807]">
    <div class="w-24 h-24 bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 mb-6 shadow-inner">
      <PhWarningCircle weight="duotone" />
    </div>
    <h2 class="font-heading text-3xl font-black text-white mb-3">Lỗi tải thông tin</h2>
    <p class="text-white/50 max-w-md mx-auto mb-10 font-medium">{{ error || 'Sự kiện không tồn tại hoặc bạn không có quyền xem.' }}</p>
    <BaseButton variant="primary" @click="router.push('/organizer')">Quay lại Tổng quan</BaseButton>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Stage as VStage, Layer as VLayer, Rect as VRect, Path as VPath, Text as VText, Circle as VCircle } from 'vue-konva'
import { getOrganizerEventDetail } from '../../services/eventService'
import { getOrganizerVenues, getOrganizerSeatMapDetail } from '../../services/venue.service'
import { store } from '../../stores/eventStore'
import BaseButton from '../../components/ui/BaseButton.vue'
import { 
  PhCalendarBlank, PhMapPin, PhTicket, PhMapPinLine, PhSpinner, 
  PhWarningCircle, PhPencilSimple, PhReceipt
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const event = ref(null)
const isLoading = ref(true)
const error = ref('')

const seatMapData = ref(null)
const isLoadingSeatMap = ref(false)
const seatMapError = ref('')
const hoveredSeat = ref(null)
const konvaContainer = ref(null)

onMounted(async () => {
  try {
    const res = await getOrganizerEventDetail(route.params.id)
    if (res && res.success && res.data) {
      event.value = res.data
      if (event.value.seatMapId) {
        await loadSeatMapLayout()
      }
    } else {
      error.value = res?.message || 'Không thể lấy thông tin sự kiện.'
    }
  } catch (err) {
    error.value = 'Lỗi kết nối khi tải chi tiết sự kiện.'
    console.error(err)
  } finally {
    isLoading.value = false
  }
})

const loadSeatMapLayout = async () => {
  isLoadingSeatMap.value = true
  seatMapError.value = ''
  try {
    const venueName = event.value.location?.venueName
    if (!venueName) throw new Error('Sự kiện không có cấu hình thông tin địa điểm.')
    
    const venuesRes = await getOrganizerVenues({ Search: venueName, PageNumber: 1, PageSize: 10 })
    if (!venuesRes || !venuesRes.success || !venuesRes.data || venuesRes.data.data.length === 0) {
      throw new Error(`Địa điểm "${venueName}" chưa được cấu hình sơ đồ ghế ngồi trên hệ thống.`)
    }
    
    const venueId = venuesRes.data.data[0].id
    const seatMapRes = await getOrganizerSeatMapDetail(venueId, event.value.seatMapId)
    if (seatMapRes && seatMapRes.success && seatMapRes.data) {
      seatMapData.value = seatMapRes.data
      initKonvaResize()
    } else {
      throw new Error(seatMapRes?.message || 'Tải dữ liệu sơ đồ ghế ngồi thất bại.')
    }
  } catch (err) {
    console.error('Error loading seatmap details:', err)
    seatMapError.value = err.message || 'Không thể tải sơ đồ ghế ngồi.'
  } finally {
    isLoadingSeatMap.value = false
  }
}

// Helpers
const isReservedTier = (tier) => {
  if (!tier || !seatMapData.value) return false
  const zone = seatMapData.value.zones?.find(z => z.id === tier.zoneId)
  return zone ? zone.isReservingSeat : false
}

const formatCurrency = (val) => {
  if (val === undefined || val === null) return '0 ₫'
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

const handleEdit = () => {
  store.toast = { message: 'Chức năng chỉnh sửa sự kiện đang được phát triển.', icon: '✨' }
}

const handleOrders = () => {
  store.toast = { message: 'Chức năng xem đơn hàng đang được phát triển.', icon: '✨' }
}

// Konva
let resizeObserver = null
let hasInitializedCenter = false

const realDimensions = computed(() => {
  if (!seatMapData.value) return { width: 0, height: 0 }
  let maxX = seatMapData.value.width || 0
  let maxY = seatMapData.value.height || 0
  
  if (seatMapData.value.width <= 100) {
    seatMapData.value.zones?.forEach(zone => {
      zone.svgElements?.forEach(el => {
        const x = parseFloat(el.x); const y = parseFloat(el.y)
        if (!isNaN(x) && x > maxX) maxX = x
        if (!isNaN(y) && y > maxY) maxY = y
      })
      zone.rows?.forEach(row => {
        row.seats?.forEach(seat => {
          const x = parseFloat(seat.x); const y = parseFloat(seat.y)
          if (!isNaN(x) && x > maxX) maxX = x
          if (!isNaN(y) && y > maxY) maxY = y
        })
      })
    })
    maxX += 100; maxY += 100
  }
  return { width: maxX, height: maxY }
})

const stageConfig = reactive({
  width: 800, height: 450, scaleX: 1, scaleY: 1, x: 0, y: 0, draggable: true
})

function handleWheel(e) {
  e.evt.preventDefault()
  const scaleBy = 1.1
  const stage = e.target.getStage()
  const oldScale = stage.scaleX()
  const pointer = stage.getPointerPosition()

  const mousePointTo = {
    x: (pointer.x - stage.x()) / oldScale,
    y: (pointer.y - stage.y()) / oldScale,
  }

  const direction = e.evt.deltaY > 0 ? -1 : 1
  const newScale = direction > 0 ? oldScale * scaleBy : oldScale / scaleBy
  if (newScale > 10 || newScale < 0.1) return

  stageConfig.scaleX = newScale
  stageConfig.scaleY = newScale
  stageConfig.x = pointer.x - mousePointTo.x * newScale
  stageConfig.y = pointer.y - mousePointTo.y * newScale
}

function handleDragEnd(e) {
  stageConfig.x = e.target.x()
  stageConfig.y = e.target.y()
}

function initKonvaResize() {
  nextTick(() => {
    if (konvaContainer.value && !resizeObserver) {
      resizeObserver = new ResizeObserver((entries) => {
        const entry = entries[0]
        const containerW = entry.contentRect.width
        const containerH = entry.contentRect.height || 450
        
        if (containerW > 0) {
          stageConfig.width = containerW
          stageConfig.height = containerH
          
          if (!hasInitializedCenter && seatMapData.value) {
            const svgW = realDimensions.value.width || 800
            const svgH = realDimensions.value.height || 450
            const scale = Math.min(containerW / svgW, containerH / svgH) * 0.95
            
            stageConfig.scaleX = scale
            stageConfig.scaleY = scale
            stageConfig.x = (containerW - svgW * scale) / 2
            stageConfig.y = (containerH - svgH * scale) / 2
            hasInitializedCenter = true
          }
        }
      })
      resizeObserver.observe(konvaContainer.value)
    }
  })
}

onUnmounted(() => {
  if (resizeObserver) resizeObserver.disconnect()
})

const tooltipBg = computed(() => {
  if (!hoveredSeat.value) return { visible: false }
  const { x, y } = hoveredSeat.value
  return {
    x: x + 14, y: y - 34,
    width: 190, height: 26,
    fill: 'rgba(0,0,0,0.85)', cornerRadius: 6, visible: true
  }
})

const tooltipText = computed(() => {
  if (!hoveredSeat.value) return { visible: false }
  const { x, y, zoneName, rowName, seatName, price } = hoveredSeat.value
  return {
    x: x + 20, y: y - 26,
    text: `${zoneName} · Hàng ${rowName}-${seatName} (${formatCurrency(price)})`,
    fontSize: 11, fill: '#ffffff', fontStyle: 'bold', visible: true
  }
})

function buildPathConfig(el) {
  return {
    data: el.data || '', fill: el.fill || 'transparent',
    stroke: el.stroke || undefined, strokeWidth: el.strokeWidth || 0, listening: false
  }
}

function buildTextConfig(el) {
  return {
    x: el.x, y: el.y, text: el.text || '',
    fontSize: el.fontSize || 12, fontFamily: el.fontFamily || 'sans-serif',
    fill: el.fill || '#ffffff', fontStyle: 'bold', align: 'center', listening: false
  }
}

function getZonePrice(zoneId) {
  if (!event.value || !event.value.ticketTypes) return 0
  const tt = event.value.ticketTypes.find(t => t.zoneId === zoneId)
  return tt ? tt.price : 0
}

function getSeatFillColor(seat, zone) {
  if (seat.layoutStatus === 'Đã bán' || seat.layoutStatus === 'Đã đặt') return '#ef4444' // occupied red
  return '#0b0f19' // available fill
}

function buildSeatConfig(seat, zone) {
  return {
    x: seat.x, y: seat.y, radius: seat.radius || 10,
    fill: getSeatFillColor(seat, zone),
    stroke: zone.color, strokeWidth: 2, id: seat.id,
    listening: true // Enable listening for hover in read-only mode
  }
}

function onSeatEnter(seat, zone, row) {
  const price = getZonePrice(zone.id)
  hoveredSeat.value = {
    x: seat.x, y: seat.y,
    zoneName: zone.zoneName, rowName: row.rowName, seatName: seat.seatName, price: price
  }
}

function onSeatLeave() {
  hoveredSeat.value = null
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.2);
}
</style>
