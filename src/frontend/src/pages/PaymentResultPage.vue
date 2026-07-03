<template>
  <div class="min-h-screen bg-[#0A0F0D] flex flex-col items-center justify-center px-6 py-20">
    <!-- Glow Background -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-1/3 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] rounded-full blur-[120px]"
        :class="isSuccess ? 'bg-primary/6' : 'bg-red-500/5'"></div>
    </div>

    <div class="relative z-10 w-full max-w-lg">
      <transition name="fade" mode="out-in">

        <!-- Loading state while verifying -->
        <div v-if="isVerifying" key="verifying" class="flex flex-col items-center text-center gap-8">
          <div class="w-24 h-24 rounded-full border-4 border-white/5 flex items-center justify-center bg-[#111916] relative">
            <svg class="absolute inset-0 w-full h-full animate-spin" viewBox="0 0 96 96">
              <circle cx="48" cy="48" r="42" stroke="#00C853" stroke-width="4" fill="none" stroke-dasharray="66 198" stroke-linecap="round"/>
            </svg>
            <PhSpinner class="text-primary text-3xl" weight="bold"/>
          </div>
          <h1 class="text-2xl font-black font-heading text-white">Đang xác nhận giao dịch...</h1>
        </div>

        <!-- Success -->
        <div v-else-if="isSuccess" key="success" class="flex flex-col items-center text-center gap-8">
          <!-- Checkmark Circle with animation -->
          <div class="relative">
            <div class="w-32 h-32 rounded-full bg-primary/10 border-2 border-primary/30 flex items-center justify-center shadow-[0_0_80px_rgba(0,200,83,0.25)]">
              <PhCheckCircle class="text-primary text-6xl" weight="fill"/>
            </div>
            <!-- Ripple effect -->
            <div class="absolute inset-0 rounded-full border-2 border-primary/20 animate-ping"></div>
          </div>

          <div class="space-y-3">
            <h1 class="text-4xl font-black font-heading text-white tracking-tight">Thanh toán thành công! 🎉</h1>
            <p class="text-white/50 text-[15px] font-medium leading-relaxed max-w-sm">
              Cảm ơn bạn đã mua vé. Vé điện tử sẽ được gửi đến email của bạn trong vài phút.
            </p>
          </div>

          <!-- Transaction Details -->
          <div class="w-full bg-[#111916] border border-white/5 rounded-3xl p-6 space-y-4 text-left">
            <div class="text-[11px] font-black text-primary uppercase tracking-widest mb-2">Chi tiết giao dịch</div>
            <div v-for="item in transactionDetails" :key="item.label" class="flex justify-between items-center text-[13px] py-2 border-b border-white/5 last:border-0">
              <span class="text-white/40 font-bold uppercase tracking-wider">{{ item.label }}</span>
              <span class="text-white font-bold font-mono text-xs max-w-[200px] text-right">{{ item.value }}</span>
            </div>
          </div>

          <div class="flex flex-col w-full gap-3">
            <button @click="router.push('/my-tickets')"
              class="w-full py-4 bg-primary hover:bg-primary-dark text-black font-black rounded-2xl transition-all hover:scale-[1.02] active:scale-95 text-[15px] flex items-center justify-center gap-2 cursor-pointer">
              <PhTicket weight="fill"/> Xem vé của tôi
            </button>
            <button @click="router.push('/')"
              class="w-full py-4 bg-[#111916] border border-white/10 text-white font-bold rounded-2xl transition-all hover:bg-white/5 text-[14px] flex items-center justify-center gap-2 cursor-pointer">
              <PhHouseLine weight="bold"/> Về trang chủ
            </button>
          </div>
        </div>

        <!-- Failure -->
        <div v-else key="failure" class="flex flex-col items-center text-center gap-8">
          <div class="w-32 h-32 rounded-full bg-red-500/10 border-2 border-red-500/20 flex items-center justify-center shadow-[0_0_60px_rgba(239,68,68,0.15)]">
            <PhXCircle class="text-red-400 text-6xl" weight="fill"/>
          </div>

          <div class="space-y-3">
            <h1 class="text-4xl font-black font-heading text-white tracking-tight">Thanh toán thất bại</h1>
            <p class="text-white/50 text-[15px] font-medium leading-relaxed max-w-sm">
              {{ failReason || 'Giao dịch của bạn đã bị hủy hoặc không thành công. Ghế đã chọn sẽ được giải phóng.' }}
            </p>
          </div>

          <!-- Error code if available -->
          <div v-if="vnpResponseCode" class="w-full bg-[#111916] border border-red-500/10 rounded-3xl p-5 flex items-center gap-4">
            <div class="w-10 h-10 rounded-xl bg-red-500/10 flex items-center justify-center shrink-0">
              <PhWarningCircle class="text-red-400 text-xl" weight="duotone"/>
            </div>
            <div class="text-left">
              <div class="text-red-400 font-bold text-[13px]">Mã lỗi VNPay: {{ vnpResponseCode }}</div>
              <div class="text-white/30 text-[12px] mt-0.5">{{ vnpResponseCodeMessage }}</div>
            </div>
          </div>

          <div class="flex flex-col w-full gap-3">
            <button @click="router.back()"
              class="w-full py-4 bg-[#111916] border border-white/10 text-white font-black rounded-2xl transition-all hover:bg-white/5 text-[14px] flex items-center justify-center gap-2 cursor-pointer">
              <PhArrowLeft weight="bold"/> Quay lại và thử lại
            </button>
            <button @click="router.push('/')"
              class="w-full py-4 text-white/30 font-bold text-[13px] flex items-center justify-center gap-2 cursor-pointer hover:text-white transition-colors">
              Về trang chủ
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
import {
  PhCheckCircle, PhXCircle, PhTicket, PhHouseLine,
  PhWarningCircle, PhArrowLeft, PhSpinner
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const isVerifying = ref(true)
const isSuccess = ref(false)
const failReason = ref('')
const vnpResponseCode = ref(null)

// VNPay returns these query params on callback
// vnp_ResponseCode = '00' means success
const VNP_RESPONSE_MESSAGES = {
  '07': 'Giao dịch bị nghi ngờ gian lận.',
  '09': 'Thẻ chưa đăng ký dịch vụ InternetBanking.',
  '10': 'Xác thực thông tin thẻ quá 3 lần.',
  '11': 'Phiên thanh toán đã hết hạn.',
  '12': 'Thẻ/tài khoản bị khóa.',
  '13': 'Sai mật khẩu OTP.',
  '24': 'Giao dịch bị hủy bởi khách hàng.',
  '51': 'Số dư tài khoản không đủ.',
  '65': 'Tài khoản vượt hạn mức giao dịch trong ngày.',
  '75': 'Ngân hàng đang bảo trì.',
  '79': 'Nhập sai mật khẩu thanh toán quá số lần quy định.',
  '99': 'Lỗi không xác định.',
}

const vnpResponseCodeMessage = computed(() => {
  if (!vnpResponseCode.value) return ''
  return VNP_RESPONSE_MESSAGES[vnpResponseCode.value] || 'Giao dịch không thành công.'
})

// Gather transaction details from URL query params
const transactionDetails = computed(() => {
  const q = route.query
  const items = []
  if (q.vnp_TxnRef) items.push({ label: 'Mã đơn hàng', value: q.vnp_TxnRef })
  if (q.vnp_TransactionNo) items.push({ label: 'Mã giao dịch VNPay', value: q.vnp_TransactionNo })
  if (q.vnp_BankCode) items.push({ label: 'Ngân hàng', value: q.vnp_BankCode })
  if (q.vnp_Amount) items.push({ label: 'Số tiền', value: formatAmount(q.vnp_Amount) })
  if (q.vnp_PayDate) items.push({ label: 'Thời gian', value: formatVnpDate(q.vnp_PayDate) })
  return items
})

function formatAmount(rawAmount) {
  if (!rawAmount) return ''
  const amount = parseInt(rawAmount) / 100
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount)
}

function formatVnpDate(dateStr) {
  if (!dateStr || dateStr.length < 14) return dateStr
  // Format: YYYYMMDDHHmmss
  const y = dateStr.slice(0, 4)
  const mo = dateStr.slice(4, 6)
  const d = dateStr.slice(6, 8)
  const h = dateStr.slice(8, 10)
  const mi = dateStr.slice(10, 12)
  return `${h}:${mi} - ${d}/${mo}/${y}`
}

onMounted(() => {
  const responseCode = route.query.vnp_ResponseCode

  setTimeout(() => {
    isVerifying.value = false

    if (responseCode === '00') {
      isSuccess.value = true
    } else {
      isSuccess.value = false
      vnpResponseCode.value = responseCode || null
      failReason.value = responseCode
        ? VNP_RESPONSE_MESSAGES[responseCode] || 'Giao dịch không thành công.'
        : 'Không nhận được phản hồi từ cổng thanh toán.'
    }
  }, 1000)
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
