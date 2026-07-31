<template>
  <Teleport to="body">
    <div v-if="store.user" class="fixed bottom-6 right-6 z-[900] flex flex-col items-end gap-3">
      <Transition
        enter-active-class="transition-all duration-300 ease-[cubic-bezier(0.23,1,0.32,1)]"
        enter-from-class="opacity-0 scale-95 translate-y-4"
        enter-to-class="opacity-100 scale-100 translate-y-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100 scale-100 translate-y-0"
        leave-to-class="opacity-0 scale-95 translate-y-4"
      >
        <div
          v-if="chatStore.isOpen"
          class="w-[360px] max-w-[calc(100vw-2rem)] h-[520px] max-h-[70vh] bg-[#111916] border border-white/10 rounded-[1.5rem] shadow-[0_40px_80px_rgba(0,0,0,0.8)] flex flex-col overflow-hidden"
        >
          <div class="flex items-center justify-between p-4 px-5 border-b border-white/5 bg-white/[0.02]">
            <div class="flex items-center gap-2">
              <PhChatCircleDots weight="fill" class="text-[#00C853] text-xl" />
              <span class="text-sm font-black font-heading text-white">Hỗ trợ TicketHub</span>
            </div>
            <button
              @click="closeChat"
              class="w-8 h-8 rounded-full flex items-center justify-center bg-white/5 border border-white/10 text-white/50 hover:text-white hover:bg-white/10 transition-all active:scale-90"
            >
              <PhX weight="bold" />
            </button>
          </div>

          <div ref="scrollEl" class="flex-1 overflow-y-auto p-4 space-y-3 hide-scroll">
            <div v-if="chatStore.messages.length === 0" class="text-white/40 text-sm text-center mt-8">
              Chào bạn! Mình có thể giúp gì về sự kiện, vé hoặc đơn hàng của bạn?
            </div>

            <div
              v-for="(m, idx) in chatStore.messages"
              :key="idx"
              class="flex"
              :class="m.role === 'user' ? 'justify-end' : 'justify-start'"
            >
              <div
                class="max-w-[80%] px-3 py-2 rounded-2xl text-sm whitespace-pre-line"
                :class="m.role === 'user' ? 'bg-[#00C853] text-black font-medium' : 'bg-white/5 text-white/90 border border-white/10'"
              >
                {{ m.content }}
              </div>
            </div>

            <div v-if="chatStore.loading" class="flex justify-start">
              <div class="px-3 py-2 rounded-2xl bg-white/5 border border-white/10 text-white/50 text-sm">
                <PhSpinner class="animate-spin text-lg" />
              </div>
            </div>
          </div>

          <form @submit.prevent="handleSend" class="flex items-center gap-2 p-3 border-t border-white/5 bg-white/[0.02]">
            <input
              v-model="draft"
              type="text"
              placeholder="Nhập câu hỏi của bạn..."
              class="flex-1 bg-white/5 border border-white/10 rounded-full px-4 py-2 text-sm text-white placeholder:text-white/30 focus:outline-none focus:border-[#00C853]/50"
              :disabled="chatStore.loading"
            />
            <button
              type="submit"
              :disabled="chatStore.loading || !draft.trim()"
              class="w-10 h-10 shrink-0 rounded-full flex items-center justify-center bg-[#00C853] text-black disabled:opacity-40 disabled:cursor-not-allowed hover:brightness-110 transition-all active:scale-90"
            >
              <PhPaperPlaneRight weight="fill" />
            </button>
          </form>
        </div>
      </Transition>

      <button
        @click="toggleChat"
        class="w-14 h-14 rounded-full flex items-center justify-center bg-[#00C853] text-black shadow-[0_20px_40px_rgba(0,200,83,0.4)] hover:brightness-110 transition-all active:scale-90"
      >
        <PhX v-if="chatStore.isOpen" weight="bold" class="text-2xl" />
        <PhChatCircleDots v-else weight="fill" class="text-2xl" />
      </button>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, nextTick, watch } from 'vue'
import { PhChatCircleDots, PhPaperPlaneRight, PhSpinner, PhX } from '@phosphor-icons/vue'
import { store } from '../../stores/eventStore'
import { chatStore, closeChat, sendChatMessage, toggleChat } from '../../stores/chatStore'

const draft = ref('')
const scrollEl = ref(null)

async function handleSend() {
  const message = draft.value
  draft.value = ''
  await sendChatMessage(message)
}

watch(
  () => chatStore.messages.length,
  async () => {
    await nextTick()
    if (scrollEl.value) scrollEl.value.scrollTop = scrollEl.value.scrollHeight
  }
)
</script>

<style scoped>
.hide-scroll::-webkit-scrollbar {
  display: none;
}
.hide-scroll {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
