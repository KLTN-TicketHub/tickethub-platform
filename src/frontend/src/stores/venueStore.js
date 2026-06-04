import { defineStore } from 'pinia';
import { ref } from 'vue';
import { venueService } from '@/services/venueService';

export const useVenueStore = defineStore('venue', () => {
  const isLoading = ref(false);
  const error = ref(null);

  const createVenue = async (payload) => {
    isLoading.value = true;
    error.value = null;
    
    try {
      const response = await venueService.createVenue(payload);
      // Giả định Interceptor của axios.js đã bóc tách data, 
      // hoặc backend trả về cấu trúc { success, message, data }
      return { 
        success: response.success ?? true, 
        message: response.message || 'Tạo địa điểm thành công!' 
      };
    } catch (err) {
      error.value = err.message || 'Đã có lỗi xảy ra.';
      return { 
        success: false, 
        message: error.value 
      };
    } finally {
      isLoading.value = false;
    }
  };

  return { 
    isLoading, 
    error, 
    createVenue 
  };
});