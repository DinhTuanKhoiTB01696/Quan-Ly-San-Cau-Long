import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'

import './assets/main.css'

import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'
import vue3GoogleLogin from 'vue3-google-login'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(Toast, {
  position: 'top-right',
  timeout: 3000,
  closeOnClick: true
})

app.use(vue3GoogleLogin, {
  clientId: '1008910270642-flh9lb3sb1241tpvs9ssj495uhvhhmgb.apps.googleusercontent.com'
})

app.mount('#app')
