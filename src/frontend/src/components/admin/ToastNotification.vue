<template>
  <div class="fixed bottom-8 right-8 z-[9999] flex flex-col gap-4 pointer-events-none">
    <transition-group 
      name="toast" 
      tag="div" 
      class="flex flex-col gap-3 items-end"
    >
      <div 
        v-for="toast in toasts" 
        :key="toast.id" 
        class="pointer-events-auto flex items-center gap-4 px-5 py-4 bg-card/80 backdrop-blur-xl border border-border-main rounded-2xl shadow-2xl shadow-black/40 min-w-[320px] max-w-[420px] group animate-in slide-in-from-right-8 fade-in duration-300"
      >
        <div class="flex-shrink-0 w-10 h-10 rounded-xl flex items-center justify-center shadow-inner" :class="getTypeClasses(toast.type)">
          <svg v-if="toast.type === 'success'" class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
          <svg v-else-if="toast.type === 'error'" class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>
          <svg v-else-if="toast.type === 'warning'" class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
        </div>
        
        <div class="flex-1">
          <p class="text-[14px] font-bold text-main">{{ toast.message }}</p>
        </div>

        <button 
          @click="removeToast(toast.id)"
          class="p-1.5 rounded-lg text-muted hover:text-main hover:bg-surface/50 transition-all opacity-0 group-hover:opacity-100 focus:opacity-100"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
        </button>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
import { toasts, removeToast } from '../../stores/adminStore'

const getTypeClasses = (type) => {
  if (type === 'success') return 'bg-primary/10 text-primary'
  if (type === 'error') return 'bg-danger/10 text-danger'
  if (type === 'warning') return 'bg-warning/10 text-warning'
  return 'bg-surface text-muted'
}
</script>

<style>
.toast-move,
.toast-enter-active,
.toast-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.toast-leave-active {
  position: absolute;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateX(40px) scale(0.9);
}
</style>

