import { getCurrentRole, redirectToRoleLogin, shouldRedirectToLogin, tryRefresh } from '../auth/auth.service'

export function setupInterceptors(api, tokenService) {
  let isRefreshing = false
  let queue = []

  const processQueue = (err, token) => {
    queue.forEach(cb => cb(err, token))
    queue = []
  }

  api.interceptors.request.use(cfg => {
    const token = tokenService.getToken()
    if (token) {
      cfg.headers = cfg.headers || {}
      cfg.headers.Authorization = `Bearer ${token}`
    }
    return cfg
  })

  api.interceptors.response.use(
    r => r,
    async err => {
      const orig = err.config
      if (!orig) {
        return Promise.reject(err)
      }

      if (err.response && err.response.status === 401 && !orig._retry) {
        if (isRefreshing) {
          return new Promise((resolve, reject) => {
            queue.push((error, newToken) => {
              if (error) return reject(error)
              orig.headers = orig.headers || {}
              orig.headers.Authorization = `Bearer ${newToken}`
              resolve(api(orig))
            })
          })
        }

        orig._retry = true
        isRefreshing = true
        try {
          const newToken = await tryRefresh()
          processQueue(null, newToken)
          orig.headers = orig.headers || {}
          orig.headers.Authorization = `Bearer ${newToken}`
          return api(orig)
        } catch (e) {
          processQueue(e, null)
          tokenService.setToken(null)
          if (shouldRedirectToLogin(e)) {
            redirectToRoleLogin(getCurrentRole())
          }
          return Promise.reject(e)
        } finally {
          isRefreshing = false
        }
      }
      return Promise.reject(err)
    }
  )
}
