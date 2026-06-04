<template>
  <div class="animate-fade-up max-w-4xl mx-auto py-8">
    
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-end justify-between gap-6 mb-8">
      <div>
        <h1 class="text-3xl font-heading font-bold text-main mb-2">My Tickets</h1>
        <p class="text-sm text-muted">Manage your digital tickets for upcoming and past events.</p>
      </div>

      <!-- Tab Navigation -->
      <div class="flex items-center gap-2 p-1.5 rounded-xl bg-surface/50 border border-border-light/20 shrink-0">
        <BaseButton 
          :variant="activeTab === 'Upcoming' ? 'primary' : 'ghost'"
          size="sm"
          class="!px-6 transition-all duration-200"
          @click="activeTab = 'Upcoming'"
          data-testid="tab-upcoming"
        >
          Upcoming
        </BaseButton>
        <BaseButton 
          :variant="activeTab === 'Past' ? 'primary' : 'ghost'"
          size="sm"
          class="!px-6 transition-all duration-200"
          @click="activeTab = 'Past'"
          data-testid="tab-past"
        >
          Past
        </BaseButton>
      </div>
    </div>

    <!-- Ticket List -->
    <div class="relative min-h-[400px]">
      <TransitionGroup 
        name="list" 
        tag="div" 
        class="flex flex-col gap-5 relative"
        enter-active-class="transition-all duration-400 ease-[cubic-bezier(0.16,1,0.3,1)]"
        enter-from-class="opacity-0 translate-y-8 scale-[0.98]"
        enter-to-class="opacity-100 translate-y-0 scale-100"
        leave-active-class="transition-all duration-300 ease-in absolute w-full"
        leave-from-class="opacity-100 translate-y-0 scale-100"
        leave-to-class="opacity-0 -translate-y-4 scale-[0.98]"
        move-class="transition-transform duration-500 ease-[cubic-bezier(0.34,1.56,0.64,1)]"
      >
        <TicketCard 
          v-for="(ticket, index) in filteredTickets" 
          :key="ticket.id" 
          :ticket="ticket"
          :style="{ transitionDelay: `${index * 50}ms` }"
        />
      </TransitionGroup>

      <!-- Empty State -->
      <Transition 
        enter-active-class="transition-all duration-300 delay-200 ease-out"
        enter-from-class="opacity-0 translate-y-4"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div v-if="filteredTickets.length === 0" class="absolute inset-0 flex flex-col items-center justify-center py-20">
          <div class="w-20 h-20 rounded-full bg-surface/50 border border-border-light/20 flex items-center justify-center mb-6">
            <svg class="w-8 h-8 text-muted" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
              <rect x="2" y="5" width="20" height="14" rx="2"></rect>
              <line x1="2" y1="12" x2="22" y2="12"></line>
              <line x1="7" y1="5" x2="7" y2="19"></line>
              <line x1="17" y1="5" x2="17" y2="19"></line>
            </svg>
          </div>
          <h3 class="text-xl font-heading font-bold text-main mb-2">No {{ activeTab.toLowerCase() }} tickets found</h3>
          <p class="text-sm text-muted mb-6 max-w-md text-center">
            You don't have any {{ activeTab.toLowerCase() }} events. Explore the marketplace to find your next unforgettable experience.
          </p>
          <BaseButton variant="primary" @click="$router.push('/')">
            Browse Events
          </BaseButton>
        </div>
      </Transition>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import TicketCard from '../components/TicketCard.vue'

const activeTab = ref('Upcoming')

// Mock Ticket Data
const mockTickets = ref([
  {
    id: 'TKT-8294-A1',
    eventName: 'Summer Music Festival 2026',
    date: '15 Aug 2026 • 18:00',
    location: 'MyDinh National Stadium',
    tier: 'VIP',
    seat: 'Row A - 04',
    status: 'Upcoming'
  },
  {
    id: 'TKT-8294-A2',
    eventName: 'Summer Music Festival 2026',
    date: '15 Aug 2026 • 18:00',
    location: 'MyDinh National Stadium',
    tier: 'VIP',
    seat: 'Row A - 05',
    status: 'Upcoming'
  },
  {
    id: 'TKT-1049-C8',
    eventName: 'Tech Innovators Summit',
    date: '12 Jan 2025 • 09:00',
    location: 'SECC Ho Chi Minh',
    tier: 'General',
    seat: 'Free Seating',
    status: 'Past'
  }
])

const filteredTickets = computed(() => {
  return mockTickets.value.filter(ticket => ticket.status === activeTab.value)
})
</script>
