<template>
  <span
    class="inline-flex items-center justify-center px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider rounded-full border whitespace-nowrap transition-colors duration-200"
    :class="variantClasses"
  >
    <slot />
  </span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  variant: {
    type: String,
    default: 'default',
    validator: (v) => ['success', 'warning', 'danger', 'info', 'primary', 'default'].includes(v)
  }
})

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'bg-primary/10 text-primary border-primary/20 shadow-[inset_0_0_8px_rgba(0,200,83,0.05)]'
    case 'warning':
      return 'bg-warning/10 text-warning border-warning/20 shadow-[inset_0_0_8px_rgba(250,173,20,0.05)]'
    case 'danger':
      return 'bg-danger/10 text-danger border-danger/20 shadow-[inset_0_0_8px_rgba(255,71,87,0.05)]'
    case 'info':
      return 'bg-info/10 text-info border-info/20 shadow-[inset_0_0_8px_rgba(59,130,246,0.05)]'
    case 'primary':
      return 'bg-accent/10 text-accent border-accent/20 shadow-[inset_0_0_8px_rgba(185,255,106,0.05)]'
    case 'default':
    default:
      return 'bg-surface/80 text-muted border-border-light shadow-sm'
  }
})
</script>
