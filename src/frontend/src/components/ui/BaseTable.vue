<template>
  <div class="w-full overflow-x-auto hide-scroll transition-all rounded-[1.5rem] border border-white/5 bg-[#111916]/50">
    <table class="w-full border-collapse text-left">
      <thead class="border-b border-white/5 bg-white/5">
        <tr>
          <th 
            v-for="column in columns" 
            :key="column.key"
            class="px-6 py-4 font-bold text-[11px] text-white/40 uppercase tracking-[0.2em] whitespace-nowrap"
            :class="column.class"
          >
            {{ column.label }}
          </th>
        </tr>
      </thead>
      <tbody class="divide-y divide-white/5">
        <template v-if="data.length > 0">
          <tr 
            v-for="(row, index) in data" 
            :key="index"
            class="hover:bg-white/5 transition-colors duration-300 group cursor-default"
          >
            <td 
              v-for="column in columns" 
              :key="column.key"
              class="px-6 py-4 text-[14px] text-white/80 font-medium align-middle group-hover:text-white transition-colors"
              :class="column.class"
            >
              <slot :name="column.key" :row="row" :index="index">
                <span class="line-clamp-1">{{ row[column.key] }}</span>
              </slot>
            </td>
          </tr>
        </template>
        <tr v-else>
          <td :colspan="columns.length" class="px-6 py-24 text-center">
            <div class="flex flex-col items-center gap-4">
              <div class="w-16 h-16 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-white/20">
                <PhMagnifyingGlass class="text-2xl" weight="duotone" />
              </div>
              <p class="text-white/60 font-medium text-[14px]">Không có dữ liệu hiển thị</p>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { PhMagnifyingGlass } from '@phosphor-icons/vue'

defineProps({
  columns: { type: Array, required: true },
  data: { type: Array, required: true }
})
</script>

<style scoped>
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
