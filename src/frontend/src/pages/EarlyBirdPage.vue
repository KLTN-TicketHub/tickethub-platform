<template>
  <div class="min-h-screen bg-[#0A0F0D]">
    <!-- Premium Editorial Hero Section -->
    <section class="relative pt-32 pb-20 overflow-hidden min-h-[90vh] flex items-center">
      <div class="absolute top-0 left-0 w-full h-full">
        <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-primary/10 via-[#0A0F0D]/80 to-[#0A0F0D] opacity-80"></div>
        <div class="absolute -top-24 -right-24 w-[500px] h-[500px] bg-primary/20 blur-[150px] rounded-full mix-blend-screen pointer-events-none"></div>
      </div>

      <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-10 w-full">
        <div class="flex flex-col lg:flex-row items-center gap-16 xl:gap-24">
          <!-- Left: Editorial Content -->
          <div class="flex-1 space-y-8 animate-fade-up">
            <div class="inline-flex items-center gap-3 px-5 py-2 rounded-full bg-primary/10 border border-primary/30 text-primary shadow-[0_0_20px_rgba(0,200,83,0.15)]">
              <PhCrown weight="fill" />
              <span class="text-[11px] font-black uppercase tracking-[0.3em]">Đặc quyền thành viên</span>
            </div>
            
            <h1 class="text-6xl sm:text-7xl lg:text-8xl xl:text-9xl font-black font-heading text-white leading-[0.95] tracking-tighter uppercase">
              Săn Vé Sớm <br/>
              <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary to-[#00A355]">Giảm 40%</span>
            </h1>

            <p class="text-lg lg:text-xl text-white/50 font-medium max-w-xl leading-relaxed">
              Trở thành "Người săn vé" chuyên nghiệp tại TicketHub. Nhận thông báo sớm nhất, ưu đãi giá vé kịch sàn và các đặc quyền VIP không dành cho số đông.
            </p>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-8 pt-6 border-t border-white/10">
              <div v-for="(b, idx) in benefits" :key="b.title" class="flex flex-col gap-3 group">
                <div class="w-12 h-12 rounded-[1.25rem] bg-[#111916] border border-white/5 flex items-center justify-center text-2xl text-primary shadow-inner group-hover:scale-110 group-hover:bg-primary/10 transition-all">
                  <component :is="b.icon" weight="duotone" />
                </div>
                <div>
                  <h4 class="font-black text-[15px] text-white mb-1 tracking-tight">{{ b.title }}</h4>
                  <p class="text-[13px] text-white/50 font-medium leading-relaxed">{{ b.desc }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Right: Registration Bento Card -->
          <div class="w-full lg:w-[480px] xl:w-[540px] animate-fade-up [animation-delay:200ms]">
            <div class="bg-[#111916]/80 backdrop-blur-2xl border border-white/10 rounded-[3rem] p-10 lg:p-12 shadow-[0_30px_100px_-20px_rgba(0,0,0,1)] relative overflow-hidden group">
              <div class="absolute inset-0 bg-gradient-to-br from-primary/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-1000"></div>
              
              <div class="relative z-10 space-y-10">
                <div>
                  <h3 class="text-3xl font-black font-heading text-white mb-2 tracking-tight">Đăng ký ngay</h3>
                  <p class="text-white/50 text-[14px] font-medium">Hoàn toàn miễn phí. Ưu đãi trọn đời.</p>
                </div>

                <form @submit.prevent="handleRegister" class="space-y-6">
                  <div class="space-y-2">
                    <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Họ và tên</label>
                    <div class="relative">
                      <PhUser class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 text-lg" weight="bold" />
                      <input 
                        type="text" 
                        placeholder="Nguyễn Văn A" 
                        class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 focus:bg-white/5 transition-all placeholder:text-white/20 placeholder:font-medium shadow-inner"
                      />
                    </div>
                  </div>
                  <div class="space-y-2">
                    <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Địa chỉ Email</label>
                    <div class="relative">
                      <PhEnvelopeSimple class="absolute left-5 top-1/2 -translate-y-1/2 text-white/30 text-lg" weight="bold" />
                      <input 
                        type="email" 
                        placeholder="example@gmail.com" 
                        class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl py-4 pl-14 pr-6 text-[15px] font-bold text-white outline-none focus:border-primary/50 focus:bg-white/5 transition-all placeholder:text-white/20 placeholder:font-medium shadow-inner"
                      />
                    </div>
                  </div>
                  <div class="space-y-3">
                    <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Thể loại yêu thích</label>
                    <div class="flex flex-wrap gap-2.5">
                      <button 
                        v-for="c in categories" 
                        :key="c" 
                        type="button"
                        @click="toggleCat(c)"
                        class="px-5 py-2.5 rounded-full text-[13px] font-bold border transition-all cursor-pointer"
                        :class="[selectedCats.includes(c) ? 'bg-primary text-black border-primary shadow-[0_0_20px_rgba(0,200,83,0.2)] hover:scale-105' : 'bg-white/5 border-white/5 text-white/50 hover:text-white hover:border-white/20']"
                      >
                        {{ c }}
                      </button>
                    </div>
                  </div>

                  <BaseButton variant="primary" size="lg" class="w-full !rounded-2xl !py-5 shadow-[0_0_40px_rgba(0,200,83,0.2)] hover:shadow-[0_0_60px_rgba(0,200,83,0.4)] text-[16px] mt-4 flex items-center justify-center gap-2">
                    <PhLightning weight="fill" /> Nhận ưu đãi ngay
                  </BaseButton>

                  <p class="text-center text-[11px] text-white/30 font-medium leading-relaxed max-w-xs mx-auto pt-2">
                    Bằng cách đăng ký, bạn đồng ý với Điều khoản và Chính sách bảo mật của TicketHub.
                  </p>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Proof / Counter Section -->
    <section class="py-24 border-t border-white/5 relative bg-[#0A0F0D]">
      <div class="max-w-[1400px] mx-auto px-6 md:px-10">
        <div class="grid grid-cols-2 md:grid-cols-4 gap-12 text-center divide-x divide-white/5">
          <div v-for="(s, idx) in stats" :key="s.label" class="space-y-3 animate-fade-up" :style="`animation-delay: ${idx * 100}ms`">
            <div class="text-5xl md:text-6xl font-black font-heading text-primary tracking-tighter">{{ s.value }}</div>
            <div class="text-[11px] font-bold text-white/40 uppercase tracking-widest">{{ s.label }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Success Modal -->
    <Transition name="fade">
      <div v-if="submitted" class="fixed inset-0 z-[2000] flex items-center justify-center p-6">
        <div class="absolute inset-0 bg-[#0A0F0D]/90 backdrop-blur-2xl" @click="submitted = false"></div>
        <div class="relative bg-[#111916] border border-primary/30 rounded-[3rem] p-12 max-w-lg w-full text-center space-y-8 animate-scale-in shadow-[0_0_100px_rgba(0,200,83,0.2)] overflow-hidden">
          <div class="absolute -top-32 -left-32 w-64 h-64 bg-primary/20 blur-[100px] rounded-full pointer-events-none"></div>
          
          <div class="w-24 h-24 mx-auto rounded-full bg-primary/10 border border-primary/20 flex items-center justify-center text-5xl text-primary shadow-inner">
            <PhRocketLaunch weight="duotone" />
          </div>
          <div class="space-y-3">
            <h2 class="text-4xl font-black font-heading text-white tracking-tight">Đăng ký thành công!</h2>
            <p class="text-white/50 font-medium leading-relaxed text-[15px]">
              Chào mừng bạn đến với cộng đồng "Early Bird". Hãy kiểm tra email để nhận mã giảm giá đặc biệt cho lần đặt vé đầu tiên.
            </p>
          </div>
          <BaseButton variant="primary" size="lg" class="!px-12 !rounded-2xl w-full" @click="submitted = false">Khám phá sự kiện</BaseButton>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, markRaw } from 'vue'
import BaseButton from '../components/ui/BaseButton.vue'
import { 
  PhCrown, PhBellRinging, PhMoney, PhStar, PhGift, 
  PhUser, PhEnvelopeSimple, PhLightning, PhRocketLaunch
} from '@phosphor-icons/vue'

const benefits = [
  { icon: markRaw(PhBellRinging), title: 'Thông báo sớm', desc: 'Nhận tin nhắn đặt vé trước 24h so với công chúng.' },
  { icon: markRaw(PhMoney), title: 'Giá Early Bird', desc: 'Tiết kiệm tới 40% cho các sự kiện chọn lọc.' },
  { icon: markRaw(PhStar), title: 'Vị trí đẹp', desc: 'Ưu tiên chọn chỗ ngồi tốt nhất trong các concert.' },
  { icon: markRaw(PhGift), title: 'Quà độc quyền', desc: 'Nhận Merch và quà tặng giới hạn từ nghệ sĩ.' },
]

const stats = [
  { value: '500k+', label: 'Thành viên' },
  { value: '40%', label: 'Giảm tối đa' },
  { value: '1.2M+', label: 'Vé đã bán' },
  { value: '24/7', label: 'Hỗ trợ VIP' },
]

const categories = ['Concerts', 'Sân khấu', 'Thể thao', 'Hội thảo', 'Triển lãm']
const selectedCats = ref(['Concerts'])
const submitted = ref(false)

const toggleCat = (c) => {
  if (selectedCats.value.includes(c)) {
    selectedCats.value = selectedCats.value.filter(i => i !== c)
  } else {
    selectedCats.value.push(c)
  }
}

const handleRegister = () => {
  submitted.value = true
}
</script>

<style scoped>
.animate-scale-in {
  animation: scaleIn 0.5s cubic-bezier(0.23, 1, 0.32, 1) forwards;
}

@keyframes scaleIn {
  from { opacity: 0; transform: scale(0.9); }
  to { opacity: 1; transform: scale(1); }
}

.fade-enter-active, .fade-leave-active { transition: opacity 0.5s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
