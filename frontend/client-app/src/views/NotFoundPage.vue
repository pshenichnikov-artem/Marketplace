<template>
  <div class="min-h-screen flex flex-col">
    <MainNavbar />
    <div class="flex-grow flex flex-col items-center justify-center bg-gray-100 px-4 py-16">
      <div class="w-full max-w-md text-center mb-12"> <!-- Добавлен отступ внизу -->
        <h1 class="text-6xl font-bold text-gray-800 mb-4">404</h1>
        <h2 class="text-2xl font-semibold text-gray-700 mb-6">{{ $t('notFound.title') }}</h2>
        <p class="text-gray-600 mb-8">{{ $t('notFound.message') }}</p>
        <div class="flex justify-center">
          <button 
            @click="$router.push('/')" 
            class="px-6 py-3 bg-indigo-600 text-white font-medium text-sm rounded-md hover:bg-indigo-700 focus:outline-none"
          >
            {{ $t('notFound.returnHome') }}
          </button>
        </div>
      </div>
    </div>
    <AppFooter />
  </div>
</template>

<script>
import AppFooter from '@/components/layout/AppFooter.vue';
import MainNavbar from '@/components/layout/MainNavbar.vue';

export default {
  name: 'NotFoundPage',
  components: {
    AppFooter,
    MainNavbar
  }
}
</script>
