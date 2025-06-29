<template>
  <div v-if="hasBanners" class="relative mb-12 carousel-container" :class="{ 'single-carousel': showSingleBanner }">
    <!-- Стрелка влево -->
    <button @click="prevSlide"
      class="absolute top-1/2 transform -translate-y-1/2 bg-gray-800 bg-opacity-50 text-white p-2 md:p-3 rounded-full hover:bg-opacity-70 z-10 transition-all duration-300 hover:scale-105 carousel-arrow carousel-arrow-left"
      :class="{ 'single-arrow': showSingleBanner }" aria-label="Previous slide">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 md:h-6 md:w-6" fill="none" viewBox="0 0 24 24"
        stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
      </svg>
    </button>

    <!-- Слайдер с баннерами -->
    <div class="carousel-track-container">
      <div class="flex transition-transform duration-500 ease-in-out carousel-track"
        :style="{ transform: `translateX(-${currentSlide * 100}%)` }">
        <div v-for="(group, groupIndex) in bannerGroups" :key="groupIndex" class="carousel-slide">
          <div class="banner-grid" :class="{ 'single-banner': showSingleBanner }">
            <div v-for="(banner, bannerIndex) in group" :key="`${groupIndex}-${bannerIndex}`" class="banner-item">
              <div class="banner-content">
                <img :src="banner.imageUrl" :alt="banner.title" class="banner-image" />
                <div class="banner-overlay"></div>
                <div class="banner-text">
                  <h3 class="banner-title">{{ banner.title }}</h3>
                  <p class="banner-description">{{ banner.description }}</p>
                  <router-link :to="banner.link" class="banner-button">
                    {{ $t('home.viewDetails') }}
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Стрелка вправо -->
    <button @click="nextSlide"
      class="absolute top-1/2 transform -translate-y-1/2 bg-gray-800 bg-opacity-50 text-white p-2 md:p-3 rounded-full hover:bg-opacity-70 z-10 transition-all duration-300 hover:scale-105 carousel-arrow carousel-arrow-right"
      :class="{ 'single-arrow': showSingleBanner }" aria-label="Next slide">
      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 md:h-6 md:w-6" fill="none" viewBox="0 0 24 24"
        stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
      </svg>
    </button>

    <!-- Индикаторы слайдов -->
    <div class="flex justify-center mt-4">
      <button v-for="(_, index) in bannerGroups" :key="index" @click="goToSlide(index)"
        class="mx-1 rounded-full focus:outline-none transition-all duration-300"
        :class="currentSlide === index ? 'bg-indigo-600 w-3 h-3 md:w-4 md:h-4' : 'bg-gray-300 w-2 h-2 md:w-3 md:h-3 hover:bg-gray-400'"
        :aria-label="`Go to slide ${index + 1}`"></button>
    </div>
  </div>

  <!-- Состояние загрузки -->
  <div v-else-if="isLoading" class="flex justify-center items-center my-12 py-12">
    <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
  </div>

</template>

<script>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { useI18n } from 'vue-i18n';
import bannerService from '@/services/api/bannerService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
  name: 'BannerCarousel',
  emits: ['bannersLoaded'],
  setup(props, { emit }) {
    const { t } = useI18n();

    const currentSlide = ref(0);
    const autoPlayInterval = ref(null);
    const isTransitioning = ref(false);
    const minBannerHeight = ref(240);
    const banners = ref([]);

    const { loading: isLoading, error, execute } = useApiRequest();

    const hasBanners = computed(() => {
      return banners.value && banners.value.length > 0;
    });

    const showSingleBanner = computed(() => {
      return banners.value.length < 6;
    });

    const bannerGroups = computed(() => {
      const groups = [];
      const bannersPerPage = showSingleBanner.value ? 1 : 3;

      for (let i = 0; i < banners.value.length; i += bannersPerPage) {
        groups.push(banners.value.slice(i, i + bannersPerPage));
      }

      return groups;
    });

    const loadBanners = async () => {
      await execute(async () => {
        return await bannerService.getBanners();
      }, {
        onSuccess: (data) => {
          banners.value = data || [];

          emit('bannersLoaded', hasBanners.value);

          console.log('Баннеры успешно загружены:', hasBanners.value);
          if (hasBanners.value) {
            setupCarousel();
          }
        },
        onError: (errorMsg) => {
          console.error('Ошибка при загрузке баннеров:', errorMsg);
          banners.value = [];
          emit('bannersLoaded', false);
        }
      });

      console.log('Загрузка баннеров завершена');
    };

    const nextSlide = () => {
      if (isTransitioning.value || bannerGroups.value.length <= 1) return;
      isTransitioning.value = true;

      const nextSlideIndex = (currentSlide.value + 1) % bannerGroups.value.length;
      currentSlide.value = nextSlideIndex;

      setTimeout(() => {
        isTransitioning.value = false;
      }, 500);

      resetAutoPlay();
    };

    const prevSlide = () => {
      if (isTransitioning.value || bannerGroups.value.length <= 1) return;
      isTransitioning.value = true;

      const prevSlideIndex = (currentSlide.value - 1 + bannerGroups.value.length) % bannerGroups.value.length;
      currentSlide.value = prevSlideIndex;

      setTimeout(() => {
        isTransitioning.value = false;
      }, 500);

      resetAutoPlay();
    };

    const goToSlide = (index) => {
      if (isTransitioning.value || currentSlide.value === index) return;
      currentSlide.value = index;
      resetAutoPlay();
    };

    const startAutoPlay = () => {
      autoPlayInterval.value = setInterval(() => {
        nextSlide();
      }, 20000);
    };

    const resetAutoPlay = () => {
      clearInterval(autoPlayInterval.value);
      startAutoPlay();
    };

    const setupCarousel = () => {
      startAutoPlay();
      setupTouchControls();
    };

    const setupTouchControls = () => {
      let touchStartX = 0;
      let touchEndX = 0;

      const handleTouchStart = (event) => {
        touchStartX = event.changedTouches[0].screenX;
      };

      const handleTouchEnd = (event) => {
        touchEndX = event.changedTouches[0].screenX;
        handleSwipe();
      };

      const handleSwipe = () => {
        const minSwipeDistance = 50;
        if (touchEndX < touchStartX - minSwipeDistance) {
          nextSlide();
        } else if (touchEndX > touchStartX + minSwipeDistance) {
          prevSlide();
        }
      };

      const carouselElement = document.querySelector('.carousel-container');
      if (carouselElement) {
        carouselElement.addEventListener('touchstart', handleTouchStart, false);
        carouselElement.addEventListener('touchend', handleTouchEnd, false);

        carouselElement._touchHandlers = {
          start: handleTouchStart,
          end: handleTouchEnd
        };
      }
    };

    const handleResize = () => {
    };

    const cleanup = () => {
      clearInterval(autoPlayInterval.value);
      window.removeEventListener('resize', handleResize);

      if (hasBanners.value) {
        const carouselElement = document.querySelector('.carousel-container');
        if (carouselElement && carouselElement._touchHandlers) {
          carouselElement.removeEventListener('touchstart', carouselElement._touchHandlers.start);
          carouselElement.removeEventListener('touchend', carouselElement._touchHandlers.end);
        }
      }
    };

    onMounted(async () => {
      await loadBanners();
      window.addEventListener('resize', handleResize);
    });

    onBeforeUnmount(() => {
      cleanup();
    });

    return {
      currentSlide,
      isLoading,
      error,
      banners,
      hasBanners,
      bannerGroups,
      showSingleBanner,
      nextSlide,
      prevSlide,
      goToSlide
    };
  }
};
</script>

<style scoped>
.carousel-container {
  position: relative;
  width: 100%;
  margin-bottom: 1.5rem;
  overflow: hidden;
}

.carousel-track-container {
  width: 100%;
  overflow: hidden;
  position: relative;
}

.carousel-track {
  display: flex;
  width: 100%;
  transition: transform 0.5s ease-in-out;
}

.carousel-slide {
  flex: 0 0 100%;
  min-width: 100%;
  box-sizing: border-box;
  padding: 0 10px;
}

.banner-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
  width: 100%;
}

.banner-grid.single-banner {
  grid-template-columns: 1fr;
  max-width: 800px;
  margin: 0 auto;
}

.banner-grid.single-banner .banner-item {
  padding-bottom: 35%;
  min-height: 180px;
  max-height: 350px;
}

.single-carousel {
  max-width: 900px;
  margin-left: auto;
  margin-right: auto;
}

.carousel-arrow-left {
  left: 2px;
}

.carousel-arrow-right {
  right: 2px;
}

.single-arrow.carousel-arrow-left {
  left: 10px;
}

.single-arrow.carousel-arrow-right {
  right: 10px;
}

@media (min-width: 768px) {
  .carousel-arrow-left {
    left: 6px;
  }

  .carousel-arrow-right {
    right: 6px;
  }

  .single-arrow.carousel-arrow-left {
    left: 15px;
  }

  .single-arrow.carousel-arrow-right {
    right: 15px;
  }
}

.banner-item {
  position: relative;
  width: 100%;
  padding-bottom: 45%;
  min-height: 160px;
  overflow: hidden;
}

.banner-content {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: 0.75rem;
  overflow: hidden;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  transition: transform 0.3s ease;
}

.banner-content:hover {
  transform: translateY(-5px);
}

.banner-image {
  position: absolute;
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.banner-content:hover .banner-image {
  transform: scale(1.05);
}

.banner-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0, 0, 0, 0.7) 0%, transparent 70%);
}

.banner-text {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  padding: 1rem;
  color: white;
  z-index: 2;
}

.banner-title {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: 0.25rem;
}

.banner-description {
  font-size: 0.875rem;
  margin-bottom: 0.75rem;
}

.banner-button {
  display: inline-block;
  background-color: rgb(79, 70, 229);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  font-weight: 500;
  font-size: 0.875rem;
  transition: background-color 0.3s;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.banner-button:hover {
  background-color: rgb(67, 56, 202);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

@media (max-width: 1024px) {
  .banner-grid {
    gap: 12px;
  }

  .banner-title {
    font-size: 1.125rem;
  }

  .banner-description {
    font-size: 0.8125rem;
  }

  .banner-button {
    padding: 0.375rem 0.875rem;
  }

  .banner-item {
    min-height: 140px;
  }

  .banner-grid.single-banner {
    max-width: 700px;
  }

  .single-carousel {
    max-width: 800px;
  }
}

@media (max-width: 768px) {
  .banner-grid {
    grid-template-columns: repeat(3, 1fr);
    gap: 10px;
  }

  .banner-item {
    min-height: 120px;
  }

  .banner-grid.single-banner {
    max-width: 600px;
  }

  .banner-grid.single-banner .banner-item {
    min-height: 160px;
    padding-bottom: 45%;
  }

  .single-carousel {
    max-width: 650px;
  }
}

@media (max-width: 640px) {
  .carousel-slide {
    padding: 0 5px;
  }

  .banner-grid {
    grid-template-columns: repeat(3, 1fr);
    gap: 8px;
  }

  .banner-item {
    min-height: 100px;
  }

  .banner-grid.single-banner {
    max-width: 90%;
  }

  .banner-grid.single-banner .banner-item {
    min-height: 140px;
    padding-bottom: 60%;
  }
}

@media (max-width: 480px) {
  .banner-text {
    padding: 0.5rem;
  }

  .banner-title {
    font-size: 0.875rem;
    margin-bottom: 0.125rem;
  }

  .banner-description {
    font-size: 0.6875rem;
    margin-bottom: 0.375rem;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .banner-button {
    padding: 0.25rem 0.625rem;
    font-size: 0.6875rem;
  }

  .banner-grid.single-banner .banner-item {
    min-height: 120px;
    padding-bottom: 65%;
  }

  .banner-grid.single-banner {
    max-width: 95%;
  }
}
</style>
