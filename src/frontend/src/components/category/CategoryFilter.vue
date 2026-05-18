<template>
  <div class="flex flex-col gap-4 mb-6">
    <!-- Filter Bar -->
    <div class="flex flex-wrap items-center gap-4 bg-card border border-border-main rounded-2xl p-4 md:px-6">
      <div class="flex items-center gap-2">
        <span class="text-[13px] font-bold text-muted uppercase tracking-wider whitespace-nowrap">Lọc theo:</span>
        <div class="flex flex-wrap gap-2">
          <button 
            v-for="filter in availableFilters" 
            :key="filter"
            @click="$emit('update:modelValue', filter)"
            class="px-4 py-1.5 rounded-full text-[13px] font-semibold transition-all duration-200 border"
            :class="[
              modelValue === filter 
                ? 'bg-primary/10 border-primary text-primary' 
                : 'bg-transparent border-border-main text-muted hover:border-primary hover:text-primary'
            ]"
          >
            {{ filter }}
          </button>
        </div>
      </div>
      
      <div class="ml-auto flex flex-wrap items-center gap-4">
        <div class="flex items-center gap-2">
          <span class="text-[13px] font-bold text-muted uppercase tracking-wider whitespace-nowrap">Sắp xếp:</span>
          <BaseSelect 
            :modelValue="sortBy" 
            @update:modelValue="$emit('update:sortBy', $event)"
            class="min-w-[140px]"
          >
            <option value="newest">Mới nhất</option>
            <option value="oldest">Cũ nhất</option>
            <option value="price-asc">Giá tăng dần</option>
            <option value="price-desc">Giá giảm dần</option>
            <option value="name-asc">Tên A → Z</option>
            <option value="name-desc">Tên Z → A</option>
          </BaseSelect>
        </div>
        
        <div class="flex items-center gap-2">
          <span class="text-[13px] font-bold text-muted uppercase tracking-wider whitespace-nowrap">Địa điểm:</span>
          <BaseSelect 
            :modelValue="city" 
            @update:modelValue="$emit('update:city', $event)"
            class="min-w-[140px]"
          >
            <option value="all">Tất cả địa điểm</option>
            <option value="hanoi">Hà Nội</option>
            <option value="hcm">Hồ Chí Minh</option>
            <option value="danang">Đà Nẵng</option>
            <option value="other">Khác</option>
          </BaseSelect>
        </div>

        <div class="flex items-center gap-2">
          <span class="text-[13px] font-bold text-muted uppercase tracking-wider whitespace-nowrap">Trạng thái:</span>
          <BaseSelect 
            :modelValue="status" 
            @update:modelValue="$emit('update:status', $event)"
            class="min-w-[140px]"
          >
            <option value="all">Tất cả</option>
            <option value="upcoming">Sắp diễn ra</option>
            <option value="ended">Đã kết thúc</option>
          </BaseSelect>
        </div>
      </div>
    </div>

    <!-- Active filters summary -->
    <div v-if="hasActiveFilters" class="flex flex-wrap items-center gap-3 bg-surface border border-border-main rounded-xl p-3 md:px-5">
      <span class="text-[13px] font-bold text-muted">🔍 Đang áp dụng:</span>
      <div class="flex flex-wrap gap-2 flex-1">
        <span 
          v-if="modelValue !== 'Tất cả'" 
          @click="$emit('update:modelValue', 'Tất cả')"
          class="inline-flex items-center gap-2 px-3 py-1 bg-primary/10 border border-primary/30 rounded-full text-[12px] font-bold text-primary cursor-pointer hover:bg-danger/10 hover:border-danger hover:text-danger transition-all"
        >
          {{ modelValue }} <span class="text-[10px]">✕</span>
        </span>
        <span 
          v-if="city !== 'all'" 
          @click="$emit('update:city', 'all')"
          class="inline-flex items-center gap-2 px-3 py-1 bg-primary/10 border border-primary/30 rounded-full text-[12px] font-bold text-primary cursor-pointer hover:bg-danger/10 hover:border-danger hover:text-danger transition-all"
        >
          {{ cityLabels[city] }} <span class="text-[10px]">✕</span>
        </span>
        <span 
          v-if="status !== 'all'" 
          @click="$emit('update:status', 'all')"
          class="inline-flex items-center gap-2 px-3 py-1 bg-primary/10 border border-primary/30 rounded-full text-[12px] font-bold text-primary cursor-pointer hover:bg-danger/10 hover:border-danger hover:text-danger transition-all"
        >
          {{ status === 'upcoming' ? 'Sắp diễn ra' : 'Đã kết thúc' }} <span class="text-[10px]">✕</span>
        </span>
        <span 
          v-if="sortBy !== 'newest'" 
          @click="$emit('update:sortBy', 'newest')"
          class="inline-flex items-center gap-2 px-3 py-1 bg-primary/10 border border-primary/30 rounded-full text-[12px] font-bold text-primary cursor-pointer hover:bg-danger/10 hover:border-danger hover:text-danger transition-all"
        >
          {{ sortLabels[sortBy] }} <span class="text-[10px]">✕</span>
        </span>
      </div>
      <button 
        class="text-[12px] font-bold text-muted hover:text-danger border border-border-main hover:border-danger px-3 py-1 rounded-full transition-all"
        @click="clearAll"
      >
        Xóa tất cả
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import BaseSelect from '../../shared/components/BaseSelect.vue'

const props = defineProps({
  modelValue: { type: String, required: true },
  availableFilters: { type: Array, required: true },
  colorTheme: { type: String, default: '' },
  sortBy: { type: String, default: 'newest' },
  city: { type: String, default: 'all' },
  status: { type: String, default: 'all' },
})

const emit = defineEmits(['update:modelValue', 'update:sortBy', 'update:city', 'update:status'])

const cityLabels = { hanoi: 'Hà Nội', hcm: 'Hồ Chí Minh', danang: 'Đà Nẵng', other: 'Khác' }
const sortLabels = { oldest: 'Cũ nhất', 'price-asc': 'Giá ↑', 'price-desc': 'Giá ↓', 'name-asc': 'Tên A→Z', 'name-desc': 'Tên Z→A' }

const hasActiveFilters = computed(() =>
  props.modelValue !== 'Tất cả' ||
  props.city !== 'all' ||
  props.status !== 'all' ||
  props.sortBy !== 'newest'
)

const clearAll = () => {
  emit('update:modelValue', 'Tất cả')
  emit('update:sortBy', 'newest')
  emit('update:city', 'all')
  emit('update:status', 'all')
}
</script>
