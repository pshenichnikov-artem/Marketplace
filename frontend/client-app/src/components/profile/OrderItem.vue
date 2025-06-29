<script setup lang="ts">
import { ref, computed } from 'vue';
import type { UserOrderResponse } from '@/types/order/UserOrderResponse';
import type { ProductResponse } from '@/types/product/ProductResponse';
import productService from '@/services/api/productService';
import { useI18n } from 'vue-i18n';

const props = defineProps<{
  order: UserOrderResponse;
}>();

const emit = defineEmits<{
  (e: 'cancel', orderId: number): void;
}>();

const { t } = useI18n();
const isExpanded = ref(false);
const products = ref<Map<number, ProductResponse>>(new Map());
const loadingProducts = ref(false);

// Форматирование даты заказа
const formattedDate = computed(() => {
  if (!props.order.orderDate) return '';
  const date = new Date(props.order.orderDate);
  return new Intl.DateTimeFormat('ru-RU', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
});

// Форматирование статуса заказа
const orderStatus = computed(() => {
  switch (props.order.status.toLowerCase()) {
    case 'completed':
      return { label: t('profile.statusCompleted'), color: 'bg-green-100 text-green-800' };
    case 'pending':
      return { label: t('profile.statusPending'), color: 'bg-yellow-100 text-yellow-800' };
    case 'cancelled':
      return { label: t('profile.statusCancelled'), color: 'bg-red-100 text-red-800' };
    case 'shipped':
      return { label: t('profile.statusShipped'), color: 'bg-blue-100 text-blue-800' };
    default:
      return { label: props.order.status, color: 'bg-gray-100 text-gray-800' };
  }
});

// Общая стоимость заказа
const totalOrderAmount = computed(() => {
  return props.order.items.reduce((sum, item) => sum + (item.price * item.quantity), 0);
});

// Форматирование валюты
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount);
};

// Количество товаров в заказе
const itemsCount = computed(() => {
  return props.order.items.reduce((sum, item) => sum + item.quantity, 0);
});

// Загрузка информации о продуктах при раскрытии заказа
const toggleExpand = async () => {
  isExpanded.value = !isExpanded.value;

  // Загружаем информацию о продуктах, только если еще не загружены
  if (isExpanded.value && products.value.size === 0) {
    loadingProducts.value = true;

    try {
      // Загружаем информацию о каждом продукте
      for (const item of props.order.items) {
        if (!products.value.has(item.productId)) {
          const response = await productService.getProductById(item.productId);
          if (response.status === 'success' && response.data) {
            products.value.set(item.productId, response.data);
          }
        }
      }
    } catch (error) {
      console.error('Ошибка при загрузке данных о продуктах:', error);
    }

    loadingProducts.value = false;
  }
};

// Обработчик кнопки отмены заказа
const handleCancelOrder = () => {
  if (confirm(t('profile.confirmCancel'))) {
    emit('cancel', props.order.id);
  }
};

// Проверка возможности отмены заказа
const canBeCancelled = computed(() => {
  return props.order.status.toLowerCase() === 'pending';
});

// Проверка, оплачен ли заказ
const isPaid = computed(() => {
  return !!props.order.payment;
});
</script>

<template>
  <div
    class="bg-white rounded-xl shadow-sm border border-gray-200 mb-4 overflow-hidden transition-all duration-300 hover:shadow-md">
    <!-- Основная информация о заказе (всегда видима) -->
    <div class="p-5 cursor-pointer hover:bg-gray-50 transition-colors duration-200" @click="toggleExpand">
      <div class="flex flex-wrap justify-between items-center gap-3 mb-3">
        <div class="flex items-center">
          <span class="font-medium text-gray-800 mr-2">{{ t('profile.order') }} #{{ order.id }}</span>
          <span :class="['px-3 py-1 rounded-full text-xs font-medium', orderStatus.color]">
            {{ orderStatus.label }}
          </span>
        </div>

        <div class="text-sm text-gray-600 flex items-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-gray-500" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          {{ formattedDate }}
        </div>
      </div>

      <div class="flex flex-wrap justify-between items-center gap-3">
        <div class="flex items-center text-gray-600 text-sm">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-gray-500" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M20 7l-8-4-8 4m16 0v10l-8 4m-8-4V7m8 4v10M4 7v10l8-4" />
          </svg>
          {{ itemsCount }} {{ t('profile.itemsCount', { count: itemsCount }) }}
        </div>

        <div class="flex items-center">
          <span class="font-bold text-indigo-600 mr-2">{{ formatCurrency(totalOrderAmount) }}</span>

          <div class="text-indigo-500 bg-indigo-50 hover:bg-indigo-100 p-1.5 rounded-full transition-colors">
            <svg :class="['w-4 h-4 transition-transform duration-300', isExpanded ? 'transform rotate-180' : '']"
              fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Раскрываемая часть с информацией о продуктах -->
    <div v-if="isExpanded" class="border-t border-gray-100 bg-gray-50 p-5 animate-fadeIn">
      <!-- Загрузка продуктов -->
      <div v-if="loadingProducts" class="flex justify-center items-center p-4 h-20">
        <div class="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-indigo-500"></div>
      </div>

      <!-- Список продуктов -->
      <div v-else>
        <h3 class="text-sm font-medium text-gray-700 mb-3">{{ t('profile.orderItems') }}</h3>

        <div class="grid gap-3 md:grid-cols-2 lg:grid-cols-3">
          <div v-for="item in order.items" :key="`${order.id}-${item.productId}`"
            class="bg-white rounded-lg border border-gray-200 p-4 hover:shadow-md transition-all duration-200">
            <router-link :to="`/product/${item.productId}`" class="block mb-2">
              <h4 class="font-medium text-indigo-600 hover:text-indigo-800 hover:underline line-clamp-2">
                {{ products.get(item.productId)?.name || t('profile.loadingProduct') }}
              </h4>
            </router-link>

            <div class="flex justify-between items-center text-gray-600 text-sm mb-2">
              <span>{{ t('profile.quantity') }}: <strong>{{ item.quantity }}</strong></span>
              <span>{{ formatCurrency(item.price) }}</span>
            </div>

            <div class="flex justify-end">
              <span class="font-bold text-gray-800">{{ formatCurrency(item.price * item.quantity) }}</span>
            </div>
          </div>
        </div>

        <div class="mt-5 pt-4 border-t border-gray-200 flex flex-wrap justify-between items-center gap-3">
          <div class="flex gap-2">
            <button v-if="!isPaid"
              class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg text-sm font-medium flex items-center transition-colors shadow-sm">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
              </svg>
              {{ t('profile.payNow') }}
            </button>

            <button v-if="canBeCancelled" @click.stop="handleCancelOrder"
              class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg text-sm font-medium flex items-center transition-colors shadow-sm">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
              {{ t('profile.cancel') }}
            </button>
          </div>

          <div class="flex items-center">
            <span class="text-gray-600 mr-2">{{ t('profile.total') }}: </span>
            <span class="font-bold text-lg text-indigo-600">{{ formatCurrency(totalOrderAmount) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

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

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
