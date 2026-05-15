<template>
  <div class="mb-16">
    <div v-if="events.length > 0" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 xl:grid-cols-5 gap-6">
      <EventCard 
        v-for="event in events" 
        :key="event.id" 
        :event="{ ...event, category: categoryLabel }" 
      />
    </div>
    
    <!-- Empty State -->
    <div v-else class="flex flex-col items-center justify-center py-20 px-5 text-center bg-surface border border-dashed border-border-main rounded-[32px]">
      <div class="text-[60px] mb-4 opacity-50 grayscale select-none">{{ categoryIcon }}</div>
      <h3 class="font-heading text-xl font-bold text-white mb-2">Không tìm thấy sự kiện nào</h3>
      <p class="text-muted max-w-[400px] leading-relaxed">
        Hiện tại chưa có sự kiện nào cho thể loại này. Vui lòng quay lại sau hoặc thử thay đổi bộ lọc.
      </p>
      <BaseButton variant="primary" class="mt-6" @click="$emit('reset')">
        Xóa bộ lọc
      </BaseButton>
    </div>
  </div>
</template>

<script setup>
import EventCard from '../../components/EventCard.vue'
import BaseButton from '../../components/ui/BaseButton.vue'

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
