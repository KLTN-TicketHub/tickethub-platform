// ─── useScroll ──────────────────────────────────────────────────────────────
// Shared composable that tracks whether the page has scrolled past a threshold.
// Returns a reactive `isScrolled` boolean ref.
//
// Usage:
//   const { isScrolled } = useScroll({ threshold: 20 })

import { ref, onMounted, onUnmounted } from 'vue'

/**
 * @param {Object} options
 * @param {number} [options.threshold=20] - Scroll distance in pixels to trigger
 * @returns {{ isScrolled: import('vue').Ref<boolean> }}
 */
export function useScroll({ threshold = 20 } = {}) {
  const isScrolled = ref(false)

  const handleScroll = () => {
    isScrolled.value = window.scrollY > threshold
  }

  onMounted(() => {
    window.addEventListener('scroll', handleScroll, { passive: true })
    handleScroll() // Sync state on initial mount
  })

  onUnmounted(() => {
    window.removeEventListener('scroll', handleScroll)
  })

  return { isScrolled }
}
