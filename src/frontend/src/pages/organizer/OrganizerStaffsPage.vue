<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Nhân viên soát vé</h1>
        <p class="text-white/50 font-medium text-lg">Cấp tài khoản cho nhân viên soát vé (check-in) tại sự kiện của bạn.</p>
      </div>
      <BaseButton variant="primary" size="lg" @click="openCreateModal">
        <PhPlus weight="bold" class="text-lg" /> Tạo tài khoản mới
      </BaseButton>
    </div>

    <!-- Staffs List -->
    <div class="flex flex-col gap-4">
      <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] overflow-hidden flex flex-col">
        <div class="p-6 border-b border-white/5 bg-white/[0.02] flex items-center justify-between">
          <h3 class="text-xl font-bold font-heading text-white">Danh sách nhân viên</h3>
          <span class="text-[12px] text-white/50 font-bold uppercase tracking-widest">{{ totalCount }} người dùng</span>
        </div>

        <div v-if="isLoading" class="p-16 flex flex-col items-center justify-center gap-3">
          <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
          <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách...</span>
        </div>

        <div v-else-if="staffsList.length === 0" class="p-16 flex flex-col items-center justify-center text-center gap-4">
          <div class="w-20 h-20 rounded-full bg-white/5 flex items-center justify-center text-4xl text-white/20 mb-2 shadow-inner">
            <PhIdentificationBadge weight="duotone" />
          </div>
          <div class="flex flex-col gap-2">
            <span class="font-bold font-heading text-white text-xl">Chưa có nhân viên nào</span>
            <span class="text-sm text-white/50">Bấm vào nút "Tạo tài khoản mới" phía trên để cấp quyền soát vé.</span>
          </div>
        </div>

        <div v-else class="divide-y divide-white/5">
          <div v-for="staff in staffsList" :key="staff.id" class="p-6 flex flex-col lg:flex-row lg:items-center justify-between gap-6 hover:bg-white/5 transition-all group">
            <div class="flex items-center gap-4">
              <div class="w-14 h-14 rounded-2xl overflow-hidden border border-white/10 flex-shrink-0 relative">
                <img :src="'https://ui-avatars.com/api/?name=' + encodeURIComponent(staff.fullName) + '&background=00E05D&color=000'" alt="Avatar" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" :class="staff.isLocked ? 'grayscale opacity-50' : ''" />
              </div>
              <div class="flex flex-col">
                <span class="font-bold text-white text-[16px] leading-snug group-hover:text-primary transition-colors">{{ staff.fullName }}</span>
                <span class="text-[13px] font-bold text-white/40 mt-0.5">@{{ staff.userName }}</span>
              </div>
            </div>
            <div class="flex flex-wrap items-center gap-6 text-sm text-white/60">
              <div class="flex items-center gap-2">
                <PhEnvelopeSimple class="text-lg opacity-60" />
                <span>{{ staff.email }}</span>
              </div>
              <div class="flex items-center gap-2">
                <PhPhone class="text-lg opacity-60" />
                <span>{{ staff.phoneNumber }}</span>
              </div>
              <span
                class="text-[11px] font-bold px-3 py-1 rounded-full uppercase tracking-wider border"
                :class="staff.isLocked ? 'bg-danger/10 text-danger border-danger/20' : 'bg-primary/10 text-primary border-primary/20'"
              >
                {{ staff.isLocked ? 'Đã khoá' : 'Đang hoạt động' }}
              </span>
              <BaseButton
                :variant="staff.isLocked ? 'primary' : 'outline'"
                size="sm"
                :class="!staff.isLocked ? '!border-danger/30 !text-danger hover:!bg-danger/10' : ''"
                :disabled="togglingStaffId === staff.id"
                @click="handleToggleLock(staff)"
              >
                <PhSpinner v-if="togglingStaffId === staff.id" class="animate-spin" />
                <template v-else>{{ staff.isLocked ? 'Mở khoá' : 'Khoá tài khoản' }}</template>
              </BaseButton>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="flex items-center justify-between p-6 border-t border-white/5">
          <BaseButton variant="outline" size="sm" :disabled="pageNumber <= 1" @click="pageNumber--; fetchStaffs()">Trang trước</BaseButton>
          <span class="text-[12px] font-bold text-white/40">Trang {{ pageNumber }} / {{ totalPages }}</span>
          <BaseButton variant="outline" size="sm" :disabled="pageNumber >= totalPages" @click="pageNumber++; fetchStaffs()">Trang sau</BaseButton>
        </div>
      </div>
    </div>

    <!-- Create Staff Modal -->
    <BaseModal :show="isModalOpen" title="Tạo tài khoản nhân viên soát vé" subtitle="Mã kích hoạt sẽ được gửi tự động tới email." size="lg" @close="closeModal">
      <!-- General Error -->
      <div v-if="validationErrors.general" class="p-4 rounded-xl bg-danger/10 border border-danger/20 text-danger text-sm font-semibold mb-6 flex gap-2">
        <PhWarningCircle class="text-xl" weight="fill" />
        <span>{{ validationErrors.general }}</span>
      </div>

      <form @submit.prevent="handleSubmit" class="flex flex-col gap-6">
        <!-- Full Name -->
        <div class="flex flex-col gap-2">
          <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest" for="staff-fullname">Họ và tên</label>
          <input
            id="staff-fullname" type="text" v-model="form.fullName" placeholder="Nhập họ tên đầy đủ" required :disabled="isSubmitting"
            class="w-full bg-[#0A0F0D] border border-white/10 rounded-xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary transition-all"
          />
          <span v-if="validationErrors.fullName" class="text-xs text-danger font-medium">{{ validationErrors.fullName }}</span>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest" for="staff-email">Địa chỉ Email</label>
            <input
              id="staff-email" type="email" v-model="form.email" placeholder="staff@tickethub.vn" required :disabled="isSubmitting"
              class="w-full bg-[#0A0F0D] border border-white/10 rounded-xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary transition-all"
            />
            <span v-if="validationErrors.email" class="text-xs text-danger font-medium">{{ validationErrors.email }}</span>
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest" for="staff-phone">Số điện thoại</label>
            <input
              id="staff-phone" type="text" v-model="form.phoneNumber" placeholder="Ví dụ: 0987654321" required :disabled="isSubmitting"
              class="w-full bg-[#0A0F0D] border border-white/10 rounded-xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary transition-all"
            />
            <span v-if="validationErrors.phoneNumber" class="text-xs text-danger font-medium">{{ validationErrors.phoneNumber }}</span>
          </div>
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
            <img :src="'https://ui-avatars.com/api/?name=' + encodeURIComponent(successData.fullName) + '&background=00E05D&color=000'" class="w-14 h-14 rounded-2xl object-cover" />
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
import { ref, onMounted } from 'vue'
import { registerStaff, getOrganizerStaffs, setStaffLockStatus } from '../../services/auth/auth.service'
import { addToast } from '../../stores/adminStore'
import { getErrorMessage } from '../../utils/apiError'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseModal from '../../components/ui/BaseModal.vue'
import { PhPlus, PhIdentificationBadge, PhEnvelopeSimple, PhPhone, PhWarningCircle, PhSpinner } from '@phosphor-icons/vue'

const staffsList = ref([])
const isLoading = ref(true)
const pageNumber = ref(1)
const pageSize = 10
const totalPages = ref(1)
const totalCount = ref(0)
const togglingStaffId = ref(null)

const fetchStaffs = async () => {
  isLoading.value = true
  try {
    const res = await getOrganizerStaffs({ pageNumber: pageNumber.value, pageSize })
    if (res && res.success && res.data) {
      staffsList.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      totalCount.value = res.data.totalCount || 0
      pageNumber.value = res.data.pageNumber || 1
    } else {
      staffsList.value = []
      totalCount.value = 0
    }
  } catch (err) {
    console.error('Error fetching organizer staffs:', err)
    addToast(getErrorMessage(err, 'Không thể tải danh sách nhân viên.'), 'error')
    staffsList.value = []
  } finally {
    isLoading.value = false
  }
}

const handleToggleLock = async (staff) => {
  togglingStaffId.value = staff.id
  try {
    const res = await setStaffLockStatus(staff.id, !staff.isLocked)
    if (res && res.success && res.data) {
      staff.isLocked = res.data.isLocked
      addToast(res.message || (staff.isLocked ? 'Đã khoá tài khoản.' : 'Đã mở khoá tài khoản.'), 'success')
    } else {
      addToast(res?.message || 'Không thể cập nhật trạng thái tài khoản.', 'error')
    }
  } catch (err) {
    console.error('Error toggling staff lock status:', err)
    addToast(getErrorMessage(err, 'Không thể cập nhật trạng thái tài khoản.'), 'error')
  } finally {
    togglingStaffId.value = null
  }
}

onMounted(() => {
  fetchStaffs()
})

// Form states
const isModalOpen = ref(false)
const isSubmitting = ref(false)
const successData = ref(null)

const form = ref({
  fullName: '',
  email: '',
  phoneNumber: ''
})

const validationErrors = ref({})

const openCreateModal = () => {
  form.value = { fullName: '', email: '', phoneNumber: '' }
  validationErrors.value = {}
  isModalOpen.value = true
}

const closeModal = () => { if (!isSubmitting.value) isModalOpen.value = false }

const handleSubmit = async () => {
  isSubmitting.value = true
  validationErrors.value = {}

  try {
    const res = await registerStaff({
      fullName: form.value.fullName,
      email: form.value.email,
      phoneNumber: form.value.phoneNumber
    })
    if (res.success) {
      const createdStaff = res.data
      successData.value = createdStaff
      isModalOpen.value = false
      addToast('Tạo tài khoản nhân viên soát vé thành công!', 'success')
      pageNumber.value = 1
      await fetchStaffs()
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
