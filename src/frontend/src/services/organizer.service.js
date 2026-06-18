import api from './api/axios'
import { EVENT_CATEGORIES, UPLOAD_COVER_IMAGE, ORGANIZER_EVENT_CREATE, VENUE_SEATMAP_DETAIL, ORGANIZER_EVENTS_LIST, EVENT_STATUSES_LOOKUP } from './api/endpoints'

/**
 * Lấy danh sách danh mục sự kiện (phân trang)
 * GET /event-categories
 */
export async function getEventCategories(params = {}) {
  const response = await api.get(EVENT_CATEGORIES, { params })
  return response.data
}

/**
 * Upload ảnh bìa sự kiện (1280×720 px)
 * POST /catalog/files/upload-cover-image
 * @param {File} file - File ảnh cần upload
 * @returns {Promise<string>} URL ảnh sau khi upload
 */
export async function uploadCoverImage(file) {
  const formData = new FormData()
  formData.append('file', file)
  const response = await api.post(UPLOAD_COVER_IMAGE, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
  return response.data
}

/**
 * Tạo sự kiện mới (không có seatmap - tự nhập địa điểm)
 * POST /catalog/events
 * @param {Object} payload - Dữ liệu sự kiện
 */
export async function createEventWithLocation(payload) {
  const response = await api.post(ORGANIZER_EVENT_CREATE, payload)
  return response.data
}

/**
 * Tạo sự kiện mới (có seatmap - chọn từ hệ thống)
 * POST /catalog/events
 * @param {Object} payload - Dữ liệu sự kiện với seatMapId và zoneId
 */
export async function createEventWithSeatMap(payload) {
  const response = await api.post(ORGANIZER_EVENT_CREATE, payload)
  return response.data
}

/**
 * Lấy chi tiết seatmap theo venueId và seatMapId (dùng để lấy danh sách zones)
 * GET /catalog/venue/:venueId/seat-maps/:seatMapId
 */
export async function getSeatMapZones(venueId, seatMapId) {
  const response = await api.get(VENUE_SEATMAP_DETAIL(venueId, seatMapId))
  return response.data
}

/**
 * Lấy danh sách sự kiện của organizer (phân trang, trạng thái, tìm kiếm)
 * GET /catalog/events
 */
export async function getOrganizerEvents(params = {}) {
  const response = await api.get(ORGANIZER_EVENTS_LIST, { params })
  return response.data
}

/**
 * Lấy danh sách trạng thái sự kiện (lookup enum)
 * GET /catalog/lookup/event-statuses
 */
export async function getEventStatuses() {
  const response = await api.get(EVENT_STATUSES_LOOKUP)
  return response.data
}

