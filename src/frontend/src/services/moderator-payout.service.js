import api from './api/axios'
import { MODERATOR_PAYOUT_REQUESTS, MODERATOR_PAYOUT_PROPOSE } from './api/endpoints'

/**
 * Lấy danh sách yêu cầu giải ngân đang chờ xử lý (phân trang)
 * GET /finance/moderator/payouts/requests
 * @param {Object} params - { pageNumber, pageSize }
 * @returns {{ success, message, data: PaginatedResult<PayoutRequestDto> }}
 */
export async function getPayoutRequests(params) {
  const response = await api.get(MODERATOR_PAYOUT_REQUESTS, { params })
  return response.data
}

/**
 * Đề xuất giải ngân cho 1 yêu cầu (áp dụng % hoa hồng)
 * POST /finance/moderator/payouts/requests/{payoutRequestId}/propose
 * @param {string} payoutRequestId
 * @param {number} appliedRate
 * @returns {{ success, message, data: EventPayoutResultDto }}
 */
export async function proposePayout(payoutRequestId, appliedRate) {
  const response = await api.post(MODERATOR_PAYOUT_PROPOSE(payoutRequestId), { appliedRate })
  return response.data
}
