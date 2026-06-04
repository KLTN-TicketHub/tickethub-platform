<template>
  <BaseModal :visible="visible" @close="handleClose" maxWidth="md">
    <template #header>
      <h2 class="text-xl font-heading font-bold text-main">Checkout Cart</h2>
      <p class="text-sm text-muted mt-1">Review your selected seats and complete the purchase.</p>
    </template>

    <div class="flex flex-col gap-4">
      <div class="flex items-center justify-between px-4 py-3 rounded-lg bg-surface/50 border border-border-light/20">
        <span class="text-sm text-muted">Ticket Price</span>
        <span class="font-medium text-main">₫ {{ formattedTicketPrice }}</span>
      </div>

      <div class="border border-border-main/50 rounded-xl overflow-hidden bg-bg-elevated/30">
        <div class="bg-surface/50 px-4 py-2 border-b border-border-main/50 text-xs font-semibold text-dimmed uppercase tracking-wider">
          Selected Seats ({{ selectedSeats.length }})
        </div>
        <ul class="divide-y divide-border-main/30 max-h-[300px] overflow-y-auto no-scrollbar">
          <li v-for="seat in selectedSeats" :key="seat.id" class="px-4 py-3 flex items-center justify-between hover:bg-surface/30 transition-colors">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-md bg-primary/10 border border-primary/20 flex items-center justify-center text-primary text-xs font-bold">
                {{ seat.label }}
              </div>
              <span class="text-sm text-main font-medium">Standard Admission</span>
            </div>
            <span class="text-sm text-muted">₫ {{ formattedTicketPrice }}</span>
          </li>
          <li v-if="selectedSeats.length === 0" class="px-4 py-8 text-center text-sm text-muted">
            No seats selected.
          </li>
        </ul>
      </div>

      <div class="mt-2 p-4 rounded-xl glass-panel flex items-center justify-between border-primary/20 shadow-glow">
        <span class="font-semibold text-main">Total Payment</span>
        <span class="text-xl font-heading font-bold text-primary">₫ {{ formattedTotalPrice }}</span>
      </div>
    </div>

    <template #footer>
      <div class="flex items-center justify-end gap-3 w-full">
        <BaseButton variant="ghost" @click="handleClose" :disabled="isProcessing">
          Cancel
        </BaseButton>
        <BaseButton variant="primary" :is-loading="isProcessing" :disabled="selectedSeats.length === 0" @click="handleCheckout" data-testid="btn-confirm-pay">
          Confirm & Pay
        </BaseButton>
      </div>
    </template>
  </BaseModal>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { delay } from '@/shared/composables/useMockApi'
import { useToast } from '@/shared/composables/useToast'
import BaseModal from '@/shared/components/BaseModal.vue'
import BaseButton from '@/shared/components/BaseButton.vue'

const props = defineProps({
  visible: { type: Boolean, default: false },
  selectedSeats: { type: Array, default: () => [] },
  ticketPrice: { type: Number, default: 500000 }
})

const emit = defineEmits(['close', 'success'])

const router = useRouter()
const toast = useToast()
const isProcessing = ref(false)

const formattedTicketPrice = computed(() => props.ticketPrice.toLocaleString('vi-VN'))
const totalPrice = computed(() => props.selectedSeats.length * props.ticketPrice)
const formattedTotalPrice = computed(() => totalPrice.value.toLocaleString('vi-VN'))

const handleClose = () => {
  if (isProcessing.value) return
  emit('close')
}

const handleCheckout = async () => {
  if (props.selectedSeats.length === 0) return
  
  isProcessing.value = true
  try {
    await delay(1500)
    toast.success('Tickets Booked Successfully!', 4000)
    emit('success')
    emit('close')
    router.push('/my-tickets')
  } catch (error) {
    toast.error('Booking failed. Please try again.')
  } finally {
    isProcessing.value = false
  }
}
</script>
