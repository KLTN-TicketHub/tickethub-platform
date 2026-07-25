<template>
  <div class="bg-card border border-border-main rounded-2xl p-5 flex flex-col gap-2">
    <div class="flex items-center justify-between gap-3">
      <h3 class="font-heading font-black text-main text-[15px] leading-snug">{{ title }}</h3>
      <BaseBadge :variant="categoryVariant" size="sm">{{ categoryLabel }}</BaseBadge>
    </div>
    <p class="text-muted text-[13px] leading-relaxed">{{ detail }}</p>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import BaseBadge from '../ui/BaseBadge.vue'

const props = defineProps({
  category: {
    type: String,
    default: 'general'
  },
  title: {
    type: String,
    required: true
  },
  detail: {
    type: String,
    default: ''
  }
})

const CATEGORY_META = {
  'market-direction': { label: 'Định hướng thị trường', variant: 'primary' },
  marketing: { label: 'Marketing', variant: 'success' },
  quality: { label: 'Chất lượng', variant: 'warning' },
  service: { label: 'Dịch vụ', variant: 'neutral' },
  general: { label: 'Gợi ý', variant: 'neutral' }
}

const categoryLabel = computed(() => CATEGORY_META[props.category]?.label || CATEGORY_META.general.label)
const categoryVariant = computed(() => CATEGORY_META[props.category]?.variant || CATEGORY_META.general.variant)
</script>
