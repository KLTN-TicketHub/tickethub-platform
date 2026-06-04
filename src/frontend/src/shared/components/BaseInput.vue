<template>
  <div class="flex flex-col gap-1.5 w-full">
    <!-- Label -->
    <label
      v-if="label"
      :for="inputId"
      class="text-sm font-medium text-muted pl-0.5 transition-colors duration-200"
      :class="{ '!text-danger': error }"
    >
      {{ label }}
    </label>

    <!-- Input Wrapper (allows future prefix/suffix icon slots) -->
    <div class="relative group">
      <!-- Prefix Icon Slot -->
      <div
        v-if="$slots.prefix"
        class="absolute left-3.5 top-1/2 -translate-y-1/2 text-muted pointer-events-none
               transition-colors duration-200 group-focus-within:text-primary"
      >
        <slot name="prefix" />
      </div>

      <!-- Native Input -->
      <input
        :id="inputId"
        v-model="model"
        v-bind="$attrs"
        :class="inputClasses"
        :aria-invalid="!!error"
        :aria-describedby="error ? errorId : undefined"
      />

      <!-- Suffix Icon Slot -->
      <div
        v-if="$slots.suffix"
        class="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted pointer-events-none
               transition-colors duration-200 group-focus-within:text-primary"
      >
        <slot name="suffix" />
      </div>
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
      >
        <svg class="w-3.5 h-3.5 shrink-0" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
          <path
            fill-rule="evenodd"
            d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
            clip-rule="evenodd"
          />
        </svg>
        {{ error }}
      </p>
    </Transition>
  </div>
</template>

<script setup>
/**
 * BaseInput — Enterprise UI Kit primitive.
 *
 * Uses Vue 3.4+ defineModel() for seamless v-model binding.
 * Forwards all native attrs ($attrs) to the <input> element.
 * Supports prefix/suffix icon slots, optional label, and error state.
 */
import { computed, useId } from 'vue'

defineOptions({ inheritAttrs: false })

const model = defineModel({ type: [String, Number], default: '' })

const props = defineProps({
  /** Visible label above the input */
  label: {
    type: String,
    default: '',
  },
  /** Error message — turns border red and shows below input */
  error: {
    type: String,
    default: '',
  },
})

const slots = defineSlots()

/* ── Unique IDs for a11y ───────────────────────────────────────────────────── */
const uid = useId()
const inputId = `input-${uid}`
const errorId = `input-error-${uid}`

/* ── Computed Classes ──────────────────────────────────────────────────────── */
const inputClasses = computed(() => {
  const base = [
    'w-full',
    'bg-bg-elevated/80 backdrop-blur-sm',
    'text-main text-sm placeholder:text-dimmed',
    'border rounded-[var(--radius-input)]',
    'py-2.5 font-sans',
    'transition-all duration-250 ease-out',
    'focus:outline-none',
  ]

  /* Padding: account for prefix/suffix icon slots */
  base.push(slots.prefix ? 'pl-10' : 'px-4')
  if (slots.suffix) base.push('pr-10')

  /* Border & focus ring states */
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

  return base.join(' ')
})
</script>
