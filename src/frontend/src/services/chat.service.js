import api from './api/axios'
import { AI_CHAT_SEND } from './api/endpoints'

export const chatService = {
  /**
   * Gửi tin nhắn tới chatbot hỗ trợ khách hàng
   * @param {string|null} sessionId
   * @param {string} message
   * @returns {Promise<{success: boolean, message: string, data: {sessionId: string, reply: string}|null}>}
   */
  async sendMessage(sessionId, message) {
    try {
      const response = await api.post(AI_CHAT_SEND, { sessionId, message })
      return response.data
    } catch (err) {
      const errorData = err.response?.data
      return {
        success: false,
        message: errorData?.message || err.message || 'Lỗi kết nối máy chủ.',
        data: null
      }
    }
  }
}
