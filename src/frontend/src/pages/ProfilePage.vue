<template>
  <div class="max-w-4xl mx-auto py-20 px-6 animate-fade-up">
    <!-- Header -->
    <div class="flex flex-col md:flex-row items-center gap-8 mb-12">
      <div class="w-32 h-32 rounded-[2.5rem] bg-gradient-to-br from-primary to-primary-dark text-black flex items-center justify-center text-5xl font-black shadow-2xl shadow-primary/20">
        {{ store.user?.initial || 'T' }}
      </div>
      <div class="text-center md:text-left space-y-2">
        <h1 class="text-4xl font-bold font-heading text-main tracking-tight">
          {{ store.user?.name || 'Trần Tuấn Phương' }}
        </h1>
        <p class="text-lg text-muted font-medium">
          {{ store.user?.email || 'phuong.tran@example.com' }}
        </p>
        <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-bold tracking-widest uppercase">
          Hạng thành viên: Gold ✨
        </div>
      </div>
    </div>
    
    <!-- Info Section -->
    <div class="space-y-8">
      <h2 class="font-heading text-xl font-bold text-main uppercase tracking-widest flex items-center gap-3">
        <span class="w-8 h-1 bg-primary rounded-full"></span>
        Thông tin tài khoản
      </h2>
      
      <div class="bg-card border border-border-main rounded-[2rem] p-8 space-y-8">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div class="space-y-1">
            <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Họ và tên</span>
            <p class="text-[15px] font-bold text-main border-b border-white/5 pb-2">{{ store.user?.name || 'Trần Tuấn Phương' }}</p>
          </div>
          <div class="space-y-1">
            <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Email</span>
            <p class="text-[15px] font-bold text-main border-b border-white/5 pb-2">{{ store.user?.email || 'phuong.tran@example.com' }}</p>
          </div>
          <div class="space-y-1">
            <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Số điện thoại</span>
            <p class="text-[15px] font-bold text-main border-b border-white/5 pb-2">0901 *** 789</p>
          </div>
          <div class="space-y-1">
            <span class="text-[11px] font-bold text-muted uppercase tracking-widest">Ngày tham gia</span>
            <p class="text-[15px] font-bold text-main border-b border-white/5 pb-2">12/05/2024</p>
          </div>
        </div>

        <div class="p-6 bg-white/5 rounded-2xl border border-white/5 flex items-center justify-between">
          <div class="flex items-center gap-4">
            <span class="text-2xl">🔐</span>
            <div class="flex flex-col">
              <span class="text-[13px] font-bold text-main">Bảo mật tài khoản</span>
              <span class="text-[11px] text-muted font-medium">Xác thực 2 yếu tố đang bật</span>
            </div>
          </div>
          <BaseButton variant="ghost" size="sm">Cài đặt</BaseButton>
        </div>
      </div>
      
      <div class="flex justify-end gap-4">
        <BaseButton variant="outline" class="!px-8">Đổi mật khẩu</BaseButton>
        <BaseButton variant="primary" class="!px-10">Cập nhật thông tin</BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
</script>
