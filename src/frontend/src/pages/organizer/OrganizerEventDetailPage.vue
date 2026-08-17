<template>
  <div v-if="isLoading" class="flex flex-col pb-20 bg-[#0A0F0D] min-h-[80vh] items-center justify-center">
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
            <!-- Badges -->
            <div class="flex flex-wrap items-center gap-3">
              <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/20 border border-primary/30 backdrop-blur-md shadow-[0_0_20px_rgba(0,200,83,0.15)]">
                <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">{{ event.categoryName || 'Sự kiện' }}</span>
              </div>
              <div 
                class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full backdrop-blur-md border transition-colors"
                :class="event.status === 'Bị từ chối' ? 'bg-[#ef4444]/20 border-[#ef4444]/30' : 'bg-white/5 border-white/10'"
              >
                <span 
                  class="text-[11px] font-black uppercase tracking-[0.2em]"
                  :class="event.status === 'Bị từ chối' ? 'text-[#ef4444]' : 'text-white/60'"
                >
                  {{ event.status || 'Chờ duyệt' }}
                </span>
              </div>
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
                  Mở bán vé: <span class="text-primary">{{ formatEventDate(event.saleOpenAt) }}</span>
                  <span v-if="event.saleCloseAt" class="text-white/40 font-medium"> — Hạn bán: <span class="text-warning font-bold">{{ formatEventDate(event.saleCloseAt) }}</span></span>
                </span>
              </div>
              <div class="flex items-start gap-3 text-white/70 text-[14px]">
                <PhMapPin weight="bold" class="text-primary text-xl flex-shrink-0 mt-0.5" />
                <span class="font-bold line-clamp-2">{{ event.location?.venueName || 'Địa điểm chưa cập nhật' }}</span>
              </div>
            </div>
          </div>

          <!-- Actions -->
          <div class="pt-6 border-t border-white/5 flex flex-col sm:flex-row items-center gap-4 mt-6">
            <BaseButton variant="outline" class="w-full sm:w-auto !rounded-xl !py-3.5 !px-8 flex items-center justify-center gap-2" @click="handleEdit">
              <PhPencilSimple weight="bold" />
              Chỉnh sửa sự kiện
            </BaseButton>
            <BaseButton variant="outline" class="w-full sm:w-auto !rounded-xl !py-3.5 !px-8 flex items-center justify-center gap-2" @click="handleReport">
              <PhChartPie weight="bold" />
              Xem báo cáo
            </BaseButton>
            <BaseButton variant="outline" class="w-full sm:w-auto !rounded-xl !py-3.5 !px-8 flex items-center justify-center gap-2" @click="handleRatings">
              <PhStar weight="bold" />
              Xem đánh giá
            </BaseButton>
            <BaseButton variant="primary" class="w-full sm:w-auto !rounded-xl !py-3.5 !px-8 flex items-center justify-center gap-2" @click="handleOrders">
              <PhReceipt weight="bold" />
              Xem đơn hàng
            </BaseButton>
            <BaseButton
              v-if="canRequestCancellation"
              variant="outline"
              class="w-full sm:w-auto !rounded-xl !py-3.5 !px-8 !border-danger/30 !text-danger hover:!bg-danger/10 flex items-center justify-center gap-2"
              @click="openCancelModal"
            >
              <PhProhibit weight="bold" />
              Hủy sự kiện
            </BaseButton>
            <span v-else-if="hasPendingCancelRequest" class="w-full sm:w-auto text-center px-8 py-3.5 rounded-xl bg-warning/10 border border-warning/20 text-warning text-sm font-bold">
              Đang chờ Moderator duyệt yêu cầu hủy
            </span>
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

    <!-- Main Content -->
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-20 mt-12 w-full">
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

          <!-- Showtimes Section -->
          <section v-if="event.showtimes && event.showtimes.length > 0" class="animate-fade-up [animation-delay:250ms]">
            <div class="flex items-center gap-6 mb-8">
              <h2 class="font-heading text-3xl font-black text-white uppercase tracking-widest whitespace-nowrap">Các suất chiếu</h2>
              <div class="h-px flex-1 bg-gradient-to-r from-white/20 to-transparent"></div>
            </div>
            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              <div v-for="(st, idx) in event.showtimes" :key="st.id" 
                   @click="selectedShowTimeIndex = idx"
                   class="p-5 rounded-2xl bg-[#111916] border shadow-lg relative group overflow-hidden transition-all duration-300 cursor-pointer"
                   :class="selectedShowTimeIndex === idx ? 'border-primary ring-1 ring-primary/50' : 'border-white/10 hover:border-primary/30'">
                <div class="absolute inset-0 bg-gradient-to-br from-primary/10 to-transparent transition-opacity" :class="selectedShowTimeIndex === idx ? 'opacity-100' : 'opacity-0 group-hover:opacity-100'"></div>
                <div class="relative z-10 flex flex-col gap-2">
                  <span class="text-primary font-bold text-[12px] uppercase tracking-widest mb-1.5">Suất {{ idx + 1 }}</span>
                  <div class="flex items-start gap-2.5 text-white/80">
                    <PhCalendarBlank weight="bold" class="text-white/40 mt-0.5 flex-shrink-0" />
                    <div class="flex flex-col gap-1 text-[13px] font-medium">
                      <span>Bắt đầu: <span class="font-bold text-white">{{ formatEventDate(st.startAt) }}</span></span>
                      <span v-if="st.endAt">Kết thúc: <span class="font-bold text-white/60">{{ formatEventDate(st.endAt) }}</span></span>
                    </div>
                  </div>
                </div>
              </div>
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
                  <PhTicket weight="fill" /> {{ event.showtimes?.[selectedShowTimeIndex]?.ticketTypes?.length || 0 }} loại vé
                </div>
              </div>

              <!-- Ticket Types List -->
              <div class="space-y-4 relative z-10 max-h-[50vh] overflow-y-auto pr-2 custom-scrollbar">
                <div v-if="!event.showtimes?.[selectedShowTimeIndex]?.ticketTypes || event.showtimes[selectedShowTimeIndex].ticketTypes.length === 0" class="py-8 text-center border border-dashed border-white/10 rounded-2xl text-white/30 text-[13px] font-medium">
                  Chưa có thông tin hạng vé.
                </div>
                
                <div v-for="tier in event.showtimes?.[selectedShowTimeIndex]?.ticketTypes" :key="tier.id" class="p-5 rounded-2xl bg-white/5 border border-white/5 flex flex-col gap-3 hover:border-primary/30 transition-colors">
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
                <BaseButton variant="outline" class="w-full !rounded-xl !py-3 flex items-center justify-center gap-2" @click="handleReport">
                  <PhChartPie weight="bold" />
                  Xem báo cáo
                </BaseButton>
                <BaseButton variant="outline" class="w-full !rounded-xl !py-3 flex items-center justify-center gap-2" @click="handleRatings">
                  <PhStar weight="bold" />
                  Xem đánh giá
                </BaseButton>
                <BaseButton variant="primary" class="w-full !rounded-xl !py-3 flex items-center justify-center gap-2" @click="handleOrders">
                  <PhReceipt weight="bold" />
                  Xem đơn hàng
                </BaseButton>
                <BaseButton
                  v-if="canRequestCancellation"
                  variant="outline"
                  class="w-full !rounded-xl !py-3 !border-danger/30 !text-danger hover:!bg-danger/10 flex items-center justify-center gap-2"
                  @click="openCancelModal"
                >
                  <PhProhibit weight="bold" />
                  Hủy sự kiện
                </BaseButton>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </div>

    <!-- Cancel Event Request Modal -->
    <div v-if="isCancelModalOpen" class="fixed inset-0 z-[10000] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="closeCancelModal"></div>

      <div class="relative bg-card/90 backdrop-blur-2xl border border-border-main rounded-[32px] w-full max-w-lg overflow-hidden shadow-2xl shadow-black/60 p-8 animate-in zoom-in-95 fade-in duration-300">
        <h3 class="text-2xl font-bold text-main mb-1 font-heading">Yêu cầu hủy sự kiện</h3>
        <p class="text-white/50 text-sm mb-6 line-clamp-1">{{ event?.title }}</p>

        <form @submit.prevent="submitCancelRequest" class="space-y-6">
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Lý do hủy <span class="text-danger">*</span></label>
            <textarea
              v-model="cancelReason"
              placeholder="VD: Sự kiện không thể tổ chức do lý do bất khả kháng..."
              rows="4"
              class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-danger/50 transition-all placeholder:text-white/20 resize-none"
            ></textarea>
            <span v-if="cancelError" class="text-danger text-xs font-bold">{{ cancelError }}</span>
            <p class="text-white/40 text-xs">Yêu cầu sẽ được gửi tới Moderator xem xét. Toàn bộ đơn hàng chưa check-in sẽ được hoàn tiền tự động nếu được duyệt.</p>
          </div>

          <div class="flex gap-3 pt-2">
            <BaseButton type="button" variant="outline" class="flex-1 !rounded-2xl" @click="closeCancelModal">
              Hủy bỏ
            </BaseButton>
            <BaseButton type="submit" variant="primary" class="flex-1 !rounded-2xl !bg-danger hover:!bg-danger/80" :disabled="isSubmittingCancel">
              <PhSpinner v-if="isSubmittingCancel" class="animate-spin text-lg" />
              <span v-else>Gửi yêu cầu hủy</span>
            </BaseButton>
          </div>
        </form>
      </div>
    </div>
  </div>

  <!-- Error State -->
  <div v-else class="flex flex-col items-center justify-center py-32 px-6 text-center min-h-[80vh] bg-[#0A0F0D]">
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
import { getOrganizerEventDetail, requestEventCancellation } from '../../services/eventService'
import { getOrganizerSeatMapDetail } from '../../services/venue.service'
import { store } from '../../stores/eventStore'
import { addToast } from '../../stores/adminStore'
import { getErrorMessage } from '../../utils/apiError'
import BaseButton from '../../components/ui/BaseButton.vue'
import {
  PhCalendarBlank, PhMapPin, PhTicket, PhMapPinLine, PhSpinner,
  PhWarningCircle, PhPencilSimple, PhReceipt, PhClock, PhChartPie, PhStar, PhProhibit
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const event = ref(null)
const isLoading = ref(true)
const error = ref('')
const selectedShowTimeIndex = ref(0)

const seatMapData = ref(null)
const isLoadingSeatMap = ref(false)
const seatMapError = ref('')
const hoveredSeat = ref(null)
const konvaContainer = ref(null)

const isCancelModalOpen = ref(false)
const cancelReason = ref('')
const cancelError = ref('')
const isSubmittingCancel = ref(false)
const hasPendingCancelRequest = ref(false)

const canRequestCancellation = computed(() => {
  if (!event.value) return false
  if (hasPendingCancelRequest.value) return false
  return event.value.status === 'Published' || event.value.status === 'Đã xuất bản'
})

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
    error.value = getErrorMessage(err, 'Lỗi kết nối khi tải chi tiết sự kiện.')
    console.error(err)
  } finally {
    isLoading.value = false
  }
})

const loadSeatMapLayout = async () => {
  isLoadingSeatMap.value = true
  seatMapError.value = ''
  try {
    const venueId = event.value.venueId
    if (!venueId) throw new Error('Sự kiện không có thông tin venueId hợp lệ.')
    
    const seatMapRes = await getOrganizerSeatMapDetail(venueId, event.value.seatMapId)
    if (seatMapRes && seatMapRes.success && seatMapRes.data) {
      seatMapData.value = seatMapRes.data
      initKonvaResize()
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

const formatTimeOnly = (dateStr) => {
  if (!dateStr) return '--:--'
  try {
    const date = new Date(dateStr)
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false })
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

const handleReport = () => {
  router.push(`/organizer/events/${event.value.id}/report`)
}

const handleRatings = () => {
  router.push(`/organizer/events/${event.value.id}/ratings`)
}

const openCancelModal = () => {
  cancelReason.value = ''
  cancelError.value = ''
  isCancelModalOpen.value = true
}

const closeCancelModal = () => {
  isCancelModalOpen.value = false
}

const submitCancelRequest = async () => {
  if (!cancelReason.value.trim()) {
    cancelError.value = 'Vui lòng nhập lý do hủy sự kiện.'
    return
  }

  isSubmittingCancel.value = true
  try {
    const res = await requestEventCancellation(event.value.id, cancelReason.value.trim())
    if (res && res.success) {
      addToast(res.message || 'Đã gửi yêu cầu hủy sự kiện thành công.', 'success')
      hasPendingCancelRequest.value = true
      closeCancelModal()
    } else {
      addToast(res?.message || 'Không thể gửi yêu cầu hủy sự kiện.', 'error')
    }
  } catch (err) {
    console.error('Error requesting event cancellation:', err)
    addToast(err.response?.data?.message || 'Có lỗi xảy ra khi gửi yêu cầu hủy sự kiện.', 'error')
  } finally {
    isSubmittingCancel.value = false
  }
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
  if (!event.value || !event.value.showtimes?.[selectedShowTimeIndex.value]?.ticketTypes) return 0
  const tt = event.value.showtimes[selectedShowTimeIndex.value].ticketTypes.find(t => t.zoneId === zoneId)
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
