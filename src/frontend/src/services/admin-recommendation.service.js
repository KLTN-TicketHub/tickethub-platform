import api from './api/axios'
import { ADMIN_RECOMMENDATIONS_TRAIN, ADMIN_RECOMMENDATIONS_STATUS } from './api/endpoints'

/**
 * Lấy trạng thái mô hình recommendation hiện tại
 * GET /ai/admin/recommendations/status
 * @returns {{ success, message, data: { lastTrainedAt, interactionCount, userWithRecommendationCount } }}
 */
export async function getRecommendationStatus() {
  const response = await api.get(ADMIN_RECOMMENDATIONS_STATUS)
  return response.data
}

/**
 * Đồng bộ dữ liệu click mới nhất và train lại mô hình recommendation
 * POST /ai/admin/recommendations/train
 * @returns {{ success, message }}
 */
export async function trainRecommendationModel() {
  const response = await api.post(ADMIN_RECOMMENDATIONS_TRAIN)
  return response.data
}
