import api from './api/axios'
import {
  NOTIFICATIONS,
  NOTIFICATION_UNREAD_COUNT,
  NOTIFICATION_MARK_READ,
  NOTIFICATION_MARK_ALL_READ,
  NOTIFICATION_DELETE,
  ADMIN_NOTIFICATION_SEND,
  ADMIN_NOTIFICATION_SENT_LIST,
  ADMIN_NOTIFICATION_STATS,
  ADMIN_NOTIFICATION_SCHEDULED,
  ADMIN_NOTIFICATION_SCHEDULED_CANCEL
} from './api/endpoints'

export const notificationService = {
  /**
   * Lấy danh sách thông báo của người dùng hiện tại
   * @param {Object} params
   * @param {boolean} [params.onlyUnread=false]
   * @param {number} [params.pageNumber=1]
   * @param {number} [params.pageSize=12]
   */
  async getNotifications({ onlyUnread = false, pageNumber = 1, pageSize = 12 } = {}) {
    const query = new URLSearchParams()
    if (onlyUnread) query.append('onlyUnread', 'true')
    query.append('pageNumber', pageNumber)
    query.append('pageSize', pageSize)

    const response = await api.get(`${NOTIFICATIONS}?${query.toString()}`)
    return response.data?.data ?? null
  },

  async getUnreadCount() {
    const response = await api.get(NOTIFICATION_UNREAD_COUNT)
    return response.data?.data ?? 0
  },

  async markAsRead(id) {
    const response = await api.patch(NOTIFICATION_MARK_READ(id))
    return response.data
  },

  async markAllAsRead() {
    const response = await api.patch(NOTIFICATION_MARK_ALL_READ)
    return response.data
  },

  async remove(id) {
    const response = await api.delete(NOTIFICATION_DELETE(id))
    return response.data
  },

  async sendAsAdmin(payload) {
    const response = await api.post(ADMIN_NOTIFICATION_SEND, payload)
    return response.data
  },

  /**
   * Lịch sử thông báo Admin đã gửi
   * @param {Object} params
   * @param {number} [params.pageNumber=1]
   * @param {number} [params.pageSize=10]
   */
  async getSentByAdmin({ pageNumber = 1, pageSize = 10 } = {}) {
    const query = new URLSearchParams()
    query.append('pageNumber', pageNumber)
    query.append('pageSize', pageSize)

    const response = await api.get(`${ADMIN_NOTIFICATION_SENT_LIST}?${query.toString()}`)
    return response.data?.data ?? null
  },

  /**
   * Thống kê thông báo trong khoảng thời gian
   * @param {Object} params
   * @param {string} [params.from] - ISO string
   * @param {string} [params.to] - ISO string
   */
  async getStats({ from, to } = {}) {
    const query = new URLSearchParams()
    if (from) query.append('from', from)
    if (to) query.append('to', to)

    const suffix = query.toString() ? `?${query.toString()}` : ''
    const response = await api.get(`${ADMIN_NOTIFICATION_STATS}${suffix}`)
    return response.data?.data ?? null
  },

  async getScheduled({ pageNumber = 1, pageSize = 10 } = {}) {
    const query = new URLSearchParams()
    query.append('pageNumber', pageNumber)
    query.append('pageSize', pageSize)

    const response = await api.get(`${ADMIN_NOTIFICATION_SCHEDULED}?${query.toString()}`)
    return response.data?.data ?? null
  },

  async cancelScheduled(id) {
    const response = await api.delete(ADMIN_NOTIFICATION_SCHEDULED_CANCEL(id))
    return response.data
  }
}
