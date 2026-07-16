<template>
  <div class="flex flex-col gap-2 w-full">
    <label v-if="label" class="text-[12px] font-bold text-white/50 uppercase tracking-widest">
      {{ label }}
    </label>

    <div class="flex items-center gap-1.5">
      <button
        v-for="star in 5"
        :key="star"
        type="button"
        class="text-2xl transition-transform hover:scale-110 cursor-pointer"
        :class="(hoverValue || modelValue) >= star ? 'text-primary' : 'text-white/20'"
        @mouseenter="hoverValue = star"
        @mouseleave="hoverValue = 0"
        @click="$emit('update:modelValue', star)"
      >
        <PhStar :weight="(hoverValue || modelValue) >= star ? 'fill' : 'regular'" />
      </button>
    </div>

    <span v-if="error" class="text-[12px] font-medium text-danger mt-1">
      {{ error }}
    </span>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { PhStar } from '@phosphor-icons/vue'

defineProps({
  modelValue: { type: Number, default: 0 },
  label: { type: String, default: '' },
  error: String
})

defineEmits(['update:modelValue'])

const hoverValue = ref(0)
</script>
