export function getErrorMessage(err, fallback = 'Đã có lỗi xảy ra. Vui lòng thử lại.') {
  const data = err?.response?.data
  if (Array.isArray(data?.errors) && data.errors.length) {
    return data.errors.map(e => e.message).filter(Boolean).join(', ')
  }
  return data?.message || err?.message || fallback
}
