<template>
  <div 
    v-if="confirmModalState.isOpen" 
    class="fixed inset-0 z-[10000] flex items-center justify-center p-4"
  >
    <!-- Backdrop -->
    <div 
      class="absolute inset-0 bg-black/80 backdrop-blur-sm animate-in fade-in duration-300"
      @click="close"
    ></div>

    <!-- Modal Content -->
    <div 
      class="relative bg-card/90 backdrop-blur-2xl border border-border-main rounded-[32px] w-full max-w-md overflow-hidden shadow-2xl shadow-black/60 animate-in zoom-in-95 fade-in duration-300"
    >
      <div class="p-8 flex flex-col items-center text-center">
        <!-- Icon Container -->
        <div class="w-16 h-16 rounded-2xl bg-danger/10 text-danger flex items-center justify-center mb-6 shadow-inner animate-bounce-short">
          <svg class="w-8 h-8" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
        </div>

        <h3 class="text-2xl font-bold text-main mb-3 font-heading">{{ confirmModalState.title }}</h3>
        <p class="text-[15px] text-muted leading-relaxed">{{ confirmModalState.message }}</p>
      </div>

      <!-- Footer Buttons -->
      <div class="px-8 pb-8 flex gap-3">
        <BaseButton 
          variant="outline" 
          size="lg" 
          class="flex-1 !rounded-2xl" 
          @click="close"
        >
          Hủy bỏ
        </BaseButton>
        <BaseButton 
          variant="danger" 
          size="lg" 
          class="flex-1 !rounded-2xl shadow-lg shadow-danger/20" 
          @click="confirm"
        >
          Xác nhận
        </BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { confirmModalState, closeConfirm } from '../../stores/adminStore'
import BaseButton from '../../shared/components/BaseButton.vue'

const close = () => {
  closeConfirm()
}

const confirm = () => {
  if (confirmModalState.onConfirm) {
    confirmModalState.onConfirm()
  }
  closeConfirm()
}
</script>



