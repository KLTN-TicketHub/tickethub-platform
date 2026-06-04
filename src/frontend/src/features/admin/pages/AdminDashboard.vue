<template>
  <div class="animate-fade-up max-w-7xl mx-auto flex flex-col gap-8 pb-12">
    
    <!-- Header -->
    <div>
      <h1 class="text-3xl font-heading font-bold text-main mb-2">Admin Dashboard</h1>
      <p class="text-sm text-muted">Manage users, monitor platform health, and oversee all operations.</p>
    </div>

    <!-- High-Level Metrics (CSS Grid) -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="glass-panel p-5 flex flex-col gap-1 relative overflow-hidden group">
        <div class="absolute -right-6 -bottom-6 w-24 h-24 rounded-full bg-info opacity-10 blur-2xl group-hover:opacity-20 transition-opacity"></div>
        <div class="flex items-center gap-2 mb-2 text-muted">
          <svg class="w-4 h-4 text-info" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>
          <span class="text-xs font-semibold uppercase tracking-wider">Total Users</span>
        </div>
        <span class="text-3xl font-heading font-bold text-main">24,592</span>
      </div>
      
      <div class="glass-panel p-5 flex flex-col gap-1 relative overflow-hidden group">
        <div class="absolute -right-6 -bottom-6 w-24 h-24 rounded-full bg-primary opacity-10 blur-2xl group-hover:opacity-20 transition-opacity"></div>
        <div class="flex items-center gap-2 mb-2 text-muted">
          <svg class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
          <span class="text-xs font-semibold uppercase tracking-wider">Active Organizers</span>
        </div>
        <span class="text-3xl font-heading font-bold text-main">342</span>
      </div>

      <div class="glass-panel p-5 flex flex-col gap-1 relative overflow-hidden group">
        <div class="absolute -right-6 -bottom-6 w-24 h-24 rounded-full bg-warning opacity-10 blur-2xl group-hover:opacity-20 transition-opacity"></div>
        <div class="flex items-center gap-2 mb-2 text-muted">
          <svg class="w-4 h-4 text-warning" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="1" x2="12" y2="23"></line><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path></svg>
          <span class="text-xs font-semibold uppercase tracking-wider">Platform Revenue</span>
        </div>
        <span class="text-3xl font-heading font-bold text-main">₫ 842M</span>
      </div>

      <div class="glass-panel p-5 flex flex-col gap-1 relative overflow-hidden group">
        <div class="absolute -right-6 -bottom-6 w-24 h-24 rounded-full bg-primary opacity-10 blur-2xl group-hover:opacity-20 transition-opacity"></div>
        <div class="flex items-center gap-2 mb-2 text-muted">
          <svg class="w-4 h-4 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"></polyline></svg>
          <span class="text-xs font-semibold uppercase tracking-wider">System Health</span>
        </div>
        <div class="flex items-center gap-2 mt-1">
          <span class="relative flex h-3 w-3">
            <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-primary opacity-75"></span>
            <span class="relative inline-flex rounded-full h-3 w-3 bg-primary"></span>
          </span>
          <span class="text-xl font-heading font-bold text-main">99.9%</span>
        </div>
      </div>
    </div>

    <!-- User Management Section -->
    <div class="flex flex-col gap-4">
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-heading font-bold text-main">User Management</h2>
        <BaseButton variant="outline" size="sm">
          <svg class="w-4 h-4 mr-1.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
          Export CSV
        </BaseButton>
      </div>

      <BaseTable 
        :columns="tableColumns" 
        :data="users" 
        :is-loading="isTableLoading"
      >
        <!-- Custom Cell: Name -->
        <template #cell(name)="{ item }">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-surface border border-border-light flex items-center justify-center text-xs font-bold text-main">
              {{ item.name.charAt(0) }}
            </div>
            <span class="font-medium text-main">{{ item.name }}</span>
          </div>
        </template>

        <!-- Custom Cell: Email -->
        <template #cell(email)="{ value }">
          <span class="text-muted">{{ value }}</span>
        </template>

        <!-- Custom Cell: Role -->
        <template #cell(role)="{ value }">
          <BaseBadge :variant="getRoleVariant(value)">
            {{ value }}
          </BaseBadge>
        </template>

        <!-- Custom Cell: Status -->
        <template #cell(status)="{ value }">
          <BaseBadge :variant="getStatusVariant(value)">
            {{ value }}
          </BaseBadge>
        </template>

        <!-- Custom Cell: Actions -->
        <template #cell(actions)="{ item }">
          <div class="flex items-center gap-2">
            <BaseButton 
              v-if="item.status === 'Active' && item.role !== 'admin'"
              variant="ghost" 
              size="sm" 
              class="!text-warning hover:!bg-warning/10"
              :is-loading="suspendingId === item.id"
              @click="suspendUser(item.id)"
              :data-testid="`btn-suspend-${item.id}`"
            >
              Suspend
            </BaseButton>
            <BaseButton 
              v-else-if="item.status === 'Suspended'"
              variant="ghost" 
              size="sm" 
              class="!text-primary hover:!bg-primary/10"
              :is-loading="suspendingId === item.id"
              @click="activateUser(item.id)"
            >
              Activate
            </BaseButton>
          </div>
        </template>
      </BaseTable>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { delay } from '@/shared/composables/useMockApi'
import { useToast } from '@/shared/composables/useToast'
import BaseButton from '@/shared/components/BaseButton.vue'
import BaseTable from '@/shared/components/BaseTable.vue'
import BaseBadge from '@/shared/components/BaseBadge.vue'

const toast = useToast()

// Table Configuration
const tableColumns = [
  { key: 'name', label: 'Name' },
  { key: 'email', label: 'Email' },
  { key: 'role', label: 'Role' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

// State
const isTableLoading = ref(true)
const suspendingId = ref(null)
const users = ref([])

// Mock Data Load
onMounted(async () => {
  await delay(800)
  users.value = [
    { id: 'usr_001', name: 'Alice Cooper', email: 'admin@tickethub.local', role: 'admin', status: 'Active' },
    { id: 'usr_002', name: 'Bob Organizer', email: 'organizer@tickethub.local', role: 'organizer', status: 'Active' },
    { id: 'usr_003', name: 'Charlie Customer', email: 'customer@tickethub.local', role: 'customer', status: 'Active' },
    { id: 'usr_004', name: 'David Spammer', email: 'david.spam@example.com', role: 'customer', status: 'Suspended' },
    { id: 'usr_005', name: 'Eva Event', email: 'eva.events@example.com', role: 'organizer', status: 'Active' },
  ]
  isTableLoading.value = false
})

// Helpers for Badge Variants
const getRoleVariant = (role) => {
  switch (role) {
    case 'admin': return 'danger'
    case 'organizer': return 'primary'
    case 'customer': return 'info'
    default: return 'default'
  }
}

const getStatusVariant = (status) => {
  switch (status) {
    case 'Active': return 'success'
    case 'Suspended': return 'warning'
    default: return 'default'
  }
}

// Action Handlers
const suspendUser = async (userId) => {
  if (suspendingId.value) return
  
  suspendingId.value = userId
  try {
    await delay(1000)
    
    // Update local state
    const userIndex = users.value.findIndex(u => u.id === userId)
    if (userIndex !== -1) {
      users.value[userIndex].status = 'Suspended'
    }
    
    toast.success('User suspended successfully.', 4000)
  } catch (error) {
    toast.error('Failed to suspend user.')
  } finally {
    suspendingId.value = null
  }
}

const activateUser = async (userId) => {
  if (suspendingId.value) return
  
  suspendingId.value = userId
  try {
    await delay(800)
    
    // Update local state
    const userIndex = users.value.findIndex(u => u.id === userId)
    if (userIndex !== -1) {
      users.value[userIndex].status = 'Active'
    }
    
    toast.success('User activated successfully.', 4000)
  } catch (error) {
    toast.error('Failed to activate user.')
  } finally {
    suspendingId.value = null
  }
}
</script>
