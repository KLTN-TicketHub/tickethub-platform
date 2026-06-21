import api from './api/axios'
import { LOCATION_PROVINCES, LOCATION_DISTRICTS, LOCATION_WARDS } from './api/endpoints'

export async function getProvinces() {
  try {
    const response = await api.get(LOCATION_PROVINCES)
    if (response.data && response.data.success) {
      return response.data.data || []
    }
    return []
  } catch (error) {
    console.error('Failed to get provinces:', error)
    return []
  }
}

export async function getDistricts(provinceCode) {
  if (!provinceCode) return []
  try {
    const response = await api.get(LOCATION_DISTRICTS(provinceCode))
    if (response.data && response.data.success) {
      return response.data.data || []
    }
    return []
  } catch (error) {
    console.error('Failed to get districts:', error)
    return []
  }
}

export async function getWards(districtCode) {
  if (!districtCode) return []
  try {
    const response = await api.get(LOCATION_WARDS(districtCode))
    if (response.data && response.data.success) {
      return response.data.data || []
    }
    return []
  } catch (error) {
    console.error('Failed to get wards:', error)
    return []
  }
}
