<template>
  <div class="max-w-4xl mx-auto py-8 animate-fade-up">
    <!-- Header -->
    <div class="mb-8 flex items-center justify-between gap-4">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-3xl md:text-4xl font-black text-white tracking-tight">Cập nhật Địa điểm</h1>
        <p class="text-white/50 font-medium text-lg">Chỉnh sửa thông tin địa điểm đang lưu trữ.</p>
      </div>
      <router-link
        to="/moderator/venues"
        class="inline-flex items-center justify-center gap-2 px-4 py-2 text-[14px] font-bold text-white/70 bg-white/5 border border-white/10 rounded-xl hover:text-white hover:bg-white/10 transition-all"
      >
        <PhArrowLeft weight="bold" />
        Trở về
      </router-link>
    </div>

    <!-- Main Form -->
    <form v-if="!isPageLoading" @submit.prevent="handleSubmit" class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-6 md:p-10 flex flex-col gap-10">
      
      <!-- Thông tin chung -->
      <div class="flex flex-col gap-6">
        <div class="flex items-center gap-3 border-b border-white/5 pb-4">
          <div class="w-8 h-8 rounded-full bg-primary/10 text-primary flex items-center justify-center">
            <PhMapPin weight="fill" />
          </div>
          <h2 class="text-xl font-bold font-heading text-white">Thông tin chung</h2>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Tên địa điểm -->
          <div class="flex flex-col md:col-span-2">
            <label for="venueName" class="text-[13px] font-bold text-white/80 mb-2">Tên địa điểm <span class="text-danger">*</span></label>
            <input
              id="venueName"
              v-model="form.venueName"
              type="text"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none"
              placeholder="Vd: Sân vận động Quốc gia Mỹ Đình"
              :disabled="isSubmitting"
            />
            <p v-if="validationErrors.venueName" class="text-[12px] text-red-400 mt-2 flex items-center gap-1"><PhWarningCircle weight="fill"/> {{ validationErrors.venueName }}</p>
          </div>

          <!-- Số điện thoại -->
          <div class="flex flex-col">
            <label for="phoneNumber" class="text-[13px] font-bold text-white/80 mb-2">Số điện thoại</label>
            <input
              id="phoneNumber"
              v-model="form.phoneNumber"
              type="tel"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none"
              placeholder="Vd: 0901234567"
              :disabled="isSubmitting"
            />
            <p v-if="validationErrors.phoneNumber" class="text-[12px] text-red-400 mt-2 flex items-center gap-1"><PhWarningCircle weight="fill"/> {{ validationErrors.phoneNumber }}</p>
          </div>

          <!-- Website -->
          <div class="flex flex-col">
            <label for="websiteUrl" class="text-[13px] font-bold text-white/80 mb-2">Website URL</label>
            <input
              id="websiteUrl"
              v-model="form.websiteUrl"
              type="url"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none"
              placeholder="https://example.com"
              :disabled="isSubmitting"
            />
            <p v-if="validationErrors.websiteUrl" class="text-[12px] text-red-400 mt-2 flex items-center gap-1"><PhWarningCircle weight="fill"/> {{ validationErrors.websiteUrl }}</p>
          </div>
        </div>
      </div>

      <!-- Địa chỉ -->
      <div class="flex flex-col gap-6">
        <div class="flex items-center gap-3 border-b border-white/5 pb-4">
          <div class="w-8 h-8 rounded-full bg-[#818cf8]/10 text-[#818cf8] flex items-center justify-center">
            <PhGlobeHemisphereWest weight="fill" />
          </div>
          <h2 class="text-xl font-bold font-heading text-white">Vị trí địa lý</h2>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Quốc gia -->
          <div class="flex flex-col">
            <label for="country" class="text-[13px] font-bold text-white/80 mb-2">Quốc gia</label>
            <input
              id="country"
              v-model="form.country"
              type="text"
              disabled
              class="w-full bg-black/20 border border-white/5 rounded-xl px-4 py-3 text-white/40 cursor-not-allowed"
            />
          </div>

          <!-- Tỉnh/Thành phố -->
          <div class="flex flex-col">
            <label for="provinceCity" class="text-[13px] font-bold text-white/80 mb-2">Tỉnh/Thành phố <span class="text-danger">*</span></label>
            <select
              id="provinceCity"
              v-model="form.provinceCity"
              @change="onProvinceChange"
              :disabled="isSubmitting || isLoadingProvinces"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:bg-[#1a231f] transition-all focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed appearance-none"
            >
              <option value="" class="text-black bg-white">Chọn Tỉnh/Thành phố</option>
              <option v-for="prov in provinces" :key="prov.code" :value="prov.name" class="text-black bg-white">
                {{ prov.name }}
              </option>
            </select>
            <p v-if="validationErrors.provinceCity" class="text-[12px] text-red-400 mt-2 flex items-center gap-1"><PhWarningCircle weight="fill"/> {{ validationErrors.provinceCity }}</p>
          </div>

          <!-- Quận/Huyện -->
          <div class="flex flex-col">
            <label for="district" class="text-[13px] font-bold text-white/80 mb-2">Quận/Huyện <span class="text-danger">*</span></label>
            <select
              id="district"
              v-model="form.district"
              @change="onDistrictChange"
              :disabled="!selectedProvinceCode || isSubmitting || isLoadingDistricts"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:bg-[#1a231f] transition-all focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed appearance-none"
            >
              <option value="" class="text-black bg-white">Chọn Quận/Huyện</option>
              <option v-for="dist in districts" :key="dist.code" :value="dist.name" class="text-black bg-white">
                {{ dist.name }}
              </option>
            </select>
            <p v-if="validationErrors.district" class="text-[12px] text-red-400 mt-2 flex items-center gap-1"><PhWarningCircle weight="fill"/> {{ validationErrors.district }}</p>
          </div>

          <!-- Phường/Xã -->
          <div class="flex flex-col">
            <label for="ward" class="text-[13px] font-bold text-white/80 mb-2">Phường/Xã <span class="text-danger">*</span></label>
            <select
              id="ward"
              v-model="form.ward"
              :disabled="!selectedDistrictCode || isSubmitting || isLoadingWards"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:bg-[#1a231f] transition-all focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed appearance-none"
            >
              <option value="" class="text-black bg-white">Chọn Phường/Xã</option>
              <option v-for="w in wards" :key="w.code" :value="w.name" class="text-black bg-white">
                {{ w.name }}
              </option>
            </select>
            <p v-if="validationErrors.ward" class="text-[12px] text-red-400 mt-2 flex items-center gap-1"><PhWarningCircle weight="fill"/> {{ validationErrors.ward }}</p>
          </div>

          <!-- Số nhà, Tên đường -->
          <div class="flex flex-col md:col-span-2">
            <label for="addressLine" class="text-[13px] font-bold text-white/80 mb-2">Số nhà, Tên đường <span class="text-danger">*</span></label>
            <input
              id="addressLine"
              v-model="form.addressLine"
              type="text"
              class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder-white/30 focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none"
              placeholder="Vd: 123 Đường Nguyễn Trãi"
              :disabled="isSubmitting"
            />
            <p v-if="validationErrors.addressLine" class="text-[12px] text-red-400 mt-2 flex items-center gap-1"><PhWarningCircle weight="fill"/> {{ validationErrors.addressLine }}</p>
          </div>
        </div>
      </div>

      <!-- Hiển thị thông báo trạng thái -->
      <div v-if="apiError" class="px-4 py-3 rounded-xl bg-red-500/10 border border-red-500/20 text-red-400 text-[13px] font-medium flex items-start gap-2">
        <PhWarningCircle weight="fill" class="w-5 h-5 flex-shrink-0" />
        <span class="mt-0.5">{{ apiError }}</span>
      </div>

      <div v-if="apiSuccess" class="px-4 py-3 rounded-xl bg-primary/10 border border-primary/20 text-primary text-[13px] font-bold flex items-center gap-2">
        <PhCheckCircle weight="fill" class="w-5 h-5 flex-shrink-0" />
        {{ apiSuccess }}
      </div>

      <!-- Actions -->
      <div class="pt-6 flex flex-col sm:flex-row items-center justify-end gap-4 border-t border-white/5">
        <button
          type="button"
          @click="$router.push('/moderator/venues')"
          class="w-full sm:w-auto px-6 py-3 text-[14px] font-bold text-white/70 bg-white/5 border border-white/10 rounded-xl hover:text-white hover:bg-white/10 transition-all focus:outline-none"
          :disabled="isSubmitting"
        >
          Hủy bỏ
        </button>
        <button
          type="submit"
          class="w-full sm:w-auto px-6 py-3 text-[14px] font-bold text-black bg-primary rounded-xl hover:scale-[1.02] active:scale-95 transition-transform flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
          :disabled="isSubmitting"
        >
          <PhCircleNotch v-if="isSubmitting" class="animate-spin w-5 h-5" weight="bold" />
          {{ isSubmitting ? 'Đang cập nhật...' : 'Cập nhật' }}
        </button>
      </div>
    </form>
    
    <!-- Loading State -->
    <div v-else class="flex flex-col items-center justify-center py-20 gap-4">
      <PhCircleNotch class="animate-spin w-10 h-10 text-primary" weight="bold" />
      <span class="text-white/50 font-medium">Đang tải dữ liệu địa điểm...</span>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { getProvinces, getDistricts, getWards } from '../../services/location.service'
import { getVenueById, updateVenue } from '../../services/venue.service'
import { PhCircleNotch, PhCheckCircle, PhWarningCircle, PhMapPin, PhGlobeHemisphereWest, PhArrowLeft } from '@phosphor-icons/vue'

const router = useRouter()
const route = useRoute()

const venueId = route.params.id

const form = reactive({
  venueName: '',
  addressLine: '',
  ward: '',
  district: '',
  provinceCity: '',
  country: 'Việt Nam',
  phoneNumber: '',
  websiteUrl: ''
})

const isPageLoading = ref(true)
const isSubmitting = ref(false)
const isLoadingProvinces = ref(false)
const isLoadingDistricts = ref(false)
const isLoadingWards = ref(false)

const provinces = ref([])
const districts = ref([])
const wards = ref([])

const validationErrors = reactive({})
const apiError = ref('')
const apiSuccess = ref('')

onMounted(async () => {
  await loadProvinces()
  await fetchVenueData()
})

const fetchVenueData = async () => {
  try {
    const res = await getVenueById(venueId)
    if (res && res.success) {
      const data = res.data
      form.venueName = data.venueName || ''
      form.addressLine = data.addressLine || ''
      form.country = data.country || 'Việt Nam'
      form.phoneNumber = data.phoneNumber || ''
      form.websiteUrl = data.websiteUrl || ''
      
      // Setting location backwards
      form.provinceCity = data.provinceCity || ''
      if (form.provinceCity) {
        await onProvinceChange()
      }
      form.district = data.district || ''
      if (form.district) {
        await onDistrictChange()
      }
      form.ward = data.ward || ''
    } else {
      apiError.value = "Không thể tải dữ liệu địa điểm."
    }
  } catch (err) {
    apiError.value = "Đã xảy ra lỗi khi tải dữ liệu."
  } finally {
    isPageLoading.value = false
  }
}

const loadProvinces = async () => {
  isLoadingProvinces.value = true
  try {
    provinces.value = await getProvinces()
  } catch (error) {
    console.error('Failed to load provinces:', error)
  } finally {
    isLoadingProvinces.value = false
  }
}

const selectedProvinceCode = computed(() => {
  const p = provinces.value.find(prov => prov.name === form.provinceCity)
  return p ? p.code : null
})

const selectedDistrictCode = computed(() => {
  const d = districts.value.find(dist => dist.name === form.district)
  return d ? d.code : null
})

const onProvinceChange = async () => {
  form.district = ''
  form.ward = ''
  districts.value = []
  wards.value = []
  
  if (!selectedProvinceCode.value) return

  isLoadingDistricts.value = true
  try {
    districts.value = await getDistricts(selectedProvinceCode.value)
  } catch (error) {
    console.error('Failed to load districts:', error)
  } finally {
    isLoadingDistricts.value = false
  }
}

const onDistrictChange = async () => {
  form.ward = ''
  wards.value = []

  if (!selectedDistrictCode.value) return

  isLoadingWards.value = true
  try {
    wards.value = await getWards(selectedDistrictCode.value)
  } catch (error) {
    console.error('Failed to load wards:', error)
  } finally {
    isLoadingWards.value = false
  }
}

const handleSubmit = async () => {
  isSubmitting.value = true
  Object.keys(validationErrors).forEach(key => delete validationErrors[key])
  apiError.value = ''
  apiSuccess.value = ''

  try {
    const payload = { ...form }
    const response = await updateVenue(venueId, payload)
    
    if (response.success) {
      apiSuccess.value = response.message || 'Cập nhật địa điểm thành công!'
      // Optionally scroll to top or redirect after delay
      setTimeout(() => {
        router.push('/moderator/venues')
      }, 1500)
    } else {
      apiError.value = response.message || 'Cập nhật thất bại. Vui lòng thử lại.'
      if (response.errors && Array.isArray(response.errors)) {
        response.errors.forEach(err => {
          if (err.field) {
            validationErrors[err.field] = err.message
          }
        })
      }
    }
  } catch (error) {
    if (error.response?.data) {
      const errData = error.response.data
      apiError.value = errData.message || 'Có lỗi xảy ra, vui lòng thử lại.'
      if (errData.errors && Array.isArray(errData.errors)) {
        errData.errors.forEach(err => {
          if (err.field) {
            validationErrors[err.field] = err.message
          }
        })
      }
    } else {
      apiError.value = 'Không thể kết nối đến máy chủ.'
    }
    console.error('Update venue error:', error)
  } finally {
    isSubmitting.value = false
  }
}
</script>
