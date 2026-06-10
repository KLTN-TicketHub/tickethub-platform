<template>
  <Transition name="modal">
    <div class="fixed inset-0 z-[1000] flex items-center justify-center p-4">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-[#050807]/80 backdrop-blur-md transition-opacity duration-300" @click="$emit('close')"></div>
      
      <!-- Modal Content -->
      <div class="relative bg-[#0A0F0D] border border-white/10 rounded-[2.5rem] w-full max-w-[440px] shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] overflow-hidden transition-all duration-300 modal-content" @click.stop>
        
        <!-- Decoration -->
        <div class="absolute -top-32 -right-32 w-64 h-64 bg-primary/20 blur-[100px] rounded-full pointer-events-none"></div>

        <div class="p-8 lg:p-10 relative z-10">
          <!-- Header -->
          <div class="flex justify-between items-start mb-8">
            <div>
              <h2 class="text-3xl font-black font-heading text-white tracking-tight leading-none mb-2">
                {{ isLogin ? 'Đăng nhập' : 'Tạo tài khoản' }}
              </h2>
              <p class="text-[14px] text-white/50 font-medium">
                {{ isLogin ? 'Chào mừng trở lại TicketHub' : 'Khởi đầu trải nghiệm tuyệt vời' }}
              </p>
            </div>
            <button class="w-10 h-10 flex items-center justify-center rounded-full bg-white/5 hover:bg-white/10 border border-white/5 text-white/50 hover:text-white transition-all cursor-pointer" @click="$emit('close')">
              <PhX weight="bold" />
            </button>
          </div>

          <!-- Tabs -->
          <div class="flex gap-2 p-1.5 bg-[#111916] rounded-2xl border border-white/5 mb-8">
            <button 
              v-for="mode in ['login', 'register']" 
              :key="mode"
              @click="setMode(mode)"
              class="flex-1 py-3 px-4 rounded-xl text-[14px] font-bold transition-all"
              :class="[
                (mode === 'login' && isLogin) || (mode === 'register' && !isLogin)
                  ? 'bg-[#0A0F0D] text-white shadow-sm border border-white/5'
                  : 'text-white/40 hover:text-white/80'
              ]"
            >
              {{ mode === 'login' ? 'Đăng nhập' : 'Đăng ký' }}
            </button>
          </div> 

          <!-- Form -->
          <form @submit.prevent="handleSubmit" class="flex flex-col gap-5">
            <!-- Animated Form Group: Name -->
            <div v-show="!isLogin" class="form-group transition-all duration-300" :class="!isLogin ? 'opacity-100 max-h-24' : 'opacity-0 max-h-0 overflow-hidden'">
              <div class="relative group">
                <PhUser class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  type="text" 
                  class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 focus:bg-[#111916] transition-all placeholder:text-white/20 shadow-inner" 
                  v-model="form.name" 
                  placeholder="Họ và tên" 
                  :required="!isLogin" 
                />
              </div>
            </div>

            <!-- Email -->
            <div class="relative group">
              <PhEnvelopeSimple class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input 
                type="text" 
                class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 focus:bg-[#111916] transition-all placeholder:text-white/20 shadow-inner" 
                v-model="form.email" 
                placeholder="Email hoặc số điện thoại" 
                required 
              />
            </div>

            <!-- Password -->
            <div class="relative group">
              <PhLockKey class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input 
                type="password" 
                class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 focus:bg-[#111916] transition-all placeholder:text-white/20 shadow-inner" 
                v-model="form.password" 
                placeholder="Mật khẩu" 
                required 
              />
            </div>

            <!-- Confirm Password -->
            <div v-show="!isLogin" class="form-group transition-all duration-300" :class="!isLogin ? 'opacity-100 max-h-24' : 'opacity-0 max-h-0 overflow-hidden'">
              <div class="relative group">
                <PhCheckCircle class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input 
                  type="password" 
                  class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 focus:bg-[#111916] transition-all placeholder:text-white/20 shadow-inner" 
                  :class="passwordMismatch ? '!border-danger focus:!border-danger' : ''"
                  v-model="form.confirmPassword" 
                  placeholder="Xác nhận mật khẩu" 
                  :required="!isLogin" 
                />
              </div>
              <div v-if="passwordMismatch" class="text-danger text-[12px] font-medium ml-1 mt-1.5 flex items-center gap-1">
                <PhWarningCircle weight="fill" /> Mật khẩu không khớp
              </div>
            </div>

            <div v-if="isLogin" class="flex justify-between items-center px-1">
              <label class="flex items-center gap-2 cursor-pointer group">
                <div class="w-5 h-5 rounded-[6px] border border-white/10 bg-[#111916] flex items-center justify-center group-hover:border-primary/50 transition-colors">
                  <PhCheck weight="bold" class="text-primary opacity-0" />
                </div>
                <span class="text-[13px] text-white/50 font-medium group-hover:text-white/80 transition-colors">Ghi nhớ tôi</span>
              </label>
              <a href="#" class="text-[13px] font-bold text-primary hover:text-[#00A355] transition-colors">Quên mật khẩu?</a>
            </div>

            <BaseButton 
              type="submit" 
              variant="primary" 
              size="lg"
              class="w-full !py-4 mt-2 !rounded-2xl shadow-[0_0_30px_rgba(0,200,83,0.15)] hover:shadow-[0_0_40px_rgba(0,200,83,0.3)] text-[15px]"
              :disabled="isLoading || passwordMismatch"
              :loading="isLoading"
            >
              {{ isLogin ? 'Đăng nhập ngay' : 'Tạo tài khoản mới' }}
            </BaseButton>
          </form>

          <!-- Social Login -->
          <div class="mt-8 pt-8 border-t border-white/5">
            <div class="text-center mb-6">
              <span class="text-[12px] text-white/40 font-bold uppercase tracking-widest bg-[#0A0F0D] px-4">Hoặc tiếp tục với</span>
            </div>

            <div class="grid grid-cols-3 gap-4">
              <button type="button" @click="handleGoogleLogin" class="h-14 rounded-2xl border border-white/5 bg-[#111916] flex items-center justify-center text-2xl text-white hover:bg-white hover:text-black hover:border-white transition-all cursor-pointer">
                <PhGoogleLogo weight="fill" />
              </button>
              <button type="button" class="h-14 rounded-2xl border border-white/5 bg-[#111916] flex items-center justify-center text-2xl text-[#1877F2] hover:bg-[#1877F2] hover:text-white hover:border-[#1877F2] transition-all cursor-pointer">
                <PhFacebookLogo weight="fill" />
              </button>
              <button type="button" class="h-14 rounded-2xl border border-white/5 bg-[#111916] flex items-center justify-center text-2xl text-white hover:bg-white hover:text-black hover:border-white transition-all cursor-pointer">
                <PhAppleLogo weight="fill" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { ref, computed } from 'vue'
import { store, login, register, isLoading } from '../stores/eventStore'
import { openGooglePopup } from '../services/auth/auth.service'
import BaseButton from './ui/BaseButton.vue'
import { 
  PhX, PhUser, PhEnvelopeSimple, PhLockKey, 
  PhCheckCircle, PhWarningCircle, PhCheck, 
  PhGoogleLogo, PhFacebookLogo, PhAppleLogo 
} from '@phosphor-icons/vue'

const emit = defineEmits(['close'])

const isLogin = computed(() => store.authMode === 'login')

const form = ref({
  name: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const passwordMismatch = computed(() => {
  if (isLogin.value) return false
  if (!form.value.confirmPassword) return false
  return form.value.password !== form.value.confirmPassword
})

const setMode = (mode) => {
  store.authMode = mode
  form.value = { name: '', email: '', password: '', confirmPassword: '' }
}

const handleSubmit = async () => {
  if (passwordMismatch.value) return
  
  if (isLogin.value) {
    await login(form.value.email, form.value.password)
  } else {
    await register(form.value.name, form.value.email, form.value.password)
  }
}

const handleGoogleLogin = () => {
  openGooglePopup(window.location.href)
}
</script>

<style scoped>
/* Modal Transition */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .modal-content {
  animation: modal-pop 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.modal-leave-active .modal-content {
  animation: modal-pop-out 0.3s cubic-bezier(0.5, 0, 0.5, 1) forwards;
}

@keyframes modal-pop {
  0% { opacity: 0; transform: scale(0.95) translateY(10px); }
  100% { opacity: 1; transform: scale(1) translateY(0); }
}

@keyframes modal-pop-out {
  0% { opacity: 1; transform: scale(1) translateY(0); }
  100% { opacity: 0; transform: scale(0.95) translateY(10px); }
}
</style>
