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
    <div v-else class="relative w-full h-56 select-none">
      <svg class="w-full h-full" viewBox="0 0 1000 300" preserveAspectRatio="none">
        <defs>
          <linearGradient :id="gradientId" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" :stop-color="color" stop-opacity="0.25" />
            <stop offset="100%" :stop-color="color" stop-opacity="0" />
          </linearGradient>
        </defs>

        <line x1="50" y1="50" x2="950" y2="50" stroke="rgba(255,255,255,0.03)" stroke-width="1" />
        <line x1="50" y1="125" x2="950" y2="125" stroke="rgba(255,255,255,0.03)" stroke-width="1" />
        <line x1="50" y1="200" x2="950" y2="200" stroke="rgba(255,255,255,0.03)" stroke-width="1" />
        <line x1="50" y1="250" x2="950" y2="250" stroke="rgba(255,255,255,0.08)" stroke-width="1" />

        <path :d="svgAreaPath" :fill="`url(#${gradientId})`" />
        <path :d="svgLinePath" fill="none" :stroke="color" stroke-width="3" stroke-linecap="round" />

        <line
          v-if="hoveredIndex !== null"
          :x1="svgPoints[hoveredIndex]?.x"
          y1="50"
          :x2="svgPoints[hoveredIndex]?.x"
          y2="250"
          stroke="rgba(255,255,255,0.15)"
          stroke-width="1.5"
          stroke-dasharray="4,4"
        />

        <g v-for="(p, idx) in svgPoints" :key="idx">
          <circle
            :cx="p.x" :cy="p.y" r="10"
            fill="rgba(255,255,255,0.08)"
            class="opacity-0 hover:opacity-100 transition-opacity cursor-pointer"
            @mouseenter="hoveredIndex = idx"
            @mouseleave="hoveredIndex = null"
          />
          <circle
            :cx="p.x" :cy="p.y" r="4.5"
            :fill="hoveredIndex === idx ? color : '#111916'"
            :stroke="color"
            stroke-width="2.5"
            style="pointer-events: none; transition: all 0.2s;"
          />
        </g>
      </svg>

      <div
        v-if="hoveredIndex !== null && data[hoveredIndex]"
        class="absolute bg-[#182019] border border-white/10 text-white rounded-xl p-3 shadow-[0_10px_30px_rgba(0,0,0,0.5)] pointer-events-none transition-all duration-100 z-10"
        :style="{ left: tooltipPosition.x + '%', top: tooltipPosition.y + '%', transform: 'translate(-50%, -115%)' }"
      >
        <div class="text-[11px] font-black uppercase tracking-wider mb-1" :style="{ color }">{{ labelFn(data[hoveredIndex]) }}</div>
        <div class="text-xs font-bold text-white">{{ data[hoveredIndex][valueKey].toLocaleString('vi-VN') }} {{ valueLabel }}</div>
      </div>

      <div class="absolute bottom-0 left-0 right-0 h-8 pl-[50px] pr-[50px] pointer-events-none">
        <span
          v-for="(label, idx) in visibleXAxisLabels" :key="idx"
          class="absolute text-[9px] font-black text-white/30 uppercase tracking-wider transform -translate-x-1/2 mt-2"
          :style="{ left: label.percentage + '%' }"
        >
          {{ label.text }}
        </span>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed } from 'vue'
import { PhSpinner } from '@phosphor-icons/vue'
import { useSvgTrendChart } from '../../composables/useSvgTrendChart'

const props = defineProps({
  title: { type: String, required: true },
  color: { type: String, default: '#00C853' },
  data: { type: Array, default: () => [] },
  valueKey: { type: String, required: true },
  valueLabel: { type: String, default: '' },
  labelFn: { type: Function, required: true },
  isLoading: { type: Boolean, default: false }
})

const dataRef = computed(() => props.data)
const { hoveredIndex, svgPoints, svgLinePath, svgAreaPath, visibleXAxisLabels, tooltipPosition } = useSvgTrendChart(dataRef, props.valueKey, props.labelFn)

const total = computed(() => props.data.reduce((sum, d) => sum + (d[props.valueKey] || 0), 0))

const gradientId = `grad-${Math.random().toString(36).slice(2, 9)}`
</script>
