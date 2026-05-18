<template>
  <div class="min-h-screen bg-bg">
    <!-- Premium Hero Section -->
    <section class="relative pt-32 pb-20 overflow-hidden">
      <div class="absolute top-0 left-0 w-full h-full">
        <div class="absolute inset-0 bg-gradient-to-b from-primary/10 via-transparent to-transparent opacity-50"></div>
        <div class="absolute -top-24 -right-24 w-96 h-96 bg-primary/20 blur-[120px] rounded-full"></div>
      </div>

      <div class="max-w-[1400px] mx-auto px-6 md:px-10 relative z-10">
        <div class="flex flex-col lg:flex-row items-center gap-16">
          <!-- Left: Content -->
          <div class="flex-1 space-y-8 animate-fade-up">
            <div class="inline-flex items-center gap-3 px-5 py-2 rounded-full bg-primary/20 border border-primary/30 text-primary">
              <span class="text-xs font-black uppercase tracking-[0.3em]">Đặc quyền thành viên</span>
            </div>
            
            <h1 class="text-6xl md:text-8xl font-black font-heading text-white leading-tight">
              Săn Vé Sớm <br/>
              <span class="text-transparent bg-clip-text bg-gradient-to-r from-primary to-primary-dark">Giảm 40%</span>
            </h1>

            <p class="text-xl text-muted font-medium max-w-xl leading-relaxed">
              Trở thành "Người săn vé" chuyên nghiệp tại TicketHub. Nhận thông báo sớm nhất, ưu đãi giá vé kịch sàn và các đặc quyền VIP không dành cho số đông.
            </p>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-6 pt-4">
              <div v-for="b in benefits" :key="b.title" class="flex items-start gap-4 p-6 bg-surface/50 border border-white/5 rounded-3xl">
                <span class="text-3xl">{{ b.icon }}</span>
                <div>
                  <h4 class="font-bold text-white mb-1">{{ b.title }}</h4>
                  <p class="text-[13px] text-muted">{{ b.desc }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Right: Registration Card -->
          <div class="w-full lg:w-[480px] animate-fade-up [animation-delay:200ms]">
            <div class="bg-card border border-white/10 rounded-[3rem] p-10 shadow-2xl relative overflow-hidden group">
              <div class="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-1000"></div>
              
              <div class="relative z-10 space-y-8">
                <div class="text-center">
                  <h3 class="text-2xl font-bold text-white mb-2">Đăng ký thành viên</h3>
                  <p class="text-muted text-sm font-medium">Hoàn toàn miễn phí. Ưu đãi trọn đời.</p>
                </div>

                <form @submit.prevent="handleRegister" class="space-y-5">
                  <div class="space-y-2">
                    <label class="text-[11px] font-bold text-muted uppercase tracking-widest ml-1">Họ và tên</label>
                    <input 
                      type="text" 
                      placeholder="Nguyễn Văn A" 
                      class="w-full bg-white/5 border border-white/10 rounded-2xl py-4 px-6 text-white outline-none focus:border-primary/50 focus:bg-white/10 transition-all"
                    />
                  </div>
                  <div class="space-y-2">
                    <label class="text-[11px] font-bold text-muted uppercase tracking-widest ml-1">Địa chỉ Email</label>
                    <input 
                      type="email" 
                      placeholder="example@gmail.com" 
                      class="w-full bg-white/5 border border-white/10 rounded-2xl py-4 px-6 text-white outline-none focus:border-primary/50 focus:bg-white/10 transition-all"
                    />
                  </div>
                  <div class="space-y-2">
                    <label class="text-[11px] font-bold text-muted uppercase tracking-widest ml-1">Thể loại yêu thích</label>
                    <div class="flex flex-wrap gap-2 pt-1">
                      <button 
                        v-for="c in categories" 
                        :key="c" 
                        type="button"
                        @click="toggleCat(c)"
                        class="px-4 py-2 rounded-xl text-[12px] font-bold border transition-all"
                        :class="[selectedCats.includes(c) ? 'bg-primary text-black border-primary' : 'bg-white/5 border-white/10 text-muted hover:border-white/20']"
                      >
                        {{ c }}
                      </button>
                    </div>
                  </div>

                  <BaseButton variant="primary" size="lg" class="w-full !rounded-2xl !py-5 shadow-xl shadow-primary/20 text-lg mt-4">
                    Nhận ưu đãi ngay ⚡
                  </BaseButton>

                  <p class="text-center text-[11px] text-muted leading-relaxed">
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
    <section class="py-20 border-t border-white/5">
      <div class="max-w-[1400px] mx-auto px-6 md:px-10">
        <div class="grid grid-cols-2 md:grid-cols-4 gap-12 text-center">
          <div v-for="s in stats" :key="s.label" class="space-y-2 animate-fade-up">
            <div class="text-4xl md:text-5xl font-black font-heading text-primary">{{ s.value }}</div>
            <div class="text-[11px] font-bold text-muted uppercase tracking-widest">{{ s.label }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Success Modal (Simplified) -->
    <Transition name="fade">
      <div v-if="submitted" class="fixed inset-0 z-[2000] flex items-center justify-center p-6">
        <div class="absolute inset-0 bg-black/90 backdrop-blur-xl" @click="submitted = false"></div>
        <div class="relative bg-card border border-primary/30 rounded-[3rem] p-12 max-w-lg w-full text-center space-y-8 animate-scale-in shadow-[0_0_100px_rgba(0,200,83,0.2)]">
          <div class="text-8xl">🚀</div>
          <h2 class="text-3xl font-bold text-white">Đăng ký thành công!</h2>
          <p class="text-muted font-medium">
            Chào mừng bạn đến với cộng đồng "Early Bird". Hãy kiểm tra email để nhận mã giảm giá đặc biệt cho lần đặt vé đầu tiên.
          </p>
          <BaseButton variant="primary" size="lg" class="!px-12 !rounded-2xl" @click="submitted = false">Khám phá ngay</BaseButton>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import BaseButton from '../shared/components/BaseButton.vue'

const benefits = [
  { icon: '🔔', title: 'Thông báo sớm', desc: 'Nhận tin nhắn đặt vé trước 24h so với công chúng.' },
  { icon: '💸', title: 'Giá Early Bird', desc: 'Tiết kiệm tới 40% cho các sự kiện chọn lọc.' },
  { icon: '🌟', title: 'Vị trí đẹp', desc: 'Ưu tiên chọn chỗ ngồi tốt nhất trong các concert.' },
  { icon: '🎁', title: 'Quà độc quyền', desc: 'Nhận Merch và quà tặng giới hạn từ nghệ sĩ.' },
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
