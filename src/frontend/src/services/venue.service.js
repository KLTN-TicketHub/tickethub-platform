import api from './api/axios'
import { 
  MODERATOR_VENUE_LIST, MODERATOR_VENUE_DETAIL, MODERATOR_VENUE_CREATE, MODERATOR_VENUE_UPDATE, MODERATOR_VENUE_DELETE,
  MODERATOR_VENUE_SEATMAPS, MODERATOR_VENUE_SEATMAP_CREATE, MODERATOR_VENUE_SEATMAP_DETAIL, MODERATOR_VENUE_SEATMAP_DELETE,
  ORGANIZER_VENUE_LIST, ORGANIZER_VENUE_SEATMAPS, ORGANIZER_VENUE_SEATMAP_DETAIL,
  PUBLIC_VENUES, PUBLIC_VENUE_SEATMAP_DETAIL,
  UPLOAD_SVG 
} from './api/endpoints'

// ==========================================
// MODERATOR VENUE APIS
// ==========================================

export async function createModeratorVenue(venueData) {
  const response = await api.post(MODERATOR_VENUE_CREATE, venueData)
  return response.data
}

export async function getModeratorVenues(params) {
  const response = await api.get(MODERATOR_VENUE_LIST, { params })
  return response.data
}

export async function getModeratorVenueById(id) {
  const response = await api.get(MODERATOR_VENUE_DETAIL(id))
  return response.data
}

export async function updateModeratorVenue(id, venueData) {
  const response = await api.put(MODERATOR_VENUE_UPDATE(id), venueData)
  return response.data
}

export async function deleteModeratorVenue(id) {
  const response = await api.delete(MODERATOR_VENUE_DELETE(id))
  return response.data
}

export async function getModeratorVenueSeatMaps(venueId, params) {
  const response = await api.get(MODERATOR_VENUE_SEATMAPS(venueId), { params })
  return response.data
}

export async function getModeratorSeatMapDetail(venueId, seatMapId) {
  const response = await api.get(MODERATOR_VENUE_SEATMAP_DETAIL(venueId, seatMapId))
  return response.data
}

export async function deleteModeratorSeatMap(venueId, seatMapId) {
  const response = await api.delete(MODERATOR_VENUE_SEATMAP_DELETE(venueId, seatMapId))
  return response.data
}

export async function createModeratorSeatMap(venueId, payload) {
  const response = await api.post(MODERATOR_VENUE_SEATMAP_CREATE(venueId), payload)
  return response.data
}

// ==========================================
// ORGANIZER VENUE APIS
// ==========================================

export async function getOrganizerVenues(params) {
  const response = await api.get(ORGANIZER_VENUE_LIST, { params })
  return response.data
}

export async function getOrganizerVenueSeatMaps(venueId, params) {
  const response = await api.get(ORGANIZER_VENUE_SEATMAPS(venueId), { params })
  return response.data
}

export async function getOrganizerSeatMapDetail(venueId, seatMapId) {
  const response = await api.get(ORGANIZER_VENUE_SEATMAP_DETAIL(venueId, seatMapId))
  return response.data
}

// ==========================================
// PUBLIC VENUE APIS
// ==========================================

export async function getVenues(params) {
  const response = await api.get(PUBLIC_VENUES, { params })
  return response.data
}

export async function getSeatMapDetail(venueId, seatMapId) {
  const response = await api.get(PUBLIC_VENUE_SEATMAP_DETAIL(venueId, seatMapId))
  return response.data
}

// ==========================================
// COMMON FILES
// ==========================================

export async function uploadSVGFile(file) {
  const formData = new FormData()
  formData.append('file', file)
  const response = await api.post(UPLOAD_SVG, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
  return response.data
}
