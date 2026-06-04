<template>
  <div class="flex flex-col gap-1.5 w-full">
    <!-- Label -->
    <label
      v-if="label"
      :for="selectId"
      class="text-sm font-medium text-muted pl-0.5 transition-colors duration-200"
      :class="{ '!text-danger': error }"
    >
      {{ label }}
    </label>

    <div class="relative group">
      <!-- Custom Dropdown Icon (since native select styling is limited) -->
      <div class="absolute right-3.5 top-1/2 -translate-y-1/2 pointer-events-none text-muted group-focus-within:text-primary transition-colors duration-200">
        <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <polyline points="6 9 12 15 18 9" />
        </svg>
      </div>

      <!-- Native Select -->
      <select
        :id="selectId"
        v-model="model"
        v-bind="$attrs"
        class="appearance-none"
        :class="selectClasses"
        :aria-invalid="!!error"
        :aria-describedby="error ? errorId : undefined"
        data-testid="base-select"
      >
        <option value="" disabled selected hidden>Select an option...</option>
        <option
          v-for="option in options"
          :key="option.value"
          :value="option.value"
          class="bg-bg text-main"
        >
          {{ option.label }}
        </option>
      </select>
    </div>

    <!-- Error Message -->
    <Transition
      enter-active-class="transition-all duration-200 ease-out"
      enter-from-class="opacity-0 -translate-y-1"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition-all duration-150 ease-in"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 -translate-y-1"
    >
      <p
        v-if="error"
        :id="errorId"
        class="text-xs text-danger pl-0.5 flex items-center gap-1"
        role="alert"
        data-testid="base-select-error"
      >
        <svg class="w-3.5 h-3.5 shrink-0" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
          <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
        </svg>
        {{ error }}
      </p>
    </Transition>
  </div>
</template>

<script setup>
import { computed, useId } from 'vue'

defineOptions({ inheritAttrs: false })

const model = defineModel({ type: [String, Number], default: '' })

const props = defineProps({
  label: {
    type: String,
    default: '',
  },
  error: {
    type: String,
    default: '',
  },
  options: {
    type: Array,
    required: true,
    // Expected format: [{ label: 'VIP', value: 'vip' }]
  },
})

const uid = useId()
const selectId = `select-${uid}`
const errorId = `select-error-${uid}`

const selectClasses = computed(() => {
  const base = [
    'w-full',
    'bg-bg-elevated/80 backdrop-blur-sm',
    'text-main text-sm',
    'border rounded-[var(--radius-input)]',
    'py-2.5 pl-4 pr-10 font-sans cursor-pointer',
    'transition-all duration-250 ease-out',
    'focus:outline-none',
  ]

  if (props.error) {
    base.push(
      'border-danger/50',
      'focus:border-danger focus:ring-2 focus:ring-danger-dim focus:shadow-[0_0_12px_rgba(255,71,87,0.1)]'
    )
  } else {
    base.push(
      'border-border-main',
      'hover:border-white/15',
      'focus:border-border-focus focus:ring-2 focus:ring-primary-dim focus:shadow-[0_0_16px_var(--color-primary-ghost)]'
    )
  }

  // Handle placeholder styling (when nothing is selected or placeholder is active)
  if (model.value === '') {
    base.push('text-dimmed')
  }

  return base.join(' ')
})
</script>
