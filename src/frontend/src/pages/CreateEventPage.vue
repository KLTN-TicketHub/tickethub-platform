<template>
  <div class="max-w-[800px] mx-auto py-10 px-4 min-h-[80vh]">
    <!-- Header -->
    <div class="mb-10 text-center">
      <h1 class="font-heading text-4xl font-bold text-main mb-2">Tạo sự kiện mới</h1>
      <p class="text-muted font-medium">Chia sẻ đam mê và kết nối cộng đồng thông qua sự kiện của bạn</p>
    </div>

    <!-- Form Container -->
    <form @submit.prevent="handleSubmit" class="flex flex-col gap-8">
      <!-- Section 1: Thông tin cơ bản -->
      <div class="bg-card border border-border-main rounded-[32px] p-8 shadow-xl">
        <div class="flex items-center gap-3 mb-8 pb-4 border-b border-border-main">
          <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-xl">📝</div>
          <h2 class="font-heading text-xl font-bold text-main">Thông tin cơ bản</h2>
        </div>

        <div class="flex flex-col gap-6">
          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Tên sự kiện *</label>
            <input type="text" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="form.title" placeholder="Ví dụ: Concert Mùa Hè 2026" required />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="flex flex-col gap-1.5">
              <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Thể loại *</label>
              <BaseSelect 
                :options="categories" 
                v-model="form.category"
                class="w-full"
              />
            </div>
            <div class="flex flex-col gap-1.5">
              <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Hình ảnh bìa (URL)</label>
              <input type="text" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="form.image" placeholder="https://..." />
            </div>
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Mô tả sự kiện</label>
            <textarea class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors h-32 resize-none" v-model="form.description" placeholder="Mô tả chi tiết về sự kiện của bạn..."></textarea>
          </div>
        </div>
      </div>

      <!-- Section 2: Thời gian & Địa điểm -->
      <div class="bg-card border border-border-main rounded-[32px] p-8 shadow-xl">
        <div class="flex items-center gap-3 mb-8 pb-4 border-b border-border-main">
          <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-xl">📍</div>
          <h2 class="font-heading text-xl font-bold text-main">Thời gian & Địa điểm</h2>
        </div>

        <div class="flex flex-col gap-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="flex flex-col gap-1.5">
              <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Ngày diễn ra *</label>
              <input type="date" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="form.date" required />
            </div>
            <div class="flex flex-col gap-1.5">
              <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Giờ bắt đầu *</label>
              <input type="time" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="form.time" required />
            </div>
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-muted uppercase tracking-wider ml-1">Địa điểm *</label>
            <input type="text" class="w-full bg-card border border-border-main rounded-xl p-3 px-4 text-[15px] text-main outline-none focus:border-primary transition-colors" v-model="form.location" placeholder="Tên rạp, nhà thi đấu, hoặc địa chỉ chi tiết..." required />
          </div>
        </div>
      </div>

      <!-- Section 3: Cấu hình vé -->
      <div class="bg-card border border-border-main rounded-[32px] p-8 shadow-xl">
        <div class="flex items-center gap-3 mb-8 pb-4 border-b border-border-main">
          <div class="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-xl">🎟️</div>
          <h2 class="font-heading text-xl font-bold text-main">Cấu hình loại vé</h2>
        </div>

        <div class="flex flex-col gap-4">
          <div v-for="(tier, idx) in form.tiers" :key="idx" class="flex items-end gap-4 p-4 rounded-2xl bg-surface border border-border-main relative group">
            <button v-if="form.tiers.length > 1" @click="removeTier(idx)" type="button" class="absolute -right-2 -top-2 w-6 h-6 rounded-full bg-danger text-white text-[12px] flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">✕</button>
            
            <div class="flex-1 flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-muted uppercase tracking-wider">Tên loại vé</label>
              <input type="text" class="w-full bg-card border border-border-main rounded-lg p-2 px-3 text-[14px] text-main outline-none focus:border-primary transition-colors" v-model="tier.name" placeholder="VD: Vé VIP" />
            </div>
            <div class="w-40 flex flex-col gap-1.5">
              <label class="text-[11px] font-bold text-muted uppercase tracking-wider">Giá vé (VND)</label>
              <input type="number" class="w-full bg-card border border-border-main rounded-lg p-2 px-3 text-[14px] text-main outline-none focus:border-primary transition-colors" v-model="tier.price" placeholder="500000" />
            </div>
          </div>

          <BaseButton variant="ghost" type="button" @click="addTier" class="mt-2 !rounded-xl !py-3">
            + Thêm loại vé mới
          </BaseButton>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex flex-col gap-4 mt-4">
        <BaseButton type="submit" variant="primary" size="lg" class="!py-4 !rounded-2xl shadow-xl shadow-primary/20">
          Hoàn tất & Đăng sự kiện
        </BaseButton>
        <BaseButton type="button" variant="ghost" @click="router.back()" class="!rounded-2xl">
          Hủy bỏ
        </BaseButton>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import BaseButton from '../components/ui/BaseButton.vue'
import BaseSelect from '../components/ui/BaseSelect.vue'

const router = useRouter()

const categories = [
  { value: 'concert', label: 'Concert' },
  { value: 'theater', label: 'Theater' },
  { value: 'sport', label: 'Sport' },
  { value: 'workshop', label: 'Workshop' },
]

const form = ref({
  title: '',
  category: 'concert',
  image: '',
  description: '',
  date: '',
  time: '',
  location: '',
  tiers: [{ name: 'Vé tiêu chuẩn', price: 0 }]
})

const addTier = () => {
  form.value.tiers.push({ name: '', price: 0 })
}

const removeTier = (idx) => {
  form.value.tiers.splice(idx, 1)
}

const handleSubmit = () => {
  console.log('Form submitted:', form.value)
  // Mock success
  alert('Sự kiện của bạn đã được gửi để xét duyệt!')
  router.push('/organizer')
}
</script>

