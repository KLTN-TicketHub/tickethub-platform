import api from './api/axios'
import { VENUE_CREATE, VENUE_LIST, VENUE_DETAIL, VENUE_UPDATE, VENUE_DELETE, VENUE_SEATMAPS, VENUE_SEATMAP_CREATE, VENUE_SEATMAP_DETAIL, VENUE_SEATMAP_DELETE, UPLOAD_SVG } from './api/endpoints'

export async function createVenue(venueData) {
  const response = await api.post(VENUE_CREATE, venueData)
  return response.data
}

export async function getVenues(params) {
  const response = await api.get(VENUE_LIST, { params })
  return response.data
}

export async function getVenueById(id) {
  const response = await api.get(VENUE_DETAIL(id))
  return response.data
}

export async function updateVenue(id, venueData) {
  const response = await api.put(VENUE_UPDATE(id), venueData)
  return response.data
}

export async function deleteVenue(id) {
  const response = await api.delete(VENUE_DELETE(id))
  return response.data
}

export async function getVenueSeatMaps(venueId, params) {
  const response = await api.get(VENUE_SEATMAPS(venueId), { params })
  return response.data
}

export async function getSeatMapDetail(venueId, seatMapId) {
  const response = await api.get(VENUE_SEATMAP_DETAIL(venueId, seatMapId))
  return response.data
}

export async function deleteSeatMap(venueId, seatMapId) {
  const response = await api.delete(VENUE_SEATMAP_DELETE(venueId, seatMapId))
  return response.data
}

export async function createSeatMap(venueId, payload) {
  const response = await api.post(VENUE_SEATMAP_CREATE(venueId), payload)
  return response.data
}

export async function uploadSVGFile(file) {
  const formData = new FormData()
  formData.append('file', file)
  const response = await api.post(UPLOAD_SVG, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
  return response.data
}


