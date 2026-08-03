<template>
  <div class="fixed top-6 right-6 z-[1002] flex flex-col gap-3 pointer-events-none w-full max-w-[380px]">
    <TransitionGroup
      enter-active-class="transition-all duration-300 ease-out"
      enter-from-class="opacity-0 translate-x-8"
      enter-to-class="opacity-100 translate-x-0"
      leave-active-class="transition-all duration-200 ease-in absolute"
      leave-from-class="opacity-100 translate-x-0"
      leave-to-class="opacity-0 translate-x-8"
    >
      <div
        v-for="toast in notificationStore.toastQueue"
        :key="toast.toastId"
        @click="handleClick(toast)"
        class="pointer-events-auto flex gap-3 p-4 bg-[#0A0F0D]/95 backdrop-blur-xl border border-white/10 rounded-2xl shadow-[0_20px_50px_rgba(0,0,0,0.5)] cursor-pointer hover:border-primary/30 transition-colors"
      >
        <div
          class="w-10 h-10 flex-shrink-0 rounded-xl border flex items-center justify-center"
          :class="getNotificationPreset(toast.type).tone"
        >
          <component :is="getNotificationPreset(toast.type).icon" weight="fill" class="text-lg" />
        </div>
        <div class="flex-1 min-w-0">
          <h4 class="text-[13px] font-bold text-white leading-snug line-clamp-1">{{ toast.title }}</h4>
          <p class="text-[12px] text-muted leading-relaxed mt-0.5 line-clamp-2">{{ toast.message }}</p>
        </div>
        <button
          @click.stop="dismissToast(toast.toastId)"
          class="flex-shrink-0 w-6 h-6 rounded-lg flex items-center justify-center text-white/30 hover:text-white hover:bg-white/10 transition-all"
        >
          <PhX weight="bold" class="text-sm" />
        </button>
      </div>
    </TransitionGroup>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { PhX } from '@phosphor-icons/vue'
import { notificationStore, dismissToast, markRead } from '../../stores/notificationStore'
import { getNotificationPreset } from '../../utils/notificationDisplay'

const router = useRouter()

const handleClick = async (toast) => {
  dismissToast(toast.toastId)
  await markRead(toast.id)
  if (toast.linkUrl) router.push(toast.linkUrl)
}
</script>
