import api from './api/axios'
import {
  AI_SUGGESTIONS_MARKET_DIRECTION,
  AI_SUGGESTIONS_PORTFOLIO,
  AI_SUGGESTIONS_EVENT
} from './api/endpoints'

/**
 * Gợi ý định hướng thị trường (xu hướng theo category toàn platform, đối chiếu portfolio của Organizer)
 * GET /ai/organizer/suggestions/market-direction
 * @param {Object} params - { range: '7d' | '30d' }
 * @returns {{ success, data: { summary, suggestions: Array }, message }}
 */
export async function getMarketDirectionSuggestions(params) {
  const response = await api.get(AI_SUGGESTIONS_MARKET_DIRECTION, { params })
  return response.data
}

/**
 * Gợi ý tổng hợp trên toàn bộ sự kiện của Organizer (event nào cần chú ý)
 * GET /ai/organizer/suggestions/portfolio
 * @param {Object} params - { range: '7d' | '30d' }
 * @returns {{ success, data: { summary, suggestions: Array }, message }}
 */
export async function getPortfolioSuggestions(params) {
  const response = await api.get(AI_SUGGESTIONS_PORTFOLIO, { params })
  return response.data
}

/**
 * Gợi ý cải thiện cho 1 sự kiện cụ thể
 * GET /ai/organizer/suggestions/events/:eventId
 * @param {string} eventId
 * @param {Object} params - { range: '7d' | '30d' }
 * @returns {{ success, data: { summary, suggestions: Array }, message }}
 */
export async function getEventSuggestions(eventId, params) {
  const response = await api.get(AI_SUGGESTIONS_EVENT(eventId), { params })
  return response.data
}
