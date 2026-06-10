<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-all duration-500 ease-[cubic-bezier(0.23,1,0.32,1)]"
      enter-from-class="opacity-0 scale-95 translate-y-4"
      enter-to-class="opacity-100 scale-100 translate-y-0"
      leave-active-class="transition-all duration-300 ease-in"
      leave-from-class="opacity-100 scale-100 translate-y-0"
      leave-to-class="opacity-0 scale-95 translate-y-4"
    >
      <div v-if="show" class="fixed inset-0 z-[1000] flex items-center justify-center p-4 sm:p-6" @mousedown.self="$emit('close')">
        <div class="absolute inset-0 bg-[#0A0F0D]/80 backdrop-blur-md transition-opacity"></div>

        <div 
          class="relative w-full overflow-hidden bg-[#111916] border border-white/10 rounded-[2rem] shadow-[0_40px_80px_rgba(0,0,0,0.8)] flex flex-col"
          :class="[maxWidthClass]"
        >
          <div class="flex items-center justify-between p-6 px-8 border-b border-white/5 bg-white/[0.02]">
            <div class="flex flex-col">
              <h2 class="text-2xl font-black font-heading text-white leading-tight">{{ title }}</h2>
              <p v-if="subtitle" class="text-[13px] font-medium text-white/50 mt-1">{{ subtitle }}</p>
            </div>
            <button 
              @click="$emit('close')"
              class="w-10 h-10 rounded-full flex items-center justify-center bg-white/5 border border-white/10 text-white/50 hover:text-white hover:bg-white/10 hover:border-white/20 transition-all group active:scale-90"
            >
              <PhX weight="bold" class="text-lg group-hover:rotate-90 transition-transform" />
            </button>
          </div>

          <div class="flex-1 overflow-y-auto max-h-[80vh] scroll-smooth p-6 px-8 hide-scroll">
            <slot />
          </div>

          <div v-if="$slots.footer" class="p-6 px-8 border-t border-white/5 bg-white/[0.02] flex items-center justify-end gap-4">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { computed } from 'vue'
import { PhX } from '@phosphor-icons/vue'

const props = defineProps({
  show: Boolean,
  title: String,
  subtitle: String,
  size: { type: String, default: 'md' }
})

defineEmits(['close'])

const maxWidthClass = computed(() => {
  switch (props.size) {
    case 'sm': return 'max-w-md'
    case 'lg': return 'max-w-3xl'
    case 'xl': return 'max-w-5xl'
    default: return 'max-w-xl'
  }
})
</script>

<style scoped>
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
