<template>
  <div class="mb-16">
    <div v-if="events.length > 0" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-x-6 gap-y-10">
      <SearchEventCard
        v-for="event in events"
        :key="event.id"
        :event="event"
      />
    </div>

    <!-- Empty State -->
    <div v-else class="flex flex-col items-center justify-center py-20 px-5 text-center bg-[#111916]/50 border border-white/5 rounded-[32px]">
      <div class="w-24 h-24 rounded-full bg-white/5 flex items-center justify-center text-5xl text-white/20 mb-6 shadow-inner">
        <PhTicket weight="duotone" />
      </div>
      <h3 class="font-heading text-2xl font-black text-white mb-2">Không tìm thấy sự kiện nào</h3>
      <p class="text-white/50 max-w-[400px] leading-relaxed font-medium">
        Hiện tại chưa có sự kiện nào cho thể loại này. Vui lòng quay lại sau hoặc thử thay đổi bộ lọc.
      </p>
      <BaseButton variant="primary" class="mt-8" @click="$emit('reset')">
        Xóa bộ lọc
      </BaseButton>
    </div>
  </div>
</template>

<script setup>
import SearchEventCard from '../SearchEventCard.vue'
import BaseButton from '../ui/BaseButton.vue'
import { PhTicket } from '@phosphor-icons/vue'

defineProps({
  events: {
    type: Array,
    required: true
  }
})

defineEmits(['reset'])
</script>
