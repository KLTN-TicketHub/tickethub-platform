import { reactive, ref } from 'vue'

// --- Toast System ---
export const toasts = reactive([])
let toastIdCounter = 0

export function addToast(message, type = 'success') {
  const id = ++toastIdCounter
  toasts.push({ id, message, type })
  
  // Auto remove after 3 seconds
  setTimeout(() => {
    removeToast(id)
  }, 3000)
}

export function removeToast(id) {
  const index = toasts.findIndex(t => t.id === id)
  if (index !== -1) {
    toasts.splice(index, 1)
  }
}

// --- Global Search & Layout ---
export const adminSearch = ref('')
export const isSidebarOpen = ref(false)

export function toggleSidebar() {
  isSidebarOpen.value = !isSidebarOpen.value
}

// --- Confirm Modal System ---
export const confirmModalState = reactive({
  isOpen: false,
  title: '',
  message: '',
  onConfirm: null
})

export function openConfirm(title, message, onConfirm) {
  confirmModalState.title = title
  confirmModalState.message = message
  confirmModalState.onConfirm = onConfirm
  confirmModalState.isOpen = true
}

export function closeConfirm() {
  confirmModalState.isOpen = false
  confirmModalState.onConfirm = null
}

// --- Mock Users Data ---
export const usersData = reactive([
  { id: 'u_1', name: 'Nguyen Van A', email: 'vana@gmail.com', role: 'user', status: 'active' },
  { id: 'u_2', name: 'Tran Thi B', email: 'thib@gmail.com', role: 'admin', status: 'active' },
  { id: 'u_3', name: 'Le Van C', email: 'vanc@gmail.com', role: 'user', status: 'disabled' },
  { id: 'u_4', name: 'Pham Minh D', email: 'minhd@yahoo.com', role: 'user', status: 'active' },
  { id: 'u_5', name: 'Hoang Ngoc E', email: 'ngoce@outlook.com', role: 'user', status: 'active' },
  { id: 'u_6', name: 'Vu Duc F', email: 'ducf@gmail.com', role: 'user', status: 'disabled' },
  { id: 'u_7', name: 'Doan Quoc G', email: 'quocg@gmail.com', role: 'user', status: 'active' },
])

// --- Mock Orders Data ---
export const ordersData = reactive([
  { 
    id: 'ORD-001', 
    user: { name: 'Nguyen Van A', email: 'vana@gmail.com' },
    event: { title: 'The Eras Tour Vietnam' },
    totalPrice: 2500000,
    status: 'confirmed',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 2).toISOString() 
  },
  { 
    id: 'ORD-002', 
    user: { name: 'Le Van C', email: 'vanc@gmail.com' },
    event: { title: 'Vietnam - Indonesia World Cup' },
    totalPrice: 500000,
    status: 'pending',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24).toISOString() 
  },
  { 
    id: 'ORD-003', 
    user: { name: 'Hoang Ngoc E', email: 'ngoce@outlook.com' },
    event: { title: 'Luala Concert 2024' },
    totalPrice: 1200000,
    status: 'confirmed',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 48).toISOString() 
  },
  { 
    id: 'ORD-004', 
    user: { name: 'Pham Minh D', email: 'minhd@yahoo.com' },
    event: { title: 'Tech Startup Summit 2025' },
    totalPrice: 0,
    status: 'cancelled',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 72).toISOString() 
  },
  { 
    id: 'ORD-005', 
    user: { name: 'Nguyen Van A', email: 'vana@gmail.com' },
    event: { title: 'Color Me Run HCM' },
    totalPrice: 600000,
    status: 'confirmed',
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 96).toISOString() 
  }
])
