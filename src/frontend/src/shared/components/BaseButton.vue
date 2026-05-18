<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    @click="$emit('click', $event)"
  >
    <span v-if="loading" class="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent"></span>
    <slot v-else name="icon-left"></slot>
    <slot></slot>
    <slot name="icon-right"></slot>
  </button>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  variant: {
    type: String,
    default: 'primary',
    validator: (v) => ['primary', 'ghost', 'outline', 'danger', 'glass'].includes(v)
  },
  size: {
    type: String,
    default: 'md',
    validator: (v) => ['sm', 'md', 'lg'].includes(v)
  },
  loading: {
    type: Boolean,
    default: false
  },
  disabled: {
    type: Boolean,
    default: false
  }
})

defineEmits(['click'])

const buttonClasses = computed(() => {
  return [
    // Base styles
    'inline-flex items-center justify-center rounded-xl font-sans font-medium transition-all duration-300 active:scale-95 disabled:opacity-50 disabled:pointer-events-none whitespace-nowrap',
    
    // Sizes
    props.size === 'sm' ? 'px-3 py-1.5 text-xs gap-1.5' : '',
    props.size === 'md' ? 'px-4 py-2 text-sm gap-2' : '',
    props.size === 'lg' ? 'px-6 py-3 text-base gap-2.5' : '',
    
    // Variants
    props.variant === 'primary' ? 'bg-primary text-black font-semibold hover:bg-[#00E05D] shadow-[0_0_15px_rgba(0,200,83,0.2)] hover:shadow-[0_0_25px_rgba(0,200,83,0.4)] hover:-translate-y-0.5' : '',
    props.variant === 'ghost' ? 'bg-transparent text-main hover:bg-white/5 hover:text-primary' : '',
    props.variant === 'outline' ? 'bg-transparent border border-white/10 text-main hover:border-primary/50 hover:bg-primary/5 hover:text-primary hover:-translate-y-0.5' : '',
    props.variant === 'danger' ? 'bg-danger text-white hover:bg-danger/90 hover:shadow-[0_0_20px_rgba(255,71,87,0.3)] hover:-translate-y-0.5' : '',
    props.variant === 'glass' ? 'glass hover:bg-white/10 text-white hover:shadow-[0_0_20px_rgba(255,255,255,0.1)] hover:-translate-y-0.5' : ''
  ]
})
</script>
