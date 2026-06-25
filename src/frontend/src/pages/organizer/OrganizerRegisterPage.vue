<template>
  <div class="min-h-screen grid grid-cols-1 lg:grid-cols-2 bg-[#0A0F0D] text-white">
    <!-- Left: Form -->
    <div class="flex items-center justify-center p-6 sm:p-10 lg:p-16 relative overflow-hidden order-2 lg:order-1">
      <!-- Background glow -->
      <div class="absolute inset-0 bg-emerald-500/5 blur-[120px] rounded-full pointer-events-none"></div>

      <div class="w-full max-w-[500px] relative z-10 my-8">
        <!-- Mobile Header -->
        <div class="flex items-center justify-center gap-3 lg:hidden mb-8">
          <div class="w-10 h-10 bg-primary text-black rounded-xl flex items-center justify-center font-black text-xl shadow-[0_0_25px_rgba(0,200,83,0.3)]">
            <PhTicket weight="fill" />
          </div>
          <span class="font-heading font-black text-2xl tracking-tight uppercase">TicketHub</span>
        </div>

        <div class="mb-8 text-center lg:text-left">
          <h1 class="font-heading text-2xl md:text-3xl font-black mb-2">Đăng ký tài khoản Đối tác</h1>
          <p class="text-white/50 font-medium">Bắt đầu tổ chức sự kiện và bán vé trên TicketHub</p>
        </div>

        <!-- General Error Block -->
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
              <span class="text-danger text-[14px] font-bold">Lỗi đăng ký</span>
              <span class="text-danger/80 text-[13px] font-medium">{{ error }}</span>
            </div>
          </div>
        </Transition>

        <!-- Success Block -->
        <div v-if="isSuccess" class="bg-[#111916] border border-primary/20 rounded-[2rem] p-8 text-center flex flex-col items-center gap-6 shadow-2xl animate-fade-up">
          <div class="w-20 h-20 rounded-full bg-primary/10 border border-primary/20 flex items-center justify-center text-primary text-4xl shadow-[0_0_30px_rgba(0,200,83,0.15)] animate-bounce">
            <PhCheckCircle weight="fill" />
          </div>
          <div class="space-y-2">
            <h3 class="font-heading text-xl font-bold text-white">Đăng ký thành công!</h3>
            <p class="text-white/60 text-[14px] leading-relaxed">
              {{ successMessage }}
            </p>
          </div>
          <router-link
            to="/organizer/login"
            class="w-full bg-primary hover:bg-emerald-500 text-black font-black py-3.5 rounded-2xl transition-all hover:-translate-y-0.5 text-[14px] flex justify-center items-center gap-2 cursor-pointer"
          >
            Đăng nhập ngay
          </router-link>
        </div>

        <!-- Registration Form -->
        <form v-else @submit.prevent="handleRegister" class="flex flex-col gap-5">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <!-- Username -->
            <div class="flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="reg-username">Tên đăng nhập <span class="text-primary">*</span></label>
              <div class="relative flex items-center group">
                <PhUser class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  id="reg-username" type="text" v-model="form.userName" placeholder="VD: nxhevent" required :disabled="isLoading"
                  class="w-full bg-white/5 border border-white/10 rounded-2xl py-3.5 pl-12 pr-4 text-[14px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium"
                />
              </div>
              <span v-if="validationErrors.userName" class="text-xs text-danger font-bold flex items-center gap-1.5 mt-0.5 px-1">
                <PhXCircle weight="fill" /> {{ validationErrors.userName[0] }}
              </span>
            </div>

            <!-- Organizer Name -->
            <div class="flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="reg-orgname">Tên tổ chức <span class="text-primary">*</span></label>
              <div class="relative flex items-center group">
                <PhBriefcase class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  id="reg-orgname" type="text" v-model="form.organizerName" placeholder="VD: Công ty Sự kiện NXH" required :disabled="isLoading"
                  class="w-full bg-white/5 border border-white/10 rounded-2xl py-3.5 pl-12 pr-4 text-[14px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium"
                />
              </div>
              <span v-if="validationErrors.organizerName" class="text-xs text-danger font-bold flex items-center gap-1.5 mt-0.5 px-1">
                <PhXCircle weight="fill" /> {{ validationErrors.organizerName[0] }}
              </span>
            </div>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <!-- Email -->
            <div class="flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="reg-email">Email <span class="text-primary">*</span></label>
              <div class="relative flex items-center group">
                <PhEnvelope class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  id="reg-email" type="email" v-model="form.email" placeholder="email@domain.com" required :disabled="isLoading"
                  class="w-full bg-white/5 border border-white/10 rounded-2xl py-3.5 pl-12 pr-4 text-[14px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium"
                />
              </div>
              <span v-if="validationErrors.email" class="text-xs text-danger font-bold flex items-center gap-1.5 mt-0.5 px-1">
                <PhXCircle weight="fill" /> {{ validationErrors.email[0] }}
              </span>
            </div>

            <!-- Phone Number -->
            <div class="flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="reg-phone">Số điện thoại <span class="text-primary">*</span></label>
              <div class="relative flex items-center group">
                <PhPhone class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  id="reg-phone" type="tel" v-model="form.phoneNumber" placeholder="0988xxxxxx" required :disabled="isLoading"
                  class="w-full bg-white/5 border border-white/10 rounded-2xl py-3.5 pl-12 pr-4 text-[14px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium"
                />
              </div>
              <span v-if="validationErrors.phoneNumber" class="text-xs text-danger font-bold flex items-center gap-1.5 mt-0.5 px-1">
                <PhXCircle weight="fill" /> {{ validationErrors.phoneNumber[0] }}
              </span>
            </div>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <!-- Password -->
            <div class="flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="reg-password">Mật khẩu <span class="text-primary">*</span></label>
              <div class="relative flex items-center group">
                <PhLockKey class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  id="reg-password" :type="showPassword ? 'text' : 'password'" v-model="form.password" placeholder="••••••••" required :disabled="isLoading"
                  class="w-full bg-white/5 border border-white/10 rounded-2xl py-3.5 pl-12 pr-10 text-[14px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium tracking-wide"
                />
                <button type="button" @click="showPassword = !showPassword" tabindex="-1" class="absolute right-3.5 text-white/30 hover:text-white transition-colors cursor-pointer">
                  <PhEye v-if="!showPassword" class="text-base" weight="bold" />
                  <PhEyeClosed v-else class="text-base" weight="bold" />
                </button>
              </div>
              <span v-if="validationErrors.password" class="text-xs text-danger font-bold flex items-center gap-1.5 mt-0.5 px-1">
                <PhXCircle weight="fill" /> {{ validationErrors.password[0] }}
              </span>
            </div>

            <!-- Confirm Password -->
            <div class="flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest ml-1" for="reg-confirmpassword">Nhập lại mật khẩu <span class="text-primary">*</span></label>
              <div class="relative flex items-center group">
                <PhLockKey class="absolute left-4 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  id="reg-confirmpassword" :type="showConfirmPassword ? 'text' : 'password'" v-model="form.confirmPassword" placeholder="••••••••" required :disabled="isLoading"
                  class="w-full bg-white/5 border border-white/10 rounded-2xl py-3.5 pl-12 pr-10 text-[14px] font-bold text-white outline-none focus:border-primary focus:bg-white/10 transition-all placeholder:text-white/20 placeholder:font-medium tracking-wide"
                />
                <button type="button" @click="showConfirmPassword = !showConfirmPassword" tabindex="-1" class="absolute right-3.5 text-white/30 hover:text-white transition-colors cursor-pointer">
                  <PhEye v-if="!showConfirmPassword" class="text-base" weight="bold" />
                  <PhEyeClosed v-else class="text-base" weight="bold" />
                </button>
              </div>
              <span v-if="validationErrors.confirmPassword" class="text-xs text-danger font-bold flex items-center gap-1.5 mt-0.5 px-1">
                <PhXCircle weight="fill" /> {{ validationErrors.confirmPassword[0] }}
              </span>
            </div>
          </div>

          <button 
            type="submit" 
            :disabled="isLoading || !form.userName || !form.organizerName || !form.email || !form.phoneNumber || !form.password || !form.confirmPassword" 
            class="mt-4 w-full bg-primary hover:bg-emerald-500 text-black font-black py-4 rounded-2xl shadow-[0_0_30px_rgba(0,200,83,0.2)] hover:shadow-[0_0_55px_rgba(0,200,83,0.4)] transition-all hover:-translate-y-0.5 disabled:opacity-50 disabled:hover:translate-y-0 disabled:shadow-none flex justify-center items-center gap-2 cursor-pointer text-[14px]"
          >
            <PhCircleNotch v-if="isLoading" class="animate-spin text-lg" weight="bold" />
            <PhCheck v-else class="text-lg" weight="bold" />
            {{ isLoading ? 'Đang đăng ký...' : 'Đăng ký Tài khoản' }}
          </button>

          <!-- Back to Login / Home links -->
          <div class="mt-4 flex flex-col gap-3 items-center text-[13px] font-bold text-white/50">
            <div>
              Đã có tài khoản?
              <router-link to="/organizer/login" class="text-primary hover:underline ml-1">Đăng nhập</router-link>
            </div>
            <router-link to="/" class="inline-flex items-center gap-1.5 text-white/35 hover:text-white transition-colors mt-2 group">
              <PhArrowLeft class="group-hover:-translate-x-0.5 transition-transform" weight="bold" />
              Quay về trang chủ
            </router-link>
          </div>
        </form>
      </div>
    </div>

    <!-- Right: Brand / Benefits Panel -->
    <div class="relative hidden lg:flex flex-col justify-between p-12 bg-[#0A0F0D] overflow-hidden border-l border-white/5 order-1 lg:order-2">
      <!-- Decor lights -->
      <div class="absolute top-[-20%] left-[-10%] w-[80%] h-[80%] bg-primary/10 blur-[130px] rounded-full mix-blend-screen pointer-events-none"></div>
      <div class="absolute bottom-[-10%] right-[-20%] w-[60%] h-[60%] bg-[#00c853]/5 blur-[100px] rounded-full mix-blend-screen pointer-events-none"></div>

      <div class="relative z-10 flex items-center gap-3">
        <div class="w-10 h-10 bg-primary text-black rounded-xl flex items-center justify-center font-black text-xl shadow-[0_0_20px_rgba(0,200,83,0.25)]">
          <PhTicket weight="fill" />
        </div>
        <span class="font-heading font-black text-2xl tracking-tight uppercase">TicketHub Partner</span>
      </div>

      <div class="relative z-10 max-w-lg my-12">
        <h2 class="font-heading text-4xl xl:text-5xl font-black leading-[1.2] mb-8 tracking-tight">
          Nâng tầm thương hiệu sự kiện của bạn
        </h2>
        
        <!-- Benefits List -->
        <div class="flex flex-col gap-6">
          <div v-for="b in benefits" :key="b.title" class="flex gap-4 items-start bg-white/[0.02] border border-white/5 rounded-2.5xl p-5 hover:border-white/10 transition-colors">
            <div class="w-10 h-10 rounded-xl bg-primary/10 border border-primary/20 flex items-center justify-center text-primary text-xl flex-shrink-0">
              <component :is="b.icon" weight="fill" />
            </div>
            <div>
              <h4 class="text-white font-bold text-[15px] mb-1">{{ b.title }}</h4>
              <p class="text-white/40 text-[13px] leading-relaxed">{{ b.description }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="relative z-10 text-white/30 text-xs font-bold tracking-widest uppercase">
        © {{ new Date().getFullYear() }} TicketHub Platform
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, markRaw } from 'vue'
import { registerOrganizer } from '../../services/auth/auth.service'
import { 
  PhUser, PhBriefcase, PhEnvelope, PhPhone, PhLockKey, PhEye, PhEyeClosed, 
  PhArrowLeft, PhXCircle, PhWarningCircle, PhCheckCircle, PhCheck, PhCircleNotch,
  PhTicket, PhShieldCheck, PhChartPieSlice, PhCalendarCheck
} from '@phosphor-icons/vue'

const isLoading = ref(false)
const error = ref('')
const validationErrors = ref({})
const isSuccess = ref(false)
const successMessage = ref('')

const showPassword = ref(false)
const showConfirmPassword = ref(false)

const form = reactive({
  userName: '',
  organizerName: '',
  email: '',
  phoneNumber: '',
  password: '',
  confirmPassword: ''
})

const benefits = [
  { 
    title: 'Công cụ tổ chức toàn diện', 
    description: 'Tạo sự kiện, thiết lập phân khu và quản lý sơ đồ chỗ ngồi thông minh chỉ với vài bước đơn giản.', 
    icon: markRaw(PhCalendarCheck) 
  },
  { 
    title: 'Theo dõi chỉ số thông minh', 
    description: 'Thống kê doanh thu, số vé đã bán và lưu lượng truy cập trực quan theo thời gian thực.', 
    icon: markRaw(PhChartPieSlice) 
  },
  { 
    title: 'Bảo mật & Tin cậy tuyệt đối', 
    description: 'Hệ thống bảo vệ giao dịch vé và thông tin khách hàng an toàn theo chuẩn quốc tế.', 
    icon: markRaw(PhShieldCheck) 
  }
]

const handleRegister = async () => {
  if (form.password !== form.confirmPassword) {
    validationErrors.value = {
      confirmPassword: ['Mật khẩu nhập lại không khớp.']
    }
    return
  }

  isLoading.value = true
  error.value = ''
  validationErrors.value = {}
  
  try {
    const res = await registerOrganizer({
      userName: form.userName,
      organizerName: form.organizerName,
      email: form.email,
      phoneNumber: form.phoneNumber,
      password: form.password,
      confirmPassword: form.confirmPassword
    })
    
    if (res.success) {
      isSuccess.value = true
      successMessage.value = res.message || 'Đăng ký tài khoản tổ chức thành công, vui lòng kiểm tra email để xác nhận tài khoản.'
    } else {
      error.value = res.message || 'Đăng ký tài khoản thất bại.'
    }
  } catch (err) {
    const errData = err.response?.data
    if (errData?.errors) {
      if (Array.isArray(errData.errors)) {
        const normalized = {}
        errData.errors.forEach(item => {
          if (item && item.field) {
            if (!normalized[item.field]) {
              normalized[item.field] = []
            }
            normalized[item.field].push(item.message)
          }
        })
        validationErrors.value = normalized
      } else {
        validationErrors.value = { ...errData.errors }
      }
    } else {
      error.value = errData?.message || 'Đã xảy ra lỗi kết nối. Vui lòng thử lại.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>
