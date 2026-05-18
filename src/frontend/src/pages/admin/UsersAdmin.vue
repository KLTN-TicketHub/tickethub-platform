<template>
  <div class="flex flex-col gap-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6">
      <div>
        <h1 class="font-heading text-3xl font-bold text-main mb-2">Quản lý người dùng</h1>
        <p class="text-muted font-medium">Quản lý thành viên và phân quyền trên hệ thống</p>
      </div>
    </div>

    <!-- Filter Bar -->
    <div class="bg-card border border-border-main p-6 rounded-[24px] flex flex-col lg:flex-row gap-4 items-center">
      <div class="flex-1 flex items-center gap-3 bg-surface border border-border-main rounded-xl px-4 w-full group focus-within:border-primary transition-all">
        <svg class="w-4 h-4 text-muted group-focus-within:text-primary transition-colors" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
        <input 
          type="text" 
          v-model="localSearch" 
          placeholder="Tìm theo tên hoặc email..." 
          class="flex-1 bg-transparent border-none py-3 text-[14px] text-main outline-none"
        />
      </div>
      <div class="flex gap-4 w-full lg:w-auto">
        <BaseSelect 
          :options="roleOptions" 
          v-model="roleFilter"
          class="min-w-[160px] flex-1 lg:flex-none"
        />
        <BaseSelect 
          :options="statusOptions" 
          v-model="statusFilter"
          class="min-w-[160px] flex-1 lg:flex-none"
        />
      </div>
    </div>

    <!-- Table Container -->
    <div class="bg-card border border-border-main rounded-[32px] overflow-hidden shadow-xl">
      <BaseTable :columns="columns" :data="filteredUsers">
        <!-- User Slot -->
        <template #user="{ row }">
          <div class="flex items-center gap-4">
            <div class="w-10 h-10 rounded-xl bg-primary/10 border border-primary/20 flex items-center justify-center text-[13px] font-bold text-primary shadow-inner uppercase">
              {{ row.name.charAt(0) }}
            </div>
            <span class="font-bold text-main">{{ row.name }}</span>
          </div>
        </template>

        <!-- Email Slot -->
        <template #email="{ row }">
          <span class="text-[14px] text-muted font-medium">{{ row.email }}</span>
        </template>

        <!-- Role Slot -->
        <template #role="{ row }">
          <span 
            class="text-[11px] font-bold uppercase tracking-widest px-2.5 py-1 rounded-lg border"
            :class="row.role === 'admin' ? 'bg-primary/5 text-primary border-primary/20' : 'bg-surface text-muted border-border-main'"
          >
            {{ row.role }}
          </span>
        </template>

        <!-- Status Slot -->
        <template #status="{ row }">
          <BaseBadge :variant="row.status === 'active' ? 'primary' : 'secondary'" size="sm">
            {{ row.status === 'active' ? 'Kích hoạt' : 'Vô hiệu' }}
          </BaseBadge>
        </template>

        <!-- Actions Slot -->
        <template #actions="{ row }">
          <div class="flex justify-end gap-2">
            <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-primary" @click="viewUser(row)">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-warning" @click="toggleStatus(row)">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"/><line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/></svg>
            </BaseButton>
            <BaseButton variant="ghost" size="sm" class="!p-2 hover:!text-danger" @click="confirmDelete(row)">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M3 6h18"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>
            </BaseButton>
          </div>
        </template>
      </BaseTable>

      <!-- Empty State -->
      <div v-if="filteredUsers.length === 0" class="py-20 flex flex-col items-center text-center">
        <div class="w-20 h-20 bg-surface rounded-full flex items-center justify-center text-4xl mb-6 shadow-inner">👤</div>
        <h3 class="text-xl font-bold text-main mb-2">Không tìm thấy người dùng</h3>
        <p class="text-muted max-w-xs mb-8">Hãy thử thay đổi từ khóa tìm kiếm hoặc bộ lọc của bạn.</p>
        <BaseButton variant="outline" size="sm" @click="resetFilters">Xóa bộ lọc</BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import { usersData, adminSearch, openConfirm, addToast } from '../../stores/adminStore'
import BaseButton from '../../shared/components/BaseButton.vue'
import BaseTable from '../../shared/components/BaseTable.vue'
import BaseBadge from '../../shared/components/BaseBadge.vue'
import BaseSelect from '../../shared/components/BaseSelect.vue'

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

// Sync with global topbar search
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

const viewUser = (user) => {
  addToast(`Đang xem hồ sơ của ${user.name}`, 'success')
}

const toggleStatus = (user) => {
  const newStatus = user.status === 'active' ? 'disabled' : 'active'
  const idx = usersData.findIndex(u => u.id === user.id)
  if (idx !== -1) {
    usersData[idx].status = newStatus
    addToast(`Người dùng "${user.name}" đã bị ${newStatus === 'active' ? 'kích hoạt' : 'vô hiệu'}`, newStatus === 'active' ? 'success' : 'warning')
  }
}

const confirmDelete = (user) => {
  openConfirm(
    'Xóa người dùng',
    `Bạn có chắc chắn muốn xóa "${user.name}"? Hành động này không thể hoàn tác.`,
    () => {
      const idx = usersData.findIndex(u => u.id === user.id)
      if (idx !== -1) {
        usersData.splice(idx, 1)
        addToast(`Người dùng "${user.name}" đã bị xóa`, 'error')
      }
    }
  )
}

const resetFilters = () => {
  localSearch.value = ''
  roleFilter.value = 'all'
  statusFilter.value = 'all'
}
</script>

