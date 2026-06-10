<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    @click="$emit('click', $event)"
  >
    <div v-if="loading" class="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent"></div>
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
  loading: Boolean,
  disabled: Boolean
})

defineEmits(['click'])

const buttonClasses = computed(() => {
  return [
    'inline-flex items-center justify-center rounded-full font-bold transition-all duration-300 active:scale-[0.98] disabled:opacity-50 disabled:pointer-events-none whitespace-nowrap outline-none focus-visible:ring-2 focus-visible:ring-primary/50',
    
    // Sizes
    props.size === 'sm' ? 'px-4 py-2 text-[13px] gap-1.5' : '',
    props.size === 'md' ? 'px-6 py-2.5 text-[14px] gap-2' : '',
    props.size === 'lg' ? 'px-8 py-3.5 text-[16px] gap-2.5' : '',
    
    // Variants
    props.variant === 'primary' ? 'bg-primary text-black hover:bg-[#00E05D] shadow-[0_4px_14px_rgba(0,200,83,0.1)] hover:shadow-[0_6px_20px_rgba(0,200,83,0.2)]' : '',
    props.variant === 'ghost' ? 'bg-transparent text-white/70 hover:bg-white/5 hover:text-white' : '',
    props.variant === 'outline' ? 'bg-transparent border border-white/10 text-white/80 hover:border-white/30 hover:bg-white/5 hover:text-white' : '',
    props.variant === 'danger' ? 'bg-danger/10 border border-danger/20 text-danger hover:bg-danger hover:text-white' : '',
    props.variant === 'glass' ? 'bg-white/5 backdrop-blur-md border border-white/10 text-white hover:bg-white/10' : ''
  ]
})
</script>
