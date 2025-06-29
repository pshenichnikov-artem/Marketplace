import axios from 'axios';
import i18n from "@/i18n/i18n.ts";
import {getToken} from "@/utils/localStorage.ts";
axios.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }

  const lang = i18n.global.locale || 'ru';  // Получаем текущий язык
  config.headers['Accept-Language'] = lang;
    return config;
  },
  (error) => Promise.reject(error)
);
