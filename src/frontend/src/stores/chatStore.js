import { reactive } from 'vue'
import { chatService } from '../services/chat.service'

export const chatStore = reactive({
  isOpen: false,
  loading: false,
  sessionId: null,
  messages: [] // { role: 'user' | 'assistant', content: string }
})

export function toggleChat() {
  chatStore.isOpen = !chatStore.isOpen
}

export function closeChat() {
  chatStore.isOpen = false
}

export async function sendChatMessage(message) {
  const trimmed = message.trim()
  if (!trimmed || chatStore.loading) return

  chatStore.messages.push({ role: 'user', content: trimmed })
  chatStore.loading = true

  try {
    const result = await chatService.sendMessage(chatStore.sessionId, trimmed)

    if (!result.success) {
      chatStore.messages.push({ role: 'assistant', content: result.message || 'Đã có lỗi xảy ra, vui lòng thử lại sau.' })
      return
    }

    chatStore.sessionId = result.data.sessionId
    chatStore.messages.push({ role: 'assistant', content: result.data.reply })
  } finally {
    chatStore.loading = false
  }
}
