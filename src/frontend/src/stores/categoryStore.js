import { defineStore } from 'pinia';
import { ref } from 'vue';
import { categoryService } from '@/services/categoryService';

export const useCategoryStore = defineStore('category', () => {
  const isLoading = ref(false);
  const error = ref(null);

  const createCategory = async (payload) => {
    isLoading.value = true;
    error.value = null;
    
    try {
      const response = await categoryService.createCategory(payload);
      return { 
        success: response?.success ?? true, 
        message: response?.message || 'Tạo danh mục thành công!' 
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

  return { isLoading, error, createCategory };
});