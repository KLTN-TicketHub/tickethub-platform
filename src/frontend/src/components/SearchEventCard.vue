<template>
  <div 
    class="flex flex-col gap-3 cursor-pointer group"
    @click="goToEvent"
  >
    <!-- Image Wrapper (16:9 ratio) -->
    <div class="relative w-full aspect-[16/9] rounded-xl overflow-hidden bg-white/5">
      <img
        :src="event.coverImageUrl || event.image"
        :alt="event.title"
        class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
        loading="lazy"
      />
      <div v-if="rank" class="absolute top-3 left-3 w-9 h-9 rounded-lg bg-primary text-black font-heading font-black text-lg flex items-center justify-center shadow-[0_0_20px_rgba(0,200,83,0.4)]">
        {{ rank }}
      </div>
    </div>

    <!-- Details -->
    <div class="flex flex-col gap-1.5">
      <!-- Title -->
      <h3 class="text-white font-bold text-sm leading-tight uppercase line-clamp-2 group-hover:text-primary transition-colors">
        {{ event.title }}
      </h3>
      
      <!-- Price -->
      <div class="text-primary font-bold text-sm">
        {{ lowestPrice }}
      </div>

      <!-- Date -->
      <div class="flex items-center gap-1.5 text-white/60 text-xs font-medium">
        <PhCalendarBlank weight="regular" class="text-sm" />
        <span>{{ formatDate(event.startAt || event.dateStart) }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { selectEvent } from '../stores/eventStore'
import { PhCalendarBlank } from '@phosphor-icons/vue'

const props = defineProps({
  event: { type: Object, required: true },
  rank: { type: Number, default: null }
})
const router = useRouter()

const lowestPrice = computed(() => {
  if (props.event.minPrice > 0) {
    return 'Từ ' + new Intl.NumberFormat('vi-VN').format(props.event.minPrice) + 'đ'
  }
  if (props.event.priceRange && props.event.priceRange.min > 0) {
    return 'Từ ' + new Intl.NumberFormat('vi-VN').format(props.event.priceRange.min) + 'đ'
  }
  return 'Miễn phí'
})

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  try {
    const d = new Date(dateStr)
    if (isNaN(d.getTime())) return dateStr
    const day = String(d.getDate()).padStart(2, '0')
    const month = String(d.getMonth() + 1).padStart(2, '0')
    const year = d.getFullYear()
    return `${day} tháng ${month}, ${year}`
  } catch(e) {
    return dateStr
  }
}

const goToEvent = () => {
  selectEvent(props.event)
  router.push('/event/' + props.event.id)
}
</script>
