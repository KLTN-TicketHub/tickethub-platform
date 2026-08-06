import api from './api/axios'
import {
  ADMIN_FINANCE_SUMMARY, ADMIN_FINANCE_TREND, ADMIN_FINANCE_BY_CATEGORY, ADMIN_PAYMENT_GATEWAY_STATS
} from './api/endpoints'

/**
 * Lấy tổng quan tài chính nền tảng theo khoảng ngày
 * GET /finance/admin/finance/reports/summary
 * @param {Object} params - { dateFrom, dateTo } (yyyy-MM-dd)
 * @returns {{ success, message, data: AdminFinanceSummaryDto }}
 */
export async function getFinanceSummary(params) {
  const response = await api.get(ADMIN_FINANCE_SUMMARY, { params })
  return response.data
}

/**
 * Lấy dữ liệu xu hướng doanh thu/phí sàn theo ngày
 * GET /finance/admin/finance/reports/trend
 * @param {Object} params - { dateFrom, dateTo }
 * @returns {{ success, message, data: AdminFinanceTrendPointDto[] }}
 */
export async function getFinanceTrend(params) {
  const response = await api.get(ADMIN_FINANCE_TREND, { params })
  return response.data
}

/**
 * Lấy doanh thu/phí sàn theo danh mục sự kiện
 * GET /finance/admin/finance/reports/by-category
 * @param {Object} params - { dateFrom, dateTo }
 * @returns {{ success, message, data: AdminFinanceByCategoryDto[] }}
 */
export async function getFinanceByCategory(params) {
  const response = await api.get(ADMIN_FINANCE_BY_CATEGORY, { params })
  return response.data
}

/**
 * Lấy thống kê giao dịch theo cổng thanh toán (Momo/VNPay)
 * GET /payment/admin/payment/reports/gateway-stats
 * @param {Object} params - { dateFrom, dateTo }
 * @returns {{ success, message, data: AdminPaymentGatewayStatsDto[] }}
 */
export async function getPaymentGatewayStats(params) {
  const response = await api.get(ADMIN_PAYMENT_GATEWAY_STATS, { params })
  return response.data
}
