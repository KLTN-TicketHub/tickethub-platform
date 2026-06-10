<template>
  <div class="flex flex-col gap-4 mb-8">
    <div class="flex flex-col xl:flex-row xl:items-center justify-between gap-6 bg-[#111916] border border-white/5 rounded-[2rem] p-4 lg:px-6">
      
      <!-- Primary Filters (Pills) -->
      <div class="flex items-center gap-4 overflow-x-auto hide-scroll pb-2 xl:pb-0">
        <button 
          v-for="filter in availableFilters" 
          :key="filter"
          @click="$emit('update:modelValue', filter)"
          class="px-5 py-2.5 rounded-full text-[13px] font-bold transition-all duration-300 whitespace-nowrap active:scale-95 border"
          :class="[
            modelValue === filter 
              ? 'bg-primary text-black border-primary shadow-[0_0_20px_rgba(0,200,83,0.2)]' 
              : 'bg-white/5 border-white/10 text-white/60 hover:bg-white/10 hover:text-white'
          ]"
        >
          {{ filter }}
        </button>
      </div>
      
      <!-- Secondary Selects -->
      <div class="flex items-center gap-3 overflow-x-auto hide-scroll">
        <BaseSelect 
          :modelValue="sortBy" 
          @update:modelValue="$emit('update:sortBy', $event)"
          class="w-[150px] flex-shrink-0"
        >
          <option value="newest">Mới nhất</option>
          <option value="oldest">Cũ nhất</option>
          <option value="price-asc">Giá tăng dần</option>
          <option value="price-desc">Giá giảm dần</option>
          <option value="name-asc">Tên A → Z</option>
          <option value="name-desc">Tên Z → A</option>
        </BaseSelect>
        
        <BaseSelect 
          :modelValue="city" 
          @update:modelValue="$emit('update:city', $event)"
          class="w-[150px] flex-shrink-0"
        >
          <option value="all">Mọi địa điểm</option>
          <option value="hanoi">Hà Nội</option>
          <option value="hcm">Hồ Chí Minh</option>
          <option value="danang">Đà Nẵng</option>
          <option value="other">Khác</option>
        </BaseSelect>

        <BaseSelect 
          :modelValue="status" 
          @update:modelValue="$emit('update:status', $event)"
          class="w-[150px] flex-shrink-0"
        >
          <option value="all">Mọi trạng thái</option>
          <option value="upcoming">Sắp diễn ra</option>
          <option value="ended">Đã kết thúc</option>
        </BaseSelect>
      </div>
    </div>

    <!-- Active filters summary -->
    <Transition enter-active-class="transition-all duration-300" enter-from-class="opacity-0 -translate-y-2" enter-to-class="opacity-100 translate-y-0" leave-active-class="transition-all duration-200" leave-from-class="opacity-100" leave-to-class="opacity-0">
      <div v-if="hasActiveFilters" class="flex flex-wrap items-center gap-3">
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-widest flex items-center gap-2">
          <PhFaders weight="bold" /> Đang áp dụng:
        </span>
        
        <div class="flex flex-wrap gap-2">
          <span v-if="modelValue !== 'Tất cả'" @click="$emit('update:modelValue', 'Tất cả')" class="inline-flex items-center gap-1.5 px-3 py-1 bg-white/5 border border-white/10 rounded-full text-[12px] font-bold text-white cursor-pointer hover:bg-danger/10 hover:text-danger hover:border-danger/30 transition-all group">
            {{ modelValue }} <PhX weight="bold" class="opacity-50 group-hover:opacity-100" />
          </span>
          <span v-if="city !== 'all'" @click="$emit('update:city', 'all')" class="inline-flex items-center gap-1.5 px-3 py-1 bg-white/5 border border-white/10 rounded-full text-[12px] font-bold text-white cursor-pointer hover:bg-danger/10 hover:text-danger hover:border-danger/30 transition-all group">
            {{ cityLabels[city] }} <PhX weight="bold" class="opacity-50 group-hover:opacity-100" />
          </span>
          <span v-if="status !== 'all'" @click="$emit('update:status', 'all')" class="inline-flex items-center gap-1.5 px-3 py-1 bg-white/5 border border-white/10 rounded-full text-[12px] font-bold text-white cursor-pointer hover:bg-danger/10 hover:text-danger hover:border-danger/30 transition-all group">
            {{ status === 'upcoming' ? 'Sắp diễn ra' : 'Đã kết thúc' }} <PhX weight="bold" class="opacity-50 group-hover:opacity-100" />
          </span>
          <span v-if="sortBy !== 'newest'" @click="$emit('update:sortBy', 'newest')" class="inline-flex items-center gap-1.5 px-3 py-1 bg-white/5 border border-white/10 rounded-full text-[12px] font-bold text-white cursor-pointer hover:bg-danger/10 hover:text-danger hover:border-danger/30 transition-all group">
            {{ sortLabels[sortBy] }} <PhX weight="bold" class="opacity-50 group-hover:opacity-100" />
          </span>
        </div>

        <button @click="clearAll" class="text-[12px] font-bold text-white/40 hover:text-white underline decoration-white/20 hover:decoration-white transition-all ml-2">
          Xóa tất cả
        </button>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import BaseSelect from '../ui/BaseSelect.vue'
import { PhFaders, PhX } from '@phosphor-icons/vue'

const props = defineProps({
  modelValue: { type: String, required: true },
  availableFilters: { type: Array, required: true },
  sortBy: { type: String, default: 'newest' },
  city: { type: String, default: 'all' },
  status: { type: String, default: 'all' },
})

const emit = defineEmits(['update:modelValue', 'update:sortBy', 'update:city', 'update:status'])

const cityLabels = { hanoi: 'Hà Nội', hcm: 'Hồ Chí Minh', danang: 'Đà Nẵng', other: 'Khác' }
const sortLabels = { oldest: 'Cũ nhất', 'price-asc': 'Giá ↑', 'price-desc': 'Giá ↓', 'name-asc': 'Tên A→Z', 'name-desc': 'Tên Z→A' }

const hasActiveFilters = computed(() =>
  props.modelValue !== 'Tất cả' || props.city !== 'all' || props.status !== 'all' || props.sortBy !== 'newest'
)

const clearAll = () => {
  emit('update:modelValue', 'Tất cả')
  emit('update:sortBy', 'newest')
  emit('update:city', 'all')
  emit('update:status', 'all')
}
</script>

<style scoped>
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
