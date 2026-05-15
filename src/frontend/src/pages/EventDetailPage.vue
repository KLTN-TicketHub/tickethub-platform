<template>
  <div v-if="event" class="flex flex-col pb-20 bg-bg min-h-screen">
    <!-- Premium Hero Section (Parallax Ready) -->
    <div class="relative w-full h-[60vh] md:h-[75vh] overflow-hidden">
      <!-- Parallax Background Image -->
      <div class="absolute inset-0 z-0">
        <img 
          :src="event.image" 
          :alt="event.title" 
          class="w-full h-full object-cover scale-110 motion-safe:animate-parallax"
        />
        <!-- Multi-layer Gradient Overlay -->
        <div class="absolute inset-0 bg-gradient-to-t from-bg via-bg/40 to-transparent"></div>
        <div class="absolute inset-0 bg-gradient-to-r from-bg/60 to-transparent"></div>
      </div>

      <!-- Hero Content -->
      <div class="relative z-10 max-w-[1400px] mx-auto px-6 md:px-10 h-full flex flex-col justify-end pb-16 md:pb-24">
        <div class="max-w-4xl space-y-6 animate-fade-up">
          <!-- Category Badge -->
          <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/20 border border-primary/30 backdrop-blur-md">
            <span class="w-1.5 h-1.5 rounded-full bg-primary animate-pulse"></span>
            <span class="text-[11px] font-bold text-primary uppercase tracking-[0.2em]">{{ categoryLabel }}</span>
          </div>

          <h1 class="text-4xl md:text-7xl font-bold font-heading text-main leading-[1.1] tracking-tight">
            {{ event.title }}
          </h1>

          <div class="flex flex-wrap items-center gap-6 text-[15px] text-muted">
            <div class="flex items-center gap-2 px-4 py-2 bg-white/5 backdrop-blur-md rounded-full border border-white/10">
              <span class="text-primary text-lg">📅</span>
              <span class="font-bold text-main">{{ formatDate(event.dateStart) }}</span>
            </div>
            <div class="flex items-center gap-2 px-4 py-2 bg-white/5 backdrop-blur-md rounded-full border border-white/10">
              <span class="text-primary text-lg">📍</span>
              <span class="font-bold text-main">{{ event.location?.name }}</span>
            </div>
            <div class="flex items-center gap-2 px-4 py-2 bg-white/5 backdrop-blur-md rounded-full border border-white/10">
              <span class="text-primary text-lg">🎟️</span>
              <span class="font-bold text-main">Từ {{ formatCurrency(event.priceRange?.min || 0) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content Layout -->
    <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-20 -mt-12">
      <div class="grid grid-cols-1 lg:grid-cols-[1fr,420px] gap-12">
        
        <!-- Left Column: Details -->
        <div class="space-y-16">
          
          <!-- Event Features Grid -->
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4 animate-fade-up [animation-delay:200ms]">
            <div v-for="h in highlights.slice(0, 4)" :key="h.text" 
                 class="group p-6 bg-card border border-border-main rounded-3xl hover:border-primary/50 transition-all duration-500">
              <div class="w-12 h-12 rounded-2xl bg-surface flex items-center justify-center text-2xl mb-4 group-hover:scale-110 group-hover:bg-primary/10 transition-all">
                {{ h.icon }}
              </div>
              <div class="text-[13px] font-bold text-main group-hover:text-primary transition-colors">{{ h.text }}</div>
            </div>
          </div>

          <!-- About Section -->
          <section id="about" class="animate-fade-up [animation-delay:300ms]">
            <div class="flex items-center gap-4 mb-8">
              <div class="h-px flex-1 bg-border-main"></div>
              <h2 class="font-heading text-2xl font-bold text-main uppercase tracking-widest px-4">Giới thiệu</h2>
              <div class="h-px flex-1 bg-border-main"></div>
            </div>
            <div class="prose prose-invert max-w-none">
              <p class="text-lg leading-relaxed text-muted font-medium whitespace-pre-wrap">
                {{ event.description || defaultDescription }}
              </p>
            </div>
          </section>

          <!-- Artist/Performers Section -->
          <section v-if="event.performers?.length" id="artists" class="animate-fade-up [animation-delay:400ms]">
            <h2 class="font-heading text-2xl font-bold text-main mb-8 flex items-center gap-3">
              <span class="w-8 h-1 bg-primary rounded-full"></span>
              Đội ngũ & Nghệ sĩ
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div v-for="p in event.performers" :key="p.name" 
                   class="relative group flex items-center gap-5 p-5 bg-surface border border-border-main rounded-3xl hover:border-primary transition-all overflow-hidden">
                <div class="absolute inset-0 bg-gradient-to-r from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
                <div class="relative w-16 h-16 rounded-2xl bg-primary/20 flex items-center justify-center text-3xl font-bold text-primary group-hover:scale-105 transition-transform">
                  {{ p.name.charAt(0) }}
                </div>
                <div class="relative">
                  <h4 class="text-[17px] font-bold text-main">{{ p.name }}</h4>
                  <p class="text-[13px] text-muted font-medium uppercase tracking-wider">{{ p.role }}</p>
                </div>
                <div class="ml-auto opacity-0 group-hover:opacity-100 transition-all translate-x-4 group-hover:translate-x-0">
                  <span class="text-primary">✨</span>
                </div>
              </div>
            </div>
          </section>

          <!-- Venue & Logistics Section -->
          <section id="venue" class="animate-fade-up [animation-delay:500ms]">
             <h2 class="font-heading text-2xl font-bold text-main mb-8 flex items-center gap-3">
              <span class="w-8 h-1 bg-primary rounded-full"></span>
              Địa điểm & Bản đồ
            </h2>
            <div class="bg-card border border-border-main rounded-[2.5rem] overflow-hidden p-2">
              <div class="relative aspect-[21/9] rounded-[2rem] bg-surface overflow-hidden border border-border-main">
                <!-- Mock Map Illustration -->
                <div class="absolute inset-0 opacity-20 grayscale bg-[url('https://www.transparenttextures.com/patterns/graphy.png')]"></div>
                <div class="absolute inset-0 flex flex-col items-center justify-center text-center p-8">
                  <div class="w-16 h-16 rounded-full bg-primary/20 flex items-center justify-center mb-4 animate-bounce">
                    <span class="text-3xl">📍</span>
                  </div>
                  <h4 class="text-xl font-bold text-main mb-2">{{ event.location?.name }}</h4>
                  <p class="text-muted text-sm max-w-sm">{{ event.location?.address || 'Thông tin địa điểm chi tiết sẽ được gửi kèm vé điện tử.' }}</p>
                </div>
                <!-- Interactive Overlay -->
                <div class="absolute inset-0 bg-primary/5 opacity-0 hover:opacity-100 transition-opacity flex items-center justify-center cursor-pointer">
                  <BaseButton variant="glass" size="sm">Xem bản đồ chi tiết</BaseButton>
                </div>
              </div>
            </div>
          </section>

          <!-- Gallery Preview -->
          <section id="gallery" class="animate-fade-up [animation-delay:600ms]">
            <h2 class="font-heading text-2xl font-bold text-main mb-8 flex items-center gap-3">
              <span class="w-8 h-1 bg-primary rounded-full"></span>
              Khoảnh khắc
            </h2>
            <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
              <div v-for="i in 3" :key="i" class="aspect-square rounded-3xl overflow-hidden border border-border-main hover:border-primary transition-all cursor-pointer group">
                <img 
                  :src="`https://images.unsplash.com/photo-${1500000000000 + i}?w=600&q=80`" 
                  class="w-full h-full object-cover grayscale opacity-60 group-hover:grayscale-0 group-hover:opacity-100 transition-all duration-700 group-hover:scale-110"
                />
              </div>
            </div>
          </section>

          <!-- Policies Section -->
          <section id="policy" class="animate-fade-up [animation-delay:700ms]">
            <h2 class="font-heading text-2xl font-bold text-main mb-8 flex items-center gap-3">
              <span class="w-8 h-1 bg-primary rounded-full"></span>
              Lưu ý
            </h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div v-for="(policy, idx) in policies" :key="idx" 
                   class="p-6 bg-surface/40 border border-border-main rounded-3xl hover:bg-surface/60 transition-all">
                <div class="text-2xl mb-4">{{ policy.icon }}</div>
                <h4 class="text-[15px] font-bold text-main mb-2">{{ policy.title }}</h4>
                <p class="text-[13px] text-muted leading-relaxed">{{ policy.text }}</p>
              </div>
            </div>
          </section>
        </div>

        <!-- Right Column: Sticky Sidebar -->
        <aside class="relative">
          <div class="sticky top-28 space-y-6">
            <!-- Main Booking Card -->
            <div class="bg-card/80 backdrop-blur-2xl border border-border-main rounded-[2.5rem] p-8 shadow-2xl overflow-hidden">
              <div class="absolute -top-24 -right-24 w-48 h-48 bg-primary/10 blur-[80px] pointer-events-none"></div>
              
              <div class="flex items-center justify-between mb-8">
                <h3 class="font-heading text-xl font-bold text-main">Chọn vé</h3>
                <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Sắp hết vé!</span>
              </div>

              <!-- Tiers Selection -->
              <div class="space-y-3 mb-8">
                <template v-if="tiers.length > 0">
                  <button
                    v-for="(tier, i) in tiers"
                    :key="i"
                    @click="selectedTier = i"
                    class="group relative w-full p-5 text-left rounded-2xl border-2 transition-all duration-300 overflow-hidden"
                    :class="[
                      selectedTier === i 
                        ? 'bg-primary/10 border-primary' 
                        : 'bg-surface/50 border-border-main hover:border-primary/30'
                    ]"
                  >
                    <div class="relative z-10 flex flex-col">
                      <div class="flex justify-between items-center mb-1">
                        <span class="font-bold text-[16px] group-hover:text-primary transition-colors" 
                              :class="selectedTier === i ? 'text-primary' : 'text-main'">
                          {{ tier.name }}
                        </span>
                        <span class="font-heading font-bold text-lg" 
                              :class="selectedTier === i ? 'text-primary' : 'text-primary/70'">
                          {{ formatCurrency(tier.price) }}
                        </span>
                      </div>
                      <span class="text-[12px] text-muted">Số lượng có hạn</span>
                    </div>
                  </button>
                </template>
              </div>

              <!-- Quantity Selector -->
              <div class="flex items-center justify-between p-4 bg-surface/50 rounded-2xl border border-border-main mb-8">
                <span class="text-sm font-bold text-muted">Số lượng</span>
                <div class="flex items-center gap-5">
                  <button @click="qty = Math.max(1, qty - 1)" :disabled="qty <= 1"
                          class="w-8 h-8 rounded-full border border-border-main flex items-center justify-center text-main hover:border-primary hover:text-primary disabled:opacity-20 transition-all">
                    −
                  </button>
                  <span class="text-lg font-bold text-main w-6 text-center">{{ qty }}</span>
                  <button @click="qty = Math.min(10, qty + 1)" :disabled="qty >= 10"
                          class="w-8 h-8 rounded-full border border-border-main flex items-center justify-center text-main hover:border-primary hover:text-primary disabled:opacity-20 transition-all">
                    +
                  </button>
                </div>
              </div>

              <!-- Summary & CTA -->
              <div class="space-y-6 pt-6 border-t border-border-main">
                <div class="flex justify-between items-end">
                  <span class="text-sm text-muted font-medium pb-1">Tổng thanh toán</span>
                  <div class="text-3xl font-bold font-heading text-primary leading-none">{{ formatCurrency(totalPrice) }}</div>
                </div>

                <BaseButton 
                  variant="primary" 
                  size="lg" 
                  class="w-full !rounded-[1.5rem] !py-5 shadow-xl shadow-primary/20 text-lg"
                  :disabled="event.status !== 'upcoming'"
                  @click="handleBuyTicket"
                >
                  {{ event.status === 'upcoming' ? 'Mua vé ngay 🎟️' : 'Đã kết thúc ⛔' }}
                </BaseButton>
              </div>

              <!-- Security Badges -->
              <div class="mt-8 pt-6 border-t border-border-main flex items-center justify-center gap-6">
                <div class="flex flex-col items-center gap-1.5 opacity-60">
                  <span class="text-lg">🔒</span>
                  <span class="text-[10px] font-bold text-muted uppercase tracking-tighter">Bảo mật</span>
                </div>
                <div class="flex flex-col items-center gap-1.5 opacity-60">
                  <span class="text-lg">📧</span>
                  <span class="text-[10px] font-bold text-muted uppercase tracking-tighter">Vé E-mail</span>
                </div>
                <div class="flex flex-col items-center gap-1.5 opacity-60">
                  <span class="text-lg">⚡</span>
                  <span class="text-[10px] font-bold text-muted uppercase tracking-tighter">Nhanh chóng</span>
                </div>
              </div>
            </div>

            <!-- Share / Save Card -->
            <div class="p-4 flex gap-4">
              <button class="flex-1 p-4 bg-card border border-border-main rounded-2xl hover:border-primary transition-all flex items-center justify-center gap-2 group">
                <span class="text-lg group-hover:scale-125 transition-transform">❤️</span>
                <span class="text-sm font-bold text-main">Lưu</span>
              </button>
              <button class="flex-1 p-4 bg-card border border-border-main rounded-2xl hover:border-primary transition-all flex items-center justify-center gap-2 group">
                <span class="text-lg group-hover:scale-125 transition-transform">🔗</span>
                <span class="text-sm font-bold text-main">Chia sẻ</span>
              </button>
            </div>
          </div>
        </aside>
      </div>
    </div>

    <!-- Sticky Bottom Bar (Ticketbox Style) -->
    <Transition
      enter-active-class="transition duration-500 ease-[cubic-bezier(0.23,1,0.32,1)]"
      enter-from-class="translate-y-full opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition duration-500 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-full opacity-0"
    >
      <div v-if="showStickyBar" class="fixed bottom-0 left-0 right-0 z-[100] bg-[#0A0F0D]/80 backdrop-blur-3xl border-t border-white/10 py-4 px-6 md:px-10 shadow-[0_-20px_50px_rgba(0,0,0,0.5)]">
        <div class="max-w-[1400px] mx-auto flex items-center justify-between gap-8">
          <div class="flex items-center gap-6 min-w-0">
            <div class="w-14 h-14 rounded-xl overflow-hidden hidden sm:block border border-white/10 flex-shrink-0">
              <img :src="event.image" class="w-full h-full object-cover" />
            </div>
            <div class="min-w-0">
              <h4 class="text-white font-bold truncate text-lg">{{ event.title }}</h4>
              <div class="flex items-center gap-3 text-muted text-[13px] font-medium">
                <span>{{ formatDate(event.dateStart) }}</span>
                <span class="w-1 h-1 rounded-full bg-white/20"></span>
                <span class="text-primary font-bold">Từ {{ formatCurrency(event.priceRange?.min || 0) }}</span>
              </div>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <div class="hidden md:flex flex-col items-end mr-4">
              <span class="text-[11px] text-muted font-bold uppercase tracking-widest">Tiết kiệm thời gian</span>
              <span class="text-white font-bold text-[14px]">Đặt vé chỉ trong 30s</span>
            </div>
            <BaseButton 
              variant="primary" 
              size="lg" 
              class="!px-10 !rounded-2xl shadow-lg shadow-primary/20 whitespace-nowrap"
              @click="handleBuyTicket"
            >
              Mua vé ngay 🎟️
            </BaseButton>
          </div>
        </div>
      </div>
    </Transition>
  </div>

  <!-- Not Found State -->
  <div v-else class="flex flex-col items-center justify-center py-32 px-6 text-center animate-fade-up">
    <div class="text-[80px] mb-6 grayscale opacity-30">🔍</div>
    <h2 class="font-heading text-3xl font-bold text-main mb-3">Không tìm thấy sự kiện</h2>
    <p class="text-muted max-w-md mx-auto mb-10 leading-relaxed">
      Sự kiện này có thể đã bị xóa hoặc không tồn tại trong hệ thống.
    </p>
    <BaseButton variant="primary" size="lg" @click="router.push('/')">
      ← Quay về trang chủ
    </BaseButton>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getEventById, openBooking } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'

const route = useRoute()
const router = useRouter()
const event = computed(() => getEventById(route.params.id))

const selectedTier = ref(0)
const qty = ref(1)
const showStickyBar = ref(false)

const handleScroll = () => {
  showStickyBar.value = window.scrollY > 600
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
    { icon: '🎶', text: 'Âm thanh vòm' },
    { icon: '📸', text: 'Photo Zone' },
    { icon: '🍻', text: 'Khu ẩm thực' },
    { icon: '🎁', text: 'Quà độc quyền' },
  ]
  return [
    { icon: '🌟', text: 'Trải nghiệm đỉnh cao' },
    { icon: '📸', text: 'Ghi lại khoảnh khắc' },
    { icon: '🗺️', text: 'Hướng dẫn tận tâm' },
    { icon: '🎒', text: 'Dịch vụ chu đáo' },
  ]
})

const policies = [
  { icon: '🎟️', title: 'Chính sách vé', text: 'Vé không hoàn trả sau khi mua. Vui lòng kiểm tra kỹ thông tin.' },
  { icon: '⏰', title: 'Thời gian', text: 'Cổng mở trước 60 phút. Vui lòng check-in sớm.' },
  { icon: '🔞', title: 'Độ tuổi', text: 'Sự kiện phù hợp cho khán giả từ 16 tuổi trở lên.' },
  { icon: '👗', title: 'Quy định', text: 'Trang phục tự do, lịch sự. Không mang đồ ăn uống vào.' },
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
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
@keyframes parallax {
  from { transform: scale(1.1) translateY(0); }
  to { transform: scale(1.2) translateY(20px); }
}

.animate-parallax {
  animation: parallax linear;
  animation-timeline: scroll();
}

.prose p {
  margin-bottom: 1.5rem;
}
</style>
