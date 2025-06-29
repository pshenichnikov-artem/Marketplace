<template>
  <div
    class="bg-white rounded-xl flex-shrink-0 shadow hover:shadow-xl transition-all flex flex-col border border-gray-100 mx-auto overflow-hidden"
    :style="{
      width: fixedSizes.width,
      height: fixedSizes.height,
      minWidth: fixedSizes.width,
      maxWidth: fixedSizes.width
    }">
    <!-- Картинка - фиксированная высота и ширина -->
    <div class="relative w-full bg-gray-100 rounded-t-xl overflow-hidden flex items-center justify-center" :style="{
      height: fixedSizes.imageHeight,
      minHeight: fixedSizes.imageHeight,
      maxHeight: fixedSizes.imageHeight
    }" @mouseenter="startAutoScroll" @mouseleave="stopAutoScroll">
      <div class="image-container">
        <img :src="currentImage.url" :alt="product.name" class="product-image transition-all duration-300" />
      </div>
      <!-- Индикаторы -->
      <div v-if="hasMultipleImages" class="absolute bottom-3 left-1/2 transform -translate-x-1/2 flex space-x-1">
        <span v-for="(img, idx) in product.images" :key="idx" :class="[
          'rounded-full',
          idx === imageIndex ? 'bg-indigo-600 w-2.5 h-2.5' : 'bg-gray-300 w-2.5 h-2.5'
        ]"></span>
      </div>
    </div>

    <!-- Информация - фиксированная высота -->
    <div class="flex-1 flex flex-col p-2 overflow-hidden" :style="{
      height: fixedSizes.infoHeight,
      minHeight: fixedSizes.infoHeight,
      maxHeight: fixedSizes.infoHeight
    }">
      <h2 class="font-bold mb-1 text-gray-900 truncate"
        :class="[titleClass, ['tiny', 'small'].includes(size) ? 'min-h-[24px]' : 'min-h-[28px]']">
        {{ product.name }}
      </h2>
      <div v-if="!['tiny', 'small'].includes(size)" class="text-gray-500 mb-0.5 truncate text-sm description-truncate">
        {{ product.description }}
      </div>
      <div class="flex items-center mb-0.5">
        <span class="font-extrabold text-indigo-600 mr-2" :class="priceClass">
          {{ product.price }} ₽
        </span>
        <span class="text-gray-400 ml-auto text-sm">
          {{ $t('catalog.inStock') }}: {{ product.stockQuantity }}
        </span>
      </div>
      <div class="flex items-center mb-1">
        <!-- Для size=tiny/small убираем надпись "seller:" -->
        <span v-if="!['tiny', 'small'].includes(size)" class="text-gray-500 text-sm">
          {{ $t('catalog.seller') }}:
        </span>
        <span :class="[
          'font-medium text-gray-800 truncate text-sm',
          ['tiny', 'small'].includes(size) ? '' : 'ml-2'
        ]">
          {{ sellerFullName }}
        </span>
      </div>

      <!-- Для size=tiny/small показываем только цифру рейтинга без звезд -->
      <div v-if="['tiny', 'small'].includes(size)" class="flex items-center mb-1 space-x-2">
        <span class="bg-indigo-50 text-indigo-700 px-2 py-0.5 rounded-md text-sm font-medium">
          {{ product.rating.toFixed(1) }}
        </span>
        <span class="text-gray-400 text-sm">
          ({{ product.reviewCount }} {{ $t('catalog.reviews') }})
        </span>
      </div>

      <!-- Для medium и large показываем звезды рейтинга -->
      <div v-else class="flex items-center mb-1 space-x-2">
        <StarRating :rating="product.rating" :size="20" :show-value="true" />
        <span class="text-gray-400 text-sm">
          ({{ product.reviewCount }} {{ $t('catalog.reviews') }})
        </span>
      </div>

      <!-- Кнопка всегда внизу карточки -->
      <div class="mt-auto pt-1">
        <button
          class="w-full px-4 py-1.5 bg-indigo-600 text-white rounded-lg font-semibold hover:bg-indigo-700 transition text-base"
          :class="{ 'text-sm': ['tiny', 'small'].includes(size) }" @click="goToProductDetails">
          {{ $t('catalog.details') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import StarRating from '@/components/ui/StarRating.vue';
import { API_BASE_URL } from '@/constants/api';

export default {
  name: 'ProductCard',
  components: {
    StarRating
  },
  props: {
    product: { type: Object, required: true },
    size: {
      type: String,
      default: 'large',
      validator: val => ['tiny', 'small', 'medium', 'large'].includes(val)
    }
  },
  data() {
    return {
      imageIndex: this.getInitialImageIndex(),
      autoScrollInterval: null,
      fallbackImageUrl: 'https://localhost:7037/images/no-image-placeholder.png' // URL к заглушке
    };
  },
  computed: {
    hasMultipleImages() {
      return Array.isArray(this.product.images) && this.product.images.length > 1;
    },
    currentImage() {
      // Проверка на наличие массива изображений и его содержимого
      if (
        Array.isArray(this.product.images) &&
        this.product.images.length > 0 &&
        this.imageIndex >= 0 &&
        this.imageIndex < this.product.images.length &&
        this.product.images[this.imageIndex].url
      ) {
        return this.product.images[this.imageIndex];
      }
      // Вернуть заглушку, если изображения нет
      return { url: this.fallbackImageUrl };
    },
    fullStars() {
      return Math.floor(this.product.rating);
    },
    hasHalfStar() {
      const r = this.product.rating - Math.floor(this.product.rating);
      return r >= 0.25 && r < 0.75;
    },
    // Используем простой объект для определения всех размеров
    fixedSizes() {
      // Определяем фиксированные размеры в зависимости от выбранного size
      const sizes = {
        tiny: {
          width: '162px',
          height: '290px',  // Увеличил на 2px
          imageHeight: '140px',
          infoHeight: '150px'  // Увеличил на 2px
        },
        small: {
          width: '216px',
          height: '340px',  // Увеличил на 16px
          imageHeight: '180px',
          infoHeight: '160px'  // Увеличил на 16px
        },
        medium: {
          width: '288px',  // Было 320px, уменьшено на 10%
          height: '432px', // Было 480px, уменьшено на 10%
          imageHeight: '245px', // Было 272px, уменьшено на 10%
          infoHeight: '187px'   // Было 208px, уменьшено на 10%
        },
        large: {
          width: '306px',  // Было 340px, уменьшено на 10%
          height: '468px', // Было 520px, уменьшено на 10%
          imageHeight: '281px', // Было 312px, уменьшено на 10%
          infoHeight: '187px'   // Было 208px, уменьшено на 10%
        }
      };

      return sizes[this.size] || sizes.large;
    },
    titleClass() {
      return {
        tiny: 'text-xs',
        small: 'text-sm',
        medium: 'text-lg',
        large: 'text-lg'
      }[this.size];
    },
    priceClass() {
      return {
        tiny: 'text-md',
        small: 'text-lg',
        medium: 'text-2xl',
        large: 'text-3xl'
      }[this.size];
    },
    sellerFullName() {
      if (!this.product.seller) return '';
      return `${this.product.seller.firstName || ''} ${this.product.seller.lastName || ''}`.trim();
    }
  },
  methods: {
    getInitialImageIndex() {
      if (!Array.isArray(this.product.images) || this.product.images.length === 0) {
        return -1;
      }
      const primaryIndex = this.product.images.findIndex(img => img.isPrimary);
      return primaryIndex !== -1 ? primaryIndex : 0;
    },
    startAutoScroll() {
      if (!this.hasMultipleImages) return;
      this.stopAutoScroll();
      this.autoScrollInterval = setInterval(() => {
        this.imageIndex =
          this.imageIndex === this.product.images.length - 1
            ? 0
            : this.imageIndex + 1;
      }, 1200);
    },
    stopAutoScroll() {
      if (this.autoScrollInterval) {
        clearInterval(this.autoScrollInterval);
        this.autoScrollInterval = null;
      }
      this.imageIndex = this.getInitialImageIndex();
    },
    goToProductDetails() {
      this.$router.push(`/product/${this.product.id}`);
    }
  },
  beforeUnmount() {
    this.stopAutoScroll();
  }
};
</script>

<style scoped>
/* Настройка изображений для предотвращения растягивания контейнера */
.image-container {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  position: relative;
}

/* Изменяем стиль изображения для растягивания по краям контейнера */
.product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  /* Изменено с contain на cover для растягивания изображения */
}

/* Удаляем абсолютное позиционирование, чтобы изображение растянулось на весь контейнер */
.product-image:hover {
  transform: scale(1.05);
}

/* Фиксированное позиционирование кнопки внизу */
.info-content {
  display: flex;
  flex-direction: column;
  height: 100%;
}

/* Явно задаем выравнивание для блоков в информационной части */
.mt-auto {
  margin-top: auto;
}

/* Обеспечиваем четкие границы для всей карточки */
.rounded-xl {
  box-sizing: border-box;
}

/* Уменьшаем размер шрифтов пропорционально */
.text-xs {
  font-size: 0.675rem;
  /* 90% от стандартного text-xs */
}

.text-sm {
  font-size: 0.81rem;
  /* 90% от стандартного text-sm */
}

.text-md,
.text-base {
  font-size: 0.9rem;
  /* 90% от стандартного text-base */
}

.text-lg {
  font-size: 1.035rem;
  /* 90% от стандартного text-lg */
}

.text-xl {
  font-size: 1.17rem;
  /* 90% от стандартного text-xl */
}

.text-2xl {
  font-size: 1.35rem;
  /* 90% от стандартного text-2xl */
}

.text-3xl {
  font-size: 1.71rem;
  /* 90% от стандартного text-3xl */
}

/* Адаптируем размер отступов */
.p-2 {
  padding: 0.45rem;
  /* 90% от стандартного p-2 */
}

.py-1\.5 {
  padding-top: 0.338rem;
  /* 90% от стандартного py-1.5 */
  padding-bottom: 0.338rem;
}

.px-4 {
  padding-left: 0.9rem;
  /* 90% от стандартного px-4 */
  padding-right: 0.9rem;
}

/* Корректировка стилей для режима small и tiny */
.text-sm.py-1\.5 {
  padding-top: 0.25rem;
  padding-bottom: 0.25rem;
}

/* Уменьшаем отступы в карточке small для экономии места */
div[style*="height: 340px"] .p-2 {
  padding: 0.35rem;
}

div[style*="height: 290px"] .p-2 {
  padding: 0.3rem;
}

/* Уменьшаем вертикальные отступы для компактности */
div[style*="height: 340px"] .mb-1,
div[style*="height: 290px"] .mb-1 {
  margin-bottom: 0.15rem;
}

div[style*="height: 340px"] .mb-0\.5,
div[style*="height: 290px"] .mb-0\.5 {
  margin-bottom: 0.05rem;
}

/* Уменьшаем отступ перед кнопкой */
div[style*="height: 340px"] .pt-1,
div[style*="height: 290px"] .pt-1 {
  padding-top: 0.15rem;
}

/* Улучшаем отображение описания */
.description-truncate {
    white-space: normal;
    height: 2.8em;
    line-height: 1.4;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
    text-overflow: ellipsis;
}
</style>
