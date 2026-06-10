<template>
  <Transition name="modal">
    <div v-if="event" class="fixed inset-0 z-[2000] flex items-center justify-center p-4 lg:p-6">
      <div class="absolute inset-0 bg-[#050807]/90 backdrop-blur-xl transition-opacity duration-300" @click="$emit('close')"></div>
      
      <div class="relative bg-[#0A0F0D] border border-white/10 rounded-[2.5rem] w-full max-w-[600px] max-h-[90vh] overflow-hidden shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] flex flex-col modal-content" @click.stop>
        
        <!-- Header -->
        <div class="flex justify-between items-center p-6 lg:p-8 border-b border-white/5 bg-[#111916]/50 backdrop-blur-md relative z-20">
          <div>
            <h2 class="text-2xl font-black font-heading text-white tracking-tight">Đặt vé sự kiện</h2>
            <p class="text-[13px] text-white/50 font-medium mt-1">Hoàn tất thủ tục đặt vé nhanh chóng an toàn.</p>
          </div>
          <button class="w-10 h-10 flex items-center justify-center rounded-full bg-white/5 hover:bg-white/10 border border-white/5 text-white/50 hover:text-white transition-all cursor-pointer shrink-0" @click="$emit('close')">
            <PhX weight="bold" />
          </button>
        </div>

        <div class="flex-1 overflow-y-auto custom-scrollbar relative z-10">
          <!-- Event Quick Info -->
          <div class="flex flex-col sm:flex-row gap-5 p-6 lg:p-8 bg-[#111916]/30 border-b border-white/5">
            <div class="w-full sm:w-28 h-28 shrink-0 rounded-[1.25rem] overflow-hidden border border-white/5 relative">
              <img :src="event.image" alt="Event" class="w-full h-full object-cover" />
              <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
            </div>
            <div class="flex flex-col justify-center gap-2">
              <h3 class="text-xl font-black text-white leading-tight tracking-tight">{{ event.title }}</h3>
              <div class="text-[13px] text-white/50 font-medium flex flex-wrap items-center gap-x-4 gap-y-2">
                <span class="flex items-center gap-1.5"><PhCalendarBlank weight="bold" /> {{ formatEventDate }} · {{ event.time || '' }}</span>
                <span class="flex items-center gap-1.5"><PhMapPin weight="bold" /> {{ eventLocation }}</span>
              </div>
            </div>
          </div>

          <!-- Steps Progress -->
          <div class="flex items-center px-8 py-8 relative">
            <div v-for="s in 3" :key="s" class="flex-1 flex items-center group relative z-10">
              <div class="flex flex-col items-center gap-3 w-full relative">
                <div 
                  class="w-10 h-10 rounded-full flex items-center justify-center text-[14px] font-black transition-all duration-300 border border-white/10"
                  :class="[
                    step === s ? 'bg-primary text-black scale-110 shadow-[0_0_20px_rgba(0,200,83,0.3)] border-transparent' : 
                    step > s ? 'bg-[#00A355] text-black border-transparent' : 
                    'bg-[#111916] text-white/30'
                  ]"
                >
                  <PhCheck v-if="step > s" weight="bold" class="text-lg" />
                  <span v-else>{{ s }}</span>
                </div>
                <span 
                  class="text-[11px] font-black uppercase tracking-widest transition-colors duration-300 absolute -bottom-6 whitespace-nowrap"
                  :class="step >= s ? 'text-white' : 'text-white/30'"
                >
                  {{ s === 1 ? 'Chọn vé' : s === 2 ? 'Thông tin' : 'Thanh toán' }}
                </span>
              </div>
            </div>
            
            <!-- Progress Line Background -->
            <div class="absolute left-16 right-16 top-[48px] h-1 bg-[#111916] rounded-full z-0"></div>
            <!-- Progress Line Fill -->
            <div class="absolute left-16 top-[48px] h-1 bg-primary rounded-full z-0 transition-all duration-500" :style="`width: ${((step - 1) / 2) * 100}%`"></div>
          </div>

          <!-- Content Padding Spacer -->
          <div class="h-6"></div>

          <!-- STEP 1: Select Tickets / Seats -->
          <div v-if="step === 1" class="px-6 lg:px-8 pb-8 flex flex-col gap-5 animate-in fade-in slide-in-from-right-4 duration-500">
            <!-- Seat Map Mode -->
            <template v-if="event.hasSeatMap">
              <div class="bg-[#111916] border border-white/5 rounded-[2rem] p-6 lg:p-8 overflow-hidden relative">
                <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_center,_var(--tw-gradient-stops))] from-white/5 to-transparent pointer-events-none"></div>
                <SeatMap :seatMap="event.seatMap" v-model="selectedSeats" class="relative z-10" />
              </div>
              <!-- Selected Seats Summary -->
              <div v-if="selectedSeats.length > 0" class="flex flex-wrap items-center gap-2 mt-2 bg-primary/5 border border-primary/20 p-4 rounded-2xl">
                <PhArmchair weight="fill" class="text-primary text-lg" />
                <span class="text-[12px] text-white/70 font-bold uppercase tracking-widest mr-2">Ghế đã chọn:</span>
                <div v-for="seat in selectedSeats" :key="seat.id" class="px-3 py-1.5 rounded-lg bg-primary/20 text-primary text-[12px] font-black border border-primary/30">
                  {{ seat.id }} <span class="opacity-50 font-medium">({{ seat.tierName }})</span>
                </div>
              </div>
            </template>

            <!-- Standard Tier Selection Mode -->
            <template v-else>
              <div 
                v-for="tier in availableTiers" 
                :key="tier.name" 
                class="flex items-center justify-between p-6 rounded-[1.5rem] border transition-all cursor-pointer relative overflow-hidden group"
                :class="[
                  selectedTier === tier 
                    ? 'bg-primary/5 border-primary shadow-[0_0_20px_rgba(0,200,83,0.1)]' 
                    : 'bg-[#111916] border-white/5 hover:border-white/20'
                ]"
                @click="selectedTier = tier"
              >
                <!-- Checked Indicator -->
                <div v-if="selectedTier === tier" class="absolute top-0 right-0 w-16 h-16 bg-primary/10 rounded-bl-[100%]">
                  <PhCheckCircle weight="fill" class="absolute top-3 right-3 text-primary text-xl" />
                </div>

                <div class="flex flex-col gap-1.5 relative z-10">
                  <div class="font-bold text-[15px]" :class="selectedTier === tier ? 'text-primary' : 'text-white'">{{ tier.name }}</div>
                  <div class="font-heading text-2xl font-black" :class="selectedTier === tier ? 'text-white' : 'text-white/50'">{{ formatPrice(tier.price) }}</div>
                </div>
                
                <!-- Quantity Control (only when selected) -->
                <div v-if="selectedTier === tier" class="flex items-center gap-3 bg-[#0A0F0D] p-1.5 rounded-xl border border-white/10 relative z-10" @click.stop>
                  <button 
                    class="w-8 h-8 rounded-lg bg-white/5 flex items-center justify-center text-white hover:bg-white/10 hover:text-primary transition-all disabled:opacity-30 disabled:hover:text-white" 
                    @click="qty > 1 ? qty-- : null"
                    :disabled="qty <= 1"
                  ><PhMinus weight="bold" /></button>
                  <div class="w-6 text-center font-black text-white">{{ qty }}</div>
                  <button 
                    class="w-8 h-8 rounded-lg bg-white/5 flex items-center justify-center text-white hover:bg-white/10 hover:text-primary transition-all disabled:opacity-30 disabled:hover:text-white" 
                    @click="qty < 10 ? qty++ : null"
                    :disabled="qty >= 10"
                  ><PhPlus weight="bold" /></button>
                </div>
              </div>
            </template>
          </div>

          <!-- STEP 2: User Info -->
          <div v-if="step === 2" class="px-6 lg:px-8 pb-8 flex flex-col gap-6 animate-in fade-in slide-in-from-right-4 duration-500">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
              <div class="relative group">
                <PhUser class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors" weight="bold" />
                <input type="text" class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors placeholder:text-white/20" v-model="formData.name" placeholder="Họ và tên *" />
              </div>
              <div class="relative group">
                <PhPhone class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors" weight="bold" />
                <input type="tel" class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors placeholder:text-white/20" v-model="formData.phone" placeholder="Số điện thoại *" />
              </div>
            </div>
            <div class="relative group">
              <PhEnvelopeSimple class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors" weight="bold" />
              <input type="email" class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors placeholder:text-white/20" v-model="formData.email" placeholder="Email nhận vé *" />
            </div>
            <div class="relative group">
              <PhNote class="absolute left-5 top-5 text-white/30 group-focus-within:text-primary transition-colors" weight="bold" />
              <textarea class="w-full bg-[#111916] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-medium text-white/80 outline-none focus:border-primary/50 transition-colors resize-none h-24 placeholder:text-white/20" v-model="formData.note" placeholder="Ghi chú thêm (tùy chọn)"></textarea>
            </div>

            <!-- Order Summary Small -->
            <div class="bg-[#111916] border border-white/5 rounded-[1.5rem] p-6 flex flex-col gap-4 mt-2">
              <div class="flex justify-between items-center text-[14px]">
                <span class="text-white/50 font-medium">Hạng vé</span>
                <span class="text-white font-bold">{{ event.hasSeatMap ? (selectedSeats.length ? 'Ghế ngồi' : '') : selectedTier?.name }}</span>
              </div>
              <div class="flex justify-between items-center text-[14px]">
                <span class="text-white/50 font-medium">Số lượng</span>
                <span class="text-white font-bold bg-white/5 px-3 py-1 rounded-lg">x{{ event.hasSeatMap ? selectedSeats.length : qty }}</span>
              </div>
              <div class="h-px bg-white/5 my-1"></div>
              <div class="flex justify-between items-center">
                <span class="text-[13px] font-bold text-white/50 uppercase tracking-widest">Tổng cộng</span>
                <span class="font-heading text-2xl font-black text-primary">{{ formatPrice(totalPrice) }}</span>
              </div>
            </div>
          </div>

          <!-- STEP 3: Payment & Success -->
          <div v-if="step === 3" class="px-6 lg:px-8 pb-8 min-h-[350px] flex flex-col items-center justify-center text-center animate-in fade-in slide-in-from-right-4 duration-500">
            <!-- Selection Mode -->
            <div v-if="!isProcessing && !isSuccess" class="w-full">
              <h3 class="font-heading text-2xl font-black text-white mb-8 tracking-tight">Thanh toán an toàn</h3>
              <div class="grid grid-cols-2 gap-4 mb-8">
                <div
                  v-for="pm in paymentMethods"
                  :key="pm.id"
                  class="flex flex-col items-center gap-3 p-5 rounded-[1.5rem] border border-white/5 transition-all cursor-pointer group relative overflow-hidden"
                  :class="[
                    selectedPayment === pm.id 
                      ? 'bg-primary/10 border-primary' 
                      : 'bg-[#111916] hover:bg-white/5'
                  ]"
                  @click="selectedPayment = pm.id"
                >
                  <component :is="pm.icon" weight="duotone" class="text-4xl transition-colors" :class="selectedPayment === pm.id ? 'text-primary' : 'text-white/50 group-hover:text-white'" />
                  <span class="text-[13px] font-bold transition-colors" :class="selectedPayment === pm.id ? 'text-primary' : 'text-white/70 group-hover:text-white'">{{ pm.label }}</span>
                  
                  <div v-if="selectedPayment === pm.id" class="absolute top-2 right-2">
                    <PhCheckCircle weight="fill" class="text-primary" />
                  </div>
                </div>
              </div>
              
              <div class="bg-primary/10 border border-primary/20 rounded-[1.5rem] p-6 shadow-inner">
                <div class="text-[12px] font-bold text-primary/70 uppercase tracking-widest mb-2">Số tiền cần thanh toán</div>
                <div class="font-heading text-4xl lg:text-5xl font-black text-primary tracking-tighter">{{ formatPrice(totalPrice) }}</div>
              </div>
            </div>

            <!-- Processing Mode -->
            <div v-if="isProcessing" class="py-16 flex flex-col items-center gap-6 animate-pulse w-full">
              <div class="w-20 h-20 border-[6px] border-[#111916] border-t-primary rounded-full animate-spin"></div>
              <div class="space-y-2">
                <h4 class="text-xl font-black text-white">Đang xử lý giao dịch</h4>
                <p class="text-[15px] font-medium text-white/50">Vui lòng không đóng cửa sổ này...</p>
              </div>
            </div>

            <!-- Success Mode -->
            <div v-if="isSuccess" class="py-8 w-full flex flex-col items-center animate-in fade-in zoom-in duration-500">
              <div class="w-24 h-24 bg-primary/10 border border-primary/20 text-primary rounded-[2rem] flex items-center justify-center text-5xl mb-6 shadow-[0_0_40px_rgba(0,200,83,0.3)]">
                <PhConfetti weight="duotone" />
              </div>
              <h3 class="font-heading text-3xl font-black text-white mb-3 tracking-tight">Giao dịch thành công!</h3>
              <p class="text-[15px] text-white/50 font-medium mb-10 max-w-sm">Vé điện tử đã được mã hóa và gửi tới email <br><b class="text-white">{{ formData.email }}</b></p>

              <!-- Ticket Stub Design -->
              <div class="w-full max-w-[340px] bg-[#111916] border border-white/10 rounded-[2rem] p-8 mb-4 relative shadow-2xl">
                <!-- Ticket Notches -->
                <div class="absolute -left-4 top-1/2 -translate-y-1/2 w-8 h-8 rounded-full bg-[#0A0F0D] border-r border-white/10 shadow-inner"></div>
                <div class="absolute -right-4 top-1/2 -translate-y-1/2 w-8 h-8 rounded-full bg-[#0A0F0D] border-l border-white/10 shadow-inner"></div>
                
                <div class="w-40 h-40 bg-white p-4 rounded-3xl mb-6 mx-auto shadow-xl relative overflow-hidden">
                  <div class="w-full h-full bg-[repeating-linear-gradient(45deg,#000_0,#000_4px,transparent_4px,transparent_8px),repeating-linear-gradient(-45deg,#000_0,#000_4px,transparent_4px,transparent_8px)] opacity-90 rounded-xl"></div>
                  <div class="absolute inset-0 flex items-center justify-center">
                    <div class="bg-white px-3 py-1.5 rounded-lg text-[11px] font-black border border-black/10 tracking-widest text-black">ES-TICKET</div>
                  </div>
                </div>
                
                <div class="space-y-4 relative z-10">
                  <div>
                    <div class="text-[11px] font-bold text-white/40 uppercase tracking-widest mb-1">Mã xác nhận</div>
                    <div class="font-mono text-xl font-black text-white tracking-[0.2em]">{{ generatedCode }}</div>
                  </div>
                  
                  <div class="h-px w-full border-b border-dashed border-white/10"></div>
                  
                  <div class="text-left">
                    <div class="text-[15px] font-bold text-white truncate">{{ event.title }}</div>
                    <div class="flex justify-between items-center mt-3 text-[12px] font-bold text-white/50 uppercase tracking-widest">
                      <span class="text-primary truncate max-w-[140px]">{{ event.hasSeatMap ? 'Ghế: ' + selectedSeats.map(s => s.id).join(', ') : selectedTier?.name }}</span>
                      <span>SL: {{ event.hasSeatMap ? selectedSeats.length : qty }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sticky Footer / Summary Bar -->
        <div v-if="!isProcessing && !isSuccess" class="p-6 lg:px-8 border-t border-white/5 bg-[#111916]/80 backdrop-blur-md flex flex-col sm:flex-row items-center justify-between gap-6 relative z-20">
          <div v-if="step === 1" class="w-full sm:w-auto flex justify-between sm:flex-col items-center sm:items-start">
            <div class="text-[12px] text-white/50 font-bold uppercase tracking-widest">Tổng cộng</div>
            <div class="font-heading text-3xl font-black text-primary leading-none tracking-tighter">{{ formatPrice(totalPrice) }}</div>
          </div>
          <div v-else class="hidden sm:block"></div>
          
          <div class="flex items-center gap-3 w-full sm:w-auto">
            <BaseButton 
              v-if="step > 1" 
              variant="ghost" 
              @click="step--"
              class="!px-6 !py-4 rounded-xl text-white/50 hover:text-white hover:bg-white/5"
            >
              Quay lại
            </BaseButton>

            <BaseButton 
              v-if="step === 1" 
              variant="primary" 
              size="lg" 
              class="w-full sm:w-auto !px-10 !py-4 !rounded-xl shadow-[0_0_30px_rgba(0,200,83,0.2)] text-[15px]"
              :disabled="event.hasSeatMap ? selectedSeats.length === 0 : !selectedTier" 
              @click="step = 2"
            >
              Tiếp tục
            </BaseButton>

            <BaseButton 
              v-if="step === 2" 
              variant="primary" 
              size="lg" 
              class="w-full sm:w-auto !px-10 !py-4 !rounded-xl shadow-[0_0_30px_rgba(0,200,83,0.2)] text-[15px]"
              :disabled="!isFormValid" 
              @click="step = 3"
            >
              Xác nhận thông tin
            </BaseButton>

            <BaseButton 
              v-if="step === 3" 
              variant="primary" 
              size="lg" 
              class="w-full sm:w-auto !px-10 !py-4 !rounded-xl shadow-[0_0_30px_rgba(0,200,83,0.2)] text-[15px] flex items-center justify-center gap-2"
              @click="processPayment"
            >
              <PhLockKey weight="bold" /> Thanh toán
            </BaseButton>
          </div>
        </div>

        <div v-if="isSuccess" class="p-6 lg:p-8 border-t border-white/5 bg-[#111916]/80 backdrop-blur-md relative z-20">
          <BaseButton variant="primary" size="lg" class="w-full !py-5 !rounded-2xl shadow-[0_0_40px_rgba(0,200,83,0.3)] text-[16px] flex items-center justify-center gap-2" @click="finishBooking">
            <PhWallet weight="fill" class="text-xl" /> Xem vé trong Ví
          </BaseButton>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { ref, computed, markRaw } from 'vue'
import BaseButton from './ui/BaseButton.vue'
import SeatMap from './SeatMap.vue'
import { 
  PhX, PhCalendarBlank, PhMapPin, PhCheck, PhArmchair, 
  PhCheckCircle, PhMinus, PhPlus, PhUser, PhPhone, 
  PhEnvelopeSimple, PhNote, PhWallet, PhBank, PhCreditCard, 
  PhLockKey, PhConfetti
} from '@phosphor-icons/vue'

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
  { id: 'momo', icon: markRaw(PhWallet), label: 'Ví MoMo' },
  { id: 'vnpay', icon: markRaw(PhBank), label: 'VNPay' },
  { id: 'card', icon: markRaw(PhCreditCard), label: 'Thẻ tín dụng' },
  { id: 'bank', icon: markRaw(PhBank), label: 'Chuyển khoản' },
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

<style scoped>
/* Modal Transition */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .modal-content {
  animation: modal-pop 0.5s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.modal-leave-active .modal-content {
  animation: modal-pop-out 0.3s cubic-bezier(0.5, 0, 0.5, 1) forwards;
}

@keyframes modal-pop {
  0% { opacity: 0; transform: scale(0.95) translateY(20px); }
  100% { opacity: 1; transform: scale(1) translateY(0); }
}

@keyframes modal-pop-out {
  0% { opacity: 1; transform: scale(1) translateY(0); }
  100% { opacity: 0; transform: scale(0.95) translateY(20px); }
}

/* Custom Scrollbar for Modal Content */
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
}
.custom-scrollbar:hover::-webkit-scrollbar-thumb {
  background-color: rgba(255, 255, 255, 0.2);
}
</style>
