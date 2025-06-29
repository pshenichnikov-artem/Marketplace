import { createI18n } from 'vue-i18n'
import ruLogin from './locale/ru/login.json'
import enLogin from './locale/en/login.json'
import ruRegister from './locale/ru/register.json'
import enRegister from './locale/en/register.json'
import ruServerErrors from './locale/ru/serverErrors.json'
import enServerErrors from './locale/en/serverErrors.json'
import ruValidationErrors from './locale/ru/validation.json'
import enValidationErrors from './locale/en/validation.json'
import ruNotFound from './locale/ru/notFound.json'
import enNotFound from './locale/en/notFound.json'
import ruUnauthorized from './locale/ru/unauthorized.json'
import enUnauthorized from './locale/en/unauthorized.json'
import ruFooter from './locale/ru/footer.json'
import enFooter from './locale/en/footer.json'
import ruDashboard from './locale/ru/dashboard.json'
import enDashboard from './locale/en/dashboard.json'
import ruNavbar from './locale/ru/navbar.json'
import enNavbar from './locale/en/navbar.json'
import ruHome from './locale/ru/home.json'
import enHome from './locale/en/home.json'
import ruProfile from './locale/ru/profile.json'
import enProfile from './locale/en/profile.json'
import ruCatalog from './locale/ru/catalog.json'
import enCatalog from './locale/en/catalog.json'
import ruProductFilter from './locale/ru/productFilter.json'
import enProductFilter from './locale/en/productFilter.json'
import ruBasePagination from './locale/ru/basePagination.json'
import enBasePagination from './locale/en/basePagination.json'
import ruProduct from './locale/ru/product.json'
import enProduct from './locale/en/product.json'
import ruCart from './locale/ru/cart.json'
import enCart from './locale/en/cart.json'
import ruCheckout from './locale/ru/checkout.json'
import enCheckout from './locale/en/checkout.json'
import ruCommon from './locale/ru/common.json'
import enCommon from './locale/en/common.json'
import ruEntityEditor from './locale/ru/entityEditor.json'
import enEntityEditor from './locale/en/entityEditor.json'
import ruStatus from './locale/ru/status.json'
import enStatus from './locale/en/status.json'

const messages = {
  ru: {
    product: ruProduct,
    login: ruLogin,
    register: ruRegister,
    serverErrors: ruServerErrors,
    validation: ruValidationErrors,
    notFound: ruNotFound,
    unauthorized: ruUnauthorized,
    footer: ruFooter,
    dashboard: ruDashboard,
    navbar: ruNavbar,
    home: ruHome,
    profile: ruProfile,
    catalog: ruCatalog,
    productFilter: ruProductFilter,
    basePagination: ruBasePagination,
    cart: ruCart,
    checkout: ruCheckout,
    common: ruCommon,
    entityEditor: ruEntityEditor,
    status: ruStatus,
  },
  en: {
    entityEditor: enEntityEditor,
    product: enProduct,
    login: enLogin,
    register: enRegister,
    serverErrors: enServerErrors,
    validation: enValidationErrors,
    notFound: enNotFound,
    unauthorized: enUnauthorized,
    footer: enFooter,
    dashboard: enDashboard,
    navbar: enNavbar,
    home: enHome,
    profile: enProfile,
    catalog: enCatalog,
    productFilter: enProductFilter,
    basePagination: enBasePagination,
    cart: enCart,
    checkout: enCheckout,
    common: enCommon,
    status: enStatus,
  },
}

// Функция для получения языка из localStorage или использования языка браузера
function getInitialLocale(): 'ru' | 'en' {
  // Проверяем, есть ли сохраненный язык в localStorage
  const savedLocale = localStorage.getItem('user-locale')

  if (savedLocale && ['ru', 'en'].includes(savedLocale)) {
    return savedLocale as 'ru' | 'en'
  }

  // Если язык не найден в localStorage,
  // пытаемся использовать язык браузера или язык из navigator.languages
  const browserLang = navigator.language.split('-')[0]

  // Проверяем поддерживаемые языки
  if (['ru', 'en'].includes(browserLang)) {
    return browserLang as 'ru' | 'en'
  }

  // Если язык браузера не поддерживается, возвращаем русский язык по умолчанию
  return 'ru'
}

const i18n = createI18n({
  locale: getInitialLocale(),
  messages,
})

// Функция для переключения и сохранения языка
export function setLocale(locale: string) {
  if (['ru', 'en'].includes(locale)) {
    // Добавляем явное приведение типов с помощью as
    i18n.global.locale = locale as 'ru' | 'en'
    localStorage.setItem('user-locale', locale)
    document.documentElement.setAttribute('lang', locale)
  }
}

// При инициализации также устанавливаем атрибут lang для HTML
document.documentElement.setAttribute('lang', i18n.global.locale as string)

export default i18n
