<template>
  <div class="max-w-6xl mx-auto py-16 px-6 min-h-[80vh]">
    <!-- PREMIUM HEADER -->
    <header class="flex flex-col md:flex-row md:items-end justify-between gap-8 mb-16 animate-fade-up">
      <div class="space-y-4">
        <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-black tracking-widest uppercase shadow-[0_0_20px_rgba(0,200,83,0.15)]">
          <PhWallet weight="fill" /> Ví của tôi
        </div>
        <h1 class="text-5xl font-black font-heading text-white tracking-tight uppercase">Vé & Thẻ của bạn</h1>
        <p class="text-lg text-white/50 font-medium max-w-md">Tất cả những khoảnh khắc tuyệt vời đã sẵn sàng để bắt đầu.</p>
      </div>
      
      <div class="flex items-center gap-6">
        <div class="flex flex-col items-end">
          <span class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Trạng thái ví</span>
          <span class="text-[15px] font-bold text-white flex items-center gap-1.5"><PhShieldCheck weight="fill" class="text-primary" /> Đã xác thực</span>
        </div>
        <div class="h-10 w-px bg-white/10 hidden md:block"></div>
        <div class="bg-[#111916] border border-white/5 rounded-2xl p-4 px-6 flex items-center gap-4 shadow-inner">
          <div class="flex flex-col">
            <span class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Tổng số vé</span>
            <span class="text-2xl font-black text-primary font-heading">
              <PhSpinner v-if="isLoading" class="animate-spin text-xl inline" />
              <span v-else>{{ totalPages > 1 ? totalCount : filteredTickets.length }}</span>
            </span>
          </div>
        </div>
      </div>
    </header>

    <!-- TABS -->
    <div class="flex gap-8 border-b border-white/10 mb-12 animate-fade-up [animation-delay:100ms]">
      <button 
        v-for="t in tabs" 
        :key="t.id"
        @click="activeTab = t.id"
        class="pb-4 text-[15px] font-bold transition-all relative cursor-pointer"
        :class="[activeTab === t.id ? 'text-primary' : 'text-white/50 hover:text-white']"
      >
        {{ t.label }}
        <div v-if="activeTab === t.id" class="absolute bottom-0 left-0 right-0 h-1 bg-primary rounded-full animate-in slide-in-from-left-2 duration-300 shadow-[0_0_10px_rgba(0,200,83,0.5)]"></div>
      </button>
    </div>

    <!-- TICKET LIST -->
    <div v-if="filteredTickets.length > 0" class="flex flex-col gap-4 animate-fade-up [animation-delay:200ms]">
      <div
        v-for="(ticket, idx) in filteredTickets"
        :key="ticket.id"
        class="group flex items-center gap-5 bg-[#111916] border border-white/5 rounded-2xl p-4 hover:border-primary/30 transition-all duration-300 cursor-pointer"
        :style="`animation-delay: ${idx * 50}ms`"
        @click="openTicketDetail(ticket)"
      >
        <!-- Thumbnail -->
        <div class="relative w-20 h-20 md:w-24 md:h-24 rounded-xl overflow-hidden shrink-0">
          <img
            :src="ticket.eventImage"
            :alt="ticket.eventTitle"
            class="w-full h-full object-cover transition-all duration-500"
            :class="activeTab === 'past' ? 'grayscale opacity-60' : 'group-hover:scale-105'"
          />
        </div>

        <!-- Info -->
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2 text-primary/80 mb-1">
            <PhCalendarBlank weight="bold" class="text-sm" />
            <span class="text-[11px] font-bold uppercase tracking-[0.15em]">{{ formatDate(ticket.showtimeStartAt) }}</span>
          </div>
          <h3 class="text-lg md:text-xl font-black font-heading text-white leading-tight truncate group-hover:text-primary transition-colors">
            {{ ticket.eventTitle }}
          </h3>
          <div class="flex flex-wrap items-center gap-x-4 gap-y-1 mt-2 text-[12px] text-white/50 font-medium">
            <span class="flex items-center gap-1.5">
              <PhTicket weight="bold" class="text-white/30" /> {{ ticket.ticketTypeName }}
            </span>
            <span class="flex items-center gap-1.5">
              <PhArmchair weight="bold" class="text-white/30" />
              {{ ticket.seatName ? (ticket.rowName + ticket.seatName) : 'Tự do' }}
            </span>
            <span class="hidden md:flex items-center gap-1.5 truncate">
              <PhMapPin weight="bold" class="text-white/30" /> {{ ticket.organizerName || 'Ban tổ chức' }}
            </span>
            <span class="font-mono text-white/40">#{{ ticket.issuedTicketId?.substring(0,8).toUpperCase() }}</span>
          </div>
        </div>

        <!-- Status + Action -->
        <div class="flex flex-col items-end gap-2.5 shrink-0">
          <BaseBadge :variant="getStatusTag(ticket).variant" size="sm">
            {{ getStatusTag(ticket).label }}
          </BaseBadge>
          <BaseButton
            v-if="activeTab === 'upcoming'"
            variant="outline"
            size="sm"
            class="!rounded-full !px-4 flex items-center gap-1.5"
            @click.stop="openTicketDetail(ticket)"
          >
            <PhQrCode weight="bold" /> Mã QR
          </BaseButton>
          <div v-else-if="myRatings[ticket.eventId]" class="flex items-center gap-1 text-[13px] font-black text-primary">
            <PhStar weight="fill" /> {{ myRatings[ticket.eventId].overallRating.toFixed(1) }}
            <span class="text-[11px] font-bold text-white/40 uppercase tracking-widest ml-1">Đã đánh giá</span>
          </div>
          <button
            v-else
            class="text-[12px] font-bold text-white/40 hover:text-white uppercase tracking-widest transition-colors"
            @click.stop="openTicketDetail(ticket)"
          >
            Chi tiết
          </button>
        </div>
      </div>
    </div>

    <!-- PAGINATION -->
    <div v-if="totalPages > 1" class="flex justify-center gap-4 mt-12 animate-fade-up">
      <BaseButton 
        variant="outline" 
        :disabled="pageIndex <= 1"
        @click="pageIndex--; fetchTickets()"
        class="!px-6"
      >
        Trang trước
      </BaseButton>
      <div class="flex items-center text-white/50 font-medium">
        Trang {{ pageIndex }} / {{ totalPages }}
      </div>
      <BaseButton 
        variant="outline" 
        :disabled="pageIndex >= totalPages"
        @click="pageIndex++; fetchTickets()"
        class="!px-6"
      >
        Trang sau
      </BaseButton>
    </div>

    <!-- EMPTY STATE -->
    <div v-if="filteredTickets.length === 0 && !isLoading" class="flex flex-col items-center justify-center py-32 text-center animate-fade-up">
      <div class="w-40 h-40 bg-[#111916] border border-white/5 rounded-[3rem] flex items-center justify-center text-7xl mb-10 shadow-[0_30px_60px_-15px_rgba(0,0,0,0.5)] relative overflow-hidden group text-white/20">
        <PhTicket weight="duotone" />
        <div class="absolute inset-0 bg-primary/10 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        <div class="absolute -right-2 -bottom-2 w-14 h-14 bg-[#0A0F0D] border border-white/10 rounded-full flex items-center justify-center text-2xl shadow-xl text-primary">
          <PhQuestion weight="bold" />
        </div>
      </div>
      <h3 class="text-4xl font-black font-heading text-white mb-4">Ví của bạn đang trống</h3>
      <p class="text-lg text-white/50 max-w-sm mb-12 font-medium">Đừng để lỡ mất những trải nghiệm tuyệt vời đang chờ đón bạn.</p>
      <router-link to="/">
        <BaseButton variant="primary" size="lg" class="!px-12 !rounded-[2rem] shadow-[0_20px_40px_rgba(0,200,83,0.3)]">
          Khám phá sự kiện ngay
        </BaseButton>
      </router-link>
    </div>

    <!-- TICKET DETAIL MODAL -->
    <BaseModal 
      :show="!!selectedTicket" 
      @close="selectedTicket = null"
      :title="selectedTicket?.eventTitle"
      subtitle="Thông tin vé điện tử chi tiết"
      size="lg"
    >
      <div v-if="selectedTicket" class="flex flex-col gap-8 py-4">
        <!-- Hero in modal -->
        <div class="relative rounded-[2rem] overflow-hidden h-56 border border-white/10 shadow-2xl">
          <img :src="selectedTicket.eventImage" class="w-full h-full object-cover opacity-60 mix-blend-screen" />
          <div class="absolute inset-0 bg-gradient-to-t from-[#0A0F0D] via-transparent to-transparent opacity-90"></div>
          <div class="absolute bottom-6 left-6 right-6">
            <h3 class="text-3xl font-black font-heading text-white tracking-tight">{{ selectedTicket.eventTitle }}</h3>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-10">
          <div class="space-y-6">
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-white/50 uppercase tracking-widest flex items-center gap-1.5"><PhUser weight="bold" /> Khán giả</span>
              <p class="text-lg font-black text-white">{{ store.user?.name || 'Khách hàng' }}</p>
            </div>
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-white/50 uppercase tracking-widest flex items-center gap-1.5"><PhTicket weight="bold" /> Loại vé</span>
              <p class="text-lg font-black text-primary uppercase tracking-widest">{{ selectedTicket.ticketTypeName }}</p>
            </div>
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-white/50 uppercase tracking-widest flex items-center gap-1.5"><PhClock weight="bold" /> Ngày giờ</span>
              <p class="text-lg font-black text-white">{{ formatDate(selectedTicket.showtimeStartAt) }}</p>
            </div>
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-white/50 uppercase tracking-widest flex items-center gap-1.5"><PhArmchair weight="bold" /> Vị trí chỗ ngồi</span>
              <p class="text-lg font-black text-white">{{ selectedTicket.seatName ? (selectedTicket.rowName + selectedTicket.seatName) : 'Khu vực đứng (Free)' }}</p>
            </div>
          </div>

          <div v-if="selectedTicket.status !== 'Used'" class="flex flex-col items-center justify-center p-8 bg-[#111916] rounded-[2rem] border border-white/5 shadow-inner">
            <div class="w-56 h-56 bg-white p-4 rounded-[2rem] shadow-[0_0_50px_rgba(255,255,255,0.1)] mb-6 relative">
              <div class="absolute inset-0 border-4 border-dashed border-black/10 rounded-[2rem] m-2 pointer-events-none"></div>
              <img v-if="selectedTicket.qrCodeBase64" :src="'data:image/png;base64,' + selectedTicket.qrCodeBase64" class="w-full h-full object-contain" />
              <div v-else class="w-full h-full bg-[repeating-linear-gradient(45deg,#000_0,#000_4px,transparent_4px,transparent_8px),repeating-linear-gradient(-45deg,#000_0,#000_4px,transparent_4px,transparent_8px)] opacity-90 rounded-xl"></div>
            </div>
            <p class="text-[14px] font-mono font-black text-white tracking-[0.4em] uppercase">{{ selectedTicket.issuedTicketId?.substring(0,8).toUpperCase() }}</p>
            <p class="text-[11px] font-bold text-white/50 uppercase mt-2 tracking-widest">Mã QR chính chủ</p>
          </div>
          <div v-else class="flex flex-col items-center justify-center p-8 bg-[#111916] rounded-[2rem] border border-white/5 shadow-inner gap-3">
            <div class="w-16 h-16 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-3xl text-primary">
              <PhCheckCircle weight="fill" />
            </div>
            <p class="text-[15px] font-black text-white">Vé đã được sử dụng</p>
            <p class="text-[12px] font-medium text-white/50 text-center leading-relaxed">Vé này đã check-in tại sự kiện nên không còn hiển thị mã QR.</p>
          </div>
        </div>

        <div v-if="selectedTicket.status !== 'Used'" class="p-6 bg-primary/10 border border-primary/20 rounded-2xl flex gap-4 mt-2">
          <PhLightbulb class="text-3xl text-primary flex-shrink-0" weight="duotone" />
          <p class="text-[13px] font-medium text-white/80 leading-relaxed">
            Vui lòng xuất trình mã QR này tại cổng kiểm soát để vào sự kiện. Hãy chuẩn bị sẵn thiết bị có độ sáng màn hình cao nhất và <strong>không chia sẻ mã này</strong> với người khác.
          </p>
        </div>

        <!-- Đánh giá đã gửi (nếu có) -->
        <div v-if="myRatingForSelected" class="p-6 bg-[#111916] border border-white/5 rounded-2xl flex flex-col gap-5">
          <div class="flex items-center justify-between">
            <h4 class="text-[13px] font-black text-white uppercase tracking-widest flex items-center gap-2">
              <PhStar weight="fill" class="text-primary" /> Đánh giá của bạn
            </h4>
            <span class="text-2xl font-black text-primary font-heading">
              {{ myRatingForSelected.overallRating.toFixed(1) }}<span class="text-sm text-white/40">/5</span>
            </span>
          </div>
          <div class="grid grid-cols-2 gap-x-6 gap-y-4">
            <StarRatingInput :model-value="myRatingForSelected.soundRating" label="Âm thanh" readonly />
            <StarRatingInput :model-value="myRatingForSelected.visualRating" label="Ánh sáng / Hình ảnh" readonly />
            <StarRatingInput :model-value="myRatingForSelected.organizationRating" label="Tổ chức / Sắp xếp" readonly />
            <StarRatingInput :model-value="myRatingForSelected.facilityRating" label="Trang thiết bị" readonly />
            <StarRatingInput :model-value="myRatingForSelected.serviceRating" label="Nhân viên / Dịch vụ" readonly />
            <StarRatingInput :model-value="myRatingForSelected.performanceRating" label="Nghệ sĩ / Chương trình" readonly />
          </div>
          <p v-if="myRatingForSelected.comment" class="text-[13px] font-medium text-white/70 leading-relaxed border-t border-white/5 pt-4">
            "{{ myRatingForSelected.comment }}"
          </p>
        </div>
      </div>

      <template #footer>
        <BaseButton variant="outline" @click="selectedTicket = null" class="!px-6">Đóng</BaseButton>
        <BaseButton v-if="selectedTicket?.status === 'Used' && !myRatingForSelected" variant="primary" class="!rounded-full shadow-lg shadow-primary/20 flex items-center gap-2" @click="openRatingModal">
          <PhStar weight="fill" /> Đánh giá sự kiện
        </BaseButton>
        <BaseButton v-else-if="selectedTicket?.status !== 'Used'" variant="primary" class="!rounded-full shadow-lg shadow-primary/20 flex items-center gap-2">
          <PhWallet weight="fill" /> Lưu vào Ví thiết bị
        </BaseButton>
      </template>
    </BaseModal>

    <!-- RATING SUBMISSION MODAL -->
    <BaseModal
      :show="showRatingModal"
      @close="showRatingModal = false"
      title="Đánh giá sự kiện"
      :subtitle="selectedTicket?.eventTitle"
      size="md"
    >
      <div class="flex flex-col gap-6 py-2">
        <StarRatingInput v-model="ratingForm.soundRating" label="Âm thanh" />
        <StarRatingInput v-model="ratingForm.visualRating" label="Ánh sáng / Hình ảnh" />
        <StarRatingInput v-model="ratingForm.organizationRating" label="Tổ chức / Sắp xếp" />
        <StarRatingInput v-model="ratingForm.facilityRating" label="Trang thiết bị / Cơ sở vật chất" />
        <StarRatingInput v-model="ratingForm.serviceRating" label="Nhân viên / Dịch vụ" />
        <StarRatingInput v-model="ratingForm.performanceRating" label="Nghệ sĩ / Chương trình biểu diễn" />

        <div class="flex flex-col gap-2">
          <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Bình luận (không bắt buộc)</label>
          <textarea
            v-model="ratingForm.comment"
            maxlength="1000"
            rows="4"
            placeholder="Chia sẻ trải nghiệm của bạn về sự kiện..."
            class="w-full rounded-2xl px-5 py-3 text-[14px] font-medium bg-white/5 border border-white/10 text-white placeholder:text-white/30 outline-none focus:border-primary/50 focus:bg-white/10 transition-all resize-none"
          ></textarea>
        </div>
      </div>

      <template #footer>
        <BaseButton variant="outline" @click="showRatingModal = false" class="!px-6" :disabled="isSubmittingRating">Huỷ</BaseButton>
        <BaseButton variant="primary" class="!rounded-full shadow-lg shadow-primary/20" :disabled="isSubmittingRating" @click="submitRating">
          {{ isSubmittingRating ? 'Đang gửi...' : 'Gửi đánh giá' }}
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { store } from '../stores/eventStore'
import BaseBadge from '../components/ui/BaseBadge.vue'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseModal from '../components/ui/BaseModal.vue'
import StarRatingInput from '../components/ui/StarRatingInput.vue'
import {
  PhWallet, PhShieldCheck, PhCheckCircle, PhCalendarBlank,
  PhMapPin, PhQrCode, PhTicket, PhQuestion, PhUser,
  PhClock, PhArmchair, PhLightbulb, PhSpinner, PhStar
} from '@phosphor-icons/vue'
import { ticketService } from '../services/ticket.service'
import { submitEventRating, getMyEventRating } from '../services/rating.service'
import { getErrorMessage } from '../utils/apiError'

const activeTab = ref('upcoming')
const selectedTicket = ref(null)
const tickets = ref([])
const isLoading = ref(false)
const pageIndex = ref(1)
const pageSize = ref(10)
const totalPages = ref(1)
const totalCount = ref(0)

const tabs = [
  { id: 'upcoming', label: 'Sắp diễn ra' },
  { id: 'past', label: 'Lịch sử đã đi' },
]

const myRatings = ref({})

const fetchTickets = async () => {
  isLoading.value = true;
  try {
    // status=1 (Valid/Upcoming), status=2 (Used/Past) - tuỳ thuộc vào backend, mặc định map theo logic
    const status = activeTab.value === 'upcoming' ? 1 : 2;
    const res = await ticketService.getMyTickets({ status, pageIndex: pageIndex.value, pageSize: pageSize.value });
    if (res.success && res.data) {
      tickets.value = res.data.data || [];
      totalPages.value = res.data.totalPages || 1;
      totalCount.value = res.data.totalCount || 0;
      if (activeTab.value === 'past') {
        await fetchMyRatings(tickets.value);
      }
    }
  } catch (error) {
    console.error('Failed to fetch tickets:', error);
    store.toast = { message: getErrorMessage(error, 'Không thể tải danh sách vé. Vui lòng thử lại.'), icon: '❌' };
  } finally {
    isLoading.value = false;
  }
}

const fetchMyRatings = async (ticketList) => {
  const eventIds = [...new Set(ticketList.map(t => t.eventId))].filter(id => !(id in myRatings.value));
  await Promise.all(eventIds.map(async (eventId) => {
    try {
      const res = await getMyEventRating(eventId);
      myRatings.value[eventId] = (res && res.success) ? res.data : null;
    } catch (error) {
      console.error('Failed to fetch my rating for event', eventId, error);
    }
  }));
}

watch(activeTab, () => {
  pageIndex.value = 1;
  fetchTickets();
});

onMounted(() => {
  fetchTickets();
});

const filteredTickets = computed(() => {
  return tickets.value;
})

const myRatingForSelected = computed(() => {
  return selectedTicket.value ? myRatings.value[selectedTicket.value.eventId] : null;
})

const getStatusTag = (ticket) => {
  if (ticket.status === 'Used') {
    return { label: 'Đã sử dụng', variant: 'neutral' }
  }
  if (ticket.status === 'Cancelled') {
    return { label: 'Đã huỷ', variant: 'danger' }
  }
  const isExpired = new Date(ticket.showtimeStartAt) < new Date()
  return isExpired
    ? { label: 'Đã hết hạn', variant: 'warning' }
    : { label: 'Còn hiệu lực', variant: 'primary' }
}

const formatDate = (dateStr) => {
  if (!dateStr) return 'Gần đây'
  try {
    const d = new Date(dateStr)
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', weekday: 'short' })
  } catch(e) {
    return dateStr
  }
}

const openTicketDetail = (ticket) => {
  selectedTicket.value = ticket
}

const showRatingModal = ref(false)
const isSubmittingRating = ref(false)
const ratingForm = ref({
  soundRating: 0,
  visualRating: 0,
  organizationRating: 0,
  facilityRating: 0,
  serviceRating: 0,
  performanceRating: 0,
  comment: ''
})

const openRatingModal = () => {
  ratingForm.value = {
    soundRating: 0,
    visualRating: 0,
    organizationRating: 0,
    facilityRating: 0,
    serviceRating: 0,
    performanceRating: 0,
    comment: ''
  }
  showRatingModal.value = true
}

const submitRating = async () => {
  const { soundRating, visualRating, organizationRating, facilityRating, serviceRating, performanceRating } = ratingForm.value
  if (![soundRating, visualRating, organizationRating, facilityRating, serviceRating, performanceRating].every(r => r >= 1 && r <= 5)) {
    store.toast = { message: 'Vui lòng chấm đủ 5 sao cho tất cả các hạng mục.', icon: '❌' }
    return
  }

  isSubmittingRating.value = true
  try {
    const res = await submitEventRating(selectedTicket.value.eventId, ratingForm.value)
    if (res && res.success) {
      store.toast = { message: 'Cảm ơn bạn đã đánh giá sự kiện!', icon: '⭐' }
      myRatings.value[selectedTicket.value.eventId] = res.data
      showRatingModal.value = false
      selectedTicket.value = null
    } else {
      store.toast = { message: res?.message || 'Gửi đánh giá thất bại.', icon: '❌' }
    }
  } catch (err) {
    console.error('Failed to submit rating:', err)
    store.toast = { message: err.response?.data?.message || 'Có lỗi xảy ra khi gửi đánh giá.', icon: '❌' }
  } finally {
    isSubmittingRating.value = false
  }
}
</script>

<style scoped>
</style>
