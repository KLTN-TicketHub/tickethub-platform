<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-all duration-500 ease-[cubic-bezier(0.23,1,0.32,1)]"
      enter-from-class="opacity-0 scale-95 backdrop-blur-0"
      enter-to-class="opacity-100 scale-100 backdrop-blur-xl"
      leave-active-class="transition-all duration-300 ease-in"
      leave-from-class="opacity-100 scale-100 backdrop-blur-xl"
      leave-to-class="opacity-0 scale-95 backdrop-blur-0"
    >
      <div v-if="show" class="fixed inset-0 z-[1000] flex items-center justify-center p-4 sm:p-6" @mousedown.self="$emit('close')">
        <!-- Backdrop Backdrop -->
        <div class="absolute inset-0 bg-black/60 backdrop-blur-sm"></div>

        <!-- Modal Content -->
        <div 
          class="relative w-full overflow-hidden bg-card border border-border-main rounded-[3rem] shadow-[0_50px_100px_-20px_rgba(0,0,0,0.8)] flex flex-col"
          :class="[maxWidthClass]"
        >
          <!-- Header -->
          <div class="flex items-center justify-between p-6 px-10 border-b border-border-main bg-card/50">
            <div class="flex flex-col">
              <h2 class="text-2xl font-bold font-heading text-main leading-tight">{{ title }}</h2>
              <p v-if="subtitle" class="text-[13px] font-medium text-muted mt-0.5">{{ subtitle }}</p>
            </div>
            <button 
              @click="$emit('close')"
              class="w-10 h-10 rounded-full flex items-center justify-center bg-surface border border-border-main text-muted hover:text-main hover:border-primary transition-all group"
            >
              <span class="text-xl group-active:scale-90 transition-transform">✕</span>
            </button>
          </div>

          <!-- Body -->
          <div class="flex-1 overflow-y-auto max-h-[80vh] scroll-smooth p-6 px-10">
            <slot />
          </div>

          <!-- Footer -->
          <div v-if="$slots.footer" class="p-6 px-10 border-t border-border-main bg-card/30 flex items-center justify-end gap-4">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  show: Boolean,
  title: String,
  subtitle: String,
  size: {
    type: String,
    default: 'md' // sm, md, lg, xl
  }
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
