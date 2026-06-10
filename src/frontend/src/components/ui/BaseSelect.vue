<template>
  <div class="flex flex-col gap-2 w-full">
    <label v-if="label" :for="id" class="text-[12px] font-bold text-white/50 uppercase tracking-widest">
      {{ label }}
    </label>
    
    <div class="relative group">
      <select
        :id="id"
        :value="modelValue"
        :disabled="disabled"
        :class="selectClasses"
        @change="$emit('update:modelValue', $event.target.value)"
      >
        <option value="" disabled hidden>{{ placeholder }}</option>
        <option v-for="option in options" :key="option.value" :value="option.value" class="bg-[#111916] text-white">
          {{ option.label }}
        </option>
      </select>
      
      <div class="absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none text-white/40 group-hover:text-white transition-colors">
        <PhCaretDown weight="bold" />
      </div>
    </div>
    
    <span v-if="error" class="text-[12px] font-medium text-danger mt-1">
      {{ error }}
    </span>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { PhCaretDown } from '@phosphor-icons/vue'

const props = defineProps({
  modelValue: { type: [String, Number], default: '' },
  options: { type: Array, required: true },
  label: { type: String, default: '' },
  id: { type: String, default: () => `select-${Math.random().toString(36).substring(2, 9)}` },
  placeholder: { type: String, default: 'Select an option' },
  disabled: Boolean,
  error: String
})

defineEmits(['update:modelValue'])

const selectClasses = computed(() => {
  return [
    'w-full appearance-none rounded-full px-5 py-2.5 pr-10 text-[14px] font-bold outline-none transition-all duration-300 cursor-pointer',
    'disabled:opacity-50 disabled:cursor-not-allowed',
    props.error 
      ? 'bg-danger/5 border border-danger/50 text-danger focus:ring-2 focus:ring-danger/20' 
      : 'bg-white/5 border border-white/10 text-white hover:border-white/20 hover:bg-white/10 focus:border-primary/50 focus:bg-white/10'
  ]
})
</script>
