<template>
  <div class="animate-fade-up max-w-5xl mx-auto flex flex-col min-h-[calc(100vh-160px)]">
    <!-- Event Header -->
    <div class="mb-6 flex flex-col md:flex-row md:items-end justify-between gap-4">
      <div>
        <div class="flex items-center gap-2 text-primary font-medium text-sm mb-2">
          <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path><path d="M13.73 21a2 2 0 0 1-3.46 0"></path></svg>
          Almost Sold Out
        </div>
        <h1 class="text-3xl font-heading font-bold text-main mb-2">
          Summer Music Festival 2026
        </h1>
        <div class="flex flex-wrap items-center gap-4 text-sm text-muted">
          <div class="flex items-center gap-1.5">
            <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
            15 Aug 2026 • 18:00
          </div>
          <div class="flex items-center gap-1.5">
            <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
            MyDinh National Stadium
          </div>
        </div>
      </div>
      <div class="glass-panel px-4 py-2 text-center shrink-0">
        <span class="text-xs text-dimmed uppercase tracking-wider block mb-0.5">Standard Admission</span>
        <span class="text-xl font-bold text-primary">₫ {{ ticketPrice.toLocaleString('vi-VN') }}</span>
      </div>
    </div>

    <!-- Main Content: Seat Map -->
    <div class="flex-1 glass-panel rounded-2xl p-6 relative overflow-hidden flex flex-col mb-24">
       <div class="mb-4">
         <h2 class="text-lg font-heading font-semibold text-main">Select Your Seats</h2>
         <p class="text-sm text-muted">Click on available seats to add them to your cart.</p>
       </div>
       <div class="flex-1 flex items-center justify-center min-h-[400px]">
          <SeatMap v-model="selectedSeats" />
       </div>
    </div>

    <!-- Sticky Bottom Bar -->
    <div class="fixed bottom-0 left-0 right-0 z-40 bg-bg-elevated/90 backdrop-blur-xl border-t border-border-main/50 shadow-[0_-10px_40px_rgba(0,0,0,0.5)] transform translate-y-0 transition-transform duration-300">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 h-20 flex items-center justify-between">
        <div class="flex items-center gap-4">
          <div class="w-12 h-12 rounded-xl bg-surface/50 border border-border-light flex flex-col items-center justify-center">
            <span class="text-lg font-bold text-main leading-none">{{ selectedSeats.length }}</span>
            <span class="text-[10px] text-muted uppercase tracking-wider">Seats</span>
          </div>
          <div class="hidden sm:flex flex-col">
            <span class="text-sm text-muted">Total Amount</span>
            <span class="text-xl font-heading font-bold text-primary">₫ {{ totalPrice.toLocaleString('vi-VN') }}</span>
          </div>
        </div>
        
        <div class="flex items-center gap-3">
          <div class="flex flex-col sm:hidden items-end mr-2">
            <span class="text-xs text-muted">Total</span>
            <span class="text-lg font-bold text-primary leading-none">₫ {{ (totalPrice / 1000).toFixed(0) }}k</span>
          </div>
          <BaseButton 
            variant="primary" 
            size="lg" 
            :disabled="selectedSeats.length === 0"
            @click="isModalOpen = true"
            data-testid="btn-checkout"
          >
            Checkout
            <svg class="w-4 h-4 ml-1.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>
          </BaseButton>
        </div>
      </div>
    </div>

    <!-- Booking Modal -->
    <BookingModal 
      :visible="isModalOpen" 
      :selected-seats="selectedSeats"
      :ticket-price="ticketPrice"
      @close="isModalOpen = false"
      @success="handleBookingSuccess"
    />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import SeatMap from '../components/SeatMap.vue'
import BookingModal from '../components/BookingModal.vue'
import BaseButton from '@/shared/components/BaseButton.vue'

const ticketPrice = 500000 // Mock price
const selectedSeats = ref([])
const isModalOpen = ref(false)

const totalPrice = computed(() => selectedSeats.value.length * ticketPrice)

const handleBookingSuccess = () => {
  // Clear seats, modal closed by itself, routing handled by modal
  selectedSeats.value = []
}
</script>
