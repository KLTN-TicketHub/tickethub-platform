<template>
  <div class="admin-login">
    <!-- Animated background -->
    <div class="admin-login__bg">
      <div class="admin-login__orb admin-login__orb--1"></div>
      <div class="admin-login__orb admin-login__orb--2"></div>
      <div class="admin-login__orb admin-login__orb--3"></div>
    </div>

    <!-- Login Card -->
    <div class="admin-login__card">
      <!-- Brand Header -->
      <div class="admin-login__brand">
        <div class="admin-login__logo">
          <div class="admin-login__logo-icon">◉</div>
          <div>
            <h1 class="admin-login__logo-text">TicketHub</h1>
            <span class="admin-login__logo-badge">ADMIN PANEL</span>
          </div>
        </div>
        <p class="admin-login__subtitle">Đăng nhập để quản lý hệ thống</p>
      </div>

      <!-- Divider -->
      <div class="admin-login__divider">
        <div class="admin-login__divider-line"></div>
        <span class="admin-login__divider-dot"></span>
        <div class="admin-login__divider-line"></div>
      </div>

      <!-- Error Alert -->
      <Transition
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div v-if="error" class="admin-login__error">
          <svg class="admin-login__error-icon" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
          </svg>
          <span>{{ error }}</span>
        </div>
      </Transition>

      <!-- Form -->
      <form @submit.prevent="handleLogin" class="admin-login__form">
        <!-- Email Field -->
        <div class="admin-login__field">
          <label class="admin-login__label" for="admin-email">Email</label>
          <div class="admin-login__input-wrap">
            <svg class="admin-login__input-icon" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75" />
            </svg>
            <input
              id="admin-email"
              type="email"
              v-model="email"
              placeholder="admin@tickethub.vn"
              required
              autocomplete="email"
              :disabled="isLoading"
              class="admin-login__input"
            />
          </div>
        </div>

        <!-- Password Field -->
        <div class="admin-login__field">
          <label class="admin-login__label" for="admin-password">Mật khẩu</label>
          <div class="admin-login__input-wrap">
            <svg class="admin-login__input-icon" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z" />
            </svg>
            <input
              id="admin-password"
              :type="showPassword ? 'text' : 'password'"
              v-model="password"
              placeholder="••••••••"
              required
              autocomplete="current-password"
              :disabled="isLoading"
              class="admin-login__input"
            />
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="admin-login__eye-btn"
              tabindex="-1"
            >
              <!-- Eye open -->
              <svg v-if="!showPassword" class="w-4.5 h-4.5" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <!-- Eye closed -->
              <svg v-else class="w-4.5 h-4.5" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="isLoading || !email || !password"
          class="admin-login__submit"
        >
          <template v-if="isLoading">
            <svg class="admin-login__spinner" viewBox="0 0 24 24" fill="none">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="3" />
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
            </svg>
            Đang xác thực...
          </template>
          <template v-else>
            <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15m3 0l3-3m0 0l-3-3m3 3H9" />
            </svg>
            Đăng nhập Admin
          </template>
        </button>
      </form>

      <!-- Footer -->
      <div class="admin-login__footer">
        <router-link to="/" class="admin-login__back-link">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
          </svg>
          Quay về trang chủ
        </router-link>
      </div>
    </div>

    <!-- Copyright -->
    <p class="admin-login__copyright">
      © {{ new Date().getFullYear() }} TicketHub Platform. All rights reserved.
    </p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAuth } from '../../features/auth/useAuth'

const { login, error, isLoading } = useAuth()

const email = ref('')
const password = ref('')
const showPassword = ref(false)

const handleLogin = async () => {
  try {
    await login(email.value, password.value)
  } catch {
    // Error is already set in useAuth state — no extra handling needed
  }
}
</script>

<style scoped>
/* ─── Full-screen layout ─────────────────────────────────────────────────── */
.admin-login {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem 1.5rem;
  background: #050807;
  position: relative;
  overflow: hidden;
}

/* ─── Animated background orbs ───────────────────────────────────────────── */
.admin-login__bg {
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}

.admin-login__orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.15;
}

.admin-login__orb--1 {
  width: 600px;
  height: 600px;
  background: #00C853;
  top: -200px;
  right: -100px;
  animation: orb-drift-1 18s ease-in-out infinite alternate;
}

.admin-login__orb--2 {
  width: 400px;
  height: 400px;
  background: #00E676;
  bottom: -100px;
  left: -50px;
  animation: orb-drift-2 22s ease-in-out infinite alternate;
}

.admin-login__orb--3 {
  width: 300px;
  height: 300px;
  background: #69F0AE;
  top: 40%;
  left: 50%;
  animation: orb-drift-3 15s ease-in-out infinite alternate;
}

@keyframes orb-drift-1 {
  0% { transform: translate(0, 0) scale(1); }
  100% { transform: translate(-80px, 60px) scale(1.15); }
}

@keyframes orb-drift-2 {
  0% { transform: translate(0, 0) scale(1); }
  100% { transform: translate(60px, -40px) scale(1.1); }
}

@keyframes orb-drift-3 {
  0% { transform: translate(-50%, 0) scale(1); opacity: 0.1; }
  100% { transform: translate(-50%, -30px) scale(1.2); opacity: 0.18; }
}

/* ─── Card ───────────────────────────────────────────────────────────────── */
.admin-login__card {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 440px;
  background: rgba(10, 15, 13, 0.85);
  backdrop-filter: blur(40px) saturate(1.4);
  -webkit-backdrop-filter: blur(40px) saturate(1.4);
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-radius: 28px;
  padding: 3rem 2.5rem;
  box-shadow:
    0 0 0 1px rgba(0, 200, 83, 0.04),
    0 25px 60px -12px rgba(0, 0, 0, 0.6),
    0 0 120px -30px rgba(0, 200, 83, 0.08);
  animation: card-enter 0.6s cubic-bezier(0.16, 1, 0.3, 1) both;
}

@keyframes card-enter {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.97);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* ─── Brand ──────────────────────────────────────────────────────────────── */
.admin-login__brand {
  text-align: center;
  margin-bottom: 2rem;
}

.admin-login__logo {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.875rem;
  margin-bottom: 0.75rem;
}

.admin-login__logo-icon {
  width: 48px;
  height: 48px;
  background: #00C853;
  color: #050807;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 900;
  box-shadow: 0 0 24px rgba(0, 200, 83, 0.3);
  flex-shrink: 0;
}

.admin-login__logo-text {
  font-family: 'Inter', system-ui, sans-serif;
  font-size: 1.625rem;
  font-weight: 900;
  letter-spacing: -0.04em;
  color: white;
  text-transform: uppercase;
  line-height: 1.1;
}

.admin-login__logo-badge {
  display: inline-block;
  font-size: 0.625rem;
  font-weight: 700;
  letter-spacing: 0.2em;
  color: #00C853;
  background: rgba(0, 200, 83, 0.08);
  border: 1px solid rgba(0, 200, 83, 0.15);
  border-radius: 6px;
  padding: 2px 8px;
  margin-top: 2px;
}

.admin-login__subtitle {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.35);
  font-weight: 500;
  margin-top: 0.5rem;
}

/* ─── Divider ────────────────────────────────────────────────────────────── */
.admin-login__divider {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 2rem;
}

.admin-login__divider-line {
  flex: 1;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.06), transparent);
}

.admin-login__divider-dot {
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background: rgba(0, 200, 83, 0.3);
}

/* ─── Error Alert ────────────────────────────────────────────────────────── */
.admin-login__error {
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

.admin-login__error-icon {
  width: 18px;
  height: 18px;
  flex-shrink: 0;
  color: #ef4444;
  margin-top: 1px;
}

/* ─── Form ───────────────────────────────────────────────────────────────── */
.admin-login__form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.admin-login__field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.admin-login__label {
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: rgba(255, 255, 255, 0.4);
  margin-left: 2px;
}

.admin-login__input-wrap {
  position: relative;
  display: flex;
  align-items: center;
}

.admin-login__input-icon {
  position: absolute;
  left: 1rem;
  width: 18px;
  height: 18px;
  color: rgba(255, 255, 255, 0.2);
  pointer-events: none;
  transition: color 0.2s;
}

.admin-login__input-wrap:focus-within .admin-login__input-icon {
  color: #00C853;
}

.admin-login__input {
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

.admin-login__input::placeholder {
  color: rgba(255, 255, 255, 0.15);
}

.admin-login__input:focus {
  border-color: rgba(0, 200, 83, 0.4);
  background: rgba(255, 255, 255, 0.05);
  box-shadow: 0 0 0 3px rgba(0, 200, 83, 0.06);
}

.admin-login__input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* ─── Eye toggle button ─────────────────────────────────────────────────── */
.admin-login__eye-btn {
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

.admin-login__eye-btn:hover {
  color: rgba(255, 255, 255, 0.6);
  background: rgba(255, 255, 255, 0.05);
}

/* ─── Submit button ──────────────────────────────────────────────────────── */
.admin-login__submit {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.625rem;
  width: 100%;
  padding: 0.9375rem;
  margin-top: 0.75rem;
  background: #00C853;
  color: #050807;
  font-size: 0.9375rem;
  font-weight: 800;
  border: none;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 0 24px rgba(0, 200, 83, 0.2);
}

.admin-login__submit:hover:not(:disabled) {
  background: #00E676;
  transform: translateY(-1px);
  box-shadow: 0 8px 32px rgba(0, 200, 83, 0.3);
}

.admin-login__submit:active:not(:disabled) {
  transform: translateY(0);
}

.admin-login__submit:disabled {
  opacity: 0.4;
  cursor: not-allowed;
  box-shadow: none;
}

/* ─── Spinner ────────────────────────────────────────────────────────────── */
.admin-login__spinner {
  width: 20px;
  height: 20px;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* ─── Footer ─────────────────────────────────────────────────────────────── */
.admin-login__footer {
  margin-top: 2rem;
  text-align: center;
}

.admin-login__back-link {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.3);
  text-decoration: none;
  transition: color 0.2s;
}

.admin-login__back-link:hover {
  color: #00C853;
}

/* ─── Copyright ──────────────────────────────────────────────────────────── */
.admin-login__copyright {
  position: relative;
  z-index: 1;
  margin-top: 2rem;
  font-size: 0.6875rem;
  color: rgba(255, 255, 255, 0.15);
  font-weight: 500;
  letter-spacing: 0.02em;
}
</style>
