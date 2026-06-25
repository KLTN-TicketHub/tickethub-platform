<template>
  <div class="flex flex-col gap-10 animate-fade-up pb-20 px-2 md:px-0 max-w-7xl mx-auto">
    
    <!-- Navigation & Status Banner -->
    <div class="flex flex-col gap-6">
      <router-link to="/moderator/events" class="inline-flex items-center gap-2 text-white/50 text-sm font-bold hover:text-primary transition-colors w-fit">
        <PhArrowLeft weight="bold" />
        Quay lại danh sách
      </router-link>

      <div v-if="isLoading" class="h-10 w-1/3 bg-white/5 animate-pulse rounded-lg"></div>
      
      <template v-else-if="event">
        <!-- Ticket-Style Hero Header -->
        <div class="relative bg-[#111916] border border-white/5 rounded-[2.5rem] overflow-hidden shadow-2xl flex flex-col md:flex-row min-h-[380px] md:h-[400px] group">
          
          <!-- Left Column: Details -->
          <div class="flex-1 p-8 lg:p-10 flex flex-col justify-between relative z-10">
            <div class="space-y-4">
              <!-- Badges -->
              <div class="flex flex-wrap items-center gap-3">
                <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/20 border border-primary/30 backdrop-blur-md shadow-[0_0_20px_rgba(0,200,83,0.15)]">
                  <span class="text-[11px] font-black text-primary uppercase tracking-[0.2em]">{{ event.categoryName || 'Sự kiện' }}</span>
                </div>
                <div 
                  class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full backdrop-blur-md border transition-colors"
                  :class="event.status === 'Bị từ chối' || event.status === 'Rejected' ? 'bg-danger/20 border-danger/30 text-danger' : 
                          event.status === 'Đã xuất bản' || event.status === 'Published' ? 'bg-primary/20 border-primary/30 text-primary' : 
                          'bg-white/5 border-white/10 text-white/60'"
                >
                  <span class="text-[11px] font-black uppercase tracking-[0.2em]">
                    {{ event.status }}
                  </span>
                </div>
              </div>

              <h1 class="text-3xl lg:text-5xl font-black font-heading text-white leading-[1.2] uppercase tracking-tight line-clamp-2">
                {{ event.title }}
              </h1>

              <div class="space-y-3 pt-3">
                <div class="flex items-center gap-3 text-white/70 text-[14px]">
                  <PhCalendarStar weight="bold" class="text-primary text-xl flex-shrink-0" />
                  <span class="font-bold">{{ formatDateString(event.startAt) }}</span>
                </div>
                <div class="flex items-start gap-3 text-white/70 text-[14px]">
                  <PhMapPin weight="bold" class="text-primary text-xl flex-shrink-0 mt-0.5" />
                  <span class="font-bold line-clamp-2">{{ event.location?.venueName || 'Địa điểm chưa cập nhật' }}</span>
                </div>
              </div>
            </div>

            <!-- Actions (Moderation Buttons) -->
            <div v-if="isPending" class="pt-6 border-t border-white/5 flex flex-col sm:flex-row items-center gap-4 mt-6">
              <button @click="openRejectModal" class="w-full sm:w-auto px-8 py-3.5 rounded-xl bg-danger/10 text-danger border border-danger/20 font-bold hover:bg-danger hover:text-white transition-all shadow-[0_0_15px_rgba(255,0,0,0.1)] cursor-pointer">
                Từ chối
              </button>
              <button @click="approveEvent" class="w-full sm:w-auto px-8 py-3.5 rounded-xl bg-primary text-black font-black hover:scale-105 transition-all shadow-[0_0_20px_rgba(0,200,83,0.2)] flex items-center justify-center gap-2 cursor-pointer">
                <PhCheck weight="bold" />
                Phê duyệt
              </button>
            </div>
          </div>

          <!-- Ticket Divider (Dashed vertical line with circular cutouts at top/bottom) -->
          <div class="hidden md:flex flex-col justify-between items-center relative w-px h-full bg-transparent z-20 shrink-0">
            <!-- Top cutout -->
            <div class="absolute -top-4 left-1/2 -translate-x-1/2 w-8 h-8 rounded-full bg-[#0A0F0D] border-b border-white/5 z-30"></div>
            <!-- Dashed Line -->
            <div class="h-full border-l border-dashed border-white/10 my-4"></div>
            <!-- Bottom cutout -->
            <div class="absolute -bottom-4 left-1/2 -translate-x-1/2 w-8 h-8 rounded-full bg-[#0A0F0D] border-t border-white/5 z-30"></div>
          </div>

          <!-- Right Column: Banner Cover -->
          <div class="w-full md:w-[45%] lg:w-[50%] h-[240px] md:h-full relative overflow-hidden shrink-0">
            <img 
              :src="event.coverImageUrl || event.image || 'https://picsum.photos/seed/event-hero/1200/800'" 
              :alt="event.title" 
              class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-1000"
            />
            <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-transparent md:bg-gradient-to-r md:from-black/20 md:via-transparent md:to-transparent"></div>
          </div>
        </div>

        <!-- Rejection Reason Notice -->
        <div v-if="event.status === 'Bị từ chối' && event.reasonForRejection" class="p-5 rounded-2xl bg-danger/10 border border-danger/20 flex gap-4">
          <PhWarningCircle class="text-danger text-2xl shrink-0" weight="fill" />
          <div class="flex flex-col gap-1">
            <span class="font-bold text-danger text-sm">Lý do từ chối</span>
            <p class="text-white/80 leading-relaxed">{{ event.reasonForRejection }}</p>
          </div>
        </div>
      </template>
    </div>

    <!-- Main Content -->
    <div v-if="!isLoading && event" class="grid grid-cols-1 lg:grid-cols-12 gap-8">
      
      <!-- Left Column: Details -->
      <div class="lg:col-span-8 flex flex-col gap-8">

        <!-- Description -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-3xl p-6 md:p-8 flex flex-col gap-4">
          <h2 class="text-sm font-bold text-white/40 uppercase tracking-widest">Nội dung sự kiện</h2>
          <div class="prose prose-invert prose-p:text-white/70 prose-headings:text-white max-w-none" v-html="event.description"></div>
        </div>

        <!-- Ticket Types -->
        <div class="flex flex-col gap-4">
          <h2 class="text-sm font-bold text-white/40 uppercase tracking-widest px-2">Cấu hình vé</h2>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div v-for="ticket in event.showtimes?.[selectedShowTimeIndex]?.ticketTypes" :key="ticket.id" class="p-6 rounded-3xl bg-[#111916]/50 border border-white/5 flex flex-col gap-4 hover:border-white/20 transition-colors">
              <div class="flex justify-between items-start">
                <span class="font-bold text-lg text-white">{{ ticket.ticketTypeName }}</span>
                <span class="px-2.5 py-1 bg-white/5 rounded-full text-xs font-bold text-white/60">{{ ticket.status }}</span>
              </div>
              <div class="flex justify-between items-end mt-auto pt-4 border-t border-white/5">
                <div class="flex flex-col">
                  <span class="text-xs text-white/40 uppercase font-bold tracking-wider mb-1">Giá vé</span>
                  <span class="text-xl font-black text-primary">{{ formatPrice(ticket.price) }}</span>
                </div>
                <div class="flex flex-col text-right">
                  <span class="text-xs text-white/40 uppercase font-bold tracking-wider mb-1">Số lượng</span>
                  <span class="text-lg font-bold text-white">{{ ticket.publishedQuota }} vé</span>
                </div>
              </div>
            </div>
          </div>
        </div>

      <!-- Showtimes -->
        <div v-if="event.showtimes && event.showtimes.length > 0" class="flex flex-col gap-5">
          <div class="flex items-center gap-4">
            <h2 class="text-xl font-black text-white uppercase tracking-wider">Các suất chiếu</h2>
            <div class="h-px flex-1 bg-white/10"></div>
          </div>
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            <div v-for="(st, idx) in event.showtimes" :key="st.id" 
                 @click="selectedShowTimeIndex = idx"
                 class="p-4 rounded-2xl bg-[#111916] border shadow-lg relative group overflow-hidden transition-all duration-300 cursor-pointer"
                 :class="selectedShowTimeIndex === idx ? 'border-primary ring-1 ring-primary/50' : 'border-white/10 hover:border-primary/30'">
              <div class="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
              <div class="relative z-10 flex flex-col gap-1">
                <span class="text-primary font-bold text-[11px] uppercase tracking-widest mb-1.5">Suất {{ idx + 1 }}</span>
                <div class="flex items-center gap-2 text-white/80">
                  <PhCalendarStar weight="bold" class="text-white/40" />
                  <span class="text-[13px] font-medium">{{ formatDateString(st.startAt).split(' - ')[0] || formatDateString(st.startAt) }}</span>
                </div>
                <div class="flex items-center gap-2 text-white/50 mt-1">
                  <PhClock weight="bold" class="text-white/30" />
                  <span class="text-[12px]">{{ formatTimeOnly(st.startAt) }} - {{ formatTimeOnly(st.endAt) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

      </div>

      <!-- Right Column: Meta Info -->
      <div class="lg:col-span-4 flex flex-col gap-6">
        
        <!-- Organizer -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-3xl p-6 flex flex-col gap-5">
          <h2 class="text-sm font-bold text-white/40 uppercase tracking-widest">Nhà tổ chức</h2>
          <div class="flex flex-col gap-2">
            <span class="font-black text-xl text-white">{{ event.organizerProfile?.organizerName }}</span>
            <div class="flex items-center gap-2 text-white/60 text-sm">
              <PhEnvelope class="text-white/40" />
              <span>{{ event.organizerProfile?.email }}</span>
            </div>
            <div class="flex items-center gap-2 text-white/60 text-sm">
              <PhPhone class="text-white/40" />
              <span>{{ event.organizerProfile?.phoneNumber }}</span>
            </div>
          </div>
        </div>

        <!-- Date & Time -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-3xl p-6 flex flex-col gap-6">
          <h2 class="text-sm font-bold text-white/40 uppercase tracking-widest">Thời gian</h2>
          <div class="flex flex-col gap-4">
            <div class="flex gap-4">
              <div class="w-12 h-12 rounded-xl bg-white/5 flex items-center justify-center shrink-0">
                <PhCalendarStar class="text-2xl text-primary" weight="duotone" />
              </div>
              <div class="flex flex-col justify-center">
                <span class="text-xs font-bold text-white/40 uppercase tracking-wider">Diễn ra lúc</span>
                <span class="font-bold text-white">{{ formatDateString(event.startAt) }}</span>
                <span class="text-sm text-white/60">đến {{ formatDateString(event.endAt) }}</span>
              </div>
            </div>
            <div class="h-px w-full bg-white/5"></div>
            <div class="flex gap-4">
              <div class="w-12 h-12 rounded-xl bg-white/5 flex items-center justify-center shrink-0">
                <PhTicket class="text-2xl text-[#818cf8]" weight="duotone" />
              </div>
              <div class="flex flex-col justify-center">
                <span class="text-xs font-bold text-white/40 uppercase tracking-wider">Mở bán vé</span>
                <span class="font-bold text-white">{{ formatDateString(event.saleOpenAt) }}</span>
                <span class="text-sm text-white/60">đến {{ formatDateString(event.saleCloseAt) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Location -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-3xl p-6 flex flex-col gap-5">
          <h2 class="text-sm font-bold text-white/40 uppercase tracking-widest">Địa điểm</h2>
          <div class="flex gap-4 items-start">
            <PhMapPin class="text-2xl text-white/40 shrink-0 mt-1" weight="duotone" />
            <div class="flex flex-col gap-1 text-sm text-white/80">
              <span class="font-bold text-white text-base">{{ event.location?.venueName }}</span>
              <span>{{ event.location?.addressLine }}</span>
              <span>{{ event.location?.ward }}, {{ event.location?.district }}</span>
              <span>{{ event.location?.provinceCity }}, {{ event.location?.country }}</span>
            </div>
          </div>
          <router-link v-if="event.seatMapId && event.venueId" :to="`/moderator/venues/${event.venueId}/seat-maps/${event.seatMapId}`" class="mt-2 flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl bg-white/5 border border-white/10 font-bold text-white/80 hover:bg-white/10 hover:text-white hover:border-primary/30 transition-colors">
            <PhMapTrifold weight="duotone" class="text-lg text-primary" />
            Xem sơ đồ ghế
          </router-link>
        </div>

      </div>

    </div>

    <!-- Reject Modal -->
    <div v-if="showRejectModal" class="fixed inset-0 z-[200] flex items-center justify-center px-4">
      <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="closeRejectModal"></div>
      <div class="relative w-full max-w-lg bg-[#111916] border border-white/10 rounded-3xl p-8 shadow-2xl animate-fade-up">
        <h3 class="text-2xl font-black text-white mb-2">Từ chối sự kiện</h3>
        <p class="text-white/60 text-sm mb-6">Vui lòng cung cấp lý do từ chối để nhà tổ chức có thể khắc phục.</p>
        
        <div class="flex flex-col gap-2 mb-8">
          <label class="text-sm font-bold text-white/80">Lý do từ chối <span class="text-danger">*</span></label>
          <textarea 
            v-model="rejectReason"
            rows="4"
            class="w-full bg-white/5 border border-white/10 rounded-xl p-4 text-white placeholder-white/30 focus:border-danger/50 focus:bg-white/10 transition-all outline-none resize-none"
            placeholder="Sự kiện vi phạm tiêu chuẩn..."
          ></textarea>
          <span v-if="rejectError" class="text-danger text-xs font-bold">{{ rejectError }}</span>
        </div>

        <div class="flex items-center justify-end gap-3">
          <button @click="closeRejectModal" class="px-5 py-2.5 rounded-xl font-bold text-white/60 hover:text-white hover:bg-white/5 transition-colors">
            Hủy
          </button>
          <button @click="submitReject" class="px-6 py-2.5 rounded-xl bg-danger text-white font-bold hover:bg-danger/80 transition-colors shadow-[0_0_15px_rgba(255,0,0,0.3)]">
            Xác nhận từ chối
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getModeratorEventDetail, reviewModeratorEvent } from '../../services/eventService'
import { PhArrowLeft, PhCheckCircle, PhWarningCircle, PhClockClockwise, PhCheck, PhCalendarStar, PhMapPin, PhEnvelope, PhPhone, PhTicket, PhMapTrifold, PhClock } from '@phosphor-icons/vue'

const route = useRoute()
const router = useRouter()

const event = ref(null)
const isLoading = ref(true)
const selectedShowTimeIndex = ref(0)

const showRejectModal = ref(false)
const rejectReason = ref('')
const rejectError = ref('')

onMounted(async () => {
  await loadEventDetail()
})

const loadEventDetail = async () => {
  isLoading.value = true
  try {
    const res = await getModeratorEventDetail(route.params.id)
    if (res.success) {
      event.value = res.data
    } else {
      alert("Không tìm thấy sự kiện")
      router.push('/moderator/events')
    }
  } catch (error) {
    console.error("Error loading event details", error)
  } finally {
    isLoading.value = false
  }
}

const isPending = computed(() => {
  return event.value?.status === 'PendingApproval' || event.value?.status === 'Chờ duyệt'
})

// UI Helpers
const getBannerClass = (status) => {
  if (status === 'Đã xuất bản' || status === 'Published') return 'border-[#00c853]/30 shadow-[0_0_30px_rgba(0,200,83,0.05)]'
  if (status === 'Bị từ chối' || status === 'Rejected') return 'border-danger/30 shadow-[0_0_30px_rgba(255,0,0,0.05)]'
  return 'border-[#ff9100]/30 shadow-[0_0_30px_rgba(255,145,0,0.05)]'
}

const getIconColorClass = (status) => {
  if (status === 'Đã xuất bản' || status === 'Published') return 'text-[#00c853]'
  if (status === 'Bị từ chối' || status === 'Rejected') return 'text-danger'
  return 'text-[#ff9100]'
}

const formatDateString = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  }).format(date)
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price || 0)
}

// Review Actions
const approveEvent = async () => {
  if (confirm(`Bạn có chắc chắn muốn duyệt xuất bản sự kiện "${event.value.title}"?`)) {
    try {
      const res = await reviewModeratorEvent(event.value.id, { isApproved: true, reason: "" })
      if (res.success) {
        alert("Đã duyệt sự kiện thành công!")
        await loadEventDetail()
      } else {
        alert(res.message || "Lỗi khi duyệt sự kiện")
      }
    } catch (e) {
      console.error(e)
      alert("Đã có lỗi xảy ra. Vui lòng thử lại sau.")
    }
  }
}

const openRejectModal = () => {
  rejectReason.value = ''
  rejectError.value = ''
  showRejectModal.value = true
}

const closeRejectModal = () => {
  showRejectModal.value = false
}

const submitReject = async () => {
  if (!rejectReason.value.trim()) {
    rejectError.value = "Lý do từ chối không được để trống."
    return
  }
  
  try {
    const res = await reviewModeratorEvent(event.value.id, { isApproved: false, reason: rejectReason.value })
    if (res.success) {
      alert("Đã từ chối sự kiện!")
      closeRejectModal()
      await loadEventDetail()
    } else {
      alert(res.message || "Lỗi khi từ chối sự kiện")
    }
  } catch (e) {
    console.error(e)
    alert("Đã có lỗi xảy ra. Vui lòng thử lại sau.")
  }
}

const formatTimeOnly = (dateStr) => {
  if (!dateStr) return '--:--'
  try {
    const date = new Date(dateStr)
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false })
  } catch (e) {
    return dateStr
  }
}
</script>
