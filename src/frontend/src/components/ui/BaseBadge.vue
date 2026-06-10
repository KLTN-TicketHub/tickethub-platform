<template>
  <span :class="badgeClasses">
    <slot></slot>
  </span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  variant: {
    type: String,
    default: 'primary',
    validator: (v) => ['primary', 'success', 'warning', 'danger', 'neutral'].includes(v)
  },
  size: {
    type: String,
    default: 'md',
    validator: (v) => ['sm', 'md'].includes(v)
  }
})

const badgeClasses = computed(() => {
  return [
    'inline-flex items-center justify-center font-bold font-sans uppercase tracking-[0.1em] rounded-full border',
    
    // Sizes
    props.size === 'sm' ? 'px-2 py-0.5 text-[9px]' : '',
    props.size === 'md' ? 'px-3 py-1 text-[11px]' : '',
    
    // Variants
    props.variant === 'primary' ? 'bg-primary/10 text-primary border-primary/20' : '',
    props.variant === 'success' ? 'bg-[#00E05D]/10 text-[#00E05D] border-[#00E05D]/20' : '',
    props.variant === 'warning' ? 'bg-yellow-500/10 text-yellow-500 border-yellow-500/20' : '',
    props.variant === 'danger' ? 'bg-red-500/10 text-red-500 border-red-500/20' : '',
    props.variant === 'neutral' ? 'bg-white/5 text-white/60 border-white/10' : ''
  ]
})
</script>
