import { createApp } from 'vue'
import './assets/tailwind.css'
import App from './App.vue'
import router from './router'
import '@/services/api/axiosConfig'
import i18n from '@/i18n/i18n'
import { createPinia } from 'pinia'

const app = createApp(App)

const pinia = createPinia()
app.use(pinia)

// Используем плагины
app.use(router)
app.use(i18n)

// Монтируем приложение
app.mount('#app')
