<template>
  <div class="min-h-screen flex items-center justify-center bg-[#0A0F0D] text-white p-6 relative overflow-hidden">
    <div class="absolute inset-0 pointer-events-none z-0">
      <div class="absolute -top-32 -right-32 w-[500px] h-[500px] bg-primary/20 blur-[150px] rounded-full mix-blend-screen"></div>
      <div class="absolute -bottom-32 -left-32 w-[400px] h-[400px] bg-[#00A355]/20 blur-[150px] rounded-full mix-blend-screen"></div>
    </div>

    <div class="w-full max-w-[400px] relative z-10">
      <div class="flex items-center justify-center gap-3 mb-12">
        <div class="w-12 h-12 bg-primary text-black rounded-2xl flex items-center justify-center font-black text-2xl shadow-[0_0_30px_rgba(0,200,83,0.3)]">
          <PhQrCode weight="fill" />
        </div>
        <span class="font-heading font-black text-3xl tracking-tight uppercase">TicketHub</span>
      </div>

      <div class="mb-10 text-center">
        <h1 class="font-heading text-3xl font-black mb-2">Đăng nhập Soát vé</h1>
        <p class="text-white/50 font-medium">Dành cho nhân viên check-in tại sự kiện</p>
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
          <div class="flex flex-col gap-0.5">
            <span class="text-danger text-[14px] font-bold">Lỗi đăng nhập</span>
            <span class="text-danger/80 text-[13px] font-medium">{{ error }}</span>
          </div>
        </div>
      </Transition>

      <form @submit.prevent="handleLogin" class="flex flex-col gap-6">
        <div class="flex flex-col gap-2">
          <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="staff-username">Tài khoản</label>
          <div class="relative flex items-center group">
            <PhUser class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
            <input
              id="staff-username" type="text" v-model="username" placeholder="Nhập tên đăng nhập" required autocomplete="username" :disabled="isLoading"
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
          <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="staff-password">Mật khẩu</label>
          <div class="relative flex items-center group">
            <PhLockKey class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
            <input
              id="staff-password" :type="showPassword ? 'text' : 'password'" v-model="password" placeholder="••••••••" required autocomplete="current-password" :disabled="isLoading"
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

        <button
          type="submit"
          :disabled="isLoading || !username || !password"
          class="mt-4 w-full bg-primary hover:bg-emerald-500 text-black font-black py-4 rounded-2xl shadow-[0_0_30px_rgba(0,200,83,0.2)] hover:shadow-[0_0_55px_rgba(0,200,83,0.4)] transition-all hover:-translate-y-1 disabled:opacity-50 disabled:hover:translate-y-0 disabled:shadow-none flex justify-center items-center gap-2 cursor-pointer"
        >
          <PhCircleNotch v-if="isLoading" class="animate-spin text-xl" weight="bold" />
          <PhSignIn v-else class="text-xl" weight="bold" />
          {{ isLoading ? 'Đang xác thực...' : 'Đăng nhập' }}
        </button>
      </form>

      <div class="mt-12 text-center">
        <router-link to="/" class="inline-flex items-center justify-center gap-2 text-white/30 hover:text-white transition-colors group text-[13px] font-bold">
          <PhArrowLeft class="group-hover:-translate-x-1 transition-transform" weight="bold" />
          Quay về trang chủ
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { loginStaff } from '../../services/auth/auth.service'
import {
  PhWarningCircle, PhUser, PhLockKey, PhEye, PhEyeClosed, PhSignIn,
  PhCircleNotch, PhArrowLeft, PhXCircle, PhQrCode
} from '@phosphor-icons/vue'

const router = useRouter()

const username = ref('')
const password = ref('')
const showPassword = ref(false)
const isLoading = ref(false)
const error = ref('')
const validationErrors = ref({})

const handleLogin = async () => {
  isLoading.value = true
  error.value = ''
  validationErrors.value = {}
  try {
    await loginStaff(username.value, password.value)
    await router.push('/staff/dashboard')
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
      error.value = errorData?.message || err.response?.data?.title || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>
