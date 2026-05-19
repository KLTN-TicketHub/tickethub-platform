import { ref } from 'vue'
import apiClient from '../../../core/axios'

export function useCreateEvent() {
  const isLoading = ref(false)
  const error = ref(null)

  const submitEvent = async (formData) => {
    isLoading.value = true
    error.value = null
    
    try {
      // POST request to /api/events
      // Axios will automatically set the correct Content-Type boundary for FormData
      const response = await apiClient.post('/events', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || err.message || 'Lỗi khi tạo sự kiện'
      throw error.value
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    submitEvent
  }
}
