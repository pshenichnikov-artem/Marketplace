<template>
  <div class="flex flex-col min-h-screen bg-gray-100">
    <div class="flex flex-grow h-screen">
      <!-- Боковое меню -->
      <aside class="w-64 bg-gray-900 text-white flex flex-col h-full fixed left-0 z-10 shadow-lg">
        <div class="p-5 border-b border-gray-800 flex items-center space-x-3">
          <div class="p-2 bg-indigo-600 rounded-lg">
            <svg class="h-6 w-6" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M12 2L2 7L12 12L22 7L12 2Z" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                stroke-linejoin="round" />
              <path d="M2 17L12 22L22 17" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                stroke-linejoin="round" />
              <path d="M2 12L12 17L22 12" stroke="currentColor" stroke-width="2" stroke-linecap="round"
                stroke-linejoin="round" />
            </svg>
          </div>
          <div class="flex flex-col">
            <h2 class="text-lg font-bold text-white whitespace-nowrap overflow-hidden text-ellipsis max-w-[200px]">
              {{ $t('dashboard.title') }}
            </h2>
            <p class="text-xs text-gray-400">
              {{ isAdmin ? $t('dashboard.adminRole') : $t('dashboard.sellerRole') }}
            </p>
          </div>
        </div>

        <div class="px-5 py-3 border-b border-gray-800">
          <LanguageSwitcher />
        </div>

        <!-- Навигационные элементы -->
        <nav class="flex-grow pt-2 overflow-y-auto scrollbar-thin scrollbar-thumb-gray-700 scrollbar-track-gray-900">
          <ul class="space-y-1 px-3">
            <!-- Секция навигации для Admin -->
            <template v-if="isAdmin">
              <li>
                <router-link to="/dashboard/products"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'products' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M20 7l-8-4-8 4m16 0v10l-8 4m-8-4V7m8 4v10M4 7v10l8-4" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.products') }}</span>
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard/users"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'users' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.users') }}</span>
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard/orders"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'orders' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.orders') }}</span>
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard/reviews"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'reviews' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.reviews') }}</span>
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard/categories"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'categories' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.categories') }}</span>
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard/banners"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'banners' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.banners') }}</span>
                </router-link>
              </li>

              <li>
                <router-link to="/dashboard/pages"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'pages' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.pages') }}</span>
                </router-link>
              </li>

            </template>

            <!-- Секция навигации для Seller -->
            <template v-else>
              <li>
                <router-link to="/dashboard/products"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'products' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M20 7l-8-4-8 4m16 0v10l-8 4m-8-4V7m8 4v10M4 7v10l8-4" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.products') }}</span>
                </router-link>
              </li>
              <li>
                <router-link to="/dashboard/orders"
                  class="flex items-center space-x-3 px-4 py-3.5 w-full text-left text-gray-300 hover:bg-gray-800 hover:text-white rounded-lg transition duration-200"
                  :class="{ 'bg-indigo-700 text-white hover:bg-indigo-600': activeComponent === 'orders' }">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                  <span class="text-lg font-medium">{{ $t('dashboard.menu.orders') }}</span>
                </router-link>
              </li>
            </template>

            <div class="my-4 border-t border-gray-800"></div>
          </ul>
        </nav>

        <div class="p-4 border-t border-gray-800 mt-auto">
          <div class="space-y-3">
            <a href="/"
              class="flex items-center space-x-3 px-4 py-3 bg-gray-800 text-white rounded-lg hover:bg-gray-700 transition duration-200 w-full">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
              </svg>
              <span class="text-lg font-medium">{{ $t('dashboard.menu.home') }}</span>
            </a>
            <button @click="logout"
              class="flex items-center space-x-3 px-4 py-3 bg-red-600 text-white rounded-lg hover:bg-red-700 transition duration-200 w-full">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
              <span class="text-lg font-medium">{{ $t('dashboard.menu.logout') }}</span>
            </button>
          </div>
        </div>
      </aside>

      <!-- Основной контент -->
      <main class="flex-1 ml-64 bg-gray-100 min-h-screen">
        <!-- Header -->
        <header class="bg-white border-b border-gray-200 shadow-sm py-4 px-6">
          <div class="flex items-center justify-between">
            <h1 class="text-2xl font-bold text-gray-800">
              {{ getPageTitle() }}
            </h1>
            <div class="flex items-center space-x-4">
              <span class="text-sm text-gray-600">{{ formattedDate }}</span>
            </div>
          </div>
        </header>

        <div class="p-6">
          <router-view class="mb-6" />
        </div>
      </main>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import LanguageSwitcher from '@/components/ui/LanguageSwitcher.vue';
import DashboardProducts from '@/components/dashboard/products/DashboardProducts.vue';
import DashboardCategories from '@/components/dashboard/categories/DashboardCategories.vue';
import { getUserRole, removeToken, removeUserRole, setAuthToken } from '@/utils/localStorage';

export default {
  name: 'Dashboard',
  components: {
    LanguageSwitcher,
    DashboardProducts,
    DashboardCategories
  },
  setup() {
    const { t } = useI18n();
    const activeComponent = ref('home');
    const route = useRoute();
    const router = useRouter();
    const hasChildRoute = ref(false);

    const updateActiveComponent = () => {
      const path = route.path;

      if (path.includes('/dashboard/products')) {
        activeComponent.value = 'products';
        hasChildRoute.value = path.includes('/edit/') || path.includes('/create');
      } else if (path.includes('/dashboard/users')) {
        activeComponent.value = 'users';
        hasChildRoute.value = path.includes('/edit/') || path.includes('/create');
      } else if (path.includes('/dashboard/categories')) {
        activeComponent.value = 'categories';
        hasChildRoute.value = path.includes('/edit/') || path.includes('/create');
      } else if (path.includes('/dashboard/orders')) {
        activeComponent.value = 'orders';
        hasChildRoute.value = path.includes('/edit/');
      } else if (path.includes('/dashboard/reviews')) {
        activeComponent.value = 'reviews';
        hasChildRoute.value = path.includes('/edit/') || path.includes('/create');
      } else if (path.includes('/dashboard/banners')) {
        activeComponent.value = 'banners';
      } else if (path.includes('/dashboard/pages')) {
        activeComponent.value = 'pages';
        hasChildRoute.value = path.includes('/edit/') || path.includes('/create');
      }
    };

    watch(() => route.path, updateActiveComponent, { immediate: true });

    onMounted(() => {
      updateActiveComponent();
    });

    const formattedDate = computed(() => {
      const now = new Date();
      return new Intl.DateTimeFormat('ru-RU', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      }).format(now);
    });

    const isAdmin = computed(() => getUserRole() === 'admin');

    function getPageTitle() {
      switch (activeComponent.value) {
        case 'products': return t('dashboard.menu.products');
        case 'users': return t('dashboard.menu.users');
        case 'orders': return t('dashboard.menu.orders');
        case 'reviews': return t('dashboard.menu.reviews');
        case 'categories': return t('dashboard.menu.categories');
        case 'banners': return t('dashboard.menu.banners');
        case 'pages': return t('dashboard.menu.pages');
        case 'statistics': return t('dashboard.menu.statistics');
        default: return t('dashboard.title');
      }
    }

    function setActiveComponent(component) {
      activeComponent.value = component;
      router.push(`/dashboard/${component}`);
    }

    function logout() {
      removeToken();
      removeUserRole();
      setAuthToken(null);
      window.location.href = '/login';
    }

    return {
      activeComponent,
      isAdmin,
      getPageTitle,
      formattedDate,
      setActiveComponent,
      logout,
      hasChildRoute
    }
  }
};
</script>

<style scoped>
.scrollbar-thin {
  scrollbar-width: thin;
}

.scrollbar-thumb-gray-700 {
  scrollbar-color: rgba(55, 65, 81, 0.7) transparent;
}

.scrollbar-thin::-webkit-scrollbar {
  width: 6px;
}

.scrollbar-thin::-webkit-scrollbar-track {
  background: rgba(17, 24, 39, 0.8);
}

.scrollbar-thin::-webkit-scrollbar-thumb {
  background-color: rgba(55, 65, 81, 0.7);
  border-radius: 20px;
}

.router-link-active {
  background-color: rgba(79, 70, 229, 0.2);
  color: white;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

@media (max-width: 768px) {
  .ml-64 {
    margin-left: 0;
  }
}
</style>
