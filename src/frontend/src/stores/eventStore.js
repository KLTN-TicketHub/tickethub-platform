import { reactive, ref } from 'vue'
import {
  fetchEvents,
  createEvent,
  patchEvent,
  removeEvent,
  fetchStars,
  fetchDestinations
} from '../services/eventService'

// ─── STATE ───────────────────────────────────────────────────────────────────

export const store = reactive({
  events: [],
  selectedEvent: null,
  tickets: [],
  wishlist: [],
  showBooking: false,
  bookingEvent: null,
  toast: null,
  user: null, // null means not logged in
  showAuth: false,
  authMode: 'login', // 'login' | 'register'
})

export const isLoading = ref(false)

// ─── INITIALISE ──────────────────────────────────────────────────────────────
// Call this once at app startup to hydrate the store from the service layer.

let initialized = false

export async function initStore() {
  if (initialized) return
  isLoading.value = true
  try {
    store.events = await fetchEvents()
  } finally {
    isLoading.value = false
    initialized = true
  }
}

// ─── SYNCHRONOUS GETTERS ─────────────────────────────────────────────────────
// These read from the already-loaded reactive store.
// They remain synchronous so that existing components don't break.

export function getEvents() {
  return store.events
}

export function getEventById(id) {
  return store.events.find(e => e.id === id) || null
}

// ─── ASYNC CRUD (through service) ────────────────────────────────────────────

export async function addEvent(eventData) {
  isLoading.value = true
  try {
    const newEvent = await createEvent(eventData)
    store.events.unshift(newEvent)
    return newEvent
  } finally {
    isLoading.value = false
  }
}

export async function updateEvent(id, updateData) {
  isLoading.value = true
  try {
    const updated = await patchEvent(id, updateData)
    const index = store.events.findIndex(e => e.id === id)
    if (index !== -1) store.events[index] = updated
    return updated
  } finally {
    isLoading.value = false
  }
}

export async function deleteEvent(id) {
  isLoading.value = true
  try {
    await removeEvent(id)
    const index = store.events.findIndex(e => e.id === id)
    if (index !== -1) store.events.splice(index, 1)
  } finally {
    isLoading.value = false
  }
}

// ─── UI ACTIONS (unchanged signatures) ───────────────────────────────────────

export function selectEvent(event) {
  store.selectedEvent = event
}

export function toggleWishlist(eventId) {
  const index = store.wishlist.indexOf(eventId)
  if (index === -1) {
    store.wishlist.push(eventId)
    store.toast = { message: 'Đã thêm vào yêu thích', icon: '❤️' }
  } else {
    store.wishlist.splice(index, 1)
    store.toast = { message: 'Đã xóa khỏi yêu thích', icon: '🤍' }
  }
}

export function openBooking(event) {
  store.bookingEvent = event
  store.showBooking = true
}

export function closeBooking() {
  store.showBooking = false
}

export function addTicket(ticket) {
  store.tickets.push(ticket)
  store.showBooking = false
  store.toast = { message: 'Booking confirmed! Check My Tickets.', icon: '🎟️' }
}

export function clearToast() {
  store.toast = null
}

// ─── AUTH ACTIONS ────────────────────────────────────────────────────────────

export function openAuth(mode = 'login') {
  store.authMode = mode
  store.showAuth = true
}

export function closeAuth() {
  store.showAuth = false
}

export async function login(email, password) {
  isLoading.value = true
  try {
    // Mock login delay
    await new Promise(resolve => setTimeout(resolve, 800))
    // Mock user data
    store.user = {
      name: email.split('@')[0],
      email: email,
      initial: email.charAt(0).toUpperCase()
    }
    store.showAuth = false
    store.toast = { message: 'Đăng nhập thành công!', icon: '👋' }
    return true
  } finally {
    isLoading.value = false
  }
}

export async function register(name, email, password) {
  isLoading.value = true
  try {
    // Mock register delay
    await new Promise(resolve => setTimeout(resolve, 1000))
    // Mock user data
    store.user = {
      name: name,
      email: email,
      initial: name.charAt(0).toUpperCase()
    }
    store.showAuth = false
    store.toast = { message: 'Đăng ký thành công!', icon: '🎉' }
    return true
  } finally {
    isLoading.value = false
  }
}

export function logout() {
  store.user = null
  store.toast = { message: 'Đã đăng xuất', icon: '🚪' }
}

export { fetchStars, fetchDestinations }
