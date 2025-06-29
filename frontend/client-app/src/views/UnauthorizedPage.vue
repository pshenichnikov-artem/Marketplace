<template>
  <div class="min-h-screen flex flex-col">
    <MainNavbar />
    <div class="flex-grow flex flex-col items-center justify-center bg-gray-100 px-4 py-16">
      <div class="w-full max-w-md text-center mb-12"> <!-- Добавлен отступ внизу -->
        <h1 class="text-6xl font-bold text-red-600 mb-4">403</h1>
        <h2 class="text-2xl font-semibold text-gray-700 mb-6">{{ $t('unauthorized.title') }}</h2>
        <p class="text-gray-600 mb-8">{{ $t('unauthorized.message') }}</p>
        <div class="flex justify-center gap-4">
          <button 
            @click="$router.push('/')" 
            class="px-6 py-3 bg-indigo-600 text-white font-medium text-sm rounded-md hover:bg-indigo-700 focus:outline-none"
          >
            {{ $t('unauthorized.returnHome') }}
          </button>
          <button 
            @click="logout" 
            class="px-6 py-3 bg-red-600 text-white font-medium text-sm rounded-md hover:bg-red-700 focus:outline-none"
          >
            {{ $t('unauthorized.logout') }}
          </button>
        </div>
      </div>
    </div>
    <AppFooter />
  </div>
</template>

<script>
import { removeToken, removeUserRole, setAuthToken } from '@/utils/localStorage';
import AppFooter from '@/components/layout/AppFooter.vue';
import MainNavbar from '@/components/layout/MainNavbar.vue';

export default {
  name: 'UnauthorizedPage',
  components: {
    AppFooter,
    MainNavbar
  },
  methods: {
    logout() {
      removeToken();
      removeUserRole();
      setAuthToken(null);
      this.$router.push('/login');
    }
  }
}
</script>
