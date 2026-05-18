<template>
  <div 
    class="w-full overflow-x-auto custom-scrollbar transition-all"
    :class="[
      variant === 'default' 
        ? 'rounded-[24px] border border-border-main bg-card shadow-xl' 
        : 'bg-transparent'
    ]"
  >
    <table class="w-full border-collapse text-left">
      <thead 
        class="border-b border-border-main sticky top-0 z-10"
        :class="variant === 'default' ? 'bg-surface/60 backdrop-blur-md' : 'bg-transparent'"
      >
        <tr>
          <th 
            v-for="column in columns" 
            :key="column.key"
            class="px-6 py-5 font-heading text-[11px] font-bold text-muted uppercase tracking-[0.15em] whitespace-nowrap"
            :class="column.class"
          >
            {{ column.label }}
          </th>
        </tr>
      </thead>
      <tbody class="divide-y divide-border-main/30">
        <template v-if="data.length > 0">
          <tr 
            v-for="(row, index) in data" 
            :key="index"
            class="hover:bg-primary/[0.04] transition-all duration-300 group relative cursor-default"
          >
            <!-- Left Highlight Border -->
            <td class="absolute left-0 top-0 bottom-0 w-1 bg-primary scale-y-0 group-hover:scale-y-100 transition-transform duration-300 origin-center z-20"></td>
            
            <td 
              v-for="column in columns" 
              :key="column.key"
              class="px-6 py-4.5 text-[14px] text-main font-medium align-middle"
              :class="column.class"
            >
              <slot :name="column.key" :row="row" :index="index">
                <span class="line-clamp-1 opacity-90 group-hover:opacity-100 transition-opacity">{{ row[column.key] }}</span>
              </slot>
            </td>
          </tr>
        </template>
        <tr v-else>
          <td :colspan="columns.length" class="px-6 py-32 text-center">
            <div class="flex flex-col items-center gap-5 animate-in fade-in zoom-in duration-700">
              <div class="relative">
                <div class="w-24 h-24 rounded-full bg-surface border border-border-main flex items-center justify-center text-4xl shadow-inner grayscale opacity-30">
                  📦
                </div>
                <div class="absolute -bottom-1 -right-1 w-8 h-8 rounded-full bg-card border border-border-main flex items-center justify-center text-xs opacity-50">🔍</div>
              </div>
              <div class="flex flex-col gap-1.5">
                <p class="text-main font-bold text-xl tracking-tight">Dữ liệu trống</p>
                <p class="text-muted text-[14px] max-w-[300px] leading-relaxed">Hiện không tìm thấy thông tin nào phù hợp để hiển thị tại đây.</p>
              </div>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
defineProps({
  columns: {
    type: Array,
    required: true,
  },
  data: {
    type: Array,
    required: true
  },
  variant: {
    type: String,
    default: 'default',
    validator: (v) => ['default', 'ghost'].includes(v)
  }
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  height: 6px;
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: var(--color-border-main);
  border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: var(--color-primary);
  opacity: 0.5;
}
</style>
