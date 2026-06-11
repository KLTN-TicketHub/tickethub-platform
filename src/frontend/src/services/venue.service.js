import api from './api/axios'
import { VENUE_CREATE, VENUE_LIST, VENUE_DETAIL, VENUE_UPDATE, VENUE_DELETE, VENUE_SEATMAPS } from './api/endpoints'

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

