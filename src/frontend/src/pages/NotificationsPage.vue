<template>
  <div class="max-w-3xl mx-auto py-16 px-6 min-h-[80vh]">
    <!-- HEADER -->
    <header class="flex flex-col md:flex-row md:items-end justify-between gap-6 mb-12 animate-fade-up">
      <div class="space-y-4">
        <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-black tracking-widest uppercase shadow-[0_0_20px_rgba(0,200,83,0.15)]">
          <PhBellRinging weight="fill" /> Hộp thư
        </div>
        <h1 class="text-5xl font-black font-heading text-white tracking-tight uppercase">Thông báo</h1>
        <p class="text-lg text-white/50 font-medium max-w-md">
          Tất cả cập nhật về đơn hàng, vé và sự kiện của bạn ở một nơi.
        </p>
      </div>

      <button
        v-if="notificationStore.unreadCount > 0"
        @click="markAllRead"
        class="self-start md:self-auto px-5 py-2.5 rounded-full text-[13px] font-bold text-black bg-primary hover:bg-primary-dark transition-all active:scale-95 shadow-[0_0_20px_rgba(0,200,83,0.2)] flex items-center gap-2"
      >
        <PhChecks weight="bold" /> Đánh dấu tất cả đã đọc
      </button>
    </header>

    <!-- TABS -->
    <div class="flex gap-8 border-b border-white/10 mb-8 animate-fade-up [animation-delay:100ms]">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        @click="setFilter(tab.id)"
        class="pb-4 text-[15px] font-bold transition-all relative cursor-pointer flex items-center gap-2"
        :class="[notificationStore.filter === tab.id ? 'text-primary' : 'text-white/50 hover:text-white']"
      >
        {{ tab.label }}
        <span
          v-if="tab.id === 'unread' && notificationStore.unreadCount > 0"
          class="px-2 py-0.5 rounded-full bg-primary/15 text-primary text-[10px] font-black"
        >
          {{ notificationStore.unreadCount }}
        </span>
        <div
          v-if="notificationStore.filter === tab.id"
          class="absolute bottom-0 left-0 right-0 h-1 bg-primary rounded-full shadow-[0_0_10px_rgba(0,200,83,0.5)]"
        ></div>
      </button>
    </div>

    <!-- LIST -->
    <div class="space-y-3 animate-fade-up [animation-delay:200ms]">
      <article
        v-for="item in notificationStore.items"
        :key="item.id"
        @click="openNotification(item)"
        class="group relative flex gap-4 p-5 bg-[#111916] border rounded-2xl transition-all duration-300 cursor-pointer hover:-translate-y-0.5 hover:border-primary/30"
        :class="item.isRead ? 'border-white/5' : 'border-primary/20 bg-primary/[0.04]'"
      >
        <div
          class="w-11 h-11 flex-shrink-0 rounded-xl border flex items-center justify-center"
          :class="getNotificationPreset(item.type).tone"
        >
          <component :is="getNotificationPreset(item.type).icon" weight="fill" class="text-xl" />
        </div>

        <div class="flex-1 min-w-0">
          <div class="flex items-start gap-3">
            <h2 class="flex-1 text-[15px] font-bold leading-snug" :class="item.isRead ? 'text-white/70' : 'text-white'">
              {{ item.title }}
            </h2>
            <span v-if="!item.isRead" class="mt-2 w-2 h-2 rounded-full bg-primary flex-shrink-0 shadow-[0_0_8px_rgba(0,200,83,0.6)]"></span>
          </div>
          <p class="text-[13px] text-muted leading-relaxed mt-1.5">{{ item.message }}</p>
          <span class="text-[11px] text-white/30 font-medium mt-2 block">{{ formatRelativeTime(item.createdAt) }}</span>
        </div>

        <button
          @click.stop="removeNotification(item.id)"
          class="absolute top-4 right-4 w-8 h-8 rounded-lg flex items-center justify-center text-white/20 opacity-0 group-hover:opacity-100 hover:text-danger hover:bg-danger/10 transition-all"
          title="Xoá thông báo"
        >
          <PhTrash weight="bold" />
        </button>
      </article>

      <!-- SKELETON -->
      <div
        v-for="n in (notificationStore.isLoading ? 3 : 0)"
        :key="`skeleton-${n}`"
        class="flex gap-4 p-5 bg-[#111916] border border-white/5 rounded-2xl animate-pulse"
      >
        <div class="w-11 h-11 rounded-xl bg-white/5"></div>
        <div class="flex-1 space-y-3 py-1">
          <div class="h-3.5 w-1/3 rounded bg-white/5"></div>
          <div class="h-3 w-3/4 rounded bg-white/5"></div>
          <div class="h-2.5 w-20 rounded bg-white/5"></div>
        </div>
      </div>

      <!-- EMPTY -->
      <div
        v-if="!notificationStore.isLoading && notificationStore.items.length === 0"
        class="py-24 text-center"
      >
        <PhBellSlash class="text-5xl text-white/15 mx-auto mb-5" weight="fill" />
        <p class="text-[16px] font-bold text-white/60">
          {{ notificationStore.filter === 'unread' ? 'Bạn đã đọc hết thông báo.' : 'Bạn chưa có thông báo nào.' }}
        </p>
        <p class="text-[13px] text-muted mt-2">Các cập nhật về đơn hàng và sự kiện sẽ xuất hiện tại đây.</p>
      </div>

      <!-- INFINITE SCROLL SENTINEL -->
      <div ref="sentinelRef" class="h-px"></div>

      <p
        v-if="!notificationStore.hasMore && notificationStore.items.length > 0"
        class="py-8 text-center text-[12px] text-white/25 font-medium uppercase tracking-widest"
      >
        Đã hiển thị toàn bộ thông báo
      </p>
    </div>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { PhBellRinging, PhBellSlash, PhChecks, PhTrash } from '@phosphor-icons/vue'
import {
  notificationStore,
  initNotifications,
  loadMore,
  markAllRead,
  markRead,
  removeNotification,
  setFilter
} from '../stores/notificationStore'
import { formatRelativeTime, getNotificationPreset } from '../utils/notificationDisplay'

const router = useRouter()

const sentinelRef = ref(null)
let observer = null

const tabs = [
  { id: 'all', label: 'Tất cả' },
  { id: 'unread', label: 'Chưa đọc' }
]

const openNotification = async (item) => {
  await markRead(item.id)
  if (item.linkUrl) router.push(item.linkUrl)
}

onMounted(async () => {
  await initNotifications()

  observer = new IntersectionObserver((entries) => {
    if (entries[0].isIntersecting) loadMore()
  }, { rootMargin: '200px' })

  if (sentinelRef.value) observer.observe(sentinelRef.value)
})

onUnmounted(() => {
  observer?.disconnect()
})
</script>
