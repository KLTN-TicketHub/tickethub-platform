<template>
  <div class="flex flex-col gap-6 w-full animate-in fade-in duration-300">
    <!-- Stage / Screen -->
    <div class="w-full h-12 bg-gradient-to-b from-primary/10 to-transparent border-t-2 border-primary/50 rounded-t-[50%] flex items-center justify-center opacity-70 mb-4">
      <span class="text-[11px] font-bold tracking-[0.3em] uppercase text-muted">Sân khấu</span>
    </div>

    <!-- Seat Layout -->
    <div class="flex flex-col gap-3 overflow-x-auto pb-4 hide-scroll">
      <div class="flex flex-col gap-3 min-w-max mx-auto">
        <div v-for="row in seatMap.rows" :key="row.id" class="flex items-center gap-4">
          <!-- Row Label Left -->
          <div class="w-6 text-center font-bold text-[13px] text-muted">{{ row.id }}</div>
          
          <!-- Seats -->
          <div class="flex items-center gap-2">
            <button 
              v-for="seat in row.seats" 
              :key="seat.id"
              class="w-9 h-9 rounded-t-xl rounded-b-md flex items-center justify-center text-[11px] font-bold transition-all"
              :class="getSeatClasses(seat, row.tier)"
              :disabled="seat.status !== 'available' && !isSelected(seat)"
              @click="toggleSeat(seat, row.tier)"
            >
              {{ seat.id }}
            </button>
          </div>
          
          <!-- Row Label Right -->
          <div class="w-6 text-center font-bold text-[13px] text-muted">{{ row.id }}</div>
        </div>
      </div>
    </div>

    <!-- Legend -->
    <div class="flex flex-wrap items-center justify-center gap-x-6 gap-y-3 pt-4 border-t border-border-main mt-2">
      <div class="flex items-center gap-2">
        <div class="w-4 h-4 rounded-t-sm rounded-b-sm bg-primary/20 border border-primary text-primary flex items-center justify-center">✓</div>
        <span class="text-[12px] text-muted">Đang chọn</span>
      </div>
      <div class="flex items-center gap-2">
        <div class="w-4 h-4 rounded-t-sm rounded-b-sm bg-card border border-border-main"></div>
        <span class="text-[12px] text-muted">Trống</span>
      </div>
      <div class="flex items-center gap-2">
        <div class="w-4 h-4 rounded-t-sm rounded-b-sm bg-danger/20 border border-danger/50 text-danger/50 flex items-center justify-center">×</div>
        <span class="text-[12px] text-muted">Đã bán</span>
      </div>
      <div class="flex items-center gap-2">
        <div class="w-4 h-4 rounded-t-sm rounded-b-sm bg-warning/20 border border-warning/50"></div>
        <span class="text-[12px] text-muted">Đang giữ</span>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  seatMap: { type: Object, required: true },
  modelValue: { type: Array, default: () => [] } // array of selected seat objects
})

const emit = defineEmits(['update:modelValue'])

const isSelected = (seat) => {
  return props.modelValue.some(s => s.id === seat.id)
}

const getSeatClasses = (seat, tierName) => {
  if (isSelected(seat)) {
    return 'bg-primary/20 border border-primary text-primary shadow-[0_0_10px_rgba(0,200,83,0.3)] scale-110'
  }
  
  if (seat.status === 'sold') {
    return 'bg-danger/10 border border-danger/30 text-danger/30 opacity-50 cursor-not-allowed'
  }
  if (seat.status === 'reserved') {
    return 'bg-warning/10 border border-warning/30 text-warning/30 opacity-50 cursor-not-allowed'
  }
  
  // Available - color code by tier if desired, or just generic available
  if (tierName === 'VVIP') return 'bg-card border border-primary text-main hover:bg-primary/10 hover:text-primary'
  if (tierName === 'VIP') return 'bg-card border border-primary/50 text-main hover:bg-primary/10 hover:text-primary'
  
  return 'bg-card border border-border-main text-main hover:border-primary hover:text-primary'
}

const toggleSeat = (seat, tierName) => {
  if (seat.status !== 'available' && !isSelected(seat)) return
  
  const currentSelected = [...props.modelValue]
  const index = currentSelected.findIndex(s => s.id === seat.id)
  
  if (index >= 0) {
    currentSelected.splice(index, 1)
  } else {
    // Only allow max 6 seats for example, or unlimited
    if (currentSelected.length >= 6) {
      alert('Bạn chỉ có thể chọn tối đa 6 vé.')
      return
    }
    // We attach the tierName to the seat so the modal can calculate price
    currentSelected.push({ ...seat, tierName })
  }
  
  emit('update:modelValue', currentSelected)
}
</script>
