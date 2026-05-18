// ─── useEventSearch ─────────────────────────────────────────────────────────
// Feature composable that encapsulates the event search logic.
// Manages search query state, filters events, and returns results.
//
// Usage:
//   const { searchQuery, searchResults, showSearch, handleSearch, clearSearch } = useEventSearch()

import { ref } from 'vue'
import { getEvents } from '../store'

const MAX_RESULTS = 6

/**
 * @returns {{
 *   searchQuery: import('vue').Ref<string>,
 *   searchResults: import('vue').Ref<Array>,
 *   showSearch: import('vue').Ref<boolean>,
 *   handleSearch: () => void,
 *   clearSearch: () => void
 * }}
 */
export function useEventSearch() {
  const searchQuery = ref('')
  const searchResults = ref([])
  const showSearch = ref(false)

  /**
   * Filters the global event list by title, performer name, or venue name.
   * Called on every input keystroke and on focus.
   */
  const handleSearch = () => {
    const q = searchQuery.value.toLowerCase().trim()

    if (!q) {
      searchResults.value = []
      showSearch.value = false
      return
    }

    const allEvents = getEvents()
    searchResults.value = allEvents
      .filter((e) =>
        e.title.toLowerCase().includes(q) ||
        (e.performers?.some((p) => p.name.toLowerCase().includes(q))) ||
        (e.location?.name?.toLowerCase().includes(q))
      )
      .slice(0, MAX_RESULTS)

    showSearch.value = true
  }

  /**
   * Resets the search state (e.g. after navigating to an event).
   */
  const clearSearch = () => {
    searchQuery.value = ''
    searchResults.value = []
    showSearch.value = false
  }

  return {
    searchQuery,
    searchResults,
    showSearch,
    handleSearch,
    clearSearch,
  }
}
