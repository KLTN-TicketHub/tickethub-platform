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
          :modelValue="city"
          @update:modelValue="$emit('update:city', $event)"
          class="w-[180px] flex-shrink-0"
        >
          <option value="">Mọi địa điểm</option>
          <option v-for="c in availableCities" :key="c.code" :value="c.name">{{ c.name }}</option>
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
          <span v-if="city" @click="$emit('update:city', '')" class="inline-flex items-center gap-1.5 px-3 py-1 bg-white/5 border border-white/10 rounded-full text-[12px] font-bold text-white cursor-pointer hover:bg-danger/10 hover:text-danger hover:border-danger/30 transition-all group">
            {{ city }} <PhX weight="bold" class="opacity-50 group-hover:opacity-100" />
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
  city: { type: String, default: '' },
  availableCities: { type: Array, default: () => [] },
})

const emit = defineEmits(['update:modelValue', 'update:city'])

const hasActiveFilters = computed(() =>
  props.modelValue !== 'Tất cả' || !!props.city
)

const clearAll = () => {
  emit('update:modelValue', 'Tất cả')
  emit('update:city', '')
}
</script>

<style scoped>
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
