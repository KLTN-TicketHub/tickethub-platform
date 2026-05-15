<template>
  <transition name="toast-slide">
    <div class="toast" v-if="visible" @click="dismiss">
      <span class="toast-icon">{{ icon || '✅' }}</span>
      <p>{{ message }}</p>
      <span class="toast-close">✕</span>
    </div>
  </transition>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({ message: String, icon: String })
const emit = defineEmits(['close'])

const visible = ref(true)

const dismiss = () => {
  visible.value = false
  setTimeout(() => emit('close'), 300)
}

onMounted(() => {
  setTimeout(dismiss, 3500)
})
</script>

<style scoped>
.toast-close {
  cursor: pointer;
  color: var(--color-muted);
  margin-left: 8px;
  font-size: 14px;
}
.toast-close:hover {
  color: var(--color-main);
}
</style>
