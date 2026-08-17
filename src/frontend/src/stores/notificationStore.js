import { reactive } from 'vue'
import { HubConnectionBuilder } from '@microsoft/signalr'
import { API_BASE_URL } from '../services/api/axios'
import { getToken, isLoggedIn } from '../services/auth/token.service'
import { notificationService } from '../services/notification.service'
import { getErrorMessage } from '../utils/apiError'
import { showToast } from '../utils/roleToast'

const PAGE_SIZE = 12
const TOAST_AUTO_DISMISS_MS = 6000

export const notificationStore = reactive({
  items: [],
  unreadCount: 0,
  filter: 'all', // 'all' | 'unread'
  pageNumber: 0,
  hasMore: true,
  isLoading: false,
  isConnected: false,
  toastQueue: []
})

export function dismissToast(toastId) {
  notificationStore.toastQueue = notificationStore.toastQueue.filter(t => t.toastId !== toastId)
}

let hubConnection = null
let initialized = false

// ─── HUB ─────────────────────────────────────────────────────────────────────

const getHubUrl = () => API_BASE_URL.replace('/api/v1', '/hubs/notifications')

export async function connectHub() {
  if (hubConnection || !getToken()) return

  hubConnection = new HubConnectionBuilder()
    .withUrl(getHubUrl(), {
      accessTokenFactory: () => getToken()
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .build()

  hubConnection.on('ReceiveNotification', (notification) => {
    if (!notification) return
    if (notificationStore.items.some(item => item.id === notification.id)) return

    notificationStore.items.unshift(notification)
    if (!notification.isRead) {
      notificationStore.unreadCount += 1
    }

    const toastId = `${notification.id}-${Date.now()}`
    notificationStore.toastQueue.push({ ...notification, toastId })
    setTimeout(() => dismissToast(toastId), TOAST_AUTO_DISMISS_MS)
  })

  hubConnection.onreconnected(() => {
    notificationStore.isConnected = true
    refreshUnreadCount()
  })

  hubConnection.onclose(() => {
    notificationStore.isConnected = false
  })

  try {
    await hubConnection.start()
    notificationStore.isConnected = true
  } catch (err) {
    console.error('[Notification] Không thể kết nối hub:', err)
    hubConnection = null
  }
}

export async function disconnectHub() {
  if (!hubConnection) return

  try {
    await hubConnection.stop()
  } catch (err) {
    console.error('[Notification] Lỗi khi đóng kết nối hub:', err)
  } finally {
    hubConnection = null
    notificationStore.isConnected = false
  }
}

// ─── LOADING ─────────────────────────────────────────────────────────────────

export async function initNotifications() {
  if (initialized || !isLoggedIn()) return
  initialized = true

  await Promise.all([loadMore(), refreshUnreadCount()])
  await connectHub()
}

export async function loadMore() {
  if (notificationStore.isLoading || !notificationStore.hasMore) return

  notificationStore.isLoading = true
  try {
    const nextPage = notificationStore.pageNumber + 1
    const result = await notificationService.getNotifications({
      onlyUnread: notificationStore.filter === 'unread',
      pageNumber: nextPage,
      pageSize: PAGE_SIZE
    })

    const data = result?.data ?? []
    const existingIds = new Set(notificationStore.items.map(item => item.id))

    notificationStore.items.push(...data.filter(item => !existingIds.has(item.id)))
    notificationStore.pageNumber = nextPage
    notificationStore.hasMore = result ? nextPage < result.totalPages : false
  } catch (err) {
    console.error('[Notification] Không thể tải danh sách thông báo:', err)
    showToast(getErrorMessage(err, 'Không thể tải danh sách thông báo.'))
    notificationStore.hasMore = false
  } finally {
    notificationStore.isLoading = false
  }
}

export async function setFilter(filter) {
  if (notificationStore.filter === filter) return

  notificationStore.filter = filter
  notificationStore.items = []
  notificationStore.pageNumber = 0
  notificationStore.hasMore = true

  await loadMore()
}

export async function refreshUnreadCount() {
  try {
    notificationStore.unreadCount = await notificationService.getUnreadCount()
  } catch (err) {
    console.error('[Notification] Không thể lấy số thông báo chưa đọc:', err)
  }
}

// ─── ACTIONS ─────────────────────────────────────────────────────────────────

export async function markRead(id) {
  const notification = notificationStore.items.find(item => item.id === id)
  if (!notification || notification.isRead) return

  notification.isRead = true
  notificationStore.unreadCount = Math.max(0, notificationStore.unreadCount - 1)

  try {
    await notificationService.markAsRead(id)
    if (notificationStore.filter === 'unread') {
      notificationStore.items = notificationStore.items.filter(item => item.id !== id)
    }
  } catch (err) {
    notification.isRead = false
    notificationStore.unreadCount += 1
    console.error('[Notification] Không thể đánh dấu đã đọc:', err)
    showToast(getErrorMessage(err, 'Không thể đánh dấu thông báo đã đọc.'))
  }
}

export async function markAllRead() {
  const previousItemsReadState = notificationStore.items.map(item => item.isRead)
  const previousUnreadCount = notificationStore.unreadCount

  try {
    await notificationService.markAllAsRead()
    notificationStore.items.forEach(item => { item.isRead = true })
    notificationStore.unreadCount = 0

    if (notificationStore.filter === 'unread') {
      notificationStore.items = []
      notificationStore.pageNumber = 0
      notificationStore.hasMore = false
    }
  } catch (err) {
    notificationStore.items.forEach((item, index) => { item.isRead = previousItemsReadState[index] })
    notificationStore.unreadCount = previousUnreadCount
    console.error('[Notification] Không thể đánh dấu tất cả đã đọc:', err)
    showToast(getErrorMessage(err, 'Không thể đánh dấu tất cả thông báo đã đọc.'))
  }
}

export async function removeNotification(id) {
  const notification = notificationStore.items.find(item => item.id === id)
  if (!notification) return

  try {
    await notificationService.remove(id)
    notificationStore.items = notificationStore.items.filter(item => item.id !== id)
    if (!notification.isRead) {
      notificationStore.unreadCount = Math.max(0, notificationStore.unreadCount - 1)
    }
  } catch (err) {
    console.error('[Notification] Không thể xoá thông báo:', err)
    showToast(getErrorMessage(err, 'Không thể xoá thông báo.'))
  }
}

export async function resetNotifications() {
  await disconnectHub()
  initialized = false
  notificationStore.items = []
  notificationStore.unreadCount = 0
  notificationStore.filter = 'all'
  notificationStore.pageNumber = 0
  notificationStore.hasMore = true
  notificationStore.isLoading = false
  notificationStore.toastQueue = []
}
