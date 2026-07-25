<template>
  <div class="min-h-screen bg-[#0A0F0D] flex items-center justify-center p-6 relative overflow-hidden">
    <!-- Animated background -->
    <div class="absolute inset-0 pointer-events-none z-0">
      <div class="absolute top-0 left-0 w-full h-full bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] from-primary/10 via-transparent to-transparent opacity-60"></div>
      <div class="absolute -top-32 -right-32 w-[500px] h-[500px] bg-primary/20 blur-[150px] rounded-full mix-blend-screen"></div>
      <div class="absolute -bottom-32 -left-32 w-[400px] h-[400px] bg-[#00A355]/20 blur-[150px] rounded-full mix-blend-screen"></div>
    </div>

    <!-- Activation Card -->
    <div class="w-full max-w-[480px] bg-[#111916]/80 backdrop-blur-xl border border-white/10 rounded-[3rem] p-10 lg:p-12 shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] relative z-10 animate-fade-up">
      <!-- Header -->
      <div class="text-center mb-10">
        <div class="w-20 h-20 mx-auto bg-primary/10 border border-primary/20 rounded-[1.5rem] flex items-center justify-center text-4xl text-primary mb-6 shadow-inner">
          <PhKey weight="duotone" />
        </div>
        <h1 class="text-4xl font-black font-heading text-white tracking-tight uppercase mb-2">Kích hoạt tài khoản</h1>
        <p class="text-[15px] text-white/50 font-medium">Thiết lập mật khẩu mới cho tài khoản {{ roleLabel }} của bạn</p>
      </div>

      <!-- General Error -->
      <Transition
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
      >
        <div v-if="validationErrors.general" class="mb-8 p-5 bg-danger/10 border border-danger/20 rounded-2xl flex items-start gap-3">
          <PhWarningCircle class="text-xl text-danger mt-0.5 shrink-0" weight="fill" />
          <span class="text-[14px] text-danger font-bold leading-relaxed">{{ validationErrors.general }}</span>
        </div>
      </Transition>

      <div v-if="missingParams" class="mb-8 p-6 bg-warning/10 border border-warning/20 rounded-2xl text-center">
        <PhWarning class="text-3xl text-warning mx-auto mb-3" weight="duotone" />
        <h3 class="text-[15px] font-black text-warning mb-1">Tham số không hợp lệ</h3>
        <p class="text-[13px] text-warning/70 font-medium">Thiếu thông tin người dùng hoặc token kích hoạt trong đường dẫn. Vui lòng kiểm tra lại email của bạn.</p>
      </div>

      <!-- Form -->
      <form v-else @submit.prevent="handleActivate" class="space-y-6">
        <!-- Password Field -->
        <div class="space-y-2">
          <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1">Mật khẩu mới</label>
          <div class="relative group">
            <PhLockKey class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="password"
              placeholder="Nhập mật khẩu mới"
              required
              autocomplete="new-password"
              :disabled="isLoading"
              class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl py-4 pl-14 pr-12 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20 shadow-inner"
            />
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="absolute right-4 top-1/2 -translate-y-1/2 w-8 h-8 flex items-center justify-center text-white/30 hover:text-white transition-colors"
              tabindex="-1"
            >
              <PhEyeSlash v-if="!showPassword" weight="bold" class="text-lg" />
              <PhEye v-else weight="bold" class="text-lg" />
            </button>
          </div>
          <!-- Field Validation Errors -->
          <div v-if="validationErrors.password" class="flex flex-col gap-1 mt-2">
            <span v-for="err in validationErrors.password" :key="err" class="text-[12px] text-danger font-medium flex items-center gap-1.5 ml-1">
              <PhXCircle weight="fill" /> {{ err }}
            </span>
          </div>
        </div>

        <!-- Confirm Password Field -->
        <div class="space-y-2">
          <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1">Xác nhận mật khẩu</label>
          <div class="relative group">
            <PhCheckCircle class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
            <input
              :type="showConfirmPassword ? 'text' : 'password'"
              v-model="confirmPassword"
              placeholder="Xác nhận lại mật khẩu"
              required
              autocomplete="new-password"
              :disabled="isLoading"
              class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl py-4 pl-14 pr-12 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20 shadow-inner"
            />
            <button
              type="button"
              @click="showConfirmPassword = !showConfirmPassword"
              class="absolute right-4 top-1/2 -translate-y-1/2 w-8 h-8 flex items-center justify-center text-white/30 hover:text-white transition-colors"
              tabindex="-1"
            >
              <PhEyeSlash v-if="!showConfirmPassword" weight="bold" class="text-lg" />
              <PhEye v-else weight="bold" class="text-lg" />
            </button>
          </div>
          <div v-if="validationErrors.confirmPassword" class="flex flex-col gap-1 mt-2">
            <span v-for="err in validationErrors.confirmPassword" :key="err" class="text-[12px] text-danger font-medium flex items-center gap-1.5 ml-1">
              <PhXCircle weight="fill" /> {{ err }}
            </span>
          </div>
        </div>

        <!-- Submit Button -->
        <BaseButton
          type="submit"
          variant="primary"
          size="lg"
          :disabled="isLoading || !password || !confirmPassword"
          class="w-full !py-4 mt-4 !rounded-2xl shadow-[0_0_30px_rgba(0,200,83,0.2)] hover:shadow-[0_0_40px_rgba(0,200,83,0.4)] text-[15px]"
        >
          <template v-if="isLoading">
            <PhCircleNotch class="animate-spin text-xl mr-2" weight="bold" /> Đang kích hoạt...
          </template>
          <template v-else>
            <PhShieldCheck weight="bold" class="text-xl mr-2" /> Hoàn tất & Kích hoạt
          </template>
        </BaseButton>
      </form>

      <!-- Footer -->
      <div class="mt-10 text-center border-t border-white/5 pt-6">
        <router-link to="/" class="inline-flex items-center gap-2 text-[13px] font-bold text-white/40 hover:text-white transition-colors">
          <PhArrowLeft weight="bold" /> Quay về trang chủ
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { activateModeratorAccount, activateStaffAccount } from '../services/auth/auth.service'
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { 
  PhKey, PhWarningCircle, PhWarning, PhLockKey, 
  PhEye, PhEyeSlash, PhCheckCircle, PhXCircle,
  PhCircleNotch, PhShieldCheck, PhArrowLeft
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const userId = route.query.userId || ''
const token = route.query.token || ''
const role = route.query.role === 'staff' ? 'staff' : 'moderator'

const roleLabel = role === 'staff' ? 'nhân viên soát vé' : 'kiểm duyệt viên'

const password = ref('')
const confirmPassword = ref('')
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const isLoading = ref(false)
const validationErrors = ref({})

const missingParams = computed(() => !userId || !token)

const handleActivate = async () => {
  isLoading.value = true
  validationErrors.value = {}
  
  const payload = {
    userId: String(userId),
    token: encodeURIComponent(String(token)),
    password: password.value,
    confirmPassword: confirmPassword.value
  }

  try {
    const res = role === 'staff'
      ? await activateStaffAccount(payload)
      : await activateModeratorAccount(payload)
    if (res.success) {
      store.toast = { message: 'Kích hoạt tài khoản thành công! Vui lòng đăng nhập.', icon: 'PhShieldCheck' }
      await router.push(role === 'staff' ? '/staff/login' : '/moderator/login')
    } else {
      validationErrors.value.general = res.message || 'Kích hoạt tài khoản không thành công.'
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
    isLoading.value = false
  }
}
</script>
