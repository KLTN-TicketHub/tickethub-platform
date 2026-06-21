<template>
  <div class="flex flex-col gap-24 pb-20 overflow-hidden">
    
    <!-- PREMIUM HERO SECTION (Asymmetric) -->
    <section class="relative min-h-[85vh] w-full pt-10">
      <div 
        v-for="(s, i) in heroSlides" 
        :key="s.id" 
        class="absolute inset-0 transition-all duration-1000 ease-[cubic-bezier(0.23,1,0.32,1)]"
        :class="[i === slide ? 'opacity-100 z-10' : 'opacity-0 z-0 pointer-events-none']"
      >
        <div class="max-w-[1440px] mx-auto px-6 md:px-10 h-full grid grid-cols-1 lg:grid-cols-12 gap-12 items-center">
          
          <!-- Text Content (Left) -->
          <div class="lg:col-span-6 relative z-20 space-y-8 transform transition-all duration-1000 delay-200" :class="[i === slide ? 'translate-y-0 opacity-100' : 'translate-y-20 opacity-0']">
            <!-- Floating Feature Badge -->
            <div class="inline-flex items-center gap-2.5 px-4 py-1.5 rounded-full border border-primary/30 text-primary uppercase tracking-[0.25em] text-[10px] font-bold">
              <span class="w-1.5 h-1.5 rounded-full bg-primary animate-pulse"></span>
              Sự kiện nổi bật
            </div>
            
            <h1 class="text-6xl md:text-[5.5rem] font-black font-heading text-white leading-[0.9] tracking-tighter drop-shadow-lg">
              {{ s.event.title }}
            </h1>
            
            <p class="text-lg md:text-xl text-white/60 max-w-lg leading-relaxed font-medium">
              Khám phá những trải nghiệm giải trí đẳng cấp và sở hữu tấm vé giới hạn ngay hôm nay tại TicketHub.
            </p>

            <div class="flex flex-wrap items-center gap-8 pt-6">
              <div class="flex flex-col">
                <span class="text-[11px] text-white/40 font-bold uppercase tracking-widest mb-1">Giá vé từ</span>
                <span class="text-3xl font-heading font-black text-white">{{ formatPrice(s.event.priceRange) }}</span>
              </div>
              <div class="flex items-center gap-4">
                <BaseButton variant="primary" size="lg" class="!px-10 !rounded-full shadow-[0_0_30px_rgba(0,200,83,0.3)] hover:scale-105 active:scale-95 transition-transform" @click="goToEvent(s.event)">
                  Đặt vé ngay
                </BaseButton>
                <BaseButton variant="ghost" size="lg" class="!rounded-full border border-white/20 hover:bg-white/10" @click="goToEvent(s.event)">
                  Chi tiết
                </BaseButton>
              </div>
            </div>
          </div>

          <!-- Visual Asset (Right) -->
          <div class="lg:col-span-6 h-[50vh] lg:h-[80vh] relative rounded-[2rem] overflow-hidden group shadow-2xl transform transition-all duration-[1.5s]" :class="[i === slide ? 'translate-x-0 opacity-100' : 'translate-x-20 opacity-0']">
            <img 
              :src="s.event.image" 
              :alt="s.event.title" 
              class="w-full h-full object-cover transition-transform duration-[20s] ease-linear scale-105" 
              :class="[i === slide ? 'scale-110 rotate-1' : '']" 
            />
            <div class="absolute inset-0 bg-gradient-to-t from-bg via-transparent to-transparent opacity-80 lg:hidden"></div>
            <div class="absolute inset-0 border border-white/10 rounded-[2rem] pointer-events-none mix-blend-overlay"></div>
          </div>
        </div>
      </div>

      <!-- Slide Controls -->
      <div class="max-w-[1440px] mx-auto px-6 md:px-10 absolute bottom-10 left-0 right-0 z-30 flex items-center justify-between pointer-events-none">
        <div class="pointer-events-auto flex gap-3">
          <div 
            v-for="(_, i) in heroSlides" 
            :key="i" 
            class="h-1 rounded-full transition-all duration-700 cursor-pointer" 
            :class="[i === slide ? 'w-16 bg-primary' : 'w-4 bg-white/20']"
            @click="goSlide(i)"
          />
        </div>
        <div class="pointer-events-auto flex items-center gap-4">
          <button class="w-12 h-12 rounded-full border border-white/20 text-white flex items-center justify-center hover:bg-white/10 active:scale-90 transition-all" @click="goSlide((slide - 1 + heroSlides.length) % heroSlides.length)">
            <PhArrowLeft weight="bold" />
          </button>
          <button class="w-12 h-12 rounded-full border border-white/20 text-white flex items-center justify-center hover:bg-white/10 active:scale-90 transition-all" @click="goSlide((slide + 1) % heroSlides.length)">
            <PhArrowRight weight="bold" />
          </button>
        </div>
      </div>
    </section>

    <!-- TRENDING SECTION (Bento Grid) -->
    <section class="max-w-[1440px] mx-auto px-6 md:px-10 pt-10">
      <div class="flex items-end justify-between mb-12">
        <div>
          <h2 class="text-4xl lg:text-5xl font-black font-heading text-white mb-3">Đang Thịnh Hành</h2>
          <p class="text-white/50 text-lg font-medium">Những sự kiện nóng hổi nhất không thể bỏ qua</p>
        </div>
        <router-link to="/search" class="text-sm font-bold text-primary hover:text-white flex items-center gap-2 group transition-all hidden md:flex">
          Xem tất cả <PhArrowRight weight="bold" class="group-hover:translate-x-1 transition-transform" />
        </router-link>
      </div>
      
      <!-- Asymmetric Grid -->
      <div class="grid grid-cols-1 md:grid-cols-6 gap-6 lg:gap-8">
        <div 
          v-for="(e, idx) in trending.slice(0, 5)" 
          :key="e.id" 
          class="animate-fade-up w-full"
          :class="{
            'md:col-span-4': idx === 0,
            'md:col-span-2': idx === 1 || idx === 2,
            'md:col-span-3': idx === 3 || idx === 4,
          }"
          :style="`animation-delay: ${idx * 100}ms`"
        >
          <EventCard :event="e" :rank="idx + 1" class="h-full w-full" />
        </div>
      </div>
    </section>

    <!-- ARTISTS SECTION -->
    <section class="py-16">
      <div class="max-w-[1440px] mx-auto px-6 md:px-10 mb-10 flex items-center justify-between">
        <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white flex items-center gap-4">
          <PhStar weight="fill" class="text-primary" /> Nghệ sĩ nổi bật
        </h2>
        <div class="flex items-center gap-4">
          <button @click="scrollContainer(starsScroll, -300)" class="w-10 h-10 rounded-full border border-white/20 flex items-center justify-center hover:bg-white/10 text-white transition-all active:scale-95"><PhArrowLeft weight="bold" /></button>
          <button @click="scrollContainer(starsScroll, 300)" class="w-10 h-10 rounded-full border border-white/20 flex items-center justify-center hover:bg-white/10 text-white transition-all active:scale-95"><PhArrowRight weight="bold" /></button>
        </div>
      </div>
      
      <div ref="starsScroll" class="flex gap-8 overflow-x-auto pb-6 hide-scroll scroll-smooth px-6 md:px-10 max-w-[1440px] mx-auto">
        <div 
          v-for="star in stars" 
          :key="star.id" 
          class="flex flex-col items-center gap-5 flex-shrink-0 group cursor-pointer"
        >
          <div class="relative">
            <div class="w-32 h-32 lg:w-40 lg:h-40 rounded-full overflow-hidden border border-white/10 group-hover:border-primary/50 transition-colors duration-500 shadow-2xl">
              <img :src="star.image" :alt="star.name" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700 grayscale group-hover:grayscale-0" />
            </div>
            <div class="absolute -bottom-2 left-1/2 -translate-x-1/2 bg-white text-black text-[11px] font-black px-3 py-1 rounded-full shadow-lg border border-black/10">
              {{ star.followers }}
            </div>
          </div>
          <div class="text-center">
            <div class="font-bold text-lg text-white group-hover:text-primary transition-colors">{{ star.name }}</div>
            <div class="text-[10px] text-white/40 font-bold uppercase tracking-[0.2em] mt-1">Nghệ sĩ</div>
          </div>
        </div>
      </div>
    </section>

    <!-- DESTINATIONS SECTION -->
    <section class="py-10">
      <div class="max-w-[1440px] mx-auto px-6 md:px-10 mb-10 flex items-center justify-between">
        <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white flex items-center gap-4">
          <PhMapPin weight="fill" class="text-primary" /> Điểm đến thú vị
        </h2>
        <div class="flex items-center gap-4">
          <button @click="scrollContainer(destinationsScroll, -400)" class="w-10 h-10 rounded-full border border-white/20 flex items-center justify-center hover:bg-white/10 text-white transition-all active:scale-95"><PhArrowLeft weight="bold" /></button>
          <button @click="scrollContainer(destinationsScroll, 400)" class="w-10 h-10 rounded-full border border-white/20 flex items-center justify-center hover:bg-white/10 text-white transition-all active:scale-95"><PhArrowRight weight="bold" /></button>
        </div>
      </div>
      
      <div ref="destinationsScroll" class="flex gap-6 overflow-x-auto pb-8 hide-scroll snap-x scroll-smooth px-6 md:px-10 max-w-[1440px] mx-auto">
        <div 
          v-for="place in destinations" 
          :key="place.id" 
          class="relative w-[340px] lg:w-[400px] aspect-[4/3] flex-shrink-0 rounded-[1.5rem] overflow-hidden group cursor-pointer border border-white/5 snap-start"
        >
          <img :src="place.image" :alt="place.name" class="absolute inset-0 w-full h-full object-cover transition-transform duration-1000 group-hover:scale-110" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/90 via-black/30 to-transparent group-hover:from-black/90 transition-colors duration-500"></div>
          
          <div class="absolute bottom-6 left-8 right-8">
            <div class="text-[11px] font-bold text-primary uppercase tracking-[0.2em] mb-2">{{ place.city }}</div>
            <div class="text-2xl font-bold font-heading text-white group-hover:text-primary transition-colors">{{ place.name }}</div>
            <div class="text-[13px] text-white/50 font-medium mt-1">{{ place.events }} sự kiện đang diễn ra</div>
          </div>
        </div>
      </div>
    </section>

    <!-- PROMO TYPOGRAPHY BANNER -->
    <section class="max-w-[1440px] mx-auto px-6 md:px-10 py-10">
      <div class="group rounded-[2rem] bg-[#111916] border border-white/5 p-12 lg:p-20 text-center relative overflow-hidden">
        <div class="absolute inset-0 opacity-10 mix-blend-overlay pointer-events-none" style="background-image: url('data:image/svg+xml,%3Csvg viewBox=%220 0 200 200%22 xmlns=%22http://www.w3.org/2000/svg%22%3E%3Cfilter id=%22noiseFilter%22%3E%3CfeTurbulence type=%22fractalNoise%22 baseFrequency=%220.65%22 numOctaves=%223%22 stitchTiles=%22stitch%22/%3E%3C/filter%3E%3Crect width=%22100%25%22 height=%22100%25%22 filter=%22url(%23noiseFilter)%22/%3E%3C/svg%3E');"></div>
        
        <div class="relative z-10 max-w-3xl mx-auto flex flex-col items-center">
          <PhTicket weight="duotone" class="text-6xl text-primary mb-6 opacity-80" />
          <h2 class="text-4xl md:text-6xl font-black font-heading text-white leading-[1.1] mb-6">
            Mở khóa <span class="text-primary">Đặc Quyền</span> Săn Vé Sớm
          </h2>
          <p class="text-lg text-white/60 font-medium mb-10 max-w-xl">
            Trở thành thành viên TicketHub để nhận thông báo sớm nhất, chỗ ngồi đẹp nhất và tiết kiệm đến 40% cho các sự kiện hàng đầu.
          </p>
          <BaseButton variant="primary" size="lg" class="!px-12 !rounded-full !text-base shadow-[0_0_30px_rgba(0,200,83,0.2)] hover:scale-105 active:scale-95 transition-transform" @click="router.push('/early-bird')">
            Tham gia ngay
          </BaseButton>
        </div>
      </div>
    </section>

    <!-- CATEGORIES -->
    <section v-for="(cat, idx) in categories" :key="cat.key" class="py-12">
      <div class="max-w-[1440px] mx-auto px-6 md:px-10 mb-10 flex items-center justify-between">
        <h2 class="text-3xl lg:text-4xl font-bold font-heading text-white flex items-center gap-4">
          <component :is="cat.icon" weight="fill" class="text-white/30" /> 
          {{ cat.label }}
        </h2>
        <div class="flex items-center gap-4">
          <router-link to="/search" class="text-sm font-bold text-white/50 hover:text-white hidden md:flex items-center gap-2 transition-colors">
            Khám phá <PhArrowRight weight="bold" />
          </router-link>
          <div class="flex gap-2">
            <button @click="scrollContainer($refs['catScroll_' + idx][0], -350)" class="w-10 h-10 rounded-full border border-white/20 flex items-center justify-center hover:bg-white/10 text-white transition-all active:scale-95"><PhArrowLeft weight="bold" /></button>
            <button @click="scrollContainer($refs['catScroll_' + idx][0], 350)" class="w-10 h-10 rounded-full border border-white/20 flex items-center justify-center hover:bg-white/10 text-white transition-all active:scale-95"><PhArrowRight weight="bold" /></button>
          </div>
        </div>
      </div>

      <div :ref="'catScroll_' + idx" class="flex gap-6 overflow-x-auto pb-12 hide-scroll snap-x scroll-smooth px-6 md:px-10 max-w-[1440px] mx-auto">
        <EventCard 
          v-for="e in cat.items" 
          :key="e.id" 
          :event="e" 
          class="w-[300px] lg:w-[340px] flex-shrink-0 snap-start"
        />
      </div>
    </section>

    <!-- TRUSTED BY -->
    <section class="max-w-[1440px] mx-auto px-6 md:px-10 py-20 border-t border-white/5">
      <div class="text-center text-[10px] uppercase tracking-[0.3em] font-bold text-white/30 mb-10">Đối tác đồng hành</div>
      <div class="flex flex-wrap justify-center items-center gap-12 lg:gap-24 opacity-40 grayscale hover:grayscale-0 transition-all duration-700">
        <div class="text-2xl font-black font-heading text-white">TICKETBOX</div>
        <div class="text-2xl font-black font-heading text-white">SPOTIFY</div>
        <div class="text-2xl font-black font-heading text-white">VIETNAM AIRLINES</div>
        <div class="text-2xl font-black font-heading text-white">VINPEARL</div>
        <div class="text-2xl font-black font-heading text-white">SHOPEE</div>
      </div>
    </section>

  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { getEvents, selectEvent, fetchStars, fetchDestinations } from '../stores/eventStore'
import EventCard from '../components/EventCard.vue'
import BaseButton from '../components/ui/BaseButton.vue'
import { 
  PhArrowLeft, PhArrowRight, PhStar, PhMapPin, PhTicket, 
  PhMicrophoneStage, PhTrophy, PhMaskHappy, PhCompass, PhBooks 
} from '@phosphor-icons/vue'

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
    { key: 'concerts', icon: PhMicrophoneStage, label: 'Nhạc & Concert' },
    { key: 'sports', icon: PhTrophy, label: 'Thể thao' },
    { key: 'arts', icon: PhMaskHappy, label: 'Nghệ thuật' },
    { key: 'experiences', icon: PhCompass, label: 'Trải nghiệm' },
    { key: 'workshops', icon: PhBooks, label: 'Workshop' },
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
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
