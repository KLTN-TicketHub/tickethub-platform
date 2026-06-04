<template>
  <div class="home-page" data-testid="home-page">
    <!-- ═══ HERO SECTION ═══════════════════════════════════════════════════ -->
    <section
      class="relative py-20 md:py-32 flex flex-col items-center text-center px-4 overflow-hidden"
      data-testid="hero-section"
    >
      <!-- Background glow decoration -->
      <div
        class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] rounded-full opacity-15 blur-[120px] pointer-events-none"
        style="background: radial-gradient(circle, var(--color-primary) 0%, transparent 70%)"
      />

      <h1
        class="font-heading text-4xl sm:text-5xl md:text-6xl font-bold leading-tight max-w-4xl animate-fade-up opacity-0"
        data-testid="hero-headline"
      >
        Discover
        <span class="text-gradient-primary">Extraordinary</span>
        <br class="hidden sm:block" />
        Events Near You
      </h1>

      <p
        class="mt-6 text-lg md:text-xl text-muted max-w-2xl animate-fade-up opacity-0 stagger-2"
        data-testid="hero-subtitle"
      >
        Vietnam's premier platform for unforgettable concerts, sports, workshops, and beyond.
        Book your next experience in seconds.
      </p>

      <!-- Decorative search bar -->
      <div class="mt-10 w-full max-w-xl animate-fade-up opacity-0 stagger-3" data-testid="hero-search">
        <BaseInput
          v-model="heroSearch"
          placeholder="Search events, artists, venues…"
          data-testid="hero-search-input"
        >
          <template #prefix>
            <svg class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
              <circle cx="11" cy="11" r="8" />
              <line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
          </template>
        </BaseInput>
      </div>

      <!-- Quick stat pills -->
      <div class="mt-8 flex flex-wrap gap-3 justify-center animate-fade-up opacity-0 stagger-4">
        <span class="text-xs text-dimmed font-mono bg-white/[0.03] border border-border-main rounded-full px-4 py-1.5">
          🔥 Trending now
        </span>
        <span class="text-xs text-dimmed font-mono bg-white/[0.03] border border-border-main rounded-full px-4 py-1.5">
          🎵 Concerts
        </span>
        <span class="text-xs text-dimmed font-mono bg-white/[0.03] border border-border-main rounded-full px-4 py-1.5">
          ⚽ Sports
        </span>
        <span class="text-xs text-dimmed font-mono bg-white/[0.03] border border-border-main rounded-full px-4 py-1.5">
          🎨 Arts
        </span>
      </div>
    </section>

    <!-- ═══ FEATURED EVENTS ════════════════════════════════════════════════ -->
    <section class="px-4 md:px-8 max-w-7xl mx-auto" data-testid="featured-events-section">
      <div class="flex items-end justify-between mb-8 animate-fade-up opacity-0 stagger-1">
        <div>
          <p class="text-primary text-sm font-semibold tracking-wide uppercase mb-1">Featured</p>
          <h2 class="font-heading text-2xl md:text-3xl font-bold text-main">
            Don't Miss Out
          </h2>
        </div>
        <router-link
          to="/"
          class="text-sm text-muted hover:text-primary transition-colors duration-200"
        >
          View all →
        </router-link>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <router-link
          v-for="(event, index) in featuredEvents"
          :key="event.id"
          :to="`/event/${event.id}`"
          class="glass-panel-hover group block overflow-hidden animate-fade-up opacity-0"
          :class="`stagger-${index + 2}`"
          :data-testid="`featured-event-card-${event.id}`"
        >
          <!-- Image placeholder -->
          <div
            class="h-48 flex items-center justify-center text-6xl transition-transform duration-500 group-hover:scale-105"
            :style="{ background: event.gradient }"
          >
            {{ event.emoji }}
          </div>

          <!-- Card body -->
          <div class="p-5">
            <div class="flex items-center gap-2 mb-2">
              <span class="text-xs font-mono text-primary bg-primary-dim px-2 py-0.5 rounded-md">
                {{ event.category }}
              </span>
              <span class="text-xs text-dimmed">
                {{ event.date }}
              </span>
            </div>

            <h3 class="text-lg font-heading font-semibold text-main group-hover:text-primary transition-colors duration-200 mb-1">
              {{ event.title }}
            </h3>

            <p class="text-sm text-muted mb-4 line-clamp-2">
              {{ event.location }}
            </p>

            <div class="flex items-center justify-between">
              <p class="text-sm text-dimmed">
                From
                <span class="text-primary font-semibold text-base">{{ event.price }}</span>
              </p>
              <BaseButton variant="outline" size="sm" data-testid="book-now-btn">
                Book Now
              </BaseButton>
            </div>
          </div>
        </router-link>
      </div>
    </section>

    <!-- ═══ CATEGORIES ═════════════════════════════════════════════════════ -->
    <section class="px-4 md:px-8 max-w-7xl mx-auto mt-24" data-testid="categories-section">
      <div class="text-center mb-10 animate-fade-up opacity-0 stagger-1">
        <p class="text-primary text-sm font-semibold tracking-wide uppercase mb-1">Explore</p>
        <h2 class="font-heading text-2xl md:text-3xl font-bold text-main">
          Browse by Category
        </h2>
      </div>

      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-4">
        <div
          v-for="(cat, index) in categories"
          :key="cat.name"
          class="glass-panel-hover p-5 flex flex-col items-center gap-3 cursor-pointer
                 group animate-fade-up opacity-0"
          :class="`stagger-${index + 2}`"
          :data-testid="`category-card-${cat.name.toLowerCase()}`"
        >
          <span class="text-4xl transition-transform duration-300 group-hover:scale-110">
            {{ cat.emoji }}
          </span>
          <span class="text-sm font-medium text-muted group-hover:text-main transition-colors duration-200">
            {{ cat.name }}
          </span>
        </div>
      </div>
    </section>

    <!-- ═══ STATS BAR ══════════════════════════════════════════════════════ -->
    <section class="px-4 md:px-8 max-w-5xl mx-auto mt-24 mb-20" data-testid="stats-section">
      <div class="glass-panel p-8 md:p-12 animate-fade-up opacity-0 stagger-1">
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-8 text-center">
          <div
            v-for="(stat, index) in stats"
            :key="stat.label"
            class="animate-fade-up opacity-0"
            :class="`stagger-${index + 2}`"
            :data-testid="`stat-${stat.label.toLowerCase().replace(/\s+/g, '-')}`"
          >
            <p class="text-3xl md:text-4xl font-heading font-bold text-gradient-primary mb-1">
              {{ stat.value }}
            </p>
            <p class="text-sm text-muted">
              {{ stat.label }}
            </p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
/**
 * HomePage — TicketHub landing page.
 *
 * Showcases featured events, browsable categories, and platform stats.
 * All data is mocked inline for now.
 */
import { ref } from 'vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import BaseInput from '@/shared/components/BaseInput.vue'

/* ── Decorative search model ──────────────────────────────────────────────── */
const heroSearch = ref('')

/* ── Featured Events (mock) ───────────────────────────────────────────────── */
const featuredEvents = ref([
  {
    id: 'evt-001',
    title: 'Sơn Tùng M-TP: Skyline Concert',
    date: 'Jun 15, 2026',
    location: 'National Stadium, Hà Nội',
    price: '850,000₫',
    category: 'Concert',
    emoji: '🎤',
    gradient: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
  },
  {
    id: 'evt-002',
    title: 'Vietnam vs Thailand — AFF Cup',
    date: 'Jul 08, 2026',
    location: 'Mỹ Đình Stadium, Hà Nội',
    price: '400,000₫',
    category: 'Sports',
    emoji: '⚽',
    gradient: 'linear-gradient(135deg, #0d1117 0%, #0a2e14 50%, #1a4024 100%)',
  },
  {
    id: 'evt-003',
    title: 'Digital Art Immersive Exhibition',
    date: 'Aug 22, 2026',
    location: 'Landmark 81, Hồ Chí Minh City',
    price: '250,000₫',
    category: 'Arts',
    emoji: '🎨',
    gradient: 'linear-gradient(135deg, #1a0a2e 0%, #2d1b4e 50%, #461e6e 100%)',
  },
])

/* ── Categories (mock) ────────────────────────────────────────────────────── */
const categories = ref([
  { name: 'Concerts', emoji: '🎵' },
  { name: 'Sports', emoji: '🏆' },
  { name: 'Arts', emoji: '🎨' },
  { name: 'Workshops', emoji: '🛠️' },
  { name: 'Experiences', emoji: '✨' },
  { name: 'Others', emoji: '🎉' },
])

/* ── Platform Stats (mock) ────────────────────────────────────────────────── */
const stats = ref([
  { value: '10,000+', label: 'Events' },
  { value: '500K+', label: 'Tickets Sold' },
  { value: '50K+', label: 'Happy Customers' },
])
</script>

<style scoped>
@reference "@/app.css";

.line-clamp-2 {
  @apply overflow-hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}
</style>
