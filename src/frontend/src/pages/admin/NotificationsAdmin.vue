<template>
  <div class="flex flex-col gap-10 animate-fade-up pb-12">
    <!-- Header -->
    <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
      <div class="flex flex-col gap-2">
        <h1 class="font-heading text-4xl md:text-5xl font-black text-white tracking-tight">Thông báo hệ thống</h1>
        <p class="text-white/50 font-medium text-lg">Gửi thông báo in-app, hẹn giờ gửi và theo dõi mức độ tiếp cận.</p>
      </div>
    </div>

    <NotificationStatsPanel ref="statsPanelRef" />

    <div class="grid grid-cols-1 xl:grid-cols-[1.15fr_1fr] gap-8 items-start">
      <!-- Compose form -->
      <form
        @submit.prevent="handleSubmit"
        class="bg-[#111916]/50 border border-white/5 rounded-[2rem] overflow-hidden flex flex-col"
      >
        <div class="p-6 border-b border-white/5 bg-white/[0.02]">
          <h3 class="text-xl font-bold font-heading text-white">Soạn thông báo</h3>
        </div>

        <div class="p-6 flex flex-col gap-6">
          <!-- Target -->
          <div class="flex flex-col gap-3">
            <span class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Đối tượng nhận</span>
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
              <button
                v-for="option in targetOptions"
                :key="option.value"
                type="button"
                @click="selectTarget(option.value)"
                class="flex flex-col items-start gap-1.5 px-4 py-3.5 rounded-2xl border text-left transition-all"
                :class="target === option.value
                  ? 'bg-primary/10 border-primary/40 text-white'
                  : 'bg-white/5 border-white/10 text-white/60 hover:border-white/20 hover:bg-white/10'"
              >
                <component
                  :is="option.icon"
                  weight="fill"
                  class="text-xl"
                  :class="target === option.value ? 'text-primary' : 'text-white/40'"
                />
                <span class="text-[13px] font-bold">{{ option.label }}</span>
              </button>
            </div>
          </div>

          <BaseSelect
            v-if="target === 'role'"
            v-model="form.targetRole"
            label="Vai trò nhận thông báo"
            placeholder="Chọn vai trò"
            :options="roleOptions"
            :error="errors.targetRole"
          />

          <div v-if="target === 'user'" class="flex flex-col gap-2 w-full">
            <label class="text-[12px] font-bold text-white/50 uppercase tracking-widest">
              Người nhận
            </label>

            <div
              v-if="isPrefilledRecipient"
              class="flex items-center justify-between gap-4 px-5 py-3 rounded-2xl bg-primary/5 border border-primary/20"
            >
              <div class="flex items-center gap-3 min-w-0">
                <div class="w-9 h-9 rounded-full bg-primary/10 border border-primary/20 flex items-center justify-center text-[13px] font-bold text-primary uppercase flex-shrink-0">
                  {{ (prefilledUser.name || '?').charAt(0) }}
                </div>
                <div class="flex flex-col min-w-0">
                  <span class="text-[13px] font-bold text-white truncate">{{ prefilledUser.name }}</span>
                  <span class="text-[12px] text-white/40 truncate">{{ prefilledUser.email }}</span>
                </div>
              </div>
              <button
                type="button"
                @click="clearPrefilledRecipient"
                class="text-[12px] font-bold text-primary hover:text-white transition-colors flex-shrink-0"
              >
                Đổi
              </button>
            </div>

            <template v-else>
              <input
                id="recipient-id"
                v-model.trim="form.recipientUserId"
                type="text"
                placeholder="Ví dụ: 3fa85f64-5717-4562-b3fc-2c963f66afa6"
                class="w-full rounded-2xl px-5 py-3 text-[14px] font-medium bg-white/5 border border-white/10 text-white outline-none transition-all hover:border-white/20 focus:border-primary/50 focus:bg-white/10 placeholder:text-white/25 font-mono"
                :class="{ '!border-danger/50 !bg-danger/5': errors.recipientUserId }"
              />
              <span v-if="errors.recipientUserId" class="text-[12px] font-medium text-danger">{{ errors.recipientUserId }}</span>
              <span v-else class="text-[12px] text-white/30">Bấm biểu tượng chuông cạnh một người dùng ở trang Người dùng, hoặc dán ID thủ công.</span>
            </template>
          </div>

          <!-- Title -->
          <div class="flex flex-col gap-2 w-full">
            <div class="flex items-center justify-between">
              <label for="notif-title" class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Tiêu đề</label>
              <span class="text-[11px] font-bold" :class="form.title.length > 200 ? 'text-danger' : 'text-white/30'">
                {{ form.title.length }}/200
              </span>
            </div>
            <input
              id="notif-title"
              v-model="form.title"
              type="text"
              maxlength="220"
              placeholder="Bảo trì hệ thống ngày 10/08"
              class="w-full rounded-2xl px-5 py-3 text-[14px] font-bold bg-white/5 border border-white/10 text-white outline-none transition-all hover:border-white/20 focus:border-primary/50 focus:bg-white/10 placeholder:text-white/25"
              :class="{ '!border-danger/50 !bg-danger/5': errors.title }"
            />
            <span v-if="errors.title" class="text-[12px] font-medium text-danger">{{ errors.title }}</span>
          </div>

          <!-- Message -->
          <div class="flex flex-col gap-2 w-full">
            <div class="flex items-center justify-between">
              <label for="notif-message" class="text-[12px] font-bold text-white/50 uppercase tracking-widest">Nội dung</label>
              <span class="text-[11px] font-bold" :class="form.message.length > 1000 ? 'text-danger' : 'text-white/30'">
                {{ form.message.length }}/1000
              </span>
            </div>
            <textarea
              id="notif-message"
              v-model="form.message"
              rows="4"
              maxlength="1100"
              placeholder="Hệ thống sẽ tạm ngưng từ 02:00 đến 04:00 để nâng cấp..."
              class="w-full rounded-2xl px-5 py-3.5 text-[14px] font-medium leading-relaxed bg-white/5 border border-white/10 text-white outline-none transition-all resize-none hover:border-white/20 focus:border-primary/50 focus:bg-white/10 placeholder:text-white/25"
              :class="{ '!border-danger/50 !bg-danger/5': errors.message }"
            ></textarea>
            <span v-if="errors.message" class="text-[12px] font-medium text-danger">{{ errors.message }}</span>
          </div>

          <!-- Link -->
          <div class="flex flex-col gap-2 w-full">
            <label for="notif-link" class="text-[12px] font-bold text-white/50 uppercase tracking-widest">
              Đường dẫn khi bấm vào <span class="normal-case tracking-normal text-white/30">(không bắt buộc)</span>
            </label>
            <input
              id="notif-link"
              v-model.trim="form.linkUrl"
              type="text"
              placeholder="/my-tickets"
              class="w-full rounded-2xl px-5 py-3 text-[14px] font-medium bg-white/5 border border-white/10 text-white outline-none transition-all hover:border-white/20 focus:border-primary/50 focus:bg-white/10 placeholder:text-white/25 font-mono"
              :class="{ '!border-danger/50 !bg-danger/5': errors.linkUrl }"
            />
            <span v-if="errors.linkUrl" class="text-[12px] font-medium text-danger">{{ errors.linkUrl }}</span>
          </div>

          <!-- Schedule -->
          <div class="flex flex-col gap-3 pt-2 border-t border-white/5">
            <label class="flex items-center gap-3 cursor-pointer mt-4">
              <span
                class="w-10 h-6 rounded-full p-1 transition-colors flex-shrink-0"
                :class="isScheduled ? 'bg-primary' : 'bg-white/10'"
                @click.prevent="toggleSchedule"
              >
                <span
                  class="block w-4 h-4 rounded-full bg-white transition-transform"
                  :class="isScheduled ? 'translate-x-4' : 'translate-x-0'"
                ></span>
              </span>
              <span class="flex flex-col">
                <span class="text-[13px] font-bold text-white">Hẹn giờ gửi</span>
                <span class="text-[12px] text-white/40">Bật để gửi vào một thời điểm trong tương lai.</span>
              </span>
            </label>

            <div v-if="isScheduled" class="flex flex-col gap-2 w-full">
              <label for="notif-schedule" class="text-[12px] font-bold text-white/50 uppercase tracking-widest">
                Thời điểm gửi
              </label>
              <input
                id="notif-schedule"
                v-model="form.scheduledAt"
                type="datetime-local"
                :min="minScheduleValue"
                class="w-full rounded-2xl px-5 py-3 text-[14px] font-bold bg-white/5 border border-white/10 text-white outline-none transition-all hover:border-white/20 focus:border-primary/50 focus:bg-white/10 [color-scheme:dark]"
                :class="{ '!border-danger/50 !bg-danger/5': errors.scheduledAt }"
              />
              <span v-if="errors.scheduledAt" class="text-[12px] font-medium text-danger">{{ errors.scheduledAt }}</span>
              <span v-else class="text-[12px] text-white/30">Hệ thống quét mỗi 30 giây nên có thể gửi trễ tối đa 30 giây.</span>
            </div>
          </div>
        </div>

        <div class="p-6 border-t border-white/5 bg-white/[0.02] flex items-center justify-between gap-4">
          <span class="text-[13px] text-white/40 font-medium">{{ targetSummary }}</span>
          <BaseButton type="submit" variant="primary" size="lg" :loading="isSending">
            <component :is="isScheduled ? PhClock : PhPaperPlaneTilt" weight="fill" class="text-lg" />
            {{ isScheduled ? 'Hẹn giờ gửi' : 'Gửi thông báo' }}
          </BaseButton>
        </div>
      </form>

      <div class="flex flex-col gap-8">
        <!-- Preview -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] overflow-hidden">
          <div class="p-6 border-b border-white/5 bg-white/[0.02]">
            <h3 class="text-xl font-bold font-heading text-white">Xem trước</h3>
          </div>
          <div class="p-6">
            <div class="flex gap-4 p-5 bg-primary/[0.04] border border-primary/20 rounded-2xl">
              <div class="w-11 h-11 flex-shrink-0 rounded-xl border flex items-center justify-center" :class="preset.tone">
                <component :is="preset.icon" weight="fill" class="text-xl" />
              </div>
              <div class="flex-1 min-w-0">
                <div class="flex items-start gap-3">
                  <h4 class="flex-1 text-[15px] font-bold text-white leading-snug">
                    {{ form.title || 'Tiêu đề thông báo' }}
                  </h4>
                  <span class="mt-2 w-2 h-2 rounded-full bg-primary flex-shrink-0 shadow-[0_0_8px_rgba(0,200,83,0.6)]"></span>
                </div>
                <p class="text-[13px] text-muted leading-relaxed mt-1.5 whitespace-pre-line">
                  {{ form.message || 'Nội dung thông báo sẽ hiển thị ở đây.' }}
                </p>
                <span class="text-[11px] text-white/30 font-medium mt-2 block">Vừa xong</span>
              </div>
            </div>
          </div>
        </div>

        <!-- History -->
        <div class="bg-[#111916]/50 border border-white/5 rounded-[2rem] overflow-hidden flex flex-col">
          <div class="px-6 pt-5 border-b border-white/5 bg-white/[0.02] flex items-center gap-6">
            <button
              v-for="tab in historyTabs"
              :key="tab.id"
              type="button"
              @click="activeTab = tab.id"
              class="pb-4 text-[14px] font-bold transition-all relative flex items-center gap-2"
              :class="activeTab === tab.id ? 'text-primary' : 'text-white/50 hover:text-white'"
            >
              {{ tab.label }}
              <span
                v-if="tab.id === 'scheduled' && pendingCount > 0"
                class="px-2 py-0.5 rounded-full bg-primary/15 text-primary text-[10px] font-black"
              >
                {{ pendingCount }}
              </span>
              <div v-if="activeTab === tab.id" class="absolute bottom-0 left-0 right-0 h-1 bg-primary rounded-full"></div>
            </button>
          </div>

          <!-- Sent -->
          <div
            v-show="activeTab === 'sent'"
            ref="sentRef"
            @scroll.passive="handleSentScroll"
            class="max-h-[520px] overflow-y-auto overscroll-contain divide-y divide-white/5"
          >
            <article
              v-for="item in sentItems"
              :key="item.id"
              @click="openDetailModal(item)"
              class="p-5 hover:bg-white/[0.03] transition-colors cursor-pointer"
            >
              <div class="flex items-center gap-2 mb-2">
                <span class="px-2.5 py-1 rounded-full text-[10px] font-black uppercase tracking-widest" :class="targetBadgeClass(item)">
                  {{ describeTarget(item) }}
                </span>
                <span class="text-[11px] text-white/30 font-medium">{{ formatRelativeTime(item.createdAt) }}</span>
              </div>
              <h4 class="text-[14px] font-bold text-white leading-snug">{{ item.title }}</h4>
              <p class="text-[12.5px] text-muted leading-relaxed mt-1">{{ item.message }}</p>
            </article>

            <div v-if="isLoadingSent" class="p-5 text-center text-[12px] text-muted font-medium">Đang tải...</div>

            <div v-else-if="sentItems.length === 0" class="p-12 text-center">
              <PhMegaphone class="text-4xl text-white/15 mx-auto mb-3" weight="fill" />
              <p class="text-[13px] text-white/50 font-bold">Chưa gửi thông báo nào.</p>
            </div>
          </div>

          <!-- Scheduled -->
          <div
            v-show="activeTab === 'scheduled'"
            ref="scheduledRef"
            @scroll.passive="handleScheduledScroll"
            class="max-h-[520px] overflow-y-auto overscroll-contain divide-y divide-white/5"
          >
            <article v-for="item in scheduledItems" :key="item.id" class="p-5 hover:bg-white/[0.03] transition-colors">
              <div class="flex items-center gap-2 mb-2">
                <span class="px-2.5 py-1 rounded-full text-[10px] font-black uppercase tracking-widest" :class="statusBadgeClass(item.status)">
                  {{ describeStatus(item.status) }}
                </span>
                <span class="px-2.5 py-1 rounded-full text-[10px] font-black uppercase tracking-widest" :class="targetBadgeClass(item)">
                  {{ describeTarget(item) }}
                </span>
              </div>
              <h4 class="text-[14px] font-bold text-white leading-snug">{{ item.title }}</h4>
              <p class="text-[12.5px] text-muted leading-relaxed mt-1">{{ item.message }}</p>

              <div class="flex items-center justify-between gap-4 mt-3">
                <span class="flex items-center gap-1.5 text-[12px] font-bold text-white/50">
                  <PhClock weight="fill" class="text-white/30" /> {{ formatSchedule(item.scheduledAt) }}
                </span>
                <button
                  v-if="item.status === 'Pending'"
                  type="button"
                  @click="handleCancel(item)"
                  class="text-[12px] font-bold text-danger/80 hover:text-danger transition-colors"
                >
                  Huỷ
                </button>
              </div>
            </article>

            <div v-if="isLoadingScheduled" class="p-5 text-center text-[12px] text-muted font-medium">Đang tải...</div>

            <div v-else-if="scheduledItems.length === 0" class="p-12 text-center">
              <PhClock class="text-4xl text-white/15 mx-auto mb-3" weight="fill" />
              <p class="text-[13px] text-white/50 font-bold">Chưa có thông báo hẹn giờ nào.</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detail stats modal -->
    <BaseModal :show="isDetailModalOpen" title="Chi tiết thông báo" @close="closeDetailModal">
      <div v-if="isLoadingDetail" class="py-12 text-center text-[13px] text-muted font-medium">Đang tải...</div>

      <div v-else-if="detailStats" class="flex flex-col gap-6">
        <div class="flex items-start gap-4">
          <div class="w-11 h-11 flex-shrink-0 rounded-xl border flex items-center justify-center" :class="getNotificationPreset(detailStats.type).tone">
            <component :is="getNotificationPreset(detailStats.type).icon" weight="fill" class="text-xl" />
          </div>
          <div class="flex-1 min-w-0">
            <h4 class="text-[16px] font-bold text-white leading-snug">{{ detailStats.title }}</h4>
            <span class="px-2.5 py-1 mt-1.5 inline-block rounded-full text-[10px] font-black uppercase tracking-widest" :class="targetBadgeClass(detailStats)">
              {{ describeTarget(detailStats) }}
            </span>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="bg-white/5 border border-white/10 rounded-2xl p-4">
            <span class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Lượt đọc</span>
            <div class="text-2xl font-black text-white mt-1 tabular-nums">{{ detailStats.readCount }}</div>
          </div>
          <div class="bg-white/5 border border-white/10 rounded-2xl p-4">
            <span class="text-[11px] font-bold text-white/40 uppercase tracking-widest">Đã gửi lúc</span>
            <div class="text-[13px] font-bold text-white mt-2">{{ formatSchedule(detailStats.createdAt) }}</div>
          </div>
        </div>

        <div class="flex flex-col gap-3">
          <div class="flex items-center justify-between gap-4 py-2 border-b border-white/5">
            <span class="text-[13px] font-medium text-white/50">Lượt đọc đầu tiên</span>
            <span class="text-[13px] font-bold text-white">
              {{ detailStats.firstReadAt ? formatSchedule(detailStats.firstReadAt) : 'Chưa có ai đọc' }}
            </span>
          </div>
          <div class="flex items-center justify-between gap-4 py-2">
            <span class="text-[13px] font-medium text-white/50">Lượt đọc gần nhất</span>
            <span class="text-[13px] font-bold text-white">
              {{ detailStats.lastReadAt ? formatSchedule(detailStats.lastReadAt) : '—' }}
            </span>
          </div>
        </div>

        <p v-if="detailStats.isBroadcast" class="text-[12px] text-white/30 leading-relaxed">
          Đây là bản tin chung nên chỉ ghi nhận được số lượt đọc, không tính được tỉ lệ vì hệ thống không lưu tổng số người thuộc nhóm nhận.
        </p>
      </div>
    </BaseModal>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { PhClock, PhGlobeHemisphereEast, PhMegaphone, PhPaperPlaneTilt, PhUser, PhUsersThree } from '@phosphor-icons/vue'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseSelect from '../../components/ui/BaseSelect.vue'
import BaseModal from '../../components/ui/BaseModal.vue'
import NotificationStatsPanel from '../../components/admin/NotificationStatsPanel.vue'
import { addToast, openConfirm } from '../../stores/adminStore'
import { notificationService } from '../../services/notification.service'
import { formatRelativeTime, getNotificationPreset } from '../../utils/notificationDisplay'
import { getErrorMessage } from '../../utils/apiError'

const PAGE_SIZE = 10
const GUID_PATTERN = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/

const targetOptions = [
  { value: 'all', label: 'Tất cả người dùng', icon: PhGlobeHemisphereEast },
  { value: 'role', label: 'Theo vai trò', icon: PhUsersThree },
  { value: 'user', label: 'Người dùng cụ thể', icon: PhUser }
]

const roleOptions = [
  { value: 'Customer', label: 'Khách hàng' },
  { value: 'Organizer', label: 'Nhà tổ chức' },
  { value: 'Moderator', label: 'Kiểm duyệt viên' },
  { value: 'Staff', label: 'Nhân viên soát vé' },
  { value: 'Admin', label: 'Quản trị viên' }
]

const historyTabs = [
  { id: 'sent', label: 'Đã gửi' },
  { id: 'scheduled', label: 'Hẹn giờ' }
]

const STATUS_LABELS = {
  Pending: 'Chờ gửi',
  Sent: 'Đã gửi',
  Cancelled: 'Đã huỷ'
}

const route = useRoute()
const router = useRouter()

const prefilledUser = ref(null)

const target = ref(route.query.userId ? 'user' : 'all')
const isScheduled = ref(false)
const isSending = ref(false)
const activeTab = ref('sent')
const statsPanelRef = ref(null)

const form = reactive({
  recipientUserId: typeof route.query.userId === 'string' ? route.query.userId : '',
  targetRole: '',
  title: '',
  message: '',
  linkUrl: '',
  scheduledAt: ''
})

if (route.query.userId) {
  prefilledUser.value = {
    id: String(route.query.userId),
    name: route.query.userName ? String(route.query.userName) : 'Người dùng',
    email: route.query.userEmail ? String(route.query.userEmail) : ''
  }
}

const errors = reactive({
  recipientUserId: '',
  targetRole: '',
  title: '',
  message: '',
  linkUrl: '',
  scheduledAt: ''
})

const sentItems = ref([])
const sentPage = ref(0)
const hasMoreSent = ref(true)
const isLoadingSent = ref(false)
const sentRef = ref(null)

const scheduledItems = ref([])
const scheduledPage = ref(0)
const hasMoreScheduled = ref(true)
const isLoadingScheduled = ref(false)
const scheduledRef = ref(null)

const isDetailModalOpen = ref(false)
const isLoadingDetail = ref(false)
const detailStats = ref(null)

const preset = computed(() => getNotificationPreset('Announcement'))

const isPrefilledRecipient = computed(() =>
  !!prefilledUser.value && form.recipientUserId === prefilledUser.value.id
)

const pendingCount = computed(() => scheduledItems.value.filter(item => item.status === 'Pending').length)

const minScheduleValue = computed(() => toLocalInputValue(new Date(Date.now() + 60 * 1000)))

const targetSummary = computed(() => {
  if (target.value === 'all') return 'Gửi tới toàn bộ người dùng của hệ thống.'
  if (target.value === 'role') {
    const role = roleOptions.find(option => option.value === form.targetRole)
    return role ? `Gửi tới tất cả ${role.label.toLowerCase()}.` : 'Chọn một vai trò để tiếp tục.'
  }
  return 'Gửi riêng cho một người dùng.'
})

const selectTarget = (value) => {
  target.value = value
  form.targetRole = ''
  form.recipientUserId = ''
  prefilledUser.value = null
  errors.targetRole = ''
  errors.recipientUserId = ''
}

const clearPrefilledRecipient = () => {
  prefilledUser.value = null
  form.recipientUserId = ''
}

const toggleSchedule = () => {
  isScheduled.value = !isScheduled.value
  form.scheduledAt = ''
  errors.scheduledAt = ''
}

const describeTarget = (item) => {
  if (item.targetRole) {
    return roleOptions.find(option => option.value === item.targetRole)?.label || item.targetRole
  }
  return item.recipientUserId ? 'Cá nhân' : 'Toàn hệ thống'
}

const targetBadgeClass = (item) => {
  if (item.targetRole) return 'bg-sky-400/10 text-sky-400'
  return item.recipientUserId ? 'bg-white/10 text-white/60' : 'bg-primary/15 text-primary'
}

const describeStatus = (status) => STATUS_LABELS[status] || status

const statusBadgeClass = (status) => {
  if (status === 'Pending') return 'bg-warning/10 text-warning'
  if (status === 'Sent') return 'bg-primary/15 text-primary'
  return 'bg-white/10 text-white/40'
}

const formatSchedule = (value) => {
  return new Date(value).toLocaleString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}

function toLocalInputValue(date) {
  const offset = date.getTimezoneOffset() * 60000
  return new Date(date.getTime() - offset).toISOString().slice(0, 16)
}

const validate = () => {
  Object.keys(errors).forEach(key => { errors[key] = '' })

  if (target.value === 'role' && !form.targetRole) {
    errors.targetRole = 'Vui lòng chọn vai trò nhận thông báo.'
  }

  if (target.value === 'user') {
    if (!form.recipientUserId) {
      errors.recipientUserId = 'Vui lòng nhập ID người nhận.'
    } else if (!GUID_PATTERN.test(form.recipientUserId)) {
      errors.recipientUserId = 'ID người nhận không đúng định dạng GUID.'
    }
  }

  if (!form.title.trim()) {
    errors.title = 'Tiêu đề thông báo không được để trống.'
  } else if (form.title.length > 200) {
    errors.title = 'Tiêu đề thông báo không được vượt quá 200 ký tự.'
  }

  if (!form.message.trim()) {
    errors.message = 'Nội dung thông báo không được để trống.'
  } else if (form.message.length > 1000) {
    errors.message = 'Nội dung thông báo không được vượt quá 1000 ký tự.'
  }

  if (form.linkUrl && form.linkUrl.length > 500) {
    errors.linkUrl = 'Đường dẫn không được vượt quá 500 ký tự.'
  }

  if (isScheduled.value) {
    if (!form.scheduledAt) {
      errors.scheduledAt = 'Vui lòng chọn thời điểm gửi.'
    } else if (new Date(form.scheduledAt).getTime() <= Date.now()) {
      errors.scheduledAt = 'Thời điểm hẹn giờ phải nằm trong tương lai.'
    }
  }

  return Object.values(errors).every(value => !value)
}

const handleSubmit = async () => {
  if (!validate()) return

  isSending.value = true
  try {
    await notificationService.sendAsAdmin({
      recipientUserId: target.value === 'user' ? form.recipientUserId : null,
      targetRole: target.value === 'role' ? form.targetRole : null,
      title: form.title.trim(),
      message: form.message.trim(),
      linkUrl: form.linkUrl || null,
      scheduledAt: isScheduled.value ? new Date(form.scheduledAt).toISOString() : null
    })

    addToast(isScheduled.value ? 'Đã hẹn giờ gửi thông báo.' : 'Đã gửi thông báo thành công.', 'success')

    form.title = ''
    form.message = ''
    form.linkUrl = ''
    form.scheduledAt = ''

    if (isScheduled.value) {
      activeTab.value = 'scheduled'
      await reloadScheduled()
    } else {
      await reloadSent()
      statsPanelRef.value?.loadStats()
    }
  } catch (err) {
    addToast(err.response?.data?.message || 'Không thể gửi thông báo. Vui lòng thử lại.', 'error')
  } finally {
    isSending.value = false
  }
}

const handleCancel = (item) => {
  openConfirm('Huỷ thông báo hẹn giờ', `Bạn chắc chắn muốn huỷ thông báo "${item.title}"?`, async () => {
    try {
      await notificationService.cancelScheduled(item.id)
      addToast('Đã huỷ thông báo hẹn giờ.', 'success')
      await reloadScheduled()
    } catch (err) {
      addToast(err.response?.data?.message || 'Không thể huỷ thông báo hẹn giờ.', 'error')
    }
  })
}

const openDetailModal = async (item) => {
  isDetailModalOpen.value = true
  isLoadingDetail.value = true
  detailStats.value = null

  try {
    detailStats.value = await notificationService.getDetailStats(item.id)
  } catch (err) {
    addToast(err.response?.data?.message || 'Không thể tải chi tiết thông báo.', 'error')
    closeDetailModal()
  } finally {
    isLoadingDetail.value = false
  }
}

const closeDetailModal = () => {
  isDetailModalOpen.value = false
  detailStats.value = null
}

const loadMoreSent = async () => {
  if (isLoadingSent.value || !hasMoreSent.value) return

  isLoadingSent.value = true
  try {
    const nextPage = sentPage.value + 1
    const result = await notificationService.getSentByAdmin({ pageNumber: nextPage, pageSize: PAGE_SIZE })

    sentItems.value.push(...(result?.data ?? []))
    sentPage.value = nextPage
    hasMoreSent.value = result ? nextPage < result.totalPages : false
  } catch (err) {
    hasMoreSent.value = false
    console.error('[Notification] Không thể tải lịch sử thông báo đã gửi:', err)
    addToast(getErrorMessage(err, 'Không thể tải lịch sử thông báo đã gửi.'), 'error')
  } finally {
    isLoadingSent.value = false
  }
}

const loadMoreScheduled = async () => {
  if (isLoadingScheduled.value || !hasMoreScheduled.value) return

  isLoadingScheduled.value = true
  try {
    const nextPage = scheduledPage.value + 1
    const result = await notificationService.getScheduled({ pageNumber: nextPage, pageSize: PAGE_SIZE })

    scheduledItems.value.push(...(result?.data ?? []))
    scheduledPage.value = nextPage
    hasMoreScheduled.value = result ? nextPage < result.totalPages : false
  } catch (err) {
    hasMoreScheduled.value = false
    console.error('[Notification] Không thể tải danh sách thông báo hẹn giờ:', err)
    addToast(getErrorMessage(err, 'Không thể tải danh sách thông báo hẹn giờ.'), 'error')
  } finally {
    isLoadingScheduled.value = false
  }
}

const reloadSent = async () => {
  sentItems.value = []
  sentPage.value = 0
  hasMoreSent.value = true
  await loadMoreSent()
}

const reloadScheduled = async () => {
  scheduledItems.value = []
  scheduledPage.value = 0
  hasMoreScheduled.value = true
  await loadMoreScheduled()
}

const handleSentScroll = () => {
  const el = sentRef.value
  if (el && el.scrollTop + el.clientHeight >= el.scrollHeight - 80) loadMoreSent()
}

const handleScheduledScroll = () => {
  const el = scheduledRef.value
  if (el && el.scrollTop + el.clientHeight >= el.scrollHeight - 80) loadMoreScheduled()
}

onMounted(async () => {
  if (route.query.userId) {
    router.replace({ path: route.path })
  }

  await Promise.all([loadMoreSent(), loadMoreScheduled()])
})
</script>
