<template>
  <header class="bg-white shadow-md border-b border-gray-200 sticky top-0 z-50">
    <div class="container mx-auto px-4">
      <div class="flex items-center justify-between h-20">
        <!-- Логотип и название -->
        <div class="flex items-center">
          <router-link to="/"
            class="flex items-center text-2xl font-bold text-indigo-600 hover:text-indigo-800 transition-colors duration-300">
            <svg class="h-8 w-8 mr-2" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M12 2L2 7L12 12L22 7L12 2Z" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                stroke-linejoin="round" />
              <path d="M2 17L12 22L22 17" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                stroke-linejoin="round" />
              <path d="M2 12L12 17L22 12" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                stroke-linejoin="round" />
            </svg>
            <span class="hidden sm:block">iMarket</span>
          </router-link>
        </div>

        <!-- Поиск - всегда виден -->
        <div class="flex-1 mx-6">
          <NavbarSearch />
        </div>

        <!-- Кнопки навигации - показаны на десктопе (теперь только от 1024px и выше) -->
        <nav class="hidden lg:flex items-center space-x-1">
          <router-link to="/catalog"
            class="px-3 py-2 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-700 transition-colors duration-200 flex items-center font-medium"
            active-class="bg-indigo-50 text-indigo-700">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
            </svg>
            {{ $t('navbar.catalog') }}
          </router-link>

          <!-- Добавляем ссылку на панель управления для seller -->
          <router-link v-if="isSeller || isAdmin" to="/dashboard"
            class="px-3 py-2 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-700 transition-colors duration-200 flex items-center font-medium"
            active-class="bg-indigo-50 text-indigo-700">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
            {{ $t('navbar.dashboard') }}
          </router-link>

          <!-- Скрыть ссылку на профиль для админа -->
          <router-link v-if="isAuthenticated && !isAdmin" to="/profile"
            class="px-3 py-2 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-700 transition-colors duration-200 flex items-center font-medium"
            active-class="bg-indigo-50 text-indigo-700">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
            {{ $t('navbar.profile') }}
          </router-link>

          <!-- Отображаем корзину только для обычных пользователей -->
          <router-link v-if="!isSellerOrAdmin" to="/cart"
            class="relative px-3 py-2 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-700 transition-colors duration-200 flex items-center font-medium"
            active-class="bg-indigo-50 text-indigo-700">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            {{ $t('navbar.cart') }}
            <span v-if="cartItemsCount > 0"
              class="absolute -top-1 -right-1 bg-indigo-600 text-white text-xs font-bold rounded-full h-5 w-5 flex items-center justify-center">
              {{ cartItemsCount }}
            </span>
          </router-link>

          <!-- Переключатель языка -->
          <div class="px-3 py-2">
            <LanguageSwitcher />
          </div>

          <button v-if="!isAuthenticated" @click="goToLogin"
            class="ml-2 bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-lg transition-colors duration-200 font-medium shadow-sm hover:shadow flex items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1" />
            </svg>
            {{ $t('navbar.login') }}
          </button>

          <button v-else @click="logout"
            class="ml-2 bg-white border border-gray-300 hover:bg-gray-50 text-gray-700 px-4 py-2 rounded-lg transition-colors duration-200 font-medium flex items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            {{ $t('navbar.logout') }}
          </button>
        </nav>

        <!-- Мобильные иконки (теперь отображаются на экранах до 1024px) -->
        <div class="flex items-center lg:hidden space-x-2">
          <!-- Иконка корзины только для обычных пользователей -->
          <router-link v-if="!isSellerOrAdmin" to="/cart"
            class="relative p-2 rounded-full hover:bg-indigo-50 transition-colors duration-200">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-gray-700" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            <span v-if="cartItemsCount > 0"
              class="absolute top-0 right-0 bg-indigo-600 text-white text-xs font-bold rounded-full h-5 w-5 flex items-center justify-center">
              {{ cartItemsCount }}
            </span>
          </router-link>

          <!-- Кнопка меню (теперь отображается на экранах до 1024px) -->
          <button @click="toggleMobileMenu" class="p-2 rounded-lg hover:bg-gray-100 transition-colors cursor-pointer"
            data-menu-btn type="button" aria-label="Меню">
            <svg v-if="!mobileMenuOpen" xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-gray-700" fill="none"
              viewBox="0 0 24 24" stroke="currentColor" pointer-events="none">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"
                pointer-events="none" />
            </svg>
            <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-gray-700" fill="none" viewBox="0 0 24 24"
              stroke="currentColor" pointer-events="none">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"
                pointer-events="none" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Мобильное меню (теперь отображается на экранах до 1024px) -->
      <div v-if="mobileMenuOpen" class="lg:hidden pb-4 animate-fadeIn">
        <nav class="flex flex-col space-y-3 mt-2">
          <router-link to="/catalog" @click="mobileMenuOpen = false"
            class="px-4 py-3 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-700 transition-colors flex items-center font-medium"
            active-class="bg-indigo-50 text-indigo-700">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
            </svg>
            {{ $t('navbar.catalog') }}
          </router-link>

          <!-- Добавляем ссылку на панель управления для seller в мобильном меню -->
          <router-link v-if="isSeller" to="/dashboard" @click="mobileMenuOpen = false"
            class="px-4 py-3 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-700 transition-colors flex items-center font-medium"
            active-class="bg-indigo-50 text-indigo-700">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
            {{ $t('navbar.dashboard') }}
          </router-link>

          <!-- Скрыть ссылку на профиль для админа в мобильном меню -->
          <router-link v-if="isAuthenticated && !isAdmin" to="/profile" @click="mobileMenuOpen = false"
            class="px-4 py-3 rounded-lg text-gray-700 hover:bg-indigo-50 hover:text-indigo-700 transition-colors flex items-center font-medium"
            active-class="bg-indigo-50 text-indigo-700">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
            {{ $t('navbar.profile') }}
          </router-link>

          <!-- Переключатель языка в мобильном меню -->
          <div class="px-4 py-3 flex justify-start">
            <LanguageSwitcher />
          </div>

          <button v-if="!isAuthenticated" @click="goToLogin"
            class="w-full flex items-center px-4 py-3 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg transition-colors font-medium shadow-sm">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1" />
            </svg>
            {{ $t('navbar.login') }}
          </button>

          <button v-else @click="logout"
            class="w-full flex items-center px-4 py-3 bg-white border border-gray-300 hover:bg-gray-50 text-gray-700 rounded-lg transition-colors font-medium">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
              stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            {{ $t('navbar.logout') }}
          </button>
        </nav>
      </div>
    </div>
  </header>
</template>

<script>
import NavbarSearch from '@/components/layout/NavbarSearch.vue';
import LanguageSwitcher from '@/components/ui/LanguageSwitcher.vue';
import authService from '@/services/api/authService';
import cartService from '@/services/api/cartService';
import { getToken, getUserRole } from '@/utils/localStorage';
import { useCartStore } from "@/stores/modules/cart"
import { watch } from 'vue';

export default {
  name: 'MainNavbar',
  components: {
    NavbarSearch,
    LanguageSwitcher
  },
  data() {
    return {
      mobileMenuOpen: false,
      cartItemsCount: 0,
      userRole: null
    };
  },
  computed: {
    isAuthenticated() {
      return !!getToken();
    },
    currentUserRole() {
      return this.userRole || getUserRole();
    },
    isAdmin() {
      return this.currentUserRole === 'admin';
    },
    isSeller() {
      return this.currentUserRole === 'seller';
    },
    isSellerOrAdmin() {
      return this.isAdmin || this.isSeller;
    }
  },
  mounted() {
    this.getCartItemsCount();
    this.userRole = getUserRole();
    window.addEventListener('resize', this.handleResize);
    document.addEventListener('click', this.handleOutsideClick);

    // Инициализация и настройка отслеживания через Pinia
    this.setupCartWatcher();
  },
  beforeUnmount() {
    window.removeEventListener('resize', this.handleResize);
    document.removeEventListener('click', this.handleOutsideClick);
  },
  methods: {
    setupCartWatcher() {
      const cartStore = useCartStore();

      // Отслеживаем изменения в хранилище корзины
      watch(() => cartStore.cartItemsCountDelta, (newDelta) => {
        if (newDelta !== 0) {
          // Обновляем счетчик товаров в корзине на величину дельты
          this.cartItemsCount += newDelta;

          // Сбрасываем дельту в 0
          cartStore.$patch({ cartItemsCountDelta: 0 });
        }
      });
    },

    toggleMobileMenu() {
      this.mobileMenuOpen = !this.mobileMenuOpen;
      console.log('Toggled mobile menu, now:', this.mobileMenuOpen);
    },
    logout() {
      authService.logout();
      this.$router.push('/login');
      this.mobileMenuOpen = false;
    },
    goToLogin() {
      this.$router.push('/login');
      this.mobileMenuOpen = false;
    },
    handleResize() {
      if (window.innerWidth >= 1024) {
        this.mobileMenuOpen = false;
      }
    },
    handleOutsideClick(event) {
      const header = document.querySelector('header');
      const menuBtn = document.querySelector('[data-menu-btn]');

      // Пропускаем обработку клика по самой кнопке меню и её дочерним элементам
      if (menuBtn && (menuBtn === event.target || menuBtn.contains(event.target))) {
        return;
      }

      // Закрываем меню, если клик был вне заголовка
      if (this.mobileMenuOpen && !header.contains(event.target)) {
        this.mobileMenuOpen = false;
      }
    },
    async getCartItemsCount() {
      // Не выполняем запрос для seller и admin
      if (!this.isAuthenticated || this.isSellerOrAdmin) {
        this.cartItemsCount = 0;
        return;
      }

      try {
        const response = await cartService.getCart("self");
        if (response.status === 'success' && response.data && response.data.items) {
          this.cartItemsCount = response.data.items.length;
        } else {
          this.cartItemsCount = 0;
        }
      } catch (error) {
        console.error('Ошибка при получении данных корзины:', error);
        this.cartItemsCount = 0;
      }
    }
  }
};
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.2s ease-out forwards;
}

/* Улучшение кликабельности кнопок на мобильных устройствах */
@media (max-width: 1023px) {

  button,
  a {
    min-height: 44px;
    /* Минимальная высота для сенсорных устройств */
    min-width: 44px;
    /* Минимальная ширина для сенсорных устройств */
  }

  /* Увеличение активной области кнопок */
  nav a,
  nav button {
    display: flex;
    align-items: center;
    width: 100%;
    padding: 12px 16px;
    margin-bottom: 8px;
  }
}
</style>
