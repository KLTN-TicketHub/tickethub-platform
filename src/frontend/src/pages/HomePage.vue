<template>
  <div class="flex flex-col gap-24 pb-20 overflow-hidden">

    <!-- HERO SECTION -->
    <section v-if="heroEvents.length > 0" class="relative min-h-[75vh] w-full pt-10">
      <div
        v-for="(ev, i) in heroEvents"
        :key="ev.id"
        class="absolute inset-0 transition-all duration-1000 ease-[cubic-bezier(0.23,1,0.32,1)]"
        :class="[i === slide ? 'opacity-100 z-10' : 'opacity-0 z-0 pointer-events-none']"
      >
        <div class="max-w-[1440px] mx-auto px-6 md:px-10 h-full grid grid-cols-1 lg:grid-cols-12 gap-12 items-center">
          <div class="lg:col-span-6 relative z-20 space-y-8 transform transition-all duration-1000 delay-200" :class="[i === slide ? 'translate-y-0 opacity-100' : 'translate-y-20 opacity-0']">
            <div class="inline-flex items-center gap-2.5 px-4 py-1.5 rounded-full border border-primary/30 text-primary uppercase tracking-[0.25em] text-[10px] font-bold">
              <span class="w-1.5 h-1.5 rounded-full bg-primary animate-pulse"></span>
              Sự kiện nổi bật
            </div>

            <h1 class="text-5xl md:text-7xl font-black font-heading text-white leading-[0.95] tracking-tighter drop-shadow-lg line-clamp-3">
              {{ ev.title }}
            </h1>

            <div class="flex flex-wrap items-center gap-8 pt-4">
              <div class="flex flex-col">
                <span class="text-[11px] text-white/40 font-bold uppercase tracking-widest mb-1">Giá vé từ</span>
                <span class="text-3xl font-heading font-black text-white">{{ formatPrice(ev.minPrice) }}</span>
              </div>
              <div class="flex items-center gap-4">
                <BaseButton variant="primary" size="lg" class="!px-10 !rounded-full shadow-[0_0_30px_rgba(0,200,83,0.3)] hover:scale-105 active:scale-95 transition-transform" @click="goToEvent(ev)">
                  Đặt vé ngay
                </BaseButton>
                <BaseButton variant="ghost" size="lg" class="!rounded-full border border-white/20 hover:bg-white/10" @click="goToEvent(ev)">
                  Chi tiết
                </BaseButton>
              </div>
            </div>
          </div>

          <div class="lg:col-span-6 h-[45vh] lg:h-[70vh] relative rounded-[2rem] overflow-hidden group shadow-2xl transform transition-all duration-[1.5s]" :class="[i === slide ? 'translate-x-0 opacity-100' : 'translate-x-20 opacity-0']">
            <img :src="ev.coverImageUrl" :alt="ev.title" class="w-full h-full object-cover transition-transform duration-[20s] ease-linear scale-105" :class="[i === slide ? 'scale-110' : '']" />
            <div class="absolute inset-0 bg-gradient-to-t from-bg via-transparent to-transparent opacity-80 lg:hidden"></div>
            <div class="absolute inset-0 border border-white/10 rounded-[2rem] pointer-events-none mix-blend-overlay"></div>
          </div>
        </div>
      </div>

      <div v-if="heroEvents.length > 1" class="max-w-[1440px] mx-auto px-6 md:px-10 absolute bottom-10 left-0 right-0 z-30 flex items-center justify-between pointer-events-none">
        <div class="pointer-events-auto flex gap-3">
          <div
            v-for="(_, i) in heroEvents"
            :key="i"
            class="h-1 rounded-full transition-all duration-700 cursor-pointer"
            :class="[i === slide ? 'w-16 bg-primary' : 'w-4 bg-white/20']"
            @click="goSlide(i)"
          />
        </div>
        <div class="pointer-events-auto flex items-center gap-4">
          <button class="w-12 h-12 rounded-full border border-white/20 text-white flex items-center justify-center hover:bg-white/10 active:scale-90 transition-all" @click="goSlide(slide - 1)">
            <PhArrowLeft weight="bold" />
          </button>
          <button class="w-12 h-12 rounded-full border border-white/20 text-white flex items-center justify-center hover:bg-white/10 active:scale-90 transition-all" @click="goSlide(slide + 1)">
            <PhArrowRight weight="bold" />
          </button>
        </div>
      </div>
    </section>

    <!-- SỰ KIỆN XU HƯỚNG -->
    <section v-if="trendingEvents.length > 0" class="max-w-[1440px] mx-auto px-6 md:px-10">
      <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white flex items-center gap-4 mb-10">
        <PhFire weight="fill" class="text-primary" /> Sự kiện xu hướng
      </h2>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
        <SearchEventCard v-for="(ev, idx) in trendingEvents" :key="ev.id" :event="ev" :rank="idx + 1" />
      </div>
    </section>

    <!-- CÁC KHỐI THEO DANH MỤC -->
    <section v-for="cat in categorySections" :key="cat.id" class="max-w-[1440px] mx-auto px-6 md:px-10">
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
    <section v-if="newestEvents.length > 0" class="max-w-[1440px] mx-auto px-6 md:px-10">
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
    <section class="max-w-[1440px] mx-auto px-6 md:px-10">
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
import BaseButton from '../components/ui/BaseButton.vue'
import { PhArrowLeft, PhArrowRight, PhFire, PhMapPin } from '@phosphor-icons/vue'

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
