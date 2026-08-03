import {
  PhBell,
  PhCalendarX,
  PhCheckCircle,
  PhClockCountdown,
  PhHandCoins,
  PhMegaphone,
  PhProhibit,
  PhReceipt,
  PhTicket,
  PhWallet,
  PhXCircle
} from '@phosphor-icons/vue'

const TYPE_PRESETS = {
  Announcement: { icon: PhMegaphone, tone: 'text-primary bg-primary/10 border-primary/20' },
  OrderPaid: { icon: PhReceipt, tone: 'text-primary bg-primary/10 border-primary/20' },
  OrderCancelled: { icon: PhXCircle, tone: 'text-danger bg-danger/10 border-danger/20' },
  OrderRefunded: { icon: PhWallet, tone: 'text-amber-400 bg-amber-400/10 border-amber-400/20' },
  TicketsIssued: { icon: PhTicket, tone: 'text-primary bg-primary/10 border-primary/20' },
  EventPendingReview: { icon: PhClockCountdown, tone: 'text-sky-400 bg-sky-400/10 border-sky-400/20' },
  EventApproved: { icon: PhCheckCircle, tone: 'text-primary bg-primary/10 border-primary/20' },
  EventRejected: { icon: PhXCircle, tone: 'text-danger bg-danger/10 border-danger/20' },
  EventCancelled: { icon: PhCalendarX, tone: 'text-danger bg-danger/10 border-danger/20' },
  EventCancellationRequested: { icon: PhProhibit, tone: 'text-amber-400 bg-amber-400/10 border-amber-400/20' },
  EventCancellationApproved: { icon: PhCheckCircle, tone: 'text-primary bg-primary/10 border-primary/20' },
  EventCancellationRejected: { icon: PhXCircle, tone: 'text-danger bg-danger/10 border-danger/20' },
  PayoutRequested: { icon: PhHandCoins, tone: 'text-sky-400 bg-sky-400/10 border-sky-400/20' },
  PayoutProposed: { icon: PhHandCoins, tone: 'text-primary bg-primary/10 border-primary/20' }
}

const DEFAULT_PRESET = { icon: PhBell, tone: 'text-white/70 bg-white/5 border-white/10' }

export function getNotificationPreset(type) {
  return TYPE_PRESETS[type] || DEFAULT_PRESET
}

export function formatRelativeTime(value) {
  if (!value) return ''

  const target = new Date(value)
  if (Number.isNaN(target.getTime())) return ''

  const seconds = Math.floor((Date.now() - target.getTime()) / 1000)

  if (seconds < 60) return 'Vừa xong'
  if (seconds < 3600) return `${Math.floor(seconds / 60)} phút trước`
  if (seconds < 86400) return `${Math.floor(seconds / 3600)} giờ trước`
  if (seconds < 604800) return `${Math.floor(seconds / 86400)} ngày trước`

  return target.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
