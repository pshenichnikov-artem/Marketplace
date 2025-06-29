<template>
  <div>
    <!-- Добавляем компонент ReviewManager перед основным содержимым -->
    <ReviewManager @review-added="onReviewAdded" />

    <StateWrapper :loading="loading" :error="error" :is-empty="!reviews || reviews.length === 0"
      :empty-message="$t('profile.noComments')">
      <!-- Кастомизация сообщения об отсутствии отзывов -->
      <template #empty>
        <div
          class="bg-gradient-to-r from-blue-50 to-indigo-50 text-blue-700 border border-blue-100 rounded-lg p-8 text-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 mx-auto mb-4 text-blue-400 opacity-80" fill="none"
            viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z" />
          </svg>
          <p class="text-xl font-medium text-indigo-700 mb-3">{{ $t('profile.noComments') }}</p>
          <p class="mt-2 text-blue-600 max-w-md mx-auto">{{ $t('profile.commentsTip') }}</p>
          <router-link to="/catalog"
            class="inline-block mt-5 bg-gradient-to-r from-blue-600 to-indigo-600 text-white px-6 py-3 rounded-lg font-medium hover:from-blue-700 hover:to-indigo-700 transition-all duration-200 shadow-md hover:shadow-lg transform hover:-translate-y-0.5">
            <div class="flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
              </svg>
              {{ $t('cart.goToCatalog') }}
            </div>
          </router-link>
        </div>
      </template>

      <!-- Контент, когда данные загружены -->
      <template #default>
        <!-- Заголовок -->
        <div class="mb-5 flex justify-between items-center">
          <h3 class="text-xl font-bold text-gray-800">{{ $t('profile.myReviews') }}</h3>
          <div class="text-sm text-gray-500">{{ $t('profile.totalReviews', { count: reviews.length }) }}</div>
        </div>

        <!-- Список отзывов -->
        <div class="space-y-6">
          <ReviewItem v-for="review in paginatedReviews" :key="review.id" :review="review"
            :product-name="productNames[review.productId]" @delete="handleReviewDeleted" />
        </div>

        <!-- Пагинация -->
        <BasePagination v-if="reviews.length > 0" :total-items="reviews.length" :current-page="currentPage"
          :page-size="pageSize" :page-sizes="[5, 10, 20]" @pagination-change="handlePaginationChange" />
      </template>
    </StateWrapper>

    <!-- Компонент уведомлений -->
    <Notification ref="toast" />
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import reviewService from '@/services/api/reviewService';
import productService from '@/services/api/productService';
import ReviewItem from '@/components/review/ReviewItem.vue';
import ReviewManager from '@/components/review/ReviewManager.vue'; // Импорт ReviewManager
import BasePagination from '@/components/ui/BasePagination.vue';
import StateWrapper from '@/components/ui/StateWrapper.vue';
import Notification from '@/components/ui/Notification.vue';
import useApiRequest from '@/hooks/useApiRequest';

export default {
  name: 'ProfileComments',
  components: {
    ReviewItem,
    BasePagination,
    StateWrapper,
    Notification,
    ReviewManager // Добавляем компонент в список компонентов
  },
  setup() {
    const { t } = useI18n();

    // Состояние
    const reviews = ref([]);
    const productNames = ref({});
    const toast = ref(null);

    // Пагинация
    const pageSize = ref(5);
    const currentPage = ref(1);
    const paginatedReviews = computed(() => {
      const startIndex = (currentPage.value - 1) * pageSize.value;
      return reviews.value.slice(startIndex, startIndex + pageSize.value);
    });

    // API хук для загрузки отзывов
    const { loading, error, execute: executeReviews } = useApiRequest();

    // Загрузка отзывов пользователя
    const fetchReviews = async () => {
      await executeReviews(async () => {
        return await reviewService.getMyReview();
      }, {
        onSuccess: async (data) => {
          reviews.value = data || [];
          // Загружаем названия продуктов для отзывов, если есть отзывы
          if (reviews.value.length > 0) {
            await enrichReviewsWithProductNames();
          }
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Получение названий продуктов для отзывов с оптимизацией запросов
    const enrichReviewsWithProductNames = async () => {
      if (reviews.value.length === 0) return;

      const productIds = [...new Set(reviews.value.map(review => review.productId))];
      const batchSize = 5; // Размер партии запросов для параллельной обработки

      const productIdBatches = productIds.reduce((batches, id, i) => {
        const batchIndex = Math.floor(i / batchSize);
        if (!batches[batchIndex]) batches[batchIndex] = [];
        batches[batchIndex].push(id);
        return batches;
      }, []);

      for (const batch of productIdBatches) {
        await Promise.all(batch.map(async (productId) => {
          const response = await productService.getProductById(productId);
          if (response.status === 'success' && response.data) {
            productNames.value[productId] = response.data.name;
          }
        }));
      }
    };

    // Обработчик изменения пагинации
    const handlePaginationChange = (event) => {
      currentPage.value = event.page;
      pageSize.value = event.pageSize;
    };

    // Заменяем confirmDeleteReview на handleReviewDeleted
    const handleReviewDeleted = (reviewId) => {
      toast.value?.success(t('profile.reviewDeleted'));
      reviews.value = reviews.value.filter(review => review.id !== reviewId);

      // Если после удаления текущая страница пуста, переходим на предыдущую
      const totalPages = Math.ceil(reviews.value.length / pageSize.value);
      if (currentPage.value > totalPages && totalPages > 0) {
        currentPage.value = totalPages;
      }
    };

    // Добавляем обработчик добавления нового отзыва
    const onReviewAdded = async (review) => {
      // Обновляем список отзывов
      await fetchReviews();
      // Показываем уведомление об успешном добавлении отзыва
      toast.value?.success(t('profile.reviewManager.reviewAdded'));
    };

    // Загрузка данных при монтировании компонента
    onMounted(() => {
      fetchReviews();
    });

    return {
      reviews,
      loading,
      error,
      productNames,
      pageSize,
      currentPage,
      paginatedReviews,
      toast,
      handlePaginationChange,
      handleReviewDeleted, // Заменяем confirmDeleteReview на handleReviewDeleted
      onReviewAdded // Добавляем функцию в возвращаемый объект
    };
  }
};
</script>

<style scoped>
.space-y-6>*+* {
  margin-top: 1.5rem;
}
</style>
