export function getErrorMessage(err, fallback = 'Đã có lỗi xảy ra. Vui lòng thử lại.') {
  const data = err?.response?.data
  if (Array.isArray(data?.errors) && data.errors.length) {
    return data.errors.map(e => e.message).filter(Boolean).join(', ')
  }
  // data.message chỉ tồn tại khi response thật sự đi qua middleware xử lý lỗi của service (JSON sạch).
  // Lỗi hạ tầng (Caddy/YARP trả 502/503 khi service đích không phản hồi được) không có body JSON đó —
  // không được rơi xuống err.message vì đó là câu tiếng Anh kỹ thuật của axios (vd "Request failed with status code 502").
  if (typeof data?.message === 'string' && data.message) return data.message
  return fallback
}
