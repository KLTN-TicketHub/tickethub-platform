<template>
  <div class="min-h-screen bg-[#0A0F0D]">
    <!-- Header Section -->
    <section class="relative pt-32 pb-20 overflow-hidden">
      <div class="absolute inset-0">
        <div class="absolute inset-0 bg-gradient-to-b from-primary/10 to-transparent"></div>
        <div class="absolute top-0 right-0 w-[600px] h-[600px] bg-primary/20 blur-[150px] rounded-full -translate-y-1/2 translate-x-1/3 mix-blend-screen pointer-events-none"></div>
      </div>

      <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-10">
        <div class="flex flex-col md:flex-row md:items-end justify-between gap-8">
          <div class="space-y-4">
            <h1 class="text-5xl md:text-7xl font-black font-heading text-white tracking-tighter uppercase">
              Nghệ Sĩ <span class="text-primary">Nổi Bật</span>
            </h1>
            <p class="text-white/50 text-lg font-medium max-w-xl leading-relaxed">
              Khám phá và theo dõi những gương mặt đang làm mưa làm gió trong làng giải trí. Đừng bỏ lỡ bất kỳ đêm diễn nào của thần tượng.
            </p>
          </div>
          
          <div class="flex items-center gap-4">
            <div class="relative group">
              <PhMagnifyingGlass class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 transition-colors group-focus-within:text-primary text-xl" weight="bold" />
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Tìm tên nghệ sĩ..." 
                class="bg-[#111916] border border-white/5 rounded-full py-4 pl-14 pr-6 text-[15px] text-white outline-none focus:border-primary/50 focus:bg-white/10 transition-all min-w-[320px] shadow-inner placeholder:text-white/30"
              />
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Stars Grid -->
    <section class="max-w-[1400px] mx-auto px-6 md:px-10 pb-32">
      <div v-if="filteredStars.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 auto-rows-[380px]">
        <div 
          v-for="(star, idx) in filteredStars" 
          :key="star.id" 
          :class="{
            'md:col-span-2 lg:col-span-2 xl:col-span-2': idx % 6 === 0,
            'lg:col-span-2 lg:row-span-2': idx % 6 === 3
          }"
          class="relative rounded-[2.5rem] overflow-hidden bg-[#111916] border border-white/5 hover:border-primary/30 transition-all duration-500 group animate-fade-up"
          :style="`animation-delay: ${(idx % 6) * 50}ms`"
        >
          <!-- Background Cover (Optional effect for bigger cells) -->
          <div class="absolute inset-0 opacity-40 group-hover:opacity-60 transition-opacity duration-700">
            <img :src="star.image" :alt="star.name" class="w-full h-full object-cover blur-3xl scale-150" />
            <div class="absolute inset-0 bg-gradient-to-t from-[#0A0F0D] via-[#0A0F0D]/80 to-transparent"></div>
          </div>

          <!-- Content -->
          <div class="absolute inset-0 p-8 flex flex-col justify-between z-10">
            <div class="flex justify-between items-start">
              <div class="bg-primary/10 border border-primary/20 text-primary text-[11px] font-black px-4 py-1.5 rounded-full uppercase tracking-widest shadow-lg shadow-primary/10">
                {{ star.followers }} FOLLOWERS
              </div>
              <BaseButton variant="ghost" size="sm" class="!rounded-full !bg-white/5 hover:!bg-primary hover:!text-black hover:!scale-110 !p-3 transition-all backdrop-blur-md">
                <PhPlus weight="bold" class="text-xl" />
              </BaseButton>
            </div>

            <div class="flex flex-col md:flex-row md:items-end gap-6 mt-8">
              <div class="w-24 h-24 md:w-32 md:h-32 rounded-[1.5rem] overflow-hidden border border-white/10 group-hover:border-primary/50 transition-all duration-500 shadow-2xl flex-shrink-0">
                <img :src="star.image" :alt="star.name" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700" />
              </div>
              <div class="flex flex-col gap-1 pb-2">
                <h3 class="text-3xl font-heading font-black text-white group-hover:text-primary transition-colors tracking-tight">{{ star.name }}</h3>
                <p class="text-[13px] text-white/50 font-bold uppercase tracking-[0.2em] flex items-center gap-2">
                  <PhCalendarBlank weight="bold" /> Sắp có 2 sự kiện
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="py-32 text-center space-y-6 bg-[#111916]/50 border border-white/5 rounded-[3rem]">
        <div class="w-24 h-24 mx-auto bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 shadow-inner">
          <PhStar weight="duotone" />
        </div>
        <div class="flex flex-col gap-2">
          <h3 class="text-3xl font-black font-heading text-white">Không tìm thấy nghệ sĩ</h3>
          <p class="text-white/50 max-w-md mx-auto font-medium">Hãy thử tìm kiếm với tên khác hoặc khám phá các danh mục sự kiện hiện có.</p>
        </div>
        <BaseButton variant="primary" size="lg" @click="searchQuery = ''" class="mt-4">Hiển thị tất cả</BaseButton>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { fetchStars } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { PhMagnifyingGlass, PhPlus, PhCalendarBlank, PhStar } from '@phosphor-icons/vue'

const searchQuery = ref('')
const stars = ref([])

onMounted(async () => {
  stars.value = await fetchStars()
})

const filteredStars = computed(() => {
  if (!searchQuery.value) return stars.value
  const q = searchQuery.value.toLowerCase().trim()
  return stars.value.filter(s => s.name.toLowerCase().includes(q))
})
</script>
