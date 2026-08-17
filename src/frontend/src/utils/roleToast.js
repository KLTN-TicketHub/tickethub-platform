import { addToast } from '../stores/adminStore'
import { store as publicStore } from '../stores/eventStore'

const ROLE_PREFIXES = ['/admin', '/moderator', '/organizer', '/staff']
const ICONS = { error: '❌', warning: '⚠️', success: '✅' }

export function showToast(message, variant = 'error', path = window.location.pathname) {
  if (ROLE_PREFIXES.some(p => path.startsWith(p))) {
    addToast(message, variant)
  } else {
    publicStore.toast = { message, icon: ICONS[variant] || ICONS.error }
  }
}
