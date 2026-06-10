<template>
  <div class="max-w-[800px] mx-auto py-12 px-4 md:px-6 min-h-[80vh]">
    <!-- Header -->
    <div class="mb-12">
      <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 border border-primary/20 text-primary text-[11px] font-bold tracking-widest uppercase mb-4 shadow-[0_0_20px_rgba(0,200,83,0.15)]">
        <PhNotePencil weight="fill" /> Tạo mới
      </div>
      <h1 class="font-heading text-4xl font-black text-white mb-2 tracking-tight">Tạo sự kiện mới</h1>
      <p class="text-white/50 font-medium">Chia sẻ đam mê và kết nối cộng đồng thông qua sự kiện của bạn</p>
    </div>

    <!-- Form Container -->
    <form @submit.prevent="handleSubmit" class="flex flex-col gap-10">
      <!-- Section 1: Thông tin cơ bản -->
      <div class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-8 lg:p-10 shadow-2xl">
        <div class="flex items-center gap-4 mb-8 pb-6 border-b border-white/5">
          <div class="w-12 h-12 rounded-[1.25rem] bg-white/5 border border-white/10 flex items-center justify-center text-2xl text-white shadow-inner">
            <PhNotePencil weight="duotone" />
          </div>
          <h2 class="font-heading text-2xl font-black text-white">Thông tin cơ bản</h2>
        </div>

        <div class="flex flex-col gap-8">
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Tên sự kiện <span class="text-danger">*</span></label>
            <div class="relative group">
              <input type="text" class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl p-4 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors placeholder:text-white/20 placeholder:font-medium" v-model="form.title" placeholder="Ví dụ: Concert Mùa Hè 2026" required />
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Thể loại <span class="text-danger">*</span></label>
              <BaseSelect 
                :options="categories" 
                v-model="form.category"
                class="w-full"
              />
            </div>
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Hình ảnh bìa (URL)</label>
              <input type="url" class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl p-4 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors placeholder:text-white/20 placeholder:font-medium" v-model="form.image" placeholder="https://..." />
            </div>
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Mô tả sự kiện</label>
            <textarea class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl p-4 text-[15px] font-medium text-white/80 outline-none focus:border-primary/50 transition-colors h-40 resize-none placeholder:text-white/20" v-model="form.description" placeholder="Mô tả chi tiết về nội dung sự kiện, quy định chung..."></textarea>
          </div>
        </div>
      </div>

      <!-- Section 2: Thời gian & Địa điểm -->
      <div class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-8 lg:p-10 shadow-2xl">
        <div class="flex items-center gap-4 mb-8 pb-6 border-b border-white/5">
          <div class="w-12 h-12 rounded-[1.25rem] bg-white/5 border border-white/10 flex items-center justify-center text-2xl text-white shadow-inner">
            <PhMapPin weight="duotone" />
          </div>
          <h2 class="font-heading text-2xl font-black text-white">Thời gian & Địa điểm</h2>
        </div>

        <div class="flex flex-col gap-8">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Ngày diễn ra <span class="text-danger">*</span></label>
              <div class="relative group">
                <PhCalendarBlank class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input type="date" class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl p-4 pl-12 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors" v-model="form.date" required />
              </div>
            </div>
            <div class="flex flex-col gap-2">
              <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Giờ bắt đầu <span class="text-danger">*</span></label>
              <div class="relative group">
                <PhClock class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
                <input type="time" class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl p-4 pl-12 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors" v-model="form.time" required />
              </div>
            </div>
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Địa điểm <span class="text-danger">*</span></label>
            <div class="relative group">
              <PhBuildings class="absolute left-4 top-1/2 -translate-y-1/2 text-white/30 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
              <input type="text" class="w-full bg-[#0A0F0D] border border-white/5 rounded-2xl p-4 pl-12 text-[15px] font-bold text-white outline-none focus:border-primary/50 transition-colors placeholder:text-white/20 placeholder:font-medium" v-model="form.location" placeholder="Tên rạp, nhà thi đấu, hoặc địa chỉ chi tiết..." required />
            </div>
          </div>
        </div>
      </div>

      <!-- Section 3: Cấu hình vé -->
      <div class="bg-[#111916] border border-white/5 rounded-[2.5rem] p-8 lg:p-10 shadow-2xl">
        <div class="flex items-center gap-4 mb-8 pb-6 border-b border-white/5">
          <div class="w-12 h-12 rounded-[1.25rem] bg-white/5 border border-white/10 flex items-center justify-center text-2xl text-white shadow-inner">
            <PhTicket weight="duotone" />
          </div>
          <h2 class="font-heading text-2xl font-black text-white">Cấu hình loại vé</h2>
        </div>

        <div class="flex flex-col gap-5">
          <div v-for="(tier, idx) in form.tiers" :key="idx" class="flex flex-col sm:flex-row items-end gap-6 p-6 rounded-[2rem] bg-[#0A0F0D] border border-white/5 relative group hover:border-white/20 transition-all">
            <button v-if="form.tiers.length > 1" @click="removeTier(idx)" type="button" class="absolute -right-3 -top-3 w-8 h-8 rounded-full bg-danger text-white flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity shadow-lg shadow-danger/20 hover:scale-110 cursor-pointer">
              <PhX weight="bold" />
            </button>
            
            <div class="flex-1 w-full flex flex-col gap-2">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Tên hạng vé</label>
              <input type="text" class="w-full bg-transparent border-b border-white/10 p-2 text-[16px] font-bold text-white outline-none focus:border-primary transition-colors placeholder:text-white/20 placeholder:font-medium" v-model="tier.name" placeholder="VD: Vé VIP" />
            </div>
            <div class="sm:w-48 w-full flex flex-col gap-2">
              <label class="text-[11px] font-bold text-white/50 uppercase tracking-widest">Giá vé (VND)</label>
              <div class="relative">
                <input type="number" class="w-full bg-transparent border-b border-white/10 p-2 pl-6 text-[16px] font-bold text-primary font-heading outline-none focus:border-primary transition-colors placeholder:text-white/20 placeholder:font-medium" v-model="tier.price" placeholder="500000" />
                <span class="absolute left-0 top-1/2 -translate-y-1/2 text-white/30 font-bold">đ</span>
              </div>
            </div>
          </div>

          <BaseButton variant="ghost" type="button" @click="addTier" class="mt-4 !rounded-2xl !py-4 border border-dashed border-white/20 hover:border-primary hover:bg-primary/5 transition-all flex items-center justify-center gap-2">
            <PhPlus weight="bold" /> Thêm hạng vé mới
          </BaseButton>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex flex-col gap-4 mt-6">
        <BaseButton type="submit" variant="primary" size="lg" class="!py-5 !rounded-2xl shadow-[0_0_40px_rgba(0,200,83,0.2)] hover:shadow-[0_0_60px_rgba(0,200,83,0.4)] text-[16px]">
          Hoàn tất & Đăng sự kiện
        </BaseButton>
        <BaseButton type="button" variant="ghost" @click="router.back()" class="!rounded-2xl text-[15px] font-bold text-white/50 hover:text-white">
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
import { 
  PhNotePencil, PhMapPin, PhCalendarBlank, PhClock, 
  PhBuildings, PhTicket, PhX, PhPlus 
} from '@phosphor-icons/vue'

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
