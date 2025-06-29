<template>
  <div class="relative group">
    <input type="text"
      class="w-full py-3 pl-12 pr-10 text-gray-700 bg-gray-50 border border-gray-300 rounded-lg focus:outline-none focus:bg-white focus:border-indigo-500 focus:ring-2 focus:ring-indigo-200 text-base transition-all duration-300 hover:border-indigo-300 group-hover:shadow-sm"
      :placeholder="$t('navbar.searchPlaceholder')" v-model="search" @keyup.enter="goToCatalog"
      @keydown.down.prevent="navigateDropdown(1)" @keydown.up.prevent="navigateDropdown(-1)"
      @keydown.esc="closeDropdown" @input="onSearchInput" @focus="isInputFocused = true" @blur="handleBlur"
      autocomplete="off" aria-label="Поиск товаров" />

    <!-- Иконка поиска -->
    <div class="absolute inset-y-0 left-0 flex items-center pl-4 transition-colors duration-300"
      :class="{ 'text-gray-500': !isInputFocused, 'text-indigo-600': isInputFocused }">
      <svg class="w-6 h-6" viewBox="0 0 24 24" fill="none">
        <path
          d="M21 21L15 15M17 10C17 13.866 13.866 17 10 17C6.13401 17 3 13.866 3 10C3 6.13401 6.13401 3 10 3C13.866 3 17 6.13401 17 10Z"
          stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
      </svg>
    </div>

    <!-- Индикатор загрузки (появляется при поиске) -->
    <div v-if="isLoading" class="absolute inset-y-0 right-0 flex items-center pr-3">
      <div class="animate-spin rounded-full h-5 w-5 border-t-2 border-b-2 border-indigo-500"></div>
    </div>

    <!-- Кнопка поиска -->
    <button v-else
      class="absolute inset-y-0 right-0 flex items-center pr-3 transition-opacity duration-300 text-gray-500 hover:text-indigo-600 focus:outline-none"
      @click="goToCatalog" type="button" aria-label="Выполнить поиск"
      :class="{ 'opacity-100': search.length > 0, 'opacity-0': search.length === 0 }">
      <span v-if="search.length > 0" class="flex items-center justify-center w-8 h-8 rounded-full hover:bg-indigo-50">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 5l7 7m0 0l-7 7m7-7H3" />
        </svg>
      </span>
    </button>

    <!-- Выпадающий список с результатами поиска -->
    <div v-if="showDropdown && searchResults.length > 0"
      class="absolute z-50 w-full bg-white mt-1 rounded-lg shadow-lg border border-gray-200 max-h-80 overflow-y-auto"
      ref="dropdownRef">
      <ul class="py-1">
        <li v-for="(product, index) in searchResults" :key="product.id" @click="goToProduct(product.id)"
          @mouseenter="activeIndex = index"
          class="flex items-center px-4 py-3 cursor-pointer transition-colors hover:bg-indigo-50"
          :class="{ 'bg-indigo-50': activeIndex === index }">
          <!-- Изображение товара (если есть) -->
          <div class="h-10 w-10 flex-shrink-0 mr-3 rounded border border-gray-200 overflow-hidden">
            <img v-if="product.images && product.images.length > 0" :src="product.images[0].url" :alt="product.name"
              class="h-full w-full object-contain">
            <div v-else class="h-full w-full bg-gray-100 flex items-center justify-center text-gray-400">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
            </div>
          </div>

          <!-- Информация о товаре -->
          <div class="flex-1 min-w-0">
            <!-- Название товара с подсветкой совпадения -->
            <p class="text-sm font-medium text-gray-900 truncate" v-html="highlightMatch(product.name)"></p>
            <!-- Цена -->
            <p class="text-sm text-indigo-600 font-semibold">{{ product.price.toLocaleString() }} ₽</p>
          </div>
        </li>
      </ul>

      <!-- Показать все результаты -->
      <div
        class="border-t border-gray-100 px-4 py-2 bg-gray-50 text-center cursor-pointer hover:bg-indigo-50 transition-colors"
        @click="goToCatalog">
        <span class="text-sm font-medium text-indigo-600">{{ $t('navbar.viewAllResults') }} ({{ totalResults }})</span>
      </div>
    </div>

    <!-- Сообщение "Нет результатов" -->
    <div v-else-if="search.length >= 2 && !isLoading && searchAttempted && searchResults.length === 0"
      class="absolute z-50 w-full bg-white mt-1 rounded-lg shadow-lg border border-gray-200 p-4 text-center">
      <p class="text-sm text-gray-500">{{ $t('navbar.noResults') }}</p>
    </div>

    <div
      class="absolute right-3 top-full mt-2 bg-white text-xs text-gray-500 py-1 px-2 rounded shadow-md opacity-0 transition-opacity duration-300 pointer-events-none"
      :class="{ 'opacity-80': isInputFocused && !isMobile && !showDropdown }">
      <span class="flex items-center">
        {{ $t('navbar.searchHint') }}
        <kbd
          class="ml-1 px-1.5 py-0.5 bg-gray-100 border border-gray-300 rounded text-gray-800 font-semibold text-xs">Enter</kbd>
      </span>
    </div>
  </div>
</template>

<script>
import productService from '@/services/api/productService';
import { debounce } from 'lodash';

export default {
  name: 'NavbarSearch',
  data() {
    return {
      search: '',
      isInputFocused: false,
      isMobile: false,
      isLoading: false,
      searchResults: [],
      totalResults: 0,
      showDropdown: false,
      activeIndex: -1,
      searchAttempted: false
    };
  },
  watch: {
    '$route.query.search': {
      immediate: true,
      handler(newVal) {
        this.search = newVal || '';
      }
    },
    '$route'() {
      this.closeDropdown();
    }
  },
  created() {
    this.debouncedSearch = debounce(this.fetchSearchResults, 300);
  },
  mounted() {
    this.checkIfMobile();
    window.addEventListener('resize', this.checkIfMobile);

    document.addEventListener('keydown', this.handleHotkeys);

    document.addEventListener('mousedown', this.handleClickOutside);
  },
  beforeUnmount() {
    window.removeEventListener('resize', this.checkIfMobile);
    document.removeEventListener('keydown', this.handleHotkeys);
    document.removeEventListener('mousedown', this.handleClickOutside);
  },
  methods: {
    onSearchInput() {
      if (!this.search || this.search.length < 2) {
        this.searchResults = [];
        this.showDropdown = false;
        return;
      }

      this.debouncedSearch(this.search);
    },

    async fetchSearchResults(query) {
      try {
        this.isLoading = true;
        this.searchAttempted = true;

        const response = await productService.getProducts(0, query, null, null, null, 4);
        console.log(response);
        if (response.status === 'success' && response.data) {
          this.searchResults = response.data.products || [];
          this.totalResults = response.data.totalItems || 0;
          this.showDropdown = true;
          this.activeIndex = -1;
        } else {
          this.searchResults = [];
          this.totalResults = 0;
        }
      } catch (error) {
        console.error('Ошибка при поиске:', error);
        this.searchResults = [];
        this.totalResults = 0;
      } finally {
        this.isLoading = false;
      }
    },

    navigateDropdown(direction) {
      if (!this.showDropdown || this.searchResults.length === 0) return;

      if (direction > 0) {
        this.activeIndex = this.activeIndex < this.searchResults.length - 1
          ? this.activeIndex + 1
          : 0;
      } else {
        this.activeIndex = this.activeIndex > 0
          ? this.activeIndex - 1
          : this.searchResults.length - 1;
      }

      document.addEventListener('keydown', this.handleEnterKeypress, { once: true });
    },

    handleEnterKeypress(event) {
      if (event.key === 'Enter' && this.activeIndex >= 0 && this.searchResults[this.activeIndex]) {
        this.goToProduct(this.searchResults[this.activeIndex].id);
        event.preventDefault();
      }
    },

    closeDropdown() {
      this.showDropdown = false;
      document.removeEventListener('keydown', this.handleEnterKeypress);
    },

    handleBlur() {
      this.isInputFocused = false;

      setTimeout(() => {
        if (!this.isDropdownClicked) {
          this.closeDropdown();
        }
      }, 150);
    },

    goToCatalog() {
      if (this.search && this.search.trim().length > 0) {
        this.$router.push({ path: '/catalog', query: { search: this.search.trim() } });
      } else {
        this.$router.push({ path: '/catalog' });
      }
      this.closeDropdown();
    },

    goToProduct(productId) {
      this.$router.push({ path: `/product/${productId}` });
      this.closeDropdown();
    },

    handleClickOutside(event) {
      if (this.$refs.dropdownRef && !this.$refs.dropdownRef.contains(event.target) &&
        !this.$el.contains(event.target)) {
        this.closeDropdown();
      }
    },

    checkIfMobile() {
      this.isMobile = window.innerWidth < 768;
    },

    handleHotkeys(event) {
      if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
        event.preventDefault();
        const searchInput = this.$el.querySelector('input');
        if (searchInput) {
          searchInput.focus();
        }
      }
    },

    highlightMatch(text) {
      if (!this.search || !text) return text;

      const regex = new RegExp(`(${this.search.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&')})`, 'gi');
      return text.replace(regex, '<span class="highlight">$1</span>');
    }
  }
};
</script>

<style scoped>
:deep(.highlight) {
  background-color: rgba(79, 70, 229, 0.1);
  color: #4f46e5;
  border-radius: 2px;
  padding: 0 2px;
  font-weight: 600;
}

button:hover svg {
  transform: translateX(2px);
  transition: transform 0.2s ease-in-out;
}

input:focus-visible {
  outline: 2px solid #4f46e5;
  outline-offset: 2px;
}

@media (max-width: 768px) {
  input {
    font-size: 16px;
    padding-top: 0.7rem;
    padding-bottom: 0.7rem;
  }
}
</style>
