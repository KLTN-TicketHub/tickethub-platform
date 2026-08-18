<template>
  <div v-if="isLoading" class="flex flex-col pb-20 bg-[#0A0F0D] min-h-screen items-center justify-center">
    <div class="flex flex-col items-center gap-4">
      <PhSpinner class="animate-spin text-primary text-5xl" weight="bold" />
      <span class="text-white/50 text-sm font-bold uppercase tracking-widest">Đang tải thông tin sự kiện...</span>
    </div>
  </div>

  <div v-else-if="event" class="flex flex-col pb-20 bg-[#0A0F0D] min-h-screen">
    <!-- Ticket-Style Hero Header -->
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 pt-8 animate-fade-up">
      <div class="relative bg-[#111916] border border-white/5 rounded-[2.5rem] overflow-hidden shadow-2xl flex flex-col md:flex-row min-h-[380px] md:h-[400px] group">
        
        <!-- Left Column: Details -->
        <div class="flex-1 p-8 lg:p-10 flex flex-col justify-between relative z-10">
          <div class="space-y-4">
            <!-- Category Badge -->
            <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/20 border border-primary/30 backdrop-blur-md shadow-[0_0_20px_rgba(0,200,83,0.15)]">
              <span class="w-2 h-2 rounded-full bg-primary animate-pulse"></span>
              <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">{{ event.categoryName || 'Sự kiện' }}</span>
            </div>

            <h1 class="text-3xl lg:text-5xl font-black font-heading text-white leading-[1.2] uppercase tracking-tight line-clamp-3">
              {{ event.title }}
            </h1>

            <div class="space-y-3 pt-3">
              <div class="flex items-center gap-3 text-white/70 text-[14px]">
                <PhCalendarBlank weight="bold" class="text-primary text-xl flex-shrink-0" />
                <span class="font-bold">
                  Bắt đầu: {{ formatEventDate(event.startAt) }}
                  <span v-if="event.endAt" class="text-white/40 font-medium"> — Kết thúc: {{ formatEventDate(event.endAt) }}</span>
                </span>
              </div>
              <div v-if="event.saleOpenAt || event.saleCloseAt" class="flex items-center gap-3 text-white/70 text-[14px]">
                <PhClock weight="bold" class="text-warning text-xl flex-shrink-0" />
                <span class="font-bold">
                  Bán vé: <span class="text-primary">{{ formatEventDate(event.saleOpenAt) }}</span>
                  <span v-if="event.saleCloseAt" class="text-white/40 font-medium"> — Hạn bán: <span class="text-warning font-bold">{{ formatEventDate(event.saleCloseAt) }}</span></span>
                </span>
              </div>
              <div class="flex items-start gap-3 text-white/70 text-[14px]">
                <PhMapPin weight="bold" class="text-primary text-xl flex-shrink-0 mt-0.5" />
                <span class="font-bold line-clamp-2">{{ event.location?.venueName || event.location?.name }}</span>
              </div>
            </div>
          </div>

          <div class="pt-6 border-t border-white/5 flex flex-col sm:flex-row items-center justify-between gap-6 mt-6">
            <div class="flex flex-col">
              <span class="text-white/40 text-[11px] font-bold uppercase tracking-widest mb-1">Giá vé từ</span>
              <span class="text-3xl font-heading font-black text-primary leading-none">{{ formatCurrency(getMinPrice()) }}</span>
            </div>
            
            <button @click="scrollToSelect" 
                    class="w-full sm:w-auto px-10 py-4 font-black rounded-xl transition-all shadow-[0_0_30px_rgba(0,200,83,0.2)] text-[14px] flex items-center justify-center gap-2"
                    :class="saleStatus === 'open' ? 'bg-primary hover:bg-primary-dark text-black hover:scale-[1.02] active:scale-95 cursor-pointer' : 'bg-white/10 text-white/50 cursor-not-allowed shadow-none'"
                    :disabled="saleStatus !== 'open'">
              <PhTicket weight="fill" /> {{ saleStatus === 'upcoming' ? 'Sắp mở bán' : (saleStatus === 'closed' ? 'Ngừng bán' : 'Mua vé ngay') }}
            </button>
          </div>
        </div>

        <!-- Ticket Divider (Dashed vertical line with circular cutouts at top/bottom) -->
        <div class="hidden md:flex flex-col justify-between items-center relative w-px h-full bg-transparent z-20 shrink-0">
          <!-- Top cutout -->
          <div class="absolute -top-4 left-1/2 -translate-x-1/2 w-8 h-8 rounded-full bg-[#0A0F0D] border-b border-white/5 z-30"></div>
          <!-- Dashed Line -->
          <div class="h-full border-l border-dashed border-white/10 my-4"></div>
          <!-- Bottom cutout -->
          <div class="absolute -bottom-4 left-1/2 -translate-x-1/2 w-8 h-8 rounded-full bg-[#0A0F0D] border-t border-white/5 z-30"></div>
        </div>

        <!-- Right Column: Banner Cover -->
        <div class="w-full md:w-[45%] lg:w-[50%] h-[240px] md:h-full relative overflow-hidden shrink-0">
          <img 
            :src="event.coverImageUrl || event.image || 'https://picsum.photos/seed/event-hero/1200/800'" 
            :alt="event.title" 
            class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-1000"
          />
          <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-transparent md:bg-gradient-to-r md:from-black/20 md:via-transparent md:to-transparent"></div>
        </div>
      </div>
    </div>

    <!-- Main Content Layout -->
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-20 mt-12">
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

          <!-- Showtimes Section -->
          <section id="showtimes" v-if="event.showtimes && event.showtimes.length > 0" class="animate-fade-up [animation-delay:350ms] scroll-mt-24">
            <div class="flex items-center gap-6 mb-8">
              <h2 class="font-heading text-3xl font-black text-white uppercase tracking-widest whitespace-nowrap">Các suất chiếu</h2>
              <div class="h-px flex-1 bg-gradient-to-r from-white/20 to-transparent"></div>
            </div>
            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              <div v-for="(st, idx) in event.showtimes" :key="st.id" 
                   @click="selectedShowTimeIndex = idx"
                   class="p-5 rounded-3xl bg-[#111916] border shadow-lg relative group overflow-hidden transition-all duration-300 cursor-pointer"
                   :class="selectedShowTimeIndex === idx ? 'border-primary ring-1 ring-primary/50 -translate-y-1' : 'border-white/5 hover:border-primary/50 hover:-translate-y-1'">
                <div class="absolute inset-0 bg-gradient-to-br from-primary/10 to-transparent transition-opacity" :class="selectedShowTimeIndex === idx ? 'opacity-100' : 'opacity-0 group-hover:opacity-100'"></div>
                <div class="relative z-10 flex flex-col gap-2">
                  <span class="text-primary font-bold text-[13px] uppercase tracking-widest mb-1">Suất {{ idx + 1 }}</span>
                  <div class="flex items-center gap-2.5 text-white/80">
                    <div class="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center shrink-0">
                      <PhCalendarBlank weight="duotone" class="text-white/60 text-lg group-hover:text-primary transition-colors" />
                    </div>
                    <span class="text-[14px] font-bold">{{ formatEventDate(st.startAt).split(', N')[0] }}</span>
                  </div>
                  <div class="flex items-center gap-2.5 text-white/50 mt-1">
                    <div class="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center shrink-0">
                      <PhClock weight="duotone" class="text-white/60 text-lg group-hover:text-primary transition-colors" />
                    </div>
                    <span class="text-[13px] font-medium">{{ formatTimeOnly(st.startAt) }} - {{ formatTimeOnly(st.endAt) }}</span>
                  </div>
                </div>
              </div>
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
                <div v-if="saleStatus === 'open'" class="px-3 py-1 bg-primary/10 border border-primary/20 rounded-full text-primary text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5 shadow-sm">
                  <PhFire weight="fill" class="animate-pulse" /> Đang bán
                </div>
                <div v-else-if="saleStatus === 'upcoming'" class="px-3 py-1 bg-blue-500/10 border border-blue-500/20 rounded-full text-blue-500 text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5 shadow-sm">
                  <PhClock weight="bold" /> Sắp mở bán
                </div>
                <div v-else-if="saleStatus === 'closed'" class="px-3 py-1 bg-red-500/10 border border-red-500/20 rounded-full text-red-500 text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5 shadow-sm">
                  <PhProhibit weight="bold" /> Ngừng bán
                </div>
              </div>

              <!-- Tiers Selection -->
              <div class="space-y-3 mb-8 relative z-10">
                <button
                  v-for="(tier, i) in event.showtimes?.[selectedShowTimeIndex]?.ticketTypes || []"
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
                      {{ isReservedTier(tier) ? 'Bản đồ ghế' : 'Vé GA / Tự do' }} · Còn lại {{ tierRemainingQuotas[tier.id] ?? tier.publishedQuota }} vé
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

              <!-- Pending order banner: user has an unpaid order for this showtime -->
              <div v-if="pendingOrder" class="mb-6 p-4 rounded-2xl bg-warning/10 border border-warning/20 flex flex-col gap-3 relative z-10">
                <div class="flex items-start gap-2.5">
                  <PhWarningCircle class="text-warning text-lg shrink-0 mt-0.5" weight="fill" />
                  <div class="flex flex-col gap-0.5">
                    <span class="text-[13px] font-bold text-white">Bạn có đơn hàng đang chờ thanh toán</span>
                    <span class="text-[12px] text-white/50 font-medium">
                      {{ formatCurrency(pendingOrder.totalPrice) }} · Hết hạn giữ chỗ lúc {{ formatTimeOnly(pendingOrder.expiresAt) }}
                    </span>
                  </div>
                </div>
                <div class="flex items-center gap-2.5">
                  <BaseButton
                    variant="primary"
                    size="sm"
                    class="flex-1 !rounded-xl"
                    @click="resumePendingOrder"
                  >
                    Tiếp tục thanh toán
                  </BaseButton>
                  <BaseButton
                    variant="danger"
                    size="sm"
                    class="flex-1 !rounded-xl"
                    :loading="isCancellingPendingOrder"
                    @click="cancelPendingOrder"
                  >
                    Hủy đơn
                  </BaseButton>
                </div>
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
                  class="w-full !rounded-2xl !py-4.5 text-[16px] font-black flex justify-center items-center gap-2"
                  :class="saleStatus === 'open' ? 'shadow-[0_0_40px_rgba(0,200,83,0.2)] hover:shadow-[0_0_60px_rgba(0,200,83,0.4)] cursor-pointer disabled:opacity-40 disabled:hover:shadow-none' : '!bg-white/10 !text-white/50 cursor-not-allowed shadow-none'"
                  :disabled="isCheckoutDisabled || isCheckingOut || saleStatus !== 'open'"
                  @click="handleBuyTicket"
                >
                  <PhSpinner v-if="isCheckingOut" class="animate-spin" weight="bold"/>
                  <PhTicket v-else weight="fill"/>
                  {{ isCheckingOut ? 'Đang xử lý...' : (saleStatus === 'upcoming' ? 'Sắp mở bán' : (saleStatus === 'closed' ? 'Đã đóng bán vé' : 'Đặt vé ngay')) }}
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
  <div v-else class="flex flex-col items-center justify-center py-32 px-6 text-center animate-fade-up min-h-screen bg-[#0A0F0D]">
    <div class="w-24 h-24 bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 mb-6 shadow-inner">
      <PhWarningCircle weight="duotone" />
    </div>
    <h2 class="font-heading text-4xl font-black text-white mb-3">Không tìm thấy sự kiện</h2>
    <p class="text-white/50 max-w-md mx-auto mb-10 font-medium">{{ error || 'Sự kiện này có thể đã bị xóa hoặc không tồn tại trong hệ thống.' }}</p>
    <BaseButton variant="primary" size="lg" @click="router.push('/')">
      <PhArrowLeft weight="bold"/> Quay lại trang sự kiện
    </BaseButton>
  </div>

  <!-- Checkout Info Modal -->
  <Teleport to="body">
    <Transition name="modal-fade">
      <div
        v-if="showCheckoutModal"
        class="fixed inset-0 z-[9999] flex items-center justify-center px-4"
        @click.self="showCheckoutModal = false"
      >
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-black/70 backdrop-blur-md"></div>

        <!-- Modal Card -->
        <div class="relative z-10 w-full max-w-md bg-[#111916] border border-white/10 rounded-[2.5rem] p-8 shadow-[0_40px_120px_rgba(0,0,0,0.8)]">
          <!-- Glow -->
          <div class="absolute -top-20 left-1/2 -translate-x-1/2 w-64 h-32 bg-primary/10 blur-[80px] rounded-full pointer-events-none"></div>

          <!-- Header -->
          <div class="flex items-center justify-between mb-8">
            <div>
              <h3 class="text-xl font-black text-white font-heading">Thông tin người nhận vé</h3>
              <p class="text-white/40 text-[12px] font-medium mt-1">Kiểm tra và bổ sung thông tin để nhận vé</p>
            </div>
            <button @click="showCheckoutModal = false" class="w-9 h-9 rounded-full bg-white/5 hover:bg-white/10 flex items-center justify-center text-white/50 hover:text-white transition-all cursor-pointer">
              <PhX weight="bold" class="text-sm"/>
            </button>
          </div>

          <!-- Summary -->
          <div class="mb-6 p-4 bg-white/3 border border-white/5 rounded-2xl space-y-2">
            <div class="flex justify-between text-[12px]">
              <span class="text-white/40 font-bold uppercase tracking-wider">Sự kiện</span>
              <span class="text-white font-bold text-right max-w-[60%] line-clamp-1">{{ event?.title }}</span>
            </div>
            <div class="flex justify-between text-[12px]">
              <span class="text-white/40 font-bold uppercase tracking-wider">Số lượng</span>
              <span class="text-white font-bold">{{ checkoutSummaryText }}</span>
            </div>
            <div class="flex justify-between text-[12px]">
              <span class="text-white/40 font-bold uppercase tracking-wider">Tổng tiền</span>
              <span class="text-primary font-black font-heading">{{ formatCurrency(totalPrice) }}</span>
            </div>
          </div>

          <!-- Form Fields -->
          <div class="space-y-4">
            <!-- Customer Name -->
            <div>
              <label class="block text-[11px] font-bold text-white/50 uppercase tracking-widest mb-2">Tên người nhận vé <span class="text-primary">*</span></label>
              <input
                v-model="checkoutForm.customerName"
                type="text"
                placeholder="Nhập họ và tên đầy đủ..."
                class="w-full px-4 py-3 bg-white/5 border rounded-xl text-white placeholder-white/20 text-[14px] font-medium outline-none transition-all"
                :class="formErrors.customerName ? 'border-red-500/60 focus:border-red-500' : 'border-white/10 focus:border-primary'"
              />
              <p v-if="formErrors.customerName" class="text-red-400 text-[11px] mt-1 font-medium">{{ formErrors.customerName }}</p>
            </div>

            <!-- Customer Phone -->
            <div>
              <label class="block text-[11px] font-bold text-white/50 uppercase tracking-widest mb-2">Số điện thoại <span class="text-primary">*</span></label>
              <input
                v-model="checkoutForm.customerPhone"
                type="tel"
                placeholder="VD: 0901234567"
                class="w-full px-4 py-3 bg-white/5 border rounded-xl text-white placeholder-white/20 text-[14px] font-medium outline-none transition-all"
                :class="formErrors.customerPhone ? 'border-red-500/60 focus:border-red-500' : 'border-white/10 focus:border-primary'"
                @keyup.enter="submitCheckout"
              />
              <p v-if="formErrors.customerPhone" class="text-red-400 text-[11px] mt-1 font-medium">{{ formErrors.customerPhone }}</p>
            </div>

            <!-- Payment Method (display only) -->
            <div class="p-4 bg-primary/5 border border-primary/15 rounded-xl flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center shrink-0">
                <PhCreditCard weight="duotone" class="text-primary text-lg"/>
              </div>
              <div>
                <div class="text-white font-bold text-[13px]">Thanh toán qua VNPay</div>
                <div class="text-white/40 text-[11px] mt-0.5">Thẻ ATM / Visa / QR Code</div>
              </div>
              <PhCheckCircle class="ml-auto text-primary text-xl" weight="fill"/>
            </div>
          </div>

          <!-- Actions -->
          <div class="mt-8 flex flex-col gap-3">
            <button
              @click="submitCheckout"
              :disabled="isCheckingOut"
              class="w-full py-4 bg-primary hover:bg-primary-dark text-black font-black rounded-2xl transition-all hover:scale-[1.02] active:scale-95 text-[15px] flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50 disabled:hover:scale-100"
            >
              <PhSpinner v-if="isCheckingOut" class="animate-spin" weight="bold"/>
              <PhArrowRight v-else weight="bold"/>
              {{ isCheckingOut ? 'Đang xử lý...' : 'Xác nhận & Thanh toán' }}
            </button>
            <button @click="showCheckoutModal = false" class="w-full py-3 text-white/30 font-bold text-[13px] hover:text-white transition-colors cursor-pointer">
              Hủy bỏ
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted, nextTick, markRaw, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Stage as VStage, Layer as VLayer, Rect as VRect, Path as VPath, Text as VText, Circle as VCircle } from 'vue-konva'
import { getEventDetail, trackEventClick } from '../services/eventService'
import { getVenues, getSeatMapDetail } from '../services/venue.service'
import { getSeatStates, lockSeat, unlockSeat, getTicketInventoryState } from '../services/seat.service'
import { checkout, getMyPendingOrder, cancelOrder } from '../services/order.service'
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr'
import { getToken } from '../services/auth/token.service'
import { API_BASE_URL } from '../services/api/axios'
import { store } from '../stores/eventStore'
import { getErrorMessage } from '../utils/apiError'
import BaseButton from '../components/ui/BaseButton.vue'
import { 
  PhCalendarBlank, PhMapPin, PhTicket, PhStar, PhArrowRight, 
  PhMapPinLine, PhNavigationArrow, PhImages, PhInfo, PhFire,
  PhMinus, PhPlus, PhProhibit, PhShieldCheck, PhEnvelopeSimple,
  PhLightning, PhHeart, PhShareNetwork, PhMagnifyingGlass,
  PhMusicNotes, PhCamera, PhBeerBottle, PhGift, PhCrown, PhCompass, 
  PhSuitcaseRolling, PhSpinner, PhWarningCircle, PhX, PhClock,
  PhCreditCard, PhCheckCircle
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
const selectedShowTimeIndex = ref(0)
const qty = ref(1)
const isCheckingOut = ref(false)
const pendingOrder = ref(null)
const isCancellingPendingOrder = ref(false)
// Checkout modal state
const showCheckoutModal = ref(false)
const checkoutForm = reactive({ customerName: '', customerPhone: '' })
const formErrors = reactive({ customerName: '', customerPhone: '' })
// Map seatId -> timeoutId for client-side 58s auto-release safety net
const seatLockTimers = new Map()
// Tracks seats being actively locked (between API call and push to selectedSeats)
// Prevents race condition where SignalR 'Selecting' arrives before selectedSeats is updated
const pendingLocks = new Set()
const tierRemainingQuotas = reactive({})

const saleStatus = computed(() => {
  if (!event.value) return 'unknown'
  const now = new Date()
  if (event.value.saleOpenAt && new Date(event.value.saleOpenAt) > now) {
    return 'upcoming'
  }
  if (event.value.saleCloseAt && new Date(event.value.saleCloseAt) < now) {
    return 'closed'
  }
  return 'open'
})

const loadTicketInventoryStates = async (showtimeId) => {
  if (!event.value || !event.value.showtimes) return;
  const showtime = event.value.showtimes.find(s => s.id === showtimeId);
  if (!showtime || !showtime.ticketTypes) return;
  
  for (const tier of showtime.ticketTypes) {
    if (!isReservedTier(tier)) {
      try {
        const res = await getTicketInventoryState(showtimeId, tier.id);
        // Note: API returns success=true/false structure
        if (res && res.success && res.data) {
          tierRemainingQuotas[tier.id] = res.data.availableQuantity;
        }
      } catch (err) {
        console.error('Lỗi khi tải số lượng vé còn lại:', err);
        store.toast = { message: getErrorMessage(err, 'Không thể tải số lượng vé còn lại.'), icon: '⚠️' }
      }
    }
  }
}
const selectedShowtimeId = computed(() => {
  return event.value?.showtimes?.[selectedShowTimeIndex.value]?.id
})

const getHubUrl = () => {
  return API_BASE_URL.replace('/api/v1', '/hubs/seatmap')
}

// ── Seat State Loading & Merging ──
const loadSeatStates = async () => {
  const showtimeId = selectedShowtimeId.value
  if (!showtimeId || !seatMapData.value) return

  try {
    const res = await getSeatStates(showtimeId)

    // Backend returns a plain array directly (no success wrapper)
    // Support both formats: direct array or { success, data } wrapper
    let seatStates = null
    if (Array.isArray(res)) {
      seatStates = res
    } else if (res && res.success && Array.isArray(res.data)) {
      seatStates = res.data
    } else if (res && Array.isArray(res.data)) {
      seatStates = res.data
    }

    if (!seatStates) {
      console.warn('[SeatStates] Không thể parse response:', res)
      return
    }

    console.log(`[SeatStates] Đã tải ${seatStates.length} trạng thái ghế.`)

    const stateMap = {}
    seatStates.forEach(s => {
      // Support both camelCase (seatId) and PascalCase (SeatId)
      const id = s.seatId ?? s.SeatId
      const status = s.status ?? s.Status
      const lockedByUserId = s.lockedByUserId ?? s.LockedByUserId
      if (id) stateMap[id] = { status, lockedByUserId }
    })

    // Reset all seats to 'Trống' first, then apply fetched states
    seatMapData.value.zones?.forEach(zone => {
      zone.rows?.forEach(row => {
        row.seats?.forEach(seat => {
          const state = stateMap[seat.id]
          if (state && state.status === 'Sold') {
            seat.layoutStatus = 'Đã bán'
          } else if (state && (state.status === 'Selecting' || state.status === 'Checkout')) {
            // Only mark as 'Đang giữ' if this seat is NOT selected by current user
            const isMineLocal = selectedSeats.value.some(s => s.id === seat.id) || pendingLocks.has(seat.id)
            const isMineRemote = store.user && store.user.id && state.lockedByUserId === store.user.id
            
            if (isMineRemote && !isMineLocal) {
               const price = getZonePrice(zone.id)
               const ticketTypeName = getZoneTicketTypeName(zone.id)
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

            if (!isMineLocal && !isMineRemote) {
              seat.layoutStatus = 'Đang giữ'
            } else {
              seat.layoutStatus = 'Trống' // Will be drawn as green because it's in selectedSeats
            }
          } else {
            seat.layoutStatus = 'Trống'
          }
        })
      })
    })
  } catch (err) {
    console.error('[SeatStates] Lỗi khi tải trạng thái ghế từ Inventory:', err)
    store.toast = { message: getErrorMessage(err, 'Không thể tải trạng thái ghế mới nhất.'), icon: '⚠️' }
  }
}

// Kiểm tra xem người dùng có đơn hàng đang chờ thanh toán cho suất chiếu đang chọn không
const checkPendingOrder = async () => {
  const showtimeId = selectedShowtimeId.value
  if (!showtimeId || !getToken()) {
    pendingOrder.value = null
    return
  }

  try {
    const res = await getMyPendingOrder(showtimeId)
    pendingOrder.value = (res && res.success && res.data) ? res.data : null
  } catch (err) {
    console.error('[PendingOrder] Lỗi khi kiểm tra đơn hàng đang chờ thanh toán:', err)
    pendingOrder.value = null
  }
}

const resumePendingOrder = () => {
  if (!pendingOrder.value) return
  router.push({ name: 'payment', query: { orderId: pendingOrder.value.orderId } })
}

const cancelPendingOrder = async () => {
  if (!pendingOrder.value) return

  isCancellingPendingOrder.value = true
  try {
    const res = await cancelOrder(pendingOrder.value.orderId)
    if (res && res.success) {
      pendingOrder.value = null
      selectedSeats.value = []
      clearAllSeatTimers()
      store.toast = { message: 'Đã hủy đơn hàng. Bạn có thể chọn vé và đặt lại.', icon: '✅' }
      if (saleStatus.value === 'open') {
        await loadSeatStates()
      }
    } else {
      store.toast = { message: res?.message || 'Không thể hủy đơn hàng.', icon: '⚠️' }
    }
  } catch (err) {
    console.error('[PendingOrder] Lỗi khi hủy đơn hàng:', err)
    store.toast = { message: getErrorMessage(err, 'Không thể hủy đơn hàng.'), icon: '❌' }
  } finally {
    isCancellingPendingOrder.value = false
  }
}

// ── SignalR Hub Real-time Setup ──
let hubConnection = null
const localUnlocks = new Set()

// Start a 58s client-side safety timer per seat.
// If SignalR doesn’t fire the Available event in time (e.g. keyspace events disabled),
// the client auto-removes the seat from its local selection.
const startSeatTimer = (seatId, showtimeId) => {
  clearSeatTimer(seatId) // clear existing timer if any
  const timerId = setTimeout(async () => {
    const idx = selectedSeats.value.findIndex(s => s.id === seatId)
    if (idx !== -1) {
      const seat = selectedSeats.value[idx]
      updateSeatStateLocally(seatId, 'Available')
      store.toast = { message: `Ghế ${seat.rowName}-${seat.seatName} đã hết thời gian giữ chỗ (60 giây).`, icon: '⏰' }
    }
    seatLockTimers.delete(seatId)
  }, 58000) // 58s — 2s before Redis 60s TTL expires
  seatLockTimers.set(seatId, timerId)
}

const clearSeatTimer = (seatId) => {
  const timerId = seatLockTimers.get(seatId)
  if (timerId !== undefined) {
    clearTimeout(timerId)
    seatLockTimers.delete(seatId)
  }
}

const clearAllSeatTimers = () => {
  seatLockTimers.forEach((timerId) => clearTimeout(timerId))
  seatLockTimers.clear()
}

const startHubConnection = async (showtimeId) => {
  if (hubConnection) {
    await stopHubConnection()
  }

  if (!getToken()) {
    console.warn('[SignalR] Không có token xác thực, bỏ qua kết nối Real-time.')
    return
  }

  const hubUrl = getHubUrl()
  console.log(`[SignalR] Kết nối tới: ${hubUrl} cho suất chiếu: ${showtimeId}`)

  hubConnection = new HubConnectionBuilder()
    .withUrl(hubUrl, {
      // Always call getToken() dynamically so reconnects get a fresh token
      accessTokenFactory: () => getToken()
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .build()

  hubConnection.on('SeatStateChanged', (data) => {
    console.log('[SignalR] SeatStateChanged received:', data)
    if (data && data.seatId) {
      updateSeatStateLocally(data.seatId, data.status)
    }
  })

  // Re-join the showtime group after automatic reconnect
  hubConnection.onreconnected(async () => {
    console.log('[SignalR] Reconnected. Re-joining showtime group:', showtimeId)
    try {
      await hubConnection.invoke('JoinShowtime', showtimeId)
    } catch (err) {
      console.error('[SignalR] Failed to re-join showtime after reconnect:', err)
    }
  })

  hubConnection.onclose((err) => {
    console.warn('[SignalR] Connection closed:', err)
  })

  try {
    await hubConnection.start()
    console.log(`[SignalR] ✓ Kết nối thành công, state: ${hubConnection.state}`)
    await hubConnection.invoke('JoinShowtime', showtimeId)
    console.log(`[SignalR] ✓ Đã join group showtime_${showtimeId}`)
  } catch (err) {
    console.error('[SignalR] ✗ Không thể kết nối hoặc join group:', err)
    hubConnection = null
  }
}

const stopHubConnection = async () => {
  if (hubConnection) {
    try {
      const showtimeId = selectedShowtimeId.value
      if (showtimeId && hubConnection.state === 'Connected') {
        await hubConnection.invoke('LeaveShowtime', showtimeId)
      }
      await hubConnection.stop()
      console.log('Đã đóng kết nối SignalR.')
    } catch (err) {
      console.error('Lỗi khi đóng kết nối SignalR:', err)
    } finally {
      hubConnection = null
    }
  }
}

const updateSeatStateLocally = (seatId, status) => {
  if (!seatMapData.value) return

  seatMapData.value.zones?.forEach(zone => {
    zone.rows?.forEach(row => {
      row.seats?.forEach(seat => {
        if (seat.id === seatId) {
          if (status === 'Available') {
            clearSeatTimer(seatId) // cancel safety timer
            // If we already handled unlock via HTTP response, skip — avoid double-removal
            if (!localUnlocks.has(seatId)) {
              const idx = selectedSeats.value.findIndex(s => s.id === seatId)
              if (idx !== -1) {
                selectedSeats.value.splice(idx, 1)
                store.toast = { message: `Ghế ${row.rowName}-${seat.seatName} đã hết hạn giữ chỗ và được giải phóng.`, icon: '⏰' }
              }
            }
            seat.layoutStatus = 'Trống'
          } else if (status === 'Selecting') {
            // Do NOT mark as 'Đang giữ' if:
            // - The seat is already in our selectedSeats (we locked it, SignalR echo)
            // - The seat is in pendingLocks (we just called lockSeat, SignalR arrived before push)
            const isMine = selectedSeats.value.some(s => s.id === seatId) || pendingLocks.has(seatId)
            if (!isMine) {
              seat.layoutStatus = 'Đang giữ'
            }
          } else if (status === 'Sold') {
            const idx = selectedSeats.value.findIndex(s => s.id === seatId)
            if (idx !== -1) {
              selectedSeats.value.splice(idx, 1)
              store.toast = { message: `Ghế ${row.rowName}-${seat.seatName} đã được mua bởi người khác.`, icon: '⚠️' }
            }
            seat.layoutStatus = 'Đã bán'
          }
        }
      })
    })
  })
}

const konvaContainer = ref(null)

// Function to unlock all selected seats proactively on refresh / close
const unlockAllSeatsProactively = () => {
  if (selectedSeats.value.length === 0 || !selectedShowtimeId.value) return
  
  const token = getToken()
  if (!token) return
  
  const url = `${API_BASE_URL}/inventory/Seats/seats/unlock`
  
  selectedSeats.value.forEach(seat => {
    fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        showtimeId: selectedShowtimeId.value,
        seatId: seat.id
      }),
      keepalive: true
    }).catch(err => console.error('Failed proactive unlock:', err))
  })
}

const handleUnloadEvents = () => {
  unlockAllSeatsProactively()
}

// Load Event details and conditionally fetch SeatMap layout
onMounted(async () => {
  window.addEventListener('beforeunload', handleUnloadEvents)
  window.addEventListener('pagehide', handleUnloadEvents)
  trackEventClick(route.params.id, 'ViewDetail').catch(() => {})
  try {
    const res = await getEventDetail(route.params.id)
    if (res && res.success && res.data) {
      event.value = res.data
      
      // If the event has a seatMapId, load the seatmap layout
      if (event.value.seatMapId) {
        await loadSeatMapLayout()
      }
      
      if (selectedShowtimeId.value && saleStatus.value === 'open') {
        await loadTicketInventoryStates(selectedShowtimeId.value)
      }

      await checkPendingOrder()
    } else {
      error.value = res?.message || 'Không thể lấy thông tin sự kiện.'
    }
  } catch (err) {
    error.value = getErrorMessage(err, 'Lỗi kết nối khi tải chi tiết sự kiện.')
    console.error(err)
  } finally {
    isLoading.value = false
  }
})

// Fetch seatmap details using the venueId and seatMapId from the event
const loadSeatMapLayout = async () => {
  isLoadingSeatMap.value = true
  seatMapError.value = ''
  try {
    const venueId = event.value?.venueId
    const seatMapId = event.value?.seatMapId
    if (!venueId || !seatMapId) {
      throw new Error('Sự kiện không có cấu hình thông tin địa điểm hoặc sơ đồ ghế.')
    }
    
    // Fetch SeatMap details directly using venueId and seatMapId
    const seatMapRes = await getSeatMapDetail(venueId, seatMapId)
    if (seatMapRes && seatMapRes.success && seatMapRes.data) {
      seatMapData.value = seatMapRes.data
      initKonvaResize()
      if (saleStatus.value === 'open') {
        await loadSeatStates()
        if (selectedShowtimeId.value) {
          await startHubConnection(selectedShowtimeId.value)
        }
      }
    } else {
      throw new Error(seatMapRes?.message || 'Tải dữ liệu sơ đồ ghế ngồi thất bại.')
    }
  } catch (err) {
    console.error('Error loading seatmap details:', err)
    seatMapError.value = getErrorMessage(err, 'Không thể tải sơ đồ ghế ngồi.')
  } finally {
    isLoadingSeatMap.value = false
  }
}

// Compute active tier details
const activeTier = computed(() => {
  if (event.value && event.value.showtimes?.[selectedShowTimeIndex.value]?.ticketTypes && event.value.showtimes[selectedShowTimeIndex.value].ticketTypes.length > 0) {
    return event.value.showtimes[selectedShowTimeIndex.value].ticketTypes[selectedTier.value]
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
  if (pendingOrder.value) return true
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
  if (!event.value || !event.value.showtimes?.[selectedShowTimeIndex.value]?.ticketTypes || event.value.showtimes[selectedShowTimeIndex.value].ticketTypes.length === 0) return 0
  return Math.min(...event.value.showtimes[selectedShowTimeIndex.value].ticketTypes.map(t => t.price))
}

const selectTierIndex = (idx) => {
  selectedTier.value = idx
  qty.value = 1
}

const removeSelectedSeat = async (id) => {
  const showtimeId = selectedShowtimeId.value
  if (!showtimeId) return

  localUnlocks.add(id)
  clearSeatTimer(id)
  try {
    const res = await unlockSeat(showtimeId, id)
    if (res && res.success) {
      const index = selectedSeats.value.findIndex(s => s.id === id)
      if (index !== -1) selectedSeats.value.splice(index, 1)
      // Also update seat's layoutStatus back to Trống
      seatMapData.value?.zones?.forEach(zone => {
        zone.rows?.forEach(row => {
          row.seats?.forEach(seat => {
            if (seat.id === id) seat.layoutStatus = 'Trống'
          })
        })
      })
    } else {
      store.toast = { message: res?.message || 'Không thể hủy khóa ghế.', icon: '⚠️' }
    }
  } catch (err) {
    console.error(err)
    const msg = getErrorMessage(err, 'Lỗi khi hủy khóa ghế.')
    store.toast = { message: msg, icon: '⚠️' }
  } finally {
    setTimeout(() => localUnlocks.delete(id), 2000)
  }
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

onUnmounted(async () => {
  window.removeEventListener('beforeunload', handleUnloadEvents)
  window.removeEventListener('pagehide', handleUnloadEvents)
  clearAllSeatTimers()
  if (resizeObserver) resizeObserver.disconnect()
  
  // Unlock any selected seats
  if (selectedSeats.value.length > 0 && selectedShowtimeId.value) {
    for (const seat of selectedSeats.value) {
      localUnlocks.add(seat.id)
      try {
        await unlockSeat(selectedShowtimeId.value, seat.id)
      } catch (e) {
        console.error(e)
      } finally {
        setTimeout(() => localUnlocks.delete(seat.id), 2000)
      }
    }
  }
  
  await stopHubConnection()
})

watch(selectedShowtimeId, async (newShowtimeId, oldShowtimeId) => {
  if (newShowtimeId) {
    // Clear selected seats first and unlock them
    if (selectedSeats.value.length > 0 && oldShowtimeId) {
      for (const seat of selectedSeats.value) {
        localUnlocks.add(seat.id)
        try {
          await unlockSeat(oldShowtimeId, seat.id)
        } catch (e) {
          console.error(e)
        } finally {
          setTimeout(() => localUnlocks.delete(seat.id), 2000)
        }
      }
    }
    selectedSeats.value = []
    clearAllSeatTimers()
    
    await stopHubConnection()
    if (seatMapData.value) {
      if (saleStatus.value === 'open') {
        await loadSeatStates()
        await startHubConnection(newShowtimeId)
      }
    }
    if (saleStatus.value === 'open') {
      await loadTicketInventoryStates(newShowtimeId)
    }
    await checkPendingOrder()
  } else {
    selectedSeats.value = []
    clearAllSeatTimers()
    pendingOrder.value = null
    await stopHubConnection()
  }
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
  if (!event.value || !event.value.showtimes?.[selectedShowTimeIndex.value]?.ticketTypes) return 0
  const tt = event.value.showtimes[selectedShowTimeIndex.value].ticketTypes.find(t => t.zoneId === zoneId)
  return tt ? tt.price : 0
}

function getZoneTicketTypeName(zoneId) {
  if (!event.value || !event.value.showtimes?.[selectedShowTimeIndex.value]?.ticketTypes) return 'Vé'
  const tt = event.value.showtimes[selectedShowTimeIndex.value].ticketTypes.find(t => t.zoneId === zoneId)
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

async function onSeatClick(seat, zone, row) {
  if (saleStatus.value !== 'open') {
    store.toast = { message: 'Hiện tại không trong thời gian bán vé.', icon: '⚠️' }
    return
  }

  const isMySelection = selectedSeats.value.some(s => s.id === seat.id)

  // Block only if seat is locked/sold by someone else (not by current user)
  if (!isMySelection) {
    if (seat.layoutStatus === 'Đã bán' || seat.layoutStatus === 'Đã đặt' || seat.layoutStatus === 'Đang giữ') {
      return
    }
  }
  
  const token = getToken()
  if (!token) {
    store.toast = { message: 'Vui lòng đăng nhập để thực hiện chọn ghế.', icon: '🔑' }
    store.showAuth = true
    return
  }

  const showtimeId = selectedShowtimeId.value
  if (!showtimeId) {
    store.toast = { message: 'Không tìm thấy thông tin suất chiếu.', icon: '⚠️' }
    return
  }
  
  const idx = selectedSeats.value.findIndex(s => s.id === seat.id)
  if (idx !== -1) {
    localUnlocks.add(seat.id)
    clearSeatTimer(seat.id)
    try {
      const res = await unlockSeat(showtimeId, seat.id)
      if (res && res.success) {
        selectedSeats.value.splice(idx, 1)
        // Update seat's layoutStatus back to Trống
        seat.layoutStatus = 'Trống'
      } else {
        store.toast = { message: res?.message || 'Không thể hủy khóa ghế.', icon: '⚠️' }
      }
    } catch (err) {
      console.error(err)
      const msg = getErrorMessage(err, 'Lỗi khi hủy khóa ghế.')
      store.toast = { message: msg, icon: '⚠️' }
    } finally {
      setTimeout(() => localUnlocks.delete(seat.id), 2000)
    }
  } else {
    if (selectedSeats.value.length >= 5) {
      store.toast = { message: 'Bạn chỉ được phép chọn tối đa 5 vé ghế ngồi.', icon: '⚠️' }
      return
    }
    
    pendingLocks.add(seat.id)
    try {
      const res = await lockSeat(showtimeId, seat.id)
      if (res && res.success) {
        const price = getZonePrice(zone.id)
        const ticketTypeName = getZoneTicketTypeName(zone.id)

        // Select seat and automatically focus selected tier index
        const zoneTicketTypes = event.value.showtimes?.[selectedShowTimeIndex.value]?.ticketTypes
        const tierIndex = zoneTicketTypes?.findIndex(t => t.zoneId === zone.id) ?? -1
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
          ticketTypeName: ticketTypeName,
          ticketTypeId: tierIndex !== -1 ? zoneTicketTypes[tierIndex].id : null
        })
        // Start client-side 58s safety auto-release timer
        startSeatTimer(seat.id, showtimeId)
      } else {
        store.toast = { message: res?.message || 'Ghế đã bị người khác chọn hoặc khóa.', icon: '⚠️' }
        await loadSeatStates()
      }
    } catch (err) {
      console.error(err)
      store.toast = { message: getErrorMessage(err, 'Lỗi khi chọn khóa ghế.'), icon: '⚠️' }
    } finally {
      pendingLocks.delete(seat.id)
    }
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

const formatTimeOnly = (dateStr) => {
  if (!dateStr) return '--:--'
  try {
    const date = new Date(dateStr)
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false })
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

// Summary text for the modal
const checkoutSummaryText = computed(() => {
  if (event.value?.seatMapId && activeTier.value && isReservedTier(activeTier.value)) {
    return `${selectedSeats.value.length} ghế ngồi`
  }
  return `${qty.value} vé ${activeTier.value?.ticketTypeName || ''}`
})

// Opens the checkout modal, pre-fills name from store.user
const handleBuyTicket = () => {
  if (!event.value || !activeTier.value) return

  // Auth check
  const token = getToken()
  if (!token) {
    store.toast = { message: 'Vui lòng đăng nhập để mua vé.', icon: '🔑' }
    store.showAuth = true
    return
  }

  if (!selectedShowtimeId.value) {
    store.toast = { message: 'Vui lòng chọn suất chiếu.', icon: '⚠️' }
    return
  }

  if (event.value.seatMapId && isReservedTier(activeTier.value) && selectedSeats.value.length === 0) {
    store.toast = { message: 'Vui lòng chọn ít nhất 1 ghế trên sơ đồ.', icon: '⚠️' }
    return
  }

  trackEventClick(event.value.id, 'PurchaseIntent').catch(() => {})

  // Pre-fill from JWT user info
  checkoutForm.customerName = store.user?.name || store.user?.fullName || ''
  checkoutForm.customerPhone = store.user?.phone || store.user?.phoneNumber || ''
  formErrors.customerName = ''
  formErrors.customerPhone = ''

  showCheckoutModal.value = true
}

// Called when user confirms in modal
const submitCheckout = async () => {
  // Validate form
  formErrors.customerName = ''
  formErrors.customerPhone = ''
  let hasError = false

  if (!checkoutForm.customerName.trim()) {
    formErrors.customerName = 'Vui lòng nhập tên người nhận vé.'
    hasError = true
  }
  if (!checkoutForm.customerPhone.trim()) {
    formErrors.customerPhone = 'Vui lòng nhập số điện thoại.'
    hasError = true
  } else if (!/^(0|\+84)[0-9]{8,10}$/.test(checkoutForm.customerPhone.trim())) {
    formErrors.customerPhone = 'Số điện thoại không đúng định dạng (VD: 0901234567).'
    hasError = true
  }

  if (hasError) return

  const showtimeId = selectedShowtimeId.value
  const selectedShowtime = event.value?.showtimes?.[selectedShowTimeIndex.value]

  if (!showtimeId || !selectedShowtime) {
    store.toast = { message: 'Không tìm thấy thông tin suất chiếu.', icon: '⚠️' }
    return
  }

  isCheckingOut.value = true
  try {
    let items = []

    if (event.value.seatMapId && isReservedTier(activeTier.value)) {
      const seatWithoutTicketType = selectedSeats.value.find(seat => !seat.ticketTypeId)
      if (seatWithoutTicketType) {
        store.toast = { message: `Không xác định được loại vé cho ghế '${seatWithoutTicketType.seatName}'. Vui lòng bỏ chọn và chọn lại ghế.`, icon: '⚠️' }
        isCheckingOut.value = false
        return
      }

      items = selectedSeats.value.map(seat => ({
        seatId: seat.id,
        ticketTypeId: seat.ticketTypeId,
        quantity: 1
      }))
    } else {
      items = [{
        seatId: null,
        ticketTypeId: activeTier.value.id,
        quantity: qty.value
      }]
    }

    // customerEmail is NOT sent — backend extracts from JWT
    const payload = {
      showtimeId: showtimeId,
      eventId: event.value.id,
      customerName: checkoutForm.customerName.trim(),
      customerPhone: checkoutForm.customerPhone.trim(),
      paymentMethod: 'VNPay',
      items: items
    }

    const res = await checkout(payload)
    if (res && res.success && res.data) {
      const orderId = res.data
      showCheckoutModal.value = false
      selectedSeats.value = []
      clearAllSeatTimers()
      qty.value = 1
      router.push({ name: 'payment', query: { orderId } })
    } else {
      store.toast = { message: res?.message || 'Không thể tạo đơn hàng. Vui lòng thử lại.', icon: '❌' }
    }
  } catch (err) {
    console.error('[Checkout] Lỗi:', err)
    store.toast = { message: getErrorMessage(err, 'Lỗi khi kết nối đến hệ thống thanh toán. Vui lòng thử lại.'), icon: '❌' }
  } finally {
    isCheckingOut.value = false
  }
}

const scrollToSelect = () => {
  const el = document.getElementById('showtimes') || document.getElementById('venue')
  if (el) {
    el.scrollIntoView({ behavior: 'smooth' })
  }
}
</script>

<style scoped>
.pb-safe {
  padding-bottom: env(safe-area-inset-bottom, 1rem);
}

.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.25s ease;
}
.modal-fade-enter-active .relative,
.modal-fade-leave-active .relative {
  transition: transform 0.25s ease, opacity 0.25s ease;
}
.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}
.modal-fade-enter-from .relative {
  transform: scale(0.95) translateY(12px);
}
</style>
