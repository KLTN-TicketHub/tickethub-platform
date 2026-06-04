<template>
  <div class="animate-fade-up max-w-[600px] mx-auto py-10 px-4 min-h-[80vh]">
    <!-- Header -->
    <div class="mb-10">
      <h1 class="font-heading text-3xl lg:text-4xl font-bold text-main mb-2">
        Tạo danh mục mới
      </h1>
      <p class="text-muted font-medium">
        Thêm danh mục sự kiện vào hệ thống TicketHub
      </p>
    </div>

    <!-- Form Panel -->
    <div class="glass-panel flex flex-col gap-2">
      <div class="flex items-center gap-3 mb-4 pb-4 border-b border-border-main">
        <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-xl">📂</div>
        <h2 class="font-heading text-xl font-bold text-main">Thông tin danh mục</h2>
      </div>

      <div class="flex flex-col gap-5">
        <BaseInput
          v-model="payload.categoryName"
          label="Tên danh mục *"
          placeholder="Ví dụ: Concert, Thể thao, Hội thảo..."
          :required="true"
        />
        <BaseInput
          v-model="payload.description"
          label="Mô tả"
          placeholder="Mô tả ngắn về danh mục này..."
        />
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
        Tạo danh mục
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
import { useCategoryStore } from '@/stores/categoryStore.js'
import { addToast } from '@/stores/adminStore.js'

const categoryStore = useCategoryStore()
const isSubmitting = ref(false)

const getInitialPayload = () => ({
  categoryName: '',
  description: ''
})

const payload = reactive(getInitialPayload())

function resetForm() {
  Object.assign(payload, getInitialPayload())
}

async function handleSubmit() {
  if (isSubmitting.value) return
  isSubmitting.value = true

  try {
    const response = await categoryStore.createCategory({ ...payload })

    if (response.success) {
      addToast(response.message, 'success')
      resetForm()
    } else {
      addToast(response.message, 'error')
    }
  } catch (err) {
    addToast('Đã xảy ra lỗi khi tạo danh mục.', 'error')
  } finally {
    isSubmitting.value = false
  }
}
</script>
