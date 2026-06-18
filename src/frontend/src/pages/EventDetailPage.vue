<template>
  <div v-if="isLoading" class="flex flex-col pb-20 bg-[#050807] min-h-screen items-center justify-center">
    <div class="flex flex-col items-center gap-4">
      <PhSpinner class="animate-spin text-primary text-5xl" weight="bold" />
      <span class="text-white/50 text-sm font-bold uppercase tracking-widest">Đang tải thông tin sự kiện...</span>
    </div>
  </div>

  <div v-else-if="event" class="flex flex-col pb-20 bg-[#050807] min-h-screen">
    <!-- Premium Edge-to-Edge Hero Section -->
    <div class="relative w-full h-[60vh] md:h-[70vh] overflow-hidden">
      <!-- Background Image -->
      <div class="absolute inset-0 z-0">
        <img 
          :src="event.coverImageUrl || 'https://picsum.photos/seed/event-hero/1200/800'" 
          :alt="event.title" 
          class="w-full h-full object-cover scale-105"
        />
        <!-- Multi-layer Gradient Overlay -->
        <div class="absolute inset-0 bg-gradient-to-t from-[#050807] via-[#050807]/60 to-transparent"></div>
        <div class="absolute inset-0 bg-gradient-to-r from-[#050807]/80 via-[#050807]/40 to-transparent"></div>
      </div>

      <!-- Hero Content -->
      <div class="relative z-10 max-w-[1400px] mx-auto px-6 md:px-10 h-full flex flex-col justify-end pb-16">
        <div class="max-w-4xl space-y-6 animate-fade-up">
          <!-- Category Badge -->
          <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/20 border border-primary/30 backdrop-blur-md shadow-[0_0_20px_rgba(0,200,83,0.15)]">
            <span class="w-2 h-2 rounded-full bg-primary animate-pulse"></span>
            <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">{{ event.categoryName || 'Sự kiện' }}</span>
          </div>

          <h1 class="text-4xl md:text-7xl font-black font-heading text-white leading-[1.1] tracking-tight uppercase">
            {{ event.title }}
          </h1>

          <div class="flex flex-wrap items-center gap-4 text-[14px]">
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10 shadow-inner">
              <PhCalendarBlank weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">{{ formatEventDate(event.startAt) }}</span>
            </div>
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10 shadow-inner">
              <PhMapPin weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">{{ event.location?.venueName }}</span>
            </div>
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10 shadow-inner">
              <PhTicket weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">Từ {{ formatCurrency(getMinPrice()) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content Layout -->
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-20 -mt-12">
      <div class="grid grid-cols-1 lg:grid-cols-[1fr_440px] gap-12 xl:gap-16">
        
        <!-- Left Column: Details -->
        <div class="space-y-16">
          
          <!-- Event Features Bento Grid -->
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4 animate-fade-up [animation-delay:200ms]">
            <div v-for="(h, i) in highlights" :key="i" 
                 class="group p-6 bg-[#111916] border border-white/5 rounded-3xl hover:border-primary/50 transition-all duration-500 hover:-translate-y-1">
              <div class="w-12 h-12 rounded-2xl bg-white/5 flex items-center justify-center text-2xl mb-4 group-hover:scale-110 group-hover:bg-primary/20 transition-all text-white/50 group-hover:text-primary">
                <component :is="h.icon" weight="duotone" />
              </div>
              <div class="text-[13px] font-bold text-white group-hover:text-primary transition-colors leading-tight">{{ h.text }}</div>
            </div>
          </div>

          <!-- About Section -->
          <section id="about" class="animate-fade-up [animation-delay:300ms] scroll-mt-24">
            <div class="flex items-center gap-6 mb-8">
              <h2 class="font-heading text-3xl font-black text-white uppercase tracking-widest whitespace-nowrap">Giới thiệu</h2>
              <div class="h-px flex-1 bg-gradient-to-r from-white/20 to-transparent"></div>
            </div>
            <div class="prose prose-invert max-w-none">
              <!-- Render HTML safely since description contains HTML markup -->
              <div v-html="event.description || defaultDescription" class="text-[17px] leading-[1.8] text-white/70 font-medium whitespace-pre-wrap"></div>
            </div>
          </section>

          <!-- Interactive SeatMap / Venue Map Section -->
          <section id="venue" class="animate-fade-up [animation-delay:400ms] scroll-mt-24">
            <div class="flex items-center gap-6 mb-8">
              <h2 class="font-heading text-3xl font-black text-white uppercase tracking-widest whitespace-nowrap">
                {{ event.seatMapId ? 'Sơ đồ chỗ ngồi' : 'Địa điểm & Bản đồ' }}
              </h2>
              <div class="h-px flex-1 bg-gradient-to-r from-white/20 to-transparent"></div>
            </div>

            <!-- Event uses a Seatmap -->
            <template v-if="event.seatMapId">
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
                    <span v-else>Cuộn để zoom · Kéo để di chuyển sơ đồ</span>
                  </div>
                </div>

                <!-- Konva Interactive Canvas -->
                <div v-if="isLoadingSeatMap" class="h-[500px] flex flex-col items-center justify-center gap-3 bg-[#0A0F0D]">
                  <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
                  <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tạo sơ đồ chỗ ngồi...</span>
                </div>

                <div v-else-if="seatMapError" class="h-[500px] flex flex-col items-center justify-center p-6 text-center gap-4 bg-[#0A0F0D]">
                  <div class="w-12 h-12 rounded-xl bg-danger/10 flex items-center justify-center text-danger">
                    <PhWarningCircle class="text-2xl" weight="fill" />
                  </div>
                  <p class="text-white/50 text-[14px] max-w-sm">{{ seatMapError }}</p>
                </div>

                <div v-else-if="seatMapData" ref="konvaContainer" class="w-full h-[500px] cursor-grab active:cursor-grabbing bg-[#080D0B] overflow-hidden relative">
                  <v-stage :config="stageConfig" @wheel="handleWheel" @dragend="handleDragEnd">
                    <v-layer>
                      <!-- Background -->
                      <v-rect :config="{ x: 0, y: 0, width: realDimensions.width, height: realDimensions.height, fill: '#080d0a' }" />

                      <!-- Render SeatMap Zones -->
                      <template v-for="zone in seatMapData.zones" :key="zone.id">
                        <!-- Custom SVG elements -->
                        <template v-for="(el, eli) in zone.svgElements" :key="`${zone.id}-el-${eli}`">
                          <v-path v-if="el.type === 'path' && el.data" :config="buildPathConfig(el)" />
                          <v-text v-if="el.type === 'text' && el.text" :config="buildTextConfig(el)" />
                        </template>

                        <!-- Reserved seating layout (seats as circles) -->
                        <template v-for="row in zone.rows" :key="`${zone.id}-row-${row.id}`">
                          <v-circle
                            v-for="seat in row.seats"
                            :key="seat.id"
                            :config="buildSeatConfig(seat, zone)"
                            @mouseenter="onSeatEnter(seat, zone, row)"
                            @mouseleave="onSeatLeave"
                            @click="onSeatClick(seat, zone, row)"
                            @tap="onSeatClick(seat, zone, row)"
                          />
                        </template>

                        <!-- GA standing zone text tag -->
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

                    <!-- Hover details tooltip -->
                    <v-layer v-if="hoveredSeat">
                      <v-rect :config={tooltipBg} />
                      <v-text :config={tooltipText} />
                    </v-layer>
                  </v-stage>
                </div>

                <!-- Legend -->
                <div class="px-6 py-4 border-t border-white/5 flex flex-wrap gap-4 bg-[#141B16] text-[11px] font-bold text-white/50">
                  <div class="flex items-center gap-1.5">
                    <div class="w-3 h-3 rounded-full bg-primary shadow-glow"></div>
                    <span>Đang chọn</span>
                  </div>
                  <div class="flex items-center gap-1.5">
                    <div class="w-3 h-3 rounded-full bg-white/5 border border-white/30"></div>
                    <span>Ghế trống</span>
                  </div>
                  <div class="flex items-center gap-1.5">
                    <div class="w-3 h-3 rounded-full bg-red-500/30 border border-red-500/60"></div>
                    <span>Đã đặt</span>
                  </div>
                  <div class="flex items-center gap-1.5">
                    <div class="w-3 h-3 rounded-full bg-[#f97316]/30 border border-[#f97316]/60"></div>
                    <span>Đang giữ</span>
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
            </template>

            <!-- Event does NOT use a Seatmap -->
            <template v-else>
              <div class="bg-[#111916] border border-white/5 rounded-[3rem] p-2 overflow-hidden group shadow-2xl">
                <div class="relative aspect-[21/9] sm:aspect-[21/8] rounded-[2.5rem] bg-[#0A0F0D] overflow-hidden border border-white/5">
                  <div class="absolute inset-0 opacity-10 bg-[radial-gradient(circle_at_center,_white_1px,_transparent_1px)]" style="background-size: 24px 24px;"></div>
                  <div class="absolute inset-0 flex flex-col items-center justify-center text-center p-8 z-10">
                    <div class="w-16 h-16 rounded-full bg-primary/20 border border-primary/30 flex items-center justify-center mb-5 animate-bounce shadow-[0_0_30px_rgba(0,200,83,0.2)] text-primary">
                      <PhMapPin weight="fill" class="text-3xl" />
                    </div>
                    <h4 class="text-2xl font-black font-heading text-white mb-2 tracking-tight">{{ event.location?.venueName }}</h4>
                    <p class="text-white/50 text-sm max-w-sm font-medium">{{ formatLocation(event.location) }}</p>
                  </div>
                  <div class="absolute inset-0 bg-primary/10 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center cursor-pointer z-20 backdrop-blur-sm">
                    <BaseButton variant="primary" size="lg" class="shadow-2xl flex items-center gap-2">
                      <PhNavigationArrow weight="fill" /> Xem bản đồ chi tiết
                    </BaseButton>
                  </div>
                </div>
              </div>
            </template>
          </section>

          <!-- Policies Section -->
          <section id="policy" class="animate-fade-up [animation-delay:500ms] scroll-mt-24">
            <h2 class="font-heading text-3xl font-black text-white mb-8 flex items-center gap-4">
              <PhInfo weight="fill" class="text-primary" />
              Thông tin cần biết
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div v-for="(policy, idx) in policies" :key="idx" 
                   class="p-6 bg-[#111916] border border-white/5 rounded-[2rem] hover:bg-white/5 transition-all">
                <div class="text-3xl mb-5 text-white/40"><component :is="policy.icon" weight="duotone" /></div>
                <h4 class="text-[16px] font-bold text-white mb-2">{{ policy.title }}</h4>
                <p class="text-[14px] text-white/50 leading-relaxed font-medium">{{ policy.text }}</p>
              </div>
            </div>
          </section>
        </div>

        <!-- Right Column: Checkout Sidebar Card -->
        <aside class="relative">
          <div class="sticky top-32 space-y-6">
            <div class="bg-[#0A0F0D]/90 backdrop-blur-3xl border border-white/10 rounded-[3rem] p-8 lg:p-10 shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] overflow-hidden relative">
              <!-- Glow backdrop -->
              <div class="absolute -top-32 -right-32 w-64 h-64 bg-primary/10 blur-[100px] pointer-events-none rounded-full"></div>
              
              <div class="flex items-center justify-between mb-8 relative z-10">
                <h3 class="font-heading text-2xl font-black text-white uppercase tracking-tight">Hạng vé</h3>
                <div class="px-3 py-1 bg-primary/10 border border-primary/20 rounded-full text-primary text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5 shadow-sm">
                  <PhFire weight="fill" class="animate-pulse" /> Đang bán
                </div>
              </div>

              <!-- Tiers Selection -->
              <div class="space-y-3 mb-8 relative z-10">
                <button
                  v-for="(tier, i) in event.ticketTypes"
                  :key="tier.id"
                  @click="selectTierIndex(i)"
                  class="group relative w-full p-5 text-left rounded-2xl border-2 transition-all duration-300 overflow-hidden cursor-pointer"
                  :class="[
                    selectedTier === i 
                      ? 'bg-primary/10 border-primary shadow-[inset_0_1px_0_rgba(0,200,83,0.1)]' 
                      : 'bg-white/5 border-white/5 hover:border-white/20'
                  ]"
                >
                  <div class="relative z-10 flex flex-col">
                    <div class="flex justify-between items-center mb-1.5">
                      <span class="font-bold text-[15px] group-hover:text-white transition-colors" 
                            :class="selectedTier === i ? 'text-primary' : 'text-white/80'">
                        {{ tier.ticketTypeName }}
                      </span>
                      <span class="font-heading font-black text-lg" 
                            :class="selectedTier === i ? 'text-primary' : 'text-white'">
                        {{ tier.price === 0 ? 'Miễn phí' : formatCurrency(tier.price) }}
                      </span>
                    </div>
                    <span class="text-[11px] font-bold uppercase tracking-wider" :class="selectedTier === i ? 'text-primary/70' : 'text-white/30'">
                      {{ isReservedTier(tier) ? 'Bản đồ ghế' : 'Vé GA / Tự do' }} · Còn lại {{ tier.publishedQuota }} vé
                    </span>
                  </div>
                </button>
              </div>

              <!-- Ticket Options Details -->
              <div class="relative z-10 mb-8 border-t border-white/5 pt-6">
                <!-- Option 1: Selected Reserved Seating (Seatmap mode & Reserved tier selected) -->
                <template v-if="event.seatMapId && isReservedTier(activeTier)">
                  <div class="flex flex-col gap-3">
                    <span class="text-[12px] font-bold text-white/50 uppercase tracking-widest mb-1">Ghế đã chọn (tối đa 5)</span>
                    
                    <div v-if="selectedSeats.length === 0" class="py-4 text-center border border-dashed border-white/10 rounded-2xl text-white/30 text-[13px] font-medium leading-relaxed">
                      Vui lòng nhấp chọn các vị trí ghế ngồi mong muốn trực tiếp trên sơ đồ phân khu.
                    </div>
                    
                    <div v-else class="flex flex-col gap-2.5">
                      <div 
                        v-for="seat in selectedSeats" 
                        :key="seat.id"
                        class="flex items-center justify-between p-3 bg-white/5 border border-white/5 rounded-xl text-[13px]"
                      >
                        <div class="flex flex-col gap-0.5">
                          <span class="font-bold text-white">{{ seat.zoneName }} · Hàng {{ seat.rowName }}-{{ seat.seatName }}</span>
                          <span class="text-white/40 text-[11px] font-bold uppercase tracking-wider">{{ seat.ticketTypeName }}</span>
                        </div>
                        <div class="flex items-center gap-3">
                          <span class="font-heading font-black text-primary">{{ formatCurrency(seat.price) }}</span>
                          <button 
                            @click="removeSelectedSeat(seat.id)"
                            class="text-white/30 hover:text-danger transition-colors cursor-pointer p-1"
                          >
                            <PhX class="text-sm" weight="bold" />
                          </button>
                        </div>
                      </div>
                    </div>
                  </div>
                </template>

                <!-- Option 2: Quantity Selector (GA / standing / manual mode) -->
                <template v-else>
                  <div class="flex items-center justify-between p-5 bg-white/5 border border-white/5 rounded-2xl">
                    <span class="text-[13px] font-bold text-white/70 uppercase tracking-widest">Số lượng</span>
                    <div class="flex items-center gap-6">
                      <button @click="qty = Math.max(1, qty - 1)" :disabled="qty <= 1"
                              class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-white hover:border-primary hover:text-primary hover:bg-primary/10 disabled:opacity-30 disabled:hover:border-white/10 disabled:hover:text-white disabled:hover:bg-white/5 transition-all cursor-pointer">
                        <PhMinus weight="bold" />
                      </button>
                      <span class="text-lg font-black text-white w-6 text-center font-heading">{{ qty }}</span>
                      <button @click="qty = Math.min(activeTier?.maxQtyQuota || 5, qty + 1)" :disabled="qty >= (activeTier?.maxQtyQuota || 5)"
                              class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-white hover:border-primary hover:text-primary hover:bg-primary/10 disabled:opacity-30 disabled:hover:border-white/10 disabled:hover:text-white disabled:hover:bg-white/5 transition-all cursor-pointer">
                        <PhPlus weight="bold" />
                      </button>
                    </div>
                  </div>
                </template>
              </div>

              <!-- Summary & Checkout -->
              <div class="space-y-6 pt-6 border-t border-white/10 relative z-10">
                <div class="flex justify-between items-end">
                  <span class="text-[13px] text-white/50 font-bold uppercase tracking-widest pb-1">Tổng cộng</span>
                  <div class="text-3xl font-black font-heading text-primary leading-none tracking-tight">{{ formatCurrency(totalPrice) }}</div>
                </div>

                <BaseButton 
                  variant="primary" 
                  size="lg" 
                  class="w-full !rounded-2xl !py-4.5 shadow-[0_0_40px_rgba(0,200,83,0.2)] hover:shadow-[0_0_60px_rgba(0,200,83,0.4)] text-[16px] font-black flex justify-center items-center gap-2 cursor-pointer disabled:opacity-40 disabled:hover:shadow-none"
                  :disabled="isCheckoutDisabled"
                  @click="handleBuyTicket"
                >
                  <PhTicket weight="fill" />
                  Đặt vé ngay
                </BaseButton>
              </div>

              <!-- Trust elements -->
              <div class="mt-8 pt-6 border-t border-white/5 flex items-center justify-between gap-2 relative z-10">
                <div class="flex flex-col items-center gap-2 text-white/30 flex-1">
                  <PhShieldCheck class="text-2xl" weight="duotone" />
                  <span class="text-[9px] font-bold uppercase tracking-widest">Bảo mật</span>
                </div>
                <div class="flex flex-col items-center gap-2 text-white/30 flex-1 border-l border-r border-white/5">
                  <PhEnvelopeSimple class="text-2xl" weight="duotone" />
                  <span class="text-[9px] font-bold uppercase tracking-widest">Vé E-mail</span>
                </div>
                <div class="flex flex-col items-center gap-2 text-white/30 flex-1">
                  <PhLightning class="text-2xl" weight="duotone" />
                  <span class="text-[9px] font-bold uppercase tracking-widest">Tức thì</span>
                </div>
              </div>
            </div>

            <!-- Share & Like Actions -->
            <div class="flex gap-4">
              <button class="flex-1 py-4 bg-[#111916] border border-white/5 rounded-2xl hover:bg-white/5 transition-all flex items-center justify-center gap-2 text-white/70 hover:text-white group cursor-pointer font-bold text-sm">
                <PhHeart class="text-lg group-hover:text-[#f43f5e] transition-colors" weight="bold" /> Lưu sự kiện
              </button>
              <button class="flex-1 py-4 bg-[#111916] border border-white/5 rounded-2xl hover:bg-white/5 transition-all flex items-center justify-center gap-2 text-white/70 hover:text-white group cursor-pointer font-bold text-sm">
                <PhShareNetwork class="text-lg group-hover:text-primary transition-colors" weight="bold" /> Chia sẻ
              </button>
            </div>
          </div>
        </aside>
      </div>
    </div>
  </div>

  <!-- Error / Not Found State -->
  <div v-else class="flex flex-col items-center justify-center py-32 px-6 text-center animate-fade-up min-h-screen bg-[#050807]">
    <div class="w-24 h-24 bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 mb-6 shadow-inner">
      <PhWarningCircle weight="duotone" />
    </div>
    <h2 class="font-heading text-4xl font-black text-white mb-3">Không tìm thấy sự kiện</h2>
    <p class="text-white/50 max-w-md mx-auto mb-10 font-medium">{{ error || 'Sự kiện này có thể đã bị xóa hoặc không tồn tại trong hệ thống.' }}</p>
    <BaseButton variant="primary" size="lg" @click="router.push('/')">
      Quay về trang chủ
    </BaseButton>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted, nextTick, markRaw } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Stage as VStage, Layer as VLayer, Rect as VRect, Path as VPath, Text as VText, Circle as VCircle } from 'vue-konva'
import { getEventDetail } from '../services/eventService'
import { getVenues, getSeatMapDetail } from '../services/venue.service'
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { 
  PhCalendarBlank, PhMapPin, PhTicket, PhStar, PhArrowRight, 
  PhMapPinLine, PhNavigationArrow, PhImages, PhInfo, PhFire,
  PhMinus, PhPlus, PhProhibit, PhShieldCheck, PhEnvelopeSimple,
  PhLightning, PhHeart, PhShareNetwork, PhMagnifyingGlass,
  PhMusicNotes, PhCamera, PhBeerBottle, PhGift, PhCrown, PhCompass, 
  PhSuitcaseRolling, PhSpinner, PhWarningCircle, PhX
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
const selectedSeats = ref([])

const selectedTier = ref(0)
const qty = ref(1)

const konvaContainer = ref(null)

// Load Event details and conditionally fetch SeatMap layout
onMounted(async () => {
  try {
    const res = await getEventDetail(route.params.id)
    if (res && res.success && res.data) {
      event.value = res.data
      
      // If the event has a seatMapId, load the seatmap layout
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

// Dynamically resolve venueId from location venueName, then fetch seatmap details
const loadSeatMapLayout = async () => {
  isLoadingSeatMap.value = true
  seatMapError.value = ''
  try {
    const venueName = event.value.location?.venueName
    if (!venueName) {
      throw new Error('Sự kiện không có cấu hình thông tin địa điểm.')
    }
    
    // Resolve Venue ID by searching for venue name
    const venuesRes = await getVenues({ Search: venueName, PageNumber: 1, PageSize: 10 })
    if (!venuesRes || !venuesRes.success || !venuesRes.data || venuesRes.data.data.length === 0) {
      throw new Error(`Địa điểm "${venueName}" chưa được cấu hình sơ đồ ghế ngồi trên hệ thống.`)
    }
    
    const venueId = venuesRes.data.data[0].id
    
    // Fetch SeatMap details
    const seatMapRes = await getSeatMapDetail(venueId, event.value.seatMapId)
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

// Compute active tier details
const activeTier = computed(() => {
  if (event.value && event.value.ticketTypes && event.value.ticketTypes.length > 0) {
    return event.value.ticketTypes[selectedTier.value]
  }
  return null
})

// Calculate total price based on active mode
const totalPrice = computed(() => {
  if (!activeTier.value) return 0
  if (event.value?.seatMapId && isReservedTier(activeTier.value)) {
    return selectedSeats.value.reduce((sum, s) => sum + s.price, 0)
  }
  return activeTier.value.price * qty.value
})

// Check if checkout button should be disabled
const isCheckoutDisabled = computed(() => {
  if (!activeTier.value) return true
  if (event.value?.seatMapId && isReservedTier(activeTier.value)) {
    return selectedSeats.value.length === 0
  }
  return qty.value < 1
})

// Helper to determine if a tier has a reserved seating zone map
const isReservedTier = (tier) => {
  if (!tier || !seatMapData.value) return false
  const zone = seatMapData.value.zones?.find(z => z.id === tier.zoneId)
  return zone ? zone.isReservingSeat : false
}

// Get minimum ticket price for the event
const getMinPrice = () => {
  if (!event.value || !event.value.ticketTypes || event.value.ticketTypes.length === 0) return 0
  return Math.min(...event.value.ticketTypes.map(t => t.price))
}

const selectTierIndex = (idx) => {
  selectedTier.value = idx
  qty.value = 1
}

const removeSelectedSeat = (id) => {
  const index = selectedSeats.value.findIndex(s => s.id === id)
  if (index !== -1) selectedSeats.value.splice(index, 1)
}

// ── Konva Resizing Logic ──
let resizeObserver = null
let hasInitializedCenter = false

const realDimensions = computed(() => {
  if (!seatMapData.value) return { width: 0, height: 0 }
  let maxX = seatMapData.value.width || 0
  let maxY = seatMapData.value.height || 0
  
  if (seatMapData.value.width <= 100) {
    seatMapData.value.zones?.forEach(zone => {
      zone.svgElements?.forEach(el => {
        const x = parseFloat(el.x)
        const y = parseFloat(el.y)
        if (!isNaN(x) && x > maxX) maxX = x
        if (!isNaN(y) && y > maxY) maxY = y
      })
      zone.rows?.forEach(row => {
        row.seats?.forEach(seat => {
          const x = parseFloat(seat.x)
          const y = parseFloat(seat.y)
          if (!isNaN(x) && x > maxX) maxX = x
          if (!isNaN(y) && y > maxY) maxY = y
        })
      })
    })
    maxX += 100
    maxY += 100
  }
  return { width: maxX, height: maxY }
})

const stageConfig = reactive({
  width: 800,
  height: 500,
  scaleX: 1,
  scaleY: 1,
  x: 0,
  y: 0,
  draggable: true
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
        const containerH = entry.contentRect.height || 500
        
        if (containerW > 0) {
          stageConfig.width = containerW
          stageConfig.height = containerH
          
          if (!hasInitializedCenter && seatMapData.value) {
            const svgW = realDimensions.value.width || 800
            const svgH = realDimensions.value.height || 500
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

// ── Tooltip and Seat styling helper ──
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
    data: el.data || '',
    fill: el.fill || 'transparent',
    stroke: el.stroke || undefined,
    strokeWidth: el.strokeWidth || 0,
    listening: false
  }
}

function buildTextConfig(el) {
  return {
    x: el.x, y: el.y, text: el.text || '',
    fontSize: el.fontSize || 12, fontFamily: el.fontFamily || 'sans-serif',
    fill: el.fill || '#ffffff', fontStyle: 'bold', align: 'center',
    listening: false
  }
}

function getZonePrice(zoneId) {
  if (!event.value || !event.value.ticketTypes) return 0
  const tt = event.value.ticketTypes.find(t => t.zoneId === zoneId)
  return tt ? tt.price : 0
}

function getZoneTicketTypeName(zoneId) {
  if (!event.value || !event.value.ticketTypes) return 'Vé'
  const tt = event.value.ticketTypes.find(t => t.zoneId === zoneId)
  return tt ? tt.ticketTypeName : 'Vé'
}

function getSeatFillColor(seat, zone) {
  const isSelected = selectedSeats.value.some(s => s.id === seat.id)
  if (isSelected) return '#00C853' // selected green
  
  if (seat.layoutStatus === 'Đã bán' || seat.layoutStatus === 'Đã đặt') return '#ef4444' // occupied red
  if (seat.layoutStatus === 'Đang giữ') return '#f97316' // holding orange
  return '#0b0f19' // available fill
}

function buildSeatConfig(seat, zone) {
  return {
    x: seat.x,
    y: seat.y,
    radius: seat.radius || 10,
    fill: getSeatFillColor(seat, zone),
    stroke: zone.color,
    strokeWidth: 2,
    id: seat.id
  }
}

function onSeatEnter(seat, zone, row) {
  const price = getZonePrice(zone.id)
  hoveredSeat.value = {
    x: seat.x,
    y: seat.y,
    zoneName: zone.zoneName,
    rowName: row.rowName,
    seatName: seat.seatName,
    price: price
  }
}

function onSeatLeave() {
  hoveredSeat.value = null
}

function onSeatClick(seat, zone, row) {
  if (seat.layoutStatus === 'Đã bán' || seat.layoutStatus === 'Đã đặt' || seat.layoutStatus === 'Đang giữ') {
    return // occupied seats cannot be clicked
  }
  
  const idx = selectedSeats.value.findIndex(s => s.id === seat.id)
  if (idx !== -1) {
    selectedSeats.value.splice(idx, 1)
  } else {
    if (selectedSeats.value.length >= 5) {
      store.toast = { message: 'Bạn chỉ được phép chọn tối đa 5 vé ghế ngồi.', icon: '⚠️' }
      return
    }
    const price = getZonePrice(zone.id)
    const ticketTypeName = getZoneTicketTypeName(zone.id)
    
    // Select seat and automatically focus selected tier index
    const tierIndex = event.value.ticketTypes.findIndex(t => t.zoneId === zone.id)
    if (tierIndex !== -1 && selectedTier.value !== tierIndex) {
      selectedTier.value = tierIndex
    }
    
    selectedSeats.value.push({
      id: seat.id,
      seatName: seat.seatName,
      seatCode: seat.seatCode,
      zoneId: zone.id,
      zoneName: zone.zoneName,
      rowName: row.rowName,
      price: price,
      ticketTypeName: ticketTypeName
    })
  }
}

// ── Styling, Category Icons, Formatting Helpers ──
const highlights = computed(() => {
  const cat = event.value?.categoryName?.toLowerCase()
  if (cat?.includes('nhạc') || cat?.includes('concert')) {
    return [
      { icon: markRaw(PhMusicNotes), text: 'Âm thanh vòm' },
      { icon: markRaw(PhCamera), text: 'Khu Photo-zone' },
      { icon: markRaw(PhBeerBottle), text: 'Khu ẩm thực' },
      { icon: markRaw(PhGift), text: 'Quà lưu niệm độc quyền' },
    ]
  }
  return [
    { icon: markRaw(PhCrown), text: 'Dịch vụ VIP' },
    { icon: markRaw(PhCamera), text: 'Ghi lại hình ảnh' },
    { icon: markRaw(PhCompass), text: 'Hướng dẫn tại quầy' },
    { icon: markRaw(PhSuitcaseRolling), text: 'Vận hành tận tâm' },
  ]
})

const policies = [
  { icon: markRaw(PhTicket), title: 'Chính sách vé', text: 'Vé không được hoàn trả hoặc trao đổi sau khi đã thanh toán thành công.' },
  { icon: markRaw(PhCalendarBlank), title: 'Thời gian check-in', text: 'Cổng soát vé sẽ mở 60 phút trước giờ biểu diễn. Vui lòng check-in sớm.' },
  { icon: markRaw(PhShieldCheck), title: 'Độ tuổi quy định', text: 'Sự kiện khuyến cáo phù hợp cho người xem từ 16 tuổi trở lên.' },
  { icon: markRaw(PhInfo), title: 'Nội quy chung', text: 'Trang phục gọn gàng lịch sự. Không mang thức ăn, thức uống ngoài vào.' },
]

const defaultDescription = computed(() =>
  `Chào mừng bạn đến với sự kiện ${event.value?.title || 'chi tiết sự kiện'}. Một trải nghiệm đẳng cấp đang chờ đón bạn.`
)

const formatEventDate = (dateStr) => {
  if (!dateStr) return 'TBA'
  try {
    const date = new Date(dateStr)
    const time = date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false })
    const dayOfWeek = date.getDay()
    const weekdayMap = {
      0: 'Chủ nhật', 1: 'Thứ 2', 2: 'Thứ 3', 3: 'Thứ 4', 4: 'Thứ 5', 5: 'Thứ 6', 6: 'Thứ 7'
    }
    const weekday = weekdayMap[dayOfWeek]
    const day = date.getDate().toString().padStart(2, '0')
    const month = (date.getMonth() + 1).toString().padStart(2, '0')
    const year = date.getFullYear()
    return `${time}, ${weekday}, Ngày ${day}/${month}/${year}`
  } catch (e) {
    return dateStr
  }
}

const formatLocation = (loc) => {
  if (!loc) return 'Chưa xác định'
  const parts = []
  if (loc.addressLine) parts.push(loc.addressLine)
  if (loc.ward) parts.push(loc.ward)
  if (loc.district) parts.push(loc.district)
  if (loc.provinceCity) parts.push(loc.provinceCity)
  return parts.join(', ')
}

const formatCurrency = (amount) => {
  if (amount === 0) return 'Miễn phí'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount)
}

const handleBuyTicket = () => {
  if (!event.value || !activeTier.value) return
  
  if (event.value.seatMapId && isReservedTier(activeTier.value)) {
    // Checkout reserved seating tickets
    store.toast = {
      message: `Đặt thành công ${selectedSeats.value.length} vé ghế ngồi (${selectedSeats.value.map(s => s.rowName + '-' + s.seatName).join(', ')}). Vui lòng check email nhận vé!`,
      icon: '🎉'
    }
    selectedSeats.value = []
  } else {
    // Checkout GA / standing / manual tickets
    store.toast = {
      message: `Đặt thành công ${qty.value} vé ${activeTier.value.ticketTypeName} cho sự kiện "${event.value.title}". Vui lòng check email nhận vé!`,
      icon: '🎉'
    }
    qty.value = 1
  }
}
</script>

<style scoped>
.pb-safe {
  padding-bottom: env(safe-area-inset-bottom, 1rem);
}
</style>
