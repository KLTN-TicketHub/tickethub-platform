<template>
  <div class="flex flex-col gap-1.5 w-full">
    <label v-if="label" :for="id" class="text-[13px] font-medium text-muted tracking-wide">
      {{ label }}
    </label>
    
    <div class="relative">
      <select
        :id="id"
        :value="modelValue"
        :disabled="disabled"
        :class="selectClasses"
        @change="$emit('update:modelValue', $event.target.value)"
      >
        <option value="" disabled hidden>{{ placeholder }}</option>
        <option 
          v-for="option in options" 
          :key="option.value" 
          :value="option.value"
        >
          {{ option.label }}
        </option>
      </select>
      
      <!-- Custom Chevron Icon to override default select arrow -->
      <div class="absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none text-muted">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="m6 9 6 6 6-6"/>
        </svg>
      </div>
    </div>
    
    <span v-if="error" class="text-xs text-danger mt-1">
      {{ error }}
    </span>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: ''
  },
  options: {
    type: Array,
    required: true,
    // Expected format: [{ label: 'Option 1', value: 'opt1' }]
  },
  label: {
    type: String,
    default: ''
  },
  id: {
    type: String,
    default: () => `select-${Math.random().toString(36).substring(2, 9)}`
  },
  placeholder: {
    type: String,
    default: 'Select an option'
  },
  disabled: {
    type: Boolean,
    default: false
  },
  error: {
    type: String,
    default: ''
  }
})

defineEmits(['update:modelValue'])

const selectClasses = computed(() => {
  return [
    // Base styles (appearance-none hides the default browser arrow)
    'w-full appearance-none bg-card border rounded-xl px-4 py-2.5 pr-10 text-main font-sans text-[15px] outline-none transition-colors duration-200 cursor-pointer',
    'disabled:opacity-50 disabled:cursor-not-allowed',
    
    // State styles
    props.error 
      ? 'border-danger focus:border-danger focus:ring-1 focus:ring-danger' 
      : 'border-border-main focus:border-primary focus:bg-card-2'
  ]
})
</script>

<style scoped>
/* Ensure options text color works across all browsers */
select option {
  background-color: var(--color-surface);
  color: var(--color-main);
}
</style>
