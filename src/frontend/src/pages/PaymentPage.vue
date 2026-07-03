<template>
  <div class="min-h-screen bg-[#0A0F0D] flex flex-col items-center justify-center px-6 py-20">
    <!-- Animated Glow Background -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-1/3 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] bg-primary/5 rounded-full blur-[120px]"></div>
    </div>

    <div class="relative z-10 w-full max-w-lg">

      <!-- ── State: Processing (Polling) ── -->
      <transition name="fade" mode="out-in">
        <div v-if="state === 'processing'" key="processing" class="flex flex-col items-center text-center gap-8">
          <!-- Animated Spinner Card -->
          <div class="w-28 h-28 rounded-full border-4 border-white/5 flex items-center justify-center bg-[#111916] shadow-[0_0_60px_rgba(0,200,83,0.15)] relative">
            <svg class="absolute inset-0 w-full h-full animate-spin" viewBox="0 0 112 112">
              <circle cx="56" cy="56" r="50" stroke="#00C853" stroke-width="4" fill="none" stroke-dasharray="80 235" stroke-linecap="round"/>
            </svg>
            <PhReceipt class="text-primary text-4xl" weight="duotone"/>
          </div>

          <div class="space-y-3">
            <h1 class="text-3xl font-black font-heading text-white tracking-tight">Đang khởi tạo thanh toán</h1>
            <p class="text-white/50 text-[15px] font-medium leading-relaxed max-w-sm">
              Hệ thống đang xử lý đơn hàng của bạn. Vui lòng không đóng trang này...
            </p>
          </div>

          <!-- Order Info -->
          <div class="w-full bg-[#111916] border border-white/5 rounded-3xl p-6 text-left space-y-4">
            <div class="flex justify-between items-center text-[13px]">
              <span class="text-white/40 font-bold uppercase tracking-wider">Mã đơn hàng</span>
              <span class="text-white font-mono font-bold text-xs truncate max-w-[200px]">{{ orderId }}</span>
            </div>
            <div class="flex justify-between items-center text-[13px]">
              <span class="text-white/40 font-bold uppercase tracking-wider">Trạng thái Saga</span>
              <span class="font-bold" :class="sagaStatusClass">{{ sagaStatus || '—' }}</span>
            </div>
            <div class="flex justify-between items-center text-[13px]">
              <span class="text-white/40 font-bold uppercase tracking-wider">Thử lần</span>
              <span class="text-white font-bold">{{ pollAttempt }} / 20</span>
            </div>

            <!-- Progress bar -->
            <div class="h-1 bg-white/5 rounded-full overflow-hidden">
              <div
                class="h-full bg-primary rounded-full transition-all duration-500"
                :style="{ width: `${Math.min((pollAttempt / 20) * 100, 100)}%` }"
              ></div>
            </div>
          </div>

          <p class="text-white/25 text-[12px]">Đang chuyển hướng đến cổng thanh toán VNPay khi sẵn sàng...</p>
        </div>

        <!-- ── State: Redirecting ── -->
        <div v-else-if="state === 'redirecting'" key="redirecting" class="flex flex-col items-center text-center gap-8">
          <div class="w-28 h-28 rounded-full bg-primary/10 border border-primary/30 flex items-center justify-center shadow-[0_0_60px_rgba(0,200,83,0.3)]">
            <PhArrowSquareOut class="text-primary text-5xl" weight="duotone"/>
          </div>

          <div class="space-y-3">
            <h1 class="text-3xl font-black font-heading text-white tracking-tight">Chuyển đến VNPay</h1>
            <p class="text-white/50 text-[15px] font-medium">Bạn sẽ được chuyển đến trang thanh toán an toàn của VNPay...</p>
          </div>

          <div class="w-full bg-[#111916] border border-primary/20 rounded-3xl p-6 flex items-center gap-4">
            <div class="w-12 h-12 rounded-2xl bg-primary/10 flex items-center justify-center shrink-0">
              <PhShieldCheck class="text-primary text-2xl" weight="duotone"/>
            </div>
            <div>
              <div class="text-white font-bold text-[14px]">Giao dịch được mã hoá SSL</div>
              <div class="text-white/40 text-[12px] mt-0.5">Thông tin thanh toán của bạn được bảo mật hoàn toàn.</div>
            </div>
          </div>

          <button @click="openPaymentLink" class="w-full py-4 bg-primary hover:bg-primary-dark text-black font-black rounded-2xl transition-all hover:scale-[1.02] active:scale-95 text-[15px] flex items-center justify-center gap-2 cursor-pointer">
            <PhCreditCard weight="fill"/> Tiếp tục thanh toán ngay
          </button>

          <p class="text-white/25 text-[12px]">Nếu không tự động chuyển hướng, hãy nhấn nút trên.</p>
        </div>

        <!-- ── State: Error / Failed ── -->
        <div v-else-if="state === 'error'" key="error" class="flex flex-col items-center text-center gap-8">
          <div class="w-28 h-28 rounded-full bg-red-500/10 border border-red-500/20 flex items-center justify-center shadow-[0_0_60px_rgba(239,68,68,0.15)]">
            <PhWarningCircle class="text-red-400 text-5xl" weight="duotone"/>
          </div>

          <div class="space-y-3">
            <h1 class="text-3xl font-black font-heading text-white tracking-tight">Không thể thanh toán</h1>
            <p class="text-white/50 text-[15px] font-medium leading-relaxed max-w-sm">{{ errorMessage }}</p>
          </div>

          <div class="flex flex-col w-full gap-3">
            <button @click="router.back()" class="w-full py-4 bg-[#111916] border border-white/10 text-white font-black rounded-2xl transition-all hover:bg-white/5 text-[14px] flex items-center justify-center gap-2 cursor-pointer">
              <PhArrowLeft weight="bold"/> Quay lại trang sự kiện
            </button>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { pollForPaymentLink } from '../services/order.service'
import {
  PhReceipt, PhArrowSquareOut, PhShieldCheck, PhCreditCard,
  PhWarningCircle, PhArrowLeft
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const orderId = computed(() => route.query.orderId)

const state = ref('processing') // 'processing' | 'redirecting' | 'error'
const sagaStatus = ref(null)
const pollAttempt = ref(0)
const errorMessage = ref('')
const paymentUrl = ref('')

const sagaStatusClass = computed(() => {
  switch (sagaStatus.value) {
    case 'Pending': return 'text-yellow-400'
    case 'Reserved': return 'text-blue-400'
    case 'PaymentInitiated': return 'text-primary'
    case 'Paid': return 'text-primary'
    case 'Failed': return 'text-red-400'
    default: return 'text-white/50'
  }
})

const openPaymentLink = () => {
  if (paymentUrl.value) {
    window.location.href = paymentUrl.value
  }
}

onMounted(async () => {
  if (!orderId.value) {
    state.value = 'error'
    errorMessage.value = 'Không tìm thấy thông tin đơn hàng. Vui lòng thử lại từ trang sự kiện.'
    return
  }

  try {
    const link = await pollForPaymentLink(orderId.value, {
      maxAttempts: 20,
      intervalMs: 2000,
      onStatusChange: (status) => {
        sagaStatus.value = status
        pollAttempt.value++
      }
    })

    paymentUrl.value = link
    state.value = 'redirecting'

    // Auto-redirect after 1.5s
    setTimeout(() => {
      window.location.href = link
    }, 1500)

  } catch (err) {
    state.value = 'error'
    errorMessage.value = err.message || 'Đã xảy ra lỗi trong quá trình khởi tạo thanh toán.'
  }
})
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(12px);
}
</style>
