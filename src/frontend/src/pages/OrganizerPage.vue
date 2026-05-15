<template>
  <div class="max-w-[1200px] mx-auto py-10 px-4 min-h-[80vh]">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-10">
      <div>
        <h1 class="font-heading text-4xl font-bold text-main mb-2">Trung tâm Tổ chức</h1>
        <p class="text-muted font-medium">Quản lý các sự kiện và theo dõi hiệu suất bán vé của bạn</p>
      </div>
      <router-link to="/create-event">
        <BaseButton variant="primary" size="lg" class="!px-8 !rounded-2xl shadow-xl shadow-primary/20">
          <span class="mr-2">+</span> Tạo sự kiện mới
        </BaseButton>
      </router-link>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
      <div v-for="stat in stats" :key="stat.label" class="bg-card border border-border-main p-6 rounded-[24px] hover:border-primary/30 transition-all group">
        <div class="flex items-center justify-between mb-4">
          <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center text-2xl group-hover:scale-110 transition-transform">
            {{ stat.icon }}
          </div>
          <span class="text-[12px] font-bold text-primary bg-primary/5 px-2 py-1 rounded-lg">+{{ stat.trend }}%</span>
        </div>
        <div class="text-muted text-[13px] font-bold uppercase tracking-wider mb-1">{{ stat.label }}</div>
        <div class="text-3xl font-heading font-bold text-main">{{ stat.value }}</div>
      </div>
    </div>

    <!-- Events Table Section -->
    <div class="bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl">
      <div class="p-8 border-b border-border-main flex items-center justify-between">
        <h2 class="font-heading text-xl font-bold text-main">Sự kiện của tôi</h2>
        <div class="flex gap-2">
          <BaseButton variant="outline" size="sm" class="!rounded-xl">Tất cả</BaseButton>
          <BaseButton variant="ghost" size="sm" class="!rounded-xl">Đang diễn ra</BaseButton>
        </div>
      </div>

      <div class="p-4">
        <BaseTable :columns="columns" :data="myEvents">
          <template #image="{ row }">
            <img :src="row.image" class="w-12 h-12 rounded-lg object-cover border border-border-main" />
          </template>

          <template #title="{ row }">
            <div class="flex flex-col">
              <span class="font-bold text-main">{{ row.title }}</span>
              <span class="text-[12px] text-muted capitalize">{{ row.category }}</span>
            </div>
          </template>

          <template #date="{ row }">
            <div class="text-muted text-[13px]">
              {{ formatDate(row.dateStart || row.date) }}
            </div>
          </template>

          <template #tickets="{ row }">
            <div class="flex flex-col gap-1">
              <div class="w-24 h-1.5 bg-surface rounded-full overflow-hidden">
                <div class="h-full bg-primary" :style="{ width: (row.sold || 45) + '%' }"></div>
              </div>
              <span class="text-[11px] font-bold text-muted">{{ row.sold || 45 }}/100 vé</span>
            </div>
          </template>

          <template #status="{ row }">
            <BaseBadge :variant="row.status === 'upcoming' ? 'primary' : 'secondary'" size="sm">
              {{ row.status === 'upcoming' ? 'Sắp diễn ra' : 'Hoàn thành' }}
            </BaseBadge>
          </template>

          <template #actions="{ row }">
            <div class="flex gap-2">
              <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-primary">
                ✏️
              </BaseButton>
              <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-danger">
                🗑️
              </BaseButton>
            </div>
          </template>
        </BaseTable>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseTable from '../components/ui/BaseTable.vue'
import BaseBadge from '../components/ui/BaseBadge.vue'

const stats = [
  { label: 'Tổng sự kiện', value: '12', icon: '🎫', trend: '12' },
  { label: 'Vé đã bán', value: '450', icon: '📈', trend: '8' },
  { label: 'Doanh thu', value: '67.5M', icon: '💰', trend: '15' },
  { label: 'Lượt xem', value: '2.4K', icon: '👁️', trend: '24' },
]

const columns = [
  { key: 'image', label: '', class: 'w-16' },
  { key: 'title', label: 'Tên sự kiện' },
  { key: 'date', label: 'Thời gian' },
  { key: 'tickets', label: 'Bán vé' },
  { key: 'status', label: 'Trạng thái' },
  { key: 'actions', label: '', class: 'text-right' },
]

const myEvents = computed(() => {
  // Show first 5 events as "my events" for demo
  return store.events.slice(0, 5).map(e => ({
    ...e,
    status: Math.random() > 0.3 ? 'upcoming' : 'completed',
    sold: Math.floor(Math.random() * 80) + 10
  }))
})

const formatDate = (dateStr) => {
  if (!dateStr) return '-'
  const d = new Date(dateStr)
  return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })
}
</script>

