import api from './api/axios'
import { ORDER_CHECKOUT, ORDER_PAYMENT_LINK } from './api/endpoints'

/**
 * Gửi yêu cầu checkout đơn hàng
 * POST /ordering/orders/checkout
 * @param {Object} payload - { showtimeId, ticketTypeId, quantity, seatIds }
 * @returns {{ success, data: orderId, message }}
 */
export async function checkout(payload) {
  const response = await api.post(ORDER_CHECKOUT, payload)
  return response.data
}

/**
 * Lấy liên kết thanh toán của đơn hàng từ Saga State
 * GET /ordering/orders/{orderId}/payment-link
 * @param {string} orderId
 * @returns {{ success, data: { status, paymentLink }, message }}
 */
export async function getPaymentLink(orderId) {
  const response = await api.get(ORDER_PAYMENT_LINK(orderId))
  return response.data
}

/**
 * Polling lấy payment link trong vòng tối đa maxAttempts lần (cách nhau intervalMs ms)
 * Resolve khi có link, reject khi hết lần thử hoặc đơn hàng bị hủy.
 * @param {string} orderId
 * @param {{ maxAttempts?: number, intervalMs?: number, onStatusChange?: Function }} options
 */
export function pollForPaymentLink(orderId, { maxAttempts = 20, intervalMs = 2000, onStatusChange } = {}) {
  return new Promise((resolve, reject) => {
    let attempts = 0

    const attempt = async () => {
      try {
        attempts++
        const res = await getPaymentLink(orderId)

        if (onStatusChange && res?.data?.status) {
          onStatusChange(res.data.status)
        }

        if (res?.success && res?.data?.paymentLink) {
          resolve(res.data.paymentLink)
          return
        }

        if (!res?.success) {
          // Đơn hàng bị hủy hoặc lỗi nghiêm trọng
          reject(new Error(res?.message || 'Đơn hàng bị hủy hoặc không thể khởi tạo thanh toán.'))
          return
        }

        if (attempts >= maxAttempts) {
          reject(new Error('Không thể lấy liên kết thanh toán sau nhiều lần thử. Vui lòng thử lại.'))
          return
        }

        setTimeout(attempt, intervalMs)
      } catch (err) {
        if (attempts >= maxAttempts) {
          reject(err)
        } else {
          setTimeout(attempt, intervalMs)
        }
      }
    }

    attempt()
  })
}
