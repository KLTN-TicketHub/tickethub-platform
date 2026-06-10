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
          placeholder="Tìm theo tên hoặc email..." 
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
      <BaseTable :columns="columns" :data="filteredUsers">
        <template #user="{ row }">
          <div class="flex items-center gap-4">
            <div class="w-10 h-10 rounded-full bg-primary/10 border border-primary/20 flex items-center justify-center text-[13px] font-bold text-primary shadow-inner uppercase">
              {{ row.name.charAt(0) }}
            </div>
            <span class="font-bold text-white">{{ row.name }}</span>
          </div>
        </template>
        <template #email="{ row }">
          <span class="text-[14px] text-white/70 font-medium">{{ row.email }}</span>
        </template>
        <template #role="{ row }">
          <span 
            class="text-[11px] font-bold uppercase tracking-widest px-3 py-1 rounded-full border"
            :class="row.role === 'admin' ? 'bg-primary/10 text-primary border-primary/20' : 'bg-white/5 text-white/60 border-white/10'"
          >
            {{ row.role }}
          </span>
        </template>
        <template #status="{ row }">
          <BaseBadge :variant="row.status === 'active' ? 'primary' : 'neutral'">
            {{ row.status === 'active' ? 'Kích hoạt' : 'Vô hiệu' }}
          </BaseBadge>
        </template>
        <template #actions="{ row }">
          <div class="flex justify-end gap-2">
            <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-white/10" @click="viewUser(row)">
              <PhEye weight="bold" class="text-white/70" />
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-warning/10 hover:!text-warning" @click="toggleStatus(row)">
              <PhProhibit weight="bold" class="text-warning/70 hover:text-warning" />
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!px-3 hover:!bg-danger/10 hover:!text-danger" @click="confirmDelete(row)">
              <PhTrash weight="bold" class="text-danger/70 hover:text-danger" />
            </BaseButton>
          </div>
        </template>
      </BaseTable>

      <div v-if="filteredUsers.length === 0" class="py-20 flex flex-col items-center text-center bg-[#111916]/50 border border-white/5 rounded-[2rem]">
        <div class="w-20 h-20 bg-white/5 rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner text-white/20">
          <PhUsers weight="duotone" />
        </div>
        <h3 class="text-xl font-bold font-heading text-white mb-2">Không tìm thấy người dùng</h3>
        <p class="text-white/50 max-w-xs mb-8">Thử thay đổi từ khóa hoặc xóa các bộ lọc để tìm lại.</p>
        <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa bộ lọc</BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { usersData, adminSearch, openConfirm, addToast } from '../../stores/adminStore'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseTable from '../../components/ui/BaseTable.vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import BaseSelect from '../../components/ui/BaseSelect.vue'
import { PhMagnifyingGlass, PhEye, PhProhibit, PhTrash, PhUsers } from '@phosphor-icons/vue'

const localSearch = ref('')
const roleFilter = ref('all')
const statusFilter = ref('all')

const roleOptions = [
  { value: 'all', label: 'Tất cả quyền' },
  { value: 'admin', label: 'Admin' },
  { value: 'user', label: 'Người dùng' },
]

const statusOptions = [
  { value: 'all', label: 'Tất cả trạng thái' },
  { value: 'active', label: 'Kích hoạt' },
  { value: 'disabled', label: 'Vô hiệu hóa' },
]

const columns = [
  { key: 'user', label: 'Thành viên' },
  { key: 'email', label: 'Email' },
  { key: 'role', label: 'Quyền' },
  { key: 'status', label: 'Trạng thái' },
  { key: 'actions', label: '', class: 'w-32' },
]

watch(adminSearch, (val) => { localSearch.value = val })

const filteredUsers = computed(() => {
  const q = localSearch.value.toLowerCase()
  return usersData.filter(u => {
    const matchSearch = !q || u.name.toLowerCase().includes(q) || u.email.toLowerCase().includes(q)
    const matchRole = roleFilter.value === 'all' || u.role === roleFilter.value
    const matchStatus = statusFilter.value === 'all' || u.status === statusFilter.value
    return matchSearch && matchRole && matchStatus
  })
})

const viewUser = (user) => { addToast(`Đang xem hồ sơ của ${user.name}`, 'success') }

const toggleStatus = (user) => {
  const newStatus = user.status === 'active' ? 'disabled' : 'active'
  const idx = usersData.findIndex(u => u.id === user.id)
  if (idx !== -1) {
    usersData[idx].status = newStatus
    addToast(`Người dùng "${user.name}" đã bị ${newStatus === 'active' ? 'kích hoạt' : 'vô hiệu'}`, newStatus === 'active' ? 'success' : 'warning')
  }
}

const confirmDelete = (user) => {
  openConfirm('Xóa người dùng', `Bạn có chắc chắn muốn xóa "${user.name}"? Hành động này không thể hoàn tác.`, () => {
    const idx = usersData.findIndex(u => u.id === user.id)
    if (idx !== -1) {
      usersData.splice(idx, 1)
      addToast(`Người dùng "${user.name}" đã bị xóa`, 'error')
    }
  })
}

const resetFilters = () => {
  localSearch.value = ''
  roleFilter.value = 'all'
  statusFilter.value = 'all'
}
</script>
<style scoped>
.hide-scroll::-webkit-scrollbar { display: none; }
.hide-scroll { -ms-overflow-style: none; scrollbar-width: none; }
</style>
