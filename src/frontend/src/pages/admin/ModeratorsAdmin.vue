<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Kiểm duyệt viên</h1>
        <p class="text-white/50 font-medium text-lg">Quản lý và cấp tài khoản cho đội ngũ kiểm duyệt viên hệ thống.</p>
      </div>
      <BaseButton variant="primary" size="lg" @click="openCreateModal">
        <PhPlus weight="bold" class="text-lg" /> Tạo tài khoản mới
      </BaseButton>
    </div>

    <!-- Moderators List -->
    <div class="flex flex-col gap-4">
      <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] overflow-hidden flex flex-col">
        <div class="p-6 border-b border-white/5 bg-white/[0.02] flex items-center justify-between">
          <h3 class="text-xl font-bold font-heading text-white">Danh sách kiểm duyệt viên</h3>
          <span class="text-[12px] text-white/50 font-bold uppercase tracking-widest">{{ moderatorsList.length }} người dùng</span>
        </div>

        <div v-if="moderatorsList.length === 0" class="p-16 flex flex-col items-center justify-center text-center gap-4">
          <div class="w-20 h-20 rounded-full bg-white/5 flex items-center justify-center text-4xl text-white/20 mb-2 shadow-inner">
            <PhShieldCheck weight="duotone" />
          </div>
          <div class="flex flex-col gap-2">
            <span class="font-bold font-heading text-white text-xl">Chưa có kiểm duyệt viên nào</span>
            <span class="text-sm text-white/50">Bấm vào nút "Tạo tài khoản mới" phía trên để cấp quyền.</span>
          </div>
        </div>

        <div v-else class="divide-y divide-white/5">
          <div v-for="mod in moderatorsList" :key="mod.id" class="p-6 flex flex-col lg:flex-row lg:items-center justify-between gap-6 hover:bg-white/5 transition-all group">
            <div class="flex items-center gap-4">
              <div class="w-14 h-14 rounded-2xl overflow-hidden border border-white/10 flex-shrink-0">
                <img :src="mod.imageUrl || 'https://ui-avatars.com/api/?name=' + encodeURIComponent(mod.fullName) + '&background=00E05D&color=000'" alt="Avatar" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              </div>
              <div class="flex flex-col">
                <span class="font-bold text-white text-[16px] leading-snug group-hover:text-primary transition-colors">{{ mod.fullName }}</span>
                <span class="text-[13px] font-bold text-white/40 mt-0.5">@{{ mod.userName }}</span>
              </div>
            </div>
            <div class="flex flex-wrap items-center gap-6 text-sm text-white/60">
              <div class="flex items-center gap-2">
                <PhEnvelopeSimple class="text-lg opacity-60" />
                <span>{{ mod.email }}</span>
              </div>
              <div class="flex items-center gap-2">
                <PhPhone class="text-lg opacity-60" />
                <span>{{ mod.phoneNumber }}</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="text-[11px] bg-primary/10 text-primary font-bold px-3 py-1 rounded-full uppercase tracking-wider border border-primary/20">Moderator</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Moderator Modal -->
    <BaseModal :show="isModalOpen" title="Tạo tài khoản Moderator" subtitle="Mã kích hoạt sẽ được gửi tự động tới email." size="lg" @close="closeModal">
      <!-- General Error -->
      <div v-if="validationErrors.general" class="p-4 rounded-xl bg-danger/10 border border-danger/20 text-danger text-sm font-semibold mb-6 flex gap-2">
        <PhWarningCircle class="text-xl" weight="fill" />
        <span>{{ validationErrors.general }}</span>
      </div>

      <form @submit.prevent="handleSubmit" class="flex flex-col gap-6">
        <!-- Full Name -->
        <div class="flex flex-col gap-2">
          <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest" for="mod-fullname">Họ và tên</label>
          <input 
            id="mod-fullname" type="text" v-model="form.fullName" placeholder="Nhập họ tên đầy đủ" required :disabled="isSubmitting"
            class="w-full bg-[#0A0F0D] border border-white/10 rounded-xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary transition-all"
          />
          <span v-if="validationErrors.fullName" class="text-xs text-danger font-medium">{{ validationErrors.fullName }}</span>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest" for="mod-email">Địa chỉ Email</label>
            <input 
              id="mod-email" type="email" v-model="form.email" placeholder="mod@tickethub.vn" required :disabled="isSubmitting"
              class="w-full bg-[#0A0F0D] border border-white/10 rounded-xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary transition-all"
            />
            <span v-if="validationErrors.email" class="text-xs text-danger font-medium">{{ validationErrors.email }}</span>
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest" for="mod-phone">Số điện thoại</label>
            <input 
              id="mod-phone" type="text" v-model="form.phoneNumber" placeholder="Ví dụ: 0987654321" required :disabled="isSubmitting"
              class="w-full bg-[#0A0F0D] border border-white/10 rounded-xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary transition-all"
            />
            <span v-if="validationErrors.phoneNumber" class="text-xs text-danger font-medium">{{ validationErrors.phoneNumber }}</span>
          </div>
        </div>

        <div class="flex flex-col gap-2">
          <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Ảnh đại diện</label>
          <div class="flex items-center gap-4">
            <div class="w-16 h-16 rounded-2xl bg-[#0A0F0D] border border-white/10 overflow-hidden flex items-center justify-center text-2xl flex-shrink-0 text-white/20">
              <img v-if="avatarPreview" :src="avatarPreview" alt="Preview" class="w-full h-full object-cover" />
              <PhCamera v-else weight="duotone" />
            </div>
            <div class="flex-1">
              <input type="file" ref="fileInput" @change="handleFileChange" accept="image/jpeg,image/jpg,image/png,image/webp" class="hidden" />
              <button 
                type="button" @click="$refs.fileInput.click()" :disabled="isSubmitting"
                class="px-5 py-2.5 rounded-full bg-white/5 border border-white/10 text-[13px] font-bold text-white hover:bg-white/10 hover:border-white/20 transition-all cursor-pointer"
              >
                Chọn ảnh đại diện
              </button>
              <p class="text-[11px] text-white/40 mt-2">Hỗ trợ định dạng .jpg, .jpeg, .png, .webp</p>
            </div>
          </div>
          <span v-if="validationErrors.avatar" class="text-xs text-danger font-medium">{{ validationErrors.avatar }}</span>
        </div>

      </form>

      <template #footer>
        <div class="flex gap-4 w-full">
          <BaseButton variant="outline" size="lg" class="flex-1" @click="closeModal" :disabled="isSubmitting">Hủy bỏ</BaseButton>
          <BaseButton variant="primary" size="lg" class="flex-1" @click="handleSubmit" :loading="isSubmitting">Tạo tài khoản</BaseButton>
        </div>
      </template>
    </BaseModal>

    <!-- Success Modal -->
    <BaseModal :show="!!successData" title="Tạo thành công!" subtitle="Thông tin đăng nhập đã gửi tới email" size="sm" @close="successData = null">
      <div v-if="successData" class="flex flex-col gap-6 pt-4">
        <div class="w-full bg-[#111916] border border-white/5 rounded-2xl p-5 flex flex-col gap-4">
          <div class="flex items-center gap-4 pb-4 border-b border-white/5">
            <img :src="successData.imageUrl || 'https://ui-avatars.com/api/?name=' + encodeURIComponent(successData.fullName) + '&background=00E05D&color=000'" class="w-14 h-14 rounded-2xl object-cover" />
            <div class="flex flex-col">
              <span class="font-bold text-white text-[16px] leading-tight">{{ successData.fullName }}</span>
              <span class="text-xs text-white/50 mt-1">@{{ successData.userName }}</span>
            </div>
          </div>
          <div class="flex justify-between text-[13px]">
            <span class="text-white/50 font-bold">Email:</span>
            <span class="text-white font-bold">{{ successData.email }}</span>
          </div>
          <div class="flex justify-between text-[13px]">
            <span class="text-white/50 font-bold">Điện thoại:</span>
            <span class="text-white font-bold">{{ successData.phoneNumber }}</span>
          </div>
          <div class="flex justify-between text-[13px]">
            <span class="text-white/50 font-bold">Tạo lúc:</span>
            <span class="text-white font-bold">{{ formatTime(successData.createdAt) }}</span>
          </div>
        </div>

        <BaseButton variant="primary" size="lg" class="w-full" @click="successData = null">Hoàn tất</BaseButton>
      </div>
    </BaseModal>

  </div>
</template>

<script setup>
import { ref } from 'vue'
import { registerModerator } from '../../services/auth/auth.service'
import { addToast } from '../../stores/adminStore'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseModal from '../../components/ui/BaseModal.vue'
import { PhPlus, PhShieldCheck, PhEnvelopeSimple, PhPhone, PhWarningCircle, PhCamera } from '@phosphor-icons/vue'

const moderatorsList = ref([])

// Form states
const isModalOpen = ref(false)
const isSubmitting = ref(false)
const successData = ref(null)
const fileInput = ref(null)

const form = ref({
  fullName: '',
  email: '',
  phoneNumber: '',
  avatarFile: null
})

const avatarPreview = ref(null)
const validationErrors = ref({})

const openCreateModal = () => {
  form.value = { fullName: '', email: '', phoneNumber: '', avatarFile: null }
  avatarPreview.value = null
  validationErrors.value = {}
  isModalOpen.value = true
}

const closeModal = () => { if (!isSubmitting.value) isModalOpen.value = false }

const handleFileChange = (e) => {
  const file = e.target.files[0]
  if (!file) return

  form.value.avatarFile = file
  
  const reader = new FileReader()
  reader.onload = (event) => { avatarPreview.value = event.target.result }
  reader.readAsDataURL(file)
}

const handleSubmit = async () => {
  isSubmitting.value = true
  validationErrors.value = {}

  const formData = new FormData()
  formData.append('fullName', form.value.fullName)
  formData.append('email', form.value.email)
  formData.append('phoneNumber', form.value.phoneNumber)
  if (form.value.avatarFile) formData.append('Avatar', form.value.avatarFile)

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
            if (!normalized[item.field]) normalized[item.field] = []
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
