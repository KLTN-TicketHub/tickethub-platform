import { createApp } from 'vue'
import App from './App.vue'
import router from './core/router'
import './assets/main.css'
import { initStore } from './features/events/store'
import { tryRefresh } from './features/events/auth/auth.service'
import { isLoggedIn } from './features/events/auth/token.service'

const app = createApp(App)
app.use(router)

// Hydrate the event store from the service layer, then mount.
// Mounting must not be blocked by init failures (avoid blank screen).
;(async () => {
  try {
    // Only refresh if user has logged in before (indicated by stored flag)
    if (isLoggedIn()) {
      // small random jitter to avoid many clients refreshing at the exact same instant
      await new Promise(res => setTimeout(res, Math.floor(Math.random() * 300)))
      await tryRefresh().catch(() => null)
    }
    await initStore()
  } catch (err) {
    console.error('[TicketHub] initStore failed, mounting anyway:', err)
  } finally {
    app.mount('#app')
  }
})()
