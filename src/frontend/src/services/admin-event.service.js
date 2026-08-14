import api from './api/axios'
import { ADMIN_EVENT_REPORT_SUMMARY, ADMIN_EVENT_REPORT_BY_CATEGORY } from './api/endpoints'

/**
 * Lấy thống kê tổng quan sự kiện theo khoảng ngày
 * GET /catalog/admin/events/reports/summary
 * @param {Object} params - { dateFrom, dateTo } (yyyy-MM-dd)
 * @returns {{ success, message, data: AdminEventSummaryDto }}
 */
export async function getEventSummary(params) {
  const response = await api.get(ADMIN_EVENT_REPORT_SUMMARY, { params })
  return response.data
}

/**
 * Lấy thống kê số lượng sự kiện theo danh mục
 * GET /catalog/admin/events/reports/by-category
 * @param {Object} params - { dateFrom, dateTo }
 * @returns {{ success, message, data: AdminEventByCategoryDto[] }}
 */
export async function getEventsByCategory(params) {
  const response = await api.get(ADMIN_EVENT_REPORT_BY_CATEGORY, { params })
  return response.data
}
