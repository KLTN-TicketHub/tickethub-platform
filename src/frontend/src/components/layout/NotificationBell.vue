<template>
  <div class="relative" ref="rootRef">
    <button
      @click.stop="toggleDropdown"
      class="relative w-10 h-10 flex items-center justify-center rounded-full bg-white/5 border border-white/10 text-white hover:bg-white/10 hover:border-primary/40 transition-all group active:scale-95"
      :class="{ '!border-primary/50 !bg-white/10': showDropdown }"
    >
      <component
        :is="notificationStore.unreadCount > 0 ? PhBellRinging : PhBell"
        class="text-lg group-hover:scale-110 transition-transform"
        weight="fill"
      />
      <span
        v-if="notificationStore.unreadCount > 0"
        class="absolute -top-1 -right-1 min-w-[16px] h-4 px-1 bg-primary text-black text-[10px] font-black rounded-full flex items-center justify-center border-2 border-[#0A0F0D]"
      >
        {{ badgeLabel }}
      </span>
    </button>

    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0 translate-y-4"
      enter-to-class="opacity-100 translate-y-0"
    >
      <div
        v-if="showDropdown"
        class="absolute top-[calc(100%+12px)] right-0 w-[360px] max-w-[calc(100vw-32px)] bg-[#0A0F0D]/98 backdrop-blur-3xl border border-white/10 rounded-2xl shadow-[0_40px_80px_rgba(0,0,0,0.8)] overflow-hidden z-[1001]"
      >
        <!-- Header -->
        <div class="flex items-center justify-between gap-3 px-4 py-3 border-b border-white/10">
          <div class="flex items-center gap-2">
            <span class="text-[14px] font-black text-white">Thông báo</span>
            <span
              v-if="notificationStore.unreadCount > 0"
              class="px-2 py-0.5 rounded-full bg-primary/15 text-primary text-[10px] font-black"
            >
              {{ notificationStore.unreadCount }} mới
            </span>
          </div>
          <button
            v-if="notificationStore.unreadCount > 0"
            @click.stop="markAllRead"
            class="text-[11px] font-bold text-primary hover:text-white transition-colors"
          >
            Đánh dấu đã đọc
          </button>
        </div>

        <!-- List -->
        <div
          ref="listRef"
          @scroll.passive="handleScroll"
          class="max-h-[420px] overflow-y-auto overscroll-contain"
        >
          <button
            v-for="item in notificationStore.items"
            :key="item.id"
            @click.stop="openNotification(item)"
            class="w-full text-left flex gap-3 px-4 py-3 border-b border-white/5 transition-colors hover:bg-white/5"
            :class="{ 'bg-primary/[0.06]': !item.isRead }"
          >
            <div
              class="w-9 h-9 flex-shrink-0 rounded-xl border flex items-center justify-center"
              :class="getNotificationPreset(item.type).tone"
            >
              <component :is="getNotificationPreset(item.type).icon" weight="fill" class="text-lg" />
            </div>
            <div class="flex-1 min-w-0">
              <div class="flex items-start gap-2">
                <span class="flex-1 text-[13px] font-bold leading-snug" :class="item.isRead ? 'text-white/70' : 'text-white'">
                  {{ item.title }}
                </span>
                <span v-if="!item.isRead" class="mt-1.5 w-2 h-2 rounded-full bg-primary flex-shrink-0"></span>
              </div>
              <p class="text-[12px] text-muted leading-relaxed mt-1 line-clamp-2">{{ item.message }}</p>
              <span class="text-[11px] text-white/30 font-medium mt-1.5 block">{{ formatRelativeTime(item.createdAt) }}</span>
            </div>
          </button>

          <div v-if="notificationStore.isLoading" class="px-4 py-4 text-center text-[12px] text-muted font-medium">
            Đang tải...
          </div>

          <div
            v-else-if="notificationStore.items.length === 0"
            class="px-6 py-12 text-center"
          >
            <PhBellSlash class="text-3xl text-white/20 mx-auto mb-3" weight="fill" />
            <p class="text-[13px] text-muted italic">Bạn chưa có thông báo nào.</p>
          </div>
        </div>

        <!-- Footer -->
        <button
          @click.stop="goToInbox"
          class="w-full px-4 py-3 text-[12px] font-bold text-primary hover:bg-white/5 transition-colors border-t border-white/10"
        >
          Xem tất cả thông báo
        </button>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { PhBell, PhBellRinging, PhBellSlash } from '@phosphor-icons/vue'
import {
  notificationStore,
  initNotifications,
  loadMore,
  markAllRead,
  markRead
} from '../../stores/notificationStore'
import { formatRelativeTime, getNotificationPreset } from '../../utils/notificationDisplay'

const route = useRoute()
const router = useRouter()

const rootRef = ref(null)
const listRef = ref(null)
const showDropdown = ref(false)

const badgeLabel = computed(() =>
  notificationStore.unreadCount > 99 ? '99+' : notificationStore.unreadCount
)

// Mỗi phân vùng route có hộp thư riêng vì router chặn user đi lạc sang phân vùng khác
const inboxPath = computed(() => {
  if (route.path.startsWith('/admin')) return '/admin/notifications'
  if (route.path.startsWith('/moderator')) return '/moderator/notifications'
  if (route.path.startsWith('/organizer')) return '/organizer/notifications'
  if (route.path.startsWith('/staff')) return '/staff/notifications'
  return '/notifications'
})

const toggleDropdown = () => {
  showDropdown.value = !showDropdown.value
}

const handleScroll = () => {
  const el = listRef.value
  if (!el) return

  const reachedBottom = el.scrollTop + el.clientHeight >= el.scrollHeight - 80
  if (reachedBottom) loadMore()
}

const openNotification = async (item) => {
  await markRead(item.id)
  showDropdown.value = false
  if (item.linkUrl) router.push(item.linkUrl)
}

const goToInbox = () => {
  showDropdown.value = false
  router.push(inboxPath.value)
}

const handleClickOutside = (event) => {
  if (!rootRef.value?.contains(event.target)) showDropdown.value = false
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  initNotifications()
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>
