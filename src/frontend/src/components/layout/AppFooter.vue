<template>
  <footer class="bg-bg border-t border-white/5 pt-20 pb-12 overflow-hidden relative">
    <!-- Decorative -->
    <div class="absolute top-0 right-1/4 w-[500px] h-[500px] bg-primary/5 blur-[120px] rounded-full pointer-events-none"></div>
    
    <div class="max-w-[1440px] mx-auto px-6 md:px-10">
      <div class="flex flex-col lg:flex-row justify-between gap-16 mb-20 relative z-10">
        
        <!-- Brand -->
        <div class="lg:w-1/3 space-y-6">
          <router-link to="/" class="flex items-center gap-3 group">
            <div class="w-10 h-10 bg-primary text-black rounded-xl flex items-center justify-center font-bold text-xl shadow-[0_0_20px_rgba(0,200,83,0.3)] group-hover:scale-110 transition-transform">
              <PhTicket weight="fill" />
            </div>
            <span class="font-heading font-black text-2xl tracking-tight text-white uppercase">TicketHub</span>
          </router-link>
          
          <p class="text-muted text-[15px] font-medium leading-relaxed max-w-sm">
            Nền tảng phân phối vé sự kiện cao cấp. Khám phá những trải nghiệm giải trí đẳng cấp và kết nối cộng đồng.
          </p>

          <div class="flex items-center gap-3 pt-2">
            <a v-for="s in SOCIALS" :key="s.name" href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center text-lg hover:bg-white/10 hover:text-white transition-all text-white/50">
              <component :is="s.icon" weight="fill" />
            </a>
          </div>
        </div>

        <!-- Links -->
        <div class="lg:w-2/3 flex flex-wrap lg:justify-end gap-12 md:gap-24">
          <div v-for="col in FOOTER_COLS" :key="col.title" class="space-y-6 min-w-[140px]">
            <h4 class="text-[12px] font-bold text-white uppercase tracking-widest flex items-center gap-2">
              <component :is="col.icon" weight="bold" class="text-primary text-lg" />
              {{ col.title }}
            </h4>
            <ul class="space-y-4">
              <li v-for="link in col.links" :key="link.label">
                <a href="#" class="text-[14px] font-medium text-white/60 hover:text-white transition-colors">
                  {{ link.label }}
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Bottom Bar -->
      <div class="pt-8 border-t border-white/5 flex flex-col md:flex-row justify-between items-center gap-6">
        <div class="flex flex-col md:flex-row items-center gap-6">
          <span class="text-[13px] text-white/40 font-medium">© 2026 TicketHub. All rights reserved.</span>
          <div class="flex items-center gap-6">
            <a href="#" class="text-[13px] text-white/40 hover:text-white transition-colors">Privacy Policy</a>
            <a href="#" class="text-[13px] text-white/40 hover:text-white transition-colors">Terms of Service</a>
          </div>
        </div>
        
        <div class="flex items-center gap-2 text-white/40 font-medium text-[13px]">
          Made with <PhHeart weight="fill" class="text-primary animate-pulse" /> in Vietnam
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { 
  PhTicket, PhFacebookLogo, PhInstagramLogo, PhYoutubeLogo, PhTiktokLogo, 
  PhHeart, PhCalendarStar, PhHeadset, PhBuildings
} from '@phosphor-icons/vue'

const SOCIALS = [
  { name: 'FB', icon: PhFacebookLogo },
  { name: 'IG', icon: PhInstagramLogo },
  { name: 'YT', icon: PhYoutubeLogo },
  { name: 'TT', icon: PhTiktokLogo },
]

const FOOTER_COLS = [
  {
    title: 'Sự kiện',
    icon: PhCalendarStar,
    links: [
      { label: 'Concert & Âm nhạc' },
      { label: 'Nghệ thuật & Sân khấu' },
      { label: 'Thể thao' },
    ]
  },
  {
    title: 'Hỗ trợ',
    icon: PhHeadset,
    links: [
      { label: 'Trung tâm trợ giúp' },
      { label: 'Chính sách hoàn tiền' },
      { label: 'Liên hệ' },
    ]
  },
  {
    title: 'Công ty',
    icon: PhBuildings,
    links: [
      { label: 'Về TicketHub' },
      { label: 'Tuyển dụng' },
      { label: 'Đối tác' },
    ]
  }
]
</script>
