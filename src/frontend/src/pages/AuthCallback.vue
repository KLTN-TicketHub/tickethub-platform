<template>
  <section class="min-h-screen flex items-center justify-center bg-[#0A0F0D] px-6 relative overflow-hidden">
    <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_center,_var(--tw-gradient-stops))] from-primary/10 via-transparent to-transparent opacity-60"></div>

    <div class="max-w-md w-full rounded-[2.5rem] border border-white/5 bg-[#111916]/80 backdrop-blur-xl p-10 shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] text-center animate-fade-up relative z-10">
      <div class="mx-auto mb-8 relative h-20 w-20 flex items-center justify-center">
        <div class="absolute inset-0 rounded-full border-4 border-white/5"></div>
        <div class="absolute inset-0 rounded-full border-4 border-transparent border-t-primary border-r-primary animate-spin"></div>
        <div class="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center text-primary text-xl">
          <PhGoogleLogo weight="fill" class="animate-pulse" />
        </div>
      </div>
      
      <h1 class="text-3xl font-black font-heading text-white tracking-tight mb-3">Đang xác thực</h1>
      <p class="text-[15px] font-medium text-white/50 leading-relaxed max-w-xs mx-auto">
        Vui lòng chờ một chút trong khi hệ thống xác nhận tài khoản Google của bạn.
      </p>
      
      <p v-if="errorMessage" class="mt-6 p-4 rounded-xl bg-danger/10 border border-danger/20 text-[14px] text-danger font-bold flex items-center justify-center gap-2">
        <PhWarningCircle weight="fill" /> {{ errorMessage }}
      </p>
    </div>
  </section>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { handleGooglePopupCallback } from '../services/auth/auth.service'
import { PhGoogleLogo, PhWarningCircle } from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()
const errorMessage = ref('')

const finishFlow = async () => {
  try {
    await handleGooglePopupCallback()
    const next = typeof route.query.next === 'string' && route.query.next ? route.query.next : '/'
    if (window.opener && window.opener !== window) {
      window.close()
      return
    }
    await router.replace(next)
  } catch (error) {
    errorMessage.value = 'Không thể hoàn tất đăng nhập Google. Vui lòng thử lại.'
    if (window.opener && window.opener !== window) {
      window.opener.postMessage({ type: 'ticket-hub:auth-error' }, window.location.origin)
      window.close()
    }
  }
}

onMounted(() => {
  finishFlow()
})
</script>