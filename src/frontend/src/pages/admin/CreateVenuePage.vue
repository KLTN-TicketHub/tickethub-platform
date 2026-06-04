<template>
  <div class="animate-fade-up max-w-[1100px] mx-auto py-10 px-4 min-h-[80vh]">
    <!-- Header -->
    <div class="mb-10">
      <h1 class="font-heading text-3xl lg:text-4xl font-bold text-main mb-2">
        Tạo địa điểm mới
      </h1>
      <p class="text-muted font-medium">
        Thêm địa điểm tổ chức sự kiện vào hệ thống TicketHub
      </p>
    </div>

    <!-- Two-column grid -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Panel 1: Basic Info -->
      <div class="glass-panel flex flex-col gap-2">
        <div class="flex items-center gap-3 mb-4 pb-4 border-b border-border-main">
          <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-xl">🏟️</div>
          <h2 class="font-heading text-xl font-bold text-main">Thông tin cơ bản</h2>
        </div>

        <div class="flex flex-col gap-5">
          <BaseInput
            v-model="payload.venueName"
            label="Tên địa điểm *"
            placeholder="Ví dụ: Nhà hát lớn Hà Nội"
            :required="true"
          />
          <BaseInput
            v-model="payload.totalCapacity"
            label="Sức chứa tổng *"
            type="number"
            placeholder="Ví dụ: 50000"
            :required="true"
          />
          <BaseInput
            v-model="payload.phoneNumber"
            label="Số điện thoại"
            type="tel"
            placeholder="+84 xxx xxx xxx"
          />
          <BaseInput
            v-model="payload.websiteUrl"
            label="Website"
            type="url"
            placeholder="https://..."
          />
        </div>
      </div>

      <!-- Panel 2: Location -->
      <div class="glass-panel flex flex-col gap-2">
        <div class="flex items-center gap-3 mb-4 pb-4 border-b border-border-main">
          <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-xl">📍</div>
          <h2 class="font-heading text-xl font-bold text-main">Vị trí</h2>
        </div>

        <div class="flex flex-col gap-5">
          <BaseInput
            v-model="payload.addressLine"
            label="Địa chỉ *"
            placeholder="Số nhà, tên đường..."
            :required="true"
          />

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
            <BaseInput
              v-model="payload.ward"
              label="Phường / Xã"
              placeholder="Phường 1"
            />
            <BaseInput
              v-model="payload.district"
              label="Quận / Huyện"
              placeholder="Quận 1"
            />
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
            <BaseInput
              v-model="payload.provinceCity"
              label="Tỉnh / Thành phố *"
              placeholder="TP. Hồ Chí Minh"
              :required="true"
            />
            <BaseInput
              v-model="payload.country"
              label="Quốc gia"
              placeholder="Việt Nam"
            />
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
            <BaseInput
              v-model="payload.longitude"
              label="Kinh độ"
              type="number"
              step="any"
              placeholder="106.6297"
            />
            <BaseInput
              v-model="payload.latitude"
              label="Vĩ độ"
              type="number"
              step="any"
              placeholder="10.8231"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Actions -->
    <div class="flex flex-col sm:flex-row gap-4 mt-10">
      <BaseButton
        variant="primary"
        size="lg"
        :loading="isSubmitting"
        class="!py-4 !rounded-2xl shadow-xl shadow-primary/20 flex-1 sm:flex-none"
        @click="handleSubmit"
      >
        Tạo địa điểm
      </BaseButton>
      <BaseButton
        variant="ghost"
        size="lg"
        class="!rounded-2xl"
        @click="resetForm"
      >
        Đặt lại
      </BaseButton>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import BaseInput from '@/components/ui/BaseInput.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import { useVenueStore } from '@/stores/useVenueStore.js'
import { addToast } from '@/stores/adminStore.js'

const venueStore = useVenueStore()
const isSubmitting = ref(false)

const getInitialPayload = () => ({
  venueName: '',
  totalCapacity: '',
  phoneNumber: '',
  websiteUrl: '',
  addressLine: '',
  ward: '',
  district: '',
  provinceCity: '',
  country: '',
  longitude: '',
  latitude: ''
})

const payload = reactive(getInitialPayload())

function resetForm() {
  Object.assign(payload, getInitialPayload())
}

async function handleSubmit() {
  if (isSubmitting.value) return
  isSubmitting.value = true

  try {
    const submissionPayload = {
      ...payload,
      totalCapacity: Number(payload.totalCapacity),
      longitude: Number(payload.longitude),
      latitude: Number(payload.latitude)
    }

    const response = await venueStore.createVenue(submissionPayload)

    if (response.success) {
      addToast(response.message, 'success')
      resetForm()
    } else {
      addToast(response.message, 'error')
    }
  } catch (err) {
    addToast('Đã xảy ra lỗi khi tạo địa điểm.', 'error')
  } finally {
    isSubmitting.value = false
  }
}
</script>
