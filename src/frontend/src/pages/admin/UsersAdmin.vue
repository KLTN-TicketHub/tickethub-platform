<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col gap-2">
      <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Quản lý người dùng</h1>
      <p class="text-white/50 font-medium text-lg">Quản lý thành viên và phân quyền trên hệ thống</p>
    </div>

    <!-- Filter Bar -->
    <div class="flex flex-col xl:flex-row xl:items-center justify-between gap-6 bg-[#111916] border border-white/5 rounded-[2rem] p-4 lg:px-6">
      <div class="flex-1 flex items-center gap-3 bg-white/5 border border-white/10 rounded-full px-4 group focus-within:border-primary/50 transition-all w-full max-w-md">
        <PhMagnifyingGlass class="text-white/40 group-focus-within:text-primary text-lg transition-colors" weight="bold" />
        <input
          type="text"
          v-model="localSearch"
          @input="handleSearch"
          placeholder="Tìm theo tên, username hoặc email..."
          class="flex-1 bg-transparent border-none py-2.5 text-[14px] text-white outline-none placeholder:text-white/30"
        />
      </div>
      <div class="flex items-center gap-3 overflow-x-auto hide-scroll">
        <BaseSelect :options="roleOptions" v-model="roleFilter" class="w-[180px] flex-shrink-0" />
        <BaseSelect :options="statusOptions" v-model="statusFilter" class="w-[180px] flex-shrink-0" />
      </div>
    </div>

    <!-- Table Container -->
    <div class="flex flex-col gap-4">
      <div v-if="isLoading" class="py-20 flex flex-col items-center justify-center gap-3">
        <PhSpinner class="animate-spin text-primary text-4xl" weight="bold" />
        <span class="text-white/40 text-[12px] font-bold uppercase tracking-widest">Đang tải danh sách người dùng...</span>
      </div>

      <template v-else>
        <BaseTable :columns="columns" :data="usersList">
          <template #user="{ row }">
            <div class="flex items-center gap-4">
              <div class="w-10 h-10 rounded-full bg-primary/10 border border-primary/20 flex items-center justify-center text-[13px] font-bold text-primary shadow-inner uppercase">
                {{ (row.fullName || row.userName || '?').charAt(0) }}
              </div>
              <div class="flex flex-col">
                <span class="font-bold text-white">{{ row.fullName }}</span>
                <span class="text-[12px] text-white/40">@{{ row.userName }}</span>
              </div>
            </div>
          </template>
          <template #email="{ row }">
            <span class="text-[14px] text-white/70 font-medium">{{ row.email }}</span>
          </template>
          <template #phoneNumber="{ row }">
            <span class="text-[14px] text-white/70 font-medium">{{ row.phoneNumber || '—' }}</span>
          </template>
          <template #roles="{ row }">
            <div class="flex flex-wrap gap-1.5">
              <span
                v-for="role in row.roles"
                :key="role"
                class="text-[11px] font-bold uppercase tracking-widest px-3 py-1 rounded-full border"
                :class="role === 'Admin' ? 'bg-primary/10 text-primary border-primary/20' : 'bg-white/5 text-white/60 border-white/10'"
              >
                {{ role }}
              </span>
            </div>
          </template>
          <template #isLocked="{ row }">
            <BaseBadge :variant="row.isLocked ? 'neutral' : 'primary'">
              {{ row.isLocked ? 'Đã khóa' : 'Đang hoạt động' }}
            </BaseBadge>
          </template>
          <template #createdAt="{ row }">
            <span class="text-[13px] text-white/50 font-medium">{{ formatDate(row.createdAt) }}</span>
          </template>
          <template #actions="{ row }">
            <div class="flex justify-end gap-2">
              <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-warning/10 hover:!text-warning" @click="openEditModal(row)">
                <PhPencilSimple weight="bold" class="text-warning/70 hover:text-warning" />
              </BaseButton>
              <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-danger/10 hover:!text-danger" @click="confirmToggleLock(row)">
                <PhProhibit weight="bold" class="text-danger/70 hover:text-danger" />
              </BaseButton>
            </div>
          </template>
        </BaseTable>

        <div v-if="usersList.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
          <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
            <PhUsers weight="duotone" />
          </div>
          <h3 class="text-xl font-bold font-heading text-white mb-2">Không tìm thấy người dùng</h3>
          <p class="text-white/50 max-w-xs mb-8">Thử thay đổi từ khóa hoặc xóa các bộ lọc để tìm lại.</p>
          <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa bộ lọc</BaseButton>
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
          <div class="text-[12px] font-medium text-white/40">
            Trang <span class="text-white font-bold">{{ currentPage }}</span> / <span class="text-white font-bold">{{ totalPages }}</span>
          </div>

          <div class="flex items-center gap-2">
            <button
              @click="changePage(currentPage - 1)"
              :disabled="currentPage === 1"
              class="group flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-transparent hover:bg-white/5 transition-all disabled:opacity-30 disabled:hover:bg-transparent disabled:cursor-not-allowed cursor-pointer"
            >
              <PhCaretLeft weight="bold" class="text-[14px] text-white/50 group-hover:text-white group-hover:-translate-x-0.5 transition-all" />
              <span class="text-[12px] font-bold text-white/50 group-hover:text-white transition-colors">Trước</span>
            </button>

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

    <!-- Edit Modal -->
    <BaseModal :show="isModalOpen" title="Sửa thông tin người dùng" @close="closeModal">
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <div class="flex flex-col gap-2">
          <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Họ và tên</label>
          <input
            v-model="modalForm.fullName"
            type="text"
            placeholder="Nhập họ và tên..."
            required
            class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20"
          />
        </div>

        <div class="flex flex-col gap-2">
          <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Số điện thoại</label>
          <input
            v-model="modalForm.phoneNumber"
            type="text"
            placeholder="Nhập số điện thoại..."
            class="w-full bg-white/5 border border-white/10 rounded-2xl px-5 py-3.5 text-[14px] text-white outline-none focus:border-primary/50 transition-all placeholder:text-white/20"
          />
        </div>

        <div class="flex gap-3 pt-4">
          <BaseButton type="button" variant="outline" class="flex-1 !rounded-2xl" @click="closeModal">
            Hủy bỏ
          </BaseButton>
          <BaseButton type="submit" variant="primary" class="flex-1 !rounded-2xl" :disabled="isSubmitting">
            <PhSpinner v-if="isSubmitting" class="animate-spin text-lg" />
            <span v-else>Lưu lại</span>
          </BaseButton>
        </div>
      </form>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { openConfirm, addToast } from '../../stores/adminStore'
import {
  getAdminUsers,
  updateAdminUser,
  setAdminUserLockStatus
} from '../../services/admin-user.service'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import BaseSelect from '../../components/ui/BaseSelect.vue'
import BaseModal from '../../components/ui/BaseModal.vue'
import {
  PhMagnifyingGlass, PhProhibit, PhPencilSimple, PhUsers,
  PhSpinner, PhCaretLeft, PhCaretRight
} from '@phosphor-icons/vue'

const localSearch = ref('')
const roleFilter = ref('all')
const statusFilter = ref('all')
const isLoading = ref(true)
const usersList = ref([])

// Pagination
const currentPage = ref(1)
const totalPages = ref(1)
const pageSize = 10

const roleOptions = [
  { value: 'all', label: 'Tất cả quyền' },
  { value: 'Admin', label: 'Admin' },
  { value: 'Customer', label: 'Khách hàng' },
  { value: 'Moderator', label: 'Kiểm duyệt viên' },
  { value: 'Organizer', label: 'Nhà tổ chức' },
  { value: 'Staff', label: 'Nhân viên' },
]

const statusOptions = [
  { value: 'all', label: 'Tất cả trạng thái' },
  { value: 'active', label: 'Đang hoạt động' },
  { value: 'locked', label: 'Đã khóa' },
]

const columns = [
  { key: 'user', label: 'Thành viên' },
  { key: 'email', label: 'Email' },
  { key: 'phoneNumber', label: 'Số điện thoại' },
  { key: 'roles', label: 'Quyền' },
  { key: 'isLocked', label: 'Trạng thái' },
  { key: 'createdAt', label: 'Ngày tạo' },
  { key: 'actions', label: '', class: 'w-32' },
]

// Modal
const isModalOpen = ref(false)
const isSubmitting = ref(false)
const editingUserId = ref(null)
const modalForm = reactive({
  fullName: '',
  phoneNumber: '',
  imageUrl: null
})

const fetchUsers = async () => {
  isLoading.value = true
  try {
    const params = {
      pageNumber: currentPage.value,
      pageSize: pageSize
    }
    if (localSearch.value.trim()) {
      params.search = localSearch.value.trim()
    }
    if (roleFilter.value !== 'all') {
      params.role = roleFilter.value
    }
    if (statusFilter.value !== 'all') {
      params.isLocked = statusFilter.value === 'locked'
    }
    const res = await getAdminUsers(params)
    if (res && res.success && res.data) {
      usersList.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      currentPage.value = res.data.pageNumber || 1
    } else {
      usersList.value = []
      totalPages.value = 1
    }
  } catch (err) {
    console.error('Error fetching users:', err)
    usersList.value = []
    addToast('Không thể tải danh sách người dùng.', 'error')
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchUsers()
})

watch([roleFilter, statusFilter], () => {
  currentPage.value = 1
  fetchUsers()
})

const handleSearch = () => {
  currentPage.value = 1
  fetchUsers()
}

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
  currentPage.value = page
  fetchUsers()
}

const resetFilters = () => {
  localSearch.value = ''
  roleFilter.value = 'all'
  statusFilter.value = 'all'
  currentPage.value = 1
  fetchUsers()
}

const formatDate = (dateStr) => {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('vi-VN')
}

const openEditModal = (user) => {
  editingUserId.value = user.id
  modalForm.fullName = user.fullName || ''
  modalForm.phoneNumber = user.phoneNumber || ''
  modalForm.imageUrl = user.imageUrl || null
  isModalOpen.value = true
}

const closeModal = () => {
  isModalOpen.value = false
}

const handleSubmit = async () => {
  if (!modalForm.fullName.trim()) return

  isSubmitting.value = true
  try {
    const payload = {
      fullName: modalForm.fullName.trim(),
      phoneNumber: modalForm.phoneNumber?.trim() || null,
      imageUrl: modalForm.imageUrl
    }
    const res = await updateAdminUser(editingUserId.value, payload)
    if (res && res.success) {
      addToast('Cập nhật thông tin người dùng thành công.', 'success')
      closeModal()
      await fetchUsers()
    } else {
      addToast(res?.message || 'Cập nhật thất bại.', 'error')
    }
  } catch (err) {
    console.error('Error updating user:', err)
    const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra trong quá trình lưu dữ liệu.'
    addToast(errorMsg, 'error')
  } finally {
    isSubmitting.value = false
  }
}

const confirmToggleLock = (user) => {
  const nextLocked = !user.isLocked
  const title = nextLocked ? 'Khóa tài khoản' : 'Mở khóa tài khoản'
  const message = nextLocked
    ? `Bạn có chắc chắn muốn khóa tài khoản "${user.fullName}"? Người dùng sẽ không thể đăng nhập.`
    : `Bạn có chắc chắn muốn mở khóa tài khoản "${user.fullName}"?`

  openConfirm(title, message, async () => {
    try {
      const res = await setAdminUserLockStatus(user.id, nextLocked)
      if (res && res.success) {
        addToast(nextLocked ? `Đã khóa tài khoản "${user.fullName}".` : `Đã mở khóa tài khoản "${user.fullName}".`, nextLocked ? 'warning' : 'success')
        await fetchUsers()
      } else {
        addToast(res?.message || 'Thao tác thất bại.', 'error')
      }
    } catch (err) {
      console.error('Error toggling lock status:', err)
      const errorMsg = err.response?.data?.message || 'Có lỗi xảy ra trong quá trình xử lý.'
      addToast(errorMsg, 'error')
    }
  })
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
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
