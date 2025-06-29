<template>
  <div
    class="bg-white rounded-xl p-5 shadow-md mb-8 border border-gray-100 transition-all duration-300 hover:shadow-lg">
    <div class="flex flex-col sm:flex-row flex-wrap gap-4 sm:items-end">
      <!-- Заголовок фильтров на мобильных устройствах -->
      <div class="w-full mb-2 sm:hidden">
        <h2 class="text-lg font-semibold text-gray-800 flex items-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-600" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
          </svg>
          {{ $t('productFilter.filterTitle') }}
        </h2>
      </div>

      <!-- Категория с использованием FilterFieldWithDropdown -->
      <div class="w-full sm:w-44 relative group">
        <FilterFieldWithDropdown :label="$t('productFilter.category')" icon="category" v-model="filters.category"
          :options="categoryOptions" :placeholder="$t('productFilter.all')"
          :no-results-text="$t('productFilter.noResults')" @select="onCategorySelect" />
      </div>

      <!-- Сортировка -->
      <div class="w-full sm:w-44 relative group">
        <BaseFilterField :label="$t('productFilter.sortBy')" type="select" :options="sortOptions"
          v-model="filters.sortBy" icon="sort" />
      </div>

      <!-- Цена от -->
      <div class="w-full sm:w-32 relative group">
        <BaseFilterField :label="$t('productFilter.minPrice')" type="number" v-model="filters.minPrice" icon="minPrice"
          :placeholder="$t('productFilter.minPricePlaceholder')" />
      </div>

      <!-- Цена до -->
      <div class="w-full sm:w-32 relative group">
        <BaseFilterField :label="$t('productFilter.maxPrice')" type="number" v-model="filters.maxPrice" icon="maxPrice"
          :placeholder="$t('productFilter.maxPricePlaceholder')" />
      </div>

      <!-- Кнопка сброса фильтров -->
      <div class="w-full sm:w-auto relative mt-2 sm:mt-0 sm:ml-auto">
        <button @click="resetFilters"
          class="flex items-center text-sm text-gray-500 hover:text-indigo-600 transition-colors px-3 py-2.5 rounded-lg hover:bg-indigo-50"
          :class="{ 'opacity-0 pointer-events-none': !hasActiveFilters, 'opacity-100': hasActiveFilters }">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
          {{ $t('productFilter.reset') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import BaseFilterField from './BaseFilterField.vue';
import FilterFieldWithDropdown from './FilterFieldWithDropdown.vue';
import categoryService from '@/services/api/categoryService';
import { prepareCategoryOptions } from '@/utils/categoryUtils';

export default {
  components: { BaseFilterField, FilterFieldWithDropdown },
  props: {
    value: {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      filters: {
        category: null,
        sortBy: null,
        minPrice: null,
        maxPrice: null
      },
      categories: [],
      suppressWatch: false
    };
  },
  computed: {
    sortOptions() {
      return [
        { label: this.$t('productFilter.popularity'), value: null },
        { label: this.$t('productFilter.priceDesc'), value: 'price_desc' },
        { label: this.$t('productFilter.priceAsc'), value: 'price_asc' },
        { label: this.$t('productFilter.name'), value: 'name' },
        { label: this.$t('productFilter.ratingHigh'), value: 'rating_high' },
        { label: this.$t('productFilter.ratingLow'), value: 'rating_low' },
        { label: this.$t('productFilter.newest'), value: 'new' },
        { label: this.$t('productFilter.oldest'), value: 'old' },
        { label: this.$t('productFilter.reviewsHigh'), value: 'reviews_high' },
        { label: this.$t('productFilter.reviewsLow'), value: 'reviews_low' }
      ];
    },
    selectedCategoryLabel() {
      if (!this.filters.category) return this.$t('productFilter.all');
      const findLabel = (opts) => {
        for (const opt of opts) {
          if (opt.id === this.filters.category) return opt.label;
          if (opt.children && opt.children.length) {
            const res = findLabel(opt.children);
            if (res) return res;
          }
        }
        return '';
      };
      return findLabel(this.categoryOptions) || '';
    },
    categoryOptions() {
      return prepareCategoryOptions(
        this.categories,
        this.$t('productFilter.all')
      );
    },
    hasActiveFilters() {
      return this.filters.category !== null ||
        this.filters.sortBy !== null ||
        this.filters.minPrice !== null ||
        this.filters.maxPrice !== null;
    }
  },
  watch: {
    filters: {
      handler(newFilters) {
        // Фильтруем пустые значения перед отправкой
        if (this.suppressWatch) return;

        const filteredFilters = {};
        Object.entries(newFilters).forEach(([key, value]) => {
          // Строго проверяем, что значение не null, не undefined и не пустая строка
          if (value !== null && value !== undefined && value !== '') {
            // Проверяем, что значение не является объектом
            if (typeof value !== 'object') {
              filteredFilters[key] = value;
            }
          }
        });
        this.$emit('apply', filteredFilters);
      },
      deep: true
    }
  },
  async mounted() {
    console.log("ProductFilters mounted");
    // Получаем категории через categoryService
    const response = await categoryService.getAll();
    console.log("Category response:", response);
    if (response.status === 'success' && Array.isArray(response.data)) {
      this.categories = response.data;

      // После получения категорий, инициализируем фильтры из пропса value
      if (this.value) {
        console.log("Initializing filters from value prop after categories loaded:", this.value);
        this.suppressWatch = true;
        this.filters.category = this.value.category !== undefined ? this.value.category : this.filters.category;
        this.filters.sortBy = this.value.sortBy !== undefined ? this.value.sortBy : this.filters.sortBy;
        this.filters.minPrice = this.value.minPrice !== undefined ? this.value.minPrice : this.filters.minPrice;
        this.filters.maxPrice = this.value.maxPrice !== undefined ? this.value.maxPrice : this.filters.maxPrice;
        this.$nextTick(() => {
          this.suppressWatch = false;
        });
      }
    } else {
      this.categories = [];
    }
  },
  methods: {
    onCategorySelect(option) {
      console.log("Selected category:", option);
      this.filters.category = option.value || option.id || null;
    },
    resetFilters() {
      this.suppressWatch = true;
      this.filters = {
        category: null,
        sortBy: null,
        minPrice: null,
        maxPrice: null
      };
      this.$nextTick(() => {
        this.suppressWatch = false;
        this.$emit('apply', {});
      });
    }
  }
};
</script>

<style scoped>
.group:hover label {
  color: #4f46e5;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }

  to {
    opacity: 1;
  }
}

.opacity-100 {
  animation: fadeIn 0.3s ease-in-out;
}
</style>
