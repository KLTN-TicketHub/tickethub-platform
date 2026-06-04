import axiosInstance from '@/services/api/axios';
import { ENDPOINTS } from '@/services/api/endpoints'; // Giả định bạn có file này, nếu không ta dùng hardcode string

export const venueService = {
  /**
   * Gửi payload tạo địa điểm mới lên hệ thống
   * @param {Object} payload - Dữ liệu từ CreateVenueRequest
   * @returns {Promise<Object>} - ApiResponse từ backend
   */
  async createVenue(payload) {
    // Nếu trong endpoints.js chưa có, ta dùng tạm chuỗi '/EventCategories'
    // Giả sử API gateway/catalog của bạn dùng '/Venues'
    const response = await axiosInstance.post('/Venues', payload);
    return response;
  }
};