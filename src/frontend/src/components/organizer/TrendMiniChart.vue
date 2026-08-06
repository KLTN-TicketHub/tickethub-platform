<template>
  <section class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-6 md:p-8 shadow-2xl">
    <div class="flex items-center justify-between mb-6">
      <h3 class="font-heading text-xl font-black text-white uppercase tracking-wider">{{ title }}</h3>
      <div class="text-2xl font-black" :style="{ color }">{{ total.toLocaleString('vi-VN') }}</div>
    </div>

    <div v-if="isLoading" class="h-56 flex items-center justify-center">
      <PhSpinner class="animate-spin text-primary text-2xl" weight="bold" />
    </div>
    <div v-else-if="data.length === 0" class="h-56 flex items-center justify-center text-white/30 font-bold text-[13px]">
      Không có dữ liệu cho khoảng thời gian này.
    </div>
    <div v-else class="relative w-full h-56">
      <Line :data="chartJsData" :options="chartOptions" />
    </div>
  </section>
</template>

<script setup>
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import { PhSpinner } from '@phosphor-icons/vue'
import { createAreaGradient } from '../../lib/chartSetup'

const props = defineProps({
  title: { type: String, required: true },
  color: { type: String, default: '#00C853' },
  data: { type: Array, default: () => [] },
  valueKey: { type: String, required: true },
  valueLabel: { type: String, default: '' },
  labelFn: { type: Function, required: true },
  isLoading: { type: Boolean, default: false }
})

const total = computed(() => props.data.reduce((sum, d) => sum + (d[props.valueKey] || 0), 0))

const chartJsData = computed(() => ({
  labels: props.data.map(d => props.labelFn(d)),
  datasets: [
    {
      data: props.data.map(d => d[props.valueKey] || 0),
      borderColor: props.color,
      borderWidth: 3,
      pointRadius: 0,
      pointHoverRadius: 5,
      pointHoverBackgroundColor: props.color,
      pointHoverBorderColor: '#ffffff',
      pointHoverBorderWidth: 2,
      tension: 0.35,
      fill: true,
      backgroundColor: (context) => {
        const { ctx, chartArea } = context.chart
        if (!chartArea) return `${props.color}20`
        return createAreaGradient(ctx, chartArea, props.color)
      }
    }
  ]
}))

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  interaction: { mode: 'index', intersect: false },
  scales: {
    x: {
      grid: { display: false },
      border: { display: false },
      ticks: {
        color: 'rgba(255,255,255,0.3)',
        font: { size: 9, weight: 'bold' },
        maxRotation: 0,
        autoSkip: true,
        maxTicksLimit: 8
      }
    },
    y: {
      display: false,
      beginAtZero: true
    }
  },
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#182019',
      borderColor: `${props.color}33`,
      borderWidth: 1,
      titleColor: props.color,
      titleFont: { size: 11, weight: 'bold' },
      bodyColor: '#ffffff',
      bodyFont: { size: 12, weight: 'bold' },
      padding: 12,
      cornerRadius: 12,
      displayColors: false,
      callbacks: {
        title: (items) => props.labelFn(props.data[items[0].dataIndex]),
        label: (item) => `${(props.data[item.dataIndex][props.valueKey] || 0).toLocaleString('vi-VN')} ${props.valueLabel}`
      }
    }
  }
}))
</script>
