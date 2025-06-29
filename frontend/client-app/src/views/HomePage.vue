<template>
  <div class="flex flex-col min-h-screen">
    <MainNavbar />
    <main class="flex-grow bg-gray-50 py-8">
      <div class="container mx-auto px-4">
        <BannerCarousel @bannersLoaded="handleBannersLoaded" />
      </div>
    </main>

    <RecentlyViewedProducts v-if="showRecentProducts" />
    <AppFooter />
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import MainNavbar from '@/components/layout/MainNavbar.vue';
import AppFooter from '@/components/layout/AppFooter.vue';
import BannerCarousel from '@/components/BannerCarousel.vue';
import RecentlyViewedProducts from '@/components/product/RecentlyViewedProducts.vue';
import historyService from '@/services/api/historyService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
  name: 'HomePage',
  components: {
    MainNavbar,
    AppFooter,
    BannerCarousel,
    RecentlyViewedProducts
  },
  setup() {
    const showRecentProducts = ref(false);
    const { execute } = useApiRequest();

    const handleBannersLoaded = (hasBanners) => {
      console.log('Состояние баннеров получено в HomePage:', hasBanners);
    };

    const checkRecentlyViewed = async () => {
      try {
        await execute(async () => {
          return await historyService.getRecentlyViewed();
        }, {
          onSuccess: (data) => {
            showRecentProducts.value = data && Array.isArray(data) && data.length > 0;
          },
          onError: () => {
            showRecentProducts.value = false;
          }
        });
      } catch (error) {
        console.error('Ошибка при проверке наличия недавно просмотренных товаров:', error);
        showRecentProducts.value = false;
      }
    };

    onMounted(async () => {
      await checkRecentlyViewed();
    });

    return {
      showRecentProducts,
      handleBannersLoaded
    };
  }
};
</script>
