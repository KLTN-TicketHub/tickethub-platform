<template>
  <div class="flex flex-col gap-24 pb-20 overflow-hidden">
    
    <!-- PREMIUM HERO SECTION -->
    <section class="relative h-[85vh] min-h-[600px] w-full rounded-[3rem] overflow-hidden group/hero shadow-[0_40px_100px_-20px_rgba(0,0,0,0.9)] border border-white/5 animate-fade-in">
      <div 
        v-for="(s, i) in heroSlides" 
        :key="s.id" 
        class="absolute inset-0 transition-all duration-1000 ease-[cubic-bezier(0.23,1,0.32,1)]"
        :class="[i === slide ? 'opacity-100 z-10 scale-100' : 'opacity-0 z-0 scale-110 pointer-events-none']"
      >
        <!-- Dynamic Background Layer -->
        <div class="absolute inset-0 overflow-hidden">
          <img 
            :src="s.event.image" 
            :alt="s.event.title" 
            class="w-full h-full object-cover transition-transform duration-[20s] ease-linear" 
            :class="[i === slide ? 'scale-115 rotate-1' : 'scale-100']" 
          />
          <!-- Multi-stage Gradient Overlays -->
          <div class="absolute inset-0 bg-gradient-to-r from-bg via-bg/80 to-transparent"></div>
          <div class="absolute inset-0 bg-gradient-to-t from-bg via-transparent to-transparent opacity-90"></div>
          <div class="absolute inset-0 bg-[radial-gradient(circle_at_30%_50%,rgba(0,200,83,0.15),transparent_50%)]"></div>
        </div>

        <!-- Content Container -->
        <div class="relative z-20 h-full max-w-[1400px] mx-auto px-6 md:px-10 flex flex-col justify-center max-w-4xl pt-20">
          <div class="space-y-8 transform transition-all duration-1000 delay-200" :class="[i === slide ? 'translate-y-0 opacity-100' : 'translate-y-20 opacity-0']">
            
            <!-- Floating Feature Badge -->
            <div class="inline-flex items-center gap-3 px-5 py-2 rounded-full bg-white/5 backdrop-blur-2xl border border-white/10 text-primary shadow-2xl">
              <span class="flex h-2 w-2 rounded-full bg-primary animate-ping"></span>
              <span class="text-[11px] font-bold tracking-[0.3em] uppercase">Sự kiện đặc sắc</span>
            </div>
            
            <h1 class="text-5xl md:text-8xl font-bold font-heading text-main leading-[1] tracking-tight drop-shadow-2xl">
              {{ s.event.title }}
            </h1>
            
            <p class="text-lg md:text-xl text-muted max-w-xl leading-relaxed font-medium">
              Khám phá trải nghiệm giải trí đẳng cấp và sở hữu tấm vé vàng ngay hôm nay tại TicketHub.
            </p>

            <div class="flex flex-wrap items-center gap-8 pt-4">
              <div class="flex flex-col">
                <span class="text-[11px] text-muted font-bold uppercase tracking-widest mb-1">Giá vé chỉ từ</span>
                <span class="text-3xl md:text-4xl font-heading font-bold text-primary">{{ formatPrice(s.event.priceRange) }}</span>
              </div>
              <div class="h-12 w-px bg-white/10 hidden md:block"></div>
              <div class="flex items-center gap-4">
                <BaseButton variant="primary" size="lg" class="!px-10 !rounded-2xl shadow-2xl shadow-primary/30" @click="goToEvent(s.event)">
                  Đặt vé ngay 🎟️
                </BaseButton>
                <BaseButton variant="glass" size="lg" class="!rounded-2xl" @click="goToEvent(s.event)">
                  Chi tiết
                </BaseButton>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Slide Navigation Icons -->
      <div class="absolute bottom-12 right-12 z-30 flex items-center gap-6">
        <button 
          class="w-14 h-14 rounded-2xl bg-white/5 backdrop-blur-xl border border-white/10 text-white hover:bg-primary hover:text-black hover:border-primary transition-all duration-500 group"
          @click="goSlide((slide - 1 + heroSlides.length) % heroSlides.length)"
        >
          <span class="text-2xl group-active:scale-90 transition-transform flex items-center justify-center">←</span>
        </button>
        <div class="flex gap-2">
          <div 
            v-for="(_, i) in heroSlides" 
            :key="i" 
            class="h-1.5 rounded-full transition-all duration-700" 
            :class="[i === slide ? 'w-12 bg-primary' : 'w-2 bg-white/20']"
          />
        </div>
        <button 
          class="w-14 h-14 rounded-2xl bg-white/5 backdrop-blur-xl border border-white/10 text-white hover:bg-primary hover:text-black hover:border-primary transition-all duration-500 group"
          @click="goSlide((slide + 1) % heroSlides.length)"
        >
          <span class="text-2xl group-active:scale-90 transition-transform flex items-center justify-center">→</span>
        </button>
      </div>
    </section>


    <!-- TRENDING SECTION -->
    <section class="max-w-[1400px] mx-auto px-6 md:px-10 animate-fade-up [animation-delay:400ms]">
      <div class="flex items-end justify-between mb-12">
        <div>
          <h2 class="text-4xl font-bold font-heading text-main mb-2 flex items-center gap-4">
            <span class="w-2 h-10 bg-primary rounded-full"></span>
            Đang Thịnh Hành
          </h2>
          <p class="text-muted font-medium">Những sự kiện nóng hổi nhất không thể bỏ qua</p>
        </div>
        <router-link to="/concerts" class="text-sm font-bold text-primary hover:text-white flex items-center gap-2 group transition-all">
          Xem tất cả <span class="group-hover:translate-x-1 transition-transform">→</span>
        </router-link>
      </div>
      
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
        <EventCard 
          v-for="(e, idx) in trending.slice(0, 4)" 
          :key="e.id" 
          :event="e" 
          :rank="idx + 1"
          :style="`animation-delay: ${idx * 100}ms`"
          class="animate-fade-up"
        />
      </div>
    </section>

    <section class="py-16 animate-fade-up [animation-delay:500ms]">
      <!-- Header inside container -->
      <div class="max-w-[1400px] mx-auto px-6 md:px-10 mb-10 flex items-center justify-between">
        <h2 class="text-3xl font-bold font-heading text-white flex items-center gap-4">
          <span class="text-2xl">⭐</span> Nghệ sĩ nổi bật
        </h2>
        <div class="flex items-center gap-6">
          <router-link to="/stars" class="text-sm font-bold text-muted hover:text-primary transition-colors">Xem tất cả</router-link>
          <div class="flex gap-2">
            <button @click="scrollContainer(starsScroll, -300)" class="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center hover:bg-primary hover:text-black transition-all">←</button>
            <button @click="scrollContainer(starsScroll, 300)" class="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center hover:bg-primary hover:text-black transition-all">→</button>
          </div>
        </div>
      </div>
      
      <!-- Full-width scroll area with side padding -->
      <div ref="starsScroll" class="flex gap-8 overflow-x-auto pb-6 hide-scroll scroll-smooth px-[max(1.5rem,calc((100vw-1400px)/2+1.5rem))] md:px-[max(2.5rem,calc((100vw-1400px)/2+2.5rem))]">
        <div 
          v-for="star in stars" 
          :key="star.id" 
          class="flex flex-col items-center gap-4 flex-shrink-0 group cursor-pointer"
        >
          <div class="relative">
            <div class="w-32 h-32 rounded-full overflow-hidden border-2 border-white/10 group-hover:border-primary transition-all duration-500 shadow-xl group-hover:shadow-primary/20">
              <img :src="star.image" :alt="star.name" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
            </div>
            <div class="absolute -bottom-2 left-1/2 -translate-x-1/2 bg-primary text-black text-[10px] font-black px-2 py-0.5 rounded-full shadow-lg">
              {{ star.followers }}
            </div>
          </div>
          <div class="text-center">
            <div class="font-bold text-white group-hover:text-primary transition-colors">{{ star.name }}</div>
            <div class="text-[11px] text-muted font-bold uppercase tracking-widest mt-0.5">Nghệ sĩ</div>
          </div>
        </div>
      </div>
    </section>

    <section class="py-16 animate-fade-up [animation-delay:600ms]">
      <div class="max-w-[1400px] mx-auto px-6 md:px-10 mb-10 flex items-center justify-between">
        <h2 class="text-3xl font-bold font-heading text-white flex items-center gap-4">
          <span class="text-2xl">📍</span> Điểm đến thú vị
        </h2>
        <div class="flex items-center gap-6">
          <router-link to="/destinations" class="text-sm font-bold text-muted hover:text-primary transition-colors">Khám phá thêm</router-link>
          <div class="flex gap-2">
            <button @click="scrollContainer(destinationsScroll, -400)" class="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center hover:bg-primary hover:text-black transition-all">←</button>
            <button @click="scrollContainer(destinationsScroll, 400)" class="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center hover:bg-primary hover:text-black transition-all">→</button>
          </div>
        </div>
      </div>
      
      <div ref="destinationsScroll" class="flex gap-6 overflow-x-auto pb-8 hide-scroll snap-x scroll-smooth px-[max(1.5rem,calc((100vw-1400px)/2+1.5rem))] md:px-[max(2.5rem,calc((100vw-1400px)/2+2.5rem))]">
        <div 
          v-for="place in destinations" 
          :key="place.id" 
          class="relative w-[340px] h-[220px] flex-shrink-0 rounded-[2rem] overflow-hidden group cursor-pointer border border-white/5 shadow-xl transition-all duration-500 hover:-translate-y-1 snap-start"
        >
          <img :src="place.image" :alt="place.name" class="absolute inset-0 w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/90 via-black/20 to-transparent group-hover:from-primary/40 transition-colors duration-500"></div>
          
          <div class="absolute bottom-5 left-6 right-6 space-y-1">
            <div class="text-[10px] font-bold text-primary-light uppercase tracking-widest">{{ place.city }}</div>
            <div class="text-lg font-bold text-white group-hover:translate-x-1 transition-transform">{{ place.name }}</div>
            <div class="text-[11px] text-white/60 font-medium">{{ place.events }} sự kiện đang diễn ra</div>
          </div>
        </div>
      </div>
    </section>

    <!-- PROMO EXPERIENCE BANNER -->
    <section class="max-w-[1400px] mx-auto px-6 md:px-10 animate-fade-up [animation-delay:500ms]">
      <div class="relative group rounded-[3rem] overflow-hidden bg-surface border border-border-main p-12 md:p-24 shadow-2xl">
        <div class="absolute inset-0 opacity-20 grayscale bg-[url('https://www.transparenttextures.com/patterns/carbon-fibre.png')]"></div>
        <div class="absolute -right-24 -bottom-24 w-96 h-96 bg-primary/20 blur-[120px] rounded-full group-hover:bg-primary/30 transition-all duration-1000"></div>
        
        <div class="relative z-10 grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <div class="space-y-8">
            <div class="inline-flex px-4 py-1.5 rounded-full bg-primary/20 border border-primary/30 text-primary text-[11px] font-bold tracking-widest uppercase">
              ✨ Đặc quyền thành viên
            </div>
            <h2 class="text-5xl md:text-6xl font-bold font-heading text-main leading-tight">Săn Vé Sớm <br/><span class="text-primary">Tiết Kiệm 40%</span></h2>
            <p class="text-lg text-muted font-medium leading-relaxed max-w-md">
              Tham gia cộng đồng TicketHub ngay hôm nay để nhận thông báo sớm nhất và ưu đãi giá vé cực sốc.
            </p>
            <BaseButton variant="primary" size="lg" class="!px-10 !rounded-2xl" @click="router.push('/early-bird')">
              Đăng ký ngay
            </BaseButton>
          </div>
          <div class="relative hidden lg:block">
            <div class="aspect-square bg-gradient-to-tr from-primary to-primary-dark rounded-[4rem] rotate-12 scale-90 opacity-10 animate-pulse"></div>
            <div class="absolute inset-0 flex items-center justify-center text-[180px] drop-shadow-2xl animate-float">
              🎟️
            </div>
          </div>
        </div>
      </div>
    </section>

    <section v-for="(cat, idx) in categories" :key="cat.key" class="py-12 animate-fade-up" :style="`animation-delay: ${600 + idx * 100}ms`">
      <div class="max-w-[1400px] mx-auto px-6 md:px-10 mb-12 flex items-center justify-between">
        <h2 class="text-4xl font-bold font-heading text-main flex items-center gap-4">
          <span class="text-4xl opacity-80 grayscale group-hover:grayscale-0 transition-all">{{ cat.icon }}</span> 
          {{ cat.label }}
        </h2>
        <div class="flex items-center gap-6">
          <router-link :to="'/' + cat.key" class="text-sm font-bold text-primary hover:text-white flex items-center gap-2 group transition-all">
            Khám phá <span class="group-hover:translate-x-1 transition-transform">→</span>
          </router-link>
          <div class="flex gap-2">
            <button @click="scrollContainer($refs['catScroll_' + idx][0], -350)" class="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center hover:bg-primary hover:text-black transition-all">←</button>
            <button @click="scrollContainer($refs['catScroll_' + idx][0], 350)" class="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center hover:bg-primary hover:text-black transition-all">→</button>
          </div>
        </div>
      </div>

      <div :ref="'catScroll_' + idx" class="flex gap-8 overflow-x-auto pb-12 hide-scroll snap-x scroll-smooth px-[max(1.5rem,calc((100vw-1400px)/2+1.5rem))] md:px-[max(2.5rem,calc((100vw-1400px)/2+2.5rem))]">
        <EventCard 
          v-for="e in cat.items" 
          :key="e.id" 
          :event="e" 
          class="w-[320px] flex-shrink-0 snap-start"
        />
      </div>
    </section>

    <!-- TRUSTED BY SECTION -->
    <section class="max-w-[1400px] mx-auto px-6 md:px-10 py-12 border-t border-border-main animate-fade-up [animation-delay:900ms]">
      <div class="flex flex-wrap justify-center md:justify-between items-center gap-12 opacity-40 grayscale hover:grayscale-0 transition-all duration-700">
        <div class="text-2xl font-bold font-heading text-main">TICKETBOX</div>
        <div class="text-2xl font-bold font-heading text-main">SPOTIFY</div>
        <div class="text-2xl font-bold font-heading text-main">VIETNAM AIRLINES</div>
        <div class="text-2xl font-bold font-heading text-main">VINPEARL</div>
        <div class="text-2xl font-bold font-heading text-main">SHOPEE</div>
      </div>
    </section>

  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { getEvents, selectEvent, fetchStars, fetchDestinations } from '../features/events/store'
import EventCard from '../features/events/components/EventCard.vue'
import BaseButton from '../shared/components/BaseButton.vue'

const router = useRouter()
const allEvents = getEvents()

const stars = ref([])
const destinations = ref([])
const starsScroll = ref(null)
const destinationsScroll = ref(null)

const slide = ref(0)
let timer = null

const heroSlides = computed(() => {
  const featured = allEvents.filter(e => e.featured).slice(0, 3)
  return featured.map((ev) => ({ id: ev.id, event: ev }))
})

const goSlide = (n) => {
  if (heroSlides.value.length === 0) return;
  slide.value = n
  resetTimer()
}

const resetTimer = () => {
  clearInterval(timer)
  timer = setInterval(() => { 
    slide.value = (slide.value + 1) % heroSlides.value.length 
  }, 8000)
}

const goToEvent = (event) => {
  selectEvent(event)
  router.push('/event/' + event.id)
}

const scrollContainer = (el, amount) => {
  if (el) el.scrollBy({ left: amount, behavior: 'smooth' })
}

onMounted(async () => { 
  if (heroSlides.value.length > 0) resetTimer()
  
  // Fetch new sections
  stars.value = await fetchStars()
  destinations.value = await fetchDestinations()
})
onUnmounted(() => clearInterval(timer))

const trending = computed(() => allEvents.filter(e => e.featured))

const categories = computed(() => {
  const cats = [
    { key: 'concerts', icon: '🎵', label: 'Nhạc & Concert' },
    { key: 'sports', icon: '⚽', label: 'Thể thao' },
    { key: 'arts', icon: '🎭', label: 'Nghệ thuật' },
    { key: 'experiences', icon: '🧭', label: 'Trải nghiệm' },
    { key: 'workshops', icon: '📚', label: 'Workshop' },
  ]
  return cats.map(c => ({
    ...c,
    items: allEvents.filter(e => e.category === c.key).slice(0, 8)
  })).filter(c => c.items.length > 0)
})

const formatPrice = (range) => {
  if (!range) return '0đ'
  if (range.min === 0) return 'Miễn phí'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(range.min)
}
</script>

<style scoped>
@keyframes float {
  0%, 100% { transform: translateY(0) rotate(12deg); }
  50% { transform: translateY(-20px) rotate(15deg); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
</style>
