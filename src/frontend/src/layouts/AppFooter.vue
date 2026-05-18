<template>
  <footer class="bg-surface border-t border-border-main pt-24 pb-12 overflow-hidden relative">
    <!-- Decorative Background Element -->
    <div class="absolute -top-24 -right-24 w-96 h-96 bg-primary/5 blur-[120px] rounded-full pointer-events-none"></div>
    
    <div class="max-w-[1440px] mx-auto px-6 md:px-10">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-12 gap-16 mb-20">
        
        <!-- Brand & About -->
        <div class="lg:col-span-4 space-y-8">
          <router-link to="/" class="flex items-center gap-4 group">
            <div class="w-12 h-12 bg-primary text-black rounded-[14px] flex items-center justify-center font-bold text-2xl shadow-[0_0_30px_rgba(0,200,83,0.3)] group-hover:scale-110 transition-all">
              ◉
            </div>
              <span class="font-heading font-black text-2xl tracking-tighter text-white uppercase">TicketHub</span>
          </router-link>
          
          <p class="text-muted font-medium leading-relaxed max-w-sm">
            TicketHub là nền tảng phân phối vé sự kiện hàng đầu, mang đến những trải nghiệm giải trí đẳng cấp và kết nối cộng đồng qua âm nhạc, nghệ thuật và thể thao.
          </p>

          <div class="flex items-center gap-4">
            <a v-for="s in SOCIALS" :key="s.name" href="#" class="w-10 h-10 rounded-xl bg-white/5 border border-white/10 flex items-center justify-center text-lg hover:bg-primary hover:text-black hover:border-primary transition-all duration-300">
              {{ s.icon }}
            </a>
          </div>
        </div>

        <!-- Links Columns -->
        <div v-for="col in FOOTER_COLS" :key="col.title" class="lg:col-span-2 space-y-8">
          <h4 class="text-[13px] font-bold text-white uppercase tracking-[0.2em]">{{ col.title }}</h4>
          <ul class="space-y-4">
            <li v-for="link in col.links" :key="link.label">
                <a href="#" class="text-[15px] font-medium text-muted hover:text-primary transition-colors flex items-center gap-2 group">
                <span class="w-1 h-1 rounded-full bg-primary opacity-0 group-hover:opacity-100 transition-opacity"></span>
                {{ link.label }}
              </a>
            </li>
          </ul>
        </div>

        <!-- Newsletter / App -->
        <div class="lg:col-span-2 space-y-8">
          <h4 class="text-[13px] font-bold text-white uppercase tracking-[0.2em]">Tải ứng dụng</h4>
          <div class="space-y-4">
            <a href="#" class="block bg-card border border-white/10 rounded-2xl p-4 hover:border-primary transition-all group">
              <div class="flex items-center gap-3">
                <span class="text-2xl group-hover:scale-110 transition-transform">🍎</span>
                <div class="flex flex-col">
                  <span class="text-[10px] text-muted font-bold uppercase">Download on</span>
                  <span class="text-[14px] font-bold text-white">App Store</span>
                </div>
              </div>
            </a>
            <a href="#" class="block bg-card border border-white/10 rounded-2xl p-4 hover:border-primary transition-all group">
              <div class="flex items-center gap-3">
                <span class="text-2xl group-hover:scale-110 transition-transform">🤖</span>
                <div class="flex flex-col">
                  <span class="text-[10px] text-muted font-bold uppercase">Get it on</span>
                  <span class="text-[14px] font-bold text-white">Google Play</span>
                </div>
              </div>
            </a>
          </div>
        </div>
      </div>

      <!-- Bottom Bar -->
      <div class="pt-10 border-t border-white/5 flex flex-col md:flex-row justify-between items-center gap-8">
        <div class="flex flex-col md:flex-row items-center gap-4 md:gap-12">
          <span class="text-[13px] text-muted font-medium">© 2025 TicketHub Corporation. All rights reserved.</span>
          <div class="flex items-center gap-6">
            <a href="#" class="text-[12px] text-muted hover:text-white transition-colors">Điều khoản sử dụng</a>
            <a href="#" class="text-[12px] text-muted hover:text-white transition-colors">Chính sách bảo mật</a>
          </div>
        </div>
        
        <div class="flex items-center gap-3 text-muted font-bold text-[11px] uppercase tracking-widest">
          Made with <span class="text-primary animate-pulse">💚</span> in Vietnam
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
const SOCIALS = [
  { name: 'FB', icon: 'f' },
  { name: 'IG', icon: 'i' },
  { name: 'YT', icon: 'y' },
  { name: 'TT', icon: 't' },
]

const FOOTER_COLS = [
  {
    title: '🎫 Sự kiện',
    links: [
      { label: 'Âm nhạc & Concert' },
      { label: 'Sân khấu & Nghệ thuật' },
      { label: 'Thể thao & Esports' },
      { label: 'Hội thảo & Workshop' },
      { label: 'Triển lãm & Trải nghiệm' },
    ]
  },
  {
    title: '🎧 Hỗ trợ',
    links: [
      { label: 'Trung tâm trợ giúp' },
      { label: 'Liên hệ hỗ trợ' },
      { label: 'Chính sách hoàn tiền' },
      { label: 'Câu hỏi thường gặp (FAQ)' },
      { label: 'Bảo mật thông tin' },
    ]
  },
  {
    title: '🏢 Công ty',
    links: [
      { label: 'Về TicketHub' },
      { label: 'Tin tức & Báo chí' },
      { label: 'Cơ hội nghề nghiệp' },
      { label: 'Trở thành đối tác' },
      { label: 'Liên hệ hợp tác' },
    ]
  }
]
</script>
