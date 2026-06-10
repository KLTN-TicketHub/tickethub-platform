<template>
  <div class="flex flex-col gap-8 animate-fade-up pb-12 px-2 md:px-0">
    <!-- Header & Actions -->
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-4">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-3xl md:text-4xl font-black text-white tracking-tight">Địa điểm</h1>
        <p class="text-white/50 font-medium text-lg">Quản lý danh sách các địa điểm tổ chức sự kiện.</p>
      </div>
      <router-link
        to="/moderator/venues/create"
        class="inline-flex items-center justify-center gap-2 px-6 py-3 text-[14px] font-bold text-black bg-primary rounded-xl hover:scale-[1.02] active:scale-95 transition-transform"
      >
        <PhPlus weight="bold" />
        Tạo địa điểm
      </router-link>
    </div>

    <!-- Filters & Search -->
    <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] p-6 md:p-8 flex flex-col gap-6">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        
        <!-- Search Autocomplete -->
        <div class="flex flex-col gap-2 relative">
          <label class="text-[13px] font-bold text-white/80">Tìm kiếm</label>
          <div class="relative w-full group">
            <PhMagnifyingGlass class="absolute left-4 top-1/2 -translate-y-1/2 text-white/40 group-focus-within:text-primary transition-colors text-lg" weight="bold" />
            <input
              v-model="filters.Search"
              @input="onSearchInput"
              type="text"
              placeholder="Tên địa điểm, mã..."
              class="w-full bg-white/5 border border-white/10 rounded-xl pl-12 pr-4 py-3 text-white placeholder-white/30 focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none"
            />
            <!-- Autocomplete Dropdown -->
            <div v-if="showSuggestions && suggestions.length > 0" class="absolute z-50 left-0 right-0 top-full mt-2 bg-[#1a231f] border border-white/10 rounded-xl shadow-2xl overflow-hidden">
              <button
                v-for="venue in suggestions"
                :key="venue.id"
                @click="selectSuggestion(venue.venueName)"
                class="w-full text-left px-4 py-3 text-[13px] text-white/80 hover:bg-white/5 hover:text-white transition-colors border-b border-white/5 last:border-0 flex items-center justify-between"
              >
                <span class="font-bold">{{ venue.venueName }}</span>
                <span class="text-white/40 text-[11px]">{{ venue.provinceCity }}</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Province Filter -->
        <div class="flex flex-col gap-2">
          <label class="text-[13px] font-bold text-white/80">Tỉnh/Thành phố</label>
          <select
            v-model="filters.ProvinceCity"
            @change="onProvinceFilterChange"
            class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none appearance-none"
          >
            <option value="" class="text-black">Tất cả</option>
            <option v-for="prov in provinces" :key="prov.code" :value="prov.name" class="text-black">
              {{ prov.name }}
            </option>
          </select>
        </div>

        <!-- District Filter -->
        <div class="flex flex-col gap-2">
          <label class="text-[13px] font-bold text-white/80">Quận/Huyện</label>
          <select
            v-model="filters.District"
            @change="loadVenues"
            :disabled="!filters.ProvinceCity"
            class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:border-primary/50 focus:bg-white/10 transition-all focus:outline-none appearance-none disabled:opacity-50"
          >
            <option value="" class="text-black">Tất cả</option>
            <option v-for="dist in districts" :key="dist.code" :value="dist.name" class="text-black">
              {{ dist.name }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <!-- Data Table -->
    <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] overflow-hidden flex flex-col">
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="border-b border-white/5 bg-white/5">
              <th class="px-6 py-4 text-[11px] font-bold uppercase tracking-wider text-white/50">Mã</th>
              <th class="px-6 py-4 text-[11px] font-bold uppercase tracking-wider text-white/50">Tên địa điểm</th>
              <th class="px-6 py-4 text-[11px] font-bold uppercase tracking-wider text-white/50">Địa chỉ</th>
              <th class="px-6 py-4 text-[11px] font-bold uppercase tracking-wider text-white/50 text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-white/5">
            <tr v-if="isLoading" class="animate-pulse">
              <td colspan="4" class="px-6 py-8 text-center text-white/40 font-medium">Đang tải dữ liệu...</td>
            </tr>
            <tr v-else-if="venues.length === 0">
              <td colspan="4" class="px-6 py-8 text-center text-white/40 font-medium">Không tìm thấy địa điểm nào.</td>
            </tr>
            <tr v-else v-for="venue in venues" :key="venue.id" class="hover:bg-white/[0.02] transition-colors group">
              <td class="px-6 py-4 text-[13px] font-medium text-white/60">{{ venue.venueCode }}</td>
              <td class="px-6 py-4">
                <div class="flex flex-col">
                  <span class="font-bold text-white group-hover:text-primary transition-colors">{{ venue.venueName }}</span>
                  <span class="text-[12px] text-white/40 mt-1">{{ venue.district }}, {{ venue.provinceCity }}</span>
                </div>
              </td>
              <td class="px-6 py-4 text-[13px] text-white/80 max-w-[200px] truncate">{{ venue.addressLine }}, {{ venue.ward }}</td>
              <td class="px-6 py-4 text-right">
                <div class="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                  <router-link :to="`/moderator/venues/${venue.id}/edit`" class="w-8 h-8 rounded-lg bg-primary/10 text-primary flex items-center justify-center hover:bg-primary hover:text-black transition-colors" title="Chỉnh sửa">
                    <PhPencilSimple weight="bold" />
                  </router-link>
                  <button @click="confirmDelete(venue)" class="w-8 h-8 rounded-lg bg-danger/10 text-danger flex items-center justify-center hover:bg-danger hover:text-white transition-colors" title="Xóa">
                    <PhTrash weight="bold" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="!isLoading && totalPages > 0" class="px-6 py-4 border-t border-white/5 flex items-center justify-between">
        <span class="text-[13px] text-white/50 font-medium">Trang {{ filters.PageNumber }} / {{ totalPages }}</span>
        <div class="flex items-center gap-2">
          <button 
            @click="prevPage" 
            :disabled="!hasPreviousPage"
            class="px-4 py-2 rounded-lg bg-white/5 text-white/80 text-[13px] font-bold hover:bg-white/10 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
          >
            Trước
          </button>
          <button 
            @click="nextPage" 
            :disabled="!hasNextPage"
            class="px-4 py-2 rounded-lg bg-white/5 text-white/80 text-[13px] font-bold hover:bg-white/10 hover:text-white disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
          >
            Sau
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { getVenues, deleteVenue } from '../../services/venue.service'
import { getProvinces, getDistricts } from '../../services/location.service'
import { PhPlus, PhMagnifyingGlass, PhPencilSimple, PhTrash } from '@phosphor-icons/vue'

const venues = ref([])
const totalPages = ref(0)
const hasNextPage = ref(false)
const hasPreviousPage = ref(false)
const isLoading = ref(false)

const filters = reactive({
  PageNumber: 1,
  PageSize: 10,
  Search: '',
  ProvinceCity: '',
  District: ''
})

// Location Data
const provinces = ref([])
const districts = ref([])

// Autocomplete State
const suggestions = ref([])
const showSuggestions = ref(false)
let searchTimeout = null

onMounted(async () => {
  await loadProvinces()
  await loadVenues()
})

const loadProvinces = async () => {
  provinces.value = await getProvinces()
}

const onProvinceFilterChange = async () => {
  filters.District = '' // reset district
  filters.PageNumber = 1
  districts.value = []
  
  if (filters.ProvinceCity) {
    const prov = provinces.value.find(p => p.name === filters.ProvinceCity)
    if (prov) {
      districts.value = await getDistricts(prov.code)
    }
  }
  await loadVenues()
}

const loadVenues = async () => {
  isLoading.value = true
  try {
    const res = await getVenues(filters)
    if (res.success && res.data) {
      venues.value = res.data.data || []
      totalPages.value = res.data.totalPages || 1
      hasNextPage.value = res.data.hasNextPage || false
      hasPreviousPage.value = res.data.hasPreviousPage || false
    }
  } catch (error) {
    console.error("Failed to load venues", error)
  } finally {
    isLoading.value = false
  }
}

// Autocomplete Debounce
const onSearchInput = () => {
  showSuggestions.value = true
  clearTimeout(searchTimeout)
  
  searchTimeout = setTimeout(async () => {
    // Fetch suggestions
    if (!filters.Search.trim()) {
      suggestions.value = []
      showSuggestions.value = false
      // Reload main list if cleared
      filters.PageNumber = 1
      await loadVenues()
      return
    }
    
    try {
      const res = await getVenues({ Search: filters.Search, PageNumber: 1, PageSize: 5 })
      if (res.success && res.data) {
        suggestions.value = res.data.data || []
      }
    } catch (e) {
      suggestions.value = []
    }
    
    // Also reload the main table
    filters.PageNumber = 1
    await loadVenues()
  }, 500)
}

const selectSuggestion = (name) => {
  filters.Search = name
  showSuggestions.value = false
  filters.PageNumber = 1
  loadVenues()
}

// Pagination
const prevPage = () => {
  if (hasPreviousPage.value) {
    filters.PageNumber--
    loadVenues()
  }
}

const nextPage = () => {
  if (hasNextPage.value) {
    filters.PageNumber++
    loadVenues()
  }
}

// Delete
const confirmDelete = async (venue) => {
  if (window.confirm(`Bạn có chắc chắn muốn xóa địa điểm "${venue.venueName}" không?`)) {
    try {
      await deleteVenue(venue.id)
      alert('Xóa địa điểm thành công')
      loadVenues()
    } catch (error) {
      alert('Lỗi khi xóa địa điểm')
    }
  }
}

// Hide autocomplete on click outside (simplified)
document.addEventListener('click', (e) => {
  if (!e.target.closest('.relative.w-full.group')) {
    showSuggestions.value = false
  }
})
</script>
