<template>
  <div class="min-h-screen grid grid-cols-1 lg:grid-cols-2 bg-[#0A0F0D] text-white">
    <!-- Left: Form -->
    <div class="flex items-center justify-center p-8 sm:p-12 lg:p-24 relative overflow-hidden order-2 lg:order-1">
      <!-- Background glow -->
      <div class="absolute inset-0 bg-emerald-500/5 blur-[120px] rounded-full pointer-events-none"></div>

      <div class="w-full max-w-[400px] relative z-10">
        <!-- Mobile Header -->
        <div class="flex items-center justify-center gap-3 lg:hidden mb-12">
          <div class="w-12 h-12 bg-primary text-black rounded-2xl flex items-center justify-center font-black text-2xl shadow-[0_0_30px_rgba(0,200,83,0.3)]">
            <PhTicket weight="fill" />
          </div>
          <span class="font-heading font-black text-3xl tracking-tight uppercase">TicketHub</span>
        </div>

        <div class="mb-10 text-center lg:text-left">
          <h1 class="font-heading text-3xl font-black mb-2">Đăng nhập Đối tác</h1>
          <p class="text-white/50 font-medium">Truy cập Dashboard dành cho Ban tổ chức</p>
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
          <!-- Username -->
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="org-username">Tài khoản</label>
            <div class="relative flex items-center group">
              <PhUser class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input 
                id="org-username" type="text" v-model="username" placeholder="Nhập tên đăng nhập" required autocomplete="username" :disabled="isLoading"
                class="w-full bg-white/5 border border-white/10 rounded-2xl py-4 pl-12 pr-4 text-[15px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium"
              />
            </div>
            <div v-if="validationErrors.userName || validationErrors.username" class="flex flex-col gap-1 mt-1 px-1">
              <span v-for="err in (validationErrors.userName || validationErrors.username)" :key="err" class="text-xs text-danger font-bold flex items-center gap-1.5">
                <PhXCircle weight="fill" /> {{ err }}
              </span>
            </div>
          </div>

          <!-- Password -->
          <div class="flex flex-col gap-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="org-password">Mật khẩu</label>
            <div class="relative flex items-center group">
              <PhLockKey class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input 
                id="org-password" :type="showPassword ? 'text' : 'password'" v-model="password" placeholder="••••••••" required autocomplete="current-password" :disabled="isLoading"
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

        <!-- Navigation / Registration links -->
        <div class="mt-12 text-center flex flex-col gap-3 text-[13px] font-bold text-white/50">
          <div>
            Chưa có tài khoản đối tác?
            <router-link to="/organizer/register" class="text-primary hover:underline ml-1">Đăng ký ngay</router-link>
          </div>
          <router-link to="/" class="inline-flex items-center justify-center gap-2 text-white/30 hover:text-white transition-colors group mt-4">
            <PhArrowLeft class="group-hover:-translate-x-1 transition-transform" weight="bold" />
            Quay về trang chủ
          </router-link>
        </div>
      </div>
    </div>

    <!-- Right: Brand / Image -->
    <div class="relative hidden lg:flex flex-col justify-between p-12 bg-[#0A0F0D] overflow-hidden border-l border-white/5 order-1 lg:order-2">
      <!-- Decor lights -->
      <div class="absolute top-[-20%] left-[-10%] w-[80%] h-[80%] bg-primary/10 blur-[130px] rounded-full mix-blend-screen pointer-events-none"></div>
      <div class="absolute bottom-[-10%] right-[-20%] w-[60%] h-[60%] bg-[#00c853]/5 blur-[100px] rounded-full mix-blend-screen pointer-events-none"></div>

      <div class="relative z-10 flex items-center gap-3">
        <div class="w-12 h-12 bg-primary text-black rounded-2xl flex items-center justify-center font-black text-2xl shadow-[0_0_30px_rgba(0,200,83,0.25)]">
          <PhTicket weight="fill" />
        </div>
        <span class="font-heading font-black text-3xl tracking-tight uppercase">TicketHub Partner</span>
      </div>

      <div class="relative z-10 max-w-md">
        <h2 class="font-heading text-5xl xl:text-6xl font-black leading-[1.1] mb-6 tracking-tight">
          Quản lý <br/><span class="text-primary">sự kiện</span> chuyên nghiệp
        </h2>
        <p class="text-white/50 text-lg font-medium leading-relaxed">
          Đăng nhập vào bảng điều khiển dành riêng cho ban tổ chức để bắt đầu vận hành chiến dịch bán vé của bạn.
        </p>
      </div>

      <div class="relative z-10 text-white/30 text-sm font-bold tracking-widest uppercase">
        © {{ new Date().getFullYear() }} TicketHub Platform
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { loginOrganizer } from '../../services/auth/auth.service'
import { 
  PhWarningCircle, PhUser, PhLockKey, PhEye, PhEyeClosed, PhSignIn, 
  PhCircleNotch, PhArrowLeft, PhXCircle, PhTicket 
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
    await loginOrganizer(username.value, password.value)
    await router.push('/organizer')
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
