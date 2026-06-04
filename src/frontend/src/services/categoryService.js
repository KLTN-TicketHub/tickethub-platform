import api from '@/services/api/axios';

export const categoryService = {
  async createCategory(payload) {
    const response = await api.post('/EventCategories', payload);
    return response;
  }
};