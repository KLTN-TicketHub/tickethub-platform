<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Recommendation Engine</h1>
        <p class="text-white/50 font-medium text-lg">Quản lý mô hình gợi ý sự kiện cho người dùng (Collaborative Filtering)</p>
      </div>
      <BaseButton
        variant="primary"
        class="!rounded-2xl !py-3.5 !px-6 flex items-center gap-2 self-start sm:self-auto"
        :disabled="isTraining"
        @click="handleTrain"
      >
        <PhSpinner v-if="isTraining" class="animate-spin text-lg" weight="bold" />
        <PhSparkle v-else weight="bold" />
        {{ isTraining ? 'Đang train...' : 'Train ngay' }}
      </BaseButton>
    </div>

    <!-- Status -->
    <div v-if="isLoading" class="py-20 flex flex-col items-center justify-center gap-3">
      <PhSpinner class="animate-spin text-primary text-4xl" weight="bold" />
      <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải trạng thái...</span>
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-3 gap-6">
      <div class="bg-[#111916] border border-white/5 rounded-[2rem] p-6 flex flex-col gap-2">
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-widest">Lần train gần nhất</span>
        <span class="text-2xl font-black text-white">{{ lastTrainedLabel }}</span>
      </div>
      <div class="bg-[#111916] border border-white/5 rounded-[2rem] p-6 flex flex-col gap-2">
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-widest">Tổng lượt tương tác</span>
        <span class="text-2xl font-black text-primary">{{ status.interactionCount ?? 0 }}</span>
      </div>
      <div class="bg-[#111916] border border-white/5 rounded-[2rem] p-6 flex flex-col gap-2">
        <span class="text-[12px] font-bold text-white/40 uppercase tracking-widest">Số user có gợi ý</span>
        <span class="text-2xl font-black text-white">{{ status.userWithRecommendationCount ?? 0 }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { addToast } from '../../stores/adminStore'
import { getRecommendationStatus, trainRecommendationModel } from '../../services/admin-recommendation.service'
import BaseButton from '../../components/ui/BaseButton.vue'
import { PhSparkle, PhSpinner } from '@phosphor-icons/vue'

const isLoading = ref(true)
const isTraining = ref(false)
const status = ref({ lastTrainedAt: null, interactionCount: 0, userWithRecommendationCount: 0 })

const lastTrainedLabel = computed(() => {
  if (!status.value.lastTrainedAt) return 'Chưa train lần nào'
  return new Date(status.value.lastTrainedAt).toLocaleString('vi-VN')
})

const fetchStatus = async () => {
  isLoading.value = true
  try {
    const res = await getRecommendationStatus()
    if (res && res.success && res.data) {
      status.value = res.data
    }
  } catch (err) {
    console.error('Error fetching recommendation status:', err)
    addToast('Không thể tải trạng thái mô hình recommendation.', 'error')
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchStatus()
})

const handleTrain = async () => {
  isTraining.value = true
  try {
    const res = await trainRecommendationModel()
    if (res && res.success) {
      addToast('Đã đồng bộ dữ liệu và train lại mô hình recommendation.', 'success')
      await fetchStatus()
    } else {
      addToast(res?.message || 'Train mô hình thất bại.', 'error')
    }
  } catch (err) {
    console.error('Error training recommendation model:', err)
    const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra khi train mô hình.'
    addToast(errorMsg, 'error')
  } finally {
    isTraining.value = false
  }
}
</script>

<style scoped>
</style>
