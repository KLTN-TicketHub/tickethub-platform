export const AUTH_REFRESH = '/auth/refresh-token'
export const AUTH_PROFILE = '/auth/profile'
export const AUTH_LOGOUT = '/auth/logout'
export const GOOGLE_REDIRECT = '/auth/google/redirect'
export const ADMIN_AUTH_LOGIN = '/auth/admin/login'
export const ADMIN_AUTH_CONFIRM = '/auth/admin/confirm'
export const MODERATOR_AUTH_LOGIN = '/auth/moderator/login'
export const ADMIN_MODERATOR_REGISTER = '/auth/admin/moderators/register'
export const MODERATOR_ACTIVATE_ACCOUNT = '/auth/moderator/activate-account'
export const ORGANIZER_AUTH_LOGIN = '/auth/organizer/login'
export const ORGANIZER_AUTH_REGISTER = '/auth/organizer'
export const ORGANIZER_STAFF_REGISTER = '/auth/organizer/staffs/register'
export const STAFF_AUTH_LOGIN = '/auth/staff/login'
export const STAFF_ACTIVATE_ACCOUNT = '/auth/staff/activate-account'



// example domain endpoints (extend as needed)
export const EVENTS = '/events'
export const USERS = '/users'
// Public Catalog
export const PUBLIC_EVENTS = '/catalog/events'
export const CATALOG_EVENT_DETAIL = (id) => `/catalog/events/${id}`
export const PUBLIC_VENUES = '/catalog/venues'
export const PUBLIC_VENUE_SEATMAP_DETAIL = (venueId, seatMapId) => `/catalog/venue/${venueId}/seat-maps/${seatMapId}`

// Moderator Venues & SeatMaps
export const MODERATOR_VENUE_LIST = '/catalog/moderator/venues'
export const MODERATOR_VENUE_DETAIL = (id) => `/catalog/moderator/venues/${id}`
export const MODERATOR_VENUE_CREATE = '/catalog/moderator/venues'
export const MODERATOR_VENUE_UPDATE = (id) => `/catalog/moderator/venues/${id}`
export const MODERATOR_VENUE_DELETE = (id) => `/catalog/moderator/venues/${id}`

export const MODERATOR_VENUE_SEATMAPS = (venueId) => `/catalog/moderator/venue/${venueId}/seat-maps`
export const MODERATOR_VENUE_SEATMAP_CREATE = (venueId) => `/catalog/moderator/venue/${venueId}/seat-maps`
export const MODERATOR_VENUE_SEATMAP_DETAIL = (venueId, seatMapId) => `/catalog/moderator/venue/${venueId}/seat-maps/${seatMapId}`
export const MODERATOR_VENUE_SEATMAP_DELETE = (venueId, seatMapId) => `/catalog/moderator/venue/${venueId}/seat-maps/${seatMapId}`

// Moderator Events & Categories
export const MODERATOR_EVENT_CATEGORIES = '/catalog/moderator/event-categories'
export const MODERATOR_EVENT_CATEGORY_DETAIL = (id) => `/catalog/moderator/event-categories/${id}`
export const MODERATOR_EVENTS_LIST = '/catalog/moderator/events'
export const MODERATOR_EVENT_DETAIL = (id) => `/catalog/moderator/events/${id}`
export const MODERATOR_EVENT_REVIEW = (id) => `/catalog/moderator/events/${id}/review`
export const MODERATOR_EVENT_CANCEL = (id) => `/catalog/moderator/events/${id}/cancel`
export const MODERATOR_EVENT_CANCELLATION_REQUESTS = '/catalog/moderator/event-cancellation-requests'
export const MODERATOR_EVENT_CANCELLATION_REVIEW = (requestId) => `/catalog/moderator/event-cancellation-requests/${requestId}/review`

// Admin Events & Categories
export const ADMIN_EVENT_CATEGORIES = '/catalog/admin/event-categories'
export const ADMIN_EVENT_CATEGORY_DETAIL = (id) => `/catalog/admin/event-categories/${id}`

// Admin Users
export const ADMIN_USERS = '/auth/admin/users'
export const ADMIN_USER_DETAIL = (id) => `/auth/admin/users/${id}`
export const ADMIN_USER_LOCK_STATUS = (id) => `/auth/admin/users/${id}/lock-status`

// Organizer Venues & SeatMaps
export const ORGANIZER_VENUE_LIST = '/catalog/organizer/venues'
export const ORGANIZER_VENUE_SEATMAPS = (venueId) => `/catalog/organizer/venue/${venueId}/seat-maps`
export const ORGANIZER_VENUE_SEATMAP_DETAIL = (venueId, seatMapId) => `/catalog/organizer/venue/${venueId}/seat-maps/${seatMapId}`

// Organizer Events
export const ORGANIZER_EVENT_CREATE = '/catalog/organizer/events'
export const ORGANIZER_EVENTS_LIST = '/catalog/organizer/events'
export const ORGANIZER_EVENT_DETAIL = (id) => `/catalog/organizer/events/${id}`
export const ORGANIZER_EVENT_CANCELLATION_REQUEST = (eventId) => `/catalog/organizer/event-cancellation-requests/events/${eventId}`

// Common Files & Lookups
export const UPLOAD_SVG = '/catalog/common/files/upload-svg'
export const UPLOAD_COVER_IMAGE = '/catalog/common/files/upload-cover-image'
export const EVENT_STATUSES_LOOKUP = '/catalog/common/lookup/event-statuses'
export const EVENT_STATUSES_FOR_MODERATOR = '/catalog/common/lookup/event-statuses-for-moderator'
export const COMMON_EVENT_CATEGORIES = '/catalog/event-categories'

// Locations API
export const LOCATION_PROVINCES = '/catalog/locations/provinces'
export const LOCATION_DISTRICTS = (provinceCode) => `/catalog/locations/provinces/${provinceCode}/districts`
export const LOCATION_WARDS = (districtCode) => `/catalog/locations/districts/${districtCode}/wards`

// Ordering / Checkout
export const ORDER_CHECKOUT = '/ordering/orders/checkout'
export const ORDER_PAYMENT_LINK = (orderId) => `/ordering/orders/${orderId}/payment-link`
export const ORDER_EVENT_REPORT = (eventId) => `/ordering/orders/reports/events/${eventId}`
export const ORDER_EVENT_ORDERS = (eventId) => `/ordering/orders/reports/events/${eventId}/orders`
export const ORDER_EVENT_CHARTS = (eventId) => `/ordering/orders/reports/events/${eventId}/charts`

// Tickets
export const MY_TICKETS = '/inventory/tickets/me'
export const TICKET_CHECKIN = (qrToken) => `/inventory/tickets/checkin/${qrToken}`

// Event Ratings
export const EVENT_RATINGS = (eventId) => `/catalog/events/${eventId}/ratings`
export const ORGANIZER_EVENT_RATINGS = (eventId) => `/catalog/organizer/events/${eventId}/ratings`

// Event Click Tracking
export const EVENT_CLICK_TRACK = (eventId, clickType) => `/catalog/events/${eventId}/click/${clickType}`

// Organizer Insights
export const ORGANIZER_EVENT_CLICK_TREND = (eventId) => `/catalog/organizer/events/${eventId}/click-trend`
export const ORGANIZER_INSIGHTS = '/catalog/organizer/insights'

// AI Suggestions (Organizer)
export const AI_SUGGESTIONS_MARKET_DIRECTION = '/ai/organizer/suggestions/market-direction'
export const AI_SUGGESTIONS_PORTFOLIO = '/ai/organizer/suggestions/portfolio'
export const AI_SUGGESTIONS_EVENT = (eventId) => `/ai/organizer/suggestions/events/${eventId}`

// AI Chat (Customer)
export const AI_CHAT_SEND = '/ai/chat'

// Notifications (in-app inbox)
export const NOTIFICATIONS = '/notification/notifications'
export const NOTIFICATION_UNREAD_COUNT = '/notification/notifications/unread-count'
export const NOTIFICATION_MARK_READ = (id) => `/notification/notifications/${id}/read`
export const NOTIFICATION_MARK_ALL_READ = '/notification/notifications/read-all'
export const NOTIFICATION_DELETE = (id) => `/notification/notifications/${id}`
export const ADMIN_NOTIFICATION_SEND = '/notification/admin/notifications'
export const ADMIN_NOTIFICATION_SENT_LIST = '/notification/admin/notifications'

// Finance - Moderator Payouts
export const MODERATOR_PAYOUT_REQUESTS = '/finance/moderator/payouts/requests'
export const MODERATOR_PAYOUT_PROPOSE = (payoutRequestId) => `/finance/moderator/payouts/requests/${payoutRequestId}/propose`

// Finance - Organizer Wallet & Payouts
export const ORGANIZER_WALLET = '/finance/organizer/wallet'
export const ORGANIZER_WALLET_TRANSACTIONS = '/finance/organizer/wallet/transactions'
export const ORGANIZER_PAYOUT_REQUEST = (eventId) => `/finance/organizer/payouts/events/${eventId}/request`
export const ORGANIZER_PAYOUT_PROPOSED = '/finance/organizer/payouts/proposed'
export const ORGANIZER_PAYOUT_ACCEPT = (payoutId) => `/finance/organizer/payouts/${payoutId}/accept`
export const ORGANIZER_PAYOUT_REJECT = (payoutId) => `/finance/organizer/payouts/${payoutId}/reject`



