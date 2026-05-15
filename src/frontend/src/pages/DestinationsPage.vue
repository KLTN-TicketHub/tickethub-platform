<template>
  <div class="min-h-screen bg-bg">
    <!-- Header Section -->
    <section class="relative pt-32 pb-20 overflow-hidden">
      <div class="absolute inset-0">
        <div class="absolute inset-0 bg-gradient-to-b from-primary/5 to-transparent"></div>
        <div class="absolute -bottom-24 -left-24 w-[500px] h-[500px] bg-primary/10 blur-[100px] rounded-full opacity-50"></div>
      </div>

      <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-10">
        <div class="flex flex-col md:flex-row md:items-end justify-between gap-10">
          <div class="space-y-4">
            <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-black uppercase tracking-widest">
              📍 Explore Places
            </div>
            <h1 class="text-5xl md:text-7xl font-black font-heading text-white tracking-tighter">
              Điểm Đến <span class="text-primary">Thú Vị</span>
            </h1>
            <p class="text-muted text-lg font-medium max-w-xl">
              Hệ thống các nhà hát, sân vận động và trung tâm hội nghị đẳng cấp. Tìm kiếm không gian hoàn hảo cho trải nghiệm tiếp theo của bạn.
            </p>
          </div>
          
          <div class="flex flex-wrap items-center gap-3">
            <button 
              v-for="city in ['Tất cả', 'TP. Hồ Chí Minh', 'Hà Nội', 'Đà Nẵng']" 
              :key="city"
              @click="selectedCity = city"
              class="px-6 py-2.5 rounded-2xl text-[13px] font-bold border transition-all"
              :class="[selectedCity === city ? 'bg-primary text-black border-primary shadow-lg shadow-primary/20' : 'bg-white/5 border-white/10 text-muted hover:border-white/20']"
            >
              {{ city }}
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- Destinations Grid -->
    <section class="max-w-[1400px] mx-auto px-6 md:px-10 pb-32">
      <div v-if="filteredDestinations.length > 0" class="grid grid-cols-1 lg:grid-cols-2 gap-10">
        <div 
          v-for="(place, idx) in filteredDestinations" 
          :key="place.id" 
          class="group relative h-[450px] rounded-[3rem] overflow-hidden border border-white/10 animate-fade-up shadow-2xl"
          :style="`animation-delay: ${idx * 100}ms`"
        >
          <!-- Background Image -->
          <img :src="place.image" :alt="place.name" class="absolute inset-0 w-full h-full object-cover transition-transform duration-1000 group-hover:scale-110" />
          
          <!-- Overlays -->
          <div class="absolute inset-0 bg-gradient-to-t from-black via-black/20 to-transparent opacity-80 group-hover:opacity-90 transition-opacity"></div>
          <div class="absolute inset-0 bg-primary/20 opacity-0 group-hover:opacity-100 transition-opacity duration-700"></div>

          <!-- Content -->
          <div class="absolute inset-0 p-10 flex flex-col justify-end transform transition-transform duration-500 group-hover:-translate-y-2">
            <div class="space-y-4">
              <div class="flex items-center gap-3">
                <span class="px-3 py-1 rounded-lg bg-primary text-black text-[10px] font-black uppercase">{{ place.city }}</span>
                <span class="text-[12px] text-white/60 font-bold">{{ place.events }} sự kiện</span>
              </div>
              
              <h3 class="text-3xl font-bold text-white leading-tight">{{ place.name }}</h3>
              
              <div class="pt-4 border-t border-white/10 flex items-center justify-between opacity-0 group-hover:opacity-100 transition-opacity duration-500">
                <span class="text-[13px] text-white/80 font-medium">Sức chứa: ~15,000 khách</span>
                <BaseButton variant="primary" size="sm" class="!rounded-xl !px-6">Xem bản đồ</BaseButton>
              </div>
            </div>
          </div>

          <!-- Interaction Link -->
          <a href="#" class="absolute inset-0 z-10"></a>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="py-32 text-center space-y-6">
        <div class="text-8xl opacity-20">🏟️</div>
        <h3 class="text-2xl font-bold text-white">Chưa có điểm đến tại khu vực này</h3>
        <p class="text-muted max-w-md mx-auto">Chúng tôi đang liên tục mở rộng mạng lưới địa điểm. Vui lòng chọn khu vực khác.</p>
        <BaseButton variant="primary" @click="selectedCity = 'Tất cả'">Tất cả địa điểm</BaseButton>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { fetchDestinations } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'

const selectedCity = ref('Tất cả')
const destinations = ref([])

onMounted(async () => {
  destinations.value = await fetchDestinations()
})

const filteredDestinations = computed(() => {
  if (selectedCity.value === 'Tất cả') return destinations.value
  return destinations.value.filter(d => d.city === selectedCity.value)
})
</script>

<style scoped>
/* Scoped styles */
</style>
