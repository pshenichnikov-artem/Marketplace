<template>
  <div class="flex flex-col min-h-screen bg-gray-50">
    <MainNavbar />

    <!-- Основной контент -->
    <div class="flex-grow px-4 py-8 lg:py-12">
      <div class="container mx-auto">
        <!-- Заголовок для мобильных устройств -->
        <h1 class="text-2xl font-bold text-gray-800 mb-6 md:hidden text-center">
          {{ $t('profile.myProfile') }}
        </h1>

        <div class="flex flex-col md:flex-row md:space-x-6">
          <!-- Левое меню - скрыто для seller -->
          <div v-if="!isSeller" class="w-full md:w-64 mb-6 md:mb-0">
            <!-- Мобильный переключатель вкладок -->
            <div class="md:hidden bg-white rounded-lg shadow mb-6 overflow-hidden">
              <select v-model="activeTab"
                class="w-full p-4 bg-white border-0 focus:ring-2 focus:ring-indigo-500 text-gray-800">
                <option value="info">{{ $t('profile.infoTab') }}</option>
                <option value="orders">{{ $t('profile.ordersTab') }}</option>
                <option value="comments">{{ $t('profile.commentsTab') }}</option>
              </select>
            </div>

            <!-- Десктопное меню -->
            <div class="hidden md:block bg-white rounded-lg shadow overflow-hidden">
              <div class="p-4 bg-indigo-50 border-l-4 border-indigo-500">
                <h2 class="font-bold text-gray-800 text-lg">{{ $t('profile.navigation') }}</h2>
              </div>

              <div class="p-2">
                <button :class="['w-full mb-1 py-3 px-4 rounded-lg text-left transition flex items-center',
                  activeTab === 'info'
                    ? 'bg-indigo-600 text-white shadow-md'
                    : 'hover:bg-gray-100 text-gray-700']" @click="activeTab = 'info'">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  {{ $t('profile.infoTab') }}
                </button>

                <button :class="['w-full mb-1 py-3 px-4 rounded-lg text-left transition flex items-center',
                  activeTab === 'orders'
                    ? 'bg-indigo-600 text-white shadow-md'
                    : 'hover:bg-gray-100 text-gray-700']" @click="activeTab = 'orders'">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                  {{ $t('profile.ordersTab') }}
                </button>

                <button :class="['w-full py-3 px-4 rounded-lg text-left transition flex items-center',
                  activeTab === 'comments'
                    ? 'bg-indigo-600 text-white shadow-md'
                    : 'hover:bg-gray-100 text-gray-700']" @click="activeTab = 'comments'">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z" />
                  </svg>
                  {{ $t('profile.commentsTab') }}
                </button>
              </div>
            </div>

            <!-- Статистика -->
            <div class="hidden md:block mt-6">
              <ProfileStats />
            </div>
          </div>

          <!-- Основной контент -->
          <div class="flex-1">
            <div class="bg-white rounded-lg shadow overflow-hidden">
              <!-- Заголовок для десктопа -->
              <div class="hidden md:block p-6 bg-indigo-50 border-b border-gray-200">
                <h1 class="text-2xl font-bold text-gray-800">
                  {{ isSeller ? $t('profile.infoTab') :
                    activeTab === 'info' ? $t('profile.infoTab') :
                      activeTab === 'orders' ? $t('profile.ordersTab') : $t('profile.commentsTab') }}
                </h1>
              </div>

              <!-- Контент активной вкладки -->
              <div class="p-6">
                <transition name="fade" mode="out-in" enter-active-class="transition-opacity ease-in-out duration-300"
                  leave-active-class="transition-opacity ease-in-out duration-300" enter-from-class="opacity-0"
                  leave-to-class="opacity-0">
                  <div :key="isSeller ? 'info' : activeTab"> <!-- Обертка для содержимого transition -->
                    <ProfileInfo v-if="isSeller || activeTab === 'info'" :is-admin="isAdmin" />
                    <ProfileOrders v-if="!isSeller && activeTab === 'orders'" />
                    <ProfileComments v-if="!isSeller && activeTab === 'comments'" />
                  </div>
                </transition>

                <!-- Отображаем статистику для обычных пользователей на мобильных устройствах -->
                <div v-if="!isSeller && isMobile" class="mt-8">
                  <ProfileStats />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <AppFooter />
  </div>
</template>

<script>
import MainNavbar from '@/components/layout/MainNavbar.vue';
import AppFooter from '@/components/layout/AppFooter.vue';
import ProfileInfo from '@/components/profile/ProfileInfo.vue';
import ProfileOrders from '@/components/profile/ProfileOrders.vue';
import ProfileComments from '@/components/profile/ProfileComments.vue';
import ProfileStats from '@/components/profile/ProfileStats.vue';
import { getUserRole } from '@/utils/localStorage';
import { ref, computed, onMounted, onUnmounted } from 'vue';

export default {
  name: 'ProfilePage',
  components: {
    MainNavbar,
    AppFooter,
    ProfileInfo,
    ProfileOrders,
    ProfileComments,
    ProfileStats
  },
  setup() {
    const activeTab = ref('info');
    const windowWidth = ref(window.innerWidth);

    // Определяем, является ли устройство мобильным
    const isMobile = computed(() => windowWidth.value < 768);

    // Определяем роли
    const isAdmin = computed(() => getUserRole() === 'admin');
    const isSeller = computed(() => getUserRole() === 'seller');

    // Обработчик изменения размера окна
    const handleResize = () => {
      windowWidth.value = window.innerWidth;
    };

    // Отслеживаем изменения хэша в URL для переключения вкладок
    const handleHashChange = () => {
      const hash = window.location.hash;
      if (hash) {
        const tab = hash.substring(1);
        // Для продавца показываем только вкладку info
        if (isSeller.value) {
          activeTab.value = 'info';
          window.location.hash = '#info';
        } else if (['info', 'orders', 'comments'].includes(tab)) {
          activeTab.value = tab;
        }
      }
    };

    onMounted(() => {
      window.addEventListener('resize', handleResize);
      handleHashChange(); // Инициализация по хэшу URL
      window.addEventListener('hashchange', handleHashChange);
    });

    onUnmounted(() => {
      window.removeEventListener('resize', handleResize);
      window.removeEventListener('hashchange', handleHashChange);
    });

    // Наблюдаем за изменениями в activeTab
    const watchActiveTab = (tab) => {
      // Для продавца всегда отображаем только вкладку info
      if (isSeller.value && tab !== 'info') {
        activeTab.value = 'info';
        window.location.hash = '#info';
      } else {
        window.location.hash = `#${tab}`;
      }
    };

    return {
      activeTab,
      isAdmin,
      isSeller,
      isMobile,
      watchActiveTab
    };
  },
  watch: {
    // Обновляем URL при смене вкладки
    activeTab: {
      handler(tab) {
        this.watchActiveTab(tab);
      }
    }
  }
};
</script>

<style scoped>
/* Тени для элементов при наведении */
.shadow:hover {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  transition: box-shadow 0.3s ease-in-out;
}

/* Стили для мобильной навигации */
select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='%234B5563'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M19 9l-7 7-7-7'%3E%3C/path%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  background-size: 1em;
  padding-right: 2.5rem;
}

/* Дополнительные медиа-запросы для адаптивности */
@media (max-width: 640px) {
  .container {
    padding: 0;
  }
}
</style>
