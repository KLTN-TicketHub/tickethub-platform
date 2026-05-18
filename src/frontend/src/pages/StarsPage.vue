<template>
  <div class="min-h-screen bg-bg">
    <!-- Header Section -->
    <section class="relative pt-32 pb-20 overflow-hidden">
      <div class="absolute inset-0">
        <div class="absolute inset-0 bg-gradient-to-b from-primary/5 to-transparent"></div>
        <div class="absolute top-0 right-0 w-[600px] h-[600px] bg-primary/10 blur-[120px] rounded-full -translate-y-1/2 translate-x-1/3"></div>
      </div>

      <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-10">
        <div class="flex flex-col md:flex-row md:items-end justify-between gap-8">
          <div class="space-y-4">
            <h1 class="text-5xl md:text-7xl font-black font-heading text-white tracking-tighter">
              Nghệ Sĩ <span class="text-primary">Nổi Bật</span>
            </h1>
            <p class="text-muted text-lg font-medium max-w-xl">
              Khám phá và theo dõi những gương mặt đang làm mưa làm gió trong làng giải trí. Đừng bỏ lỡ bất kỳ đêm diễn nào của thần tượng.
            </p>
          </div>
          
          <div class="flex items-center gap-4">
            <div class="relative group">
              <span class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 transition-colors group-focus-within:text-primary">🔍</span>
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Tìm tên nghệ sĩ..." 
                class="bg-white/5 border border-white/10 rounded-2xl py-3.5 pl-12 pr-6 text-sm text-white outline-none focus:border-primary/50 focus:bg-white/10 transition-all min-w-[300px]"
              />
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Stars Grid -->
    <section class="max-w-[1400px] mx-auto px-6 md:px-10 pb-32">
      <div v-if="filteredStars.length > 0" class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-8">
        <div 
          v-for="(star, idx) in filteredStars" 
          :key="star.id" 
          class="flex flex-col items-center gap-6 p-8 rounded-[3rem] bg-surface border border-white/5 hover:border-primary/30 hover:bg-white/5 transition-all duration-500 group animate-fade-up"
          :style="`animation-delay: ${idx * 50}ms`"
        >
          <div class="relative">
            <div class="w-32 h-32 md:w-40 md:h-40 rounded-full overflow-hidden border-4 border-white/5 group-hover:border-primary transition-all duration-500 shadow-2xl">
              <img :src="star.image" :alt="star.name" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700" />
            </div>
            <div class="absolute -bottom-2 left-1/2 -translate-x-1/2 bg-primary text-black text-[11px] font-black px-4 py-1 rounded-full shadow-xl">
              {{ star.followers }} FOLLOWERS
            </div>
          </div>

          <div class="text-center space-y-2">
            <h3 class="text-xl font-bold text-white group-hover:text-primary transition-colors">{{ star.name }}</h3>
            <p class="text-[11px] text-muted font-bold uppercase tracking-[0.2em]">Sắp có 2 sự kiện</p>
          </div>

          <BaseButton 
            variant="ghost" 
            size="sm" 
            class="w-full !rounded-2xl !bg-white/5 !border-white/10 group-hover:!bg-primary group-hover:!text-black group-hover:!border-primary transition-all"
          >
            Theo dõi
          </BaseButton>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="py-32 text-center space-y-6">
        <div class="text-8xl opacity-20">🎭</div>
        <h3 class="text-2xl font-bold text-white">Không tìm thấy nghệ sĩ phù hợp</h3>
        <p class="text-muted max-w-md mx-auto">Hãy thử tìm kiếm với tên khác hoặc khám phá các danh mục sự kiện hiện có.</p>
        <BaseButton variant="primary" @click="searchQuery = ''">Hiển thị tất cả</BaseButton>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { fetchStars } from '../features/events/store'
import BaseButton from '../shared/components/BaseButton.vue'

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

<style scoped>
/* Scoped styles if needed */
</style>
