<template>
  <div class="flex flex-col gap-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6">
      <div>
        <h1 class="font-heading text-3xl font-bold text-main mb-2">Quản lý sự kiện</h1>
        <p class="text-muted font-medium">Theo dõi và cập nhật tất cả sự kiện trên hệ thống</p>
      </div>
      <BaseButton variant="primary" size="lg" class="!px-8 !rounded-2xl shadow-xl shadow-primary/20" @click="openCreateModal">
        <span class="mr-2">+</span> Tạo sự kiện mới
      </BaseButton>
    </div>

    <!-- Filter Bar -->
    <div class="bg-card border border-border-main p-6 rounded-[24px] flex flex-col lg:flex-row gap-4 items-center">
      <div class="flex-1 flex items-center gap-3 bg-surface border border-border-main rounded-xl px-4 w-full group focus-within:border-primary transition-all">
        <svg class="w-4 h-4 text-muted group-focus-within:text-primary transition-colors" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
        <input 
          type="text" 
          v-model="localSearch" 
          placeholder="Tìm theo tên, địa điểm hoặc nghệ sĩ..." 
          class="flex-1 bg-transparent border-none py-3 text-[14px] text-main outline-none"
        />
      </div>
      <div class="flex gap-4 w-full lg:w-auto">
        <BaseSelect 
          :options="statusOptions" 
          v-model="statusFilter"
          class="min-w-[160px] flex-1 lg:flex-none"
        />
        <BaseSelect 
          :options="categoryOptions" 
          v-model="categoryFilter"
          class="min-w-[160px] flex-1 lg:flex-none"
        />
      </div>
    </div>

    <!-- Table Container -->
    <div class="bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl">
      <BaseTable :columns="columns" :data="filteredEvents">
        <!-- Event Slot -->
        <template #event="{ row }">
          <div class="flex items-center gap-4">
            <div class="w-14 h-10 rounded-lg overflow-hidden border border-border-main">
              <img :src="row.image" alt="thumb" class="w-full h-full object-cover" />
            </div>
            <div class="flex flex-col">
              <span class="font-bold text-main group-hover:text-primary transition-colors">{{ row.title }}</span>
              <span class="text-[11px] font-bold text-muted uppercase tracking-wider">{{ row.slug }}</span>
            </div>
          </div>
        </template>

        <!-- Location Slot -->
        <template #location="{ row }">
          <div class="flex flex-col">
            <span class="text-main font-medium">{{ row.location.name }}</span>
            <span class="text-[12px] text-muted capitalize">{{ row.location.city }}</span>
          </div>
        </template>

        <!-- Date Slot -->
        <template #date="{ row }">
          <span class="text-[14px] text-muted font-medium">{{ formatDate(row.dateStart) }}</span>
        </template>

        <!-- Price Slot -->
        <template #price="{ row }">
          <span class="text-[14px] font-bold text-primary">{{ formatPrice(row.priceRange) }}</span>
        </template>

        <!-- Status Slot -->
        <template #status="{ row }">
          <BaseBadge :variant="getStatusVariant(row.status)" size="sm">
            {{ getStatusLabel(row.status) }}
          </BaseBadge>
        </template>

        <!-- Actions Slot -->
        <template #actions="{ row }">
          <div class="flex justify-end gap-2">
            <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-primary" @click="openEditModal(row)">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M12 20h9"/><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"/></svg>
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-danger" @click="handleDelete(row)">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M3 6h18"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>
            </BaseButton>
          </div>
        </template>
      </BaseTable>

      <!-- Empty State -->
      <div v-if="filteredEvents.length === 0" class="py-20 flex flex-col items-center text-center">
        <div class="w-20 h-20 bg-surface rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner">🔍</div>
        <h3 class="text-xl font-bold text-main mb-2">Không tìm thấy sự kiện</h3>
        <p class="text-muted max-w-xs mb-8">Hãy thử thay đổi từ khóa tìm kiếm hoặc bộ lọc của bạn.</p>
        <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa bộ lọc</BaseButton>
      </div>
    </div>

    <EventFormModal 
      :is-open="isModalOpen" 
      :event-data="currentEditEvent" 
      @close="closeModal" 
      @save="handleSave"
    />
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { getEvents, addEvent, updateEvent, deleteEvent } from '../../stores/eventStore'
import { adminSearch, openConfirm, addToast } from '../../stores/adminStore'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import BaseSelect from '../../components/ui/BaseSelect.vue'
import EventFormModal from '../../components/admin/EventFormModal.vue'

const allEvents = computed(() => getEvents())

const localSearch = ref('')
const statusFilter = ref('all')
const categoryFilter = ref('all')

const isModalOpen = ref(false)
const currentEditEvent = ref(null)

const statusOptions = [
  { value: 'all', label: 'Tất cả trạng thái' },
  { value: 'upcoming', label: 'Sắp diễn ra' },
  { value: 'sold-out', label: 'Hết vé' },
  { value: 'ended', label: 'Đã kết thúc' },
]

const categoryOptions = [
  { value: 'all', label: 'Tất cả thể loại' },
  { value: 'concerts', label: 'Concert' },
  { value: 'arts', label: 'Sân khấu' },
  { value: 'sports', label: 'Thể thao' },
  { value: 'experiences', label: 'Trải nghiệm' },
  { value: 'workshops', label: 'Workshop' },
]

const columns = [
  { key: 'event', label: 'Sự kiện' },
  { key: 'category', label: 'Thể loại', class: 'capitalize' },
  { key: 'location', label: 'Địa điểm' },
  { key: 'date', label: 'Thời gian' },
  { key: 'price', label: 'Giá vé' },
  { key: 'status', label: 'Trạng thái' },
  { key: 'actions', label: '', class: 'w-24' },
]

// Sync with global Topbar search
watch(adminSearch, (val) => { localSearch.value = val })

const filteredEvents = computed(() => {
  const q = localSearch.value.toLowerCase()
  return allEvents.value.filter(e => {
    const performers = e.performers?.map(p => p.name.toLowerCase()).join(' ') || ''
    const matchSearch = !q ||
      e.title.toLowerCase().includes(q) ||
      performers.includes(q) ||
      e.location.name.toLowerCase().includes(q)
    const matchStatus = statusFilter.value === 'all' || e.status === statusFilter.value
    const matchCat = categoryFilter.value === 'all' || e.category === categoryFilter.value
    return matchSearch && matchStatus && matchCat
  })
})

const getStatusVariant = (status) => {
  if (status === 'upcoming') return 'primary'
  if (status === 'sold-out') return 'danger'
  return 'secondary'
}

const getStatusLabel = (status) => {
  const map = {
    'upcoming': 'Sắp diễn ra',
    'sold-out': 'Hết vé',
    'ended': 'Kết thúc'
  }
  return map[status] || status
}

const formatDate = (dateStr) => {
  if (!dateStr) return 'TBA'
  try { return new Date(dateStr).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' }) }
  catch(e) { return dateStr }
}

const formatPrice = (range) => {
  if (!range) return 'Miễn phí'
  if (range.min === 0 && range.max === 0) return 'Miễn phí'
  const fmt = n => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(n).replace('₫', 'đ')
  if (range.min === range.max) return fmt(range.min)
  return `${fmt(range.min)}`
}

const openCreateModal = () => {
  currentEditEvent.value = null
  isModalOpen.value = true
}

const openEditModal = (event) => {
  currentEditEvent.value = event
  isModalOpen.value = true
}

const closeModal = () => { isModalOpen.value = false }

const handleSave = async (eventData) => {
  if (eventData.id) {
    await updateEvent(eventData.id, eventData)
    addToast('Đã cập nhật sự kiện!', 'success')
  } else {
    await addEvent(eventData)
    addToast('Đã tạo sự kiện mới!', 'success')
  }
  closeModal()
}

const handleDelete = (event) => {
  openConfirm(
    'Xóa sự kiện',
    `Bạn có chắc chắn muốn xóa "${event.title}"? Hành động này không thể hoàn tác.`,
    async () => {
      await deleteEvent(event.id)
      addToast(`Đã xóa sự kiện "${event.title}"`, 'error')
    }
  )
}

const resetFilters = () => {
  localSearch.value = ''
  statusFilter.value = 'all'
  categoryFilter.value = 'all'
}
</script>

