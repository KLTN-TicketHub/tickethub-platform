<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Quản lý danh mục</h1>
        <p class="text-white/50 font-medium text-lg">Quản lý các loại phân loại sự kiện trên toàn hệ thống</p>
      </div>
      <BaseButton variant="primary" class="!rounded-2xl !py-3.5 !px-6 flex items-center gap-2 self-start sm:self-auto" @click="openCreateModal">
        <PhPlus weight="bold" />
        Thêm danh mục
      </BaseButton>
    </div>

    <!-- Filter Bar -->
    <div class="flex flex-col xl:flex-row xl:items-center justify-between gap-6 bg-[#111916] border border-white/5 rounded-[2rem] p-4 lg:px-6">
      <div class="flex-1 flex items-center gap-3 bg-white/5 border border-white/10 rounded-full px-4 group focus-within:border-primary/50 transition-all w-full max-w-md">
        <PhMagnifyingGlass class="text-white/40 group-focus-within:text-primary text-lg transition-colors" weight="bold" />
        <input 
          type="text" 
          v-model="localSearch" 
          @input="handleSearch"
          placeholder="Tìm theo tên danh mục..." 
          class="flex-1 bg-transparent border-none py-2.5 text-[14px] text-white outline-none placeholder:text-white/30"
        />
      </div>
    </div>

    <!-- Table Container -->
    <div class="flex flex-col gap-4">
      <div v-if="isLoading" class="py-20 flex flex-col items-center justify-center gap-3">
        <PhSpinner class="animate-spin text-primary text-4xl" weight="bold" />
        <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách danh mục...</span>
      </div>

      <template v-else>
        <BaseTable :columns="columns" :data="categoriesList">
          <template #id="{ row }">
            <span class="font-mono text-xs text-white/50">{{ row.id.substring(0, 8) }}...</span>
          </template>
          <template #categoryName="{ row }">
            <span class="font-bold text-white">{{ row.categoryName }}</span>
          </template>
          <template #recommendedCommissionRate="{ row }">
            <span class="font-bold text-primary">{{ row.recommendedCommissionRate }}%</span>
          </template>
          <template #description="{ row }">
            <span class="text-[14px] text-white/70 font-medium line-clamp-1" :title="row.description">{{ row.description || 'Không có mô tả' }}</span>
          </template>
          <template #actions="{ row }">
            <div class="flex justify-end gap-2">
              <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-warning/10 hover:!text-warning" @click="openEditModal(row)">
                <PhPencilSimple weight="bold" class="text-warning/70 hover:text-warning" />
              </BaseButton>
              <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-danger/10 hover:!text-danger" @click="confirmDelete(row)">
                <PhTrash weight="bold" class="text-danger/70 hover:text-danger" />
              </BaseButton>
            </div>
          </template>
        </BaseTable>

        <div v-if="categoriesList.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
            <PhFolderOpen weight="duotone" />
          </div>
          <h3 class="text-xl font-bold font-heading text-white mb-2">Không tìm thấy danh mục</h3>
          <p class="text-white/50 max-w-xs mb-8">Thử thay đổi từ khóa tìm kiếm.</p>
          <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa tìm kiếm</BaseButton>
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
          <div class="text-[12px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ currentPage }}</span> / <span class="text-white font-bold">{{ totalPages }}</span>
          </div>
          
          <div class="flex items-center gap-2">
            <!-- Prev -->
            <button 
              @click="changePage(currentPage - 1)"
              :disabled="currentPage === 1"
              class="group flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <PhCaretLeft weight="bold" class="text-[14px] text-white/50 group-hover:text-white group-hover:-translate-x-0.5 transition-all" />
              <span class="text-[12px] font-bold text-white/50 group-hover:text-white transition-colors">Trước</span>
            </button>

            <!-- Page Numbers -->
            <div class="flex items-center gap-1">
              <template v-for="(page, index) in computedVisiblePages" :key="index">
                <span v-if="page === '...'" class="w-8 text-center text-white/30 font-bold tracking-widest text-[12px]">...</span>
                <button 
                  v-else
                  @click="changePage(page)"
                  class="w-8 h-8 flex items-center justify-center rounded-lg font-bold text-[12px] transition-all cursor-pointer"
                  :class="currentPage === page ? 'text-black bg-primary shadow-lg' : 'text-white/60 hover:text-white hover:bg-white/5'"
                >
                  {{ page }}
                </button>
              </template>
            </div>

            <!-- Next -->
            <button 
              @click="changePage(currentPage + 1)"
              :disabled="currentPage === totalPages"
              class="group flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <span class="text-[12px] font-bold text-white/50 group-hover:text-white transition-colors">Sau</span>
              <PhCaretRight weight="bold" class="text-[14px] text-white/50 group-hover:text-white group-hover:translate-x-0.5 transition-all" />
            </button>
          </div>
        </div>
      </template>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="isModalOpen" class="fixed inset-0 z-[10000] flex items-center justify-center p-4">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="closeModal"></div>
      
      <!-- Modal Content -->
      <div class="relative bg-card/90 backdrop-blur-2xl border border-border-main rounded-[32px] w-full max-w-lg overflow-hidden shadow-2xl shadow-black/60 p-8 animate-in zoom-in-95 fade-in duration-300">
        <h3 class="text-2xl font-bold text-main mb-6 font-heading">{{ isEditMode ? 'Cập nhật danh mục' : 'Thêm danh mục mới' }}</h3>
        
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Tên danh mục</label>
            <input 
              v-model="modalForm.categoryName"
              type="text" 
              placeholder="Nhập tên danh mục..." 
              required
              class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20"
            />
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Tỷ lệ hoa hồng đề xuất (%)</label>
            <input 
              v-model.number="modalForm.recommendedCommissionRate"
              type="number" 
              min="0"
              max="100"
              step="0.01"
              placeholder="Nhập tỷ lệ hoa hồng..." 
              required
              class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20"
            />
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Mô tả</label>
            <textarea 
              v-model="modalForm.description"
              placeholder="Nhập mô tả chi tiết danh mục..." 
              rows="4"
              class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20 resize-none"
            ></textarea>
          </div>

          <div class="flex gap-3 pt-4">
            <BaseButton 
              type="button" 
              variant="outline" 
              class="flex-1 !rounded-2xl" 
              @click="closeModal"
            >
              Hủy bỏ
            </BaseButton>
            <BaseButton 
              type="submit" 
              variant="primary" 
              class="flex-1 !rounded-2xl"
              :disabled="isSubmitting"
            >
              <PhSpinner v-if="isSubmitting" class="animate-spin text-lg" />
              <span v-else>Lưu lại</span>
            </BaseButton>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { openConfirm, addToast } from '../../stores/adminStore'
import {
  getAdminEventCategories,
  createAdminEventCategory,
  updateAdminEventCategory,
  deleteAdminEventCategory
} from '../../services/admin-category.service'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import { 
  PhMagnifyingGlass, PhPlus, PhPencilSimple, PhTrash, 
  PhFolderOpen, PhSpinner, PhCaretLeft, PhCaretRight 
} from '@phosphor-icons/vue'

const localSearch = ref('')
const isLoading = ref(true)
const categoriesList = ref([])

// Pagination
const currentPage = ref(1)
const totalPages = ref(1)
const pageSize = 10

const columns = [
  { key: 'id', label: 'Mã danh mục' },
  { key: 'categoryName', label: 'Tên danh mục' },
  { key: 'recommendedCommissionRate', label: 'Tỷ lệ hoa hồng' },
  { key: 'description', label: 'Mô tả' },
  { key: 'actions', label: '', class: 'w-32' },
]

// Modal Properties
const isModalOpen = ref(false)
const isEditMode = ref(false)
const isSubmitting = ref(false)
const editingCategoryId = ref(null)
const modalForm = reactive({
  categoryName: '',
  recommendedCommissionRate: 0,
  description: ''
})

const fetchCategories = async () => {
  isLoading.value = true
  try {
    const params = {
      pageNumber: currentPage.value,
      pageSize: pageSize
    }
    if (localSearch.value.trim()) {
      params.search = localSearch.value.trim()
    }
    const res = await getAdminEventCategories(params)
    if (res && res.success && res.data) {
      categoriesList.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      currentPage.value = res.data.pageNumber || 1
    } else {
      categoriesList.value = []
      totalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching categories:', err)
    categoriesList.value = []
    addToast('Không thể tải danh sách danh mục sự kiện.', 'error')
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchCategories()
})

const handleSearch = () => {
  currentPage.value = 1
  fetchCategories()
}

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  fetchCategories()
}

const resetFilters = () => {
  localSearch.value = ''
  currentPage.value = 1
  fetchCategories()
}

const openCreateModal = () => {
  isEditMode.value = false
  modalForm.categoryName = ''
  modalForm.recommendedCommissionRate = 0
  modalForm.description = ''
  editingCategoryId.value = null
  isModalOpen.value = true
}

const openEditModal = (category) => {
  isEditMode.value = true
  modalForm.categoryName = category.categoryName
  modalForm.recommendedCommissionRate = category.recommendedCommissionRate || 0
  modalForm.description = category.description || ''
  editingCategoryId.value = category.id
  isModalOpen.value = true
}

const closeModal = () => {
  isModalOpen.value = false
}

const handleSubmit = async () => {
  if (!modalForm.categoryName.trim()) return
  if (modalForm.recommendedCommissionRate < 0 || modalForm.recommendedCommissionRate > 100) {
    addToast('Tỷ lệ hoa hồng phải nằm trong khoảng từ 0% đến 100%.', 'error')
    return
  }
  
  isSubmitting.value = true
  try {
    const payload = {
      categoryName: modalForm.categoryName.trim(),
      recommendedCommissionRate: Number(modalForm.recommendedCommissionRate),
      description: modalForm.description.trim()
    }

    let res
    if (isEditMode.value) {
      res = await updateAdminEventCategory(editingCategoryId.value, payload)
    } else {
      res = await createAdminEventCategory(payload)
    }

    if (res && res.success) {
      addToast(isEditMode.value ? 'Cập nhật danh mục thành công.' : 'Thêm mới danh mục thành công.', 'success')
      closeModal()
      await fetchCategories()
    } else {
      addToast(res?.message || 'Lưu danh mục thất bại.', 'error')
    }
  } catch (err) {
    console.error('Error saving category:', err)
    const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra trong quá trình lưu dữ liệu.'
    addToast(errorMsg, 'error')
  } finally {
    isSubmitting.value = false
  }
}

const confirmDelete = (category) => {
  openConfirm(
    'Xóa danh mục', 
    `Bạn có chắc chắn muốn xóa danh mục "${category.categoryName}"? Thao tác này không thể hoàn tác.`, 
    async () => {
      try {
        const res = await deleteAdminEventCategory(category.id)
        if (res && res.success) {
          addToast('Xóa danh mục thành công.', 'success')
          await fetchCategories()
        } else {
          addToast(res?.message || 'Xóa danh mục thất bại.', 'error')
        }
      } catch (err) {
        console.error('Error deleting category:', err)
        const errorMsg = err.response?.data?.message || 'Không thể xóa danh mục. Có thể có các sự kiện đang thuộc danh mục này.'
        addToast(errorMsg, 'error')
      }
    }
  )
}

const computedVisiblePages = computed(() => {
  const current = currentPage.value
  const total = totalPages.value
  if (total <= 5) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }
  if (current <= 3) {
    return [1, 2, 3, '...', total]
  }
  if (current >= total - 2) {
    return [1, '...', total - 2, total - 1, total]
  }
  return [1, '...', current, '...', total]
})
</script>

<style scoped>
</style>
