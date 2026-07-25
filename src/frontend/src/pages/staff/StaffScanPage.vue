<template>
  <div class="flex flex-col h-full">
    <!-- Camera viewport -->
    <div class="relative flex-1 min-h-0 bg-black flex items-center justify-center overflow-hidden">
      <div id="qr-reader" class="w-full h-full [&_video]:!w-full [&_video]:!h-full [&_video]:object-cover"></div>

      <!-- Scan frame overlay -->
      <div v-if="cameraState === 'running'" class="absolute inset-0 flex items-center justify-center pointer-events-none">
        <div class="w-[250px] h-[250px] relative">
          <div class="absolute -top-1 -left-1 w-10 h-10 border-t-4 border-l-4 border-primary rounded-tl-2xl"></div>
          <div class="absolute -top-1 -right-1 w-10 h-10 border-t-4 border-r-4 border-primary rounded-tr-2xl"></div>
          <div class="absolute -bottom-1 -left-1 w-10 h-10 border-b-4 border-l-4 border-primary rounded-bl-2xl"></div>
          <div class="absolute -bottom-1 -right-1 w-10 h-10 border-b-4 border-r-4 border-primary rounded-br-2xl"></div>
        </div>
      </div>

      <!-- Camera loading / error state -->
      <div v-if="cameraState !== 'running'" class="absolute inset-0 flex flex-col items-center justify-center gap-4 p-8 text-center bg-[#0A0F0D]">
        <PhCircleNotch v-if="cameraState === 'starting'" class="text-4xl text-primary animate-spin" weight="bold" />
        <template v-else-if="cameraState === 'error'">
          <PhCameraSlash class="text-5xl text-danger/70" weight="duotone" />
          <div class="flex flex-col gap-1">
            <span class="font-bold text-white text-[15px]">Không thể mở camera</span>
            <span class="text-white/50 text-[13px] max-w-xs">{{ cameraError }}</span>
          </div>
          <BaseButton variant="outline" size="md" @click="startScanner">Thử lại</BaseButton>
        </template>
      </div>
    </div>

    <!-- Manual token entry fallback -->
    <div class="flex-shrink-0 p-4 border-t border-white/5 bg-[#111916]">
      <form @submit.prevent="handleManualSubmit" class="flex gap-2">
        <input
          v-model="manualToken"
          type="text"
          placeholder="Hoặc nhập mã vé thủ công..."
          :disabled="isProcessing"
          class="flex-1 bg-[#0A0F0D] border border-white/10 rounded-xl px-4 py-3 text-[14px] text-white outline-none focus:border-primary transition-all placeholder:text-white/30"
        />
        <BaseButton type="submit" variant="outline" size="md" :disabled="isProcessing || !manualToken.trim()">
          Check-in
        </BaseButton>
      </form>
    </div>

    <!-- Result overlay -->
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div v-if="result" class="fixed inset-0 z-50 flex flex-col items-center justify-center p-6 gap-6" :class="result.success ? 'bg-primary/95' : 'bg-danger/95'">
        <div class="w-24 h-24 rounded-full bg-black/20 flex items-center justify-center">
          <PhCheckCircle v-if="result.success" class="text-6xl text-black" weight="fill" />
          <PhXCircle v-else class="text-6xl text-white" weight="fill" />
        </div>

        <div class="text-center">
          <h2 class="font-heading text-2xl font-black" :class="result.success ? 'text-black' : 'text-white'">
            {{ result.success ? 'Check-in thành công' : 'Check-in thất bại' }}
          </h2>
          <p class="mt-2 text-[15px] font-bold" :class="result.success ? 'text-black/70' : 'text-white/80'">{{ result.message }}</p>
        </div>

        <div v-if="result.ticket" class="w-full max-w-sm bg-black/20 rounded-2xl p-5 flex flex-col gap-3" :class="result.success ? 'text-black' : 'text-white'">
          <div class="flex justify-between text-[13px]">
            <span class="opacity-60 font-bold">Khách hàng:</span>
            <span class="font-bold">{{ result.ticket.customerName }}</span>
          </div>
          <div class="flex justify-between text-[13px]">
            <span class="opacity-60 font-bold">Sự kiện:</span>
            <span class="font-bold text-right">{{ result.ticket.eventTitle }}</span>
          </div>
          <div v-if="result.ticket.seatName" class="flex justify-between text-[13px]">
            <span class="opacity-60 font-bold">Ghế:</span>
            <span class="font-bold">{{ result.ticket.rowName ? `${result.ticket.rowName} - ` : '' }}{{ result.ticket.seatName }}</span>
          </div>
          <div class="flex justify-between text-[13px]">
            <span class="opacity-60 font-bold">Loại vé:</span>
            <span class="font-bold">{{ result.ticket.ticketTypeName }}</span>
          </div>
        </div>

        <BaseButton variant="outline" size="lg" class="w-full max-w-sm !border-black/20" @click="resumeScanning">
          Quét vé tiếp theo
        </BaseButton>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { Html5Qrcode } from 'html5-qrcode'
import { ticketService } from '../../services/ticket.service'
import BaseButton from '../../components/ui/BaseButton.vue'
import { PhCircleNotch, PhCameraSlash, PhCheckCircle, PhXCircle } from '@phosphor-icons/vue'

const cameraState = ref('starting') // starting | running | error
const cameraError = ref('')
const isProcessing = ref(false)
const result = ref(null)
const manualToken = ref('')

let html5QrCode = null

const startScanner = async () => {
  cameraState.value = 'starting'
  cameraError.value = ''

  try {
    html5QrCode = new Html5Qrcode('qr-reader')
    await html5QrCode.start(
      { facingMode: 'environment' },
      { fps: 10, qrbox: { width: 250, height: 250 } },
      onDecoded,
      () => {}
    )
    cameraState.value = 'running'
  } catch (err) {
    cameraState.value = 'error'
    cameraError.value = 'Vui lòng cấp quyền truy cập camera cho trình duyệt và thử lại, hoặc nhập mã vé thủ công bên dưới.'
  }
}

const onDecoded = async (decodedText) => {
  if (isProcessing.value) return
  await processToken(decodedText.trim())
}

const processToken = async (token) => {
  if (!token) return
  isProcessing.value = true

  if (html5QrCode && cameraState.value === 'running') {
    try { html5QrCode.pause(true) } catch (e) {}
  }

  try {
    const res = await ticketService.checkInTicket(token)
    result.value = {
      success: !!res.success,
      message: res.message || (res.success ? 'Check-in thành công.' : 'Check-in thất bại.'),
      ticket: res.data || null
    }
  } catch (err) {
    result.value = {
      success: false,
      message: 'Lỗi kết nối máy chủ. Vui lòng thử lại.',
      ticket: null
    }
  } finally {
    isProcessing.value = false
  }
}

const handleManualSubmit = async () => {
  const token = manualToken.value.trim()
  manualToken.value = ''
  await processToken(token)
}

const resumeScanning = () => {
  result.value = null
  if (html5QrCode && cameraState.value === 'running') {
    try { html5QrCode.resume() } catch (e) {}
  }
}

onMounted(() => {
  startScanner()
})

onBeforeUnmount(async () => {
  if (html5QrCode) {
    try {
      await html5QrCode.stop()
      html5QrCode.clear()
    } catch (e) {}
  }
})
</script>
