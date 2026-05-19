<template>
  <section class="min-h-screen flex items-center justify-center bg-surface px-6">
    <div class="max-w-md w-full rounded-[28px] border border-border-main bg-card p-8 shadow-2xl text-center animate-fade-up">
      <div class="mx-auto mb-5 relative h-16 w-16 flex items-center justify-center">
        <div class="absolute inset-0 rounded-full border-4 border-primary/15"></div>
        <div class="absolute inset-0 rounded-full border-4 border-transparent border-t-primary border-r-primary animate-spin"></div>
        <div class="h-6 w-6 rounded-full bg-primary/15 flex items-center justify-center">
          <div class="h-2.5 w-2.5 rounded-full bg-primary animate-pulse"></div>
        </div>
      </div>
      <h1 class="text-2xl font-bold text-main mb-2">Đang hoàn tất đăng nhập</h1>
      <p class="text-sm text-muted">
        Vui lòng chờ một chút trong khi hệ thống xác nhận tài khoản Google của bạn.
      </p>
      <p v-if="errorMessage" class="mt-4 text-sm text-danger font-medium">
        {{ errorMessage }}
      </p>
    </div>
  </section>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { handleGooglePopupCallback } from '../services/auth/auth.service'

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