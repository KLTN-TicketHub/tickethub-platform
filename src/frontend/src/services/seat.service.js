import api from './api/axios'

/**
 * Lấy danh sách trạng thái ghế của suất chiếu (Selecting, Sold, Available)
 * GET /inventory/Seats/showtimes/{showtimeId}/seats
 */
export async function getSeatStates(showtimeId) {
  const response = await api.get(`/inventory/Seats/showtimes/${showtimeId}/seats`)
  return response.data
}

/**
 * Khóa ghế (giữ ghế) cho user hiện tại
 * POST /inventory/Seats/seats/lock
 */
export async function lockSeat(showtimeId, seatId) {
  const response = await api.post('/inventory/Seats/seats/lock', { showtimeId, seatId })
  return response.data
}

/**
 * Hủy khóa ghế (bỏ chọn)
 * POST /inventory/Seats/seats/unlock
 */
export async function unlockSeat(showtimeId, seatId) {
  const response = await api.post('/inventory/Seats/seats/unlock', { showtimeId, seatId })
  return response.data
}

/**
 * Lấy trạng thái tồn kho của một loại vé (ví dụ: vé đứng) trong một suất chiếu
 * GET /inventory/Tickets/showtimes/{showtimeId}/types/{ticketTypeId}
 */
export async function getTicketInventoryState(showtimeId, ticketTypeId) {
  const response = await api.get(`/inventory/Tickets/showtimes/${showtimeId}/types/${ticketTypeId}`)
  return response.data
}
