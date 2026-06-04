<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-300 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-200 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="visible"
        class="fixed inset-0 z-[100] flex items-center justify-center p-4 sm:p-6"
      >
        <!-- Backdrop -->
        <div
          class="absolute inset-0 glass-modal"
          @click="handleClose"
          data-testid="modal-backdrop"
        />

        <!-- Modal Container -->
        <Transition
          enter-active-class="transition-all duration-300 ease-[cubic-bezier(0.34,1.56,0.64,1)]"
          enter-from-class="opacity-0 scale-90 translate-y-4"
          enter-to-class="opacity-100 scale-100 translate-y-0"
          leave-active-class="transition-all duration-200 ease-in"
          leave-from-class="opacity-100 scale-100"
          leave-to-class="opacity-0 scale-95"
          appear
        >
          <div
            v-if="visible"
            class="glass-panel relative w-full max-h-[90vh] flex flex-col overflow-hidden"
            :class="maxWidthClass"
            role="dialog"
            aria-modal="true"
            @click.stop
            data-testid="base-modal"
          >
            <!-- Close Button -->
            <BaseButton
              variant="icon"
              size="sm"
              class="absolute top-4 right-4 z-10"
              @click="handleClose"
              aria-label="Close dialog"
              data-testid="btn-close-modal"
            >
              <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <line x1="18" y1="6" x2="6" y2="18" />
                <line x1="6" y1="6" x2="18" y2="18" />
              </svg>
            </BaseButton>

            <!-- Header -->
            <div v-if="$slots.header" class="px-6 py-5 border-b border-border-main/50 shrink-0">
              <slot name="header" />
            </div>

            <!-- Body (Scrollable) -->
            <div class="p-6 overflow-y-auto overflow-x-hidden no-scrollbar">
              <slot />
            </div>

            <!-- Footer -->
            <div v-if="$slots.footer" class="px-6 py-4 border-t border-border-main/50 bg-bg-elevated/50 shrink-0">
              <slot name="footer" />
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { computed } from 'vue'
import BaseButton from '@/shared/components/BaseButton.vue'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  maxWidth: {
    type: String,
    default: 'md',
    validator: (v) => ['sm', 'md', 'lg', 'xl', '2xl', 'full'].includes(v)
  }
})

const emit = defineEmits(['close'])

const handleClose = () => {
  emit('close')
}

const maxWidthClass = computed(() => {
  switch (props.maxWidth) {
    case 'sm': return 'max-w-sm'
    case 'lg': return 'max-w-lg'
    case 'xl': return 'max-w-xl'
    case '2xl': return 'max-w-2xl'
    case 'full': return 'max-w-[95vw]'
    case 'md':
    default: return 'max-w-md'
  }
})
</script>
