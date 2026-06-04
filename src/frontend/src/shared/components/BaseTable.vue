<template>
  <div class="glass-panel overflow-hidden w-full flex flex-col">
    <div class="overflow-x-auto no-scrollbar">
      <table class="w-full text-left text-sm whitespace-nowrap">
        <!-- Table Header -->
        <thead class="bg-surface/50 border-b border-border-main/50 text-xs text-dimmed uppercase tracking-wider font-semibold">
          <tr>
            <th 
              v-for="col in columns" 
              :key="col.key" 
              class="px-6 py-4"
              scope="col"
            >
              {{ col.label }}
            </th>
          </tr>
        </thead>

        <!-- Table Body -->
        <tbody class="divide-y divide-border-main/30">
          
          <!-- Loading State -->
          <template v-if="isLoading">
            <tr v-for="i in 5" :key="`skeleton-${i}`" class="animate-pulse">
              <td v-for="col in columns" :key="`skeleton-${i}-${col.key}`" class="px-6 py-4">
                <div class="h-4 rounded bg-border-main/40 shimmer-bg w-3/4"></div>
              </td>
            </tr>
          </template>

          <!-- Empty State -->
          <template v-else-if="!data || data.length === 0">
            <tr>
              <td :colspan="columns.length" class="px-6 py-16 text-center">
                <div class="flex flex-col items-center justify-center">
                  <div class="w-12 h-12 rounded-full bg-surface/50 border border-border-light/20 flex items-center justify-center mb-3">
                    <svg class="w-5 h-5 text-muted" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <circle cx="11" cy="11" r="8"></circle>
                      <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
                    </svg>
                  </div>
                  <span class="text-main font-medium text-sm">No records found</span>
                  <span class="text-xs text-muted mt-1">There is currently no data to display here.</span>
                </div>
              </td>
            </tr>
          </template>

          <!-- Data Rows -->
          <template v-else>
            <tr 
              v-for="(row, index) in data" 
              :key="row.id || index"
              class="hover:bg-surface/30 transition-colors duration-150"
            >
              <td 
                v-for="col in columns" 
                :key="col.key" 
                class="px-6 py-4"
              >
                <!-- Dynamic Slot for Custom Rendering -->
                <slot 
                  :name="'cell(' + col.key + ')'" 
                  :item="row" 
                  :value="row[col.key]"
                >
                  <!-- Default Rendering -->
                  <span class="text-main">{{ row[col.key] }}</span>
                </slot>
              </td>
            </tr>
          </template>
          
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
defineProps({
  columns: {
    type: Array,
    required: true,
    // Expected format: [{ key: 'email', label: 'Email Address' }]
  },
  data: {
    type: Array,
    default: () => []
  },
  isLoading: {
    type: Boolean,
    default: false
  }
})
</script>
