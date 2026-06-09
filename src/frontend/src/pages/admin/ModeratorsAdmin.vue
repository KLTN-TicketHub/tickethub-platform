<template>
  <div class="flex flex-col gap-8 animate-in fade-in slide-in-from-bottom-4 duration-500 pb-12">
    <!-- Header -->
    <div class="flex justify-between items-center">
      <div>
        <h1 class="font-heading text-3xl font-bold text-main mb-2">Kiểm duyệt viên</h1>
        <p class="text-muted font-medium italic">Quản lý và cấp tài khoản cho đội ngũ kiểm duyệt viên hệ thống.</p>
      </div>
      <button 
        @click="openCreateModal"
        class="flex items-center gap-2 px-5 py-3 rounded-2xl bg-primary text-surface font-bold hover:bg-primary-hover shadow-lg shadow-primary/25 hover:shadow-primary/40 hover:-translate-y-0.5 transition-all duration-200 cursor-pointer"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
        </svg>
        Tạo tài khoản mới
      </button>
    </div>

    <!-- Moderators List -->
    <div class="bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl flex flex-col">
      <div class="p-6 border-b border-border-main bg-card/50 flex items-center justify-between">
        <h3 class="text-lg font-bold text-main">Danh sách kiểm duyệt viên</h3>
        <span class="text-[12px] text-muted font-semibold">{{ moderatorsList.length }} người dùng</span>
      </div>

      <div v-if="moderatorsList.length === 0" class="p-16 flex flex-col items-center justify-center text-center gap-4">
        <div class="w-16 h-16 rounded-2xl bg-surface border border-border-main flex items-center justify-center text-3xl">
          👥
        </div>
        <div class="flex flex-col gap-1">
          <span class="font-bold text-main text-lg">Chưa có kiểm duyệt viên nào</span>
          <span class="text-sm text-muted">Bấm vào nút "Tạo tài khoản mới" phía trên để cấp quyền.</span>
        </div>
      </div>

      <div v-else class="divide-y divide-border-main/30">
        <div v-for="mod in moderatorsList" :key="mod.id" class="p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4 hover:bg-surface/30 transition-all group">
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-2xl overflow-hidden border border-border-main shadow-inner group-hover:border-primary/30 transition-all flex-shrink-0">
              <img :src="mod.imageUrl || 'https://ui-avatars.com/api/?name=' + encodeURIComponent(mod.fullName) + '&background=6366f1&color=fff'" alt="Avatar" class="w-full h-full object-cover" />
            </div>
            <div class="flex flex-col">
              <span class="font-bold text-main leading-snug group-hover:text-primary transition-colors">{{ mod.fullName }}</span>
              <span class="text-[12px] font-semibold text-muted">@{{ mod.userName }}</span>
            </div>
          </div>
          <div class="flex flex-wrap items-center gap-6 text-sm text-muted">
            <div class="flex items-center gap-2">
              <svg class="w-4 h-4 opacity-60" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75" />
              </svg>
              <span>{{ mod.email }}</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-4 h-4 opacity-60" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-2.824-1.802-5.122-4.1-6.924-6.924l1.293-.97a1.242 1.242 0 00.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z" />
              </svg>
              <span>{{ mod.phoneNumber }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-[12px] bg-indigo-500/10 text-indigo-400 font-bold px-2 py-0.5 rounded-md uppercase tracking-wider">Moderator</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Moderator Modal -->
    <div v-if="isModalOpen" class="fixed inset-0 z-[1000] flex items-center justify-center p-4">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-black/70 backdrop-blur-md" @click="closeModal"></div>
      
      <!-- Modal Content -->
      <div class="bg-card border border-border-main rounded-[28px] max-w-lg w-full p-8 shadow-2xl relative z-10 animate-in fade-in zoom-in-95 duration-200">
        <button @click="closeModal" class="absolute top-6 right-6 text-muted hover:text-main text-xl transition-colors cursor-pointer">✕</button>

        <h3 class="text-xl font-bold text-main mb-2">Tạo tài khoản Moderator</h3>
        <p class="text-sm text-muted mb-6">Mã kích hoạt tài khoản sẽ được hệ thống gửi tự động tới email của Moderator.</p>

        <!-- General Error -->
        <div v-if="validationErrors.general" class="p-4 rounded-xl bg-danger/10 border border-danger/20 text-danger text-sm font-semibold mb-6 flex gap-2">
          <span>⚠️</span>
          <span>{{ validationErrors.general }}</span>
        </div>

        <form @submit.prevent="handleSubmit" class="flex flex-col gap-5">
          <!-- Full Name -->
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-muted uppercase tracking-widest" for="mod-fullname">Họ và tên</label>
            <input 
              id="mod-fullname"
              type="text" 
              v-model="form.fullName" 
              placeholder="Nhập họ tên đầy đủ" 
              required
              :disabled="isSubmitting"
              class="w-full bg-surface border border-border-main/80 rounded-xl px-4 py-3.5 text-[14px] text-main outline-none focus:border-primary transition-all"
            />
            <span v-if="validationErrors.fullName" class="text-xs text-danger font-medium">{{ validationErrors.fullName }}</span>
          </div>

          <!-- Email -->
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-muted uppercase tracking-widest" for="mod-email">Địa chỉ Email</label>
            <input 
              id="mod-email"
              type="email" 
              v-model="form.email" 
              placeholder="mod@tickethub.vn" 
              required
              :disabled="isSubmitting"
              class="w-full bg-surface border border-border-main/80 rounded-xl px-4 py-3.5 text-[14px] text-main outline-none focus:border-primary transition-all"
            />
            <span v-if="validationErrors.email" class="text-xs text-danger font-medium">{{ validationErrors.email }}</span>
          </div>

          <!-- Phone Number -->
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-muted uppercase tracking-widest" for="mod-phone">Số điện thoại</label>
            <input 
              id="mod-phone"
              type="text" 
              v-model="form.phoneNumber" 
              placeholder="Ví dụ: 0987654321" 
              required
              :disabled="isSubmitting"
              class="w-full bg-surface border border-border-main/80 rounded-xl px-4 py-3.5 text-[14px] text-main outline-none focus:border-primary transition-all"
            />
            <span v-if="validationErrors.phoneNumber" class="text-xs text-danger font-medium">{{ validationErrors.phoneNumber }}</span>
          </div>

          <!-- Avatar Upload -->
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-muted uppercase tracking-widest">Ảnh đại diện (Avatar)</label>
            
            <div class="flex items-center gap-4">
              <!-- Preview Circle -->
              <div class="w-16 h-16 rounded-2xl bg-surface border border-border-main overflow-hidden flex items-center justify-center text-2xl flex-shrink-0">
                <img v-if="avatarPreview" :src="avatarPreview" alt="Preview" class="w-full h-full object-cover" />
                <span v-else>📷</span>
              </div>
              
              <!-- File selector button -->
              <div class="flex-1">
                <input 
                  type="file" 
                  ref="fileInput" 
                  @change="handleFileChange" 
                  accept="image/jpeg,image/jpg,image/png,image/webp"
                  class="hidden" 
                />
                <button 
                  type="button" 
                  @click="$refs.fileInput.click()"
                  :disabled="isSubmitting"
                  class="px-4 py-2.5 rounded-xl bg-surface border border-border-main text-[13px] font-bold hover:bg-surface/80 hover:border-primary/50 transition-all cursor-pointer"
                >
                  Chọn ảnh đại diện
                </button>
                <p class="text-[11px] text-muted mt-1">Hỗ trợ định dạng .jpg, .jpeg, .png, .webp</p>
              </div>
            </div>
            <span v-if="validationErrors.avatar" class="text-xs text-danger font-medium">{{ validationErrors.avatar }}</span>
          </div>

          <!-- Action buttons -->
          <div class="flex gap-4 mt-6">
            <button 
              type="button" 
              @click="closeModal" 
              :disabled="isSubmitting"
              class="flex-1 px-4 py-3.5 rounded-xl border border-border-main text-[14px] font-bold hover:bg-surface/50 transition-all cursor-pointer"
            >
              Hủy bỏ
            </button>
            <button 
              type="submit" 
              :disabled="isSubmitting"
              class="flex-1 px-4 py-3.5 rounded-xl bg-primary text-surface font-bold hover:bg-primary-hover shadow-lg shadow-primary/20 transition-all flex items-center justify-center gap-2 cursor-pointer"
            >
              <svg v-if="isSubmitting" class="w-5 h-5 animate-spin" viewBox="0 0 24 24" fill="none">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="3" />
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
              </svg>
              <span>{{ isSubmitting ? 'Đang tạo...' : 'Tạo tài khoản' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Success Feedback Overlay Modal -->
    <div v-if="successData" class="fixed inset-0 z-[1001] flex items-center justify-center p-4">
      <div class="absolute inset-0 bg-black/80 backdrop-blur-md"></div>
      
      <div class="bg-card border border-primary/20 rounded-[28px] max-w-md w-full p-8 shadow-2xl relative z-10 text-center flex flex-col items-center gap-5 animate-in fade-in zoom-in-95 duration-200">
        <div class="w-16 h-16 rounded-full bg-primary/10 border border-primary/20 flex items-center justify-center text-primary text-3xl">
          ✓
        </div>
        
        <div>
          <h3 class="text-xl font-bold text-main">Tạo thành công!</h3>
          <p class="text-sm text-muted mt-1">Thông tin đăng nhập đã được gửi tới email của người dùng.</p>
        </div>

        <!-- Detail Card -->
        <div class="w-full bg-surface border border-border-main rounded-2xl p-5 text-left flex flex-col gap-3">
          <div class="flex items-center gap-4 pb-3 border-b border-border-main/50">
            <img :src="successData.imageUrl || 'https://ui-avatars.com/api/?name=' + encodeURIComponent(successData.fullName) + '&background=6366f1&color=fff'" class="w-12 h-12 rounded-xl object-cover" />
            <div class="flex flex-col">
              <span class="font-bold text-main leading-tight">{{ successData.fullName }}</span>
              <span class="text-xs text-muted">@{{ successData.userName }}</span>
            </div>
          </div>
          <div class="flex justify-between text-xs">
            <span class="text-muted font-medium">Email:</span>
            <span class="text-main font-bold">{{ successData.email }}</span>
          </div>
          <div class="flex justify-between text-xs">
            <span class="text-muted font-medium">Điện thoại:</span>
            <span class="text-main font-bold">{{ successData.phoneNumber }}</span>
          </div>
          <div class="flex justify-between text-xs">
            <span class="text-muted font-medium">Thời gian tạo:</span>
            <span class="text-main font-bold">{{ formatTime(successData.createdAt) }}</span>
          </div>
        </div>

        <button 
          @click="successData = null"
          class="w-full px-4 py-3 rounded-xl bg-primary text-surface font-bold hover:bg-primary-hover shadow-lg shadow-primary/20 transition-all cursor-pointer"
        >
          Hoàn tất
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { registerModerator } from '../../services/auth/auth.service'
import { addToast } from '../../stores/adminStore'

const moderatorsList = ref([])

// Form states
const isModalOpen = ref(false)
const isSubmitting = ref(false)
const successData = ref(null)

const form = ref({
  fullName: '',
  email: '',
  phoneNumber: '',
  avatarFile: null
})

const avatarPreview = ref(null)
const validationErrors = ref({})

const openCreateModal = () => {
  form.value = {
    fullName: '',
    email: '',
    phoneNumber: '',
    avatarFile: null
  }
  avatarPreview.value = null
  validationErrors.value = {}
  isModalOpen.value = true
}

const closeModal = () => {
  if (!isSubmitting.value) {
    isModalOpen.value = false
  }
}

const handleFileChange = (e) => {
  const file = e.target.files[0]
  if (!file) return

  form.value.avatarFile = file
  
  // Show image preview
  const reader = new FileReader()
  reader.onload = (event) => {
    avatarPreview.value = event.target.result
  }
  reader.readAsDataURL(file)
}

const handleSubmit = async () => {
  isSubmitting.value = true
  validationErrors.value = {}

  // Construct multipart/form-data
  const formData = new FormData()
  formData.append('fullName', form.value.fullName)
  formData.append('email', form.value.email)
  formData.append('phoneNumber', form.value.phoneNumber)
  if (form.value.avatarFile) {
    formData.append('Avatar', form.value.avatarFile)
  }

  try {
    const res = await registerModerator(formData)
    if (res.success) {
      const createdMod = res.data
      moderatorsList.value.unshift(createdMod)
      successData.value = createdMod
      isModalOpen.value = false
      addToast('Tạo tài khoản Moderator thành công!', 'success')
    } else {
      validationErrors.value.general = res.message || 'Có lỗi xảy ra khi tạo tài khoản.'
    }
  } catch (err) {
    const errorData = err.response?.data
    if (errorData?.errors) {
      if (Array.isArray(errorData.errors)) {
        const normalized = {}
        errorData.errors.forEach(item => {
          if (item && item.field) {
            if (!normalized[item.field]) {
              normalized[item.field] = []
            }
            normalized[item.field].push(item.message)
          }
        })
        validationErrors.value = normalized
      } else {
        validationErrors.value = { ...errorData.errors }
      }
    } else {
      validationErrors.value.general = errorData?.message || err.message || 'Lỗi kết nối máy chủ.'
    }
  } finally {
    isSubmitting.value = false
  }
}

const formatTime = (timeString) => {
  if (!timeString) return ''
  return new Date(timeString).toLocaleString('vi-VN')
}
</script>
