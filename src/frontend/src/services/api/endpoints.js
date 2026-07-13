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

// Organizer Venues & SeatMaps
export const ORGANIZER_VENUE_LIST = '/catalog/organizer/venues'
export const ORGANIZER_VENUE_SEATMAPS = (venueId) => `/catalog/organizer/venue/${venueId}/seat-maps`
export const ORGANIZER_VENUE_SEATMAP_DETAIL = (venueId, seatMapId) => `/catalog/organizer/venue/${venueId}/seat-maps/${seatMapId}`

// Organizer Events
export const ORGANIZER_EVENT_CREATE = '/catalog/organizer/events'
export const ORGANIZER_EVENTS_LIST = '/catalog/organizer/events'
export const ORGANIZER_EVENT_DETAIL = (id) => `/catalog/organizer/events/${id}`

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



