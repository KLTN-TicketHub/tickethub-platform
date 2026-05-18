<template>
  <div 
    class="group relative flex flex-col bg-[#111916] border border-white/5 rounded-[20px] overflow-hidden cursor-pointer transition-all duration-500 hover:-translate-y-2 hover:border-primary/50 hover:shadow-[0_20px_40px_-15px_rgba(0,200,83,0.2)]"
    @click="goToEvent"
  >
    <!-- Poster Image Wrapper -->
    <div class="relative w-full aspect-[4/5] overflow-hidden bg-[#0A0F0D]">
      <img 
        class="w-full h-full object-cover transition-transform duration-700 ease-out group-hover:scale-110 group-hover:opacity-80" 
        :src="event.image" 
        :alt="event.title" 
        loading="lazy" 
      />
      
      <!-- Gradient overlay -->
      <div class="absolute inset-0 bg-gradient-to-t from-[#111916] via-transparent to-transparent opacity-90"></div>

      <!-- Hover Reveal: View Details -->
      <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300 z-20 pointer-events-none">
        <span class="px-6 py-2.5 rounded-full bg-white/10 backdrop-blur-md border border-white/20 text-white font-medium shadow-xl translate-y-4 group-hover:translate-y-0 transition-all duration-500">
          Xem chi tiết
        </span>
      </div>

      <!-- Wishlist -->
      <button 
        class="absolute top-4 right-4 w-10 h-10 flex items-center justify-center rounded-full bg-black/40 backdrop-blur-md border border-white/10 text-xl transition-all duration-300 hover:scale-110 hover:bg-black/60 z-30"
        @click.stop="toggleWishlist(event.id)"
      >
        {{ isWishlisted ? '❤️' : '🤍' }}
      </button>

      <!-- Status / Featured badges / Rank -->
      <div class="absolute top-4 left-4 flex flex-col gap-2 z-20">
        <!-- Ticketbox-style Rank Badge -->
        <div v-if="rank" class="w-10 h-10 flex items-center justify-center rounded-lg bg-primary text-black font-black text-xl shadow-[0_0_20px_rgba(0,200,83,0.4)]">
          {{ rank }}
        </div>
        <div v-if="event.featured" class="px-3 py-1.5 rounded-lg bg-danger/90 backdrop-blur-md text-white text-[11px] font-bold tracking-widest uppercase shadow-lg shadow-danger/20">
          🔥 HOT
        </div>
        <div v-else-if="event.status === 'ended'" class="px-3 py-1.5 rounded-lg bg-black/75 backdrop-blur-md text-muted text-[11px] font-bold tracking-widest uppercase border border-white/10">
          Đã kết thúc
        </div>
        <div v-else-if="event.status === 'sold-out'" class="px-3 py-1.5 rounded-lg bg-danger/85 backdrop-blur-md text-white text-[11px] font-bold tracking-widest uppercase border border-danger/50">
          Hết vé
        </div>
        <div v-else-if="event.status === 'upcoming'" class="px-3 py-1.5 rounded-lg bg-primary/20 backdrop-blur-md border border-primary/50 text-primary text-[11px] font-bold tracking-widest uppercase">
          Sắp diễn ra
        </div>
      </div>

      <!-- Category Label (Moved to image bottom) -->
      <div class="absolute bottom-4 left-4 right-4 z-20 flex justify-between items-end">
        <div class="text-[11px] font-bold text-primary uppercase tracking-widest bg-black/40 backdrop-blur-md px-3 py-1.5 rounded-lg border border-white/5">
          {{ categoryLabel }}
        </div>
        <!-- Price overlay -->
        <div class="flex items-baseline gap-1.5 px-3 py-1.5 rounded-lg bg-black/40 backdrop-blur-md border border-white/5">
          <span class="text-[11px] text-white/70 font-medium">Từ</span>
          <span class="font-heading text-sm font-bold text-primary">{{ lowestPrice }}</span>
        </div>
      </div>
    </div>

    <!-- Body -->
    <div class="flex flex-col gap-2 p-5 flex-1 relative z-10 bg-[#111916]">
      <div class="font-heading text-lg font-bold text-white line-clamp-2 leading-snug group-hover:text-primary transition-colors duration-300">
        {{ event.title }}
      </div>
      
      <div v-if="primaryPerformer" class="text-[13px] text-muted font-medium mb-1">
        Với sự tham gia của <span class="text-white/80">{{ primaryPerformer }}</span>
      </div>

      <div class="mt-auto pt-3 border-t border-white/5 flex items-center justify-between text-[13px] text-muted font-medium">
        <div class="flex items-center gap-2">
          <span class="text-lg">📅</span> {{ formatDate(event.dateStart) }}
        </div>
        <div class="flex items-center gap-2 max-w-[50%] truncate">
          <span class="text-lg">📍</span> <span class="truncate">{{ event.location?.name || 'Đang cập nhật' }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { selectEvent, store, toggleWishlist as storeToggleWishlist } from '../store'

const props = defineProps({ 
  event: { type: Object, required: true },
  rank: { type: Number, default: null }
})
const router = useRouter()

const isWishlisted = computed(() => store.wishlist.includes(props.event.id))

const CATEGORY_LABELS = {
  concerts: 'Nhạc & Concert',
  arts: 'Sân khấu & Nghệ thuật',
  sports: 'Thể thao',
  experiences: 'Trải nghiệm',
  workshops: 'Workshop',
}

const categoryLabel = computed(() => CATEGORY_LABELS[props.event.category] || props.event.category || '')

const primaryPerformer = computed(() => {
  if (props.event.performers && props.event.performers.length > 0) {
    return props.event.performers[0].name
  }
  return null
})

const lowestPrice = computed(() => {
  if (props.event.priceRange && props.event.priceRange.min > 0) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(props.event.priceRange.min)
  }
  return 'Miễn phí'
})

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  try {
    const d = new Date(dateStr)
    if (isNaN(d.getTime())) return dateStr
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: 'short', year: 'numeric' })
  } catch(e) {
    return dateStr
  }
}

const toggleWishlist = (id) => {
  storeToggleWishlist(id)
}

const goToEvent = () => {
  selectEvent(props.event)
  router.push('/event/' + props.event.id)
}
</script>

<style scoped>
.event-info-item {
  display: flex;
  align-items: center;
  gap: 4px;
}
</style>
