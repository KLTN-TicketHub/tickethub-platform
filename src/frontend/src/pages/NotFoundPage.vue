<template>
  <div class="min-h-[80vh] flex items-center justify-center py-20 px-4 animate-fade-up">
    <div class="max-w-2xl w-full text-center space-y-10 relative">
      <!-- Decor Background -->
      <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-primary/20 blur-[120px] rounded-full pointer-events-none"></div>

      <div class="w-28 h-28 mx-auto bg-[#111916] border border-white/5 rounded-[2rem] flex items-center justify-center text-6xl text-primary shadow-inner relative z-10 group hover:scale-110 transition-transform">
        <PhMagnifyingGlass weight="duotone" class="group-hover:rotate-12 transition-transform duration-500" />
      </div>

      <div class="space-y-4 relative z-10">
        <h1 class="font-heading text-8xl font-black text-white tracking-tighter">404</h1>
        <h2 class="text-3xl font-black text-white">Không tìm thấy trang</h2>
        <p class="text-lg text-white/50 font-medium max-w-md mx-auto">
          Trang bạn đang tìm kiếm có thể đã bị xóa, thay đổi đường dẫn hoặc tạm thời không khả dụng.
        </p>
      </div>

      <div class="flex flex-col sm:flex-row items-center justify-center gap-4 pt-4 relative z-10">
        <BaseButton variant="primary" size="lg" class="w-full sm:w-auto !px-10 !rounded-2xl flex items-center justify-center gap-2" @click="goHome">
          <PhHouse weight="fill" /> Về trang chủ
        </BaseButton>
        <BaseButton variant="ghost" size="lg" class="w-full sm:w-auto !px-10 !rounded-2xl border border-white/10 hover:border-white/20 hover:bg-white/5 flex items-center justify-center gap-2" @click="router.back()">
          <PhArrowLeft weight="bold" /> Quay lại
        </BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { PhMagnifyingGlass, PhHouse, PhArrowLeft } from '@phosphor-icons/vue'

const router = useRouter()

const homePath = computed(() => {
  if (!store.user) return '/'
  const roles = store.user.roles || []
  if (roles.some(r => r.toLowerCase() === 'admin')) return '/admin/dashboard'
  if (roles.some(r => r.toLowerCase() === 'moderator')) return '/moderator/dashboard'
  if (roles.some(r => r.toLowerCase() === 'organizer')) return '/organizer'
  if (roles.some(r => r.toLowerCase() === 'staff')) return '/staff/dashboard'
  return '/'
})

const goHome = () => {
  router.push(homePath.value)
}
</script>
