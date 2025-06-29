<template>
  <div
    class="bg-white rounded-xl flex-shrink-0 shadow hover:shadow-xl transition-all flex flex-col border border-gray-100 mx-auto overflow-hidden"
    :style="{
      width: fixedSizes.width,
      height: fixedSizes.height,
      minWidth: fixedSizes.width,
      maxWidth: fixedSizes.width
    }">
    <div class="relative w-full bg-gray-100 rounded-t-xl overflow-hidden flex items-center justify-center" :style="{
      height: fixedSizes.imageHeight,
      minHeight: fixedSizes.imageHeight,
      maxHeight: fixedSizes.imageHeight
    }" @mouseenter="startAutoScroll" @mouseleave="stopAutoScroll">
      <div class="image-container">
        <img :src="currentImage.url" :alt="product.name" class="product-image transition-all duration-300" />
      </div>
      <div v-if="hasMultipleImages" class="absolute bottom-3 left-1/2 transform -translate-x-1/2 flex space-x-1">
        <span v-for="(img, idx) in product.images" :key="idx" :class="[
          'rounded-full',
          idx === imageIndex ? 'bg-indigo-600 w-2.5 h-2.5' : 'bg-gray-300 w-2.5 h-2.5'
        ]"></span>
      </div>
    </div>

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

      <div v-if="['tiny', 'small'].includes(size)" class="flex items-center mb-1 space-x-2">
        <span class="bg-indigo-50 text-indigo-700 px-2 py-0.5 rounded-md text-sm font-medium">
          {{ product.rating.toFixed(1) }}
        </span>
        <span class="text-gray-400 text-sm">
          ({{ product.reviewCount }} {{ $t('catalog.reviews') }})
        </span>
      </div>

      <div v-else class="flex items-center mb-1 space-x-2">
        <StarRating :rating="product.rating" :size="20" :show-value="true" />
        <span class="text-gray-400 text-sm">
          ({{ product.reviewCount }} {{ $t('catalog.reviews') }})
        </span>
      </div>

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
      if (
        Array.isArray(this.product.images) &&
        this.product.images.length > 0 &&
        this.imageIndex >= 0 &&
        this.imageIndex < this.product.images.length &&
        this.product.images[this.imageIndex].url
      ) {
        return this.product.images[this.imageIndex];
      }
      return { url: this.fallbackImageUrl };
    },
    fullStars() {
      return Math.floor(this.product.rating);
    },
    hasHalfStar() {
      const r = this.product.rating - Math.floor(this.product.rating);
      return r >= 0.25 && r < 0.75;
    },
    fixedSizes() {
      const sizes = {
        tiny: {
          width: '162px',
          height: '290px',
          imageHeight: '140px',
          infoHeight: '150px'
        },
        small: {
          width: '216px',
          height: '340px',
          imageHeight: '180px',
          infoHeight: '160px'
        },
        medium: {
          width: '288px',
          height: '432px',
          imageHeight: '245px',
          infoHeight: '187px'
        },
        large: {
          width: '306px',
          height: '468px',
          imageHeight: '281px',
          infoHeight: '187px'
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
.image-container {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  position: relative;
}

.product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.product-image:hover {
  transform: scale(1.05);
}

.info-content {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.mt-auto {
  margin-top: auto;
}

.rounded-xl {
  box-sizing: border-box;
}

.text-xs {
  font-size: 0.675rem;
}

.text-sm {
  font-size: 0.81rem;
}

.text-md,
.text-base {
  font-size: 0.9rem;
}

.text-lg {
  font-size: 1.035rem;
}

.text-xl {
  font-size: 1.17rem;
}

.text-2xl {
  font-size: 1.35rem;
}

.text-3xl {
  font-size: 1.71rem;
}

.p-2 {
  padding: 0.45rem;
}

.py-1\.5 {
  padding-top: 0.338rem;
  padding-bottom: 0.338rem;
}

.px-4 {
  padding-left: 0.9rem;
  padding-right: 0.9rem;
}

.text-sm.py-1\.5 {
  padding-top: 0.25rem;
  padding-bottom: 0.25rem;
}

div[style*="height: 340px"] .p-2 {
  padding: 0.35rem;
}

div[style*="height: 290px"] .p-2 {
  padding: 0.3rem;
}

div[style*="height: 340px"] .mb-1,
div[style*="height: 290px"] .mb-1 {
  margin-bottom: 0.15rem;
}

div[style*="height: 340px"] .mb-0\.5,
div[style*="height: 290px"] .mb-0\.5 {
  margin-bottom: 0.05rem;
}

div[style*="height: 340px"] .pt-1,
div[style*="height: 290px"] .pt-1 {
  padding-top: 0.15rem;
}

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
