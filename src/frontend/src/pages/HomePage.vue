<template>
  <div class="flex flex-col gap-24 pb-20 overflow-hidden">

    <!-- HERO SECTION -->
    <section v-if="heroEvents.length > 0" class="w-full max-w-[1440px] mx-auto px-6 md:px-10 pt-10">
      <div
        class="relative w-full h-[220px] md:h-[300px] lg:h-[360px] rounded-2xl overflow-hidden group cursor-pointer border border-white/10 shadow-xl"
        @click="goToEvent(heroEvents[slide])"
      >
        <div
          v-for="(ev, i) in heroEvents"
          :key="ev.id"
          class="absolute inset-0 transition-opacity duration-1000 ease-[cubic-bezier(0.23,1,0.32,1)]"
          :class="[i === slide ? 'opacity-100 z-10' : 'opacity-0 z-0 pointer-events-none']"
        >
          <img :src="ev.coverImageUrl" :alt="ev.title" class="w-full h-full object-cover transition-transform duration-[8s] ease-linear" :class="[i === slide ? 'scale-105' : 'scale-100']" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/5 to-transparent"></div>
        </div>

        <div class="absolute bottom-4 left-4 z-20 px-4 py-2 rounded-xl bg-black/40 backdrop-blur-md border border-white/10">
          <div class="text-[9px] text-white/60 font-bold uppercase tracking-widest">Giá vé từ</div>
          <div class="text-base font-heading font-black text-white">{{ formatPrice(heroEvents[slide]?.minPrice) }}</div>
        </div>

        <div v-if="heroEvents.length > 1" class="absolute bottom-4 right-4 z-20 flex gap-1.5">
          <div
            v-for="(_, i) in heroEvents"
            :key="i"
            class="h-1 rounded-full transition-all duration-700 cursor-pointer"
            :class="[i === slide ? 'w-6 bg-primary' : 'w-2 bg-white/30 hover:bg-white/50']"
            @click.stop="goSlide(i)"
          />
        </div>
      </div>
    </section>

    <!-- SỰ KIỆN XU HƯỚNG -->
    <section v-if="trendingEvents.length > 0" class="w-full max-w-[1440px] mx-auto px-6 md:px-10">
      <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white flex items-center gap-4 mb-10">
        <PhFire weight="fill" class="text-primary" /> Sự kiện xu hướng
      </h2>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
        <SearchEventCard v-for="(ev, idx) in trendingEvents" :key="ev.id" :event="ev" :rank="idx + 1" />
      </div>
    </section>

    <!-- CÁC KHỐI THEO DANH MỤC -->
    <section v-for="cat in categorySections" :key="cat.id" class="w-full max-w-[1440px] mx-auto px-6 md:px-10">
      <div class="flex items-center justify-between mb-10">
        <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white">{{ cat.categoryName }}</h2>
        <router-link :to="{ path: '/search', query: { CategoryId: cat.id } }" class="text-sm font-bold text-white/50 hover:text-white flex items-center gap-2 transition-colors">
          Xem thêm <PhArrowRight weight="bold" />
        </router-link>
      </div>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
        <SearchEventCard v-for="ev in cat.events" :key="ev.id" :event="ev" />
      </div>
    </section>

    <!-- SỰ KIỆN MỚI ĐĂNG -->
    <section v-if="newestEvents.length > 0" class="w-full max-w-[1440px] mx-auto px-6 md:px-10">
      <div class="flex items-center justify-between mb-10">
        <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white">Sự kiện mới đăng</h2>
        <router-link to="/search" class="text-sm font-bold text-white/50 hover:text-white flex items-center gap-2 transition-colors">
          Xem thêm <PhArrowRight weight="bold" />
        </router-link>
      </div>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
        <SearchEventCard v-for="ev in newestEvents" :key="ev.id" :event="ev" />
      </div>
    </section>

    <!-- KHÁM PHÁ THEO THÀNH PHỐ -->
    <section class="w-full max-w-[1440px] mx-auto px-6 md:px-10">
      <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white flex items-center gap-4 mb-10">
        <PhMapPin weight="fill" class="text-primary" /> Khám phá theo thành phố
      </h2>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <router-link
          v-for="city in cityTiles"
          :key="city.name"
          :to="city.query ? { path: '/search', query: { ProvinceCity: city.query } } : '/search'"
          class="relative aspect-square rounded-[1.5rem] overflow-hidden group cursor-pointer border border-white/5"
        >
          <img :src="city.image" :alt="city.name" class="absolute inset-0 w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/90 via-black/30 to-transparent"></div>
          <div class="absolute bottom-7 left-7 right-7 flex flex-col gap-2">
            <PhMapPin weight="fill" class="text-primary text-2xl" />
            <span class="text-2xl lg:text-3xl font-bold font-heading text-white leading-tight">{{ city.name }}</span>
          </div>
        </router-link>
      </div>
    </section>

  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { getPublicEvents, getPublicEventCategories, getTrendingEvents } from '../services/eventService'
import { selectEvent } from '../stores/eventStore'
import SearchEventCard from '../components/SearchEventCard.vue'
import { PhArrowRight, PhFire, PhMapPin } from '@phosphor-icons/vue'

const router = useRouter()

const heroEvents = ref([])
const trendingEvents = ref([])
const categorySections = ref([])
const newestEvents = ref([])

const slide = ref(0)
let heroTimer = null

const cityTiles = [
  { name: 'Tp. Hồ Chí Minh', query: 'Hồ Chí Minh', image: 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=800&q=80' },
  { name: 'Hà Nội', query: 'Hà Nội', image: 'https://images.unsplash.com/photo-1509030450996-dd1a26dda07a?w=800&q=80' },
  { name: 'Đà Lạt', query: 'Đà Lạt', image: 'https://images.unsplash.com/photo-1528127269322-539801943592?w=800&q=80' },
  { name: 'Vị trí khác', query: null, image: 'https://images.unsplash.com/photo-1528181304800-259b08848526?w=800&q=80' },
]

const goSlide = (n) => {
  if (heroEvents.value.length === 0) return
  slide.value = (n + heroEvents.value.length) % heroEvents.value.length
  resetHeroTimer()
}

const resetHeroTimer = () => {
  clearInterval(heroTimer)
  if (heroEvents.value.length <= 1) return
  heroTimer = setInterval(() => { slide.value = (slide.value + 1) % heroEvents.value.length }, 8000)
}

const goToEvent = (ev) => {
  selectEvent(ev)
  router.push('/event/' + ev.id)
}

const formatPrice = (val) => {
  if (!val) return 'Miễn phí'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(val)
}

const loadHero = async () => {
  try {
    const res = await getPublicEvents({ pageSize: 3, pageNumber: 1 })
    if (res?.success && Array.isArray(res.data?.data)) {
      heroEvents.value = res.data.data
      resetHeroTimer()
    }
  } catch (err) {
    console.error('[HomePage] Không thể tải sự kiện nổi bật cho hero:', err)
  }
}

const loadTrendingEvents = async () => {
  try {
    const res = await getTrendingEvents(4)
    if (res?.success && Array.isArray(res.data)) trendingEvents.value = res.data
  } catch (err) {
    console.error('[HomePage] Không thể tải sự kiện xu hướng:', err)
  }
}

const loadCategorySections = async () => {
  try {
    const res = await getPublicEventCategories()
    const categories = res?.data?.data ?? res?.data ?? []
    const topCategories = categories.slice(0, 4)

    const sections = await Promise.all(topCategories.map(async (cat) => {
      try {
        const evRes = await getPublicEvents({ categoryId: cat.id, pageSize: 4 })
        return {
          id: cat.id,
          categoryName: cat.categoryName,
          events: evRes?.success && Array.isArray(evRes.data?.data) ? evRes.data.data : []
        }
      } catch (err) {
        console.error(`[HomePage] Không thể tải sự kiện cho danh mục "${cat.categoryName}":`, err)
        return { id: cat.id, categoryName: cat.categoryName, events: [] }
      }
    }))

    categorySections.value = sections.filter(s => s.events.length > 0)
  } catch (err) {
    console.error('[HomePage] Không thể tải danh mục sự kiện:', err)
  }
}

const loadNewestEvents = async () => {
  try {
    const res = await getPublicEvents({ pageSize: 4, pageNumber: 1 })
    if (res?.success && Array.isArray(res.data?.data)) newestEvents.value = res.data.data
  } catch (err) {
    console.error('[HomePage] Không thể tải sự kiện mới đăng:', err)
  }
}

onMounted(() => {
  loadHero()
  loadTrendingEvents()
  loadCategorySections()
  loadNewestEvents()
})

onUnmounted(() => clearInterval(heroTimer))
</script>
