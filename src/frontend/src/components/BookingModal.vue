<template>
  <div v-if="event" class="fixed inset-0 bg-black/80 backdrop-blur-sm z-[500] flex items-center justify-center p-4" @click="$emit('close')">
    <div class="bg-surface border border-border-main rounded-[32px] w-full max-w-[560px] max-h-[90vh] overflow-hidden shadow-2xl animate-in zoom-in duration-300 flex flex-col" @click.stop>
      <!-- Header -->
      <div class="flex justify-between items-center p-6 px-8 border-b border-border-main bg-card/50">
        <h2 class="font-heading text-2xl font-bold text-main">Đặt vé sự kiện</h2>
        <button class="w-8 h-8 flex items-center justify-center rounded-full bg-surface border border-border-main text-muted hover:text-danger hover:border-danger transition-all" @click="$emit('close')">✕</button>
      </div>

      <div class="flex-1 overflow-y-auto">
        <!-- Event Quick Info -->
        <div class="flex gap-4 p-6 px-8 bg-card/30 border-b border-border-main">
          <img :src="event.image" alt="Event" class="w-20 h-20 rounded-xl object-cover border border-border-main" />
          <div class="flex flex-col justify-center gap-1">
            <h3 class="font-bold text-main leading-tight">{{ event.title }}</h3>
            <div class="text-[13px] text-muted flex flex-wrap gap-x-4">
              <span>📅 {{ formatEventDate }} · {{ event.time || '' }}</span>
              <span>📍 {{ eventLocation }}</span>
            </div>
          </div>
        </div>

        <!-- Steps Progress -->
        <div class="flex items-center px-8 py-6">
          <div v-for="s in 3" :key="s" class="flex-1 flex items-center group">
            <div class="flex flex-col items-center gap-2 relative z-10">
              <div 
                class="w-8 h-8 rounded-full flex items-center justify-center text-[13px] font-bold transition-all border-2"
                :class="[
                  step === s ? 'bg-primary border-primary text-black scale-110 shadow-lg shadow-primary/20' : 
                  step > s ? 'bg-primary border-primary text-black' : 
                  'bg-surface border-border-main text-muted'
                ]"
              >
                {{ step > s ? '✓' : s }}
              </div>
              <span 
                class="text-[11px] font-bold uppercase tracking-wider transition-colors"
                :class="step >= s ? 'text-main' : 'text-muted'"
              >
                {{ s === 1 ? 'Chọn vé' : s === 2 ? 'Thông tin' : 'Thanh toán' }}
              </span>
            </div>
            <!-- Connector Line -->
            <div 
              v-if="s < 3" 
              class="flex-1 h-[2px] mx-4 -mt-6 transition-all duration-500"
              :class="step > s ? 'bg-primary' : 'bg-border-main'"
            ></div>
          </div>
        </div>

        <!-- STEP 1: Select Tickets / Seats -->
        <div v-if="step === 1" class="px-8 pb-8 flex flex-col gap-4">
          
          <!-- Seat Map Mode -->
          <template v-if="event.hasSeatMap">
            <div class="bg-card/50 border border-border-main rounded-2xl p-6">
              <SeatMap :seatMap="event.seatMap" v-model="selectedSeats" />
            </div>
            <!-- Selected Seats Summary -->
            <div v-if="selectedSeats.length > 0" class="flex flex-wrap gap-2 mt-2">
              <span class="text-[12px] text-muted font-bold uppercase mr-2 flex items-center">Ghế đã chọn:</span>
              <div v-for="seat in selectedSeats" :key="seat.id" class="px-2 py-1 rounded bg-primary/20 text-primary border border-primary/30 text-[12px] font-bold">
                {{ seat.id }} ({{ seat.tierName }})
              </div>
            </div>
          </template>

          <!-- Standard Tier Selection Mode -->
          <template v-else>
            <div 
              v-for="tier in availableTiers" 
              :key="tier.name" 
              class="flex items-center justify-between p-5 rounded-2xl border-2 cursor-pointer transition-all hover:bg-card"
              :class="[
                selectedTier === tier 
                  ? 'bg-primary/10 border-primary shadow-lg shadow-primary/10' 
                  : 'bg-card/50 border-border-main hover:border-primary/50'
              ]"
              @click="selectedTier = tier"
            >
              <div class="flex flex-col gap-1">
                <div class="font-bold" :class="selectedTier === tier ? 'text-primary' : 'text-main'">{{ tier.name }}</div>
                <div class="font-heading text-lg font-bold" :class="selectedTier === tier ? 'text-primary' : 'text-primary/70'">{{ formatPrice(tier.price) }}</div>
              </div>
              
              <!-- Quantity Control (only when selected) -->
              <div v-if="selectedTier === tier" class="flex items-center gap-3 bg-surface p-1 px-2 rounded-lg border border-border-main" @click.stop>
                <button 
                  class="w-7 h-7 rounded bg-card flex items-center justify-center text-main hover:text-primary transition-colors disabled:opacity-30" 
                  @click="qty > 1 ? qty-- : null"
                  :disabled="qty <= 1"
                >−</button>
                <div class="w-6 text-center font-bold text-main">{{ qty }}</div>
                <button 
                  class="w-7 h-7 rounded bg-card flex items-center justify-center text-main hover:text-primary transition-colors disabled:opacity-30" 
                  @click="qty < 10 ? qty++ : null"
                  :disabled="qty >= 10"
                >+</button>
              </div>
            </div>
          </template>
        </div>

        <!-- STEP 2: User Info -->
        <div v-if="step === 2" class="px-8 pb-8 flex flex-col gap-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="flex flex-col gap-1.5">
              <label class="text-[12px] font-bold text-muted uppercase tracking-wider ml-1">Họ và tên *</label>
              <input type="text" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="formData.name" placeholder="Ví dụ: Nguyễn Văn A" />
            </div>
            <div class="flex flex-col gap-1.5">
              <label class="text-[12px] font-bold text-muted uppercase tracking-wider ml-1">Số điện thoại *</label>
              <input type="tel" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="formData.phone" placeholder="09xxxx..." />
            </div>
          </div>
          <div class="flex flex-col gap-1.5">
            <label class="text-[12px] font-bold text-muted uppercase tracking-wider ml-1">Email *</label>
            <input type="email" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="formData.email" placeholder="email@example.com" />
          </div>
          <div class="flex flex-col gap-1.5">
            <label class="text-[12px] font-bold text-muted uppercase tracking-wider ml-1">Ghi chú (tuỳ chọn)</label>
            <textarea class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors resize-none h-20" v-model="formData.note" placeholder="VD: Cần hỗ trợ xe lăn..."></textarea>
          </div>

          <!-- Order Summary Small -->
          <div class="bg-card/50 border border-border-main rounded-2xl p-5 flex flex-col gap-3">
            <div class="flex justify-between text-[14px]">
              <span class="text-muted">Loại vé: <span class="text-main font-bold">{{ event.hasSeatMap ? (selectedSeats.length ? 'Ghế ngồi' : '') : selectedTier?.name }}</span></span>
              <span class="text-muted">Số lượng: <span class="text-main font-bold">x{{ event.hasSeatMap ? selectedSeats.length : qty }}</span></span>
            </div>
            <div class="h-[1px] bg-border-main"></div>
            <div class="flex justify-between items-center">
              <span class="text-[14px] font-bold text-main">Tổng cộng</span>
              <span class="font-heading text-xl font-bold text-primary">{{ formatPrice(totalPrice) }}</span>
            </div>
          </div>
        </div>

        <!-- STEP 3: Payment & Success -->
        <div v-if="step === 3" class="px-8 pb-8 min-h-[300px] flex flex-col items-center justify-center text-center">
          <!-- Selection Mode -->
          <div v-if="!isProcessing && !isSuccess" class="w-full">
            <h3 class="font-heading text-xl font-bold text-main mb-6">Chọn phương thức thanh toán</h3>
            <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 mb-8">
              <div
                v-for="pm in paymentMethods"
                :key="pm.id"
                class="flex flex-col items-center gap-2 p-4 rounded-xl border-2 cursor-pointer transition-all"
                :class="[
                  selectedPayment === pm.id 
                    ? 'bg-primary/10 border-primary' 
                    : 'bg-card border-border-main hover:border-primary/50'
                ]"
                @click="selectedPayment = pm.id"
              >
                <div class="text-2xl">{{ pm.icon }}</div>
                <span class="text-[13px] font-bold" :class="selectedPayment === pm.id ? 'text-primary' : 'text-muted'">{{ pm.label }}</span>
              </div>
            </div>
            <div class="bg-primary/5 border border-primary/20 rounded-2xl p-6 mb-8">
              <div class="text-[14px] text-muted mb-1">Số tiền cần thanh toán</div>
              <div class="font-heading text-4xl font-bold text-primary">{{ formatPrice(totalPrice) }}</div>
            </div>
          </div>

          <!-- Processing Mode -->
          <div v-if="isProcessing" class="py-12 flex flex-col items-center gap-6 animate-pulse">
            <div class="w-16 h-16 border-4 border-border-main border-t-primary rounded-full animate-spin"></div>
            <p class="text-[15px] font-medium text-muted">Đang xử lý giao dịch an toàn...</p>
          </div>

          <!-- Success Mode -->
          <div v-if="isSuccess" class="py-4 w-full flex flex-col items-center animate-in fade-in zoom-in duration-500">
            <div class="w-20 h-20 bg-primary/20 text-primary rounded-full flex items-center justify-center text-4xl mb-6 shadow-xl shadow-primary/20">🎉</div>
            <h3 class="font-heading text-2xl font-bold text-primary mb-2">Thanh toán thành công!</h3>
            <p class="text-[14px] text-muted mb-8">Vé điện tử đã được gửi tới email <br><b class="text-main">{{ formData.email }}</b></p>

            <div class="w-full max-w-[320px] bg-card border-2 border-dashed border-primary rounded-[24px] p-6 mb-8 relative overflow-hidden group">
              <!-- Ticket Notches -->
              <div class="absolute left-[-12px] top-1/2 -translate-y-1/2 w-6 h-6 rounded-full bg-surface"></div>
              <div class="absolute right-[-12px] top-1/2 -translate-y-1/2 w-6 h-6 rounded-full bg-surface"></div>
              
              <div class="w-32 h-32 bg-white/10 rounded-xl mb-4 mx-auto flex items-center justify-center text-4xl bg-[repeating-linear-gradient(45deg,rgba(0,200,83,0.1)_0,rgba(0,200,83,0.1)_10px,transparent_10px,transparent_20px)]">QR</div>
              <div class="font-mono text-lg font-bold text-main mb-1 tracking-widest">{{ generatedCode }}</div>
              <div class="text-[13px] text-muted font-bold truncate px-4">{{ event.title }}</div>
              <div class="mt-4 pt-4 border-t border-border-main flex justify-between text-[11px] font-bold uppercase tracking-wider text-muted">
                <span>{{ event.hasSeatMap ? 'Ghế: ' + selectedSeats.map(s => s.id).join(', ') : selectedTier?.name }}</span>
                <span>Số lượng: {{ event.hasSeatMap ? selectedSeats.length : qty }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sticky Footer / Summary Bar -->
      <div v-if="!isProcessing && !isSuccess" class="p-6 px-8 border-t border-border-main bg-card/50 flex items-center justify-between gap-6">
        <div v-if="step === 1">
          <div class="text-[13px] text-muted font-bold uppercase tracking-wider">Tổng cộng</div>
          <div class="font-heading text-2xl font-bold text-primary leading-none">{{ formatPrice(totalPrice) }}</div>
        </div>
        
        <BaseButton 
          v-if="step > 1" 
          variant="ghost" 
          @click="step--"
          class="!px-6"
        >
          ← Quay lại
        </BaseButton>

        <BaseButton 
          v-if="step === 1" 
          variant="primary" 
          size="lg" 
          class="!px-10 !rounded-2xl"
          :disabled="event.hasSeatMap ? selectedSeats.length === 0 : !selectedTier" 
          @click="step = 2"
        >
          Tiếp tục →
        </BaseButton>

        <BaseButton 
          v-if="step === 2" 
          variant="primary" 
          size="lg" 
          class="!px-10 !rounded-2xl"
          :disabled="!isFormValid" 
          @click="step = 3"
        >
          Tiếp tục thanh toán
        </BaseButton>

        <BaseButton 
          v-if="step === 3" 
          variant="primary" 
          size="lg" 
          class="w-full !rounded-2xl shadow-xl shadow-primary/20"
          @click="processPayment"
        >
          Xác nhận thanh toán
        </BaseButton>
      </div>

      <div v-if="isSuccess" class="p-6 px-8 border-t border-border-main bg-card/50">
        <BaseButton variant="primary" size="lg" class="w-full !rounded-2xl shadow-xl shadow-primary/20" @click="finishBooking">
          🎟️ Xem vé của tôi
        </BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import BaseButton from './ui/BaseButton.vue'
import SeatMap from './SeatMap.vue'

const props = defineProps({ event: Object })
const emit = defineEmits(['close', 'success'])

const step = ref(1)
const selectedTier = ref(null)
const qty = ref(1)
const selectedSeats = ref([])
const isProcessing = ref(false)
const isSuccess = ref(false)
const generatedCode = ref('')
const selectedPayment = ref('momo')

const formData = ref({ name: 'Nguyễn Văn A', phone: '0901234567', email: 'user@example.com', note: '' })

const paymentMethods = [
  { id: 'momo', icon: '💜', label: 'MoMo' },
  { id: 'vnpay', icon: '🏦', label: 'VNPay' },
  { id: 'card', icon: '💳', label: 'Thẻ tín dụng' },
  { id: 'bank', icon: '🏧', label: 'Chuyển khoản' },
]

// Handle unified schema: dateStart + location object
const formatEventDate = computed(() => {
  const dateStr = props.event?.dateStart || props.event?.date || ''
  if (!dateStr) return ''
  try {
    const d = new Date(dateStr)
    if (isNaN(d.getTime())) return dateStr
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
  } catch(e) {
    return dateStr
  }
})

const eventLocation = computed(() => {
  if (typeof props.event?.location === 'object') return props.event.location.name || ''
  return props.event?.location || props.event?.venue || ''
})

const availableTiers = computed(() => {
  if (props.event?.tiers && props.event.tiers.length > 0) return props.event.tiers
  const price = props.event?.priceRange?.min || (typeof props.event?.price === 'string' ? parseInt(props.event.price.replace(/\D/g, '')) || 0 : props.event?.price || 0)
  return [{ name: 'Vé tiêu chuẩn', price }]
})

const totalPrice = computed(() => {
  if (props.event?.hasSeatMap) {
    return selectedSeats.value.reduce((total, seat) => {
      const tier = availableTiers.value.find(t => t.name === seat.tierName)
      return total + (tier ? tier.price : 0)
    }, 0)
  }
  return selectedTier.value ? selectedTier.value.price * qty.value : 0
})

const isFormValid = computed(() => formData.value.name && formData.value.phone && formData.value.email)

const formatPrice = (val) => {
  if (val === 0) return 'Miễn phí'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

const processPayment = () => {
  isProcessing.value = true
  setTimeout(() => {
    isProcessing.value = false
    isSuccess.value = true
    generatedCode.value = 'ES-' + Math.random().toString(36).substr(2, 6).toUpperCase()
  }, 2000)
}

const finishBooking = () => {
  emit('success', {
    id: Date.now().toString(),
    event: props.event,
    tier: props.event?.hasSeatMap 
      ? Array.from(new Set(selectedSeats.value.map(s => s.tierName))).join(', ') 
      : (selectedTier.value?.name || 'Standard'),
    qty: props.event?.hasSeatMap ? selectedSeats.value.length : qty.value,
    seats: props.event?.hasSeatMap ? selectedSeats.value : [],
    total: totalPrice.value,
    code: generatedCode.value,
    date: new Date().toISOString()
  })
}
</script>
