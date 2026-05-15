<template>
  <div class="fixed inset-0 bg-black/80 backdrop-blur-sm z-[500] flex items-center justify-center p-4" @click="$emit('close')">
    <div class="bg-surface border border-border-main rounded-[32px] w-full max-w-[420px] max-h-[90vh] overflow-y-auto p-8 shadow-2xl animate-in zoom-in duration-300" @click.stop>
      <!-- Header -->
      <div class="flex justify-between items-center mb-6">
        <h2 class="font-heading text-2xl font-bold text-main">{{ isLogin ? 'Đăng nhập' : 'Đăng ký' }}</h2>
        <button class="w-8 h-8 flex items-center justify-center rounded-full bg-surface/50 border border-border-main text-muted hover:text-danger hover:border-danger transition-all" @click="$emit('close')">✕</button>
      </div>

      <!-- Tabs -->
      <div class="flex border-b border-border-main mb-8">
        <button 
          v-for="mode in ['login', 'register']" 
          :key="mode"
          @click="setMode(mode)"
          class="flex-1 py-3 font-bold transition-all border-b-2"
          :class="[
            (mode === 'login' && isLogin) || (mode === 'register' && !isLogin)
              ? 'text-primary border-primary'
              : 'text-muted border-transparent hover:text-main'
          ]"
        >
          {{ mode === 'login' ? 'Đăng nhập' : 'Đăng ký' }}
        </button>
      </div> 

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="flex flex-col gap-5">
        <div v-if="!isLogin" class="flex flex-col gap-1.5">
          <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Họ và tên</label>
          <input 
            type="text" 
            class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" 
            v-model="form.name" 
            placeholder="Nhập họ và tên" 
            required 
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Email / Số điện thoại</label>
          <input 
            type="text" 
            class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" 
            v-model="form.email" 
            placeholder="Nhập email hoặc số điện thoại" 
            required 
          />
        </div>

        <div class="flex flex-col gap-1.5">
          <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Mật khẩu</label>
          <input 
            type="password" 
            class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" 
            v-model="form.password" 
            placeholder="Nhập mật khẩu" 
            required 
          />
        </div>

        <div v-if="!isLogin" class="flex flex-col gap-1.5">
          <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Xác nhận mật khẩu</label>
          <input 
            type="password" 
            class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" 
            v-model="form.confirmPassword" 
            placeholder="Nhập lại mật khẩu" 
            required 
          />
          <div v-if="passwordMismatch" class="text-danger text-[12px] font-medium ml-1">Mật khẩu không khớp</div>
        </div>

        <div v-if="isLogin" class="text-right">
          <a href="#" class="text-[13px] font-bold text-primary hover:underline transition-all">Quên mật khẩu?</a>
        </div>

        <BaseButton 
          type="submit" 
          variant="primary" 
          size="lg"
          class="w-full !py-4 mt-2 !rounded-2xl shadow-xl shadow-primary/20"
          :disabled="isLoading || passwordMismatch"
          :loading="isLoading"
        >
          {{ isLogin ? 'Đăng nhập' : 'Đăng ký' }}
        </BaseButton>
      </form>

      <!-- Social Login -->
      <div class="mt-10">
        <div class="relative text-center mb-6">
          <div class="absolute inset-0 flex items-center">
            <div class="w-full border-t border-border-main"></div>
          </div>
          <span class="relative bg-surface px-4 text-[13px] text-muted font-medium">Hoặc tiếp tục với</span>
        </div>

        <div class="flex gap-4 justify-center">
          <button class="w-12 h-12 rounded-full border border-border-main bg-card flex items-center justify-center text-xl text-[#DB4437] hover:border-[#DB4437] hover:-translate-y-1 transition-all shadow-lg hover:shadow-[#DB4437]/20">
            G
          </button>
          <button class="w-12 h-12 rounded-full border border-border-main bg-card flex items-center justify-center text-xl text-[#4267B2] hover:border-[#4267B2] hover:-translate-y-1 transition-all shadow-lg hover:shadow-[#4267B2]/20">
            f
          </button>
          <button class="w-12 h-12 rounded-full border border-border-main bg-card flex items-center justify-center text-xl text-white hover:border-white hover:-translate-y-1 transition-all shadow-lg hover:shadow-white/20">
            
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { store, login, register, isLoading } from '../stores/eventStore'
import BaseButton from './ui/BaseButton.vue'

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
  // Reset form
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
</script>

