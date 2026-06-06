<template>
  <div class="forbidden-page">
    <!-- Background Orbs -->
    <div class="forbidden-page__bg">
      <div class="forbidden-page__orb forbidden-page__orb--1"></div>
      <div class="forbidden-page__orb forbidden-page__orb--2"></div>
    </div>

    <!-- Content Card -->
    <div class="forbidden-page__card">
      <div class="forbidden-page__icon-wrap">
        <svg class="forbidden-page__icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
      </div>

      <h1 class="forbidden-page__title">403</h1>
      <h2 class="forbidden-page__subtitle">Truy cập bị từ chối</h2>
      <p class="forbidden-page__desc">
        Tài khoản của bạn không có đủ quyền hạn để truy cập vào phân vùng này. Vui lòng quay lại trang chủ hoặc đăng nhập bằng một tài khoản khác.
      </p>

      <div class="forbidden-page__actions">
        <button @click="goHome" class="forbidden-page__btn forbidden-page__btn--primary">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 12l8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25" />
          </svg>
          Quay lại trang chủ
        </button>

        <button @click="handleLogout" class="forbidden-page__btn forbidden-page__btn--secondary">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15m3 0l3-3m0 0l-3-3m3 3H9" />
          </svg>
          Đăng xuất tài khoản
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { logout } from '../services/auth/auth.service'
import { store } from '../stores/eventStore'

const router = useRouter()

const homePath = computed(() => {
  if (!store.user) return '/'
  const roles = store.user.roles || []
  if (roles.some(r => r.toLowerCase() === 'admin')) return '/admin/dashboard'
  if (roles.some(r => r.toLowerCase() === 'moderator')) return '/moderator/dashboard'
  if (roles.some(r => r.toLowerCase() === 'organizer')) return '/organizer'
  if (roles.some(r => r.toLowerCase() === 'staff')) return '/staff/dashboard'
  return '/'
})

const goHome = () => {
  router.push(homePath.value)
}

const handleLogout = async () => {
  await logout()
}
</script>

<style scoped>
.forbidden-page {
  min-height: 80vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1.5rem;
  background: #050807;
  position: relative;
  overflow: hidden;
  border-radius: 32px;
  margin: 2rem 0;
  box-shadow: inset 0 0 80px rgba(0, 0, 0, 0.8);
}

.forbidden-page__bg {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}

.forbidden-page__orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.15;
}

.forbidden-page__orb--1 {
  width: 400px;
  height: 400px;
  background: #ef4444;
  top: -100px;
  right: -50px;
  animation: float-slow 15s ease-in-out infinite alternate;
}

.forbidden-page__orb--2 {
  width: 300px;
  height: 300px;
  background: #f59e0b;
  bottom: -50px;
  left: -50px;
  animation: float-slow 20s ease-in-out infinite alternate-reverse;
}

@keyframes float-slow {
  0% { transform: translate(0, 0) scale(1); }
  100% { transform: translate(40px, -40px) scale(1.1); }
}

.forbidden-page__card {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 460px;
  background: rgba(10, 15, 13, 0.85);
  backdrop-filter: blur(40px) saturate(1.4);
  -webkit-backdrop-filter: blur(40px) saturate(1.4);
  border: 1px solid rgba(239, 68, 68, 0.15);
  border-radius: 28px;
  padding: 3.5rem 2.5rem;
  text-align: center;
  box-shadow:
    0 25px 60px -12px rgba(0, 0, 0, 0.6),
    0 0 100px -30px rgba(239, 68, 68, 0.08);
  animation: enter 0.5s cubic-bezier(0.16, 1, 0.3, 1) both;
}

@keyframes enter {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.forbidden-page__icon-wrap {
  width: 72px;
  height: 72px;
  background: rgba(239, 68, 68, 0.08);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem;
  color: #ef4444;
  box-shadow: 0 0 30px rgba(239, 68, 68, 0.1);
}

.forbidden-page__icon {
  width: 32px;
  height: 32px;
}

.forbidden-page__title {
  font-family: 'Outfit', 'Inter', system-ui, sans-serif;
  font-size: 5rem;
  font-weight: 900;
  letter-spacing: -0.05em;
  color: #ef4444;
  line-height: 1;
  margin-bottom: 0.5rem;
  text-shadow: 0 0 40px rgba(239, 68, 68, 0.15);
}

.forbidden-page__subtitle {
  font-size: 1.5rem;
  font-weight: 800;
  color: white;
  margin-bottom: 1rem;
}

.forbidden-page__desc {
  font-size: 0.9375rem;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.45);
  margin-bottom: 2.5rem;
}

.forbidden-page__actions {
  display: flex;
  flex-direction: column;
  gap: 0.875rem;
}

.forbidden-page__btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.625rem;
  padding: 0.9375rem 1.5rem;
  border-radius: 14px;
  font-size: 0.9375rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
  border: none;
  width: 100%;
}

.forbidden-page__btn--primary {
  background: #ef4444;
  color: white;
  box-shadow: 0 0 20px rgba(239, 68, 68, 0.2);
}

.forbidden-page__btn--primary:hover {
  background: #f87171;
  transform: translateY(-1px);
  box-shadow: 0 8px 25px rgba(239, 68, 68, 0.3);
}

.forbidden-page__btn--secondary {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  color: rgba(255, 255, 255, 0.8);
}

.forbidden-page__btn--secondary:hover {
  background: rgba(255, 255, 255, 0.06);
  border-color: rgba(255, 255, 255, 0.15);
  color: white;
  transform: translateY(-1px);
}

.forbidden-page__btn:active {
  transform: translateY(0);
}
</style>
