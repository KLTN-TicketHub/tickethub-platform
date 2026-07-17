import api from './api/axios'
import { ORGANIZER_EVENT_CLICK_TREND, ORGANIZER_INSIGHTS } from './api/endpoints'

/**
 * Lấy xu hướng tương tác (lượt xem, lượt có ý định mua) của 1 sự kiện
 * GET /catalog/organizer/events/:eventId/click-trend
 * @param {string} eventId
 * @param {Object} params - { range: '7d' | '30d' }
 * @returns {{ success, data: Array<{ date, viewCount, purchaseIntentCount }>, message }}
 */
export async function getEventClickTrend(eventId, params) {
  const response = await api.get(ORGANIZER_EVENT_CLICK_TREND(eventId), { params })
  return response.data
}

/**
 * Lấy thống kê tổng quan xu hướng tương tác trên toàn bộ sự kiện của Organizer
 * GET /catalog/organizer/insights
 * @param {Object} params - { range: '7d' | '30d' }
 * @returns {{ success, data: { trend: Array, topEvents: Array }, message }}
 */
export async function getOrganizerInsights(params) {
  const response = await api.get(ORGANIZER_INSIGHTS, { params })
  return response.data
}
