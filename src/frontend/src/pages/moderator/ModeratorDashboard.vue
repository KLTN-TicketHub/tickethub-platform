<template>
  <div class="flex flex-col gap-10 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Kiểm duyệt</h1>
      <p class="text-white/50 font-medium text-lg max-w-2xl">Quản lý và phê duyệt các sự kiện mới nhất từ ban tổ chức.</p>
    </div>

    <!-- Stats Bento Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div v-for="(stat, idx) in mainStats" :key="stat.label" 
        class="relative overflow-hidden rounded-[2rem] p-8 flex flex-col gap-6 group transition-transform duration-500 hover:-translate-y-1"
        :class="stat.bgClass"
      >
        <div class="flex justify-between items-start relative z-10">
          <div class="w-12 h-12 rounded-full flex items-center justify-center text-xl shadow-lg" :class="stat.iconBgClass">
            <component :is="stat.icon" weight="fill" :class="stat.iconColorClass" />
          </div>
          <span class="text-[10px] font-bold uppercase tracking-[0.2em]" :class="stat.iconColorClass">{{ stat.sub }}</span>
        </div>
        <div class="flex flex-col relative z-10 mt-4">
          <span class="text-[13px] font-bold text-white/50 mb-1">{{ stat.label }}</span>
          <span class="text-4xl font-heading font-black text-white tracking-tight">{{ stat.value }}</span>
        </div>
        <div class="absolute -bottom-10 -right-10 w-40 h-40 rounded-full blur-[50px] opacity-20 pointer-events-none transition-opacity duration-500 group-hover:opacity-40" :class="stat.glowClass"></div>
      </div>
    </div>

    <!-- Main Content Area -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
      
      <!-- Events Awaiting Approval (Takes 8 cols) -->
      <div class="lg:col-span-8 flex flex-col gap-6">
        <div class="flex justify-between items-end px-2">
          <h3 class="text-2xl font-bold font-heading text-white">Sự kiện chờ duyệt</h3>
          <span class="text-warning text-[13px] font-bold flex items-center gap-2">
            <PhWarningCircle weight="fill" /> Cần xử lý
          </span>
        </div>
        <BaseTable :columns="eventColumns" :data="pendingEvents">
          <template #event="{ row }">
            <div class="flex items-center gap-4">
              <div class="w-12 h-12 rounded-xl overflow-hidden border border-white/10 flex-shrink-0">
                <img :src="row.image" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              </div>
              <span class="font-bold text-white group-hover:text-primary transition-colors line-clamp-1">{{ row.title }}</span>
            </div>
          </template>
          <template #category="{ row }">
            <span class="text-[13px] text-white/60 font-bold uppercase tracking-wider">{{ row.category }}</span>
          </template>
          <template #date="{ row }">
            <span class="text-[14px] text-white/80 font-medium">{{ formatDate(row.dateStart) }}</span>
          </template>
          <template #action="{ row }">
            <div class="flex justify-end gap-2">
              <button @click="approveEvent(row.id)" class="px-3 py-1.5 text-[11px] font-bold rounded-lg bg-primary/10 text-primary hover:bg-primary hover:text-black transition-all">Duyệt</button>
              <button @click="rejectEvent(row.id)" class="px-3 py-1.5 text-[11px] font-bold rounded-lg bg-danger/10 text-danger hover:bg-danger hover:text-white transition-all">Từ chối</button>
            </div>
          </template>
        </BaseTable>
      </div>

      <!-- Right Column (Takes 4 cols) -->
      <div class="lg:col-span-4 flex flex-col gap-8">
        
        <!-- Categories Stats -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-8 flex flex-col gap-6">
          <h3 class="text-xl font-bold font-heading text-white">Phân bổ thể loại</h3>
          <div class="flex flex-col gap-5">
            <div v-for="cat in categories" :key="cat.name" class="flex justify-between items-center group">
              <span class="text-[14px] font-bold text-white/80 flex items-center gap-3">
                <component :is="cat.icon" weight="fill" class="text-white/40 group-hover:text-primary transition-colors" />
                {{ cat.name }}
              </span>
              <span class="text-[13px] font-bold text-white px-3 py-1 bg-white/5 rounded-full">{{ cat.count }}</span>
            </div>
          </div>
        </div>

        <!-- System health -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-8 flex flex-col gap-5">
          <h3 class="text-xl font-bold font-heading text-white">Trạng thái công việc</h3>
          
          <div class="flex items-center justify-between p-4 bg-white/5 rounded-2xl border border-white/10">
            <div class="flex flex-col">
              <span class="text-[13px] font-bold text-white">Tỷ lệ duyệt sạch</span>
              <span class="text-[11px] text-white/40">Không khiếu nại</span>
            </div>
            <span class="text-lg font-bold text-primary">100%</span>
          </div>

          <div class="flex items-center justify-between p-4 bg-white/5 rounded-2xl border border-white/10">
            <div class="flex flex-col">
              <span class="text-[13px] font-bold text-white">Phản hồi</span>
              <span class="text-[11px] text-white/40">Trung bình xử lý</span>
            </div>
            <span class="text-lg font-bold text-[#818cf8]">&lt; 15 phút</span>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { getEvents } from '../../stores/eventStore'
import BaseTable from '../../components/ui/BaseTable.vue'
import { 
  PhTicket, PhCheckCircle, PhClock, PhTag, PhWarningCircle,
  PhMicrophoneStage, PhTrophy, PhMaskHappy, PhCompass, PhBooks
} from '@phosphor-icons/vue'

const allEvents = ref(getEvents())

const eventColumns = [
  { key: 'event', label: 'Sự kiện' },
  { key: 'category', label: 'Thể loại' },
  { key: 'date', label: 'Thời gian' },
  { key: 'action', label: 'Thao tác', class: 'text-right' },
]

const mainStats = computed(() => {
  const events = allEvents.value
  const pending = events.filter(e => e.status === 'upcoming').slice(0, 3)
  const approved = events.length - pending.length

  return [
    { 
      label: 'Tổng sự kiện', 
      value: events.length, 
      sub: 'Hệ thống',
      icon: PhTicket,
      bgClass: 'bg-[#111916] border border-white/5',
      iconBgClass: 'bg-[#818cf8]/10',
      iconColorClass: 'text-[#818cf8]',
      glowClass: 'bg-[#818cf8]'
    },
    { 
      label: 'Đã phê duyệt', 
      value: approved, 
      sub: 'Đang hiển thị',
      icon: PhCheckCircle,
      bgClass: 'bg-[#111916] border border-white/5',
      iconBgClass: 'bg-primary/10',
      iconColorClass: 'text-primary',
      glowClass: 'bg-primary'
    },
    { 
      label: 'Chờ duyệt', 
      value: pending.length, 
      sub: 'Cần xử lý',
      icon: PhClock,
      bgClass: 'bg-[#111916] border border-white/5',
      iconBgClass: 'bg-warning/10',
      iconColorClass: 'text-warning',
      glowClass: 'bg-warning'
    },
    { 
      label: 'Thể loại', 
      value: 5, 
      sub: 'Đang mở',
      icon: PhTag,
      bgClass: 'bg-[#111916] border border-white/5',
      iconBgClass: 'bg-[#f472b6]/10',
      iconColorClass: 'text-[#f472b6]',
      glowClass: 'bg-[#f472b6]'
    }
  ]
})

const pendingEvents = computed(() => allEvents.value.filter(e => e.status === 'upcoming').slice(0, 4))

const categories = computed(() => {
  const cats = { concerts: 0, sports: 0, arts: 0, experiences: 0, workshops: 0 }
  const labels = { concerts: 'Concert', sports: 'Thể thao', arts: 'Sân khấu', experiences: 'Trải nghiệm', workshops: 'Workshop' }
  const icons = { concerts: PhMicrophoneStage, sports: PhTrophy, arts: PhMaskHappy, experiences: PhCompass, workshops: PhBooks }
  
  allEvents.value.forEach(e => {
    if (cats[e.category] !== undefined) cats[e.category]++
  })

  return Object.entries(cats).map(([key, val]) => ({
    name: labels[key],
    icon: icons[key],
    count: val
  }))
})

const approveEvent = (id) => { allEvents.value = allEvents.value.filter(e => e.id !== id) }
const rejectEvent = (id) => { allEvents.value = allEvents.value.filter(e => e.id !== id) }

const formatDate = (d) => {
  if (!d) return 'TBA'
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
</script>
