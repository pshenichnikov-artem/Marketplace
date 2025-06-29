<template>
  <div class="flex flex-col min-h-screen bg-gray-50">
    <MainNavbar />
    <main class="flex-grow container mx-auto px-4 py-8">
      <!-- Используем StateWrapper для управления состояниями -->
      <StateWrapper :loading="loading" :error="error" :is-empty="!product">
        <!-- Информация о товаре -->
        <template #default>
          <div class="bg-white rounded-xl shadow p-6 mb-8">
            <div class="flex flex-col md:flex-row">
              <!-- Левая колонка с фотографиями -->
              <div class="w-full md:w-2/5 mb-6 md:mb-0">
                <!-- Основная фотография -->
                <div
                  class="relative w-full h-96 rounded-xl overflow-hidden mb-6 border border-gray-200 shadow-md hover:shadow-lg transition-all duration-300 group">
                  <img :src="selectedImage().url" :alt="product.name"
                    class="w-full h-full object-contain p-2 group-hover:scale-105 transition-transform duration-300" />
                </div>

                <!-- Галерея миниатюр -->
                <div class="flex flex-wrap gap-3 justify-center">
                  <div v-for="(image, index) in product.images" :key="index"
                    class="w-20 h-20 rounded-lg overflow-hidden border-2 cursor-pointer hover:border-indigo-500 transition-all duration-200 hover:shadow-md hover:scale-105"
                    :class="selectedImageIndex === index ? 'border-indigo-500 ring-2 ring-indigo-300 shadow-md scale-105' : 'border-gray-200'"
                    @click="selectImage(index)">
                    <img :src="image.url" :alt="`${product.name} ${index + 1}`" class="w-full h-full object-cover" />
                  </div>
                </div>
              </div>

              <!-- Средняя колонка с информацией -->
              <div class="w-full md:w-2/5 md:px-8">
                <!-- Название товара с декоративным элементом -->
                <h1 class="text-3xl font-bold text-gray-900 mb-5 relative pb-3 border-b border-gray-200">
                  {{ product.name }}
                  <span class="absolute bottom-0 left-0 w-24 h-1 bg-indigo-600 rounded-full"></span>
                </h1>

                <!-- Категория товара -->
                <div class="mb-4 flex items-center">
                  <span class="px-3 py-1 bg-indigo-50 text-indigo-700 rounded-full text-sm font-medium">
                    {{ categoryName || $t('product.noCategory') }}
                  </span>
                </div>

                <!-- Рейтинг в улучшенном дизайне -->
                <div class="flex items-center bg-gray-50 p-3 rounded-lg mb-6 shadow-sm">
                  <div class="flex items-center">
                    <StarRating :rating="product.rating" :size="24" :show-value="true" />
                  </div>
                  <span class="text-gray-600 ml-3 font-medium">({{ product.reviewCount }} {{ $t('product.reviews')
                    }})</span>
                </div>

                <!-- Информационные блоки -->
                <div class="space-y-6">
                  <!-- Описание -->
                  <div
                    class="p-4 bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md transition-shadow duration-300">
                    <h2 class="text-lg font-semibold text-indigo-700 mb-2 flex items-center">
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      {{ $t('product.description') }}
                    </h2>
                    <p class="text-gray-700 leading-relaxed">{{ product.description }}</p>
                  </div>

                  <!-- Продавец -->
                  <div
                    class="p-4 bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md transition-shadow duration-300">
                    <h2 class="text-lg font-semibold text-indigo-700 mb-2 flex items-center">
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                      </svg>
                      {{ $t('product.seller') }}
                    </h2>
                    <div class="flex items-center">
                      <span class="text-gray-700 font-medium">{{ product.seller.firstName }} {{ product.seller.lastName
                        }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Правая колонка с ценой и кнопкой -->
              <div
                class="w-full md:w-1/5 bg-white p-6 rounded-xl shadow-lg border border-gray-200 transition-all duration-300 hover:shadow-xl">
                <!-- Блок с ценой -->
                <div class="mb-6 bg-gray-50 rounded-lg p-4 border border-gray-100">
                  <div class="text-3xl font-bold text-indigo-600 flex items-center gap-2">
                    {{ product.price.toLocaleString() }} ₽
                  </div>
                </div>

                <!-- Информация о наличии -->
                <div class="mb-6 flex items-center">
                  <div class="h-2.5 w-2.5 rounded-full bg-green-500 mr-2"></div>
                  <span class="text-sm font-medium text-gray-700">
                    {{ $t('product.inStock') }}: <strong>{{ product.stockQuantity }} шт.</strong>
                  </span>
                </div>

                <div class="w-full">
                  <!-- StateWrapper для операций с корзиной -->
                  <StateWrapper :loading="cartLoading">
                    <!-- Кнопка "В корзину", если товара нет в корзине -->
                    <template #default>
                      <button v-if="!cartItem"
                        class="w-full bg-indigo-600 text-white py-3.5 px-4 rounded-lg font-semibold hover:bg-indigo-700 transition-all duration-300 flex items-center justify-center shadow-md hover:shadow-lg active:scale-98 focus:ring-2 focus:ring-indigo-300"
                        @click="addToCart">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                          stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                        </svg>
                        <span class="text-base">{{ $t('product.addToCart') }}</span>
                      </button>

                      <!-- Элементы управления количеством, если товар есть в корзине -->
                      <div v-else class="flex flex-col space-y-4">
                        <!-- Блок с управлением количеством -->
                        <div
                          class="flex items-center justify-between bg-white border border-gray-200 rounded-lg shadow-sm p-3 hover:shadow transition-all duration-300">
                          <button
                            class="bg-indigo-600 text-white w-10 h-10 rounded-md flex items-center justify-center hover:bg-indigo-700 transition-all duration-300 shadow-sm hover:shadow focus:ring-2 focus:ring-indigo-300 active:scale-95"
                            @click="updateCartItemQuantity(-1)" :disabled="cartLoading">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20"
                              fill="currentColor">
                              <path fill-rule="evenodd" d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
                                clip-rule="evenodd" />
                            </svg>
                          </button>

                          <div
                            class="font-semibold text-gray-900 text-center text-2xl min-w-[50px] px-2 py-1 rounded-md bg-gray-50">
                            {{ cartItem ? cartItem.quantity : 0 }}
                          </div>

                          <button
                            class="bg-indigo-600 text-white w-10 h-10 rounded-md flex items-center justify-center hover:bg-indigo-700 transition-all duration-300 shadow-sm hover:shadow focus:ring-2 focus:ring-indigo-300 active:scale-95"
                            @click="updateCartItemQuantity(1)"
                            :disabled="cartLoading || (product && cartItem && cartItem.quantity >= product.stockQuantity)">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20"
                              fill="currentColor">
                              <path fill-rule="evenodd"
                                d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
                                clip-rule="evenodd" />
                            </svg>
                          </button>
                        </div>

                        <!-- Кнопки действий -->
                        <div class="grid grid-cols-1 gap-3 mt-2">
                          <router-link to="/cart"
                            class="w-full text-center bg-indigo-600 text-white py-3 px-4 rounded-lg hover:bg-indigo-700 transition-all duration-300 flex items-center justify-center shadow-md hover:shadow-lg active:scale-98">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                              stroke="currentColor">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                            </svg>
                            {{ $t('product.goToCart') }}
                          </router-link>

                          <button
                            class="w-full bg-white border border-red-500 text-red-500 py-2.5 px-4 rounded-lg hover:bg-red-50 transition-colors flex items-center justify-center active:scale-98"
                            @click="deleteCartItem" :disabled="cartLoading">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                              stroke="currentColor">
                              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                            </svg>
                            {{ $t('product.removeFromCart') }}
                          </button>
                        </div>
                      </div>
                    </template>
                  </StateWrapper>
                </div>
              </div>
            </div>
          </div>

          <!-- Секция отзывов с использованием вынесенного компонента -->
          <ProductReviews :product-id="product.id" />
        </template>
      </StateWrapper>
    </main>
    <AppFooter />

    <Notification ref="toast" />
  </div>
</template>

<script>
import MainNavbar from '@/components/layout/MainNavbar.vue';
import AppFooter from '@/components/layout/AppFooter.vue';
import StarRating from '@/components/ui/StarRating.vue';
import productService from '@/services/api/productService';
import categoryService from '@/services/api/categoryService';
import cartService from "@/services/api/cartService.js";
import ProductReviews from '@/components/product/ProductReviews.vue';
import StateWrapper from '@/components/ui/StateWrapper.vue';
import Notification from '@/components/ui/Notification.vue';
import useApiRequest from '@/hooks/useApiRequest';
import { ref, watch } from 'vue';
import { useCartStore } from '@/stores/modules/cart';

export default {
  name: 'ProductPage',
  components: {
    MainNavbar,
    AppFooter,
    StarRating,
    StateWrapper,
    Notification,
    ProductReviews
  },
  setup() {
    const product = ref(null);
    const categoryName = ref('');
    const selectedImageIndex = ref(0);
    const cart = ref(null);
    const cartItem = ref(null);
    const toast = ref(null);
    const cartStore = useCartStore();
    const fallbackImageUrl = ref('https://localhost:7037/images/no-image-placeholder.png'); // URL заглушки

    // API хуки для разных операций
    const { loading, error, execute: executeProductLoad } = useApiRequest();
    const { loading: cartLoading, execute: executeCartOperation } = useApiRequest();

    // Загрузка данных о продукте
    const fetchProductData = async (productId) => {
      await executeProductLoad(async () => {
        return await productService.getProductById(productId);
      }, {
        onSuccess: async (data) => {
          product.value = data;

          // Устанавливаем изображение по умолчанию
          selectedImageIndex.value = product.value.images.findIndex(img => img.isPrimary);
          if (selectedImageIndex.value === -1 && product.value.images.length > 0) {
            selectedImageIndex.value = 0;
          }

          // Загружаем категорию
          if (product.value.categoryId) {
            await fetchCategory(product.value.categoryId);
          }

          // Проверяем, есть ли этот товар в корзине, если корзина уже загружена
          if (cart.value?.items) {
            updateCartItem();
          }
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Загрузка категории
    const fetchCategory = async (categoryId) => {
      console.log('Fetching category with ID:', categoryId);
      const response = await categoryService.getById(categoryId);
      if (response.status === 'success' && response.data) {
        categoryName.value = response.data.categoryName;
      }
    };

    // Загрузка корзины
    const fetchCart = async () => {
      await executeCartOperation(async () => {
        return await cartService.getCart("self");
      }, {
        onSuccess: (data) => {
          cart.value = data;
          updateCartItem();
        }
      });
    };

    // Обновление информации о товаре в корзине
    const updateCartItem = () => {
      if (product.value && cart.value?.items) {
        cartItem.value = cart.value.items.find(item =>
          item.product.id === product.value.id);
      }
    };

    // Отслеживание изменений в продукте и корзине
    watch(() => [product.value?.id, cart.value?.items], () => {
      updateCartItem();
    });

    // Добавление в корзину
    const addToCart = async () => {
      await executeCartOperation(async () => {
        const request = {
          productId: product.value.id,
          quantity: 1
        };
        return await cartService.addCartItem(request);
      }, {
        onSuccess: (data) => {
          cartItem.value = {
            id: data,
            product: product.value,
            quantity: 1
          };
          toast.value.success('Товар добавлен в корзину');

          // Уведомляем MainNavbar о добавлении в корзину
          cartStore.delta(1);
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Обновление количества в корзине
    const updateCartItemQuantity = async (change) => {
      if (!cartItem.value) return;

      const newQuantity = cartItem.value.quantity + change;

      if (newQuantity < 1 || newQuantity > product.value.stockQuantity || newQuantity > 10) {
        return;
      }

      await executeCartOperation(async () => {
        const request = {
          productId: product.value.id,
          quantity: newQuantity
        };
        return await cartService.updateCartItem(cartItem.value.id, request);
      }, {
        onSuccess: () => {
          cartItem.value.quantity = newQuantity;
          toast.value.success('Количество товара обновлено');
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Удаление из корзины
    const deleteCartItem = async () => {
      if (!cartItem.value) return;

      await executeCartOperation(async () => {
        return await cartService.removeCartItem(cartItem.value.id);
      }, {
        onSuccess: () => {
          cartItem.value = null;
          toast.value.success('Товар удален из корзины');

          // Уведомляем MainNavbar об удалении из корзины
          cartStore.delta(-1);
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Выбор изображения
    const selectImage = (index) => {
      selectedImageIndex.value = index;
    };

    return {
      product,
      categoryName,
      loading,
      error,
      selectedImageIndex,
      cart,
      cartItem,
      cartLoading,
      toast,
      fetchProductData,
      fetchCart,
      selectImage,
      addToCart,
      updateCartItemQuantity,
      deleteCartItem,
      selectedImage: () => {
        // Проверяем наличие изображений и валидность индекса
        if (
          product.value?.images &&
          Array.isArray(product.value.images) &&
          product.value.images.length > 0 &&
          selectedImageIndex.value >= 0 &&
          selectedImageIndex.value < product.value.images.length
        ) {
          return product.value.images[selectedImageIndex.value];
        }
        // Если изображений нет или индекс некорректный, возвращаем заглушку
        return { url: fallbackImageUrl.value };
      }
    };
  },
  created() {
    const productId = parseInt(this.$route.params.id);
    if (!isNaN(productId)) {
      this.fetchProductData(productId);
      this.fetchCart();
    } else {
      this.error = this.$t('product.error');
    }
  }
};
</script>
