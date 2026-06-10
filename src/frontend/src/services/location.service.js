const BASE_URL = 'https://provinces.open-api.vn/api/v1'

export async function getProvinces() {
  const response = await fetch(`${BASE_URL}/p/`)
  const data = await response.json()
  return data || []
}

export async function getDistricts(provinceCode) {
  if (!provinceCode) return []
  const response = await fetch(`${BASE_URL}/p/${provinceCode}?depth=2`)
  const data = await response.json()
  return data?.districts || []
}

export async function getWards(districtCode) {
  if (!districtCode) return []
  const response = await fetch(`${BASE_URL}/d/${districtCode}?depth=2`)
  const data = await response.json()
  return data?.wards || []
}
