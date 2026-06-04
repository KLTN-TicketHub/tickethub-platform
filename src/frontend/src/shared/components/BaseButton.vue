<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || isLoading"
    v-bind="$attrs"
  >
    <!-- Loading Spinner -->
    <svg
      v-if="isLoading"
      class="animate-spin-slow shrink-0"
      :class="spinnerSizeClass"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
    >
      <circle
        class="opacity-20"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        stroke-width="3"
      />
      <path
        class="opacity-90"
        d="M12 2a10 10 0 0 1 10 10"
        stroke="currentColor"
        stroke-width="3"
        stroke-linecap="round"
      />
    </svg>

    <!-- Button Content (hidden during loading to preserve width) -->
    <span :class="isLoading ? 'opacity-0 w-0 overflow-hidden' : 'contents'">
      <slot />
    </span>
  </button>
</template>

<script setup>
/**
 * BaseButton — Enterprise UI Kit primitive.
 *
 * Variants: primary | ghost | outline | danger | icon
 * Sizes:    sm | md | lg
 *
 * Forwards all unrecognized attrs (aria-*, data-*, etc.) to the <button>.
 */
import { computed } from 'vue'

defineOptions({ inheritAttrs: false })

const props = defineProps({
  /** Visual variant */
  variant: {
    type: String,
    default: 'primary',
    validator: (v) => ['primary', 'ghost', 'outline', 'danger', 'icon'].includes(v),
  },
  /** Sizing preset */
  size: {
    type: String,
    default: 'md',
    validator: (v) => ['sm', 'md', 'lg'].includes(v),
  },
  /** Shows a spinner and disables interaction */
  isLoading: {
    type: Boolean,
    default: false,
  },
  /** Disables the button */
  disabled: {
    type: Boolean,
    default: false,
  },
})

/* ── Size Classes ──────────────────────────────────────────────────────────── */
const sizeClasses = {
  sm: 'px-3 py-1.5 text-xs gap-1.5 rounded-lg',
  md: 'px-5 py-2.5 text-sm gap-2 rounded-[var(--radius-button)]',
  lg: 'px-7 py-3.5 text-base gap-2.5 rounded-[var(--radius-button)]',
}

const spinnerSizes = {
  sm: 'w-3.5 h-3.5',
  md: 'w-4 h-4',
  lg: 'w-5 h-5',
}

/* ── Variant Classes ───────────────────────────────────────────────────────── */
const variantClasses = {
  primary: [
    'bg-primary text-bg font-semibold',
    'shadow-[0_0_16px_rgba(0,200,83,0.25)]',
    'hover:bg-primary-light hover:shadow-[0_0_24px_rgba(0,200,83,0.4)]',
    'active:scale-[0.97] active:shadow-[0_0_12px_rgba(0,200,83,0.2)]',
  ].join(' '),

  ghost: [
    'bg-transparent text-muted font-medium',
    'hover:bg-white/[0.04] hover:text-main',
    'active:bg-white/[0.06]',
  ].join(' '),

  outline: [
    'bg-transparent text-primary font-medium',
    'border border-border-main',
    'hover:border-primary/40 hover:bg-primary-ghost hover:shadow-[0_0_12px_rgba(0,200,83,0.1)]',
    'active:bg-primary-dim',
  ].join(' '),

  danger: [
    'bg-danger/15 text-danger font-semibold',
    'border border-danger/20',
    'hover:bg-danger/25 hover:border-danger/35',
    'active:scale-[0.97]',
  ].join(' '),

  icon: [
    'bg-transparent text-muted p-0',
    'hover:text-main hover:bg-white/[0.04]',
    'active:bg-white/[0.06]',
    '!rounded-lg',
  ].join(' '),
}

/* ── Icon-variant size overrides (square buttons) ──────────────────────────── */
const iconSizeClasses = {
  sm: 'w-8 h-8 !p-0 rounded-lg',
  md: 'w-10 h-10 !p-0 rounded-[var(--radius-button)]',
  lg: 'w-12 h-12 !p-0 rounded-[var(--radius-button)]',
}

/* ── Computed ──────────────────────────────────────────────────────────────── */
const spinnerSizeClass = computed(() => spinnerSizes[props.size])

const buttonClasses = computed(() => {
  const base = [
    'inline-flex items-center justify-center',
    'font-sans tracking-wide cursor-pointer',
    'transition-all duration-200 ease-out',
    'select-none whitespace-nowrap',
    'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/50 focus-visible:ring-offset-2 focus-visible:ring-offset-bg',
  ].join(' ')

  const size = props.variant === 'icon'
    ? iconSizeClasses[props.size]
    : sizeClasses[props.size]

  const variant = variantClasses[props.variant]

  const state = (props.disabled || props.isLoading)
    ? 'opacity-50 !cursor-not-allowed pointer-events-none'
    : ''

  return [base, size, variant, state].filter(Boolean).join(' ')
})
</script>
