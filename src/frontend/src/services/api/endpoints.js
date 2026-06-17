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

// Catalog
export const VENUE_CREATE = '/catalog/venues'
export const VENUE_LIST = '/catalog/venues'
export const VENUE_DETAIL = (id) => `/catalog/venues/${id}`
export const VENUE_UPDATE = (id) => `/catalog/venues/${id}`
export const VENUE_DELETE = (id) => `/catalog/venues/${id}`
export const VENUE_SEATMAPS = (venueId) => `/catalog/venue/${venueId}/seat-maps`
export const VENUE_SEATMAP_CREATE = (venueId) => `/catalog/venue/${venueId}/seat-maps`
export const VENUE_SEATMAP_DETAIL = (venueId, seatMapId) => `/catalog/venue/${venueId}/seat-maps/${seatMapId}`
export const VENUE_SEATMAP_DELETE = (venueId, seatMapId) => `/catalog/venue/${venueId}/seat-maps/${seatMapId}`
export const UPLOAD_SVG = '/catalog/files/upload-svg'

// Locations (Vietnam Provinces API)
export const LOCATION_PROVINCES = '/p/'
export const LOCATION_DISTRICTS = (provinceCode) => `/p/${provinceCode}`
export const LOCATION_WARDS = (districtCode) => `/d/${districtCode}`

// Event Categories
export const EVENT_CATEGORIES = '/catalog/event-categories'

// File Upload
export const UPLOAD_COVER_IMAGE = '/catalog/files/upload-cover-image'

// Organizer Events
export const ORGANIZER_EVENT_CREATE = '/catalog/events'
