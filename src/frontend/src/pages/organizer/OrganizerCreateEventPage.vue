<template>
  <div class="max-w-[920px] mx-auto py-10 px-4 md:px-0 min-h-screen animate-fade-up">

    <!-- ── Header ──────────────────────────────────────────────────── -->
    <div class="mb-10">
      <router-link
        to="/organizer"
        class="inline-flex items-center gap-1.5 text-[13px] font-bold text-white/40 hover:text-primary transition-colors mb-5 cursor-pointer"
      >
        <PhArrowLeft weight="bold" />
        Quay lại Organizer
      </router-link>
      <h1 class="font-heading text-3xl md:text-4xl font-black text-white tracking-tight mb-1.5">
        Tạo sự kiện mới
      </h1>
      <p class="text-white/40 font-medium">Điền đầy đủ thông tin để đăng sự kiện của bạn lên nền tảng.</p>
    </div>

    <!-- ── Step Indicator ──────────────────────────────────────────── -->
    <div class="flex items-center gap-0 mb-10 overflow-x-auto pb-2">
      <template v-for="(step, idx) in steps" :key="step.key">
        <div class="flex items-center gap-2 flex-shrink-0">
          <button
            @click="goToStep(idx)"
            :disabled="idx > maxReachedStep"
            class="flex items-center gap-2 px-3 py-2 rounded-xl transition-all cursor-pointer disabled:cursor-not-allowed"
            :class="[
              currentStep === idx
                ? 'bg-primary/10 text-primary border border-primary/30'
                : idx < currentStep
                  ? 'text-white/60 hover:text-white'
                  : 'text-white/25 opacity-50'
            ]"
          >
            <div
              class="w-6 h-6 rounded-full flex items-center justify-center text-[11px] font-black flex-shrink-0"
              :class="[
                currentStep === idx ? 'bg-primary text-black'
                  : idx < currentStep ? 'bg-white/15 text-white/70'
                  : 'bg-white/5 text-white/30'
              ]"
            >
              <PhCheck v-if="idx < currentStep" weight="bold" class="text-[10px]" />
              <span v-else>{{ idx + 1 }}</span>
            </div>
            <span class="text-[13px] font-bold whitespace-nowrap">{{ step.label }}</span>
          </button>
        </div>
        <div v-if="idx < steps.length - 1" class="flex-1 min-w-[16px] h-[1px] bg-white/10 mx-1" />
      </template>
    </div>

    <!-- ── STEP 0: Chọn loại sự kiện ──────────────────────────────── -->
    <transition name="step-fade" mode="out-in">
      <div v-if="currentStep === 0" key="step0">
        <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] p-8 md:p-10">
          <div class="flex items-center gap-3 mb-8 pb-6 border-b border-white/5">
            <div class="w-9 h-9 rounded-[10px] bg-primary/10 text-primary flex items-center justify-center">
              <PhLightbulb weight="fill" />
            </div>
            <div>
              <h2 class="font-heading text-xl font-black text-white">Chọn loại địa điểm</h2>
              <p class="text-white/40 text-[13px] mt-0.5">Sự kiện của bạn sẽ diễn ra ở đâu?</p>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
            <!-- Option 1: Địa điểm tự nhập -->
            <button
              @click="selectMode('manual')"
              class="group relative p-7 rounded-[1.5rem] border-2 text-left transition-all cursor-pointer"
              :class="form.mode === 'manual'
                ? 'border-primary bg-primary/5 shadow-[0_0_30px_rgba(0,200,83,0.1)]'
                : 'border-white/8 bg-white/[0.02] hover:border-white/20 hover:bg-white/[0.04]'"
            >
              <div
                class="w-12 h-12 rounded-2xl flex items-center justify-center mb-5 transition-colors"
                :class="form.mode === 'manual' ? 'bg-primary/15 text-primary' : 'bg-white/5 text-white/40 group-hover:text-white/70'"
              >
                <PhMapPin weight="duotone" class="text-2xl" />
              </div>
              <h3 class="font-heading text-[17px] font-black text-white mb-2">Địa điểm tự nhập</h3>
              <p class="text-white/40 text-[13px] leading-relaxed">
                Nhập tên địa điểm và địa chỉ thủ công. Phù hợp với sự kiện ở địa điểm chưa có trên hệ thống.
              </p>
              <div v-if="form.mode === 'manual'" class="absolute top-4 right-4 w-6 h-6 rounded-full bg-primary flex items-center justify-center">
                <PhCheck weight="bold" class="text-black text-[11px]" />
              </div>
            </button>

            <!-- Option 2: Chọn Venue & SeatMap -->
            <button
              @click="selectMode('seatmap')"
              class="group relative p-7 rounded-[1.5rem] border-2 text-left transition-all cursor-pointer"
              :class="form.mode === 'seatmap'
                ? 'border-[#818cf8] bg-[#818cf8]/5 shadow-[0_0_30px_rgba(129,140,248,0.1)]'
                : 'border-white/8 bg-white/[0.02] hover:border-white/20 hover:bg-white/[0.04]'"
            >
              <div
                class="w-12 h-12 rounded-2xl flex items-center justify-center mb-5 transition-colors"
                :class="form.mode === 'seatmap' ? 'bg-[#818cf8]/15 text-[#818cf8]' : 'bg-white/5 text-white/40 group-hover:text-white/70'"
              >
                <PhSquaresFour weight="duotone" class="text-2xl" />
              </div>
              <h3 class="font-heading text-[17px] font-black text-white mb-2">Chọn Venue & Sơ đồ ghế</h3>
              <p class="text-white/40 text-[13px] leading-relaxed">
                Chọn địa điểm và sơ đồ chỗ ngồi từ hệ thống. Mỗi khu vực (zone) sẽ tương ứng một loại vé.
              </p>
              <div v-if="form.mode === 'seatmap'" class="absolute top-4 right-4 w-6 h-6 rounded-full bg-[#818cf8] flex items-center justify-center">
                <PhCheck weight="bold" class="text-black text-[11px]" />
              </div>
            </button>
          </div>

          <div class="flex justify-end mt-8">
            <button
              @click="nextStep"
              :disabled="!form.mode"
              class="inline-flex items-center gap-2 px-7 py-3 rounded-xl text-[14px] font-bold text-black bg-primary hover:scale-[1.02] active:scale-95 transition-transform disabled:opacity-40 disabled:cursor-not-allowed disabled:hover:scale-100 cursor-pointer"
            >
              Tiếp theo
              <PhArrowRight weight="bold" />
            </button>
          </div>
        </div>
      </div>

      <!-- ── STEP 1: Thông tin cơ bản ───────────────────────────────── -->
      <div v-else-if="currentStep === 1" key="step1">
        <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] p-8 md:p-10 flex flex-col gap-7">
          <div class="flex items-center gap-3 pb-6 border-b border-white/5">
            <div class="w-9 h-9 rounded-[10px] bg-primary/10 text-primary flex items-center justify-center">
              <PhNotePencil weight="fill" />
            </div>
            <div>
              <h2 class="font-heading text-xl font-black text-white">Thông tin cơ bản</h2>
              <p class="text-white/40 text-[13px] mt-0.5">Tiêu đề, danh mục và thông tin thời gian</p>
            </div>
          </div>

          <!-- Tiêu đề -->
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
              Tên sự kiện <span class="text-danger">*</span>
            </label>
            <input
              v-model="form.title"
              type="text"
              id="event-title"
              placeholder="VD: Concert Mùa Hè 2026 - Sơn Tùng M-TP"
              class="w-full bg-white/4 border rounded-xl px-4 py-3 text-[15px] font-bold text-white placeholder-white/20 transition-all focus:outline-none focus:bg-white/6"
              :class="errors.title ? 'border-danger/60 focus:border-danger/80' : 'border-white/10 focus:border-primary/50'"
            />
            <p v-if="errors.title" class="text-[12px] text-danger flex items-center gap-1.5">
              <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors.title }}
            </p>
          </div>

          <!-- Category -->
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
              Danh mục <span class="text-danger">*</span>
            </label>
            <div class="relative">
              <PhTag weight="bold" class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 pointer-events-none" />
              <select
                v-model="form.categoryId"
                id="event-category"
                class="w-full bg-white/4 border border-white/10 rounded-xl pl-10 pr-4 py-3 text-[14px] font-bold text-white focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none appearance-none"
                :class="errors.categoryId ? 'border-danger/60' : 'border-white/10'"
                :disabled="isLoadingCategories"
              >
                <option value="" class="text-black bg-white">
                  {{ isLoadingCategories ? 'Đang tải...' : 'Chọn danh mục sự kiện' }}
                </option>
                <option
                  v-for="cat in categories"
                  :key="cat.id"
                  :value="cat.id"
                  class="text-black bg-white"
                >{{ cat.categoryName }}</option>
              </select>
            </div>
            <p v-if="errors.categoryId" class="text-[12px] text-danger flex items-center gap-1.5">
              <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors.categoryId }}
            </p>
          </div>

          <!-- Các suất chiếu -->
          <div class="flex flex-col gap-4">
            <div class="flex items-center justify-between">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Các suất chiếu <span class="text-danger">*</span>
              </label>
              <button
                type="button"
                @click="addShowTime"
                class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary/10 text-primary text-[12px] font-bold hover:bg-primary/20 transition-colors"
              >
                <PhPlus weight="bold" /> Thêm suất chiếu
              </button>
            </div>

            <div class="flex flex-col gap-3">
              <div
                v-for="(st, idx) in form.showTimes"
                :key="idx"
                class="flex flex-col md:flex-row items-start md:items-center gap-4 p-4 rounded-xl border border-white/10 bg-white/5 relative group transition-all hover:border-white/20"
              >
                <!-- Nút xóa -->
                <button
                  v-if="form.showTimes.length > 1"
                  type="button"
                  @click="removeShowTime(idx)"
                  class="absolute -top-2 -right-2 w-6 h-6 rounded-full bg-danger text-white flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity shadow-lg z-10 cursor-pointer hover:bg-danger/80"
                >
                  <PhX weight="bold" class="text-[10px]" />
                </button>

                <div class="flex-1 w-full grid grid-cols-1 md:grid-cols-2 gap-4">
                  <!-- Start At -->
                  <div class="flex flex-col gap-2">
                    <label class="text-[11px] font-bold text-white/40 uppercase">Bắt đầu</label>
                    <div class="relative">
                      <PhCalendarBlank weight="bold" class="absolute left-3 top-1/2 -translate-y-1/2 text-white/30" />
                      <input
                        v-model="st.startAt"
                        type="datetime-local"
                        class="w-full bg-white/5 border rounded-lg pl-9 pr-3 py-2.5 text-[13px] font-bold text-white focus:outline-none transition-colors [color-scheme:dark]"
                        :class="errors[`showTimes[${idx}].startAt`] ? 'border-danger/60 focus:border-danger/80' : 'border-white/10 focus:border-primary/50'"
                      />
                    </div>
                    <p v-if="errors[`showTimes[${idx}].startAt`]" class="text-[11px] text-danger mt-1">
                      {{ errors[`showTimes[${idx}].startAt`] }}
                    </p>
                  </div>

                  <!-- End At -->
                  <div class="flex flex-col gap-2">
                    <label class="text-[11px] font-bold text-white/40 uppercase">Kết thúc</label>
                    <div class="relative">
                      <PhCalendarBlank weight="bold" class="absolute left-3 top-1/2 -translate-y-1/2 text-white/30" />
                      <input
                        v-model="st.endAt"
                        type="datetime-local"
                        class="w-full bg-white/5 border rounded-lg pl-9 pr-3 py-2.5 text-[13px] font-bold text-white focus:outline-none transition-colors [color-scheme:dark]"
                        :class="errors[`showTimes[${idx}].endAt`] ? 'border-danger/60 focus:border-danger/80' : 'border-white/10 focus:border-primary/50'"
                      />
                    </div>
                    <p v-if="errors[`showTimes[${idx}].endAt`]" class="text-[11px] text-danger mt-1">
                      {{ errors[`showTimes[${idx}].endAt`] }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
            
            <p v-if="errors.showTimes" class="text-[12px] text-danger flex items-center gap-1.5">
              <PhWarningCircle weight="fill" /> {{ errors.showTimes }}
            </p>
          </div>

          <!-- Thời gian bán vé -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Mở bán vé <span class="text-danger">*</span>
              </label>
              <div class="relative group">
                <PhTicket weight="bold" class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 pointer-events-none group-focus-within:text-primary transition-colors" />
                <input
                  v-model="form.saleOpenAt"
                  type="datetime-local"
                  id="sale-open"
                  class="w-full bg-white/4 border rounded-xl pl-10 pr-4 py-3 text-[14px] font-bold text-white focus:outline-none transition-all focus:bg-white/6 [color-scheme:dark]"
                  :class="errors.saleOpenAt ? 'border-danger/60 focus:border-danger/80' : 'border-white/10 focus:border-primary/50'"
                />
              </div>
              <p v-if="errors.saleOpenAt" class="text-[12px] text-danger flex items-center gap-1.5">
                <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors.saleOpenAt }}
              </p>
            </div>
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Đóng bán vé <span class="text-danger">*</span>
              </label>
              <div class="relative group">
                <PhTicket weight="bold" class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 pointer-events-none group-focus-within:text-primary transition-colors" />
                <input
                  v-model="form.saleCloseAt"
                  type="datetime-local"
                  id="sale-close"
                  class="w-full bg-white/4 border rounded-xl pl-10 pr-4 py-3 text-[14px] font-bold text-white focus:outline-none transition-all focus:bg-white/6 [color-scheme:dark]"
                  :class="errors.saleCloseAt ? 'border-danger/60 focus:border-danger/80' : 'border-white/10 focus:border-primary/50'"
                />
              </div>
              <p v-if="errors.saleCloseAt" class="text-[12px] text-danger flex items-center gap-1.5">
                <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors.saleCloseAt }}
              </p>
            </div>
          </div>

          <!-- Mô tả - Rich HTML editor -->
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">Mô tả sự kiện</label>

            <!-- Toolbar -->
            <div class="flex flex-wrap items-center gap-1 px-3 py-2 bg-white/4 border border-white/10 rounded-t-xl border-b-white/5">
              <button
                v-for="tool in editorTools"
                :key="tool.cmd"
                type="button"
                @click="execCommand(tool.cmd, tool.val)"
                class="w-7 h-7 rounded-lg flex items-center justify-center text-white/50 hover:text-white hover:bg-white/10 transition-colors cursor-pointer text-[13px] font-bold"
                :title="tool.label"
              >
                <component :is="tool.icon" v-if="tool.icon" weight="bold" />
                <span v-else>{{ tool.label }}</span>
              </button>
              <div class="w-[1px] h-5 bg-white/10 mx-1" />
              <select
                @change="e => execCommand('formatBlock', e.target.value)"
                class="bg-transparent text-white/50 text-[12px] font-bold focus:outline-none cursor-pointer hover:text-white transition-colors"
              >
                <option value="p" class="text-black bg-white">Đoạn văn</option>
                <option value="h2" class="text-black bg-white">Tiêu đề lớn</option>
                <option value="h3" class="text-black bg-white">Tiêu đề nhỏ</option>
              </select>
            </div>

            <!-- Editable area -->
            <div
              ref="editorRef"
              id="event-description"
              contenteditable="true"
              @input="onDescriptionInput"
              class="min-h-[200px] bg-white/4 border border-white/10 border-t-0 rounded-b-xl px-4 py-3 text-[14px] text-white/80 leading-relaxed focus:outline-none focus:border-primary/50 transition-all prose-custom"
              :class="{ 'rounded-b-xl': true }"
              :data-placeholder="'Mô tả chi tiết nội dung sự kiện, quy định, thông tin nghệ sĩ...'"
            />
          </div>

          <div class="flex items-center justify-between pt-4 border-t border-white/5">
            <button
              @click="prevStep"
              class="inline-flex items-center gap-2 px-5 py-3 rounded-xl text-[14px] font-bold text-white/60 bg-white/5 hover:bg-white/10 hover:text-white transition-all cursor-pointer"
            >
              <PhArrowLeft weight="bold" /> Quay lại
            </button>
            <button
              @click="nextStep"
              class="inline-flex items-center gap-2 px-7 py-3 rounded-xl text-[14px] font-bold text-black bg-primary hover:scale-[1.02] active:scale-95 transition-transform cursor-pointer"
            >
              Tiếp theo <PhArrowRight weight="bold" />
            </button>
          </div>
        </div>
      </div>

      <!-- ── STEP 2: Ảnh bìa ─────────────────────────────────────── -->
      <div v-else-if="currentStep === 2" key="step2">
        <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] p-8 md:p-10 flex flex-col gap-7">
          <div class="flex items-center gap-3 pb-6 border-b border-white/5">
            <div class="w-9 h-9 rounded-[10px] bg-primary/10 text-primary flex items-center justify-center">
              <PhImage weight="fill" />
            </div>
            <div>
              <h2 class="font-heading text-xl font-black text-white">Ảnh bìa sự kiện</h2>
              <p class="text-white/40 text-[13px] mt-0.5">Tải lên ảnh bìa kích thước 1280 x 720 px (tỉ lệ 16:9)</p>
            </div>
          </div>

          <!-- Upload zone -->
          <div
            class="relative group border-2 border-dashed rounded-2xl transition-all overflow-hidden cursor-pointer"
            :class="[
              isDragging ? 'border-primary bg-primary/5' : 'border-white/15 hover:border-white/30 hover:bg-white/[0.02]',
              errors.coverImageUrl ? 'border-danger/40' : ''
            ]"
            @dragover.prevent="isDragging = true"
            @dragleave="isDragging = false"
            @drop.prevent="handleFileDrop"
            @click="triggerFileInput"
          >
            <!-- Preview -->
            <div v-if="form.coverImagePreview" class="relative">
              <img
                :src="form.coverImagePreview"
                alt="Ảnh bìa sự kiện"
                class="w-full object-cover rounded-2xl"
                style="aspect-ratio: 16/9"
              />
              <div class="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity rounded-2xl flex flex-col items-center justify-center gap-2">
                <PhUpload class="text-3xl text-white" weight="bold" />
                <span class="text-white font-bold text-[14px]">Thay ảnh khác</span>
              </div>
              <button
                type="button"
                @click.stop="removeImage"
                class="absolute top-3 right-3 w-8 h-8 rounded-full bg-black/60 text-white flex items-center justify-center hover:bg-danger transition-colors cursor-pointer"
              >
                <PhX weight="bold" class="text-[12px]" />
              </button>
            </div>

            <!-- Empty state -->
            <div v-else class="flex flex-col items-center justify-center py-16 px-8 text-center">
              <div
                class="w-16 h-16 rounded-2xl flex items-center justify-center mb-5 transition-colors"
                :class="isDragging ? 'bg-primary/20 text-primary' : 'bg-white/5 text-white/30'"
              >
                <PhImage weight="duotone" class="text-3xl" />
              </div>
              <p class="text-white font-bold text-[15px] mb-1.5">
                {{ isDragging ? 'Thả ảnh vào đây' : 'Kéo thả ảnh hoặc nhấn để chọn' }}
              </p>
              <p class="text-white/30 text-[13px]">PNG, JPG, WEBP. Kích thước tối ưu: 1280 × 720 px</p>

              <!-- Upload progress -->
              <div v-if="isUploadingImage" class="mt-5 w-full max-w-[240px]">
                <div class="flex justify-between text-[12px] font-bold mb-2">
                  <span class="text-white/50">Đang tải lên...</span>
                  <span class="text-primary">{{ uploadProgress }}%</span>
                </div>
                <div class="w-full h-1.5 bg-white/10 rounded-full overflow-hidden">
                  <div
                    class="h-full bg-primary rounded-full transition-all duration-300"
                    :style="{ width: uploadProgress + '%' }"
                  />
                </div>
              </div>
            </div>

            <input
              ref="fileInputRef"
              type="file"
              accept="image/png,image/jpeg,image/webp"
              class="hidden"
              @change="handleFileSelect"
            />
          </div>

          <p v-if="errors.coverImageUrl" class="text-[12px] text-danger flex items-center gap-1.5 -mt-4">
            <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors.coverImageUrl }}
          </p>

          <!-- API error for upload -->
          <div v-if="uploadError" class="px-4 py-3 rounded-xl bg-danger/10 border border-danger/20 text-danger text-[13px] font-medium flex items-start gap-2">
            <PhWarningCircle weight="fill" class="w-5 h-5 flex-shrink-0 mt-0.5" />
            <span>{{ uploadError }}</span>
          </div>

          <div class="flex items-center justify-between pt-4 border-t border-white/5">
            <button @click="prevStep" class="inline-flex items-center gap-2 px-5 py-3 rounded-xl text-[14px] font-bold text-white/60 bg-white/5 hover:bg-white/10 hover:text-white transition-all cursor-pointer">
              <PhArrowLeft weight="bold" /> Quay lại
            </button>
            <button @click="nextStep" class="inline-flex items-center gap-2 px-7 py-3 rounded-xl text-[14px] font-bold text-black bg-primary hover:scale-[1.02] active:scale-95 transition-transform cursor-pointer">
              Tiếp theo <PhArrowRight weight="bold" />
            </button>
          </div>
        </div>
      </div>

      <!-- ── STEP 3: Địa điểm (mode: manual) ───────────────────────── -->
      <div v-else-if="currentStep === 3 && form.mode === 'manual'" key="step3-manual">
        <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] p-8 md:p-10 flex flex-col gap-7">
          <div class="flex items-center gap-3 pb-6 border-b border-white/5">
            <div class="w-9 h-9 rounded-[10px] bg-primary/10 text-primary flex items-center justify-center">
              <PhMapPin weight="fill" />
            </div>
            <div>
              <h2 class="font-heading text-xl font-black text-white">Thông tin địa điểm</h2>
              <p class="text-white/40 text-[13px] mt-0.5">Nhập địa chỉ tổ chức sự kiện</p>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Tên địa điểm -->
            <div class="flex flex-col gap-2 md:col-span-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Tên địa điểm <span class="text-danger">*</span>
              </label>
              <input
                v-model="form.location.venueName"
                type="text"
                id="venue-name"
                placeholder="VD: Sân khấu Diamond, Nhà hát Bến Thành..."
                class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-3 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                :class="errors['location.venueName'] ? 'border-danger/60' : 'border-white/10'"
              />
              <p v-if="errors['location.venueName']" class="text-[12px] text-danger flex items-center gap-1.5">
                <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors['location.venueName'] }}
              </p>
            </div>

            <!-- Tỉnh / TP -->
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Tỉnh / Thành phố <span class="text-danger">*</span>
              </label>
              <select
                v-model="form.location.provinceCity"
                @change="onProvinceChange"
                :disabled="isLoadingProvinces"
                id="province"
                class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-3 text-[14px] font-bold text-white focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none appearance-none disabled:opacity-50"
                :class="errors['location.provinceCity'] ? 'border-danger/60' : 'border-white/10'"
              >
                <option value="" class="text-black bg-white">Chọn Tỉnh / Thành phố</option>
                <option v-for="p in provinces" :key="p.code" :value="p.name" class="text-black bg-white">{{ p.name }}</option>
              </select>
              <p v-if="errors['location.provinceCity']" class="text-[12px] text-danger flex items-center gap-1.5">
                <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors['location.provinceCity'] }}
              </p>
            </div>

            <!-- Quận / Huyện -->
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Quận / Huyện <span class="text-danger">*</span>
              </label>
              <select
                v-model="form.location.district"
                @change="onDistrictChange"
                :disabled="!selectedProvinceCode || isLoadingDistricts"
                id="district"
                class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-3 text-[14px] font-bold text-white focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none appearance-none disabled:opacity-50"
                :class="errors['location.district'] ? 'border-danger/60' : 'border-white/10'"
              >
                <option value="" class="text-black bg-white">Chọn Quận / Huyện</option>
                <option v-for="d in districts" :key="d.code" :value="d.name" class="text-black bg-white">{{ d.name }}</option>
              </select>
              <p v-if="errors['location.district']" class="text-[12px] text-danger flex items-center gap-1.5">
                <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors['location.district'] }}
              </p>
            </div>

            <!-- Phường / Xã -->
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Phường / Xã <span class="text-danger">*</span>
              </label>
              <select
                v-model="form.location.ward"
                :disabled="!selectedDistrictCode || isLoadingWards"
                id="ward"
                class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-3 text-[14px] font-bold text-white focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none appearance-none disabled:opacity-50"
                :class="errors['location.ward'] ? 'border-danger/60' : 'border-white/10'"
              >
                <option value="" class="text-black bg-white">Chọn Phường / Xã</option>
                <option v-for="w in wards" :key="w.code" :value="w.name" class="text-black bg-white">{{ w.name }}</option>
              </select>
              <p v-if="errors['location.ward']" class="text-[12px] text-danger flex items-center gap-1.5">
                <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors['location.ward'] }}
              </p>
            </div>

            <!-- Số nhà, đường -->
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                Số nhà, Tên đường <span class="text-danger">*</span>
              </label>
              <input
                v-model="form.location.addressLine"
                type="text"
                id="address-line"
                placeholder="VD: 123 Nguyễn Trãi"
                class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-3 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                :class="errors['location.addressLine'] ? 'border-danger/60' : 'border-white/10'"
              />
              <p v-if="errors['location.addressLine']" class="text-[12px] text-danger flex items-center gap-1.5">
                <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors['location.addressLine'] }}
              </p>
            </div>

            <!-- Quốc gia (readonly) -->
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">Quốc gia</label>
              <input
                value="Việt Nam"
                type="text"
                disabled
                class="w-full bg-black/20 border border-white/5 rounded-xl px-4 py-3 text-[14px] font-bold text-white/40 cursor-not-allowed"
              />
            </div>
          </div>

          <div class="flex items-center justify-between pt-4 border-t border-white/5">
            <button @click="prevStep" class="inline-flex items-center gap-2 px-5 py-3 rounded-xl text-[14px] font-bold text-white/60 bg-white/5 hover:bg-white/10 hover:text-white transition-all cursor-pointer">
              <PhArrowLeft weight="bold" /> Quay lại
            </button>
            <button @click="nextStep" class="inline-flex items-center gap-2 px-7 py-3 rounded-xl text-[14px] font-bold text-black bg-primary hover:scale-[1.02] active:scale-95 transition-transform cursor-pointer">
              Tiếp theo <PhArrowRight weight="bold" />
            </button>
          </div>
        </div>
      </div>

      <!-- ── STEP 3: Chọn Venue & SeatMap (mode: seatmap) ─────────── -->
      <div v-else-if="currentStep === 3 && form.mode === 'seatmap'" key="step3-seatmap">
        <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] p-8 md:p-10 flex flex-col gap-7">
          <div class="flex items-center gap-3 pb-6 border-b border-white/5">
            <div class="w-9 h-9 rounded-[10px] bg-[#818cf8]/10 text-[#818cf8] flex items-center justify-center">
              <PhSquaresFour weight="fill" />
            </div>
            <div>
              <h2 class="font-heading text-xl font-black text-white">Chọn Venue & Sơ đồ ghế</h2>
              <p class="text-white/40 text-[13px] mt-0.5">Chọn địa điểm và sơ đồ chỗ ngồi từ hệ thống</p>
            </div>
          </div>

          <!-- Search venues -->
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">Tìm kiếm địa điểm</label>
            <div class="relative group">
              <PhMagnifyingGlass weight="bold" class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 pointer-events-none group-focus-within:text-primary transition-colors" />
              <input
                v-model="venueSearch"
                @input="onVenueSearch"
                type="text"
                id="venue-search"
                placeholder="Tên địa điểm, mã..."
                class="w-full bg-white/4 border border-white/10 rounded-xl pl-10 pr-4 py-3 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
              />
            </div>
          </div>

          <!-- Venue list -->
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
              Chọn địa điểm <span class="text-danger">*</span>
            </label>

            <div v-if="isLoadingVenues" class="py-8 flex items-center justify-center gap-3 text-white/40">
              <PhCircleNotch weight="bold" class="animate-spin text-xl" />
              <span class="text-[14px] font-medium">Đang tải danh sách địa điểm...</span>
            </div>

            <div v-else-if="venues.length === 0" class="py-10 flex flex-col items-center gap-3 text-white/30 border border-white/5 rounded-xl">
              <PhBuildings class="text-3xl" weight="duotone" />
              <span class="text-[14px] font-medium">Không tìm thấy địa điểm nào</span>
            </div>

            <div v-else class="grid grid-cols-1 gap-3">
              <button
                v-for="venue in venues"
                :key="venue.id"
                type="button"
                @click="selectVenue(venue)"
                class="group relative p-4 rounded-2xl border-2 text-left transition-all cursor-pointer"
                :class="form.selectedVenueId === venue.id
                  ? 'border-[#818cf8] bg-[#818cf8]/5'
                  : 'border-white/8 hover:border-white/20 hover:bg-white/[0.02]'"
              >
                <div class="flex items-center justify-between">
                  <div class="flex flex-col gap-0.5">
                    <span class="font-bold text-white text-[14px] group-hover:text-[#818cf8] transition-colors">{{ venue.venueName }}</span>
                    <span class="text-white/40 text-[12px]">{{ venue.addressLine }}, {{ venue.district }}, {{ venue.provinceCity }}</span>
                  </div>
                  <div class="flex items-center gap-2 flex-shrink-0 ml-4">
                    <span class="text-[11px] font-mono text-white/30 bg-white/5 px-2 py-1 rounded-lg">{{ venue.venueCode }}</span>
                    <div
                      class="w-5 h-5 rounded-full border-2 flex items-center justify-center transition-all flex-shrink-0"
                      :class="form.selectedVenueId === venue.id ? 'border-[#818cf8] bg-[#818cf8]' : 'border-white/20'"
                    >
                      <PhCheck v-if="form.selectedVenueId === venue.id" weight="bold" class="text-black text-[9px]" />
                    </div>
                  </div>
                </div>
              </button>
            </div>

            <!-- Venue pagination -->
            <div v-if="venueTotalPages > 1" class="flex items-center justify-between pt-2">
              <span class="text-[12px] text-white/40">Trang {{ venueFilters.PageNumber }} / {{ venueTotalPages }}</span>
              <div class="flex gap-2">
                <button @click="venuePrevPage" :disabled="!venueHasPrev" class="px-3 py-1.5 rounded-lg bg-white/5 text-white/60 text-[12px] font-bold hover:bg-white/10 disabled:opacity-30 disabled:cursor-not-allowed transition-colors cursor-pointer">Trước</button>
                <button @click="venueNextPage" :disabled="!venueHasNext" class="px-3 py-1.5 rounded-lg bg-white/5 text-white/60 text-[12px] font-bold hover:bg-white/10 disabled:opacity-30 disabled:cursor-not-allowed transition-colors cursor-pointer">Sau</button>
              </div>
            </div>

            <p v-if="errors.selectedVenueId" class="text-[12px] text-danger flex items-center gap-1.5">
              <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors.selectedVenueId }}
            </p>
          </div>

          <!-- SeatMap select (khi đã chọn venue) -->
          <div v-if="form.selectedVenueId" class="flex flex-col gap-3 border-t border-white/5 pt-6">
            <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
              Chọn sơ đồ ghế <span class="text-danger">*</span>
            </label>

            <div v-if="isLoadingSeatMaps" class="py-6 flex items-center justify-center gap-3 text-white/40">
              <PhCircleNotch weight="bold" class="animate-spin text-xl" />
              <span class="text-[14px] font-medium">Đang tải sơ đồ ghế...</span>
            </div>

            <div v-else-if="seatMaps.length === 0" class="py-8 flex flex-col items-center gap-3 text-white/30 border border-white/5 rounded-xl">
              <PhSquaresFour class="text-3xl" weight="duotone" />
              <span class="text-[14px] font-medium">Địa điểm này chưa có sơ đồ ghế</span>
            </div>

            <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <button
                v-for="sm in seatMaps"
                :key="sm.id"
                type="button"
                @click="selectSeatMap(sm)"
                class="group relative p-4 rounded-2xl border-2 text-left transition-all cursor-pointer"
                :class="form.seatMapId === sm.id
                  ? 'border-[#818cf8] bg-[#818cf8]/5'
                  : 'border-white/8 hover:border-white/20 hover:bg-white/[0.02]'"
              >
                <div class="flex items-start justify-between gap-2">
                  <div>
                    <p class="font-bold text-white text-[14px] mb-0.5 group-hover:text-[#818cf8] transition-colors">{{ sm.seatMapName }}</p>
                    <p class="text-[11px] font-mono text-white/30">{{ sm.seatMapCode }}</p>
                  </div>
                  <div
                    class="w-5 h-5 rounded-full border-2 flex items-center justify-center transition-all flex-shrink-0 mt-0.5"
                    :class="form.seatMapId === sm.id ? 'border-[#818cf8] bg-[#818cf8]' : 'border-white/20'"
                  >
                    <PhCheck v-if="form.seatMapId === sm.id" weight="bold" class="text-black text-[9px]" />
                  </div>
                </div>
              </button>
            </div>

            <p v-if="errors.seatMapId" class="text-[12px] text-danger flex items-center gap-1.5">
              <PhWarningCircle weight="fill" class="flex-shrink-0" />{{ errors.seatMapId }}
            </p>
          </div>

          <div class="flex items-center justify-between pt-4 border-t border-white/5">
            <button @click="prevStep" class="inline-flex items-center gap-2 px-5 py-3 rounded-xl text-[14px] font-bold text-white/60 bg-white/5 hover:bg-white/10 hover:text-white transition-all cursor-pointer">
              <PhArrowLeft weight="bold" /> Quay lại
            </button>
            <button @click="nextStep" class="inline-flex items-center gap-2 px-7 py-3 rounded-xl text-[14px] font-bold text-black bg-primary hover:scale-[1.02] active:scale-95 transition-transform cursor-pointer">
              Tiếp theo <PhArrowRight weight="bold" />
            </button>
          </div>
        </div>
      </div>

      <!-- ── STEP 4: Loại vé ─────────────────────────────────────── -->
      <div v-else-if="currentStep === 4" key="step4">
        <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] p-8 md:p-10 flex flex-col gap-7">
          <div class="flex items-center gap-3 pb-6 border-b border-white/5">
            <div class="w-9 h-9 rounded-[10px] bg-primary/10 text-primary flex items-center justify-center">
              <PhTicket weight="fill" />
            </div>
            <div>
              <h2 class="font-heading text-xl font-black text-white">
                {{ form.mode === 'seatmap' ? 'Cấu hình vé theo khu vực' : 'Loại vé' }}
              </h2>
              <p class="text-white/40 text-[13px] mt-0.5">
                {{ form.mode === 'seatmap' ? 'Mỗi zone/khu vực tương ứng một loại vé' : 'Thêm các loại vé cho sự kiện' }}
              </p>
            </div>
          </div>

          <!-- SeatMap mode: load zones từ seatmap đã chọn -->
          <div v-if="form.mode === 'seatmap'">
            <div v-if="isLoadingZones" class="py-8 flex items-center justify-center gap-3 text-white/40">
              <PhCircleNotch weight="bold" class="animate-spin text-xl" />
              <span class="font-medium text-[14px]">Đang tải thông tin khu vực...</span>
            </div>

            <div v-else-if="availableZones.length === 0" class="py-10 flex flex-col items-center gap-3 text-white/30 border border-white/5 rounded-xl">
              <PhWarningCircle class="text-3xl" weight="duotone" />
              <span class="text-[14px] font-medium">Sơ đồ này không có khu vực bán vé nào</span>
            </div>

            <div v-else class="grid grid-cols-1 xl:grid-cols-2 gap-8">
              <!-- Cột trái: Sơ đồ & Chọn khu vực -->
              <div class="flex flex-col gap-4">
                <!-- Sơ đồ ghế -->
                <div class="flex flex-col gap-2">
                  <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">Sơ đồ ghế</label>
                  <div ref="konvaContainer" class="w-full bg-[#080D0B] border border-white/10 rounded-2xl overflow-hidden relative" style="height: 300px;">
                    <v-stage v-if="seatMapData" :config="stageConfig" @wheel="handleWheel" @dragend="handleDragEnd">
                      <v-layer>
                        <v-rect :config="{ x: 0, y: 0, width: realDimensions.width, height: realDimensions.height, fill: '#0b0f19' }" />
                        <template v-for="zone in seatMapData.zones" :key="zone.id">
                          <template v-for="(el, eli) in zone.svgElements" :key="`${zone.id}-el-${eli}`">
                            <v-path v-if="el.type === 'path' && el.data" :config="buildPathConfig(el)" />
                            <v-text v-if="el.type === 'text' && el.text" :config="buildTextConfig(el)" />
                          </template>
                          <template v-for="row in zone.rows" :key="`${zone.id}-row-${row.id}`">
                            <v-circle v-for="seat in row.seats" :key="seat.id" :config="buildSeatConfig(seat, zone)" />
                          </template>
                        </template>
                      </v-layer>
                    </v-stage>
                  </div>
                </div>

                <!-- Danh sách khu vực -->
                <div class="flex flex-col gap-2">
                  <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">
                    Chọn khu vực bán vé ({{ form.ticketTypes.length }}/{{ availableZones.length }})
                  </label>
                  <div class="grid grid-cols-1 sm:grid-cols-2 gap-2 max-h-[300px] overflow-y-auto pr-2 custom-scrollbar">
                    <button
                      v-for="z in availableZones" :key="z.id" type="button" @click="toggleZone(z)"
                      class="flex items-center gap-3 p-3 rounded-xl border text-left transition-colors cursor-pointer"
                      :class="isZoneSelected(z.id) ? 'bg-primary/10 border-primary/30' : 'border-white/10 hover:bg-white/5'"
                    >
                      <div class="w-3 h-3 rounded-full flex-shrink-0" :style="{ background: z.color || '#00c853' }"></div>
                      <span class="flex-1 text-[13px] font-bold text-white truncate">{{ z.zoneName }}</span>
                      <div class="w-4 h-4 rounded border flex items-center justify-center flex-shrink-0" :class="isZoneSelected(z.id) ? 'border-primary bg-primary' : 'border-white/20'">
                        <PhCheck v-if="isZoneSelected(z.id)" class="text-black text-[10px]" weight="bold" />
                      </div>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Cột phải: Cấu hình loại vé -->
              <div class="flex flex-col gap-4">
                <label class="text-[12px] font-bold text-white/60 uppercase tracking-widest">Cấu hình loại vé</label>
                
                <div v-if="form.ticketTypes.length === 0" class="py-12 flex flex-col items-center gap-3 text-white/30 border border-white/5 rounded-2xl bg-white/[0.02]">
                  <PhTicket class="text-4xl" weight="duotone" />
                  <span class="text-[13px] font-medium text-center px-4">Vui lòng chọn khu vực bên trái để cấu hình vé.</span>
                </div>

                <div v-else class="flex flex-col gap-4 max-h-[660px] overflow-y-auto pr-2 custom-scrollbar">
                  <div
                    v-for="(ticket, idx) in form.ticketTypes"
                    :key="ticket._zoneId"
                    class="p-5 rounded-2xl bg-[#0D1410] border border-white/8 relative"
                  >
                    <button type="button" @click="removeZoneTicket(ticket._zoneId)" class="absolute right-3 top-3 w-8 h-8 rounded-xl flex items-center justify-center text-white/40 hover:bg-white/10 hover:text-white transition-colors cursor-pointer">
                      <PhX weight="bold" />
                    </button>
                    
                    <div class="flex items-center gap-3 mb-5">
                      <div class="w-3 h-3 rounded-full" :style="{ background: ticket._zoneColor || '#00c853' }" />
                      <span class="font-bold text-white text-[14px]">{{ ticket._zoneName }}</span>
                    </div>

                    <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
                      <div class="flex flex-col gap-2 md:col-span-2">
                        <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Tên loại vé *</label>
                        <input v-model="ticket.ticketTypeName" type="text" placeholder="VD: Vé VIP..."
                          class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                          :class="errors[`ticketTypes[${idx}].ticketTypeName`] ? 'border-danger/60' : ''" />
                        <p v-if="errors[`ticketTypes[${idx}].ticketTypeName`]" class="text-[11px] text-danger flex items-center gap-1"><PhWarningCircle weight="fill" class="flex-shrink-0 text-[10px]" />{{ errors[`ticketTypes[${idx}].ticketTypeName`] }}</p>
                      </div>
                      <div class="flex flex-col gap-2">
                        <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Giá vé (VND) *</label>
                        <div class="relative">
                          <span class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 font-bold text-[14px]">₫</span>
                          <input v-model.number="ticket.price" type="number" min="0" placeholder="500000"
                            class="w-full bg-white/4 border border-white/10 rounded-xl pl-8 pr-4 py-2.5 text-[14px] font-bold text-primary placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                            :class="errors[`ticketTypes[${idx}].price`] ? 'border-danger/60' : ''" />
                        </div>
                        <p v-if="errors[`ticketTypes[${idx}].price`]" class="text-[11px] text-danger flex items-center gap-1"><PhWarningCircle weight="fill" class="flex-shrink-0 text-[10px]" />{{ errors[`ticketTypes[${idx}].price`] }}</p>
                      </div>
                      <div class="flex flex-col gap-2">
                        <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Số lượng phát hành *</label>
                        <input v-model.number="ticket.publishedQuota" type="number" min="1" placeholder="100"
                          class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                          :class="errors[`ticketTypes[${idx}].publishedQuota`] ? 'border-danger/60' : ''" />
                        <p v-if="errors[`ticketTypes[${idx}].publishedQuota`]" class="text-[11px] text-danger flex items-center gap-1"><PhWarningCircle weight="fill" class="flex-shrink-0 text-[10px]" />{{ errors[`ticketTypes[${idx}].publishedQuota`] }}</p>
                      </div>
                      <div class="flex flex-col gap-2">
                        <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Tối thiểu / đơn</label>
                        <input v-model.number="ticket.minQtyQuota" type="number" min="1" placeholder="1"
                          class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none" />
                      </div>
                      <div class="flex flex-col gap-2">
                        <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Tối đa / đơn</label>
                        <input v-model.number="ticket.maxQtyQuota" type="number" min="1" placeholder="5"
                          class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none" />
                      </div>
                      <div class="flex flex-col gap-2 md:col-span-2">
                        <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Mô tả (tuỳ chọn)</label>
                        <input v-model="ticket.description" type="text" placeholder="VD: Bao gồm ghế ngồi..."
                          class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] text-white/80 placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none" />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Manual mode: thêm/xoá loại vé tự do -->
          <div v-else class="flex flex-col gap-4">
            <div
              v-for="(ticket, idx) in form.ticketTypes"
              :key="idx"
              class="relative group p-5 rounded-2xl bg-[#0D1410] border border-white/8 hover:border-white/15 transition-all"
            >
              <button
                v-if="form.ticketTypes.length > 1"
                type="button"
                @click="removeTicket(idx)"
                class="absolute -right-2 -top-2 w-7 h-7 rounded-full bg-danger text-white flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity shadow-lg hover:scale-110 cursor-pointer"
              >
                <PhX weight="bold" class="text-[11px]" />
              </button>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
                <div class="flex flex-col gap-2 md:col-span-2">
                  <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Tên loại vé *</label>
                  <input
                    v-model="ticket.ticketTypeName"
                    type="text"
                    :id="`ticket-name-manual-${idx}`"
                    placeholder="VD: Vé VIP, Vé Thường, Early Bird..."
                    class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                    :class="errors[`ticketTypes[${idx}].ticketTypeName`] ? 'border-danger/60' : ''"
                  />
                  <p v-if="errors[`ticketTypes[${idx}].ticketTypeName`]" class="text-[11px] text-danger flex items-center gap-1">
                    <PhWarningCircle weight="fill" class="flex-shrink-0 text-[10px]" />{{ errors[`ticketTypes[${idx}].ticketTypeName`] }}
                  </p>
                </div>

                <div class="flex flex-col gap-2">
                  <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Giá vé (VND) *</label>
                  <div class="relative">
                    <span class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 font-bold text-[14px]">₫</span>
                    <input
                      v-model.number="ticket.price"
                      type="number"
                      min="0"
                      :id="`ticket-price-manual-${idx}`"
                      placeholder="500000"
                      class="w-full bg-white/4 border border-white/10 rounded-xl pl-8 pr-4 py-2.5 text-[14px] font-bold text-primary placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                      :class="errors[`ticketTypes[${idx}].price`] ? 'border-danger/60' : ''"
                    />
                  </div>
                  <p v-if="errors[`ticketTypes[${idx}].price`]" class="text-[11px] text-danger flex items-center gap-1">
                    <PhWarningCircle weight="fill" class="flex-shrink-0 text-[10px]" />{{ errors[`ticketTypes[${idx}].price`] }}
                  </p>
                </div>

                <div class="flex flex-col gap-2">
                  <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Số lượng phát hành *</label>
                  <input
                    v-model.number="ticket.publishedQuota"
                    type="number"
                    min="1"
                    :id="`ticket-quota-manual-${idx}`"
                    placeholder="100"
                    class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                    :class="errors[`ticketTypes[${idx}].publishedQuota`] ? 'border-danger/60' : ''"
                  />
                  <p v-if="errors[`ticketTypes[${idx}].publishedQuota`]" class="text-[11px] text-danger flex items-center gap-1">
                    <PhWarningCircle weight="fill" class="flex-shrink-0 text-[10px]" />{{ errors[`ticketTypes[${idx}].publishedQuota`] }}
                  </p>
                </div>

                <div class="flex flex-col gap-2">
                  <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Tối thiểu / đơn</label>
                  <input v-model.number="ticket.minQtyQuota" type="number" min="1" :id="`ticket-min-manual-${idx}`" placeholder="1"
                    class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none" />
                </div>
                <div class="flex flex-col gap-2">
                  <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Tối đa / đơn</label>
                  <input v-model.number="ticket.maxQtyQuota" type="number" min="1" :id="`ticket-max-manual-${idx}`" placeholder="5"
                    class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] font-bold text-white placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none" />
                </div>

                <div class="flex flex-col gap-2 md:col-span-2">
                  <label class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Mô tả (tuỳ chọn)</label>
                  <input
                    v-model="ticket.description"
                    type="text"
                    :id="`ticket-desc-manual-${idx}`"
                    placeholder="Quyền lợi, ưu đãi đặc biệt..."
                    class="w-full bg-white/4 border border-white/10 rounded-xl px-4 py-2.5 text-[14px] text-white/80 placeholder-white/20 focus:border-primary/50 focus:bg-white/6 transition-all focus:outline-none"
                  />
                </div>
              </div>
            </div>

            <!-- Add ticket -->
            <button
              type="button"
              @click="addTicket"
              class="flex items-center justify-center gap-2 py-4 rounded-2xl border-2 border-dashed border-white/15 text-white/50 hover:border-primary/40 hover:text-primary hover:bg-primary/[0.03] transition-all text-[14px] font-bold cursor-pointer"
            >
              <PhPlus weight="bold" />
              Thêm loại vé mới
            </button>
          </div>

          <div class="flex items-center justify-between pt-4 border-t border-white/5">
            <button @click="prevStep" class="inline-flex items-center gap-2 px-5 py-3 rounded-xl text-[14px] font-bold text-white/60 bg-white/5 hover:bg-white/10 hover:text-white transition-all cursor-pointer">
              <PhArrowLeft weight="bold" /> Quay lại
            </button>
            <button @click="nextStep" class="inline-flex items-center gap-2 px-7 py-3 rounded-xl text-[14px] font-bold text-black bg-primary hover:scale-[1.02] active:scale-95 transition-transform cursor-pointer">
              Xem lại & Xác nhận <PhArrowRight weight="bold" />
            </button>
          </div>
        </div>
      </div>

      <!-- ── STEP 5: Xem lại & Submit ───────────────────────────────── -->
      <div v-else-if="currentStep === 5" key="step5">
        <div class="flex flex-col gap-6">

          <!-- Preview card -->
          <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] overflow-hidden">
            <!-- Cover image preview -->
            <div v-if="form.coverImagePreview" class="relative" style="aspect-ratio: 16/9">
              <img :src="form.coverImagePreview" alt="Cover" class="w-full h-full object-cover" />
              <div class="absolute inset-0 bg-gradient-to-t from-[#111916] via-transparent to-transparent" />
            </div>
            <div v-else class="flex items-center justify-center bg-white/[0.02] border-b border-white/5" style="aspect-ratio: 16/9">
              <div class="flex flex-col items-center gap-2 text-white/20">
                <PhImage class="text-5xl" weight="duotone" />
                <span class="text-[13px] font-medium">Chưa có ảnh bìa</span>
              </div>
            </div>

            <div class="p-7 md:p-9 flex flex-col gap-6">
              <div>
                <p class="text-[11px] font-bold uppercase tracking-widest text-white/30 mb-2">{{ selectedCategoryName }}</p>
                <h2 class="font-heading text-2xl font-black text-white mb-3 leading-tight">{{ form.title || 'Chưa có tiêu đề' }}</h2>
                <div
                  class="text-[14px] text-white/60 leading-relaxed prose-custom line-clamp-3"
                  v-html="form.description || '<span class=\'text-white/25\'>Chưa có mô tả</span>'"
                />
              </div>

              <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div class="flex flex-col gap-1">
                  <span class="text-[11px] text-white/30 font-bold uppercase tracking-widest">Bắt đầu</span>
                  <span class="text-[13px] font-bold text-white">{{ formatDateTimeDisplay(form.startAt) }}</span>
                </div>
                <div class="flex flex-col gap-1">
                  <span class="text-[11px] text-white/30 font-bold uppercase tracking-widest">Kết thúc</span>
                  <span class="text-[13px] font-bold text-white">{{ formatDateTimeDisplay(form.endAt) }}</span>
                </div>
                <div class="flex flex-col gap-1">
                  <span class="text-[11px] text-white/30 font-bold uppercase tracking-widest">Mở bán</span>
                  <span class="text-[13px] font-bold text-primary">{{ formatDateTimeDisplay(form.saleOpenAt) }}</span>
                </div>
                <div class="flex flex-col gap-1">
                  <span class="text-[11px] text-white/30 font-bold uppercase tracking-widest">Đóng bán</span>
                  <span class="text-[13px] font-bold text-white/60">{{ formatDateTimeDisplay(form.saleCloseAt) }}</span>
                </div>
              </div>

              <!-- Location preview -->
              <div v-if="form.mode === 'manual'" class="flex items-start gap-3 p-4 rounded-xl bg-white/[0.03] border border-white/8">
                <PhMapPin weight="fill" class="text-primary flex-shrink-0 mt-0.5" />
                <div>
                  <p class="font-bold text-white text-[14px]">{{ form.location.venueName }}</p>
                  <p class="text-white/40 text-[13px]">{{ form.location.addressLine }}, {{ form.location.ward }}, {{ form.location.district }}, {{ form.location.provinceCity }}</p>
                </div>
              </div>
              <div v-else class="flex items-start gap-3 p-4 rounded-xl bg-[#818cf8]/[0.05] border border-[#818cf8]/20">
                <PhSquaresFour weight="fill" class="text-[#818cf8] flex-shrink-0 mt-0.5" />
                <div>
                  <p class="font-bold text-white text-[14px]">{{ selectedSeatMapName }}</p>
                  <p class="text-white/40 text-[13px]">{{ form.ticketTypes.length }} khu vực / loại vé</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Ticket types summary -->
          <div class="bg-[#111916]/60 border border-white/8 rounded-[2rem] p-7 md:p-9">
            <h3 class="font-heading text-[17px] font-black text-white mb-5">Loại vé ({{ form.ticketTypes.length }})</h3>
            <div class="flex flex-col gap-3">
              <div
                v-for="(ticket, idx) in form.ticketTypes"
                :key="idx"
                class="flex items-center justify-between p-4 rounded-xl bg-white/[0.03] border border-white/8"
              >
                <div class="flex flex-col gap-0.5">
                  <span class="font-bold text-white text-[14px]">{{ ticket.ticketTypeName || `Loại vé ${idx + 1}` }}</span>
                  <span class="text-white/30 text-[12px]">{{ ticket.publishedQuota || 0 }} vé</span>
                </div>
                <span class="font-heading font-black text-primary text-[17px]">
                  {{ ticket.price === 0 ? 'Miễn phí' : formatPrice(ticket.price) }}
                </span>
              </div>
            </div>
          </div>

          <!-- API error -->
          <div v-if="apiError" class="px-5 py-4 rounded-2xl bg-danger/10 border border-danger/25 text-danger text-[13px] font-medium flex items-start gap-3">
            <PhWarningCircle weight="fill" class="w-5 h-5 flex-shrink-0 mt-0.5" />
            <span>{{ apiError }}</span>
          </div>

          <!-- Actions -->
          <div class="flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-4 pt-2">
            <button
              @click="prevStep"
              :disabled="isSubmitting"
              class="inline-flex items-center justify-center gap-2 px-5 py-3.5 rounded-xl text-[14px] font-bold text-white/60 bg-white/5 hover:bg-white/10 hover:text-white transition-all cursor-pointer disabled:opacity-50"
            >
              <PhArrowLeft weight="bold" /> Chỉnh sửa lại
            </button>
            <button
              @click="handleSubmit"
              :disabled="isSubmitting"
              class="inline-flex items-center justify-center gap-2 px-8 py-3.5 rounded-xl text-[15px] font-black text-black bg-primary hover:scale-[1.02] active:scale-95 transition-transform shadow-[0_0_30px_rgba(0,200,83,0.25)] hover:shadow-[0_0_50px_rgba(0,200,83,0.4)] cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
            >
              <PhCircleNotch v-if="isSubmitting" weight="bold" class="animate-spin" />
              <PhRocketLaunch v-else weight="fill" />
              {{ isSubmitting ? 'Đang đăng sự kiện...' : 'Đăng sự kiện' }}
            </button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, nextTick, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { Stage as VStage, Layer as VLayer, Rect as VRect, Path as VPath, Text as VText, Circle as VCircle } from 'vue-konva'
import {
  PhArrowLeft, PhArrowRight, PhCheck, PhLightbulb, PhMapPin, PhSquaresFour,
  PhNotePencil, PhCalendarBlank, PhTicket, PhTag, PhImage, PhUpload,
  PhWarningCircle, PhMagnifyingGlass, PhBuildings, PhCircleNotch,
  PhX, PhPlus, PhRocketLaunch, PhLink
} from '@phosphor-icons/vue'
import { getEventCategories, uploadCoverImage, createEventWithLocation, createEventWithSeatMap, getSeatMapZones } from '../../services/organizer.service'
import { getOrganizerVenues, getOrganizerVenueSeatMaps, getOrganizerSeatMapDetail } from '../../services/venue.service'
import { getProvinces, getDistricts, getWards } from '../../services/location.service'

const router = useRouter()

// ── Steps ────────────────────────────────────────────────────────────────────
const steps = [
  { key: 'mode', label: 'Loại địa điểm' },
  { key: 'info', label: 'Thông tin' },
  { key: 'cover', label: 'Ảnh bìa' },
  { key: 'location', label: 'Địa điểm' },
  { key: 'tickets', label: 'Loại vé' },
  { key: 'review', label: 'Xác nhận' },
]
const currentStep = ref(0)
const maxReachedStep = ref(0)

function goToStep(idx) {
  if (idx <= maxReachedStep.value) currentStep.value = idx
}
function nextStep() {
  if (!validateCurrentStep()) return
  if (currentStep.value < steps.length - 1) {
    currentStep.value++
    if (currentStep.value > maxReachedStep.value) maxReachedStep.value = currentStep.value
    // Load zones when reaching ticket step in seatmap mode
    if (currentStep.value === 4 && form.mode === 'seatmap' && form.seatMapId) {
      loadZones()
    }
  }
}
function prevStep() {
  if (currentStep.value > 0) currentStep.value--
}

// ── Form state ────────────────────────────────────────────────────────────────
const form = reactive({
  mode: '',       // 'manual' | 'seatmap'
  title: '',
  categoryId: '',
  description: '',
  showTimes: [
    { startAt: '', endAt: '' }
  ],
  saleOpenAt: '',
  saleCloseAt: '',
  coverImageUrl: '',   // pathname - gửi lên API
  coverImagePreview: '', // full URL - dùng để preview trong browser
  // Mode: manual
  location: {
    venueName: '',
    addressLine: '',
    ward: '',
    district: '',
    provinceCity: '',
    country: 'Việt Nam',
  },
  // Mode: seatmap
  selectedVenueId: '',
  seatMapId: '',
  ticketTypes: []
})

const errors = ref({})
const apiError = ref('')
const isSubmitting = ref(false)

// ── Categories ─────────────────────────────────────────────────────────────
const categories = ref([])
const isLoadingCategories = ref(false)

const selectedCategoryName = computed(() => {
  if (!form.categoryId) return 'Chưa chọn danh mục'
  return categories.value.find(c => c.id === form.categoryId)?.categoryName || ''
})

async function loadCategories() {
  isLoadingCategories.value = true
  try {
    const res = await getEventCategories({ PageSize: 100 })
    if (res.success && res.data) {
      categories.value = res.data.data || []
    }
  } catch (e) {
    console.error('Không thể tải danh mục:', e)
  } finally {
    isLoadingCategories.value = false
  }
}

// ── Image upload ──────────────────────────────────────────────────────────
const fileInputRef = ref(null)
const isDragging = ref(false)
const isUploadingImage = ref(false)
const uploadProgress = ref(0)
const uploadError = ref('')

function triggerFileInput() {
  if (!isUploadingImage.value) fileInputRef.value?.click()
}

function removeImage() {
  form.coverImageUrl = ''
  form.coverImagePreview = ''
  uploadError.value = ''
}

async function processImageFile(file) {
  if (!file || !file.type.startsWith('image/')) {
    uploadError.value = 'Vui lòng chọn file ảnh hợp lệ (PNG, JPG, WEBP).'
    return
  }
  uploadError.value = ''
  isUploadingImage.value = true
  uploadProgress.value = 0

  // Fake progress bar
  const progressInterval = setInterval(() => {
    if (uploadProgress.value < 85) uploadProgress.value += Math.random() * 15
  }, 200)

  try {
    const res = await uploadCoverImage(file)
    clearInterval(progressInterval)
    uploadProgress.value = 100
    if (res.success && res.data) {
      // BE trả về full URL (vd: https://localhost:7170/uploads/...)
      // Lưu full URL để preview ảnh, chỉ lưu pathname để gửi API
      const rawUrl = res.data?.url || res.data
      form.coverImagePreview = rawUrl
      try {
        form.coverImageUrl = new URL(rawUrl).pathname
      } catch {
        form.coverImageUrl = rawUrl
      }
    } else {
      uploadError.value = res.message || 'Tải ảnh lên thất bại.'
    }
  } catch (error) {
    clearInterval(progressInterval)
    const errData = error.response?.data
    if (errData?.success === false && errData.errors?.length) {
      uploadError.value = errData.errors.map(e => e.message).join(', ')
    } else {
      uploadError.value = errData?.message || 'Lỗi kết nối khi tải ảnh lên.'
    }
  } finally {
    isUploadingImage.value = false
    setTimeout(() => { uploadProgress.value = 0 }, 600)
  }
}

function handleFileSelect(e) {
  processImageFile(e.target.files[0])
}

function handleFileDrop(e) {
  isDragging.value = false
  processImageFile(e.dataTransfer.files[0])
}

// ── Rich text editor ──────────────────────────────────────────────────────
const editorRef = ref(null)

const editorTools = [
  { cmd: 'bold',        label: 'B' },
  { cmd: 'italic',      label: 'I' },
  { cmd: 'insertUnorderedList', label: '•' },
  { cmd: 'insertOrderedList',   label: '1.' },
  { cmd: 'createLink',  label: 'Link', icon: PhLink },
]

function execCommand(cmd, val = null) {
  if (cmd === 'createLink') {
    const url = prompt('Nhập URL:')
    if (url) document.execCommand('createLink', false, url)
  } else {
    document.execCommand(cmd, false, val)
  }
  editorRef.value?.focus()
}

function onDescriptionInput() {
  form.description = editorRef.value?.innerHTML || ''
}

// ── Location (manual mode) ────────────────────────────────────────────────
const provinces = ref([])
const districts = ref([])
const wards = ref([])
const isLoadingProvinces = ref(false)
const isLoadingDistricts = ref(false)
const isLoadingWards = ref(false)
const selectedProvinceCode = ref(null)
const selectedDistrictCode = ref(null)

async function loadProvinces() {
  isLoadingProvinces.value = true
  try { provinces.value = await getProvinces() }
  catch (e) { console.error(e) }
  finally { isLoadingProvinces.value = false }
}

async function onProvinceChange() {
  form.location.district = ''
  form.location.ward = ''
  selectedProvinceCode.value = null
  districts.value = []
  wards.value = []
  const prov = provinces.value.find(p => p.name === form.location.provinceCity)
  if (prov) {
    selectedProvinceCode.value = prov.code
    isLoadingDistricts.value = true
    try { districts.value = await getDistricts(prov.code) }
    catch (e) { console.error(e) }
    finally { isLoadingDistricts.value = false }
  }
}

async function onDistrictChange() {
  form.location.ward = ''
  selectedDistrictCode.value = null
  wards.value = []
  const dist = districts.value.find(d => d.name === form.location.district)
  if (dist) {
    selectedDistrictCode.value = dist.code
    isLoadingWards.value = true
    try { wards.value = await getWards(dist.code) }
    catch (e) { console.error(e) }
    finally { isLoadingWards.value = false }
  }
}

// ── Venues (seatmap mode) ──────────────────────────────────────────────────
const venues = ref([])
const seatMaps = ref([])
const isLoadingVenues = ref(false)
const isLoadingSeatMaps = ref(false)
const venueSearch = ref('')
const venueTotalPages = ref(0)
const venueHasNext = ref(false)
const venueHasPrev = ref(false)
let venueSearchTimeout = null

const venueFilters = reactive({ PageNumber: 1, PageSize: 6, Search: '' })

async function loadVenues() {
  isLoadingVenues.value = true
  try {
    const res = await getOrganizerVenues(venueFilters)
    if (res.success && res.data) {
      venues.value = res.data.data || []
      venueTotalPages.value = res.data.totalPages || 1
      venueHasNext.value = res.data.hasNextPage || false
      venueHasPrev.value = res.data.hasPreviousPage || false
    }
  } catch (e) {
    console.error(e)
  } finally {
    isLoadingVenues.value = false
  }
}

function onVenueSearch() {
  clearTimeout(venueSearchTimeout)
  venueSearchTimeout = setTimeout(() => {
    venueFilters.Search = venueSearch.value
    venueFilters.PageNumber = 1
    loadVenues()
  }, 400)
}

function venueNextPage() {
  if (venueHasNext.value) { venueFilters.PageNumber++; loadVenues() }
}
function venuePrevPage() {
  if (venueHasPrev.value) { venueFilters.PageNumber--; loadVenues() }
}

async function selectVenue(venue) {
  form.selectedVenueId = venue.id
  form.seatMapId = ''
  seatMaps.value = []
  form.ticketTypes = []
  // Load seatmaps of this venue
  isLoadingSeatMaps.value = true
  try {
    const res = await getOrganizerVenueSeatMaps(venue.id, { PageNumber: 1, PageSize: 50 })
    if (res.success && res.data) {
      seatMaps.value = res.data.data || []
    }
  } catch (e) {
    console.error(e)
  } finally {
    isLoadingSeatMaps.value = false
  }
}

const selectedSeatMapName = computed(() => {
  return seatMaps.value.find(s => s.id === form.seatMapId)?.seatMapName || ''
})

async function selectSeatMap(sm) {
  form.seatMapId = sm.id
  form.ticketTypes = []
}

// ── Zones (seatmap mode, step 4) ──────────────────────────────────────────
const availableZones = ref([])
const isLoadingZones = ref(false)
const seatMapData = ref(null)
const konvaContainer = ref(null)

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
      // Organizer seatMapData might not have `rows` returned, so we just check svgElements
    })
    maxX += 100
    maxY += 100
  }
  return { width: maxX, height: maxY }
})

const stageConfig = reactive({
  width: 400,
  height: 300,
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

function buildSeatConfig(seat, zone) {
  return {
    x: seat.x, y: seat.y, radius: seat.radius || 10,
    fill: '#0b0f19', stroke: zone.color, strokeWidth: 2,
    id: seat.svgElementId, listening: false
  }
}

function isZoneSelected(zoneId) {
  return form.ticketTypes.some(t => t._zoneId === zoneId)
}

function toggleZone(zone) {
  const existingIdx = form.ticketTypes.findIndex(t => t._zoneId === zone.id)
  if (existingIdx !== -1) {
    form.ticketTypes.splice(existingIdx, 1)
  } else {
    form.ticketTypes.push({
      zoneId: zone.id, _zoneId: zone.id, _zoneName: zone.zoneName, _zoneColor: zone.color,
      ticketTypeName: zone.zoneName, description: '', price: zone.basePrice || 0,
      publishedQuota: zone.capacity || 0, minQtyQuota: 1, maxQtyQuota: 5,
      displayOrder: form.ticketTypes.length + 1
    })
  }
}

function removeZoneTicket(zoneId) {
  const existingIdx = form.ticketTypes.findIndex(t => t._zoneId === zoneId)
  if (existingIdx !== -1) form.ticketTypes.splice(existingIdx, 1)
}

async function loadZones() {
  if (!form.selectedVenueId || !form.seatMapId) return
  isLoadingZones.value = true
  try {
    const res = await getOrganizerSeatMapDetail(form.selectedVenueId, form.seatMapId)
    if (res.success && res.data) {
      seatMapData.value = res.data
      availableZones.value = (res.data.zones || []).filter(z => z.isSalable && !z.isStage)
      
      // Chỉ init lần đầu
      if (form.ticketTypes.length === 0) {
        form.ticketTypes = availableZones.value.map((z, idx) => ({
          zoneId: z.id, _zoneId: z.id, _zoneName: z.zoneName, _zoneColor: z.color,
          ticketTypeName: z.zoneName, description: '', price: z.basePrice || 0,
          publishedQuota: z.capacity || 0, minQtyQuota: 1, maxQtyQuota: 5,
          displayOrder: idx + 1
        }))
      }

      hasInitializedCenter = false
      await nextTick()
      
      if (konvaContainer.value) {
        if (resizeObserver) resizeObserver.disconnect()
        
        resizeObserver = new ResizeObserver((entries) => {
          const entry = entries[0]
          const containerW = entry.contentRect.width
          const containerH = entry.contentRect.height || 300
          
          if (containerW > 0) {
            stageConfig.width = containerW
            stageConfig.height = containerH
            
            if (!hasInitializedCenter && seatMapData.value) {
              const svgW = realDimensions.value.width || 800
              const svgH = realDimensions.value.height || 600
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
    }
  } catch (e) {
    console.error(e)
  } finally {
    isLoadingZones.value = false
  }
}

onUnmounted(() => {
  if (resizeObserver) resizeObserver.disconnect()
})

// ── Tickets (manual mode) ──────────────────────────────────────────────────
function addTicket() {
  form.ticketTypes.push({
    ticketTypeName: '',
    description: '',
    price: 0,
    publishedQuota: 0,
    minQtyQuota: 1,
    maxQtyQuota: 5,
    displayOrder: form.ticketTypes.length + 1
  })
}

function removeTicket(idx) {
  form.ticketTypes.splice(idx, 1)
}

// ── ShowTimes ──────────────────────────────────────────────────────────────
function addShowTime() {
  form.showTimes.push({ startAt: '', endAt: '' })
}

function removeShowTime(idx) {
  form.showTimes.splice(idx, 1)
}

// ── Mode select ────────────────────────────────────────────────────────────
function selectMode(mode) {
  form.mode = mode
  // Reset mode-specific data
  if (mode === 'manual') {
    form.selectedVenueId = ''
    form.seatMapId = ''
    if (form.ticketTypes.length === 0) addTicket()
  } else {
    form.location = { venueName: '', addressLine: '', ward: '', district: '', provinceCity: '', country: 'Việt Nam' }
    form.ticketTypes = []
  }
}

// ── Validation ────────────────────────────────────────────────────────────
function validateCurrentStep() {
  errors.value = {}

  if (currentStep.value === 0) {
    if (!form.mode) { errors.value.mode = 'Vui lòng chọn loại địa điểm.'; return false }
  }

  if (currentStep.value === 1) {
    if (!form.title.trim()) errors.value.title = 'Tên sự kiện không được để trống.'
    if (!form.categoryId) errors.value.categoryId = 'Vui lòng chọn danh mục.'
    if (form.showTimes.length === 0) {
      errors.value.showTimes = 'Vui lòng thêm ít nhất một suất chiếu.'
    } else {
      let hasShowTimeErr = false
      let latestEnd = null
      form.showTimes.forEach((st, idx) => {
        if (!st.startAt) { errors.value[`showTimes[${idx}].startAt`] = 'Bắt buộc'; hasShowTimeErr = true }
        if (!st.endAt) { errors.value[`showTimes[${idx}].endAt`] = 'Bắt buộc'; hasShowTimeErr = true }
        if (st.startAt && st.endAt && new Date(st.endAt) <= new Date(st.startAt)) {
          errors.value[`showTimes[${idx}].endAt`] = 'Phải sau bắt đầu'
          hasShowTimeErr = true
        }
        if (st.endAt) {
          const stDate = new Date(st.endAt)
          if (!latestEnd || stDate > latestEnd) latestEnd = stDate
        }
      })
      if (!hasShowTimeErr && form.saleCloseAt && latestEnd && new Date(form.saleCloseAt) >= latestEnd) {
        errors.value.saleCloseAt = 'Đóng bán vé phải trước thời gian kết thúc của sự kiện (suất chiếu muộn nhất).'
      }
    }
    if (!form.saleOpenAt) errors.value.saleOpenAt = 'Vui lòng chọn thời gian mở bán.'
    if (!form.saleCloseAt) errors.value.saleCloseAt = 'Vui lòng chọn thời gian đóng bán.'
    if (form.saleOpenAt && form.saleCloseAt && new Date(form.saleCloseAt) <= new Date(form.saleOpenAt)) {
      errors.value.saleCloseAt = 'Thời gian đóng bán phải sau thời gian mở bán.'
    }
    if (Object.keys(errors.value).length > 0) return false
  }

  // Step 2: cover image - optional (no required)

  if (currentStep.value === 3) {
    if (form.mode === 'manual') {
      if (!form.location.venueName.trim()) errors.value['location.venueName'] = 'Tên địa điểm không được để trống.'
      if (!form.location.provinceCity) errors.value['location.provinceCity'] = 'Vui lòng chọn Tỉnh / Thành phố.'
      if (!form.location.district) errors.value['location.district'] = 'Vui lòng chọn Quận / Huyện.'
      if (!form.location.ward) errors.value['location.ward'] = 'Vui lòng chọn Phường / Xã.'
      if (!form.location.addressLine.trim()) errors.value['location.addressLine'] = 'Số nhà, Tên đường không được để trống.'
      if (Object.keys(errors.value).length > 0) return false
    } else {
      if (!form.selectedVenueId) { errors.value.selectedVenueId = 'Vui lòng chọn một địa điểm.'; return false }
      if (!form.seatMapId) { errors.value.seatMapId = 'Vui lòng chọn sơ đồ ghế.'; return false }
    }
  }

  if (currentStep.value === 4) {
    let hasErr = false
    form.ticketTypes.forEach((t, idx) => {
      if (!t.ticketTypeName.trim()) {
        errors.value[`ticketTypes[${idx}].ticketTypeName`] = 'Tên loại vé không được để trống.'
        hasErr = true
      }
      if (t.price === null || t.price === undefined || t.price === '' || isNaN(t.price) || t.price < 0) {
        errors.value[`ticketTypes[${idx}].price`] = 'Giá vé không hợp lệ.'
        hasErr = true
      }
      if (!t.publishedQuota || t.publishedQuota < 1) {
        errors.value[`ticketTypes[${idx}].publishedQuota`] = 'Số lượng phải lớn hơn 0.'
        hasErr = true
      }
    })
    if (form.ticketTypes.length === 0) {
      apiError.value = 'Vui lòng thêm ít nhất một loại vé.'
      hasErr = true
    }
    if (hasErr) return false
  }

  return true
}

// ── Submit ─────────────────────────────────────────────────────────────────
async function handleSubmit() {
  apiError.value = ''
  isSubmitting.value = true

  try {
    let payload
    const commonFields = {
      categoryId: form.categoryId,
      title: form.title,
      description: form.description,
      showTimes: form.showTimes.map(st => ({
        startAt: new Date(st.startAt).toISOString(),
        endAt: new Date(st.endAt).toISOString(),
      })),
      saleOpenAt: new Date(form.saleOpenAt).toISOString(),
      saleCloseAt: new Date(form.saleCloseAt).toISOString(),
      coverImageUrl: form.coverImageUrl || null,
    }

    const ticketTypesPayload = form.ticketTypes.map((t, idx) => ({
      ...(form.mode === 'seatmap' ? { zoneId: t.zoneId } : {}),
      ticketTypeName: t.ticketTypeName,
      description: t.description,
      price: t.price,
      publishedQuota: t.publishedQuota,
      minQtyQuota: t.minQtyQuota || 1,
      maxQtyQuota: t.maxQtyQuota || 5,
      displayOrder: t.displayOrder || idx + 1
    }))

    if (form.mode === 'manual') {
      payload = {
        ...commonFields,
        ticketTypes: ticketTypesPayload,
        location: {
          venueName: form.location.venueName,
          addressLine: form.location.addressLine,
          ward: form.location.ward,
          district: form.location.district,
          provinceCity: form.location.provinceCity,
          country: 'Việt Nam'
        }
      }
      await createEventWithLocation(payload)
    } else {
      payload = {
        ...commonFields,
        seatMapId: form.seatMapId,
        ticketTypes: ticketTypesPayload,
      }
      await createEventWithSeatMap(payload)
    }

    router.push('/organizer')
  } catch (error) {
    const errData = error.response?.data
    if (errData?.success === false && errData.errors?.length) {
      // Map field errors
      errData.errors.forEach(err => {
        errors.value[err.field] = err.message
      })
      apiError.value = errData.message || 'Dữ liệu không hợp lệ, vui lòng kiểm tra lại.'
    } else {
      apiError.value = errData?.message || 'Đã có lỗi xảy ra. Vui lòng thử lại.'
    }
  } finally {
    isSubmitting.value = false
  }
}

// ── Helpers ────────────────────────────────────────────────────────────────
function formatDateTimeDisplay(val) {
  if (!val) return '--'
  return new Date(val).toLocaleString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

function formatPrice(val) {
  if (!val && val !== 0) return ''
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

// ── Mount ──────────────────────────────────────────────────────────────────
onMounted(async () => {
  await loadCategories()
  await loadProvinces()
  await loadVenues()
  // Add initial ticket for manual mode
  if (form.mode === 'manual' && form.ticketTypes.length === 0) addTicket()
})
</script>

<style scoped>
.step-fade-enter-active,
.step-fade-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.step-fade-enter-from {
  opacity: 0;
  transform: translateX(16px);
}
.step-fade-leave-to {
  opacity: 0;
  transform: translateX(-16px);
}

/* Rich text editor styles */
.prose-custom :deep(h2) {
  font-size: 1.2rem;
  font-weight: 800;
  color: #fff;
  margin: 0.75rem 0 0.4rem;
  font-family: var(--font-heading, inherit);
}
.prose-custom :deep(h3) {
  font-size: 1.05rem;
  font-weight: 700;
  color: rgba(255,255,255,.8);
  margin: 0.6rem 0 0.3rem;
}
.prose-custom :deep(p) {
  margin: 0.3rem 0;
}
.prose-custom :deep(ul) {
  list-style: disc;
  padding-left: 1.2rem;
  margin: 0.4rem 0;
}
.prose-custom :deep(ol) {
  list-style: decimal;
  padding-left: 1.2rem;
  margin: 0.4rem 0;
}
.prose-custom :deep(a) {
  color: #00c853;
  text-decoration: underline;
}
.prose-custom:empty:before {
  content: attr(data-placeholder);
  color: rgba(255,255,255,.2);
  pointer-events: none;
}
</style>
