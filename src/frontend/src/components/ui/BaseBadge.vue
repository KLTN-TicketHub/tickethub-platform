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
    // Base styles
    'inline-flex items-center justify-center font-bold font-sans uppercase tracking-wider rounded-full',
    
    // Sizes
    props.size === 'sm' ? 'px-2 py-0.5 text-[10px]' : '',
    props.size === 'md' ? 'px-3 py-1 text-xs' : '',
    
    // Variants
    props.variant === 'primary' ? 'bg-primary-dim text-primary' : '',
    props.variant === 'success' ? 'bg-primary-dim text-primary border border-primary/20' : '', // Similar to primary but with border
    props.variant === 'warning' ? 'bg-gold/15 text-gold border border-gold/30' : '',
    props.variant === 'danger' ? 'bg-danger/15 text-danger border border-danger/30' : '',
    props.variant === 'neutral' ? 'bg-card text-muted border border-border-main' : ''
  ]
})
</script>
