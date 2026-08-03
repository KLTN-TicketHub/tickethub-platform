import api from './api/axios'
import {
  NOTIFICATIONS,
  NOTIFICATION_UNREAD_COUNT,
  NOTIFICATION_MARK_READ,
  NOTIFICATION_MARK_ALL_READ,
  NOTIFICATION_DELETE,
  ADMIN_NOTIFICATION_SEND
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
  }
}
