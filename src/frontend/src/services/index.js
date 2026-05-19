import api from './api/axios'
import { setupInterceptors } from './api/interceptors'
import * as tokenService from './auth/token.service'

setupInterceptors(api, tokenService)

export { api }
export * as authService from './auth/auth.service'
export * as tokenService from './auth/token.service'
