import axios from 'axios'
import { setupInterceptors } from './interceptors'
import * as tokenService from '../auth/token.service'

export const API_BASE_URL = import.meta.env.VITE_API_URL

const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Register request/response interceptors with the token service
try {
  setupInterceptors(api, tokenService)
} catch (e) {
  // fail gracefully in environments where setup may error
  // console.warn('[api] setupInterceptors failed', e)
}

export default api
