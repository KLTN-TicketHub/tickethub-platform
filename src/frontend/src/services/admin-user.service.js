import api from './api/axios'
import { ADMIN_USERS, ADMIN_USER_DETAIL, ADMIN_USER_LOCK_STATUS } from './api/endpoints'

/**
 * Lấy danh sách người dùng có phân trang, tìm kiếm, lọc theo role/trạng thái cho Admin
 * GET /auth/admin/users
 * @param {Object} params - { pageNumber, pageSize, search, role, isLocked }
 * @returns {{ success, message, data: PaginatedResult<UserListItemDto> }}
 */
export async function getAdminUsers(params) {
  const response = await api.get(ADMIN_USERS, { params })
  return response.data
}

/**
 * Lấy chi tiết người dùng
 * GET /auth/admin/users/{id}
 * @param {string} id
 * @returns {{ success, message, data: UserDetailDto }}
 */
export async function getAdminUserById(id) {
  const response = await api.get(ADMIN_USER_DETAIL(id))
  return response.data
}

/**
 * Cập nhật thông tin cơ bản của người dùng
 * PUT /auth/admin/users/{id}
 * @param {string} id
 * @param {Object} data - { fullName, phoneNumber, imageUrl }
 * @returns {{ success, message, data: UserDetailDto }}
 */
export async function updateAdminUser(id, data) {
  const response = await api.put(ADMIN_USER_DETAIL(id), data)
  return response.data
}

/**
 * Khoá / mở khoá tài khoản người dùng
 * PATCH /auth/admin/users/{id}/lock-status
 * @param {string} id
 * @param {boolean} isLocked
 * @returns {{ success, message, data: UserDetailDto }}
 */
export async function setAdminUserLockStatus(id, isLocked) {
  const response = await api.patch(ADMIN_USER_LOCK_STATUS(id), { isLocked })
  return response.data
}
