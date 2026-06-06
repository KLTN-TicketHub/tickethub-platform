<template>
  <div class="flex flex-col gap-8 animate-in fade-in slide-in-from-bottom-4 duration-500 pb-12">
    <!-- Header -->
    <div>
      <h1 class="font-heading text-3xl font-bold text-main mb-2">Tổng quan kiểm duyệt</h1>
      <p class="text-muted font-medium italic">Chào mừng trở lại! Dưới đây là thống kê tình trạng phê duyệt và nội dung hệ thống hôm nay.</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      <div v-for="stat in mainStats" :key="stat.label" 
        class="bg-card border border-border-main p-6 rounded-[24px] flex items-center gap-5 group hover:border-primary/30 hover:shadow-2xl hover:shadow-primary/5 transition-all duration-300"
      >
        <div 
          class="w-14 h-14 rounded-2xl flex items-center justify-center text-2xl group-hover:scale-110 transition-transform duration-300 shadow-inner" 
          :style="{ backgroundColor: stat.bg, color: stat.color }"
        >
          <component :is="stat.icon" class="w-6 h-6" />
        </div>
        <div class="flex flex-col">
          <span class="text-[11px] font-bold text-muted uppercase tracking-widest mb-1">{{ stat.label }}</span>
          <span class="text-2xl font-bold text-main">{{ stat.value }}</span>
          <span class="text-[12px] text-muted font-medium mt-1">{{ stat.sub }}</span>
        </div>
      </div>
    </div>

    <!-- Dashboard Content -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Events Awaiting Approval -->
      <div class="lg:col-span-2 bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl flex flex-col">
        <div class="p-6 border-b border-border-main flex justify-between items-center bg-card/50">
          <h3 class="text-lg font-bold text-main">Sự kiện chờ duyệt</h3>
          <span class="text-warning text-[13px] font-bold">Cần xử lý gấp</span>
        </div>
        
        <BaseTable :columns="eventColumns" :data="pendingEvents" variant="ghost">
          <template #event="{ row }">
            <div class="flex items-center gap-3">
              <img :src="row.image" class="w-10 h-7 rounded-md object-cover border border-border-main shadow-sm" />
              <span class="font-bold text-main group-hover:text-primary transition-colors line-clamp-1">{{ row.title }}</span>
            </div>
          </template>
          
          <template #category="{ row }">
            <span class="text-[13px] text-muted font-medium capitalize">{{ row.category }}</span>
          </template>
          
          <template #date="{ row }">
            <span class="text-[13px] text-muted font-medium">{{ formatDate(row.dateStart) }}</span>
          </template>
          
          <template #action="{ row }">
            <div class="flex justify-end gap-2">
              <button @click="approveEvent(row.id)" class="px-2.5 py-1 text-[11px] font-bold rounded-lg bg-primary/10 text-primary hover:bg-primary hover:text-white transition-all cursor-pointer">Duyệt</button>
              <button @click="rejectEvent(row.id)" class="px-2.5 py-1 text-[11px] font-bold rounded-lg bg-danger/10 text-danger hover:bg-danger hover:text-white transition-all cursor-pointer">Từ chối</button>
            </div>
          </template>
        </BaseTable>
      </div>

      <!-- Categories Stats / Quick Actions -->
      <div class="flex flex-col gap-8">
        <!-- Event Categories List -->
        <div class="bg-card border border-border-main rounded-[32px] p-8 shadow-xl">
          <h3 class="text-lg font-bold text-main mb-6">Thể loại sự kiện</h3>
          <div class="flex flex-col gap-6">
            <div v-for="cat in categories" :key="cat.name" class="flex flex-col gap-2.5">
              <div class="flex justify-between items-center">
                <span class="text-[13px] font-bold text-main flex items-center gap-2">
                  <span class="w-6 h-6 flex items-center justify-center rounded-lg bg-surface border border-border-main text-[12px]">{{ cat.icon }}</span>
                  {{ cat.name }}
                </span>
                <span class="text-[12px] font-bold text-muted">{{ cat.count }} sự kiện</span>
              </div>
            </div>
          </div>
        </div>

        <!-- System health -->
        <div class="bg-card border border-border-main rounded-[32px] p-8 shadow-xl flex flex-col gap-4">
          <h3 class="text-lg font-bold text-main">Trạng thái công việc</h3>
          <div class="flex items-center justify-between p-4 bg-surface rounded-2xl border border-border-main/50">
            <div class="flex flex-col">
              <span class="text-[13px] font-bold text-main">Tỷ lệ duyệt sạch</span>
              <span class="text-[11px] text-muted">Không có khiếu nại</span>
            </div>
            <span class="text-lg font-bold text-primary">100%</span>
          </div>
          <div class="flex items-center justify-between p-4 bg-surface rounded-2xl border border-border-main/50">
            <div class="flex flex-col">
              <span class="text-[13px] font-bold text-main">Thời gian phản hồi</span>
              <span class="text-[11px] text-muted">Trung bình xử lý</span>
            </div>
            <span class="text-lg font-bold text-indigo-400">&lt; 15 phút</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, h, ref } from 'vue'
import { getEvents } from '../../stores/eventStore'
import BaseTable from '../../components/ui/BaseTable.vue'

// Local list of events (since we want approval actions to change state reactively)
const allEvents = ref(getEvents())

const eventColumns = [
  { key: 'event', label: 'Sự kiện' },
  { key: 'category', label: 'Thể loại' },
  { key: 'date', label: 'Thời gian' },
  { key: 'action', label: 'Thao tác', class: 'text-right' },
]

// Icons
const EventIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('rect', { x: '3', y: '4', width: '18', height: '18', rx: '2' }), h('line', { x1: '16', y1: '2', x2: '16', y2: '6' }), h('line', { x1: '8', y1: '2', x2: '8', y2: '6' }), h('line', { x1: '3', y1: '10', x2: '21', y2: '10' })])
const CheckedIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('path', { d: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z' })])
const ClockIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('circle', { cx: '12', cy: '12', r: '10' }), h('polyline', { points: '12 6 12 12 16 14' })])
const TagIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', strokeWidth: '2', viewBox: '0 0 24 24' }, [h('path', { d: 'M20.59 13.41l-7.17 7.17a2 2 0 0 1-2.83 0L2 12V2h10l8.59 8.59a2 2 0 0 1 0 2.82z' }), h('line', { x1: '7', y1: '7', x2: '7.01', y2: '7' })])

const mainStats = computed(() => {
  const events = allEvents.value
  // Simulate some pending/approved based on status/dates
  const pending = events.filter(e => e.status === 'upcoming').slice(0, 3)
  const approved = events.length - pending.length

  return [
    { 
      label: 'Tổng sự kiện', 
      value: events.length, 
      sub: 'Tất cả trong hệ thống',
      icon: EventIcon,
      bg: 'rgba(99, 102, 241, 0.1)',
      color: '#818cf8'
    },
    { 
      label: 'Đã phê duyệt', 
      value: approved, 
      sub: 'Đang mở bán / Hiển thị',
      icon: CheckedIcon,
      bg: 'var(--color-primary-dim)',
      color: 'var(--color-primary)'
    },
    { 
      label: 'Chờ kiểm duyệt', 
      value: pending.length, 
      sub: 'Yêu cầu chờ xử lý',
      icon: ClockIcon,
      bg: 'var(--color-warning-dim)',
      color: 'var(--color-warning)'
    },
    { 
      label: 'Thể loại', 
      value: 5, 
      sub: 'Đang hoạt động',
      icon: TagIcon,
      bg: 'rgba(236, 72, 153, 0.1)',
      color: '#f472b6'
    }
  ]
})

const pendingEvents = computed(() => {
  // Let's filter some events that are upcoming to simulate pending moderation list
  return allEvents.value.filter(e => e.status === 'upcoming').slice(0, 4)
})

const categories = computed(() => {
  const cats = { concerts: 0, sports: 0, arts: 0, experiences: 0, workshops: 0 }
  const labels = { concerts: 'Concert', sports: 'Thể thao', arts: 'Sân khấu', experiences: 'Trải nghiệm', workshops: 'Workshop' }
  const icons = { concerts: '🎵', sports: '⚽', arts: '🎭', experiences: '🧭', workshops: '📚' }
  
  allEvents.value.forEach(e => {
    if (cats[e.category] !== undefined) cats[e.category]++
  })

  return Object.entries(cats).map(([key, val]) => ({
    name: labels[key],
    icon: icons[key],
    count: val
  }))
})

const approveEvent = (id) => {
  // Remove event from view or change status to simulate approval
  allEvents.value = allEvents.value.filter(e => e.id !== id)
}

const rejectEvent = (id) => {
  // Remove event from view
  allEvents.value = allEvents.value.filter(e => e.id !== id)
}

const formatDate = (d) => {
  if (!d) return 'TBA'
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
</script>
