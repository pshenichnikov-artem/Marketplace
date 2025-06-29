<template>
  <div class="flex flex-col min-h-screen bg-gray-50">
    <MainNavbar />
    <main class="flex-grow container mx-auto px-4 py-8">
      <ProductFilters :value="filters" @apply="applyFilters" />

      <StateWrapper :loading="loading" :error="error" :is-empty="!products || products.length === 0"
        empty-message="Товары не найдены">
        <!-- Продукты -->
        <template #default>
          <div
            class="grid grid-cols-2 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 sm:gap-6 product-grid">
            <ProductCard v-for="product in products" :key="product.id" :product="product" :size="cardSize" />
          </div>

          <!-- Пагинация -->
          <BasePagination v-if="totalItems > pageSize" ref="pagination" :total-items="totalItems"
            :page-sizes="[20, 50, 100, 150]" :current-page="currentPage" :page-size="pageSize"
            @pagination-change="onPaginationChange" class="mt-8" />
        </template>

        <!-- Кастомизация пустого состояния -->
        <template #empty>
          <div
            class="text-center py-12 bg-gradient-to-r from-blue-50 to-indigo-50 rounded-xl shadow-sm border border-blue-100">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 mx-auto mb-4 text-blue-400 opacity-80" fill="none"
              viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            <p class="text-xl font-medium text-indigo-700 mb-3">{{ $t('catalog.noProductsFound') }}</p>
            <p class="text-blue-600 max-w-md mx-auto mb-6">{{ $t('catalog.tryChangingFilters') }}</p>
            <button @click="resetFilters"
              class="bg-indigo-600 hover:bg-indigo-700 text-white px-6 py-3 rounded-lg font-medium transition-colors shadow-md inline-flex items-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              {{ $t('catalog.resetFilters') }}
            </button>
          </div>
        </template>
      </StateWrapper>
    </main>
    <AppFooter />

    <!-- Компонент уведомлений -->
    <Notification ref="toast" />
  </div>
</template>

<script>
import { ref, onMounted, computed, watch } from 'vue';
import MainNavbar from '@/components/layout/MainNavbar.vue';
import AppFooter from '@/components/layout/AppFooter.vue';
import ProductCard from '@/components/product/ProductCard.vue';
import BasePagination from "@/components/ui/BasePagination.vue";
import ProductFilters from "@/components/ui/filters/ProductFilters.vue";
import StateWrapper from '@/components/ui/StateWrapper.vue';
import Notification from '@/components/ui/Notification.vue';
import productService from "@/services/api/productService";
import useApiRequest from '@/hooks/useApiRequest';

export default {
  name: 'CatalogPage',
  components: {
    MainNavbar,
    AppFooter,
    ProductCard,
    ProductFilters,
    BasePagination,
    StateWrapper,
    Notification
  },
  setup() {
    const products = ref([]);
    const totalItems = ref(0);
    const filters = ref({
      category: null,
      sortBy: null,
      minPrice: null,
      maxPrice: null,
      search: null
    });
    const currentPage = ref(1);
    const pageSize = ref(20);
    const windowWidth = ref(window.innerWidth);
    const pagination = ref(null);
    const toast = ref(null);

    // API хук
    const { loading, error, execute } = useApiRequest();

    // Динамически определяем размер карточки
    const cardSize = computed(() => {
      if (windowWidth.value < 450) {
        return 'tiny';
      }
      else if (windowWidth.value < 600) {
        return 'small';
      } else if (windowWidth.value < 768) {
        return 'medium';
      } else {
        return 'large';
      }
    });

    // Загрузка продуктов
    const fetchProducts = async () => {
      await execute(async () => {
        const params = {};
        if (currentPage.value > 1) params.page = currentPage.value;
        if (filters.value.search) params.search = filters.value.search;
        if (filters.value.sortBy) params.orderBy = filters.value.sortBy;
        if (filters.value.category) params.categoryId = filters.value.category;
        if (filters.value.minPrice) params.minPrice = filters.value.minPrice;
        if (filters.value.maxPrice) params.maxPrice = filters.value.maxPrice;
        if (pageSize.value !== 20) params.pageSize = pageSize.value;

        return await productService.getProducts(
          currentPage.value,
          filters.value.search || undefined,
          filters.value.sortBy || undefined,
          filters.value.category || undefined,
          null, // sellerId
          pageSize.value,
          filters.value.minPrice || undefined,
          filters.value.maxPrice || undefined
        );
      }, {
        onSuccess: (data) => {
          products.value = data.products;
          totalItems.value = data.totalCount;
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Инициализация из URL
    const initFromUrl = () => {
      const query = window.location.search
        ? Object.fromEntries(new URLSearchParams(window.location.search))
        : {};

      filters.value = {
        category: parseCategory(query.category),
        sortBy: query.sortBy || null,
        minPrice: parseNumber(query.minPrice),
        maxPrice: parseNumber(query.maxPrice),
        search: query.search || null
      };

      currentPage.value = parseNumber(query.page) || 1;
      pageSize.value = parseNumber(query.pageSize) || 20;

      fetchProducts();
    };

    // Вспомогательные функции для парсинга параметров
    const parseCategory = (value) => {
      if (!value) return null;
      if (value === 'null') return null;
      const parsed = parseInt(value);
      return isNaN(parsed) ? null : parsed;
    };

    const parseNumber = (value) => {
      if (!value) return null;
      const parsed = parseFloat(value);
      return isNaN(parsed) ? null : parsed;
    };

    // Обработчик изменения размера окна
    const handleResize = () => {
      windowWidth.value = window.innerWidth;
    };

    // Применение фильтров
    const applyFilters = (newFilters) => {
      filters.value = { ...newFilters };
      currentPage.value = 1;

      updateUrl();
    };

    // Сброс фильтров
    const resetFilters = () => {
      filters.value = {
        category: null,
        sortBy: null,
        minPrice: null,
        maxPrice: null,
        search: null
      };
      currentPage.value = 1;

      updateUrl();
    };

    // Обработка пагинации
    const onPaginationChange = ({ page, pageSize: newPageSize }) => {
      currentPage.value = page;
      pageSize.value = newPageSize;

      updateUrl();
    };

    // Обновление URL
    const updateUrl = () => {
      const query = {};

      if (filters.value.category !== null && filters.value.category !== undefined) {
        query.category = filters.value.category.toString();
      }
      if (filters.value.sortBy) query.sortBy = filters.value.sortBy;
      if (filters.value.minPrice) query.minPrice = filters.value.minPrice;
      if (filters.value.maxPrice) query.maxPrice = filters.value.maxPrice;
      if (filters.value.search) query.search = filters.value.search;
      if (currentPage.value > 1) query.page = currentPage.value.toString();
      if (pageSize.value !== 20) query.pageSize = pageSize.value.toString();

      const queryString = new URLSearchParams(query).toString();
      const newUrl = queryString ? `?${queryString}` : window.location.pathname;

      // Обновляем URL без перезагрузки страницы
      window.history.pushState(
        { path: newUrl },
        '',
        newUrl
      );

      // Загружаем продукты с новыми параметрами
      fetchProducts();
    };

    // Наблюдатели и хуки жизненного цикла
    onMounted(() => {
      window.addEventListener('resize', handleResize);
      initFromUrl();
    });

    watch(() => window.location.search, () => {
      initFromUrl();
    });

    return {
      products,
      totalItems,
      filters,
      currentPage,
      pageSize,
      windowWidth,
      cardSize,
      loading,
      error,
      pagination,
      toast,
      fetchProducts,
      applyFilters,
      resetFilters,
      onPaginationChange,
      handleResize
    };
  },
  beforeUnmount() {
    window.removeEventListener('resize', this.handleResize);
  }
};
</script>

<style scoped>
/* Обеспечиваем одинаковое расстояние между карточками на всех размерах экрана */
.grid {
  min-width: 0;
  /* Важно для предотвращения переполнения карточек */
}

/* Предотвращаем наложение карточек друг на друга */
.grid>* {
  min-width: 0;
  max-width: 100%;
}

/* Уменьшаем отступы на маленьких экранах для максимального использования пространства */
@media (max-width: 480px) {
  .grid {
    gap: 0.75rem;
    /* 12px вместо 1.5rem (24px) */
  }
}

/* Добавляем горизонтальное центрирование карточек */
@media (min-width: 768px) and (max-width: 1023px) {
  .grid {
    justify-content: center;
  }

  main {
    max-width: 95%;
  }
}

/* Улучшаем контейнер для карточек продуктов */
.product-grid {
  display: grid;
  justify-content: center;
  justify-items: center;
}

/* Гарантируем, что карточки не будут растягиваться даже в контейнере с flex-grow */
.product-grid>* {
  width: max-content !important;
}

@media (max-width: 639px) {

  /* На очень маленьких экранах уменьшаем отступы еще больше */
  .product-grid {
    column-gap: 0.5rem;
    row-gap: 1rem;
    padding: 0 0.25rem;
  }
}
</style>
