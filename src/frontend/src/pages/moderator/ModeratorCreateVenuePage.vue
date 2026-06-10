<template>
  <div class="max-w-4xl mx-auto py-8 animate-fade-up">
    <div class="mb-8 flex flex-col gap-2">
      <h1 class="font-heading text-3xl md:text-4xl font-black text-white tracking-tight">Thêm mới Địa điểm</h1>
      <p class="text-white/50 font-medium text-lg">Tạo mới một địa điểm tổ chức sự kiện để sử dụng trong nền tảng.</p>
    </div>

    <form @submit.prevent="handleSubmit" class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-6 md:p-10 flex flex-col gap-10">
      
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
        <PhCheckCircle weight="fill" class="w-5 h-5" />
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
          {{ isSubmitting ? 'Đang lưu...' : 'Tạo địa điểm' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getProvinces, getDistricts, getWards } from '../../services/location.service'
import { createVenue } from '../../services/venue.service'
import { PhCircleNotch, PhCheckCircle, PhWarningCircle, PhMapPin, PhGlobeHemisphereWest } from '@phosphor-icons/vue'

const router = useRouter()

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

const validationErrors = ref({})
const apiError = ref('')
const apiSuccess = ref('')
const isSubmitting = ref(false)

// State load location
const provinces = ref([])
const districts = ref([])
const wards = ref([])

const isLoadingProvinces = ref(false)
const isLoadingDistricts = ref(false)
const isLoadingWards = ref(false)

// Tracking selected codes for fetching nested data
const selectedProvinceCode = ref(null)
const selectedDistrictCode = ref(null)

onMounted(async () => {
  await loadProvinces()
})

const loadProvinces = async () => {
  try {
    isLoadingProvinces.value = true
    provinces.value = await getProvinces()
  } catch (error) {
    console.error('Lỗi khi tải danh sách Tỉnh/Thành phố:', error)
  } finally {
    isLoadingProvinces.value = false
  }
}

const onProvinceChange = async () => {
  // Reset district, ward
  form.district = ''
  form.ward = ''
  selectedProvinceCode.value = null
  districts.value = []
  wards.value = []

  const selectedProv = provinces.value.find(p => p.name === form.provinceCity)
  if (selectedProv) {
    selectedProvinceCode.value = selectedProv.code
    try {
      isLoadingDistricts.value = true
      districts.value = await getDistricts(selectedProv.code)
    } catch (error) {
      console.error('Lỗi khi tải danh sách Quận/Huyện:', error)
    } finally {
      isLoadingDistricts.value = false
    }
  }
}

const onDistrictChange = async () => {
  form.ward = ''
  selectedDistrictCode.value = null
  wards.value = []

  const selectedDist = districts.value.find(d => d.name === form.district)
  if (selectedDist) {
    selectedDistrictCode.value = selectedDist.code
    try {
      isLoadingWards.value = true
      wards.value = await getWards(selectedDist.code)
    } catch (error) {
      console.error('Lỗi khi tải danh sách Phường/Xã:', error)
    } finally {
      isLoadingWards.value = false
    }
  }
}

const handleSubmit = async () => {
  validationErrors.value = {}
  apiError.value = ''
  apiSuccess.value = ''
  
  // Basic pre-flight check
  if (!form.venueName.trim()) validationErrors.value.venueName = 'Tên địa điểm không được để trống.'
  if (!form.provinceCity) validationErrors.value.provinceCity = 'Tỉnh/Thành phố không được để trống.'
  if (!form.district) validationErrors.value.district = 'Quận/Huyện không được để trống.'
  if (!form.ward) validationErrors.value.ward = 'Phường/Xã không được để trống.'
  if (!form.addressLine.trim()) validationErrors.value.addressLine = 'Số nhà, Tên đường không được để trống.'

  if (Object.keys(validationErrors.value).length > 0) {
    return
  }

  try {
    isSubmitting.value = true
    const response = await createVenue({ ...form })
    
    if (response.success) {
      apiSuccess.value = response.message || 'Tạo địa điểm thành công.'
      // Optionally reset form
      form.venueName = ''
      form.addressLine = ''
      form.ward = ''
      form.district = ''
      form.provinceCity = ''
      form.phoneNumber = ''
      form.websiteUrl = ''
      districts.value = []
      wards.value = []
      selectedProvinceCode.value = null
      selectedDistrictCode.value = null
      
      // Navigate away after success
      setTimeout(() => {
        router.push('/moderator/venues')
      }, 1500)
    }
  } catch (error) {
    console.error('Lỗi khi tạo Venue:', error)
    // Xử lý lỗi trả về từ API (validation errors 422)
    const errData = error.response?.data
    if (errData && errData.success === false && errData.errors) {
      errData.errors.forEach(err => {
        validationErrors.value[err.field] = err.message
      })
      apiError.value = errData.message || 'Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.'
    } else {
      apiError.value = 'Đã có lỗi xảy ra trong quá trình tạo địa điểm. Vui lòng thử lại sau.'
    }
  } finally {
    isSubmitting.value = false
  }
}
</script>
