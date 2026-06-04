<template>
  <!-- Full-screen Overlay -->
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-300 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-200 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="visible"
        class="fixed inset-0 z-[100] flex items-center justify-center p-4"
      >
        <!-- Backdrop -->
        <div
          class="absolute inset-0 bg-black/70 backdrop-blur-sm"
          @click="handleClose"
        />

        <!-- Modal Card -->
        <Transition
          enter-active-class="transition-all duration-300 ease-[cubic-bezier(0.34,1.56,0.64,1)]"
          enter-from-class="opacity-0 scale-90 translate-y-4"
          enter-to-class="opacity-100 scale-100 translate-y-0"
          leave-active-class="transition-all duration-200 ease-in"
          leave-from-class="opacity-100 scale-100"
          leave-to-class="opacity-0 scale-95"
          appear
        >
          <div
            v-if="visible"
            class="glass-panel relative z-10 w-full max-w-md p-8"
            role="dialog"
            aria-modal="true"
            aria-labelledby="auth-modal-title"
            @click.stop
          >
            <!-- Close Button -->
            <BaseButton
              variant="icon"
              size="sm"
              class="absolute top-4 right-4"
              aria-label="Close dialog"
              @click="handleClose"
            >
              <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <line x1="18" y1="6" x2="6" y2="18" />
                <line x1="6" y1="6" x2="18" y2="18" />
              </svg>
            </BaseButton>

            <!-- Header -->
            <div class="text-center mb-8">
              <div
                class="w-14 h-14 mx-auto mb-4 rounded-2xl bg-primary/10 border border-primary/20
                       flex items-center justify-center animate-scale-in"
              >
                <svg class="w-7 h-7 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                  <path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z" />
                  <path d="M13 5v2" /><path d="M13 17v2" /><path d="M13 11v2" />
                </svg>
              </div>
              <h2
                id="auth-modal-title"
                class="text-xl font-heading font-bold text-main"
              >
                Welcome back
              </h2>
              <p class="text-sm text-muted mt-1.5">
                Sign in to your TicketHub account
              </p>
            </div>

            <!-- Login Form -->
            <form @submit.prevent="handleSubmit" class="space-y-5">
              <BaseInput
                v-model="form.email"
                label="Email"
                type="email"
                placeholder="you@example.com"
                autocomplete="email"
                :error="fieldErrors.email"
                :disabled="authStore.isLoading"
              >
                <template #prefix>
                  <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <rect width="20" height="16" x="2" y="4" rx="2" />
                    <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" />
                  </svg>
                </template>
              </BaseInput>

              <BaseInput
                v-model="form.password"
                label="Password"
                type="password"
                placeholder="••••••••"
                autocomplete="current-password"
                :error="fieldErrors.password"
                :disabled="authStore.isLoading"
              >
                <template #prefix>
                  <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <rect width="18" height="11" x="3" y="11" rx="2" ry="2" />
                    <path d="M7 11V7a5 5 0 0 1 10 0v4" />
                  </svg>
                </template>
              </BaseInput>

              <!-- Global Error -->
              <Transition
                enter-active-class="transition-all duration-200 ease-out"
                enter-from-class="opacity-0 -translate-y-1"
                enter-to-class="opacity-100 translate-y-0"
                leave-active-class="transition-all duration-150 ease-in"
                leave-from-class="opacity-100"
                leave-to-class="opacity-0"
              >
                <div
                  v-if="authStore.error"
                  class="flex items-center gap-2.5 px-4 py-3 rounded-[var(--radius-badge)]
                         bg-danger-dim border border-danger/20 text-danger text-sm"
                  role="alert"
                >
                  <svg class="w-4 h-4 shrink-0" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                    <circle cx="12" cy="12" r="10" />
                    <line x1="12" y1="8" x2="12" y2="12" />
                    <line x1="12" y1="16" x2="12.01" y2="16" />
                  </svg>
                  {{ authStore.error }}
                </div>
              </Transition>

              <!-- Submit -->
              <BaseButton
                type="submit"
                variant="primary"
                size="lg"
                class="w-full"
                :is-loading="authStore.isLoading"
                :disabled="authStore.isLoading"
              >
                Sign In
              </BaseButton>
            </form>

            <!-- Divider -->
            <div class="flex items-center gap-3 my-6">
              <div class="flex-1 h-px bg-border-main" />
              <span class="text-xs text-dimmed uppercase tracking-wider font-medium">
                Quick Demo
              </span>
              <div class="flex-1 h-px bg-border-main" />
            </div>

            <!-- Demo Login Shortcuts -->
            <div class="grid grid-cols-3 gap-2">
              <button
                v-for="demo in demoAccounts"
                :key="demo.email"
                type="button"
                class="demo-btn group"
                :disabled="authStore.isLoading"
                @click="fillDemo(demo)"
              >
                <span
                  class="flex items-center justify-center w-8 h-8 rounded-lg text-sm font-bold
                         transition-all duration-200"
                  :class="demo.badgeClass"
                >
                  {{ demo.initial }}
                </span>
                <span class="text-xs text-muted group-hover:text-main transition-colors duration-200">
                  {{ demo.label }}
                </span>
              </button>
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
/**
 * AuthModal — Glass-panel login dialog.
 *
 * Uses BaseInput / BaseButton exclusively. Provides 3 demo-login shortcuts
 * that autofill credentials for rapid development testing.
 * Emits 'close' and 'success' events to the parent.
 */
import { reactive, watch } from 'vue'
import { useAuthStore } from '@/features/auth/store'
import BaseButton from '@/shared/components/BaseButton.vue'
import BaseInput from '@/shared/components/BaseInput.vue'

const props = defineProps({
  /** Controls visibility — parent should v-model or v-if this. */
  visible: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['close', 'success'])

const authStore = useAuthStore()

/* ── Form State ────────────────────────────────────────────────────────────── */
const form = reactive({
  email: '',
  password: '',
})

const fieldErrors = reactive({
  email: '',
  password: '',
})

/* ── Clear errors when inputs change ───────────────────────────────────────── */
watch(
  () => form.email,
  () => {
    fieldErrors.email = ''
    authStore.clearError()
  },
)

watch(
  () => form.password,
  () => {
    fieldErrors.password = ''
    authStore.clearError()
  },
)

/* ── Reset form when modal opens ───────────────────────────────────────────── */
watch(
  () => props.visible,
  (isVisible) => {
    if (isVisible) {
      form.email = ''
      form.password = ''
      fieldErrors.email = ''
      fieldErrors.password = ''
      authStore.clearError()
    }
  },
)

/* ── Validation ────────────────────────────────────────────────────────────── */
function validate() {
  let valid = true

  if (!form.email.trim()) {
    fieldErrors.email = 'Email is required.'
    valid = false
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email.trim())) {
    fieldErrors.email = 'Please enter a valid email address.'
    valid = false
  }

  if (!form.password.trim()) {
    fieldErrors.password = 'Password is required.'
    valid = false
  }

  return valid
}

/* ── Handlers ──────────────────────────────────────────────────────────────── */
async function handleSubmit() {
  if (!validate()) return

  try {
    await authStore.login(form.email, form.password)
    emit('success')
    emit('close')
  } catch {
    // Error is already in authStore.error — displayed in the template
  }
}

function handleClose() {
  if (authStore.isLoading) return // Don't allow closing during login
  emit('close')
}

function fillDemo(demo) {
  form.email = demo.email
  form.password = demo.password
  fieldErrors.email = ''
  fieldErrors.password = ''
  authStore.clearError()
}

/* ── Demo Account Config ───────────────────────────────────────────────────── */
const demoAccounts = [
  {
    label: 'Admin',
    email: 'admin@tickethub.local',
    password: 'demo',
    initial: 'A',
    badgeClass: 'bg-danger/15 text-danger border border-danger/25 group-hover:bg-danger/25',
  },
  {
    label: 'Organizer',
    email: 'organizer@tickethub.local',
    password: 'demo',
    initial: 'O',
    badgeClass: 'bg-primary/15 text-primary border border-primary/25 group-hover:bg-primary/25',
  },
  {
    label: 'Customer',
    email: 'customer@tickethub.local',
    password: 'demo',
    initial: 'C',
    badgeClass: 'bg-info/15 text-info border border-info/25 group-hover:bg-info/25',
  },
]
</script>

<style scoped>
@reference "@/app.css";

.demo-btn {
  @apply flex flex-col items-center gap-1.5 py-3 rounded-xl
         cursor-pointer bg-transparent border border-transparent
         transition-all duration-200 ease-out
         hover:bg-white/[0.03] hover:border-border-light
         disabled:opacity-40 disabled:pointer-events-none;
}
</style>
