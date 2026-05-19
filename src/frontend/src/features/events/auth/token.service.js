// simple in-memory token holder
let accessToken = null
const LOGIN_FLAG_KEY = 'ticket-hub:is-logged-in'

export function setToken(t) {
  accessToken = t
  if (t) {
    localStorage.setItem(LOGIN_FLAG_KEY, 'true')
  }
}

export function getToken() {
  return accessToken
}

export function isLoggedIn() {
  return localStorage.getItem(LOGIN_FLAG_KEY) === 'true'
}

export function clearLogin() {
  accessToken = null
  localStorage.removeItem(LOGIN_FLAG_KEY)
}

export default { setToken, getToken, isLoggedIn, clearLogin }
