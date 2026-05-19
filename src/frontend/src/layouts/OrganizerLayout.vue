<template>
  <div class="flex h-screen w-full bg-[#050806] text-white overflow-hidden font-sans">
    
    <!-- LEFT SIDEBAR -->
    <aside class="w-64 bg-[#0A0F0D] border-r border-white/5 flex flex-col flex-shrink-0 z-20">
      <!-- Logo Area -->
      <div class="h-20 flex items-center px-6 border-b border-white/5">
        <router-link to="/organizer" class="flex items-center gap-3 group">
          <div class="w-8 h-8 bg-primary text-black rounded-lg flex items-center justify-center font-bold text-lg shadow-[0_0_15px_rgba(0,200,83,0.2)] group-hover:scale-110 transition-transform">
            ◉
          </div>
          <div class="flex flex-col">
            <span class="font-heading font-black text-sm tracking-tight uppercase text-white leading-none">TicketHub</span>
            <span class="text-[10px] font-bold text-primary uppercase tracking-widest mt-0.5">Organizer Center</span>
          </div>
        </router-link>
      </div>

      <!-- Navigation List -->
      <nav class="flex-1 py-6 px-4 flex flex-col gap-2 overflow-y-auto custom-scrollbar">
        <router-link 
          to="/organizer" 
          class="flex items-center gap-3 px-4 py-3 rounded-xl text-[14px] font-bold text-muted hover:text-white hover:bg-white/5 transition-all group"
          exact-active-class="!text-primary bg-primary/10"
        >
          <span class="text-xl opacity-60 group-hover:opacity-100 transition-opacity">📅</span>
          Sự kiện của tôi
        </router-link>
        
        <a 
          href="#" 
          class="flex items-center gap-3 px-4 py-3 rounded-xl text-[14px] font-bold text-muted hover:text-white hover:bg-white/5 transition-all group"
        >
          <span class="text-xl opacity-60 group-hover:opacity-100 transition-opacity">📊</span>
          Quản lý báo cáo
        </a>
        
        <a 
          href="#" 
          class="flex items-center gap-3 px-4 py-3 rounded-xl text-[14px] font-bold text-muted hover:text-white hover:bg-white/5 transition-all group"
        >
          <span class="text-xl opacity-60 group-hover:opacity-100 transition-opacity">📜</span>
          Điều khoản
        </a>
      </nav>

      <!-- Language Toggle / Bottom Footer -->
      <div class="p-4 border-t border-white/5">
        <button class="flex items-center gap-2 px-4 py-2 w-full rounded-xl bg-white/5 hover:bg-white/10 transition-colors text-sm font-bold text-muted hover:text-white">
          <span>🌐</span> Tiếng Việt
        </button>
      </div>
    </aside>

    <!-- MAIN AREA -->
    <main class="flex-1 flex flex-col min-w-0 relative">
      
      <!-- TOPBAR -->
      <header class="h-20 bg-[#0A0F0D]/80 backdrop-blur-md border-b border-white/5 flex items-center justify-between px-8 shrink-0 z-10">
        
        <!-- Breadcrumb / Title (Optional placeholder) -->
        <div class="flex items-center gap-2 text-sm font-medium text-muted">
          <span>Trang chủ</span>
          <span>/</span>
          <span class="text-white">Khu vực BTC</span>
        </div>

        <!-- Topbar Actions -->
        <div class="flex items-center gap-4">
          <router-link 
            to="/create-event"
            class="flex items-center gap-2 bg-primary hover:bg-primary-dim text-black px-4 py-2.5 rounded-xl font-bold text-[13px] shadow-[0_0_15px_rgba(0,200,83,0.3)] hover:-translate-y-0.5 transition-all"
          >
            ➕ Tạo sự kiện
          </router-link>

          <div class="w-px h-6 bg-white/10 mx-2"></div>

          <!-- User Profile Indicator -->
          <div class="flex items-center gap-3 cursor-pointer group p-1.5 rounded-xl hover:bg-white/5 transition-colors">
            <div class="flex flex-col items-end hidden sm:flex">
              <span class="text-[13px] font-bold text-white">Organizer Admin</span>
              <span class="text-[11px] text-muted">admin@tickethub.vn</span>
            </div>
            <div class="w-10 h-10 rounded-xl bg-surface border border-white/10 flex items-center justify-center font-black text-primary group-hover:border-primary/50 transition-colors">
              OA
            </div>
          </div>
        </div>
      </header>

      <!-- CONTENT VIEW -->
      <div class="flex-1 overflow-y-auto custom-scrollbar bg-[#050806] relative">
        <!-- We use padding to ensure the nested pages don't need excessive top padding anymore -->
        <router-view class="!pt-10 !pb-20" />
      </div>

    </main>

  </div>
</template>

<script setup>
// OrganizerLayout Logic
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.2);
}
</style>
