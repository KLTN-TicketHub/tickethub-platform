<template>
  <div class="min-h-screen bg-[#050807]">
    <!-- Header Section -->
    <section class="relative pt-32 pb-20 overflow-hidden">
      <div class="absolute inset-0">
        <div class="absolute inset-0 bg-gradient-to-b from-primary/10 to-transparent"></div>
        <div class="absolute -bottom-24 -left-24 w-[500px] h-[500px] bg-primary/20 blur-[150px] rounded-full mix-blend-screen pointer-events-none"></div>
      </div>

      <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-10">
        <div class="flex flex-col lg:flex-row lg:items-end justify-between gap-10">
          <div class="space-y-4">
            <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-black uppercase tracking-widest shadow-[0_0_20px_rgba(0,200,83,0.15)]">
              <PhMapPin weight="fill" /> Explore Places
            </div>
            <h1 class="text-5xl md:text-7xl font-black font-heading text-white tracking-tighter uppercase">
              Điểm Đến <span class="text-primary">Thú Vị</span>
            </h1>
            <p class="text-white/50 text-lg font-medium max-w-xl leading-relaxed">
              Hệ thống các nhà hát, sân vận động và trung tâm hội nghị đẳng cấp. Tìm kiếm không gian hoàn hảo cho trải nghiệm tiếp theo của bạn.
            </p>
          </div>
          
          <div class="flex flex-wrap items-center gap-3">
            <button 
              v-for="city in ['Tất cả', 'TP. Hồ Chí Minh', 'Hà Nội', 'Đà Nẵng']" 
              :key="city"
              @click="selectedCity = city"
              class="px-6 py-3 rounded-full text-[14px] font-bold border transition-all cursor-pointer"
              :class="[selectedCity === city ? 'bg-primary text-black border-primary shadow-[0_0_30px_rgba(0,200,83,0.3)] hover:bg-primary-hover hover:scale-105' : 'bg-white/5 border-white/10 text-white hover:border-white/30 hover:bg-white/10']"
            >
              {{ city }}
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- Destinations Grid -->
    <section class="max-w-[1400px] mx-auto px-6 md:px-10 pb-32">
      <div v-if="filteredDestinations.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 auto-rows-[450px]">
        <div 
          v-for="(place, idx) in filteredDestinations" 
          :key="place.id" 
          :class="{
            'md:col-span-2 lg:col-span-2 xl:col-span-2': idx % 5 === 0,
            'lg:col-span-2 lg:row-span-2': idx % 5 === 3
          }"
          class="group relative rounded-[2.5rem] overflow-hidden border border-white/10 bg-[#111916] animate-fade-up shadow-2xl cursor-pointer"
          :style="`animation-delay: ${(idx % 5) * 100}ms`"
        >
          <!-- Background Image -->
          <img :src="place.image" :alt="place.name" class="absolute inset-0 w-full h-full object-cover transition-transform duration-1000 group-hover:scale-110" />
          
          <!-- Overlays -->
          <div class="absolute inset-0 bg-gradient-to-t from-[#0A0F0D] via-[#0A0F0D]/40 to-transparent opacity-90 group-hover:opacity-100 transition-opacity"></div>
          <div class="absolute inset-0 bg-primary/20 opacity-0 group-hover:opacity-100 transition-opacity duration-700 mix-blend-overlay"></div>

          <!-- Content -->
          <div class="absolute inset-0 p-10 flex flex-col justify-end transform transition-transform duration-500 group-hover:-translate-y-2 z-10">
            <div class="space-y-4">
              <div class="flex items-center gap-3">
                <span class="px-3 py-1.5 rounded-full bg-white/10 backdrop-blur-md border border-white/20 text-white text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5">
                  <PhMapPin weight="bold" /> {{ place.city }}
                </span>
                <span class="px-3 py-1.5 rounded-full bg-primary/20 backdrop-blur-md border border-primary/30 text-primary text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5">
                  <PhTicket weight="bold" /> {{ place.events }} sự kiện
                </span>
              </div>
              
              <h3 class="text-4xl font-black font-heading text-white leading-tight tracking-tight">{{ place.name }}</h3>
              
              <div class="pt-6 border-t border-white/10 flex flex-col sm:flex-row sm:items-center justify-between gap-4 opacity-0 group-hover:opacity-100 transition-all duration-500 translate-y-4 group-hover:translate-y-0">
                <span class="text-[14px] text-white/70 font-medium flex items-center gap-2">
                  <PhUsers weight="bold" class="text-lg" /> Sức chứa: ~15,000 khách
                </span>
                <BaseButton variant="primary" size="sm" class="!rounded-full !px-6 !py-2.5 shadow-[0_0_20px_rgba(0,200,83,0.3)] hover:scale-105 transition-transform flex items-center gap-2">
                  Xem bản đồ <PhArrowRight weight="bold" />
                </BaseButton>
              </div>
            </div>
          </div>

          <!-- Interaction Link -->
          <a href="#" class="absolute inset-0 z-20"></a>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="py-32 text-center space-y-6 bg-[#111916]/50 border border-white/5 rounded-[3rem]">
        <div class="w-24 h-24 mx-auto bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 shadow-inner">
          <PhBuildings weight="duotone" />
        </div>
        <div class="flex flex-col gap-2">
          <h3 class="text-3xl font-black font-heading text-white">Chưa có điểm đến tại khu vực này</h3>
          <p class="text-white/50 max-w-md mx-auto font-medium">Chúng tôi đang liên tục mở rộng mạng lưới địa điểm. Vui lòng chọn khu vực khác.</p>
        </div>
        <BaseButton variant="primary" size="lg" @click="selectedCity = 'Tất cả'" class="mt-4">Tất cả địa điểm</BaseButton>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { fetchDestinations } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { PhMapPin, PhTicket, PhUsers, PhArrowRight, PhBuildings } from '@phosphor-icons/vue'

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
