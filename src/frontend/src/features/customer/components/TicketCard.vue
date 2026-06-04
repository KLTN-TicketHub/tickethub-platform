<template>
  <div class="glass-panel group flex flex-col sm:flex-row overflow-hidden transition-all duration-300 hover:-translate-y-1 hover:shadow-glow-strong">
    
    <!-- Main Event Details (Left) -->
    <div class="flex-1 p-5 sm:p-6 flex flex-col justify-between relative z-10">
      
      <!-- Header -->
      <div class="flex items-start justify-between gap-4 mb-6">
        <div>
          <h3 class="text-xl font-heading font-bold text-main mb-1 group-hover:text-primary transition-colors">
            {{ ticket.eventName }}
          </h3>
          <div class="flex items-center gap-2 text-sm text-muted">
            <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
            {{ ticket.date }}
          </div>
          <div class="flex items-center gap-2 text-sm text-muted mt-1">
            <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
            {{ ticket.location }}
          </div>
        </div>
        
        <!-- Status Badge -->
        <span 
          class="px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider rounded-full border shrink-0"
          :class="statusBadgeClasses"
        >
          {{ ticket.status }}
        </span>
      </div>

      <!-- Ticket Meta Info -->
      <div class="flex items-center gap-6 p-4 rounded-xl bg-surface/50 border border-border-light/30">
        <div class="flex flex-col">
          <span class="text-[10px] text-dimmed uppercase tracking-wider mb-0.5">Tier</span>
          <span class="font-bold text-main">{{ ticket.tier }}</span>
        </div>
        <div class="h-8 w-px bg-border-light/20"></div>
        <div class="flex flex-col">
          <span class="text-[10px] text-dimmed uppercase tracking-wider mb-0.5">Seat</span>
          <span class="font-bold text-main">{{ ticket.seat }}</span>
        </div>
      </div>
    </div>

    <!-- Divider Line with Notches (Visible on desktop) -->
    <div class="hidden sm:block w-px relative ticket-notch border-l border-dashed border-border-main/60 shrink-0"></div>
    
    <!-- Mobile Divider (Horizontal) -->
    <div class="sm:hidden h-px w-full relative border-t border-dashed border-border-main/60">
      <div class="absolute -left-3 top-1/2 -translate-y-1/2 w-6 h-6 rounded-full bg-bg shadow-[inset_-1px_0_0_rgba(255,255,255,0.06)] z-10"></div>
      <div class="absolute -right-3 top-1/2 -translate-y-1/2 w-6 h-6 rounded-full bg-bg shadow-[inset_1px_0_0_rgba(255,255,255,0.06)] z-10"></div>
    </div>

    <!-- QR Stub (Right) -->
    <div class="p-6 sm:w-48 shrink-0 flex flex-col items-center justify-center bg-surface/30 relative z-10">
      <div class="w-24 h-24 sm:w-32 sm:h-32 rounded-xl border border-border-light/20 bg-bg-elevated p-2 flex items-center justify-center overflow-hidden group-hover:border-primary/40 transition-colors duration-300">
        <div class="w-full h-full rounded-lg qr-pattern"></div>
      </div>
      <span class="text-[10px] text-dimmed font-mono mt-3 uppercase tracking-widest text-center break-all w-full">
        {{ ticket.id || 'TKT-1337-XYZ' }}
      </span>
    </div>

  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  ticket: {
    type: Object,
    required: true,
    // Expected structure:
    // { eventName, date, location, tier, seat, status, id }
  }
})

const statusBadgeClasses = computed(() => {
  switch (props.ticket.status.toLowerCase()) {
    case 'upcoming':
      return 'bg-primary/10 text-primary border-primary/20'
    case 'past':
      return 'bg-surface text-dimmed border-border-light'
    case 'used':
      return 'bg-info/10 text-info border-info/20'
    case 'cancelled':
      return 'bg-danger/10 text-danger border-danger/20'
    default:
      return 'bg-surface text-muted border-border-light'
  }
})
</script>

<style scoped>
@reference "@/app.css";
</style>
