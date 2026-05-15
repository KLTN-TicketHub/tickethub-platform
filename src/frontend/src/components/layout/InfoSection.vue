<template>
  <section class="grid grid-cols-1 md:grid-cols-2 gap-8 my-16">
    <!-- Support Block -->
    <div class="bg-surface border border-border-main rounded-[32px] p-8 md:p-10 flex flex-col gap-8 shadow-xl">
      <div class="flex items-center justify-between">
        <h2 class="font-heading text-2xl font-bold text-main flex items-center gap-3">
          <span class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-primary">🎧</span>
          Hỗ trợ
        </h2>
        <span class="text-xs font-bold text-muted uppercase tracking-widest">24/7 Support</span>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <a 
          v-for="item in supportItems" 
          :key="item.label"
          href="#"
          class="bg-card border border-border-main p-5 rounded-2xl group hover:border-primary/50 hover:bg-primary/5 transition-all duration-300 flex flex-col gap-2"
        >
          <div class="text-xl group-hover:scale-110 transition-transform duration-300">{{ item.icon }}</div>
          <div class="font-bold text-main text-[15px] group-hover:text-primary transition-colors">{{ item.label }}</div>
          <p class="text-[12px] text-muted leading-relaxed line-clamp-2">{{ item.desc }}</p>
        </a>
      </div>
    </div>

    <!-- Company Block -->
    <div class="bg-surface border border-border-main rounded-[32px] p-8 md:p-10 flex flex-col gap-8 shadow-xl">
      <div class="flex items-center justify-between">
        <h2 class="font-heading text-2xl font-bold text-main flex items-center gap-3">
          <span class="w-10 h-10 rounded-xl bg-accent/10 flex items-center justify-center text-accent">🏢</span>
          Công ty
        </h2>
        <span class="text-xs font-bold text-muted uppercase tracking-widest">About Us</span>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <a 
          v-for="item in companyItems" 
          :key="item.label"
          href="#"
          class="bg-card border border-border-main p-5 rounded-2xl group hover:border-accent/50 hover:bg-accent/5 transition-all duration-300 flex flex-col gap-2"
        >
          <div class="text-xl group-hover:scale-110 transition-transform duration-300">{{ item.icon }}</div>
          <div class="font-bold text-main text-[15px] group-hover:text-accent transition-colors">{{ item.label }}</div>
          <p class="text-[12px] text-muted leading-relaxed line-clamp-2">{{ item.desc }}</p>
        </a>
      </div>
    </div>
  </section>
</template>

<script setup>
const supportItems = [
  { icon: '❓', label: 'Trung tâm trợ giúp', desc: 'Giải đáp thắc mắc về đặt vé, hoàn tiền và sự kiện.' },
  { icon: '📧', label: 'Liên hệ chúng tôi', desc: 'Gửi tin nhắn cho đội ngũ hỗ trợ trực tuyến.' },
  { icon: '🔄', label: 'Chính sách hoàn tiền', desc: 'Quy trình và điều kiện hoàn tiền vé dễ dàng.' },
  { icon: '📚', label: 'Câu hỏi thường gặp', desc: 'Những thắc mắc phổ biến nhất từ người dùng.' },
]

const companyItems = [
  { icon: '🌟', label: 'Về TicketHub', desc: 'Sứ mệnh kết nối cộng đồng qua những sự kiện tuyệt vời.' },
  { icon: '📰', label: 'Báo chí & Tin tức', desc: 'Cập nhật những thông tin mới nhất về thị trường.' },
  { icon: '🚀', label: 'Cơ hội nghề nghiệp', desc: 'Gia nhập đội ngũ năng động và sáng tạo của chúng tôi.' },
  { icon: '🤝', label: 'Trở thành đối tác', desc: 'Hợp tác tổ chức và phân phối vé chuyên nghiệp.' },
]
</script>
