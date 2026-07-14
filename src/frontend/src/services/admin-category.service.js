import api from './api/axios'
import { ADMIN_EVENT_CATEGORIES, ADMIN_EVENT_CATEGORY_DETAIL } from './api/endpoints'

/**
 * Lấy danh sách danh mục sự kiện có phân trang & tìm kiếm cho Admin
 * GET /catalog/admin/event-categories
 * @param {Object} params - { pageNumber, pageSize, search }
 * @returns {{ success, message, data: PaginatedResult<EventCategoryDto> }}
 */
export async function getAdminEventCategories(params) {
  const response = await api.get(ADMIN_EVENT_CATEGORIES, { params })
  return response.data
}

/**
 * Lấy chi tiết danh mục sự kiện
 * GET /catalog/admin/event-categories/{id}
 * @param {string} id
 * @returns {{ success, message, data: EventCategoryDto }}
 */
export async function getAdminEventCategoryById(id) {
  const response = await api.get(ADMIN_EVENT_CATEGORY_DETAIL(id))
  return response.data
}

/**
 * Tạo mới danh mục sự kiện
 * POST /catalog/admin/event-categories
 * @param {Object} data - { name, description }
 * @returns {{ success, message, data: EventCategoryDto }}
 */
export async function createAdminEventCategory(data) {
  const response = await api.post(ADMIN_EVENT_CATEGORIES, data)
  return response.data
}

/**
 * Cập nhật danh mục sự kiện
 * PUT /catalog/admin/event-categories/{id}
 * @param {string} id
 * @param {Object} data - { name, description }
 * @returns {{ success, message, data: EventCategoryDto }}
 */
export async function updateAdminEventCategory(id, data) {
  const response = await api.put(ADMIN_EVENT_CATEGORY_DETAIL(id), data)
  return response.data
}

/**
 * Xóa danh mục sự kiện
 * DELETE /catalog/admin/event-categories/{id}
 * @param {string} id
 * @returns {{ success, message }}
 */
export async function deleteAdminEventCategory(id) {
  const response = await api.delete(ADMIN_EVENT_CATEGORY_DETAIL(id))
  return response.data
}
