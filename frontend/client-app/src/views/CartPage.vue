<template>
  <div class="flex flex-col min-h-screen bg-gray-50">
    <MainNavbar />
    <main class="flex-grow container mx-auto px-4 py-8">
      <div class="mb-6">
        <h1 class="text-3xl font-bold text-gray-900">{{ $t('cart.title') }}</h1>
      </div>

      <StateWrapper :loading="loading" :error="error" :is-empty="!cart || !cart.items || cart.items.length === 0"
        :empty-message="$t('cart.empty')">
        <template #empty>
          <div class="bg-white rounded-xl shadow-md p-8 text-center border border-gray-200">
            <div class="flex flex-col items-center justify-center py-12">
              <div class="bg-indigo-100 p-4 rounded-full mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 text-indigo-500" fill="none"
                  viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <h2 class="text-2xl font-bold text-gray-800 mb-2">{{ $t('cart.empty') }}</h2>
              <p class="text-gray-600 mb-6 max-w-md">{{ $t('cart.emptyDescription') }}</p>
              <router-link to="/catalog"
                class="bg-indigo-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-indigo-700 transition-colors duration-300 shadow-md hover:shadow-lg flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
                </svg>
                {{ $t('cart.goToCatalog') }}
              </router-link>
            </div>
          </div>
        </template>

        <!-- Содержимое корзины -->
        <template #default>
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <!-- Список товаров в корзине -->
            <div class="lg:col-span-2">
              <div class="bg-white rounded-xl shadow-md border border-gray-200 overflow-hidden">
                <!-- Заголовок таблицы -->
                <div
                  class="hidden sm:grid grid-cols-12 gap-4 p-4 bg-gray-50 border-b border-gray-200 font-semibold text-gray-700">
                  <div class="col-span-5">{{ $t('cart.product') }}</div>
                  <div class="col-span-2 text-center">{{ $t('cart.price') }}</div>
                  <div class="col-span-3 text-center">{{ $t('cart.quantity') }}</div>
                  <div class="col-span-2 text-center">{{ $t('cart.total') }}</div>
                </div>

                <!-- Товары -->
                <div>
                  <CartItem v-for="item in cart.items" :key="item.id" :item="item"
                    :is-loading="loadingItems.has(item.id)"
                    @update:loading="(value) => handleLoadingState(item.id, value)"
                    @quantity-updated="handleQuantityUpdated" @item-deleted="handleItemDeleted" />
                </div>
              </div>
            </div>

            <!-- Сводка заказа -->
            <div>
              <div class="bg-white rounded-xl shadow-md border border-gray-200 p-6 sticky top-24">
                <div class="flex justify-between mb-6">
                  <span class="text-lg font-bold text-gray-800">{{ $t('cart.total') }}</span>
                  <span class="text-lg font-bold text-indigo-600">{{ formatCurrency(total) }}</span>
                </div>
                <button @click="checkout"
                  class="w-full bg-indigo-600 text-white py-3 rounded-lg font-semibold hover:bg-indigo-700 transition-all duration-300 shadow-md hover:shadow-lg flex items-center justify-center">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2z" />
                  </svg>
                  {{ $t('cart.checkout') }}
                </button>
              </div>
            </div>
          </div>
        </template>
      </StateWrapper>
    </main>
    <AppFooter />

    <!-- Модальное окно оформления заказа  -->
    <CheckoutModal v-if="isCheckoutModalOpen" :total-amount="total" :items="cart ? cart.items : []"
      @close="isCheckoutModalOpen = false" @order-submitted="processOrder" />

    <Notification ref="toast" />
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import MainNavbar from '@/components/layout/MainNavbar.vue';
import AppFooter from '@/components/layout/AppFooter.vue';
import CartItem from '@/components/cart/CartItem.vue';
import CheckoutModal from '@/components/checkout/CheckoutModal.vue';
import StateWrapper from '@/components/ui/StateWrapper.vue';
import Notification from '@/components/ui/Notification.vue';
import cartService from '@/services/api/cartService';
import orderService from '@/services/api/orderService';
import useApiRequest from '@/hooks/useApiRequest';
import { useCartStore } from '@/stores/modules/cart';

export default {
  name: 'CartPage',
  components: {
    MainNavbar,
    AppFooter,
    CartItem,
    CheckoutModal,
    StateWrapper,
    Notification
  },
  setup() {
    const { t } = useI18n();
    const cart = ref(null);
    const loadingItems = ref(new Set());
    const shipping = ref(0);
    const tax = ref(0);
    const discount = ref(0);
    const isCheckoutModalOpen = ref(false);
    const toast = ref(null);
    const cartStore = useCartStore();

    const { loading, error, execute: executeCartLoad } = useApiRequest();
    const { execute: executeOrder } = useApiRequest();
    const { execute: executeCartClear } = useApiRequest();

    const subtotal = computed(() => {
      if (!cart.value || !cart.value.items || cart.value.items.length === 0) return 0;
      return cart.value.items.reduce((sum, item) => sum + (item.product.price * item.quantity), 0);
    });

    const total = computed(() => {
      return subtotal.value + shipping.value + tax.value - discount.value;
    });

    const fetchCart = async () => {
      await executeCartLoad(async () => {
        return await cartService.getCart("self");
      }, {
        onSuccess: (data) => {
          cart.value = data;
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    const handleLoadingState = (itemId, isLoading) => {
      if (isLoading) {
        loadingItems.value.add(itemId);
      } else {
        loadingItems.value.delete(itemId);
      }
    };

    const handleQuantityUpdated = (itemId, newQuantity) => {
      const item = cart.value.items.find(item => item.id === itemId);
      if (item) {
        item.quantity = newQuantity;
      }
    };

    const handleItemDeleted = (itemId) => {
      cart.value.items = cart.value.items.filter(item => item.id !== itemId);
      cartStore.delta(-1);
    };

    const checkout = () => {
      isCheckoutModalOpen.value = true;
    };

    const processOrder = async (orderData) => {
      await executeOrder(async () => {
        const orderRequest = {
          shippingAddress: orderData.address,
          items: orderData.items.map(item => ({
            productId: item.product.id,
            quantity: item.quantity
          }))
        };
        return await orderService.createOrder(orderRequest);
      }, {
        onSuccess: () => {
          toast.value.success(t('checkout.orderSuccess'));
          const itemsCount = cart.value.items.length;
          clearCart();
          cartStore.delta(-itemsCount);
        },
        showErrorNotification: true,
        notificationRef: toast
      });

      isCheckoutModalOpen.value = false;
    };

    const clearCart = async () => {
      if (!cart.value || !cart.value.items || cart.value.items.length === 0) {
        return;
      }

      const itemIds = cart.value.items.map(item => item.id);

      await executeCartClear(async () => {
        for (const itemId of itemIds) {
          await cartService.removeCartItem(itemId);
        }
        return { status: 'success' };
      }, {
        onSuccess: () => {
          cart.value = { items: [] };
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    const formatCurrency = (value) => {
      return `${value.toLocaleString()} ₽`;
    };

    onMounted(() => {
      fetchCart();
    });

    return {
      cart,
      loading,
      error,
      loadingItems,
      shipping,
      tax,
      discount,
      subtotal,
      total,
      isCheckoutModalOpen,
      toast,
      handleLoadingState,
      handleQuantityUpdated,
      handleItemDeleted,
      checkout,
      processOrder,
      clearCart,
      formatCurrency
    };
  }
};
</script>

<style scoped>
@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.bg-white {
  transition: all 0.3s ease;
}

.bg-white:hover {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

@media (max-width: 640px) {
  .container {
    padding: 0;
  }
}
</style>
