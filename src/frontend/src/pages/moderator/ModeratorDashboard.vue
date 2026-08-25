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
        <div v-if="isLoadingPending" class="p-16 flex flex-col items-center justify-center gap-3 bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
          <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách...</span>
        </div>
        <div v-else-if="pendingEvents.length === 0" class="p-16 flex flex-col items-center justify-center text-center gap-3 bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <PhCheckCircle weight="duotone" class="text-4xl text-white/20" />
          <span class="font-bold font-heading text-white text-lg">Không có sự kiện nào chờ duyệt</span>
        </div>
        <BaseTable v-else :columns="eventColumns" :data="pendingEvents">
          <template #event="{ row }">
            <router-link :to="`/moderator/events/${row.id}`" class="flex items-center gap-4">
              <div class="w-12 h-12 rounded-xl overflow-hidden border border-white/10 flex-shrink-0">
                <img :src="row.coverImageUrl || 'https://picsum.photos/seed/' + row.id + '/200/200'" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              </div>
              <span class="font-bold text-white group-hover:text-primary transition-colors line-clamp-1">{{ row.title }}</span>
            </router-link>
          </template>
          <template #location="{ row }">
            <span class="text-[13px] text-white/60 font-bold uppercase tracking-wider">{{ row.location?.provinceCity || 'Chưa cập nhật' }}</span>
          </template>
          <template #date="{ row }">
            <span class="text-[14px] text-white/80 font-medium">{{ formatDate(row.startAt) }}</span>
          </template>
          <template #action="{ row }">
            <div class="flex justify-end gap-2">
              <button :disabled="processingEventId === row.id" @click="approveEvent(row)" class="px-3 py-1.5 text-[11px] font-bold rounded-lg bg-primary/10 text-primary hover:bg-primary hover:text-black transition-all disabled:opacity-40">Duyệt</button>
              <router-link :to="`/moderator/events/${row.id}`" class="px-3 py-1.5 text-[11px] font-bold rounded-lg bg-danger/10 text-danger hover:bg-danger hover:text-white transition-all">Từ chối</router-link>
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

      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted } from 'vue'
import { getEvents } from '../../stores/eventStore'
import { getModeratorEvents, reviewModeratorEvent } from '../../services/eventService'
import { addToast } from '../../stores/adminStore'
import { getErrorMessage } from '../../utils/apiError'
import BaseTable from '../../components/ui/BaseTable.vue'
import {
  PhTicket, PhCheckCircle, PhClock, PhTag, PhWarningCircle, PhSpinner,
  PhMicrophoneStage, PhTrophy, PhMaskHappy, PhCompass, PhBooks
} from '@phosphor-icons/vue'

// "Phân bổ thể loại" panel chưa có endpoint đếm sự kiện theo thể loại cho moderator, giữ tạm nguồn cũ.
const allEvents = ref(getEvents())

const eventColumns = [
  { key: 'event', label: 'Sự kiện' },
  { key: 'location', label: 'Địa điểm' },
  { key: 'date', label: 'Thời gian' },
  { key: 'action', label: 'Thao tác', class: 'text-right' },
]

const pendingEvents = ref([])
const isLoadingPending = ref(true)
const processingEventId = ref(null)

const totalEventsCount = ref(0)
const pendingCount = ref(0)
const publishedCount = ref(0)

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

const mainStats = computed(() => [
  {
    label: 'Tổng sự kiện',
    value: totalEventsCount.value,
    sub: 'Hệ thống',
    icon: PhTicket,
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-[#818cf8]/10',
    iconColorClass: 'text-[#818cf8]',
    glowClass: 'bg-[#818cf8]'
  },
  {
    label: 'Đã phê duyệt',
    value: publishedCount.value,
    sub: 'Đang hiển thị',
    icon: PhCheckCircle,
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-primary/10',
    iconColorClass: 'text-primary',
    glowClass: 'bg-primary'
  },
  {
    label: 'Chờ duyệt',
    value: pendingCount.value,
    sub: 'Cần xử lý',
    icon: PhClock,
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-warning/10',
    iconColorClass: 'text-warning',
    glowClass: 'bg-warning'
  },
  {
    label: 'Thể loại',
    value: categories.value.length,
    sub: 'Đang mở',
    icon: PhTag,
    bgClass: 'bg-[#111916] border border-white/5',
    iconBgClass: 'bg-[#f472b6]/10',
    iconColorClass: 'text-[#f472b6]',
    glowClass: 'bg-[#f472b6]'
  }
])

const loadPendingEvents = async () => {
  isLoadingPending.value = true
  try {
    const res = await getModeratorEvents({ PageNumber: 1, PageSize: 4, Status: 'PendingApproval' })
    if (res.success && res.data) {
      pendingEvents.value = res.data.data || []
      pendingCount.value = res.data.totalCount || 0
    } else {
      pendingEvents.value = []
    }
  } catch (err) {
    console.error('Error loading pending events:', err)
    addToast(getErrorMessage(err, 'Không thể tải danh sách sự kiện chờ duyệt.'), 'error')
    pendingEvents.value = []
  } finally {
    isLoadingPending.value = false
  }
}

const loadEventCounts = async () => {
  try {
    const [totalRes, publishedRes] = await Promise.all([
      getModeratorEvents({ PageNumber: 1, PageSize: 1 }),
      getModeratorEvents({ PageNumber: 1, PageSize: 1, Status: 'Published' })
    ])
    if (totalRes.success && totalRes.data) totalEventsCount.value = totalRes.data.totalCount || 0
    if (publishedRes.success && publishedRes.data) publishedCount.value = publishedRes.data.totalCount || 0
  } catch (err) {
    console.error('Error loading event counts:', err)
  }
}

const approveEvent = async (row) => {
  if (!confirm(`Bạn có chắc chắn muốn duyệt xuất bản sự kiện "${row.title}"?`)) return

  processingEventId.value = row.id
  try {
    const res = await reviewModeratorEvent(row.id, { isApproved: true, reason: '' })
    if (res.success) {
      addToast('Đã duyệt sự kiện thành công!', 'success')
      await Promise.all([loadPendingEvents(), loadEventCounts()])
    } else {
      addToast(res.message || 'Không thể duyệt sự kiện.', 'error')
    }
  } catch (err) {
    console.error('Error approving event:', err)
    addToast(getErrorMessage(err, 'Không thể duyệt sự kiện.'), 'error')
  } finally {
    processingEventId.value = null
  }
}

onMounted(() => {
  loadPendingEvents()
  loadEventCounts()
})

const formatDate = (d) => {
  if (!d) return 'TBA'
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
</script>
