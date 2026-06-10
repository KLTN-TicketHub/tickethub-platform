<template>
  <div v-if="event" class="flex flex-col pb-20 bg-[#050807] min-h-screen">
    <!-- Premium Edge-to-Edge Hero Section -->
    <div class="relative w-full h-[75vh] md:h-[85vh] overflow-hidden">
      <!-- Background Image with Parallax effect -->
      <div class="absolute inset-0 z-0">
        <img 
          :src="event.image" 
          :alt="event.title" 
          class="w-full h-full object-cover scale-110 motion-safe:animate-parallax"
        />
        <!-- Multi-layer Gradient Overlay for deep contrast -->
        <div class="absolute inset-0 bg-gradient-to-t from-[#050807] via-[#050807]/60 to-transparent"></div>
        <div class="absolute inset-0 bg-gradient-to-r from-[#050807]/80 via-[#050807]/40 to-transparent"></div>
      </div>

      <!-- Hero Content -->
      <div class="relative z-10 max-w-[1400px] mx-auto px-6 md:px-10 h-full flex flex-col justify-end pb-16 md:pb-24">
        <div class="max-w-4xl space-y-6 animate-fade-up">
          <!-- Category Badge -->
          <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/20 border border-primary/30 backdrop-blur-md shadow-[0_0_20px_rgba(0,200,83,0.15)]">
            <span class="w-2 h-2 rounded-full bg-primary animate-pulse"></span>
            <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">{{ categoryLabel }}</span>
          </div>

          <h1 class="text-5xl md:text-8xl font-black font-heading text-white leading-[1.05] tracking-tight">
            {{ event.title }}
          </h1>

          <div class="flex flex-wrap items-center gap-4 text-[15px]">
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10 shadow-inner">
              <PhCalendarBlank weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">{{ formatDate(event.dateStart) }}</span>
            </div>
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10 shadow-inner">
              <PhMapPin weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">{{ event.location?.name }}</span>
            </div>
            <div class="flex items-center gap-2.5 px-5 py-2.5 bg-white/5 backdrop-blur-md rounded-full border border-white/10 shadow-inner">
              <PhTicket weight="bold" class="text-primary text-xl" />
              <span class="font-bold text-white">Từ {{ formatCurrency(event.priceRange?.min || 0) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content Layout -->
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-20 -mt-12">
      <div class="grid grid-cols-1 lg:grid-cols-[1fr,440px] gap-12 xl:gap-16">
        
        <!-- Left Column: Details -->
        <div class="space-y-16">
          
          <!-- Event Features Bento Grid -->
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4 animate-fade-up [animation-delay:200ms]">
            <div v-for="(h, i) in highlights.slice(0, 4)" :key="i" 
                 class="group p-6 bg-[#111916] border border-white/5 rounded-3xl hover:border-primary/50 transition-all duration-500 hover:-translate-y-1">
              <div class="w-12 h-12 rounded-2xl bg-white/5 flex items-center justify-center text-2xl mb-4 group-hover:scale-110 group-hover:bg-primary/20 transition-all text-white/50 group-hover:text-primary">
                <component :is="h.icon" weight="duotone" />
              </div>
              <div class="text-[13px] font-bold text-white group-hover:text-primary transition-colors leading-tight">{{ h.text }}</div>
            </div>
          </div>

          <!-- About Section -->
          <section id="about" class="animate-fade-up [animation-delay:300ms] scroll-mt-24">
            <div class="flex items-center gap-6 mb-8">
              <h2 class="font-heading text-3xl font-black text-white uppercase tracking-widest whitespace-nowrap">Giới thiệu</h2>
              <div class="h-px flex-1 bg-gradient-to-r from-white/20 to-transparent"></div>
            </div>
            <div class="prose prose-invert max-w-none">
              <p class="text-[17px] leading-[1.8] text-white/70 font-medium whitespace-pre-wrap">
                {{ event.description || defaultDescription }}
              </p>
            </div>
          </section>

          <!-- Artist/Performers Section -->
          <section v-if="event.performers?.length" id="artists" class="animate-fade-up [animation-delay:400ms] scroll-mt-24">
            <h2 class="font-heading text-3xl font-black text-white mb-8 flex items-center gap-4">
              <PhStar weight="fill" class="text-primary" />
              Đội ngũ & Nghệ sĩ
            </h2>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div v-for="p in event.performers" :key="p.name" 
                   class="relative group flex items-center gap-5 p-5 bg-[#111916] border border-white/5 rounded-[2rem] hover:border-primary/50 transition-all overflow-hidden cursor-pointer">
                <div class="absolute inset-0 bg-gradient-to-r from-primary/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
                <div class="relative w-16 h-16 rounded-[1.25rem] bg-white/5 border border-white/10 flex items-center justify-center text-3xl font-bold text-white group-hover:scale-105 group-hover:bg-primary group-hover:text-black transition-all">
                  {{ p.name.charAt(0) }}
                </div>
                <div class="relative flex-1">
                  <h4 class="text-[18px] font-bold text-white leading-tight group-hover:text-primary transition-colors">{{ p.name }}</h4>
                  <p class="text-[12px] text-white/50 font-bold uppercase tracking-[0.15em] mt-0.5">{{ p.role }}</p>
                </div>
                <div class="relative w-8 h-8 rounded-full bg-white/5 flex items-center justify-center opacity-0 group-hover:opacity-100 -translate-x-4 group-hover:translate-x-0 transition-all">
                  <PhArrowRight weight="bold" class="text-white" />
                </div>
              </div>
            </div>
          </section>

          <!-- Venue & Logistics Section -->
          <section id="venue" class="animate-fade-up [animation-delay:500ms] scroll-mt-24">
             <h2 class="font-heading text-3xl font-black text-white mb-8 flex items-center gap-4">
              <PhMapPinLine weight="fill" class="text-primary" />
              Địa điểm & Bản đồ
            </h2>
            <div class="bg-[#111916] border border-white/5 rounded-[3rem] p-2 overflow-hidden group">
              <div class="relative aspect-[21/9] sm:aspect-[21/8] rounded-[2.5rem] bg-[#0A0F0D] overflow-hidden border border-white/5">
                <!-- Map Decor -->
                <div class="absolute inset-0 opacity-10 bg-[radial-gradient(circle_at_center,_white_1px,_transparent_1px)]" style="background-size: 24px 24px;"></div>
                <div class="absolute inset-0 flex flex-col items-center justify-center text-center p-8 z-10">
                  <div class="w-16 h-16 rounded-full bg-primary/20 border border-primary/30 flex items-center justify-center mb-5 animate-bounce shadow-[0_0_30px_rgba(0,200,83,0.2)] text-primary">
                    <PhMapPin weight="fill" class="text-3xl" />
                  </div>
                  <h4 class="text-2xl font-black font-heading text-white mb-2 tracking-tight">{{ event.location?.name }}</h4>
                  <p class="text-white/50 text-sm max-w-sm font-medium">{{ event.location?.address || 'Thông tin địa điểm chi tiết sẽ được hiển thị khi bạn mua vé thành công.' }}</p>
                </div>
                <!-- Interactive Overlay -->
                <div class="absolute inset-0 bg-primary/10 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center cursor-pointer z-20 backdrop-blur-sm">
                  <BaseButton variant="primary" size="lg" class="shadow-2xl flex items-center gap-2">
                    <PhNavigationArrow weight="fill" /> Xem bản đồ chi tiết
                  </BaseButton>
                </div>
              </div>
            </div>
          </section>

          <!-- Gallery Preview Bento -->
          <section id="gallery" class="animate-fade-up [animation-delay:600ms] scroll-mt-24">
            <h2 class="font-heading text-3xl font-black text-white mb-8 flex items-center gap-4">
              <PhImages weight="fill" class="text-primary" />
              Khoảnh khắc
            </h2>
            <div class="grid grid-cols-2 md:grid-cols-3 gap-4 auto-rows-[200px]">
              <div v-for="i in 3" :key="i" 
                   class="rounded-3xl overflow-hidden border border-white/5 hover:border-primary/50 transition-all cursor-pointer group"
                   :class="{'md:col-span-2 md:row-span-2': i === 1}">
                <img 
                  :src="`https://images.unsplash.com/photo-${1500000000000 + i}?w=800&q=80`" 
                  class="w-full h-full object-cover grayscale opacity-50 group-hover:grayscale-0 group-hover:opacity-100 transition-all duration-700 group-hover:scale-110"
                />
              </div>
            </div>
          </section>

          <!-- Policies Section -->
          <section id="policy" class="animate-fade-up [animation-delay:700ms] scroll-mt-24">
            <h2 class="font-heading text-3xl font-black text-white mb-8 flex items-center gap-4">
              <PhInfo weight="fill" class="text-primary" />
              Thông tin cần biết
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div v-for="(policy, idx) in policies" :key="idx" 
                   class="p-6 bg-[#111916] border border-white/5 rounded-[2rem] hover:bg-white/5 transition-all">
                <div class="text-3xl mb-5 text-white/40"><component :is="policy.icon" weight="duotone" /></div>
                <h4 class="text-[16px] font-bold text-white mb-2">{{ policy.title }}</h4>
                <p class="text-[14px] text-white/50 leading-relaxed font-medium">{{ policy.text }}</p>
              </div>
            </div>
          </section>
        </div>

        <!-- Right Column: Sticky Sidebar Booking Card -->
        <aside class="relative">
          <div class="sticky top-32 space-y-6">
            <div class="bg-[#0A0F0D]/90 backdrop-blur-3xl border border-white/10 rounded-[3rem] p-8 lg:p-10 shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] overflow-hidden relative">
              <!-- Glow -->
              <div class="absolute -top-32 -right-32 w-64 h-64 bg-primary/20 blur-[100px] pointer-events-none rounded-full"></div>
              
              <div class="flex items-center justify-between mb-8 relative z-10">
                <h3 class="font-heading text-2xl font-black text-white">Chọn hạng vé</h3>
                <div class="px-3 py-1 bg-warning/10 border border-warning/20 rounded-full text-warning text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5 animate-pulse">
                  <PhFire weight="fill" /> Sắp hết
                </div>
              </div>

              <!-- Tiers Selection -->
              <div class="space-y-3 mb-8 relative z-10">
                <template v-if="tiers.length > 0">
                  <button
                    v-for="(tier, i) in tiers"
                    :key="i"
                    @click="selectedTier = i"
                    class="group relative w-full p-5 text-left rounded-2xl border-2 transition-all duration-300 overflow-hidden cursor-pointer"
                    :class="[
                      selectedTier === i 
                        ? 'bg-primary/10 border-primary' 
                        : 'bg-white/5 border-white/5 hover:border-white/20'
                    ]"
                  >
                    <div class="relative z-10 flex flex-col">
                      <div class="flex justify-between items-center mb-1.5">
                        <span class="font-bold text-[16px] group-hover:text-white transition-colors" 
                              :class="selectedTier === i ? 'text-primary' : 'text-white/80'">
                          {{ tier.name }}
                        </span>
                        <span class="font-heading font-black text-xl" 
                              :class="selectedTier === i ? 'text-primary' : 'text-white'">
                          {{ formatCurrency(tier.price) }}
                        </span>
                      </div>
                      <span class="text-[12px] font-medium" :class="selectedTier === i ? 'text-primary/70' : 'text-white/40'">Số lượng có hạn</span>
                    </div>
                  </button>
                </template>
              </div>

              <!-- Quantity Selector -->
              <div class="flex items-center justify-between p-5 bg-white/5 rounded-2xl border border-white/5 mb-8 relative z-10">
                <span class="text-[14px] font-bold text-white/70 uppercase tracking-widest">Số lượng</span>
                <div class="flex items-center gap-6">
                  <button @click="qty = Math.max(1, qty - 1)" :disabled="qty <= 1"
                          class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-white hover:border-primary hover:text-primary hover:bg-primary/10 disabled:opacity-30 disabled:hover:border-white/10 disabled:hover:text-white disabled:hover:bg-white/5 transition-all cursor-pointer">
                    <PhMinus weight="bold" />
                  </button>
                  <span class="text-xl font-black text-white w-6 text-center font-heading">{{ qty }}</span>
                  <button @click="qty = Math.min(10, qty + 1)" :disabled="qty >= 10"
                          class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-white hover:border-primary hover:text-primary hover:bg-primary/10 disabled:opacity-30 disabled:hover:border-white/10 disabled:hover:text-white disabled:hover:bg-white/5 transition-all cursor-pointer">
                    <PhPlus weight="bold" />
                  </button>
                </div>
              </div>

              <!-- Summary & CTA -->
              <div class="space-y-6 pt-6 border-t border-white/10 relative z-10">
                <div class="flex justify-between items-end">
                  <span class="text-[13px] text-white/50 font-bold uppercase tracking-widest pb-1">Tổng cộng</span>
                  <div class="text-4xl font-black font-heading text-primary leading-none tracking-tight">{{ formatCurrency(totalPrice) }}</div>
                </div>

                <BaseButton 
                  variant="primary" 
                  size="lg" 
                  class="w-full !rounded-2xl !py-5 shadow-[0_0_40px_rgba(0,200,83,0.2)] hover:shadow-[0_0_60px_rgba(0,200,83,0.4)] text-lg flex justify-center items-center gap-2"
                  :disabled="event.status !== 'upcoming'"
                  @click="handleBuyTicket"
                >
                  <PhTicket v-if="event.status === 'upcoming'" weight="fill" />
                  <PhProhibit v-else weight="fill" />
                  {{ event.status === 'upcoming' ? 'Mua vé ngay' : 'Đã kết thúc' }}
                </BaseButton>
              </div>

              <!-- Security Badges -->
              <div class="mt-8 pt-6 border-t border-white/5 flex items-center justify-between gap-2 relative z-10">
                <div class="flex flex-col items-center gap-2 text-white/40 flex-1">
                  <PhShieldCheck class="text-2xl" weight="duotone" />
                  <span class="text-[10px] font-bold uppercase tracking-widest">Bảo mật</span>
                </div>
                <div class="flex flex-col items-center gap-2 text-white/40 flex-1 border-l border-r border-white/5">
                  <PhEnvelopeSimple class="text-2xl" weight="duotone" />
                  <span class="text-[10px] font-bold uppercase tracking-widest">Vé E-mail</span>
                </div>
                <div class="flex flex-col items-center gap-2 text-white/40 flex-1">
                  <PhLightning class="text-2xl" weight="duotone" />
                  <span class="text-[10px] font-bold uppercase tracking-widest">Tức thì</span>
                </div>
              </div>
            </div>

            <!-- Share / Save Action Buttons -->
            <div class="flex gap-4">
              <button class="flex-1 py-4 bg-[#111916] border border-white/5 rounded-2xl hover:bg-white/5 transition-all flex items-center justify-center gap-2 text-white/70 hover:text-white group cursor-pointer font-bold text-sm">
                <PhHeart class="text-lg group-hover:text-[#f43f5e] transition-colors" weight="bold" /> Lưu sự kiện
              </button>
              <button class="flex-1 py-4 bg-[#111916] border border-white/5 rounded-2xl hover:bg-white/5 transition-all flex items-center justify-center gap-2 text-white/70 hover:text-white group cursor-pointer font-bold text-sm">
                <PhShareNetwork class="text-lg group-hover:text-primary transition-colors" weight="bold" /> Chia sẻ
              </button>
            </div>
          </div>
        </aside>
      </div>
    </div>

    <!-- Sticky Bottom Bar for Mobile / Scrolling (Hidden on large screens when sidebar is visible) -->
    <Transition
      enter-active-class="transition duration-500 ease-[cubic-bezier(0.23,1,0.32,1)]"
      enter-from-class="translate-y-full opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition duration-500 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-full opacity-0"
    >
      <div v-if="showStickyBar" class="fixed bottom-0 left-0 right-0 z-[100] lg:hidden bg-[#0A0F0D]/90 backdrop-blur-xl border-t border-white/10 p-4 pb-safe shadow-[0_-20px_50px_rgba(0,0,0,0.8)]">
        <div class="max-w-[1400px] mx-auto flex items-center justify-between gap-4">
          <div class="flex-1 min-w-0">
            <span class="text-[11px] text-white/50 font-bold uppercase tracking-widest block mb-0.5">Giá từ</span>
            <div class="text-xl font-black font-heading text-primary truncate">{{ formatCurrency(event.priceRange?.min || 0) }}</div>
          </div>
          <BaseButton 
            variant="primary" 
            size="lg" 
            class="!px-8 !rounded-xl shadow-lg shadow-primary/20 whitespace-nowrap"
            @click="handleBuyTicket"
          >
            Mua ngay
          </BaseButton>
        </div>
      </div>
    </Transition>
  </div>

  <!-- Not Found State -->
  <div v-else class="flex flex-col items-center justify-center py-32 px-6 text-center animate-fade-up min-h-screen bg-[#050807]">
    <div class="w-24 h-24 bg-white/5 rounded-full flex items-center justify-center text-5xl text-white/20 mb-6">
      <PhMagnifyingGlass weight="duotone" />
    </div>
    <h2 class="font-heading text-4xl font-black text-white mb-3">Không tìm thấy sự kiện</h2>
    <p class="text-white/50 max-w-md mx-auto mb-10 font-medium">Sự kiện này có thể đã bị xóa hoặc không tồn tại trong hệ thống.</p>
    <BaseButton variant="primary" size="lg" @click="router.push('/')">
      Quay về trang chủ
    </BaseButton>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, markRaw } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getEventById, openBooking } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { 
  PhCalendarBlank, PhMapPin, PhTicket, PhStar, PhArrowRight, 
  PhMapPinLine, PhNavigationArrow, PhImages, PhInfo, PhFire,
  PhMinus, PhPlus, PhProhibit, PhShieldCheck, PhEnvelopeSimple,
  PhLightning, PhHeart, PhShareNetwork, PhMagnifyingGlass,
  PhMusicNotes, PhCamera, PhBeerBottle, PhGift, PhCrown, PhCompass, PhSuitcaseRolling
} from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()
const event = computed(() => getEventById(route.params.id))

const selectedTier = ref(0)
const qty = ref(1)
const showStickyBar = ref(false)

const handleScroll = () => {
  // Show sticky bar only when scrolling past the hero, and mainly on mobile
  showStickyBar.value = window.scrollY > window.innerHeight * 0.7 && window.innerWidth < 1024
}

const CATEGORY_LABELS = {
  concerts: 'Nhạc & Concert',
  arts: 'Sân khấu & Nghệ thuật',
  sports: 'Thể thao',
  experiences: 'Trải nghiệm',
  workshops: 'Workshop',
}

const categoryLabel = computed(() => CATEGORY_LABELS[event.value?.category] || event.value?.category || '')
const tiers = computed(() => event.value?.tiers || [])
const currentTierPrice = computed(() => {
  if (tiers.value.length > 0 && tiers.value[selectedTier.value]) {
    return tiers.value[selectedTier.value].price
  }
  return event.value?.priceRange?.min || 0
})
const totalPrice = computed(() => currentTierPrice.value * qty.value)

const highlights = computed(() => {
  const cat = event.value?.category
  if (cat === 'concerts') return [
    { icon: markRaw(PhMusicNotes), text: 'Âm thanh vòm' },
    { icon: markRaw(PhCamera), text: 'Photo Zone' },
    { icon: markRaw(PhBeerBottle), text: 'Khu ẩm thực' },
    { icon: markRaw(PhGift), text: 'Quà độc quyền' },
  ]
  return [
    { icon: markRaw(PhCrown), text: 'Trải nghiệm VIP' },
    { icon: markRaw(PhCamera), text: 'Ghi lại khoảnh khắc' },
    { icon: markRaw(PhCompass), text: 'Hướng dẫn tận tâm' },
    { icon: markRaw(PhSuitcaseRolling), text: 'Dịch vụ chu đáo' },
  ]
})

const policies = [
  { icon: markRaw(PhTicket), title: 'Chính sách vé', text: 'Vé không hoàn trả sau khi mua. Vui lòng kiểm tra kỹ thông tin.' },
  { icon: markRaw(PhCalendarBlank), title: 'Thời gian', text: 'Cổng mở trước 60 phút. Vui lòng check-in sớm.' },
  { icon: markRaw(PhShieldCheck), title: 'Độ tuổi', text: 'Sự kiện phù hợp cho khán giả từ 16 tuổi trở lên.' },
  { icon: markRaw(PhInfo), title: 'Quy định', text: 'Trang phục tự do, lịch sự. Không mang đồ ăn uống vào.' },
]

const defaultDescription = computed(() =>
  `Chào mừng bạn đến với sự kiện ${event.value?.title}. Một trải nghiệm đẳng cấp đang chờ đón bạn.`
)

const formatDate = (dateStr) => {
  if (!dateStr) return 'TBA'
  try {
    const d = new Date(dateStr)
    return d.toLocaleDateString('vi-VN', { weekday: 'long', day: '2-digit', month: 'long', year: 'numeric' })
  } catch(e) { return dateStr }
}

const formatCurrency = (amount) => {
  if (amount === 0) return 'Miễn phí'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount)
}

const handleBuyTicket = () => {
  if (event.value) openBooking(event.value)
}

onMounted(() => {
  window.scrollTo(0, 0)
  window.addEventListener('scroll', handleScroll, { passive: true })
  // Init state
  handleScroll()
  window.addEventListener('resize', handleScroll)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
  window.removeEventListener('resize', handleScroll)
})
</script>

<style scoped>
@keyframes parallax {
  from { transform: scale(1.1) translateY(0); }
  to { transform: scale(1.2) translateY(30px); }
}

.animate-parallax {
  animation: parallax linear;
  animation-timeline: scroll();
}

.pb-safe {
  padding-bottom: env(safe-area-inset-bottom, 1rem);
}
</style>
