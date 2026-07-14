import api from './api/axios'
import {
  ORGANIZER_WALLET, ORGANIZER_WALLET_TRANSACTIONS, ORGANIZER_PAYOUT_REQUEST,
  ORGANIZER_PAYOUT_PROPOSED, ORGANIZER_PAYOUT_ACCEPT, ORGANIZER_PAYOUT_REJECT
} from './api/endpoints'

/**
 * Lấy thông tin ví hiện tại (số dư)
 * GET /finance/organizer/wallet
 * @returns {{ success, message, data: WalletDto }}
 */
export async function getWallet() {
  const response = await api.get(ORGANIZER_WALLET)
  return response.data
}

/**
 * Lấy lịch sử giao dịch ví (phân trang)
 * GET /finance/organizer/wallet/transactions
 * @param {Object} params - { pageNumber, pageSize }
 * @returns {{ success, message, data: PaginatedResult<WalletTransactionDto> }}
 */
export async function getWalletTransactions(params) {
  const response = await api.get(ORGANIZER_WALLET_TRANSACTIONS, { params })
  return response.data
}

/**
 * Gửi yêu cầu giải ngân cho 1 sự kiện đã kết thúc
 * POST /finance/organizer/payouts/events/{eventId}/request
 * @param {string} eventId
 * @returns {{ success, message }}
 */
export async function requestPayout(eventId) {
  const response = await api.post(ORGANIZER_PAYOUT_REQUEST(eventId))
  return response.data
}

/**
 * Lấy danh sách đề xuất giải ngân đang chờ xác nhận (phân trang)
 * GET /finance/organizer/payouts/proposed
 * @param {Object} params - { pageNumber, pageSize }
 * @returns {{ success, message, data: PaginatedResult<ProposedPayoutDto> }}
 */
export async function getProposedPayouts(params) {
  const response = await api.get(ORGANIZER_PAYOUT_PROPOSED, { params })
  return response.data
}

/**
 * Chấp nhận đề xuất giải ngân (tiền được cộng vào ví)
 * POST /finance/organizer/payouts/{payoutId}/accept
 * @param {string} payoutId
 * @returns {{ success, message, data: EventPayoutResultDto }}
 */
export async function acceptPayout(payoutId) {
  const response = await api.post(ORGANIZER_PAYOUT_ACCEPT(payoutId))
  return response.data
}

/**
 * Từ chối đề xuất giải ngân (chuyển lại cho Moderator xem xét)
 * POST /finance/organizer/payouts/{payoutId}/reject
 * @param {string} payoutId
 * @param {string} [reason]
 * @returns {{ success, message }}
 */
export async function rejectPayout(payoutId, reason) {
  const response = await api.post(ORGANIZER_PAYOUT_REJECT(payoutId), { reason })
  return response.data
}
