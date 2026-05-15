// ─── EVENT SERVICE LAYER ─────────────────────────────────────────────────────
// This module simulates API calls with Promises and artificial delay.
// When a real backend is available, replace the body of each method
// with actual fetch/axios calls. The function signatures stay the same.

import { MOCK_EVENTS } from '../mocks/eventMock'

// In-memory store that simulates a database
let events = [...MOCK_EVENTS]

// Simulated network delay (ms)
const DELAY = 350

function delay(ms = DELAY) {
  return new Promise(resolve => setTimeout(resolve, ms))
}

function generateId() {
  return 'ev_' + Date.now().toString(36) + '_' + Math.random().toString(36).slice(2, 6)
}

function generateSlug(title) {
  return title
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/(^-|-$)+/g, '')
}

function computeStatus(dateStart, dateEnd) {
  const now = new Date()
  const start = new Date(dateStart)
  const end = dateEnd ? new Date(dateEnd) : null
  if (end && now > end) return 'ended'
  if (now > start) return 'ended'
  return 'upcoming'
}

// ─── PUBLIC API ──────────────────────────────────────────────────────────────

/**
 * Fetch all events.
 * Future: GET /api/events
 */
export async function fetchEvents() {
  await delay()
  return [...events]
}

/**
 * Create a new event.
 * Future: POST /api/events
 */
export async function createEvent(eventData) {
  await delay()
  const newEvent = {
    ...eventData,
    id: generateId(),
    slug: generateSlug(eventData.title),
    status: computeStatus(eventData.dateStart, eventData.dateEnd),
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }
  events.unshift(newEvent)
  return { ...newEvent }
}

/**
 * Update an existing event.
 * Future: PUT /api/events/:id
 */
export async function patchEvent(id, updateData) {
  await delay()
  const index = events.findIndex(e => e.id === id)
  if (index === -1) throw new Error(`Event not found: ${id}`)

  events[index] = {
    ...events[index],
    ...updateData,
    slug: updateData.title ? generateSlug(updateData.title) : events[index].slug,
    status: computeStatus(
      updateData.dateStart || events[index].dateStart,
      updateData.dateEnd || events[index].dateEnd
    ),
    updatedAt: new Date().toISOString()
  }
  return { ...events[index] }
}

/**
 * Delete an event by ID.
 * Future: DELETE /api/events/:id
 */
export async function removeEvent(id) {
  await delay()
  const index = events.findIndex(e => e.id === id)
  if (index === -1) throw new Error(`Event not found: ${id}`)
  events.splice(index, 1)
  return { success: true }
}

// ─── STARS & DESTINATIONS ───────────────────────────────────────────────────

const MOCK_STARS = [
  { id: 1, name: 'Hà Anh Tuấn', image: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400&h=400&fit=crop', followers: '1.2M' },
  { id: 2, name: 'Mỹ Tâm', image: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=400&h=400&fit=crop', followers: '2.5M' },
  { id: 3, name: 'Đen Vâu', image: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=400&fit=crop', followers: '3.1M' },
  { id: 4, name: 'Sơn Tùng M-TP', image: 'https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?w=400&h=400&fit=crop', followers: '5.4M' },
  { id: 5, name: 'Vũ.', image: 'https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=400&h=400&fit=crop', followers: '800K' },
  { id: 6, name: 'Hoàng Thùy Linh', image: 'https://images.unsplash.com/photo-1488426862026-3ee34a7d66df?w=400&h=400&fit=crop', followers: '1.1M' },
  { id: 7, name: 'Binz', image: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400&h=400&fit=crop', followers: '2.8M' },
]

const MOCK_DESTINATIONS = [
  { id: 1, name: 'Diamond Arena', city: 'TP. Hồ Chí Minh', image: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=800&h=500&fit=crop', events: 12 },
  { id: 2, name: 'Sân vận động Quân khu 7', city: 'TP. Hồ Chí Minh', image: 'https://images.unsplash.com/photo-1522770179533-24471fcdba45?w=800&h=500&fit=crop', events: 8 },
  { id: 3, name: 'Nhà hát Hòa Bình', city: 'TP. Hồ Chí Minh', image: 'https://images.unsplash.com/photo-1514306191717-452ec28c7814?w=800&h=500&fit=crop', events: 5 },
  { id: 4, name: 'Trung tâm Hội nghị Quốc gia', city: 'Hà Nội', image: 'https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=800&h=500&fit=crop', events: 15 },
  { id: 5, name: 'Phố đi bộ Nguyễn Huệ', city: 'TP. Hồ Chí Minh', image: 'https://images.unsplash.com/photo-1555436169-20d93ea9a7ff?w=800&h=500&fit=crop', events: 20 },
  { id: 6, name: 'Nhà thi đấu Phú Thọ', city: 'TP. Hồ Chí Minh', image: 'https://images.unsplash.com/photo-1577223625816-7546f13df25d?w=800&h=500&fit=crop', events: 4 },
]

export async function fetchStars() {
  await delay(200)
  return [...MOCK_STARS]
}

export async function fetchDestinations() {
  await delay(200)
  return [...MOCK_DESTINATIONS]
}
