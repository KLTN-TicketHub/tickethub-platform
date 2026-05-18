// ─── useClickOutside ────────────────────────────────────────────────────────
// Shared composable that fires a callback when a click occurs outside
// the given element ref(s).
//
// Usage (single ref):
//   useClickOutside(dropdownRef, () => { isOpen.value = false })
//
// Usage (multiple refs):
//   useClickOutside([menuRef, triggerRef], () => { isOpen.value = false })

import { onMounted, onUnmounted, unref } from 'vue'

/**
 * @param {import('vue').Ref<HTMLElement> | import('vue').Ref<HTMLElement>[]} targets
 *   A single template ref or an array of template refs to treat as "inside".
 * @param {Function} callback
 *   Invoked when a click lands outside ALL of the target elements.
 */
export function useClickOutside(targets, callback) {
  const handler = (event) => {
    const refs = Array.isArray(targets) ? targets : [targets]

    const isInside = refs.some((target) => {
      const el = unref(target)
      return el && (el === event.target || el.contains(event.target))
    })

    if (!isInside) {
      callback(event)
    }
  }

  onMounted(() => {
    document.addEventListener('click', handler, true)
  })

  onUnmounted(() => {
    document.removeEventListener('click', handler, true)
  })
}
