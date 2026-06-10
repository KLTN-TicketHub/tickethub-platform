<template>
  <div class="max-w-[1400px] mx-auto py-12 px-6 lg:px-10 min-h-[80vh]">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-8 mb-12 animate-fade-up">
      <div class="space-y-4">
        <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-black tracking-widest uppercase shadow-[0_0_20px_rgba(0,200,83,0.15)]">
          <PhSuitcase weight="fill" /> Organizer Dashboard
        </div>
        <h1 class="font-heading text-4xl lg:text-5xl font-black text-white tracking-tight uppercase">Trung tâm Tổ chức</h1>
        <p class="text-white/50 font-medium text-lg max-w-xl">Quản lý các sự kiện và theo dõi hiệu suất bán vé của bạn một cách trực quan nhất.</p>
      </div>
      <router-link to="/create-event">
        <BaseButton variant="primary" size="lg" class="!px-8 !rounded-2xl shadow-[0_0_30px_rgba(0,200,83,0.3)] hover:scale-105 transition-transform flex items-center gap-2">
          <PhPlus weight="bold" /> Tạo sự kiện mới
        </BaseButton>
      </router-link>
    </div>

    <!-- Stats Grid (Bento) -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-16 animate-fade-up [animation-delay:100ms]">
      <div v-for="(stat, idx) in stats" :key="stat.label" class="bg-[#111916] border border-white/5 p-8 rounded-[2.5rem] hover:border-primary/30 transition-all group shadow-2xl relative overflow-hidden">
        <div class="absolute -right-8 -top-8 w-24 h-24 bg-white/5 rounded-full blur-2xl group-hover:bg-primary/10 transition-colors"></div>
        <div class="flex items-center justify-between mb-8 relative z-10">
          <div class="w-14 h-14 rounded-[1.25rem] bg-[#0A0F0D] border border-white/10 flex items-center justify-center text-3xl group-hover:scale-110 transition-transform shadow-inner" :class="stat.color">
            <component :is="stat.icon" weight="duotone" />
          </div>
          <span class="text-[12px] font-black bg-primary/10 border border-primary/20 text-primary px-3 py-1.5 rounded-xl flex items-center gap-1 shadow-sm">
            <PhTrendUp weight="bold" /> {{ stat.trend }}%
          </span>
        </div>
        <div class="relative z-10">
          <div class="text-white/40 text-[12px] font-bold uppercase tracking-widest mb-1.5">{{ stat.label }}</div>
          <div class="text-4xl lg:text-5xl font-heading font-black text-white tracking-tighter">{{ stat.value }}</div>
        </div>
      </div>
    </div>

    <!-- Events Table Section -->
    <div class="bg-[#111916] border border-white/5 rounded-[3rem] overflow-hidden shadow-2xl animate-fade-up [animation-delay:200ms]">
      <div class="p-8 lg:p-10 border-b border-white/5 flex flex-col sm:flex-row sm:items-center justify-between gap-6">
        <h2 class="font-heading text-2xl font-black text-white flex items-center gap-3">
          <PhCalendarCheck weight="fill" class="text-primary" /> Sự kiện của tôi
        </h2>
        <div class="flex gap-3 bg-[#0A0F0D] p-1.5 rounded-2xl border border-white/5">
          <button class="px-5 py-2.5 rounded-xl bg-white/10 text-white font-bold text-[13px] shadow-sm transition-colors">Tất cả</button>
          <button class="px-5 py-2.5 rounded-xl hover:bg-white/5 text-white/50 hover:text-white font-bold text-[13px] transition-colors">Đang diễn ra</button>
        </div>
      </div>

      <div class="p-6">
        <BaseTable :columns="columns" :data="myEvents">
          <template #image="{ row }">
            <div class="w-16 h-16 rounded-[1rem] overflow-hidden border border-white/10 shadow-md">
              <img :src="row.image" class="w-full h-full object-cover grayscale-[0.5] hover:grayscale-0 transition-all" />
            </div>
          </template>

          <template #title="{ row }">
            <div class="flex flex-col justify-center">
              <span class="font-bold text-[15px] text-white">{{ row.title }}</span>
              <span class="text-[12px] text-white/40 font-medium uppercase tracking-wider mt-1">{{ row.category }}</span>
            </div>
          </template>

          <template #date="{ row }">
            <div class="flex items-center gap-2 text-white/70 font-medium">
              <PhCalendarBlank class="text-white/30" />
              {{ formatDate(row.dateStart || row.date) }}
            </div>
          </template>

          <template #tickets="{ row }">
            <div class="flex flex-col gap-2 min-w-[120px]">
              <div class="w-full h-2 bg-[#0A0F0D] border border-white/5 rounded-full overflow-hidden shadow-inner">
                <div class="h-full bg-gradient-to-r from-primary to-[#00A355]" :style="{ width: (row.sold || 45) + '%' }"></div>
              </div>
              <div class="flex justify-between items-center text-[11px] font-bold uppercase tracking-widest text-white/50">
                <span>Đã bán: <span class="text-white">{{ row.sold || 45 }}</span></span>
                <span>/100</span>
              </div>
            </div>
          </template>

          <template #status="{ row }">
            <BaseBadge :variant="row.status === 'upcoming' ? 'primary' : 'secondary'" size="sm" class="uppercase tracking-widest">
              {{ row.status === 'upcoming' ? 'Sắp diễn ra' : 'Hoàn thành' }}
            </BaseBadge>
          </template>

          <template #actions="{ row }">
            <div class="flex items-center justify-end gap-2">
              <button class="w-10 h-10 rounded-xl bg-white/5 hover:bg-white/10 border border-white/5 flex items-center justify-center text-white/50 hover:text-white transition-colors cursor-pointer">
                <PhPencilSimple weight="bold" />
              </button>
              <button class="w-10 h-10 rounded-xl bg-white/5 hover:bg-danger/10 border border-white/5 hover:border-danger/20 flex items-center justify-center text-white/50 hover:text-danger transition-colors cursor-pointer">
                <PhTrash weight="bold" />
              </button>
            </div>
          </template>
        </BaseTable>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, markRaw } from 'vue'
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseTable from '../components/ui/BaseTable.vue'
import BaseBadge from '../components/ui/BaseBadge.vue'
import { 
  PhSuitcase, PhPlus, PhTicket, PhTrendUp, PhCurrencyCircleDollar, 
  PhEye, PhCalendarCheck, PhCalendarBlank, PhPencilSimple, PhTrash 
} from '@phosphor-icons/vue'

const stats = [
  { label: 'Tổng sự kiện', value: '12', icon: markRaw(PhTicket), trend: '12', color: 'text-primary' },
  { label: 'Vé đã bán', value: '450', icon: markRaw(PhTrendUp), trend: '8', color: 'text-info' },
  { label: 'Doanh thu', value: '67.5M', icon: markRaw(PhCurrencyCircleDollar), trend: '15', color: 'text-warning' },
  { label: 'Lượt xem', value: '2.4K', icon: markRaw(PhEye), trend: '24', color: 'text-[#f43f5e]' },
]

const columns = [
  { key: 'image', label: '', class: 'w-20' },
  { key: 'title', label: 'Tên sự kiện' },
  { key: 'date', label: 'Thời gian' },
  { key: 'tickets', label: 'Bán vé', class: 'hidden md:table-cell' },
  { key: 'status', label: 'Trạng thái' },
  { key: 'actions', label: '', class: 'text-right w-24' },
]

const myEvents = computed(() => {
  return store.events.slice(0, 5).map(e => ({
    ...e,
    status: Math.random() > 0.3 ? 'upcoming' : 'completed',
    sold: Math.floor(Math.random() * 80) + 10
  }))
})

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  const d = new Date(dateStr)
  return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
</script>
