<template>
  <section class="bg-surface border border-border-main rounded-[2.5rem] p-6 md:p-8 shadow-2xl">
    <h2 class="font-heading text-2xl font-black text-main uppercase tracking-wider mb-2">{{ title }}</h2>

    <div v-if="isLoading" class="py-12 flex justify-center">
      <PhSpinner class="animate-spin text-primary text-3xl" weight="bold" />
    </div>
    <div v-else-if="error" class="py-8 text-center text-muted font-bold text-[13px]">
      {{ error }}
    </div>
    <div v-else-if="!data || !data.suggestions || data.suggestions.length === 0" class="py-8 text-center text-muted font-bold text-[13px]">
      Chưa có đủ dữ liệu để đưa ra gợi ý trong khoảng thời gian này.
    </div>
    <template v-else>
      <p class="text-muted text-[13px] mb-5">{{ data.summary }}</p>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <SuggestionCard
          v-for="(s, idx) in data.suggestions"
          :key="idx"
          :category="s.category"
          :title="s.title"
          :detail="s.detail"
        />
      </div>
    </template>
  </section>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { PhSpinner } from '@phosphor-icons/vue'
import SuggestionCard from './SuggestionCard.vue'
import { getErrorMessage } from '../../utils/apiError'

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  fetcher: {
    type: Function,
    required: true
  },
  range: {
    type: String,
    default: '30d'
  }
})

const isLoading = ref(true)
const error = ref('')
const data = ref(null)

const load = async () => {
  isLoading.value = true
  error.value = ''
  try {
    const res = await props.fetcher(props.range)
    if (res && res.success) {
      data.value = res.data
    } else {
      error.value = res?.message || 'Không thể tải gợi ý AI.'
    }
  } catch (err) {
    console.error('Error fetching AI suggestions:', err)
    error.value = getErrorMessage(err, 'Không thể tạo gợi ý AI lúc này. Vui lòng thử lại sau.')
  } finally {
    isLoading.value = false
  }
}

watch(() => props.range, load)

onMounted(load)
</script>
