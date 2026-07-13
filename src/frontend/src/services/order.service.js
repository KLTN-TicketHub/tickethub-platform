import api from './api/axios'
import { ORDER_CHECKOUT, ORDER_PAYMENT_LINK, ORDER_EVENT_REPORT, ORDER_EVENT_ORDERS, ORDER_EVENT_CHARTS } from './api/endpoints'

/**
 * Lấy báo cáo chi tiết sự kiện
 * GET /ordering/orders/reports/events/{eventId}
 * @param {string} eventId
 * @returns {{ success, data: { eventId, eventTitle, eventImage, totalRevenue, totalTicketsSold, totalCapacity, fillRate, showtimes: [...] }, message }}
 */
export async function getEventReport(eventId) {
  const response = await api.get(ORDER_EVENT_REPORT(eventId))
  return response.data
}

/**
 * Lấy danh sách đơn hàng phân trang của sự kiện
 * GET /ordering/orders/reports/events/{eventId}/orders
 * @param {string} eventId
 * @param {Object} params - { pageNumber, pageSize, search }
 * @returns {{ success, data: PaginatedResult<EventOrderListItemDto>, message }}
 */
export async function getEventOrders(eventId, params) {
  const response = await api.get(ORDER_EVENT_ORDERS(eventId), { params })
  return response.data
}

/**
 * Lấy dữ liệu thống kê biểu đồ của sự kiện (24h hoặc 30d)
 * GET /ordering/orders/reports/events/{eventId}/charts
 * @param {string} eventId
 * @param {string} range - '24h' hoặc '30d'
 * @returns {{ success, data: Array<{ totalAmount, ticketSold, legend }>, message }}
 */
export async function getEventChartData(eventId, range) {
  const response = await api.get(ORDER_EVENT_CHARTS(eventId), { params: { range } })
  return response.data
}

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
