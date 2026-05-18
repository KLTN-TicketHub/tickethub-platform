import { createApp } from 'vue'
import App from './App.vue'
import router from './core/router'
import './assets/main.css'
import { initStore } from './features/events/store'

const app = createApp(App)
app.use(router)

// Hydrate the event store from the service layer, then mount.
// Mounting must not be blocked by init failures (avoid blank screen).
;(async () => {
  try {
    await initStore()
  } catch (err) {
    console.error('[TicketHub] initStore failed, mounting anyway:', err)
  } finally {
    app.mount('#app')
  }
})()
