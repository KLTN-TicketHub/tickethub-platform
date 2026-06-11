<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12 px-2 md:px-0">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-4">
      <div class="flex flex-col gap-2">
        <div class="flex items-center gap-2 text-white/50 text-sm font-medium mb-1">
          <router-link :to="`/moderator/venues/${venueId}/seat-maps`" class="hover:text-primary transition-colors flex items-center gap-1">
            <PhArrowLeft weight="bold" class="text-xs" />
            Sơ đồ ghế
          </router-link>
          <span>/</span>
          <span class="text-white/30">Tạo mới</span>
        </div>
        <h1 class="font-heading text-3xl md:text-4xl font-black text-white tracking-tight">Tạo Sơ đồ Ghế</h1>
        <p class="text-white/50 font-medium text-lg">Upload file SVG từ Figma và cấu hình sơ đồ chỗ ngồi.</p>
      </div>
    </div>

    <!-- Step Indicator -->
    <div class="flex items-center gap-0">
      <div v-for="(step, i) in steps" :key="i" class="flex items-center">
        <div class="flex items-center gap-2 px-4 py-2 rounded-xl transition-all"
          :class="currentStep === i ? 'bg-primary/15 text-primary' : currentStep > i ? 'text-primary/60' : 'text-white/30'">
          <div class="w-6 h-6 rounded-full flex items-center justify-center text-[11px] font-black border-2 transition-all"
            :class="currentStep === i ? 'bg-primary text-black border-primary' : currentStep > i ? 'bg-primary/20 text-primary border-primary/40' : 'bg-white/5 text-white/40 border-white/10'">
            <PhCheck v-if="currentStep > i" weight="bold" />
            <span v-else>{{ i + 1 }}</span>
          </div>
          <span class="text-[13px] font-bold hidden sm:block">{{ step }}</span>
        </div>
        <div v-if="i < steps.length - 1" class="w-8 h-px bg-white/10 mx-1"></div>
      </div>
    </div>

    <!-- ── STEP 0: Upload SVG ── -->
    <div v-if="currentStep === 0" class="flex flex-col gap-6">
      <!-- Upload Drop Zone -->
      <div
        class="rounded-[2rem] border-2 border-dashed transition-all duration-300 flex flex-col items-center justify-center py-16 px-6 cursor-pointer select-none"
        :class="isDragging ? 'border-primary bg-primary/5' : 'border-white/10 bg-[#111916]/30 hover:border-white/20 hover:bg-white/[0.02]'"
        @dragover.prevent="isDragging = true"
        @dragleave="isDragging = false"
        @drop.prevent="onDrop"
        @click="triggerFileInput"
      >
        <input ref="fileInputRef" type="file" accept=".svg" class="hidden" @change="onFileChange" />
        <div class="w-16 h-16 rounded-2xl flex items-center justify-center mb-6 transition-all"
          :class="isDragging ? 'bg-primary/20 text-primary' : 'bg-white/5 text-white/40'">
          <PhUploadSimple class="text-4xl" weight="duotone" />
        </div>
        <p class="text-white font-bold text-lg mb-2">Kéo thả file SVG vào đây</p>
        <p class="text-white/40 text-sm">hoặc click để chọn file từ máy tính</p>
        <p class="text-white/20 text-xs mt-3">Chỉ chấp nhận file .svg theo chuẩn TicketHub Seatmap</p>
      </div>

      <!-- Parse Errors -->
      <div v-if="parseErrors.length > 0" class="bg-red-500/10 border border-red-500/20 rounded-2xl p-5 flex flex-col gap-3">
        <div class="flex items-center gap-2 text-red-400 font-bold text-[14px]">
          <PhWarningCircle weight="fill" />
          Lỗi phân tích SVG ({{ parseErrors.length }})
        </div>
        <ul class="list-disc list-inside text-red-400/80 text-[13px] space-y-1">
          <li v-for="(err, i) in parseErrors" :key="i">{{ err }}</li>
        </ul>
      </div>

      <!-- Parse Warnings -->
      <div v-if="parseWarnings.length > 0" class="bg-yellow-500/10 border border-yellow-500/20 rounded-2xl p-5 flex flex-col gap-3">
        <div class="flex items-center gap-2 text-yellow-400 font-bold text-[14px]">
          <PhWarning weight="fill" />
          Cảnh báo ({{ parseWarnings.length }})
        </div>
        <ul class="list-disc list-inside text-yellow-400/80 text-[13px] space-y-1">
          <li v-for="(w, i) in parseWarnings" :key="i">{{ w }}</li>
        </ul>
      </div>

      <!-- Parsed Zone Summary -->
      <div v-if="parsedData && parsedData.zones.length > 0" class="flex flex-col gap-4">
        <h2 class="text-white font-bold text-lg flex items-center gap-2">
          <PhCheckCircle weight="fill" class="text-primary" />
          Phát hiện {{ parsedData.zones.length }} phân khu từ "{{ svgFileName }}"
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div v-for="zone in parsedData.zones" :key="zone.zoneName"
            class="bg-[#111916]/50 border border-white/5 rounded-2xl p-5 flex flex-col gap-3">
            <div class="flex items-center justify-between">
              <span class="font-bold text-white text-[14px] truncate">{{ zone.zoneName }}</span>
              <span class="text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full"
                :class="zone.isStage ? 'bg-sky-500/10 text-sky-400' : zone.isSalable ? 'bg-primary/10 text-primary' : 'bg-white/5 text-white/40'">
                {{ zone.isStage ? 'Sân khấu' : zone.isSalable ? 'Có bán vé' : 'Không bán' }}
              </span>
            </div>
            <div class="flex flex-col gap-1.5 text-[12px] text-white/60">
              <div class="flex items-center justify-between">
                <span>Loại:</span>
                <span class="font-semibold text-white/80">{{ zone.isReservingSeat ? 'Ghế cố định' : (zone.isSalable ? 'Đứng tự do (GA)' : 'Trang trí') }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span>Số hàng:</span>
                <span class="font-semibold text-white/80">{{ zone._parsed.rowCount }}</span>
              </div>
              <div v-if="zone._parsed.rowCount > 0" class="flex items-center justify-between">
                <span>Tổng ghế:</span>
                <span class="font-bold text-primary">{{ zone._parsed.totalSeats }} ghế</span>
              </div>
              <div v-if="zone._parsed.rowCount > 0" class="pt-1 border-t border-white/5">
                <div v-for="row in zone._parsed.rowDetails" :key="row.rowLabel"
                  class="flex items-center justify-between text-[11px]">
                  <span class="text-white/40">Hàng {{ row.rowLabel }}:</span>
                  <span class="text-white/60">{{ row.count }} ghế</span>
                </div>
              </div>
              <div v-if="zone.capacity > 0 && !zone.isReservingSeat" class="flex items-center justify-between">
                <span>Sức chứa:</span>
                <span class="font-bold text-primary">{{ zone.capacity }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Total Stats -->
        <div class="bg-primary/5 border border-primary/20 rounded-2xl p-5 flex flex-wrap gap-6 justify-center">
          <div class="flex flex-col items-center gap-1">
            <span class="text-2xl font-black text-primary">{{ totalStats.zones }}</span>
            <span class="text-[12px] text-white/50 font-medium">Phân khu</span>
          </div>
          <div class="w-px bg-white/10 self-stretch hidden sm:block"></div>
          <div class="flex flex-col items-center gap-1">
            <span class="text-2xl font-black text-white">{{ totalStats.salableZones }}</span>
            <span class="text-[12px] text-white/50 font-medium">Zone bán vé</span>
          </div>
          <div class="w-px bg-white/10 self-stretch hidden sm:block"></div>
          <div class="flex flex-col items-center gap-1">
            <span class="text-2xl font-black text-[#818cf8]">{{ totalStats.totalRows }}</span>
            <span class="text-[12px] text-white/50 font-medium">Tổng số hàng</span>
          </div>
          <div class="w-px bg-white/10 self-stretch hidden sm:block"></div>
          <div class="flex flex-col items-center gap-1">
            <span class="text-2xl font-black text-emerald-400">{{ totalStats.totalSeats }}</span>
            <span class="text-[12px] text-white/50 font-medium">Tổng ghế ngồi</span>
          </div>
        </div>

        <div class="flex justify-end">
          <button @click="goToStep(1)"
            class="px-8 py-3 text-[14px] font-bold text-black bg-primary rounded-xl hover:scale-[1.02] active:scale-95 transition-transform flex items-center gap-2">
            Tiếp theo: Xem Preview
            <PhArrowRight weight="bold" />
          </button>
        </div>
      </div>
    </div>

    <!-- ── STEP 1: Konva Preview ── -->
    <div v-if="currentStep === 1" class="flex flex-col gap-6">
      <div class="flex items-center gap-3">
        <h2 class="text-white font-bold text-xl">Preview sơ đồ ghế</h2>
        <span class="text-[12px] text-white/40 font-medium">{{ parsedData?.svgWidth }}×{{ parsedData?.svgHeight }}px</span>
      </div>

      <!-- Konva Canvas Container -->
      <div class="bg-[#0A0F0D] border border-white/10 rounded-[2rem] overflow-hidden">
        <div ref="konvaContainer" class="w-full" style="min-height: 400px; max-height: 600px; overflow: auto;">
          <v-stage ref="stageRef" :config="stageConfig">
            <v-layer ref="layerRef">
              <!-- Background -->
              <v-rect :config="{ x: 0, y: 0, width: stageConfig.width, height: stageConfig.height, fill: '#0b0f19' }" />

              <!-- Zones -->
              <template v-for="zone in parsedData?.zones" :key="zone.zoneName">
                <!-- SVG Elements (paths, texts) -->
                <template v-for="(el, eli) in zone.svgElements" :key="'el-' + eli">
                  <v-path v-if="el.type === 'path' && el.data"
                    :config="buildPathConfig(el, zone)" />
                  <v-text v-if="el.type === 'text' && el.text"
                    :config="buildTextConfig(el)" />
                </template>
                <!-- Seats (circles) -->
                <template v-for="row in zone.rows" :key="'row-' + row.rowLabel">
                  <v-circle v-for="seat in row.seatRequests" :key="seat.svgElementId"
                    :config="buildSeatConfig(seat, zone)"
                    @mouseenter="onSeatHover(seat, zone)"
                    @mouseleave="hoveredSeat = null" />
                </template>
              </template>
            </v-layer>

            <!-- Tooltip Layer -->
            <v-layer v-if="hoveredSeat">
              <v-rect :config="tooltipBgConfig" />
              <v-text :config="tooltipTextConfig" />
            </v-layer>
          </v-stage>
        </div>
      </div>

      <!-- Zone Legend -->
      <div class="flex flex-wrap gap-3">
        <div v-for="zone in parsedData?.zones" :key="zone.zoneName"
          class="flex items-center gap-2 px-3 py-1.5 bg-white/5 rounded-full text-[12px] font-bold">
          <div class="w-3 h-3 rounded-full" :style="{ background: zone.color }"></div>
          <span class="text-white/70">{{ zone.zoneName }}</span>
          <span v-if="zone._parsed.totalSeats > 0" class="text-white/40">· {{ zone._parsed.totalSeats }} ghế</span>
        </div>
      </div>

      <div class="flex justify-between">
        <button @click="goToStep(0)" class="px-6 py-3 text-[14px] font-bold text-white/70 bg-white/5 border border-white/10 rounded-xl hover:bg-white/10 transition-colors flex items-center gap-2">
          <PhArrowLeft weight="bold" /> Quay lại
        </button>
        <button @click="goToStep(2)"
          class="px-8 py-3 text-[14px] font-bold text-black bg-primary rounded-xl hover:scale-[1.02] active:scale-95 transition-transform flex items-center gap-2">
          Tiếp theo: Cấu hình
          <PhArrowRight weight="bold" />
        </button>
      </div>
    </div>

    <!-- ── STEP 2: Configure Zones ── -->
    <div v-if="currentStep === 2" class="flex flex-col gap-6">
      <!-- SeatMap Name -->
      <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-6 md:p-8 flex flex-col gap-6">
        <div class="flex items-center gap-3 border-b border-white/5 pb-4">
          <div class="w-8 h-8 rounded-full bg-primary/10 text-primary flex items-center justify-center">
            <PhTextT weight="fill" />
          </div>
          <h2 class="text-xl font-bold font-heading text-white">Thông tin chung</h2>
        </div>
        <div class="flex flex-col gap-2">
          <label class="text-[13px] font-bold text-white/80">Tên sơ đồ ghế <span class="text-red-400">*</span></label>
          <input v-model="seatMapName" type="text"
            class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none"
            placeholder="VD: Sơ đồ ghế sân khấu chính 2026" />
          <p v-if="configErrors.seatMapName" class="text-[12px] text-red-400 flex items-center gap-1">
            <PhWarningCircle weight="fill" /> {{ configErrors.seatMapName }}
          </p>
        </div>
      </div>

      <!-- Zone Config Cards -->
      <div class="flex flex-col gap-4">
        <h2 class="text-white font-bold text-xl">Cấu hình từng phân khu</h2>
        <div v-for="(zone, zi) in zoneConfigs" :key="zone.zoneName"
          class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-6 md:p-8 flex flex-col gap-6">

          <!-- Zone Header -->
          <div class="flex items-center justify-between border-b border-white/5 pb-4">
            <div class="flex items-center gap-3">
              <div class="w-4 h-4 rounded-full border-2 border-white/20" :style="{ background: zone.color }"></div>
              <h3 class="text-lg font-bold text-white">{{ zone.zoneName }}</h3>
              <span class="text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full"
                :class="zone.isStage ? 'bg-sky-500/10 text-sky-400' : 'bg-primary/10 text-primary'">
                {{ zone.isStage ? 'Sân khấu' : (zone.isReservingSeat ? 'Ghế cố định' : 'GA (Đứng)') }}
              </span>
            </div>
            <div class="flex items-center gap-2 text-[12px] text-white/40">
              <span>{{ zone._parsed.totalSeats > 0 ? zone._parsed.totalSeats + ' ghế' : zone.capacity + ' sức chứa' }}</span>
            </div>
          </div>

          <!-- Stage Zone: read-only info -->
          <div v-if="zone.isStage" class="flex items-center gap-2 text-[13px] text-sky-400/70">
            <PhInfo weight="fill" />
            Khu sân khấu — không cần cấu hình thêm.
          </div>

          <!-- Salable Zone Config -->
          <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <!-- Color -->
            <div class="flex flex-col gap-2">
              <label class="text-[13px] font-bold text-white/80">Màu sắc</label>
              <div class="flex items-center gap-3">
                <input type="color" v-model="zone.color"
                  class="w-10 h-10 rounded-lg border-2 border-white/10 bg-transparent cursor-pointer" />
                <input v-model="zone.color" type="text"
                  class="flex-1 bg-white/5 border border-white/10 rounded-xl px-3 py-2.5 text-white text-[13px] font-mono focus:border-primary/50 focus:outline-none transition-all"
                  placeholder="#6366f1" />
              </div>
            </div>

            <!-- Display Order -->
            <div class="flex flex-col gap-2">
              <label class="text-[13px] font-bold text-white/80">Thứ tự hiển thị</label>
              <input v-model.number="zone.displayOrder" type="number" min="0"
                class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:outline-none transition-all"
                placeholder="0" />
            </div>

            <!-- Base Price -->
            <div class="flex flex-col gap-2">
              <label class="text-[13px] font-bold text-white/80">Giá vé cơ bản (VNĐ) <span class="text-red-400">*</span></label>
              <input v-model.number="zone.basePrice" type="number" min="0"
                class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:outline-none transition-all"
                placeholder="500000" />
              <p class="text-[11px] text-white/30">{{ formatPrice(zone.basePrice) }}</p>
            </div>

            <!-- Is Salable toggle -->
            <div class="flex flex-col gap-2">
              <label class="text-[13px] font-bold text-white/80">Mở bán vé</label>
              <button type="button" @click="zone.isSalable = !zone.isSalable"
                class="flex items-center gap-3 px-4 py-3 rounded-xl border transition-all"
                :class="zone.isSalable ? 'bg-primary/10 border-primary/30 text-primary' : 'bg-white/5 border-white/10 text-white/50'">
                <div class="w-5 h-5 rounded-full border-2 flex items-center justify-center transition-all"
                  :class="zone.isSalable ? 'bg-primary border-primary' : 'border-white/30'">
                  <PhCheck v-if="zone.isSalable" weight="bold" class="text-black text-[10px]" />
                </div>
                {{ zone.isSalable ? 'Có bán vé' : 'Không bán vé' }}
              </button>
            </div>

            <!-- GA Capacity (only for non-reserving zones) -->
            <div v-if="!zone.isReservingSeat" class="flex flex-col gap-2">
              <label class="text-[13px] font-bold text-white/80">Sức chứa (GA) <span class="text-red-400">*</span></label>
              <input v-model.number="zone.capacity" type="number" min="1"
                class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:outline-none transition-all"
                placeholder="500" />
            </div>
          </div>
        </div>
      </div>

      <!-- Zone config errors -->
      <div v-if="zoneConfigErrors.length > 0" class="bg-red-500/10 border border-red-500/20 rounded-2xl p-5 flex flex-col gap-3">
        <div class="flex items-center gap-2 text-red-400 font-bold text-[14px]">
          <PhWarningCircle weight="fill" /> Lỗi cấu hình ({{ zoneConfigErrors.length }})
        </div>
        <ul class="list-disc list-inside text-red-400/80 text-[13px] space-y-1">
          <li v-for="(err, i) in zoneConfigErrors" :key="i">{{ err }}</li>
        </ul>
      </div>

      <div class="flex justify-between">
        <button @click="goToStep(1)" class="px-6 py-3 text-[14px] font-bold text-white/70 bg-white/5 border border-white/10 rounded-xl hover:bg-white/10 transition-colors flex items-center gap-2">
          <PhArrowLeft weight="bold" /> Quay lại
        </button>
        <button @click="goToReview"
          class="px-8 py-3 text-[14px] font-bold text-black bg-primary rounded-xl hover:scale-[1.02] active:scale-95 transition-transform flex items-center gap-2">
          Xem lại & Xác nhận
          <PhArrowRight weight="bold" />
        </button>
      </div>
    </div>

    <!-- ── STEP 3: Review & Submit ── -->
    <div v-if="currentStep === 3" class="flex flex-col gap-6">
      <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-6 md:p-8 flex flex-col gap-6">
        <h2 class="text-xl font-bold text-white border-b border-white/5 pb-4">Tóm tắt sơ đồ ghế</h2>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="flex flex-col gap-1">
            <span class="text-[12px] text-white/40 font-medium uppercase tracking-wider">Tên sơ đồ</span>
            <span class="text-white font-bold text-[16px]">{{ seatMapName }}</span>
          </div>
          <div class="flex flex-col gap-1">
            <span class="text-[12px] text-white/40 font-medium uppercase tracking-wider">File SVG</span>
            <span class="text-white font-bold text-[16px]">{{ svgFileName }}</span>
          </div>
          <div class="flex flex-col gap-1">
            <span class="text-[12px] text-white/40 font-medium uppercase tracking-wider">Kích thước</span>
            <span class="text-white font-bold">{{ parsedData?.svgWidth }}×{{ parsedData?.svgHeight }}px</span>
          </div>
          <div class="flex flex-col gap-1">
            <span class="text-[12px] text-white/40 font-medium uppercase tracking-wider">Tổng ghế</span>
            <span class="text-primary font-black text-[20px]">{{ totalStats.totalSeats }} ghế</span>
          </div>
        </div>

        <!-- Zone summary table -->
        <div class="overflow-x-auto rounded-2xl border border-white/5">
          <table class="w-full text-left">
            <thead class="bg-white/5">
              <tr>
                <th class="px-4 py-3 text-[11px] uppercase tracking-wider text-white/50">Zone</th>
                <th class="px-4 py-3 text-[11px] uppercase tracking-wider text-white/50">Loại</th>
                <th class="px-4 py-3 text-[11px] uppercase tracking-wider text-white/50 text-center">Hàng / Ghế</th>
                <th class="px-4 py-3 text-[11px] uppercase tracking-wider text-white/50 text-right">Giá vé</th>
                <th class="px-4 py-3 text-[11px] uppercase tracking-wider text-white/50 text-center">Thứ tự</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-white/5">
              <tr v-for="zone in zoneConfigs" :key="zone.zoneName" class="text-[13px]">
                <td class="px-4 py-3">
                  <div class="flex items-center gap-2">
                    <div class="w-3 h-3 rounded-full flex-shrink-0" :style="{ background: zone.color }"></div>
                    <span class="font-bold text-white">{{ zone.zoneName }}</span>
                  </div>
                </td>
                <td class="px-4 py-3 text-white/60">{{ zone.isStage ? 'Sân khấu' : (zone.isReservingSeat ? 'Ghế cố định' : 'Đứng tự do') }}</td>
                <td class="px-4 py-3 text-center">
                  <span v-if="zone._parsed.rowCount > 0" class="text-white/80">
                    {{ zone._parsed.rowCount }} hàng / {{ zone._parsed.totalSeats }} ghế
                  </span>
                  <span v-else-if="zone.capacity > 0" class="text-white/80">{{ zone.capacity }} sức chứa</span>
                  <span v-else class="text-white/30">—</span>
                </td>
                <td class="px-4 py-3 text-right font-bold text-primary">
                  {{ zone.isStage ? '—' : (zone.basePrice > 0 ? formatPrice(zone.basePrice) : 'Miễn phí') }}
                </td>
                <td class="px-4 py-3 text-center text-white/60">{{ zone.displayOrder }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Validation Errors from full validate -->
      <div v-if="submitErrors.length > 0" class="bg-red-500/10 border border-red-500/20 rounded-2xl p-5 flex flex-col gap-3">
        <div class="flex items-center gap-2 text-red-400 font-bold text-[14px]">
          <PhWarningCircle weight="fill" /> Lỗi validation ({{ submitErrors.length }})
        </div>
        <ul class="list-disc list-inside text-red-400/80 text-[13px] space-y-1">
          <li v-for="(err, i) in submitErrors" :key="i">{{ err }}</li>
        </ul>
      </div>

      <!-- API BE Errors -->
      <div v-if="apiErrors.length > 0" class="bg-red-500/10 border border-red-500/20 rounded-2xl p-5 flex flex-col gap-3">
        <div class="flex items-center gap-2 text-red-400 font-bold text-[14px]">
          <PhWarningCircle weight="fill" /> Lỗi từ máy chủ
        </div>
        <p v-if="apiErrorMessage" class="text-red-400/80 text-[13px]">{{ apiErrorMessage }}</p>
        <ul class="list-disc list-inside text-red-400/80 text-[13px] space-y-1">
          <li v-for="(err, i) in apiErrors" :key="i"><b>{{ err.field }}:</b> {{ err.message }}</li>
        </ul>
      </div>

      <!-- Success -->
      <div v-if="isSuccess" class="bg-primary/10 border border-primary/20 rounded-2xl p-5 flex items-center gap-3">
        <PhCheckCircle weight="fill" class="text-primary text-2xl flex-shrink-0" />
        <span class="text-primary font-bold">Tạo sơ đồ ghế thành công! Đang chuyển hướng...</span>
      </div>

      <div class="flex justify-between">
        <button @click="goToStep(2)" :disabled="isSubmitting"
          class="px-6 py-3 text-[14px] font-bold text-white/70 bg-white/5 border border-white/10 rounded-xl hover:bg-white/10 transition-colors flex items-center gap-2 disabled:opacity-50">
          <PhArrowLeft weight="bold" /> Quay lại
        </button>
        <button @click="handleSubmit" :disabled="isSubmitting"
          class="px-8 py-3 text-[14px] font-bold text-black bg-primary rounded-xl hover:scale-[1.02] active:scale-95 transition-transform flex items-center gap-2 disabled:opacity-50 disabled:hover:scale-100 disabled:cursor-not-allowed">
          <PhCircleNotch v-if="isSubmitting" class="animate-spin w-5 h-5" weight="bold" />
          <PhFloppyDisk v-else weight="fill" />
          {{ isSubmitting ? submitStatus : 'Tạo Sơ đồ Ghế' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, reactive, watch, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Stage as VStage, Layer as VLayer, Rect as VRect, Path as VPath, Text as VText, Circle as VCircle } from 'vue-konva'
import { parseSVGFile } from '../../utils/svgParser.js'
import { validateSeatMap, validateZoneConfig } from '../../utils/svgValidator.js'
import { createSeatMap, uploadSVGFile } from '../../services/venue.service.js'
import {
  PhArrowLeft, PhArrowRight, PhUploadSimple, PhWarningCircle, PhWarning,
  PhCheckCircle, PhCheck, PhCircleNotch, PhFloppyDisk, PhTextT, PhInfo
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()
const venueId = route.params.id

// ── State ──
const currentStep = ref(0)
const steps = ['Upload SVG', 'Preview', 'Cấu hình', 'Xác nhận']

const fileInputRef = ref(null)
const isDragging = ref(false)
const svgFileName = ref('')
const svgFile = ref(null)
const parsedData = ref(null)
const parseErrors = ref([])
const parseWarnings = ref([])

const seatMapName = ref('')
const zoneConfigs = ref([])
const configErrors = reactive({})
const zoneConfigErrors = ref([])

const isSubmitting = ref(false)
const submitStatus = ref('')
const submitErrors = ref([])
const apiErrors = ref([])
const apiErrorMessage = ref('')
const isSuccess = ref(false)

const hoveredSeat = ref(null)
const konvaContainer = ref(null)

// ── Computed ──
const totalStats = computed(() => {
  if (!parsedData.value) return { zones: 0, salableZones: 0, totalRows: 0, totalSeats: 0 }
  const zones = parsedData.value.zones
  return {
    zones: zones.length,
    salableZones: zones.filter(z => z.isSalable).length,
    totalRows: zones.reduce((s, z) => s + z._parsed.rowCount, 0),
    totalSeats: zones.reduce((s, z) => s + z._parsed.totalSeats, 0)
  }
})

// Konva stage config — scale to container width
const stageConfig = computed(() => {
  if (!parsedData.value) return { width: 600, height: 400 }
  const containerW = konvaContainer.value?.clientWidth || 800
  const svgW = parsedData.value.svgWidth || 999
  const svgH = parsedData.value.svgHeight || 666
  const scale = Math.min(containerW / svgW, 1)
  return {
    width: Math.round(svgW * scale),
    height: Math.round(svgH * scale),
    scaleX: scale,
    scaleY: scale
  }
})

const tooltipBgConfig = computed(() => {
  if (!hoveredSeat.value) return { visible: false }
  const { x, y } = hoveredSeat.value
  return { x: x + 15, y: y - 30, width: 140, height: 28, fill: 'rgba(0,0,0,0.8)', cornerRadius: 6, visible: true }
})

const tooltipTextConfig = computed(() => {
  if (!hoveredSeat.value) return { visible: false }
  const { x, y, zoneName, rowLabel, seatName } = hoveredSeat.value
  return {
    x: x + 20, y: y - 22,
    text: `${zoneName} · Hàng ${rowLabel} · Ghế ${seatName}`,
    fontSize: 11, fill: '#ffffff', visible: true
  }
})

// ── Methods ──
function triggerFileInput() {
  fileInputRef.value?.click()
}

async function processFile(file) {
  if (!file || !file.name.endsWith('.svg')) {
    parseErrors.value = ['Chỉ chấp nhận file có định dạng .svg']
    return
  }

  svgFile.value = file
  svgFileName.value = file.name
  parseErrors.value = []
  parseWarnings.value = []
  parsedData.value = null

  const result = await parseSVGFile(file)

  if (result.errors.length > 0) {
    parseErrors.value = result.errors
    return
  }

  parseWarnings.value = result.warnings
  parsedData.value = result.parsed

  // Initialize zoneConfigs from parsed zones
  zoneConfigs.value = result.parsed.zones.map(z => ({
    ...z,
    color: z.isStage ? '#38bdf8' : defaultZoneColors[Math.random() > 0.5 ? 0 : 1],
    basePrice: 0,
    displayOrder: z.isStage ? 0 : 1
  }))
}

const defaultZoneColors = ['#6366f1', '#f59e0b', '#10b981', '#ec4899', '#8b5cf6', '#f97316']
let colorIndex = 0

watch(parsedData, (newData) => {
  if (!newData) return
  colorIndex = 0
  zoneConfigs.value = newData.zones.map(z => {
    const color = z.isStage ? '#38bdf8' : defaultZoneColors[colorIndex++ % defaultZoneColors.length]
    return { ...z, color, basePrice: 0, displayOrder: z.isStage ? 0 : colorIndex }
  })
}, { immediate: false })

function onFileChange(e) {
  processFile(e.target.files[0])
}

function onDrop(e) {
  isDragging.value = false
  processFile(e.dataTransfer.files[0])
}

function goToStep(step) {
  currentStep.value = step
  if (step === 1) {
    nextTick(() => {
      // re-init Konva with correct container size
    })
  }
}

function goToReview() {
  // Validate config
  zoneConfigErrors.value = []
  if (!seatMapName.value.trim()) {
    zoneConfigErrors.value.push('Tên sơ đồ ghế không được để trống.')
    return
  }
  const { valid, errors } = validateZoneConfig(zoneConfigs.value)
  if (!valid) {
    zoneConfigErrors.value = errors
    return
  }
  currentStep.value = 3
  submitErrors.value = []
  apiErrors.value = []
  apiErrorMessage.value = ''
}

function buildPathConfig(el, zone) {
  return {
    data: el.data,
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
    align: 'center',
    listening: false
  }
}

function buildSeatConfig(seat, zone) {
  return {
    x: seat.x,
    y: seat.y,
    radius: seat.radius,
    fill: '#0b0f19',
    stroke: zone.color,
    strokeWidth: 2,
    id: seat.svgElementId
  }
}

function onSeatHover(seat, zone) {
  hoveredSeat.value = {
    x: seat.x,
    y: seat.y,
    zoneName: zone.zoneName,
    rowLabel: zone.rows?.find(r => r.seatRequests.some(s => s.svgElementId === seat.svgElementId))?.rowLabel || '',
    seatName: seat.seatName
  }
}

function formatPrice(val) {
  if (!val || isNaN(val)) return ''
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

async function handleSubmit() {
  // Final full validation
  const { valid, errors } = validateSeatMap({
    seatMapName: seatMapName.value,
    svgWidth: parsedData.value?.svgWidth,
    svgHeight: parsedData.value?.svgHeight,
    zones: zoneConfigs.value
  })

  if (!valid) {
    submitErrors.value = errors
    return
  }
  submitErrors.value = []

  isSubmitting.value = true
  apiErrors.value = []
  apiErrorMessage.value = ''

  try {
    // Step 1: Upload SVG
    submitStatus.value = 'Đang tải file SVG...'
    const uploadRes = await uploadSVGFile(svgFile.value)
    const rawUrl = uploadRes?.data?.fileUrl || uploadRes?.data?.url || uploadRes?.data || ''
    // Chỉ lấy phần path từ /uploads trở đi, bỏ protocol://host:port
    let svgFileUrl = rawUrl
    try {
      const parsed = new URL(rawUrl)
      svgFileUrl = parsed.pathname  // e.g. /uploads/seatmaps/abc123.svg
    } catch {
      // rawUrl đã là path rồi (không có host), giữ nguyên
      svgFileUrl = rawUrl
    }

    // Step 2: Build payload
    submitStatus.value = 'Đang tạo sơ đồ ghế...'
    const payload = {
      seatMapName: seatMapName.value.trim(),
      width: parsedData.value.svgWidth,
      height: parsedData.value.svgHeight,
      svgFileUrl: svgFileUrl,
      zones: zoneConfigs.value.map(zone => ({
        zoneName: zone.zoneName,
        color: zone.color,
        x: zone.x,
        y: zone.y,
        width: zone.width,
        height: zone.height,
        capacity: zone.isReservingSeat ? zone._parsed.totalSeats : (zone.capacity || 0),
        isStage: zone.isStage,
        isReservingSeat: zone.isReservingSeat,
        isSalable: zone.isSalable,
        svgElementId: zone.svgElementId,
        basePrice: zone.basePrice || 0,
        displayOrder: zone.displayOrder || 0,
        svgElements: zone.svgElements || [],
        rows: zone.rows || []
      }))
    }

    // Step 3: POST
    const res = await createSeatMap(venueId, payload)

    if (res.success) {
      isSuccess.value = true
      setTimeout(() => {
        router.push(`/moderator/venues/${venueId}/seat-maps`)
      }, 1800)
    } else {
      apiErrorMessage.value = res.message || 'Tạo sơ đồ ghế thất bại.'
      if (res.errors) apiErrors.value = res.errors
    }
  } catch (err) {
    const errData = err.response?.data
    if (errData?.success === false) {
      apiErrorMessage.value = errData.message || 'Đã xảy ra lỗi.'
      apiErrors.value = errData.errors || []
    } else {
      apiErrorMessage.value = 'Không thể kết nối đến máy chủ. Vui lòng thử lại.'
    }
    console.error('Submit SeatMap error:', err)
  } finally {
    isSubmitting.value = false
    submitStatus.value = ''
  }
}
</script>
