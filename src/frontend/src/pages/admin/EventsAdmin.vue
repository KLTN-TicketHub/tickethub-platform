<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Quản lý sự kiện</h1>
        <p class="text-white/50 font-medium text-lg">Theo dõi và cập nhật tất cả sự kiện trên hệ thống</p>
      </div>
      <BaseButton variant="primary" size="lg" @click="openCreateModal">
        <PhPlus weight="bold" class="text-lg" /> Tạo sự kiện
      </BaseButton>
    </div>

    <!-- Filter Bar -->
    <div class="flex flex-col xl:flex-row xl:items-center justify-between gap-6 bg-[#111916] border border-white/5 rounded-[2rem] p-4 lg:px-6">
      <div class="flex-1 flex items-center gap-3 bg-white/5 border border-white/10 rounded-full px-4 group focus-within:border-primary/50 transition-all w-full max-w-md">
        <PhMagnifyingGlass class="text-white/40 group-focus-within:text-primary text-lg transition-colors" weight="bold" />
        <input 
          type="text" 
          v-model="localSearch" 
          placeholder="Tìm theo tên, địa điểm..." 
          class="flex-1 bg-transparent border-none py-2.5 text-[14px] text-white outline-none placeholder:text-white/30"
        />
      </div>
      <div class="flex items-center gap-3 overflow-x-auto hide-scroll">
        <BaseSelect :options="statusOptions" v-model="statusFilter" class="w-[180px] flex-shrink-0" />
        <BaseSelect :options="categoryOptions" v-model="categoryFilter" class="w-[180px] flex-shrink-0" />
      </div>
    </div>

    <!-- Table Container -->
    <div class="flex flex-col gap-4">
      <BaseTable :columns="columns" :data="filteredEvents">
        <template #event="{ row }">
          <div class="flex items-center gap-4">
            <div class="w-14 h-10 rounded-xl overflow-hidden border border-white/10 flex-shrink-0">
              <img :src="row.image" alt="thumb" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
            </div>
            <div class="flex flex-col">
              <span class="font-bold text-white group-hover:text-primary transition-colors line-clamp-1">{{ row.title }}</span>
              <span class="text-[11px] font-bold text-white/40 uppercase tracking-[0.1em] mt-0.5">{{ row.slug }}</span>
            </div>
          </div>
        </template>
        <template #location="{ row }">
          <div class="flex flex-col">
            <span class="text-white/90 font-bold text-[14px]">{{ row.location.name }}</span>
            <span class="text-[12px] text-white/50">{{ row.location.city }}</span>
          </div>
        </template>
        <template #date="{ row }">
          <span class="text-[14px] text-white/70 font-medium">{{ formatDate(row.dateStart) }}</span>
        </template>
        <template #price="{ row }">
          <span class="text-[14px] font-bold text-primary">{{ formatPrice(row.priceRange) }}</span>
        </template>
        <template #status="{ row }">
          <BaseBadge :variant="getStatusVariant(row.status)">{{ getStatusLabel(row.status) }}</BaseBadge>
        </template>
        <template #actions="{ row }">
          <div class="flex justify-end gap-2">
            <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-white/10" @click="openEditModal(row)">
              <PhPencilSimple weight="bold" class="text-white/70" />
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-danger/10 hover:!text-danger" @click="handleDelete(row)">
              <PhTrash weight="bold" class="text-danger/70 hover:text-danger" />
            </BaseButton>
          </div>
        </template>
      </BaseTable>

      <div v-if="filteredEvents.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
        <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
          <PhMagnifyingGlass weight="duotone" />
        </div>
        <h3 class="text-xl font-bold font-heading text-white mb-2">Không tìm thấy sự kiện</h3>
        <p class="text-white/50 max-w-xs mb-8">Thử thay đổi từ khóa hoặc xóa các bộ lọc để tìm lại.</p>
        <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa bộ lọc</BaseButton>
      </div>
    </div>

    <EventFormModal :is-open="isModalOpen" :event-data="currentEditEvent" @close="closeModal" @save="handleSave" />
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
import { PhPlus, PhMagnifyingGlass, PhPencilSimple, PhTrash } from '@phosphor-icons/vue'

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
  return 'neutral'
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
<style scoped>
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
