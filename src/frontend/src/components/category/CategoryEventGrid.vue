<template>
  <div class="mb-16">
    <div v-if="events.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 auto-rows-[450px]">
      <div 
        v-for="(event, idx) in events" 
        :key="event.id"
        :class="{
          'lg:col-span-2 lg:row-span-2': idx % 7 === 0,
          'md:col-span-2': idx % 7 === 3
        }"
        class="h-full animate-fade-up"
        :style="`animation-delay: ${(idx % 7) * 50}ms`"
      >
        <EventCard 
          :event="{ ...event, category: categoryLabel }" 
          class="h-full w-full"
        />
      </div>
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
import EventCard from '../../components/EventCard.vue'
import BaseButton from '../../components/ui/BaseButton.vue'
import { PhTicket } from '@phosphor-icons/vue'

defineProps({
  events: {
    type: Array,
    required: true
  },
  categoryLabel: {
    type: String,
    required: true
  },
  categoryIcon: {
    type: String,
    default: '🎫'
  }
})

defineEmits(['reset'])
</script>
