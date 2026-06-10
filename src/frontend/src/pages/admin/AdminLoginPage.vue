<template>
  <div class="min-h-screen grid grid-cols-1 lg:grid-cols-2 bg-[#050807] text-white">
    <!-- Left: Brand / Image -->
    <div class="relative hidden lg:flex flex-col justify-between p-12 bg-[#0A0F0D] overflow-hidden border-r border-white/5">
      <!-- Decor -->
      <div class="absolute top-[-20%] left-[-10%] w-[80%] h-[80%] bg-primary/20 blur-[120px] rounded-full mix-blend-screen pointer-events-none"></div>
      <div class="absolute bottom-[-10%] right-[-20%] w-[60%] h-[60%] bg-[#00E676]/10 blur-[100px] rounded-full mix-blend-screen pointer-events-none"></div>

      <div class="relative z-10 flex items-center gap-3">
        <div class="w-12 h-12 bg-primary text-black rounded-2xl flex items-center justify-center font-black text-2xl shadow-[0_0_30px_rgba(0,200,83,0.3)]">
          <PhShieldCheck weight="fill" />
        </div>
        <span class="font-heading font-black text-3xl tracking-tight uppercase">TicketHub</span>
      </div>

      <div class="relative z-10 max-w-md">
        <h2 class="font-heading text-5xl xl:text-6xl font-black leading-[1.1] mb-6 tracking-tight">Hệ thống <br/><span class="text-primary">quản trị</span></h2>
        <p class="text-white/50 text-lg font-medium leading-relaxed">Giám sát mọi hoạt động, quản lý người dùng và sự kiện với độ bảo mật tuyệt đối.</p>
      </div>

      <div class="relative z-10 text-white/30 text-sm font-bold tracking-widest uppercase">
        © {{ new Date().getFullYear() }} TicketHub Platform
      </div>
    </div>

    <!-- Right: Form -->
    <div class="flex items-center justify-center p-8 sm:p-12 lg:p-24 relative overflow-hidden">
      <!-- Mobile decor -->
      <div class="absolute inset-0 bg-primary/5 blur-[100px] lg:hidden rounded-full pointer-events-none"></div>

      <div class="w-full max-w-[400px] relative z-10">
        <!-- Mobile Header -->
        <div class="flex items-center justify-center gap-3 lg:hidden mb-12">
          <div class="w-12 h-12 bg-primary text-black rounded-2xl flex items-center justify-center font-black text-2xl shadow-[0_0_30px_rgba(0,200,83,0.3)]">
            <PhShieldCheck weight="fill" />
          </div>
          <span class="font-heading font-black text-3xl tracking-tight uppercase">TicketHub</span>
        </div>

        <div class="mb-10 text-center lg:text-left">
          <h1 class="font-heading text-3xl font-black mb-2">Đăng nhập Admin</h1>
          <p class="text-white/50 font-medium">Truy cập vào hệ thống điều hành</p>
        </div>

        <Transition
          enter-active-class="transition duration-300 ease-out"
          enter-from-class="opacity-0 -translate-y-4"
          enter-to-class="opacity-100 translate-y-0"
          leave-active-class="transition duration-200 ease-in"
          leave-from-class="opacity-100"
          leave-to-class="opacity-0"
        >
          <div v-if="error" class="mb-6 p-4 rounded-2xl bg-danger/10 border border-danger/20 flex gap-3 items-start">
            <PhWarningCircle class="text-danger text-xl flex-shrink-0 mt-0.5" weight="fill" />
            <span class="text-danger text-[14px] font-bold">{{ error }}</span>
          </div>
        </Transition>

        <form v-if="step === 1" @submit.prevent="handleLogin" class="flex flex-col gap-6">
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="admin-username">Tài khoản</label>
            <div class="relative flex items-center group">
              <PhUser class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input 
                id="admin-username" type="text" v-model="username" placeholder="Nhập tên đăng nhập" required autocomplete="username" :disabled="isLoading"
                class="w-full bg-white/5 border border-white/10 rounded-2xl py-4 pl-12 pr-4 text-[15px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium"
              />
            </div>
            <div v-if="validationErrors.userName || validationErrors.username" class="flex flex-col gap-1 mt-1 px-1">
              <span v-for="err in (validationErrors.userName || validationErrors.username)" :key="err" class="text-xs text-danger font-bold flex items-center gap-1.5">
                <PhXCircle weight="fill" /> {{ err }}
              </span>
            </div>
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="admin-password">Mật khẩu</label>
            <div class="relative flex items-center group">
              <PhLockKey class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input 
                id="admin-password" :type="showPassword ? 'text' : 'password'" v-model="password" placeholder="••••••••" required autocomplete="current-password" :disabled="isLoading"
                class="w-full bg-white/5 border border-white/10 rounded-2xl py-4 pl-12 pr-12 text-[15px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium tracking-wide"
              />
              <button type="button" @click="showPassword = !showPassword" tabindex="-1" class="absolute right-4 text-white/30 hover:text-white transition-colors cursor-pointer">
                <PhEye v-if="!showPassword" class="text-lg" weight="bold" />
                <PhEyeClosed v-else class="text-lg" weight="bold" />
              </button>
            </div>
            <div v-if="validationErrors.password" class="flex flex-col gap-1 mt-1 px-1">
              <span v-for="err in validationErrors.password" :key="err" class="text-xs text-danger font-bold flex items-center gap-1.5">
                <PhXCircle weight="fill" /> {{ err }}
              </span>
            </div>
          </div>

          <button type="submit" :disabled="isLoading || !username || !password" class="mt-4 w-full bg-primary hover:bg-primary-hover text-black font-black py-4 rounded-2xl shadow-[0_0_40px_rgba(0,200,83,0.3)] hover:shadow-[0_0_60px_rgba(0,200,83,0.5)] transition-all hover:-translate-y-1 disabled:opacity-50 disabled:hover:translate-y-0 disabled:shadow-none flex justify-center items-center gap-2 cursor-pointer">
            <PhCircleNotch v-if="isLoading" class="animate-spin text-xl" weight="bold" />
            <PhSignIn v-else class="text-xl" weight="bold" />
            {{ isLoading ? 'Đang xác thực...' : 'Đăng nhập' }}
          </button>
        </form>

        <form v-else @submit.prevent="handleConfirm" class="flex flex-col gap-6">
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1 text-center" for="admin-code">Nhập mã xác thực (OTP)</label>
            <div class="relative flex items-center group">
              <PhShieldCheck class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input 
                id="admin-code" type="text" v-model="code" placeholder="Mã 6 số" required maxlength="6" :disabled="isLoading"
                class="w-full bg-white/5 border border-white/10 rounded-2xl py-5 pl-12 pr-4 text-2xl font-black text-center tracking-[0.5em] text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/10 placeholder:tracking-normal"
              />
            </div>
            <div v-if="validationErrors.code" class="flex flex-col gap-1 mt-1 px-1">
              <span v-for="err in validationErrors.code" :key="err" class="text-xs text-danger font-bold flex items-center gap-1.5 justify-center">
                <PhXCircle weight="fill" /> {{ err }}
              </span>
            </div>
          </div>

          <button type="submit" :disabled="isLoading || !code" class="mt-4 w-full bg-primary hover:bg-primary-hover text-black font-black py-4 rounded-2xl shadow-[0_0_40px_rgba(0,200,83,0.3)] hover:shadow-[0_0_60px_rgba(0,200,83,0.5)] transition-all hover:-translate-y-1 disabled:opacity-50 disabled:hover:translate-y-0 disabled:shadow-none flex justify-center items-center gap-2 cursor-pointer">
            <PhCircleNotch v-if="isLoading" class="animate-spin text-xl" weight="bold" />
            <PhCheckCircle v-else class="text-xl" weight="bold" />
            {{ isLoading ? 'Đang kiểm tra...' : 'Xác nhận mã' }}
          </button>

          <button type="button" @click="step = 1" class="text-white/40 hover:text-white text-[13px] font-bold transition-colors text-center mt-4 cursor-pointer">
            Quay lại nhập tài khoản
          </button>
        </form>

        <div class="mt-16 text-center">
          <router-link to="/" class="inline-flex items-center gap-2 text-white/30 hover:text-white font-bold text-[13px] transition-colors group">
            <PhArrowLeft class="group-hover:-translate-x-1 transition-transform" weight="bold" />
            Quay về trang chủ
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { loginAdmin, confirmAdmin } from '../../services/auth/auth.service'
import { PhShieldCheck, PhWarningCircle, PhUser, PhLockKey, PhEye, PhEyeClosed, PhSignIn, PhCircleNotch, PhShield, PhCheckCircle, PhArrowLeft, PhXCircle } from '@phosphor-icons/vue'

const router = useRouter()

const step = ref(1)
const username = ref('')
const password = ref('')
const code = ref('')
const showPassword = ref(false)
const isLoading = ref(false)
const error = ref('')
const validationErrors = ref({})

const handleLogin = async () => {
  isLoading.value = true
  error.value = ''
  validationErrors.value = {}
  try {
    await loginAdmin(username.value, password.value)
    step.value = 2
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
      error.value = errorData?.message || err.response?.data?.title || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.'
    }
  } finally {
    isLoading.value = false
  }
}

const handleConfirm = async () => {
  isLoading.value = true
  error.value = ''
  validationErrors.value = {}
  try {
    await confirmAdmin(username.value, code.value)
    await router.push('/admin/dashboard')
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
      error.value = errorData?.message || err.response?.data?.title || 'Mã xác thực không chính xác hoặc đã hết hạn.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>
