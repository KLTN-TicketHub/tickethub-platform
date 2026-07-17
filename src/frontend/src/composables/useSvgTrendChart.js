import { ref, computed } from 'vue'

/**
 * Composable tính toán các thuộc tính vẽ biểu đồ đường/vùng SVG (line + area chart)
 * dùng chung cho các trang thống kê xu hướng (Organizer Insights, Event Report).
 * Tách ra từ logic chart doanh thu ở OrganizerEventReportPage.vue để dùng lại
 * cho nhiều loại số liệu khác nhau (views, purchase intent...) mà không phải
 * chép lại phần toán học (điểm toạ độ, path SVG, vị trí tooltip).
 *
 * @param {import('vue').Ref<Array>} dataRef - ref chứa mảng dữ liệu điểm
 * @param {string} valueKey - tên field chứa giá trị số cần vẽ
 * @param {(item: Object) => string} labelFn - hàm lấy nhãn hiển thị trục X từ 1 điểm dữ liệu
 */
export function useSvgTrendChart(dataRef, valueKey, labelFn) {
  const hoveredIndex = ref(null)

  const maxValue = computed(() => {
    if (dataRef.value.length === 0) return 1
    const max = Math.max(...dataRef.value.map(d => d[valueKey]))
    return max > 0 ? max : 1
  })

  const svgPoints = computed(() => {
    const total = dataRef.value.length
    if (total === 0) return []
    const width = 900
    const height = 200
    const maxVal = maxValue.value

    return dataRef.value.map((d, index) => {
      const x = total === 1 ? 50 : 50 + (index / (total - 1)) * width
      const y = 250 - (d[valueKey] / maxVal) * height
      return { x, y }
    })
  })

  const svgLinePath = computed(() => {
    const pts = svgPoints.value
    if (pts.length === 0) return ''
    return pts.map((p, i) => (i === 0 ? `M ${p.x} ${p.y}` : `L ${p.x} ${p.y}`)).join(' ')
  })

  const svgAreaPath = computed(() => {
    const pts = svgPoints.value
    if (pts.length === 0) return ''
    const linePath = svgLinePath.value
    const firstX = pts[0].x
    const lastX = pts[pts.length - 1].x
    return `${linePath} L ${lastX} 250 L ${firstX} 250 Z`
  })

  const visibleXAxisLabels = computed(() => {
    const total = dataRef.value.length
    if (total === 0) return []
    const indices = []

    let step = 1
    if (total > 20) step = 5
    else if (total > 10) step = 3

    for (let i = 0; i < total; i++) {
      if (i === 0 || i === total - 1 || i % step === 0) {
        indices.push(i)
      }
    }

    return indices.map(i => {
      const percentage = total === 1 ? 50 : (i / (total - 1)) * 90 + 5
      return { percentage, text: labelFn(dataRef.value[i]) }
    })
  })

  const tooltipPosition = computed(() => {
    if (hoveredIndex.value === null) return { x: 0, y: 0 }
    const index = hoveredIndex.value
    const total = dataRef.value.length
    const pctX = total === 1 ? 50 : (index / (total - 1)) * 90 + 5

    const maxVal = maxValue.value
    const val = dataRef.value[index][valueKey]
    const pctY = 100 - (val / maxVal) * 66.6 - 16.6

    return { x: pctX, y: pctY }
  })

  return {
    hoveredIndex,
    maxValue,
    svgPoints,
    svgLinePath,
    svgAreaPath,
    visibleXAxisLabels,
    tooltipPosition
  }
}
