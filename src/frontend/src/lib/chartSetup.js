import {
  Chart as ChartJS,
  CategoryScale,
  Filler,
  LinearScale,
  LineController,
  LineElement,
  PointElement,
  Tooltip
} from 'chart.js'

ChartJS.register(CategoryScale, Filler, LinearScale, LineController, LineElement, PointElement, Tooltip)

export function createAreaGradient(ctx, chartArea, colorHex) {
  const gradient = ctx.createLinearGradient(0, chartArea.top, 0, chartArea.bottom)
  gradient.addColorStop(0, `${colorHex}40`)
  gradient.addColorStop(1, `${colorHex}00`)
  return gradient
}
