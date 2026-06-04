<template>
  <div class="event-detail-page" data-testid="event-detail-page">
    <!-- Hero Section -->
    <section class="hero-section" data-testid="event-hero">
      <div class="hero-gradient" />
      <div class="hero-content animate-fade-up">
        <div class="hero-emoji-wrap">
          <span class="hero-emoji">{{ mockEvent.emoji }}</span>
        </div>
        <div class="hero-category-badge stagger-1">
          {{ mockEvent.category }}
        </div>
        <h1
          class="hero-title font-heading stagger-2"
          data-testid="event-title"
        >
          {{ mockEvent.title }}
        </h1>
        <div class="hero-badges stagger-3">
          <span class="hero-badge" data-testid="event-date">
            <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            {{ mockEvent.date }}
          </span>
          <span class="hero-badge" data-testid="event-location">
            <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
            {{ mockEvent.location }}
          </span>
          <span class="hero-badge">
            <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            {{ mockEvent.time }}
          </span>
        </div>
      </div>
    </section>

    <!-- Main Content -->
    <div class="event-content">
      <!-- Description -->
      <section class="content-section animate-fade-up stagger-4">
        <div class="glass-panel section-panel" data-testid="event-description">
          <h2 class="section-heading font-heading">About This Event</h2>
          <p class="section-text">{{ mockEvent.description }}</p>
          <div class="event-details-grid">
            <div class="detail-item">
              <span class="detail-label">Organizer</span>
              <span class="detail-value">{{ mockEvent.organizer }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Duration</span>
              <span class="detail-value">{{ mockEvent.duration }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Category</span>
              <span class="detail-value">{{ mockEvent.category }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">Age Restriction</span>
              <span class="detail-value">{{ mockEvent.ageRestriction }}</span>
            </div>
          </div>
        </div>
      </section>

      <!-- Ticket Tiers -->
      <section class="content-section animate-fade-up stagger-5">
        <h2 class="section-heading font-heading" data-testid="tiers-heading">
          <svg class="w-5 h-5 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z" />
          </svg>
          Ticket Tiers
        </h2>
        <div class="tiers-grid">
          <div
            v-for="(tier, index) in mockEvent.tiers"
            :key="tier.id"
            class="tier-card glass-panel-hover animate-fade-up"
            :class="`stagger-${index + 5}`"
            :data-testid="`tier-card-${tier.id}`"
          >
            <div class="tier-accent" :style="{ background: tier.color }" />
            <div class="tier-body">
              <div class="tier-header">
                <h3 class="tier-name font-heading">{{ tier.name }}</h3>
                <span v-if="tier.badge" class="tier-badge" :style="{ color: tier.color, borderColor: tier.color + '40' }">
                  {{ tier.badge }}
                </span>
              </div>
              <p class="tier-desc">{{ tier.description }}</p>
              <div class="tier-footer">
                <div class="tier-price-block">
                  <span class="tier-currency">$</span>
                  <span class="tier-price font-heading">{{ tier.price }}</span>
                  <span class="tier-per">/seat</span>
                </div>
                <div class="tier-availability">
                  <div class="avail-dot" :class="tier.available > 20 ? 'avail-good' : 'avail-low'" />
                  <span class="avail-text">{{ tier.available }} left</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Book Now CTA -->
      <section class="cta-section animate-fade-up stagger-7" data-testid="event-cta">
        <div class="glass-panel cta-panel">
          <div class="cta-content">
            <div class="cta-text">
              <h3 class="cta-heading font-heading">Ready to Secure Your Spot?</h3>
              <p class="cta-subtext">Choose your preferred seats and complete your booking in seconds.</p>
            </div>
            <BaseButton
              variant="primary"
              size="lg"
              data-testid="book-now-button"
              @click="showBooking = true"
            >
              <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z" />
              </svg>
              Book Now
            </BaseButton>
          </div>
        </div>
      </section>
    </div>

    <!-- Booking Modal -->
    <BookingModal
      :visible="showBooking"
      :event="mockEvent"
      @close="showBooking = false"
      @success="handleBookingSuccess"
    />
  </div>
</template>

<script setup>
/**
 * EventDetailPage — Public event detail view.
 *
 * Route: /event/:id
 *
 * Renders hero section, description, ticket tiers, and a CTA
 * that opens the BookingModal for seat selection and checkout.
 */
import { ref } from 'vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import BookingModal from '@/features/booking/components/BookingModal.vue'
import { useToast } from '@/shared/composables/useToast'

const toast = useToast()

/* ── Booking Modal State ──────────────────────────────────────────────────── */
const showBooking = ref(false)

function handleBookingSuccess(booking) {
  showBooking.value = false
  toast.success(`🎉 You're all set! ${booking.seats.length} seat(s) booked for $${booking.total.toFixed(2)}`)
}

/* ── Mock Event Data ──────────────────────────────────────────────────────── */
const mockEvent = {
  id: 'evt-2026-aurora',
  title: 'Aurora Music Festival 2026',
  emoji: '🎶',
  category: 'Music Festival',
  date: 'Saturday, June 14, 2026',
  time: '6:00 PM — 11:30 PM',
  location: 'Starlight Arena, Ho Chi Minh City',
  organizer: 'Nebula Events Co.',
  duration: '5 hours 30 minutes',
  ageRestriction: 'All Ages',
  description:
    'Experience the biggest music festival of the year featuring world-class DJs, immersive light shows, and an unforgettable atmosphere. Aurora 2026 brings together over 30 artists across 3 stages, with state-of-the-art sound systems and holographic visual performances. From electronic beats to indie rock, there\'s something for every music lover. Food trucks, art installations, and VIP lounges complete the ultimate festival experience.',
  tiers: [
    {
      id: 'tier-vip',
      name: 'VIP',
      price: 250,
      color: '#FFD700',
      badge: '⭐ Premium',
      description: 'Front-row seats with complimentary drinks, backstage access, and exclusive merch.',
      available: 12,
    },
    {
      id: 'tier-premium',
      name: 'Premium',
      price: 150,
      color: '#B9FF6A',
      badge: 'Best Value',
      description: 'Great visibility with comfortable seating and priority entry to the venue.',
      available: 48,
    },
    {
      id: 'tier-general',
      name: 'General',
      price: 75,
      color: '#3B82F6',
      badge: null,
      description: 'Standard admission with full access to all stages and common areas.',
      available: 156,
    },
  ],
}
</script>

<style scoped>
@reference "@/app.css";

/* ── Page Layout ───────────────────────────────────────────────────────────── */
.event-detail-page {
  @apply min-h-screen;
}

/* ── Hero Section ──────────────────────────────────────────────────────────── */
.hero-section {
  @apply relative flex items-center justify-center text-center overflow-hidden;
  min-height: 400px;
  padding: 80px 24px 48px;
}

.hero-gradient {
  @apply absolute inset-0;
  background:
    radial-gradient(ellipse 60% 50% at 50% 30%, rgba(0, 200, 83, 0.12) 0%, transparent 70%),
    radial-gradient(ellipse 40% 40% at 70% 60%, rgba(185, 255, 106, 0.06) 0%, transparent 60%),
    linear-gradient(180deg, var(--color-bg) 0%, var(--color-surface) 100%);
}

.hero-content {
  @apply relative z-10 flex flex-col items-center gap-4 max-w-2xl;
}

.hero-emoji-wrap {
  @apply flex items-center justify-center w-20 h-20 rounded-2xl mb-2;
  background: rgba(0, 200, 83, 0.08);
  border: 1px solid rgba(0, 200, 83, 0.15);
  box-shadow: 0 0 40px rgba(0, 200, 83, 0.1);
}

.hero-emoji {
  font-size: 40px;
  line-height: 1;
}

.hero-category-badge {
  @apply inline-flex items-center px-3 py-1 text-xs font-medium font-mono uppercase tracking-wider rounded-full;
  @apply text-primary;
  background: var(--color-primary-dim);
  border: 1px solid rgba(0, 200, 83, 0.2);
}

.hero-title {
  @apply text-3xl sm:text-4xl lg:text-5xl font-bold text-main leading-tight;
}

.hero-badges {
  @apply flex flex-wrap items-center justify-center gap-3 mt-2;
}

.hero-badge {
  @apply inline-flex items-center gap-1.5 px-3 py-1.5 text-xs text-muted rounded-lg;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.06);
}

/* ── Main Content ──────────────────────────────────────────────────────────── */
.event-content {
  @apply max-w-4xl mx-auto px-4 sm:px-6 pb-24 space-y-8;
  margin-top: -8px;
}

.content-section {
  @apply opacity-0;
}

.section-panel {
  @apply p-6;
}

.section-heading {
  @apply flex items-center gap-2 text-lg font-semibold text-main mb-4 tracking-wide;
}

.section-text {
  @apply text-sm leading-relaxed text-muted mb-5;
}

/* ── Event Details Grid ────────────────────────────────────────────────────── */
.event-details-grid {
  @apply grid grid-cols-2 sm:grid-cols-4 gap-4;
}

.detail-item {
  @apply flex flex-col gap-1 p-3 rounded-xl;
  background: rgba(255, 255, 255, 0.02);
  border: 1px solid rgba(255, 255, 255, 0.04);
}

.detail-label {
  @apply text-[10px] font-mono uppercase tracking-wider text-dimmed;
}

.detail-value {
  @apply text-xs font-medium text-main;
}

/* ── Ticket Tiers ──────────────────────────────────────────────────────────── */
.tiers-grid {
  @apply grid grid-cols-1 sm:grid-cols-3 gap-4 mt-4;
}

.tier-card {
  @apply relative overflow-hidden flex flex-col opacity-0;
}

.tier-accent {
  @apply h-1 w-full;
}

.tier-body {
  @apply flex flex-col gap-3 p-5 flex-1;
}

.tier-header {
  @apply flex items-start justify-between gap-2;
}

.tier-name {
  @apply text-base font-semibold text-main;
}

.tier-badge {
  @apply shrink-0 inline-flex items-center px-2 py-0.5 text-[10px] font-semibold uppercase tracking-wider rounded-md;
  border: 1px solid;
  background: rgba(255, 255, 255, 0.02);
}

.tier-desc {
  @apply text-xs text-muted leading-relaxed flex-1;
}

.tier-footer {
  @apply flex items-end justify-between gap-3 pt-2;
  border-top: 1px solid rgba(255, 255, 255, 0.04);
}

.tier-price-block {
  @apply flex items-baseline gap-0.5;
}

.tier-currency {
  @apply text-sm text-muted font-mono;
}

.tier-price {
  @apply text-2xl font-bold text-gradient-primary;
}

.tier-per {
  @apply text-[10px] text-dimmed font-mono ml-0.5;
}

.tier-availability {
  @apply flex items-center gap-1.5;
}

.avail-dot {
  @apply w-1.5 h-1.5 rounded-full;
}

.avail-good {
  @apply bg-primary;
  box-shadow: 0 0 6px rgba(0, 200, 83, 0.4);
}

.avail-low {
  @apply bg-warning;
  box-shadow: 0 0 6px rgba(250, 173, 20, 0.4);
}

.avail-text {
  @apply text-[10px] text-muted font-mono;
}

/* ── CTA Section ───────────────────────────────────────────────────────────── */
.cta-section {
  @apply opacity-0;
}

.cta-panel {
  @apply p-6;
  background: linear-gradient(
    135deg,
    rgba(0, 200, 83, 0.04) 0%,
    rgba(20, 27, 22, 0.65) 50%,
    rgba(185, 255, 106, 0.03) 100%
  );
}

.cta-content {
  @apply flex flex-col sm:flex-row items-center justify-between gap-5;
}

.cta-text {
  @apply text-center sm:text-left;
}

.cta-heading {
  @apply text-lg font-semibold text-main mb-1;
}

.cta-subtext {
  @apply text-sm text-muted;
}
</style>
