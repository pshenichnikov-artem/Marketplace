<template>
  <div class="border-b border-gray-200 last:border-0">
    <!-- Десктопный вид (> 640px) -->
    <div class="hidden sm:grid sm:grid-cols-12 sm:gap-4 sm:items-center p-4">
      <!-- Фото и название товара -->
      <div class="col-span-5 flex items-center">
        <!-- Фото товара -->
        <div class="w-20 h-20 rounded-lg overflow-hidden shadow border border-gray-100 mr-4 flex-shrink-0">
          <img :src="productImage" :alt="item.product.name" class="w-full h-full object-cover" />
        </div>

        <!-- Название и описание -->
        <div class="flex-1">
          <router-link :to="`/product/${item.product.id}`"
            class="font-semibold text-indigo-600 hover:text-indigo-800 hover:underline transition-colors">
            {{ item.product.name }}
          </router-link>
          <div class="text-sm text-gray-500 mt-1">{{ truncatedDescription }}</div>
        </div>
      </div>

      <!-- Цена за единицу -->
      <div class="col-span-2 text-gray-800 font-medium text-center">
        {{ formattedPrice }}
      </div>

      <!-- Количество с кнопками +/- -->
      <div class="col-span-3">
        <div class="flex items-center justify-center">
          <button
            class="bg-gray-200 text-gray-600 w-8 h-8 rounded-md flex items-center justify-center hover:bg-gray-300 transition-all duration-300"
            @click="updateQuantity(-1)" :disabled="isLoading">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clip-rule="evenodd" />
            </svg>
          </button>

          <div class="w-12 text-center font-semibold text-gray-800 px-2">
            <span v-if="isLoading"
              class="inline-block w-4 h-4 border-2 border-gray-400 border-t-indigo-600 rounded-full animate-spin"></span>
            <span v-else>{{ item.quantity }}</span>
          </div>

          <button
            class="bg-gray-200 text-gray-600 w-8 h-8 rounded-md flex items-center justify-center hover:bg-gray-300 transition-all duration-300"
            @click="updateQuantity(1)" :disabled="isLoading || item.quantity >= item.product.stockQuantity">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd"
                d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z"
                clip-rule="evenodd" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Итоговая цена товара -->
      <div class="col-span-2 font-semibold text-indigo-600 text-center flex items-center justify-center">
        <div>{{ formattedTotalPrice }}</div>

        <!-- Кнопка удаления на десктопе -->
        <button @click="handleDelete" :disabled="isLoading" class="text-gray-400 hover:text-red-500 ml-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>
    </div>

    <!-- Мобильный вид (< 640px) -->
    <div class="sm:hidden p-3">
      <div class="flex mb-3">
        <!-- Фото товара -->
        <div class="w-24 h-24 rounded-lg overflow-hidden shadow border border-gray-100 mr-3 flex-shrink-0">
          <img :src="productImage" :alt="item.product.name" class="w-full h-full object-cover" />
        </div>

        <!-- Информация о товаре и цена -->
        <div class="flex-1 flex flex-col">
          <router-link :to="`/product/${item.product.id}`"
            class="font-semibold text-indigo-600 hover:text-indigo-800 hover:underline transition-colors text-sm">
            {{ item.product.name }}
          </router-link>

          <!-- Добавляем краткое описание товара -->
          <div class="text-xs text-gray-500 mt-1 line-clamp-2">{{ truncatedDescription }}</div>

          <div class="flex justify-between items-center mt-1">
            <div class="text-xs text-gray-500">{{ $t('cart.unitPrice') }}: <span class="font-medium text-gray-800">{{
              formattedPrice }}</span></div>
          </div>

          <!-- Добавляем продавца и наличие -->
          <div class="flex justify-between items-center mt-1">
            <div class="text-xs text-gray-500">{{ $t('cart.inStock') }}: <span class="text-gray-800">{{
              item.product.stockQuantity }}</span></div>
            <!-- Перемещаем кнопку удаления в правый верхний угол -->
            <button @click="handleDelete" :disabled="isLoading" class="text-red-500 hover:text-red-700 p-1">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Отдельный блок для управления количеством и итоговой ценой -->
      <div class="flex items-center justify-between mt-2 bg-gray-50 p-2 rounded-lg">
        <!-- Счетчик количества -->
        <div class="flex items-center">
          <button
            class="bg-gray-200 text-gray-600 w-8 h-8 rounded-md flex items-center justify-center hover:bg-gray-300 transition-all duration-300"
            @click="updateQuantity(-1)" :disabled="isLoading">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clip-rule="evenodd" />
            </svg>
          </button>

          <div class="w-10 text-center font-semibold text-gray-800 px-1 text-sm">
            <span v-if="isLoading"
              class="inline-block w-4 h-4 border-2 border-gray-400 border-t-indigo-600 rounded-full animate-spin"></span>
            <span v-else>{{ item.quantity }}</span>
          </div>

          <button
            class="bg-gray-200 text-gray-600 w-8 h-8 rounded-md flex items-center justify-center hover:bg-gray-300 transition-all duration-300"
            @click="updateQuantity(1)" :disabled="isLoading || item.quantity >= item.product.stockQuantity">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd"
                d="M10 5a1 1 0 011 1v3h3a1 1 0 110 2h-3v3a1 1 0 11-2 0v-3H6a1 1 0 110-2h3V6a1 1 0 011-1z"
                clip-rule="evenodd" />
            </svg>
          </button>
        </div>

        <!-- Итоговая цена с подписью -->
        <div class="flex flex-col items-end">
          <span class="text-xs text-gray-500">{{ $t('cart.totalPrice') }}:</span>
          <div class="text-indigo-600 font-semibold text-base">
            {{ formattedTotalPrice }}
          </div>
        </div>
      </div>

      <!-- Добавляем кнопку "Перейти к товару" -->
      <div class="mt-2 flex justify-end">
        <router-link :to="`/product/${item.product.id}`"
          class="text-xs text-indigo-600 hover:text-indigo-800 underline">
          {{ $t('cart.goToProduct') }}
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import cartService from '@/services/api/cartService';
import { useCartStore } from "@/stores/modules/cart"

export default {
  name: 'CartItem',
  props: {
    item: {
      type: Object,
      required: true
    },
    isLoading: {
      type: Boolean,
      default: false
    }
  },
  setup() {
    const cartStore = useCartStore();
    return { cartStore };
  },
  computed: {
    productImage() {
      if (!this.item.product.images || this.item.product.images.length === 0) {
        return '';
      }
      const primaryImage = this.item.product.images.find(img => img.isPrimary);
      return primaryImage ? primaryImage.url : this.item.product.images[0].url;
    },
    truncatedDescription() {
      if (!this.item.product.description) return '';
      return this.item.product.description.length > 60
        ? this.item.product.description.substring(0, 60) + '...'
        : this.item.product.description;
    },
    formattedPrice() {
      return `${this.item.product.price.toLocaleString()} ₽`;
    },
    formattedTotalPrice() {
      return `${(this.item.product.price * this.item.quantity).toLocaleString()} ₽`;
    }
  },
  methods: {
    async updateQuantity(change) {
      if (this.isLoading) return;

      const newQuantity = this.item.quantity + change;

      if (newQuantity < 1) return;
      if (newQuantity > this.item.product.stockQuantity || newQuantity > this.item.product.quantity) return;

      this.$emit('update:loading', true);

      try {
        const request = {
          productId: this.item.product.id,
          quantity: newQuantity
        };

        const result = await cartService.updateCartItem(this.item.id, request);
        if (result.status === "success") {
          this.$emit('quantity-updated', this.item.id, newQuantity);
        } else {
          console.error('Ошибка при обновлении товара в корзине:', result.message);
        }
      } catch (error) {
        console.error('Ошибка при обновлении количества товара:', error);
      } finally {
        this.$emit('update:loading', false);
      }
    },

    async handleDelete() {
      if (this.isLoading) return;

      this.$emit('update:loading', true);

      try {
        const result = await cartService.removeCartItem(this.item.id);

        if (result.status === "success") {
          this.$emit('item-deleted', this.item.id);
          // Уведомлять MainNavbar об удалении теперь будет CartPage
        } else {
          console.error('Ошибка при удалении товара из корзины:', result.message);
        }
      } catch (error) {
        console.error('Ошибка при удалении товара из корзины:', error);
      } finally {
        this.$emit('update:loading', false);
      }
    }
  }
};
</script>

<style scoped>
/* Обеспечиваем доступный размер кнопок для тачскринов */
button {
  min-height: 32px;
  min-width: 32px;
}

/* Увеличиваем размеры кнопок на мобильных */
@media (max-width: 640px) {
  button[class*="w-8"] {
    min-height: 36px;
    min-width: 36px;
  }
}

/* Ограничение высоты описания для мобильной версии */
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
