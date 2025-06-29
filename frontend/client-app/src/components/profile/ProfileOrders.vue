<template>
  <div>
    <StateWrapper :loading="loading" :error="error" :is-empty="!orders || orders.length === 0"
      :empty-message="$t('profile.noOrders')">
      <!-- Кастомизация сообщения об отсутствии заказов -->
      <template #empty>
        <div
          class="bg-gradient-to-r from-blue-50 to-indigo-50 text-blue-700 border border-blue-100 rounded-lg p-8 text-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 mx-auto mb-4 text-blue-400 opacity-80" fill="none"
            viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M20 7l-8-4-8 4m16 0v10l-8 4m-8-4V7m8 4v10M4 7v10l8-4" />
          </svg>
          <p class="text-xl font-medium text-indigo-700 mb-3">{{ $t('profile.noOrders') }}</p>
          <p class="mt-2 text-blue-600 max-w-md mx-auto">{{ $t('profile.startShopping') }}</p>
          <router-link to="/catalog"
            class="inline-block mt-5 bg-gradient-to-r from-blue-600 to-indigo-600 text-white px-6 py-3 rounded-lg font-medium hover:from-blue-700 hover:to-indigo-700 transition-all duration-200 shadow-md hover:shadow-lg transform hover:-translate-y-0.5">
            <div class="flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              {{ $t('cart.goToCatalog') }}
            </div>
          </router-link>
        </div>
      </template>

      <!-- Контент, когда данные загружены -->
      <template #default>
        <!-- Детали заказа -->
        <div v-if="orderDetails" class="mb-6">
          <div class="bg-white rounded-xl shadow-lg border border-indigo-100 overflow-hidden">
            <!-- Шапка с информацией о заказе -->
            <div
              class="bg-gradient-to-r from-indigo-50 to-blue-50 px-6 py-4 border-b border-indigo-100 flex justify-between items-center">
              <div class="flex items-center">
                <button @click="closeOrderDetails"
                  class="mr-3 text-indigo-600 hover:text-indigo-800 transition-colors p-1 hover:bg-white rounded-full">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                  </svg>
                </button>
                <h3 class="font-bold text-indigo-800 text-lg">{{ $t('profile.orderDetails') }} #{{ orderDetails.id }}
                </h3>
              </div>
              <div class="text-sm text-gray-600 flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-gray-500" fill="none"
                  viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                {{ formatDate(orderDetails.orderDate) }}
              </div>
            </div>

            <div class="p-6">
              <!-- Статус заказа и оплата -->
              <div class="mb-6 bg-gray-50 rounded-lg p-4 flex flex-wrap items-center justify-between gap-4">
                <div class="flex items-center">
                  <span class="font-medium text-gray-700 mr-2">{{ $t('profile.status') }}:</span>
                  <span :class="getStatusClass(orderDetails.status)">
                    {{ getStatusText(orderDetails.status) }}
                  </span>
                </div>

                <!-- Оплата -->
                <div v-if="orderDetails.payment" class="flex items-center">
                  <span class="text-green-600 font-medium flex items-center bg-green-50 px-3 py-1 rounded-full">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-1" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    {{ $t('profile.paid') }} {{ formatDate(orderDetails.payment.paymentDate) }}
                  </span>
                </div>

                <!-- Кнопки действий -->
                <div class="flex items-center gap-2 ml-auto">
                  <button v-if="!orderDetails.payment"
                    class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg font-medium flex items-center transition-colors shadow-sm"
                    @click="payOrder(orderDetails.id)">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                    </svg>
                    {{ $t('profile.payNow') }}
                  </button>
                  <button v-if="orderDetails.status === 'pending'"
                    class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg font-medium flex items-center transition-colors shadow-sm"
                    @click="cancelOrder(orderDetails.id)">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                    {{ $t('profile.cancel') }}
                  </button>
                </div>
              </div>

              <!-- Детали заказа -->
              <div class="bg-white rounded-lg border border-gray-200 p-4 mb-6">
                <h4 class="font-medium text-gray-800 mb-4 flex items-center">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500" fill="none"
                    viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                  {{ $t('profile.orderItems') }}
                </h4>

                <div class="overflow-x-auto pb-2 -mx-4 px-4">
                  <div class="min-w-[600px]">
                    <!-- Заголовок таблицы -->
                    <div class="grid grid-cols-12 gap-4 mb-2 font-medium text-gray-700 border-b border-gray-200 pb-2">
                      <div class="col-span-6">{{ $t('profile.product') }}</div>
                      <div class="col-span-2 text-right">{{ $t('profile.price') }}</div>
                      <div class="col-span-2 text-center">{{ $t('profile.quantity') }}</div>
                      <div class="col-span-2 text-right">{{ $t('profile.subtotal') }}</div>
                    </div>

                    <!-- Элементы заказа -->
                    <div v-for="(item, index) in orderDetails.items" :key="index"
                      class="grid grid-cols-12 gap-4 py-3 border-b border-gray-100 items-center hover:bg-gray-50 transition-colors">
                      <div class="col-span-6 flex items-center">
                        <div class="flex-grow min-w-0">
                          <router-link :to="`/product/${item.productId}`"
                            class="font-medium text-indigo-600 hover:text-indigo-800 hover:underline truncate block">
                            {{ item.product?.name || `${$t('profile.product')} #${item.productId}` }}
                          </router-link>
                          <div class="text-sm text-gray-500 truncate" v-if="item.product?.seller">
                            {{ item.product.seller.firstName }} {{ item.product.seller.lastName }}
                          </div>
                        </div>
                      </div>
                      <div class="col-span-2 text-right font-medium">{{ formatCurrency(item.price) }}</div>
                      <div class="col-span-2 text-center">{{ item.quantity }}</div>
                      <div class="col-span-2 text-right font-semibold">{{ formatCurrency(item.price * item.quantity) }}
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Итоги заказа -->
                <div class="mt-4 border-t border-gray-200 pt-4">
                  <div class="flex justify-between mb-2">
                    <span class="text-gray-600">{{ $t('profile.total') }}:</span>
                    <span class="font-bold text-xl text-indigo-600">{{
                      formatCurrency(calculateTotal(orderDetails.items))
                      }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Заголовок списка заказов -->
        <div v-if="!orderDetails" class="mb-5 flex justify-between items-center">
          <h3 class="text-xl font-bold text-gray-800">{{ $t('profile.yourOrders') }}</h3>
          <div class="text-sm text-gray-500">{{ $t('profile.totalOrders', { count: orders.length }) }}</div>
        </div>

        <!-- Список заказов -->
        <div class="grid gap-4">
          <div v-for="order in orders" :key="order.id"
            class="bg-white rounded-xl shadow-sm border border-gray-200 hover:shadow-md transition-all duration-300 overflow-hidden"
            :class="{ 'border-indigo-300 ring-1 ring-indigo-300': selectedOrderId === order.id }">
            <div class="p-5 cursor-pointer" @click="toggleOrderDetails(order)">
              <div class="flex flex-wrap justify-between gap-2 mb-3">
                <div class="flex items-center">
                  <span class="font-semibold text-gray-800 mr-2">{{ $t('profile.order') }} #{{ order.id }}</span>
                  <span class="text-sm text-gray-500 flex items-center">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-gray-400" fill="none"
                      viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                    </svg>
                    {{ formatDate(order.orderDate) }}
                  </span>
                </div>
                <div :class="getStatusClass(order.status)">
                  {{ getStatusText(order.status) }}
                </div>
              </div>

              <div class="flex flex-wrap justify-between gap-2 items-center">
                <div class="text-gray-600">
                  {{ $t('profile.items', { count: order.items.length }) }}
                </div>

                <div class="flex items-center">
                  <span class="mr-3 font-medium text-gray-800">{{ formatCurrency(calculateTotal(order.items)) }}</span>
                  <button class="text-indigo-600 hover:text-indigo-800 flex items-center transition-colors"
                    @click.stop="toggleOrderDetails(order)">
                    <span class="mr-1">{{ $t('profile.details') }}</span>
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5"
                      :class="{ 'transform rotate-180': selectedOrderId === order.id }" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                  </button>
                </div>
              </div>
            </div>

            <div v-if="isOrderExpanded(order.id)" class="border-t border-gray-100 bg-gray-50 p-4 animate-fadeIn">
              <div class="overflow-x-auto">
                <div class="flex flex-nowrap gap-4 pb-2" style="min-width: max-content;">
                  <div v-for="(item, index) in order.items" :key="index"
                    class="flex-shrink-0 w-52 bg-white rounded-lg shadow-sm border border-gray-100 overflow-hidden hover:shadow-md transition-all duration-200">
                    <div class="p-4">
                      <router-link :to="`/product/${item.productId}`" class="block">
                        <div class="font-medium text-indigo-600 hover:text-indigo-800 truncate mb-2">
                          {{ item.product?.name || `${$t('profile.product')} #${item.productId}` }}
                        </div>
                      </router-link>
                      <div class="flex justify-between text-sm">
                        <span class="text-gray-600">{{ item.quantity }} {{ $t('profile.units') }}</span>
                        <span class="font-semibold">{{ formatCurrency(item.price) }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="flex justify-between items-center mt-4 pt-3 border-t border-gray-200">
                <div class="flex gap-2">
                  <button v-if="!order.payment"
                    class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg text-sm font-medium flex items-center transition-colors shadow-sm"
                    @click="payOrder(order.id)">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                    </svg>
                    {{ $t('profile.payNow') }}
                  </button>
                  <button v-if="order.status === 'pending'"
                    class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg text-sm font-medium flex items-center transition-colors shadow-sm"
                    @click="cancelOrder(order.id)">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                    {{ $t('profile.cancel') }}
                  </button>
                </div>
                <div>
                  <button
                    class="text-indigo-600 hover:text-indigo-800 text-sm font-medium flex items-center transition-colors"
                    @click="showOrderDetails(order)">
                    {{ $t('profile.viewDetails') }}
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 ml-1" fill="none" viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
    </StateWrapper>

    <!-- Компонент уведомлений -->
    <Notification ref="toast" />
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import orderService from '@/services/api/orderService';
import productService from '@/services/api/productService';
import StateWrapper from '@/components/ui/StateWrapper.vue';
import Notification from '@/components/ui/Notification.vue';
import useApiRequest from '@/hooks/useApiRequest';

export default {
  name: 'ProfileOrders',
  components: {
    StateWrapper,
    Notification
  },
  setup() {
    const orders = ref([]);
    const expandedOrderIds = ref(new Set());
    const selectedOrderId = ref(null);
    const orderDetails = ref(null);
    const toast = ref(null);
    const { t } = useI18n();

    // API хук для загрузки заказов
    const { loading, error, execute: executeOrders } = useApiRequest();

    // API хук для отмены заказов
    const { execute: executeCancelOrder } = useApiRequest();

    // Загрузка заказов
    const fetchOrders = async () => {
      await executeOrders(async () => {
        return await orderService.getMyOrders();
      }, {
        onSuccess: async (data) => {
          orders.value = data;
          // Предзагрузка информации о продуктах для каждого заказа
          await enrichOrdersWithProductInfo(orders.value);
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Обогащаем заказы информацией о продуктах
    const enrichOrdersWithProductInfo = async (ordersList) => {
      if (!ordersList || ordersList.length === 0) return;

      const productIds = new Set();
      ordersList.forEach(order => {
        if (order.items) {
          order.items.forEach(item => {
            productIds.add(item.productId);
          });
        }
      });

      const productsMap = {};
      const batchSize = 10;
      const productIdBatches = Array.from(productIds).reduce((batches, id, i) => {
        const batchIndex = Math.floor(i / batchSize);
        if (!batches[batchIndex]) batches[batchIndex] = [];
        batches[batchIndex].push(id);
        return batches;
      }, []);

      for (const batch of productIdBatches) {
        await Promise.all(batch.map(async (productId) => {
          const response = await productService.getProductById(productId);
          if (response.status === 'success' && response.data) {
            productsMap[productId] = response.data;
          }
        }));
      }

      ordersList.forEach(order => {
        if (order.items) {
          order.items.forEach(item => {
            item.product = productsMap[item.productId];
          });
        }
      });
    };

    // Форматирование даты
    const formatDate = (dateString) => {
      if (!dateString) return '';
      const date = new Date(dateString);
      return new Intl.DateTimeFormat(navigator.language || 'ru-RU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }).format(date);
    };

    // Форматирование валюты
    const formatCurrency = (amount) => {
      if (amount === undefined || amount === null) return '';
      return new Intl.NumberFormat(navigator.language || 'ru-RU', {
        style: 'currency',
        currency: 'RUB',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }).format(amount);
    };

    // Расчет общей суммы заказа
    const calculateTotal = (items) => {
      if (!items || !items.length) return 0;
      return items.reduce((total, item) => total + (item.price * item.quantity), 0);
    };

    // Получение класса статуса
    const getStatusClass = (status) => {
      const classes = {
        'pending': 'px-3 py-1 rounded-full bg-yellow-100 text-yellow-800 text-sm font-medium',
        'processing': 'px-3 py-1 rounded-full bg-blue-100 text-blue-800 text-sm font-medium',
        'shipped': 'px-3 py-1 rounded-full bg-indigo-100 text-indigo-800 text-sm font-medium',
        'delivered': 'px-3 py-1 rounded-full bg-green-100 text-green-800 text-sm font-medium',
        'cancelled': 'px-3 py-1 rounded-full bg-red-100 text-red-800 text-sm font-medium',
        'completed': 'px-3 py-1 rounded-full bg-green-100 text-green-800 text-sm font-medium',
      };
      return classes[status.toLowerCase()] || 'px-3 py-1 rounded-full bg-gray-100 text-gray-800 text-sm font-medium';
    };

    // Получение текста статуса
    const getStatusText = (status) => {
      const statusTexts = {
        'pending': 'В обработке',
        'processing': 'Обрабатывается',
        'shipped': 'Отправлен',
        'delivered': 'Доставлен',
        'cancelled': 'Отменен',
        'completed': 'Завершен'
      };
      return statusTexts[status.toLowerCase()] || status;
    };

    // Проверка, развернут ли заказ
    const isOrderExpanded = (orderId) => {
      return expandedOrderIds.value.has(orderId);
    };

    // Переключение развертывания заказа
    const toggleOrderDetails = (order) => {
      if (expandedOrderIds.value.has(order.id)) {
        expandedOrderIds.value.delete(order.id);
      } else {
        expandedOrderIds.value.add(order.id);
      }
      selectedOrderId.value = isOrderExpanded(order.id) ? order.id : null;
    };

    // Показать полные детали заказа
    const showOrderDetails = (order) => {
      orderDetails.value = order;
    };

    // Закрыть детали заказа
    const closeOrderDetails = () => {
      orderDetails.value = null;
    };

    // Оплатить заказ
    const payOrder = async (orderId) => {
      alert(`Оплата заказа #${orderId} будет реализована в следующем релизе`);
      // Здесь будет код для оплаты заказа
    };

    // Отменить заказ
    const cancelOrder = async (orderId) => {
      if (!confirm(t('profile.confirmCancel'))) return;

      await executeCancelOrder(async () => {
        return await orderService.cancelOrder(orderId);
      }, {
        onSuccess: () => {
          toast.value.success(t('profile.orderCancelledSuccess'));
          fetchOrders(); // Обновляем список заказов
        },
        showErrorNotification: true,
        notificationRef: toast
      });
    };

    // Загрузка заказов при монтировании
    onMounted(() => {
      fetchOrders();
    });

    return {
      orders,
      loading,
      error,
      expandedOrderIds,
      selectedOrderId,
      orderDetails,
      toast,
      formatDate,
      formatCurrency,
      calculateTotal,
      getStatusClass,
      getStatusText,
      isOrderExpanded,
      toggleOrderDetails,
      showOrderDetails,
      closeOrderDetails,
      payOrder,
      cancelOrder
    };
  }
};
</script>

<style scoped>
.animate-fadeIn {
  animation: fadeIn 0.3s ease forwards;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Стили для горизонтальной прокрутки */
.overflow-x-auto {
  scrollbar-width: thin;
  scrollbar-color: rgba(156, 163, 175, 0.5) transparent;
}

.overflow-x-auto::-webkit-scrollbar {
  height: 6px;
}

.overflow-x-auto::-webkit-scrollbar-track {
  background: transparent;
}

.overflow-x-auto::-webkit-scrollbar-thumb {
  background-color: rgba(156, 163, 175, 0.5);
  border-radius: 20px;
}

.overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background-color: rgba(156, 163, 175, 0.8);
}
</style>
