import api from './api/axios'
import { EVENT_RATINGS, EVENT_MY_RATING, ORGANIZER_EVENT_RATINGS } from './api/endpoints'

/**
 * Gửi đánh giá cho sự kiện (yêu cầu đã check-in vé)
 * POST /catalog/events/:eventId/ratings
 * @param {string} eventId
 * @param {Object} payload - { soundRating, visualRating, organizationRating, facilityRating, serviceRating, performanceRating, comment }
 * @returns {{ success, data: EventRatingDto, message }}
 */
export async function submitEventRating(eventId, payload) {
  const response = await api.post(EVENT_RATINGS(eventId), payload)
  return response.data
}

/**
 * Lấy đánh giá của chính người dùng hiện tại cho 1 sự kiện (null nếu chưa đánh giá)
 * GET /catalog/events/:eventId/ratings/me
 * @param {string} eventId
 * @returns {{ success, data: EventRatingDto|null, message }}
 */
export async function getMyEventRating(eventId) {
  const response = await api.get(EVENT_MY_RATING(eventId))
  return response.data
}

/**
 * Lấy danh sách đánh giá phân trang của sự kiện (dành cho Organizer sở hữu sự kiện)
 * GET /catalog/organizer/events/:eventId/ratings
 * @param {string} eventId
 * @param {Object} params - { pageNumber, pageSize }
 * @returns {{ success, data: PaginatedResult<EventRatingDto>, message }}
 */
export async function getOrganizerEventRatings(eventId, params) {
  const response = await api.get(ORGANIZER_EVENT_RATINGS(eventId), { params })
  return response.data
}
