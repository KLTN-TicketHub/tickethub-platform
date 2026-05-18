<template>
  <div 
    v-if="isOpen" 
    class="fixed inset-0 z-[1000] flex items-center justify-center p-4 sm:p-6"
  >
    <!-- Backdrop -->
    <div 
      class="absolute inset-0 bg-black/80 backdrop-blur-sm animate-in fade-in duration-300"
      @click="close"
    ></div>

    <!-- Modal Content -->
    <div 
      class="relative bg-card/90 backdrop-blur-2xl border border-border-main rounded-[32px] w-full max-w-2xl max-h-[90vh] flex flex-col overflow-hidden shadow-2xl shadow-black/60 animate-in zoom-in-95 fade-in duration-300"
    >
      <!-- Header -->
      <div class="px-8 py-6 border-bottom border-border-main flex justify-between items-center bg-surface/30">
        <h2 class="text-xl font-bold text-main font-heading">
          {{ eventData.id ? 'Chỉnh sửa sự kiện' : 'Tạo sự kiện mới' }}
        </h2>
        <button 
          @click="close"
          class="w-10 h-10 rounded-full flex items-center justify-center text-muted hover:text-main hover:bg-surface/50 transition-all"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M18 6L6 18M6 6l12 12"/></svg>
        </button>
      </div>

      <!-- Body -->
      <div class="p-8 overflow-y-auto custom-scrollbar flex flex-col gap-8">
        <!-- Basic Info Section -->
        <div class="flex flex-col gap-5">
          <div class="flex items-center gap-2 text-primary">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
            <h3 class="text-[11px] font-bold uppercase tracking-widest">Thông tin cơ bản</h3>
          </div>
          
          <div class="space-y-4">
            <div class="flex flex-col gap-1.5">
              <label class="text-[13px] font-bold text-muted ml-1">Tiêu đề sự kiện</label>
              <input 
                type="text" 
                v-model="formData.title" 
                placeholder="Nhập tên sự kiện..."
                class="w-full bg-surface/50 border border-border-main rounded-xl px-4 py-3 text-main focus:outline-none focus:border-primary/50 focus:ring-4 focus:ring-primary/5 transition-all placeholder:text-muted/50"
              />
            </div>
            
            <div class="flex flex-col gap-1.5">
              <label class="text-[13px] font-bold text-muted ml-1">Mô tả</label>
              <textarea 
                v-model="formData.description" 
                placeholder="Mô tả chi tiết về sự kiện..."
                rows="4"
                class="w-full bg-surface/50 border border-border-main rounded-xl px-4 py-3 text-main focus:outline-none focus:border-primary/50 focus:ring-4 focus:ring-primary/5 transition-all placeholder:text-muted/50 resize-none"
              ></textarea>
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div class="flex flex-col gap-1.5">
                <label class="text-[13px] font-bold text-muted ml-1">Thể loại</label>
                <select 
                  v-model="formData.category"
                  class="w-full bg-surface/50 border border-border-main rounded-xl px-4 py-3 text-main focus:outline-none focus:border-primary/50 transition-all appearance-none cursor-pointer"
                >
                  <option value="concerts">Concerts</option>
                  <option value="arts">Sân khấu & Nghệ thuật</option>
                  <option value="sports">Thể thao</option>
                  <option value="experiences">Trải nghiệm</option>
                  <option value="workshops">Workshops</option>
                </select>
              </div>
              <div class="flex flex-col gap-1.5">
                <label class="text-[13px] font-bold text-muted ml-1">Trạng thái</label>
                <div class="px-4 py-3 bg-surface/20 border border-border-main/50 rounded-xl text-muted text-[14px] italic">
                  Tự động dựa trên thời gian
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Performers Section -->
        <div class="flex flex-col gap-5 p-6 bg-surface/20 rounded-2xl border border-border-main/50">
          <div class="flex justify-between items-center">
            <div class="flex items-center gap-2 text-primary">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2m12-11a4 4 0 11-8 0 4 4 0 018 0z"/></svg>
              <h3 class="text-[11px] font-bold uppercase tracking-widest">Nghệ sĩ & Diễn giả</h3>
            </div>
            <button 
              @click="addPerformer"
              class="text-[12px] font-bold text-primary hover:underline"
            >
              + Thêm người
            </button>
          </div>
          
          <div class="flex flex-col gap-3">
            <div 
              v-for="(perf, idx) in formData.performers" 
              :key="idx" 
              class="flex gap-3 animate-in fade-in slide-in-from-left-4 duration-300"
            >
              <input 
                type="text" 
                v-model="perf.name" 
                placeholder="Tên nghệ sĩ"
                class="flex-1 bg-surface/50 border border-border-main rounded-lg px-3 py-2 text-[13px] text-main focus:outline-none focus:border-primary/50 transition-all"
              />
              <input 
                type="text" 
                v-model="perf.role" 
                placeholder="Vai trò"
                class="w-32 bg-surface/50 border border-border-main rounded-lg px-3 py-2 text-[13px] text-main focus:outline-none focus:border-primary/50 transition-all"
              />
              <button 
                @click="removePerformer(idx)"
                class="w-10 h-10 rounded-lg flex items-center justify-center bg-danger/10 text-danger hover:bg-danger/20 transition-all"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M6 18L18 6M6 6l12 12"/></svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Location & Time Section -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-8">
          <div class="flex flex-col gap-5">
            <div class="flex items-center gap-2 text-primary">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/><path d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
              <h3 class="text-[11px] font-bold uppercase tracking-widest">Địa điểm</h3>
            </div>
            <div class="space-y-4">
              <input 
                type="text" 
                v-model="formData.location.name" 
                placeholder="Tên địa điểm (Vd: SVĐ Mỹ Đình)"
                class="w-full bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all"
              />
              <div class="grid grid-cols-2 gap-3">
                <input 
                  type="text" 
                  v-model="formData.location.city" 
                  placeholder="Thành phố"
                  class="bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all"
                />
                <input 
                  type="text" 
                  v-model="formData.location.address" 
                  placeholder="Địa chỉ cụ thể"
                  class="bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all"
                />
              </div>
            </div>
          </div>

          <div class="flex flex-col gap-5">
            <div class="flex items-center gap-2 text-primary">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
              <h3 class="text-[11px] font-bold uppercase tracking-widest">Thời gian</h3>
            </div>
            <div class="space-y-4">
              <div class="flex flex-col gap-1">
                <label class="text-[11px] font-bold text-muted">Ngày bắt đầu</label>
                <input 
                  type="date" 
                  v-model="formData.dateStart" 
                  class="w-full bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all invert"
                />
              </div>
              <div class="flex flex-col gap-1">
                <label class="text-[11px] font-bold text-muted">Ngày kết thúc (Không bắt buộc)</label>
                <input 
                  type="date" 
                  v-model="formData.dateEnd" 
                  class="w-full bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all invert"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Pricing & Media -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-8">
          <div class="flex flex-col gap-5">
            <div class="flex items-center gap-2 text-primary">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M12 8c-1.657 0-3 1.343-3 3s1.343 3 3 3 3-1.343 3-3-1.343-3-3-3z"/><path d="M2 17l10 5 10-5M2 12l10 5 10-5M12 2l10 5-10 5L2 7l10-5z"/></svg>
              <h3 class="text-[11px] font-bold uppercase tracking-widest">Giá vé (VND)</h3>
            </div>
            <div class="grid grid-cols-2 gap-3">
              <input 
                type="number" 
                v-model.number="formData.priceRange.min" 
                placeholder="Tối thiểu"
                class="bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all"
              />
              <input 
                type="number" 
                v-model.number="formData.priceRange.max" 
                placeholder="Tối đa"
                class="bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all"
              />
            </div>
          </div>

          <div class="flex flex-col gap-5">
            <div class="flex items-center gap-2 text-primary">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
              <h3 class="text-[11px] font-bold uppercase tracking-widest">Hình ảnh (URL)</h3>
            </div>
            <input 
              type="text" 
              v-model="formData.image" 
              placeholder="https://images.unsplash.com/..."
              class="w-full bg-surface/50 border border-border-main rounded-xl px-4 py-2.5 text-[14px] text-main focus:outline-none focus:border-primary/50 transition-all"
            />
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="px-8 py-6 border-top border-border-main flex justify-end gap-3 bg-surface/30">
        <BaseButton 
          variant="outline" 
          @click="close"
          class="!rounded-xl px-6"
        >
          Hủy bỏ
        </BaseButton>
        <BaseButton 
          variant="primary" 
          @click="save"
          class="!rounded-xl px-8 shadow-lg shadow-primary/20"
        >
          {{ eventData.id ? 'Cập nhật' : 'Đăng sự kiện' }}
        </BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import BaseButton from '../../shared/components/BaseButton.vue'

const props = defineProps({
  isOpen: Boolean,
  eventData: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['close', 'save'])

const defaultForm = () => ({
  id: '',
  title: '',
  description: '',
  category: 'concerts',
  status: 'upcoming',
  performers: [{ name: '', role: 'Artist' }],
  location: { name: '', address: '', city: 'hcm' },
  dateStart: new Date().toISOString().split('T')[0],
  dateEnd: '',
  priceRange: { min: 0, max: 0 },
  image: ''
})

const formData = ref(defaultForm())

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    if (props.eventData && Object.keys(props.eventData).length > 0) {
      formData.value = JSON.parse(JSON.stringify(props.eventData))
    } else {
      formData.value = defaultForm()
    }
  }
})

const addPerformer = () => {
  formData.value.performers.push({ name: '', role: '' })
}

const removePerformer = (idx) => {
  formData.value.performers.splice(idx, 1)
}

const close = () => {
  emit('close')
}

const save = () => {
  if (!formData.value.title || !formData.value.dateStart) {
    alert("Vui lòng nhập Tiêu đề và Ngày bắt đầu.")
    return
  }
  emit('save', JSON.parse(JSON.stringify(formData.value)))
}
</script>

<style>
.custom-scrollbar::-webkit-scrollbar {
  width: 5px;
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

