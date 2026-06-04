<template>
  <div class="animate-fade-up">
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
      <div>
        <h1 class="text-2xl font-heading font-bold text-main mb-1">
          Organizer Dashboard
        </h1>
        <p class="text-muted text-sm">
          Overview of your event performance and sales.
        </p>
      </div>
      <BaseButton
        variant="primary"
        size="md"
        @click="$router.push('/organizer/create')"
        data-testid="btn-create-new-event"
      >
        <svg class="w-4 h-4 mr-1.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <circle cx="12" cy="12" r="10" />
          <line x1="12" y1="8" x2="12" y2="16" />
          <line x1="8" y1="12" x2="16" y2="12" />
        </svg>
        Create New Event
      </BaseButton>
    </div>

    <!-- Metrics Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
      <div
        v-for="(metric, index) in metrics"
        :key="metric.title"
        class="glass-panel p-5 flex flex-col gap-2 relative overflow-hidden"
        :class="`stagger-${index + 1}`"
      >
        <!-- Subtle background glow for the card based on accent color (simulated via CSS) -->
        <div class="absolute -right-6 -top-6 w-24 h-24 rounded-full opacity-10 blur-2xl" :class="metric.bgClass"></div>
        
        <div class="flex items-center gap-2.5 text-muted mb-1">
          <div class="p-1.5 rounded-lg bg-surface/50 border border-border-light/50">
             <component :is="metric.icon" class="w-4 h-4" :class="metric.iconClass" />
          </div>
          <span class="text-sm font-medium">{{ metric.title }}</span>
        </div>
        <div class="text-3xl font-heading font-bold text-main">
          {{ metric.value }}
        </div>
        <div class="flex items-center gap-1.5 text-xs">
          <span :class="metric.trend > 0 ? 'text-primary' : 'text-danger'" class="flex items-center font-medium">
            <svg v-if="metric.trend > 0" class="w-3 h-3 mr-0.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="23 6 13.5 15.5 8.5 10.5 1 18" /><polyline points="17 6 23 6 23 12" /></svg>
            <svg v-else class="w-3 h-3 mr-0.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="23 18 13.5 8.5 8.5 13.5 1 6" /><polyline points="17 18 23 18 23 12" /></svg>
            {{ Math.abs(metric.trend) }}%
          </span>
          <span class="text-dimmed">vs last month</span>
        </div>
      </div>
    </div>

    <!-- Recent Activity Placeholder -->
    <div class="glass-panel p-6 stagger-5">
      <h2 class="text-lg font-heading font-semibold text-main mb-4">Recent Activity</h2>
      <div class="flex flex-col items-center justify-center py-12 text-center border border-dashed border-border-main/50 rounded-xl bg-surface/30">
        <div class="w-12 h-12 rounded-full bg-border-main/20 flex items-center justify-center mb-3">
          <svg class="w-6 h-6 text-muted" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
            <polyline points="14 2 14 8 20 8"></polyline>
            <line x1="16" y1="13" x2="8" y2="13"></line>
            <line x1="16" y1="17" x2="8" y2="17"></line>
            <polyline points="10 9 9 9 8 9"></polyline>
          </svg>
        </div>
        <p class="text-main font-medium text-sm">No recent activity</p>
        <p class="text-muted text-xs mt-1 max-w-xs">When people buy tickets or interact with your events, it will show up here.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { h } from 'vue'
import BaseButton from '@/shared/components/BaseButton.vue'

// Icons
const IconDollar = (_, { attrs }) => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
  h('line', { x1: '12', y1: '1', x2: '12', y2: '23' }),
  h('path', { d: 'M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6' })
])

const IconTicket = (_, { attrs }) => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
  h('rect', { x: '2', y: '5', width: '20', height: '14', rx: '2' }),
  h('path', { d: 'M2 12h20' }),
  h('path', { d: 'M7 5v14' }),
  h('path', { d: 'M17 5v14' })
])

const IconCalendar = (_, { attrs }) => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
  h('rect', { x: '3', y: '4', width: '18', height: '18', rx: '2', ry: '2' }),
  h('line', { x1: '16', y1: '2', x2: '16', y2: '6' }),
  h('line', { x1: '8', y1: '2', x2: '8', y2: '6' }),
  h('line', { x1: '3', y1: '10', x2: '21', y2: '10' })
])

const IconUsers = (_, { attrs }) => h('svg', { viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'stroke-linecap': 'round', 'stroke-linejoin': 'round', ...attrs }, [
  h('path', { d: 'M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2' }),
  h('circle', { cx: '9', cy: '7', r: '4' }),
  h('path', { d: 'M23 21v-2a4 4 0 0 0-3-3.87' }),
  h('path', { d: 'M16 3.13a4 4 0 0 1 0 7.75' })
])

// Mock Data
const metrics = [
  { title: 'Total Revenue', value: '₫ 124.5M', trend: 12.5, icon: IconDollar, iconClass: 'text-primary', bgClass: 'bg-primary' },
  { title: 'Tickets Sold', value: '1,432', trend: 8.2, icon: IconTicket, iconClass: 'text-info', bgClass: 'bg-info' },
  { title: 'Active Events', value: '3', trend: 0, icon: IconCalendar, iconClass: 'text-accent', bgClass: 'bg-accent' },
  { title: 'Page Views', value: '12.4K', trend: -4.1, icon: IconUsers, iconClass: 'text-warning', bgClass: 'bg-warning' },
]
</script>
