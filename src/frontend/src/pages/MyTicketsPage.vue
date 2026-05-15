<template>
  <div class="max-w-6xl mx-auto py-16 px-6 min-h-[80vh]">
    <!-- PREMIUM HEADER -->
    <header class="flex flex-col md:flex-row md:items-end justify-between gap-8 mb-16 animate-fade-up">
      <div class="space-y-4">
        <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-bold tracking-widest uppercase">
          🎟️ Ví của tôi
        </div>
        <h1 class="text-5xl font-bold font-heading text-main tracking-tight">Vé & Thẻ của bạn</h1>
        <p class="text-lg text-muted font-medium max-w-md">Tất cả những khoảnh khắc tuyệt vời đã sẵn sàng để bắt đầu.</p>
      </div>
      
      <div class="flex items-center gap-4">
        <div class="flex flex-col items-end">
          <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Trạng thái ví</span>
          <span class="text-[15px] font-bold text-main">Đã xác thực 🔐</span>
        </div>
        <div class="h-10 w-px bg-border-main hidden md:block"></div>
        <div class="bg-card border border-border-main rounded-2xl p-3 px-6 flex items-center gap-4">
          <div class="flex flex-col">
            <span class="text-[11px] font-bold text-muted uppercase">Tổng số vé</span>
            <span class="text-xl font-bold text-primary">{{ store.tickets.length }}</span>
          </div>
        </div>
      </div>
    </header>

    <!-- TABS -->
    <div class="flex gap-8 border-b border-border-main mb-12 animate-fade-up [animation-delay:100ms]">
      <button 
        v-for="t in tabs" 
        :key="t.id"
        @click="activeTab = t.id"
        class="pb-4 text-[15px] font-bold transition-all relative"
        :class="[activeTab === t.id ? 'text-primary' : 'text-muted hover:text-main']"
      >
        {{ t.label }}
        <div v-if="activeTab === t.id" class="absolute bottom-0 left-0 right-0 h-1 bg-primary rounded-full animate-in slide-in-from-left-2 duration-300"></div>
      </button>
    </div>

    <!-- TICKET WALLET LIST -->
    <div v-if="filteredTickets.length > 0" class="grid grid-cols-1 gap-10 animate-fade-up [animation-delay:200ms]">
      <div 
        v-for="(ticket, idx) in filteredTickets" 
        :key="ticket.id" 
        class="group relative flex flex-col lg:flex-row bg-surface border border-border-main rounded-[2.5rem] overflow-hidden hover:border-primary/50 transition-all duration-500 shadow-2xl hover:shadow-primary/10 cursor-pointer"
        :style="`animation-delay: ${idx * 100}ms`"
        @click="openTicketDetail(ticket)"
      >
        <!-- Hologram Effect Layer -->
        <div class="absolute inset-0 pointer-events-none opacity-0 group-hover:opacity-10 bg-[radial-gradient(circle_at_var(--x,50%)_var(--y,50%),rgba(0,200,83,0.5),transparent_50%)] transition-opacity duration-700"></div>

        <!-- Left: Poster Area -->
        <div class="lg:w-80 h-64 lg:h-auto overflow-hidden relative">
          <img :src="ticket.event?.image" :alt="ticket.event?.title" class="w-full h-full object-cover grayscale-[0.3] group-hover:grayscale-0 transition-all duration-700" />
          <div class="absolute inset-0 bg-gradient-to-t from-bg via-transparent to-transparent"></div>
          
          <div class="absolute top-6 left-6 flex flex-col gap-2">
            <BaseBadge variant="primary" class="!bg-primary !text-black font-black uppercase tracking-tighter">
              {{ ticket.tier }}
            </BaseBadge>
            <div class="px-3 py-1 rounded-lg bg-black/60 backdrop-blur-md border border-white/10 text-[10px] text-white font-bold tracking-widest uppercase">
              CONFIRMED
            </div>
          </div>
        </div>

        <!-- Middle: Information Layer -->
        <div class="flex-1 p-8 lg:p-12 flex flex-col justify-between border-b lg:border-b-0 lg:border-r border-dashed border-border-main relative bg-card/40">
          <!-- Ticket Notches -->
          <div class="hidden lg:block absolute -right-[16px] -top-[16px] w-8 h-8 rounded-full bg-bg border border-border-main z-10"></div>
          <div class="hidden lg:block absolute -right-[16px] -bottom-[16px] w-8 h-8 rounded-full bg-bg border border-border-main z-10"></div>

          <div class="space-y-8">
            <div class="space-y-3">
              <div class="flex items-center gap-3 text-primary">
                <span class="text-xl">📅</span>
                <span class="text-[13px] font-bold uppercase tracking-[0.2em]">{{ formatDate(ticket.event?.dateStart) }}</span>
              </div>
              <h2 class="text-3xl font-bold font-heading text-main leading-tight group-hover:text-primary transition-colors duration-500">
                {{ ticket.event?.title }}
              </h2>
            </div>
            
            <div class="grid grid-cols-2 md:grid-cols-4 gap-8">
              <div class="flex flex-col">
                <span class="text-[10px] font-bold uppercase tracking-widest text-muted mb-1">Mã xác nhận</span>
                <span class="font-mono text-[16px] font-bold text-main tracking-widest">{{ ticket.code }}</span>
              </div>
              <div class="flex flex-col">
                <span class="text-[10px] font-bold uppercase tracking-widest text-muted mb-1">Cổng vào</span>
                <span class="text-[16px] font-bold text-main">Gate {{ ticket.gate || 'A1' }}</span>
              </div>
              <div class="flex flex-col">
                <span class="text-[10px] font-bold uppercase tracking-widest text-muted mb-1">Số lượng</span>
                <span class="text-[16px] font-bold text-main">x{{ ticket.qty }}</span>
              </div>
              <div class="flex flex-col">
                <span class="text-[10px] font-bold uppercase tracking-widest text-muted mb-1">Chỗ ngồi</span>
                <span class="text-[16px] font-bold text-primary truncate">
                  {{ ticket.seats?.length ? ticket.seats.map(s => s.id).join(', ') : 'Tự do' }}
                </span>
              </div>
            </div>
          </div>

          <div class="mt-10 flex items-center gap-4 text-muted border-t border-white/5 pt-6">
            <div class="w-10 h-10 rounded-xl bg-surface border border-border-main flex items-center justify-center text-xl">📍</div>
            <div class="flex flex-col">
              <span class="text-[11px] font-bold uppercase tracking-widest text-white/40">Địa điểm</span>
              <span class="font-medium text-main">{{ ticket.event?.location?.name || 'VNDiamond Arena' }}</span>
            </div>
          </div>
        </div>

        <!-- Right: Stub / Scannable -->
        <div class="w-full lg:w-72 p-10 lg:p-12 flex flex-col items-center justify-center gap-8 bg-surface/50 group/stub">
          <div class="relative w-32 h-32 lg:w-40 lg:h-40 bg-white rounded-[2rem] p-4 shadow-2xl group-hover/stub:scale-105 transition-transform duration-700">
            <!-- Mock QR with CSS Patterns -->
            <div class="w-full h-full bg-[repeating-linear-gradient(45deg,#000_0,#000_2px,transparent_2px,transparent_6px),repeating-linear-gradient(-45deg,#000_0,#000_2px,transparent_2px,transparent_6px)] opacity-80"></div>
            <div class="absolute inset-0 flex items-center justify-center">
              <div class="bg-white p-2 rounded-lg text-[10px] font-bold shadow-xl border border-black/5">ES-WALLET</div>
            </div>
          </div>
          
          <div class="space-y-4 w-full">
            <BaseButton variant="primary" size="md" class="w-full !rounded-2xl !bg-white !text-black hover:!bg-primary transition-colors">
              Mở QR Code
            </BaseButton>
            <div class="flex items-center justify-center gap-2 text-[10px] font-bold text-muted uppercase tracking-[0.2em]">
              <span class="animate-pulse">●</span> Scannable Ticket
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- EMPTY STATE -->
    <div v-else class="flex flex-col items-center justify-center py-32 text-center animate-fade-up">
      <div class="w-40 h-40 bg-card border border-border-main rounded-[3rem] flex items-center justify-center text-7xl mb-10 shadow-[0_30px_60px_-15px_rgba(0,0,0,0.5)] relative overflow-hidden group">
        <div class="absolute inset-0 bg-primary/10 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        🎟️
        <div class="absolute -right-2 -bottom-2 w-14 h-14 bg-surface border border-border-main rounded-full flex items-center justify-center text-2xl shadow-xl">❓</div>
      </div>
      <h3 class="text-3xl font-bold font-heading text-main mb-4">Ví của bạn đang trống</h3>
      <p class="text-lg text-muted max-w-sm mb-12 font-medium">Đừng để lỡ mất những trải nghiệm tuyệt vời đang chờ đón bạn.</p>
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
      :title="selectedTicket?.event?.title"
      subtitle="Thông tin vé điện tử chi tiết"
      size="lg"
    >
      <div v-if="selectedTicket" class="flex flex-col gap-8 py-4">
        <div class="relative rounded-[2rem] overflow-hidden h-48 border border-white/10">
          <img :src="selectedTicket.event?.image" class="w-full h-full object-cover opacity-50" />
          <div class="absolute inset-0 bg-gradient-to-t from-bg to-transparent"></div>
          <div class="absolute bottom-6 left-6">
            <h3 class="text-3xl font-bold text-white">{{ selectedTicket.event?.title }}</h3>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-10">
          <div class="space-y-6">
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Khán giả</span>
              <p class="text-lg font-bold text-main">{{ store.user?.name || 'Khách hàng' }}</p>
            </div>
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Loại vé</span>
              <p class="text-lg font-bold text-primary">{{ selectedTicket.tier }}</p>
            </div>
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Ngày giờ</span>
              <p class="text-lg font-bold text-main">{{ formatDate(selectedTicket.event?.dateStart) }} · {{ selectedTicket.event?.time || '19:00' }}</p>
            </div>
            <div class="space-y-1">
              <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Vị trí chỗ ngồi</span>
              <p class="text-lg font-bold text-main">{{ selectedTicket.seats?.length ? selectedTicket.seats.map(s => s.id).join(', ') : 'Khu vực đứng (Free)' }}</p>
            </div>
          </div>

          <div class="flex flex-col items-center justify-center p-8 bg-surface rounded-[2rem] border border-border-main shadow-inner">
            <div class="w-48 h-48 bg-white p-4 rounded-3xl shadow-2xl mb-6">
               <div class="w-full h-full bg-[repeating-linear-gradient(45deg,#000_0,#000_2px,transparent_2px,transparent_6px),repeating-linear-gradient(-45deg,#000_0,#000_2px,transparent_2px,transparent_6px)] opacity-80"></div>
            </div>
            <p class="text-[12px] font-mono font-bold text-main tracking-[0.3em] uppercase">{{ selectedTicket.code }}</p>
            <p class="text-[11px] font-bold text-muted uppercase mt-2">Mã QR chính chủ</p>
          </div>
        </div>

        <div class="p-6 bg-primary/10 border border-primary/20 rounded-2xl flex items-center gap-4">
          <span class="text-2xl">💡</span>
          <p class="text-[13px] font-medium text-main leading-relaxed">
            Vui lòng xuất trình mã QR này tại cổng kiểm soát để vào sự kiện. Không chia sẻ mã này với người khác.
          </p>
        </div>
      </div>

      <template #footer>
        <BaseButton variant="outline" @click="selectedTicket = null">Đóng</BaseButton>
        <BaseButton variant="primary" class="!rounded-xl">Lưu về Apple/Google Wallet</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { store } from '../stores/eventStore'
import BaseBadge from '../components/ui/BaseBadge.vue'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseModal from '../components/ui/BaseModal.vue'

const activeTab = ref('upcoming')
const selectedTicket = ref(null)

const tabs = [
  { id: 'upcoming', label: 'Sắp diễn ra' },
  { id: 'past', label: 'Lịch sử đã đi' },
]

const filteredTickets = computed(() => {
  if (activeTab.value === 'upcoming') {
    return store.tickets
  }
  return []
})

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
</script>

<style scoped>
.group:hover {
  --x: 50%;
  --y: 50%;
}
</style>
