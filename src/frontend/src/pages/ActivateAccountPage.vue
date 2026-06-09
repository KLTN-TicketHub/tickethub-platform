<template>
  <div class="activate-account">
    <!-- Animated background -->
    <div class="activate-account__bg">
      <div class="activate-account__orb activate-account__orb--1"></div>
      <div class="activate-account__orb activate-account__orb--2"></div>
    </div>

    <!-- Activation Card -->
    <div class="activate-account__card">
      <!-- Header -->
      <div class="activate-account__brand">
        <div class="activate-account__logo">
          <div class="activate-account__logo-icon">🔑</div>
          <div>
            <h1 class="activate-account__logo-text">TicketHub</h1>
            <span class="activate-account__logo-badge">KÍCH HOẠT TÀI KHOẢN</span>
          </div>
        </div>
        <p class="activate-account__subtitle">Thiết lập mật khẩu mới cho tài khoản kiểm duyệt viên của bạn</p>
      </div>

      <!-- Divider -->
      <div class="activate-account__divider">
        <div class="activate-account__divider-line"></div>
        <span class="activate-account__divider-dot"></span>
        <div class="activate-account__divider-line"></div>
      </div>

      <!-- General Error -->
      <Transition
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
      >
        <div v-if="validationErrors.general" class="activate-account__error">
          <svg class="activate-account__error-icon" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
          </svg>
          <span>{{ validationErrors.general }}</span>
        </div>
      </Transition>

      <div v-if="missingParams" class="p-6 rounded-2xl bg-danger/10 border border-danger/20 text-danger text-center text-sm font-semibold mb-6 flex flex-col gap-2">
        <span>⚠️ Tham số không hợp lệ</span>
        <span class="text-xs opacity-80 font-normal">Thiếu thông tin người dùng hoặc token kích hoạt trong đường dẫn. Vui lòng kiểm tra lại email của bạn.</span>
      </div>

      <!-- Form -->
      <form v-else @submit.prevent="handleActivate" class="activate-account__form">
        <!-- Password Field -->
        <div class="activate-account__field">
          <label class="activate-account__label" for="act-password">Mật khẩu mới</label>
          <div class="activate-account__input-wrap">
            <svg class="activate-account__input-icon" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z" />
            </svg>
            <input
              id="act-password"
              :type="showPassword ? 'text' : 'password'"
              v-model="password"
              placeholder="Nhập mật khẩu mới"
              required
              autocomplete="new-password"
              :disabled="isLoading"
              class="activate-account__input"
            />
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="activate-account__eye-btn"
              tabindex="-1"
            >
              <svg v-if="!showPassword" class="w-4.5 h-4.5" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <svg v-else class="w-4.5 h-4.5" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88" />
              </svg>
            </button>
          </div>
          <!-- Field Validation Errors -->
          <div v-if="validationErrors.password" class="flex flex-col gap-1.5 mt-1 px-1">
            <span v-for="err in validationErrors.password" :key="err" class="text-xs text-danger font-medium flex items-center gap-1.5">
              <span>✕</span> {{ err }}
            </span>
          </div>
        </div>

        <!-- Confirm Password Field -->
        <div class="activate-account__field mt-2">
          <label class="activate-account__label" for="act-confirmpassword">Xác nhận mật khẩu</label>
          <div class="activate-account__input-wrap">
            <svg class="activate-account__input-icon" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z" />
            </svg>
            <input
              id="act-confirmpassword"
              :type="showConfirmPassword ? 'text' : 'password'"
              v-model="confirmPassword"
              placeholder="Xác nhận lại mật khẩu"
              required
              autocomplete="new-password"
              :disabled="isLoading"
              class="activate-account__input"
            />
            <button
              type="button"
              @click="showConfirmPassword = !showConfirmPassword"
              class="activate-account__eye-btn"
              tabindex="-1"
            >
              <svg v-if="!showConfirmPassword" class="w-4.5 h-4.5" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <svg v-else class="w-4.5 h-4.5" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88" />
              </svg>
            </button>
          </div>
          <div v-if="validationErrors.confirmPassword" class="flex flex-col gap-1.5 mt-1 px-1">
            <span v-for="err in validationErrors.confirmPassword" :key="err" class="text-xs text-danger font-medium flex items-center gap-1.5">
              <span>✕</span> {{ err }}
            </span>
          </div>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="isLoading || !password || !confirmPassword"
          class="activate-account__submit"
        >
          <template v-if="isLoading">
            <svg class="activate-account__spinner" viewBox="0 0 24 24" fill="none">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="3" />
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
            </svg>
            Đang kích hoạt tài khoản...
          </template>
          <template v-else>
            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            Hoàn tất & Kích hoạt
          </template>
        </button>
      </form>

      <!-- Footer -->
      <div class="activate-account__footer">
        <router-link to="/" class="activate-account__back-link">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
          </svg>
          Quay về trang chủ
        </router-link>
      </div>
    </div>

    <!-- Copyright -->
    <p class="activate-account__copyright">
      © {{ new Date().getFullYear() }} TicketHub Platform. All rights reserved.
    </p>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { activateModeratorAccount } from '../services/auth/auth.service'
import { store } from '../stores/eventStore'

const route = useRoute()
const router = useRouter()

const userId = route.query.userId || ''
const token = route.query.token || ''

const password = ref('')
const confirmPassword = ref('')
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const isLoading = ref(false)
const validationErrors = ref({})

const missingParams = computed(() => !userId || !token)

const handleActivate = async () => {
  isLoading.value = true
  validationErrors.value = {}
  
  const payload = {
    userId: String(userId),
    token: encodeURIComponent(String(token)),
    password: password.value,
    confirmPassword: confirmPassword.value
  }

  try {
    const res = await activateModeratorAccount(payload)
    if (res.success) {
      store.toast = { message: 'Kích hoạt tài khoản thành công! Vui lòng đăng nhập.', icon: '🔑' }
      // Redirect to moderator login
      await router.push('/moderator/login')
    } else {
      validationErrors.value.general = res.message || 'Kích hoạt tài khoản không thành công.'
    }
  } catch (err) {
    const errorData = err.response?.data
    if (errorData?.errors) {
      if (Array.isArray(errorData.errors)) {
        const normalized = {}
        errorData.errors.forEach(item => {
          if (item && item.field) {
            if (!normalized[item.field]) {
              normalized[item.field] = []
            }
            normalized[item.field].push(item.message)
          }
        })
        validationErrors.value = normalized
      } else {
        validationErrors.value = { ...errorData.errors }
      }
    } else {
      validationErrors.value.general = errorData?.message || err.message || 'Lỗi kết nối máy chủ.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.activate-account {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem 1.5rem;
  background: #050508;
  position: relative;
  overflow: hidden;
}

.activate-account__bg {
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}

.activate-account__orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.12;
}

.activate-account__orb--1 {
  width: 600px;
  height: 600px;
  background: #a855f7;
  top: -200px;
  right: -100px;
  animation: drift-1 18s ease-in-out infinite alternate;
}

.activate-account__orb--2 {
  width: 400px;
  height: 400px;
  background: #6366f1;
  bottom: -100px;
  left: -50px;
  animation: drift-2 22s ease-in-out infinite alternate;
}

@keyframes drift-1 {
  0% { transform: translate(0, 0) scale(1); }
  100% { transform: translate(-80px, 60px) scale(1.15); }
}

@keyframes drift-2 {
  0% { transform: translate(0, 0) scale(1); }
  100% { transform: translate(60px, -40px) scale(1.1); }
}

.activate-account__card {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 450px;
  background: rgba(10, 10, 15, 0.85);
  backdrop-filter: blur(40px) saturate(1.4);
  -webkit-backdrop-filter: blur(40px) saturate(1.4);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 28px;
  padding: 3rem 2.5rem;
  box-shadow:
    0 0 0 1px rgba(168, 85, 247, 0.04),
    0 25px 60px -12px rgba(0, 0, 0, 0.6),
    0 0 120px -30px rgba(168, 85, 247, 0.08);
  animation: enter 0.6s cubic-bezier(0.16, 1, 0.3, 1) both;
}

@keyframes enter {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.97);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.activate-account__brand {
  text-align: center;
  margin-bottom: 2rem;
}

.activate-account__logo {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.875rem;
  margin-bottom: 0.75rem;
}

.activate-account__logo-icon {
  width: 48px;
  height: 48px;
  background: #a855f7;
  color: white;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  box-shadow: 0 0 24px rgba(168, 85, 247, 0.3);
  flex-shrink: 0;
}

.activate-account__logo-text {
  font-family: 'Inter', system-ui, sans-serif;
  font-size: 1.625rem;
  font-weight: 900;
  letter-spacing: -0.04em;
  color: white;
  text-transform: uppercase;
  line-height: 1.1;
}

.activate-account__logo-badge {
  display: inline-block;
  font-size: 0.625rem;
  font-weight: 700;
  letter-spacing: 0.2em;
  color: #6366f1;
  background: rgba(99, 102, 241, 0.08);
  border: 1px solid rgba(99, 102, 241, 0.15);
  border-radius: 6px;
  padding: 2px 8px;
  margin-top: 2px;
}

.activate-account__subtitle {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.35);
  font-weight: 500;
  margin-top: 0.5rem;
}

.activate-account__divider {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 2rem;
}

.activate-account__divider-line {
  flex: 1;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.06), transparent);
}

.activate-account__divider-dot {
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background: rgba(168, 85, 247, 0.3);
}

.activate-account__error {
  display: flex;
  align-items: flex-start;
  gap: 0.625rem;
  padding: 0.875rem 1rem;
  background: rgba(239, 68, 68, 0.08);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 14px;
  margin-bottom: 1.5rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #fca5a5;
  line-height: 1.5;
}

.activate-account__error-icon {
  width: 18px;
  height: 18px;
  flex-shrink: 0;
  color: #ef4444;
  margin-top: 1px;
}

.activate-account__form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.activate-account__field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.activate-account__label {
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: rgba(255, 255, 255, 0.4);
  margin-left: 2px;
}

.activate-account__input-wrap {
  position: relative;
  display: flex;
  align-items: center;
}

.activate-account__input-icon {
  position: absolute;
  left: 1rem;
  width: 18px;
  height: 18px;
  color: rgba(255, 255, 255, 0.2);
  pointer-events: none;
  transition: color 0.2s;
}

.activate-account__input-wrap:focus-within .activate-account__input-icon {
  color: #a855f7;
}

.activate-account__input {
  width: 100%;
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 14px;
  padding: 0.875rem 1rem 0.875rem 2.75rem;
  font-size: 0.9375rem;
  font-weight: 500;
  color: white;
  outline: none;
  transition: all 0.25s ease;
}

.activate-account__input::placeholder {
  color: rgba(255, 255, 255, 0.15);
}

.activate-account__input:focus {
  border-color: rgba(168, 85, 247, 0.4);
  background: rgba(255, 255, 255, 0.05);
  box-shadow: 0 0 0 3px rgba(168, 85, 247, 0.06);
}

.activate-account__input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.activate-account__eye-btn {
  position: absolute;
  right: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0.375rem;
  border-radius: 8px;
  color: rgba(255, 255, 255, 0.25);
  background: transparent;
  border: none;
  cursor: pointer;
  transition: all 0.2s;
}

.activate-account__eye-btn:hover {
  color: rgba(255, 255, 255, 0.6);
  background: rgba(255, 255, 255, 0.05);
}

.activate-account__submit {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.625rem;
  width: 100%;
  padding: 0.9375rem;
  margin-top: 0.75rem;
  background: #a855f7;
  color: white;
  font-size: 0.9375rem;
  font-weight: 800;
  border: none;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 0 24px rgba(168, 85, 247, 0.2);
}

.activate-account__submit:hover:not(:disabled) {
  background: #9333ea;
  transform: translateY(-1px);
  box-shadow: 0 8px 32px rgba(168, 85, 247, 0.3);
}

.activate-account__submit:active:not(:disabled) {
  transform: translateY(0);
}

.activate-account__submit:disabled {
  opacity: 0.4;
  cursor: not-allowed;
  box-shadow: none;
}

.activate-account__spinner {
  width: 20px;
  height: 20px;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.activate-account__footer {
  margin-top: 2rem;
  text-align: center;
}

.activate-account__back-link {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.3);
  text-decoration: none;
  transition: color 0.2s;
}

.activate-account__back-link:hover {
  color: #a855f7;
}

.activate-account__copyright {
  position: relative;
  z-index: 1;
  margin-top: 2rem;
  font-size: 0.6875rem;
  color: rgba(255, 255, 255, 0.15);
  font-weight: 500;
  letter-spacing: 0.02em;
}
</style>
