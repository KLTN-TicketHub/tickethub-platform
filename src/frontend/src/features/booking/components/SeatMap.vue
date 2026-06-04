<template>
  <div class="flex flex-col gap-6 w-full animate-fade-up">
    <!-- Legend -->
    <div class="flex flex-wrap items-center justify-center gap-4 text-xs font-medium text-muted bg-surface/30 py-3 rounded-xl border border-border-main/50">
      <div class="flex items-center gap-2"><div class="w-4 h-4 rounded-md bg-surface border border-border-light"></div> Available</div>
      <div class="flex items-center gap-2"><div class="w-4 h-4 rounded-md bg-primary/20 border border-primary shadow-glow"></div> Selected</div>
      <div class="flex items-center gap-2"><div class="w-4 h-4 rounded-md bg-warning/20 border border-warning/50"></div> Locked</div>
      <div class="flex items-center gap-2"><div class="w-4 h-4 rounded-md bg-bg-elevated border border-border-main opacity-50"></div> Sold</div>
    </div>

    <!-- Screen / Stage Indicator -->
    <div class="w-full max-w-2xl mx-auto flex flex-col items-center gap-2 mb-4">
      <div class="w-3/4 h-2 rounded-t-full bg-gradient-to-t from-primary/30 to-transparent"></div>
      <span class="text-xs font-semibold tracking-widest text-primary/70 uppercase">Stage</span>
    </div>

    <!-- Seat Grid -->
    <div class="w-full overflow-x-auto no-scrollbar pb-4">
      <div class="min-w-[600px] max-w-4xl mx-auto grid grid-cols-10 gap-2.5 md:gap-3 p-4 glass-panel">
        <div 
          v-for="seat in seats" 
          :key="seat.id"
          class="aspect-square rounded-lg flex items-center justify-center text-[10px] font-bold cursor-pointer transition-all duration-200 select-none relative group"
          :class="getSeatClasses(seat)"
          :data-testid="`seat-${seat.id}`"
          @click="toggleSeat(seat)"
        >
          {{ seat.label }}
          
          <!-- Tooltip on hover -->
          <div class="absolute -top-8 left-1/2 -translate-x-1/2 bg-bg-elevated text-main text-xs py-1 px-2 rounded opacity-0 group-hover:opacity-100 pointer-events-none whitespace-nowrap z-10 shadow-modal border border-border-light/20 transition-opacity">
            {{ seat.label }} ({{ seat.state }})
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['update:modelValue'])

const seats = ref([])

// Mock Data Generation
onMounted(() => {
  const rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H']
  const cols = 10
  const generatedSeats = []

  for (let r = 0; r < rows.length; r++) {
    for (let c = 1; c <= cols; c++) {
      const id = `${rows[r]}${c}`
      
      // Randomly assign states for realism
      let state = 'available'
      const rand = Math.random()
      if (rand > 0.85) {
        state = 'sold'
      } else if (rand > 0.75) {
        state = 'locked'
      }

      generatedSeats.push({
        id,
        label: id,
        state,
      })
    }
  }

  seats.value = generatedSeats
})

const getSeatClasses = (seat) => {
  switch (seat.state) {
    case 'available':
      return 'bg-surface border border-border-light hover:border-primary/50 hover:bg-surface/80 text-muted hover:text-main'
    case 'selected':
      return 'bg-primary/20 border-2 border-primary text-primary shadow-glow scale-105'
    case 'locked':
      return 'bg-warning/10 border border-warning/30 text-warning/50 cursor-not-allowed'
    case 'sold':
      return 'bg-bg-elevated border border-border-main opacity-40 text-dimmed cursor-not-allowed'
    default:
      return 'bg-surface border border-border-light'
  }
}

const toggleSeat = (seat) => {
  if (seat.state === 'sold' || seat.state === 'locked') return

  const newSelectedSeats = [...props.modelValue]
  const index = newSelectedSeats.findIndex(s => s.id === seat.id)

  if (seat.state === 'available') {
    seat.state = 'selected'
    newSelectedSeats.push(seat)
  } else if (seat.state === 'selected') {
    seat.state = 'available'
    if (index !== -1) newSelectedSeats.splice(index, 1)
  }

  emit('update:modelValue', newSelectedSeats)
}

// Ensure external state clears are reflected visually if necessary
watch(() => props.modelValue, (newVal) => {
  if (newVal.length === 0) {
     seats.value.forEach(s => {
       if (s.state === 'selected') s.state = 'available'
     })
  }
}, { deep: true })
</script>
