<template>
  <div class="max-w-[1400px] mx-auto py-16 px-6 lg:px-10 min-h-[80vh]">
    <div class="grid grid-cols-1 md:grid-cols-3 xl:grid-cols-4 gap-6 animate-fade-up">
      <!-- Main User Identity Card (Spans 2 cols) -->
      <div class="md:col-span-2 xl:col-span-2 bg-[#111916] border border-white/5 rounded-[3rem] p-8 lg:p-12 shadow-2xl relative overflow-hidden group">
        <!-- Decor -->
        <div class="absolute -top-32 -right-32 w-64 h-64 bg-primary/20 blur-[100px] rounded-full pointer-events-none group-hover:scale-150 transition-transform duration-1000"></div>
        <div class="absolute bottom-0 left-0 w-full h-1/2 bg-gradient-to-t from-black/20 to-transparent pointer-events-none"></div>

        <div class="flex flex-col sm:flex-row items-center sm:items-start gap-8 relative z-10">
          <div class="relative">
            <div class="w-32 h-32 lg:w-40 lg:h-40 rounded-[2.5rem] bg-gradient-to-br from-[#00E676] to-[#00A355] text-black flex items-center justify-center text-6xl font-black shadow-[0_0_40px_rgba(0,200,83,0.4)] border-4 border-[#050807]">
              {{ store.user?.initial || 'T' }}
            </div>
            <div class="absolute -bottom-4 -right-4 w-12 h-12 bg-[#050807] rounded-full flex items-center justify-center">
              <div class="w-10 h-10 bg-[#111916] border border-white/10 rounded-full flex items-center justify-center text-white/50 hover:text-white hover:border-primary transition-all cursor-pointer shadow-inner">
                <PhCamera weight="fill" />
              </div>
            </div>
          </div>
          
          <div class="text-center sm:text-left space-y-3 flex-1">
            <div class="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-warning/10 border border-warning/20 text-warning text-[11px] font-black tracking-widest uppercase shadow-[0_0_20px_rgba(255,193,7,0.1)] mb-2">
              <PhCrown weight="fill" /> Hạng thành viên: Gold
            </div>
            <h1 class="text-4xl lg:text-5xl font-black font-heading text-white tracking-tight leading-none">
              {{ store.user?.name || 'Trần Tuấn Phương' }}
            </h1>
            <p class="text-lg text-white/50 font-medium pb-2">
              {{ store.user?.email || 'phuong.tran@example.com' }}
            </p>
            <div class="flex flex-wrap items-center justify-center sm:justify-start gap-3 pt-2">
              <BaseButton variant="primary" class="!px-6 !py-2 !rounded-xl text-sm">Chỉnh sửa hồ sơ</BaseButton>
              <BaseButton variant="ghost" class="!px-6 !py-2 !rounded-xl text-sm bg-white/5 border border-white/10">Đăng xuất</BaseButton>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Stats Card 1 -->
      <div class="bg-[#111916] border border-white/5 rounded-[3rem] p-8 shadow-2xl flex flex-col justify-between group cursor-pointer hover:border-primary/30 transition-colors">
        <div class="flex items-center justify-between mb-8">
          <div class="w-12 h-12 bg-white/5 border border-white/10 rounded-2xl flex items-center justify-center text-primary text-xl shadow-inner group-hover:scale-110 transition-transform">
            <PhTicket weight="fill" />
          </div>
          <PhArrowUpRight class="text-white/30 group-hover:text-white transition-colors" weight="bold" />
        </div>
        <div class="space-y-1">
          <h3 class="text-5xl font-black font-heading text-white tracking-tighter">{{ store.tickets?.length || 12 }}</h3>
          <p class="text-[13px] font-bold text-white/40 uppercase tracking-widest">Vé đã đặt</p>
        </div>
      </div>

      <!-- Quick Stats Card 2 -->
      <div class="bg-[#111916] border border-white/5 rounded-[3rem] p-8 shadow-2xl flex flex-col justify-between group cursor-pointer hover:border-primary/30 transition-colors xl:col-start-4">
        <div class="flex items-center justify-between mb-8">
          <div class="w-12 h-12 bg-white/5 border border-white/10 rounded-2xl flex items-center justify-center text-[#f43f5e] text-xl shadow-inner group-hover:scale-110 transition-transform">
            <PhHeart weight="fill" />
          </div>
          <PhArrowUpRight class="text-white/30 group-hover:text-white transition-colors" weight="bold" />
        </div>
        <div class="space-y-1">
          <h3 class="text-5xl font-black font-heading text-white tracking-tighter">5</h3>
          <p class="text-[13px] font-bold text-white/40 uppercase tracking-widest">Sự kiện đã lưu</p>
        </div>
      </div>

      <!-- Personal Information Card -->
      <div class="md:col-span-2 xl:col-span-2 bg-[#111916] border border-white/5 rounded-[3rem] p-8 lg:p-12 shadow-2xl">
        <div class="flex items-center justify-between mb-8">
          <h2 class="font-heading text-2xl font-black text-white flex items-center gap-3">
            <PhUserCircle weight="fill" class="text-primary" /> Thông tin cá nhân
          </h2>
        </div>
        
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-8">
          <div class="space-y-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Họ và tên</label>
            <div class="p-4 bg-[#0A0F0D] rounded-2xl border border-white/5 text-[15px] font-bold text-white group-focus-within:border-primary/50 transition-colors flex items-center justify-between cursor-text">
              {{ store.user?.name || 'Trần Tuấn Phương' }}
              <PhPencilSimple class="text-white/20 hover:text-white transition-colors cursor-pointer" weight="bold" />
            </div>
          </div>
          <div class="space-y-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Email</label>
            <div class="p-4 bg-[#0A0F0D] rounded-2xl border border-white/5 text-[15px] font-bold text-white/70 flex items-center justify-between cursor-not-allowed">
              {{ store.user?.email || 'phuong.tran@example.com' }}
              <PhLockKey class="text-white/20" weight="bold" />
            </div>
          </div>
          <div class="space-y-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Số điện thoại</label>
            <div class="p-4 bg-[#0A0F0D] rounded-2xl border border-white/5 text-[15px] font-bold text-white flex items-center justify-between cursor-text">
              0901 *** 789
              <PhPencilSimple class="text-white/20 hover:text-white transition-colors cursor-pointer" weight="bold" />
            </div>
          </div>
          <div class="space-y-2">
            <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Ngày tham gia</label>
            <div class="p-4 bg-[#0A0F0D] rounded-2xl border border-white/5 text-[15px] font-bold text-white/70 flex items-center justify-between">
              12/05/2024
              <PhCalendarBlank class="text-white/20" weight="bold" />
            </div>
          </div>
        </div>
      </div>

      <!-- Security & Settings Card -->
      <div class="md:col-span-1 xl:col-span-2 bg-[#111916] border border-white/5 rounded-[3rem] p-8 lg:p-12 shadow-2xl flex flex-col justify-between">
        <h2 class="font-heading text-2xl font-black text-white flex items-center gap-3 mb-8">
          <PhShieldCheck weight="fill" class="text-primary" /> Bảo mật & Cài đặt
        </h2>

        <div class="space-y-4 flex-1">
          <div class="p-5 bg-[#0A0F0D] rounded-2xl border border-white/5 flex items-center justify-between group hover:border-white/20 transition-colors cursor-pointer">
            <div class="flex items-center gap-4">
              <div class="w-10 h-10 rounded-xl bg-white/5 flex items-center justify-center text-white/50 group-hover:text-primary transition-colors">
                <PhKey weight="fill" />
              </div>
              <div class="flex flex-col">
                <span class="text-[14px] font-bold text-white">Đổi mật khẩu</span>
                <span class="text-[12px] text-white/50 font-medium">Cập nhật mật khẩu mới</span>
              </div>
            </div>
            <PhCaretRight class="text-white/30 group-hover:text-white transition-colors" weight="bold" />
          </div>

          <div class="p-5 bg-[#0A0F0D] rounded-2xl border border-white/5 flex items-center justify-between group hover:border-white/20 transition-colors cursor-pointer">
            <div class="flex items-center gap-4">
              <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-primary">
                <PhFingerprint weight="fill" />
              </div>
              <div class="flex flex-col">
                <span class="text-[14px] font-bold text-white">Xác thực 2 yếu tố</span>
                <span class="text-[12px] text-primary font-bold">Đang bật</span>
              </div>
            </div>
            <BaseButton variant="ghost" size="sm" class="!px-4 !py-1.5 !rounded-lg bg-white/5 hover:bg-white/10 text-white text-xs">Tùy chỉnh</BaseButton>
          </div>
        </div>
      </div>

      <!-- Payment Methods (Placeholder) -->
      <div class="md:col-span-3 xl:col-span-4 bg-[#111916] border border-white/5 rounded-[3rem] p-8 lg:p-12 shadow-2xl">
        <div class="flex items-center justify-between mb-8">
          <h2 class="font-heading text-2xl font-black text-white flex items-center gap-3">
            <PhCreditCard weight="fill" class="text-primary" /> Phương thức thanh toán
          </h2>
          <BaseButton variant="ghost" class="!bg-white/5 !border-white/10 !rounded-xl flex items-center gap-2">
            <PhPlus weight="bold" /> Thêm thẻ
          </BaseButton>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="p-6 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-[2rem] relative overflow-hidden shadow-xl shadow-indigo-500/20">
            <div class="absolute -right-10 -top-10 w-32 h-32 bg-white/10 rounded-full blur-2xl"></div>
            <PhCreditCard class="text-white/50 text-4xl mb-6" weight="duotone" />
            <div class="space-y-1 relative z-10">
              <p class="text-[14px] font-mono font-bold text-white tracking-[0.2em]">**** **** **** 1234</p>
              <div class="flex items-center justify-between text-white/70 text-[12px] font-bold uppercase tracking-widest mt-4">
                <span>Trần Tuấn Phương</span>
                <span>12/26</span>
              </div>
            </div>
          </div>
          
          <div class="p-6 bg-[#0A0F0D] border border-white/5 border-dashed rounded-[2rem] flex flex-col items-center justify-center text-white/30 hover:text-white hover:border-white/20 transition-all cursor-pointer group min-h-[160px]">
            <PhPlus class="text-3xl mb-2 group-hover:scale-110 transition-transform" weight="bold" />
            <span class="text-[13px] font-bold">Thêm phương thức mới</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { store } from '../stores/eventStore'
import BaseButton from '../components/ui/BaseButton.vue'
import { 
  PhCamera, PhCrown, PhTicket, PhArrowUpRight, PhHeart, 
  PhUserCircle, PhPencilSimple, PhLockKey, PhCalendarBlank,
  PhShieldCheck, PhKey, PhCaretRight, PhFingerprint,
  PhCreditCard, PhPlus
} from '@phosphor-icons/vue'
</script>

<style scoped>
</style>
