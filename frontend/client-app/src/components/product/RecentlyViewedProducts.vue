<template>
    <div class="recently-viewed-section py-4" v-if="products.length > 0">
        <div class="container mx-auto px-4">
            <h2 class="text-xl font-bold mb-3 text-gray-800">{{ $t('home.recentlyViewed') }}</h2>

            <div class="relative">
                <!-- Кнопки навигации -->
                <button v-if="showLeftArrow" @click="scrollLeft"
                    class="scroll-button left-0 -left-2 bg-gradient-to-r from-white to-transparent">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
                    </svg>
                </button>

                <!-- Карточки продуктов -->
                <div ref="scrollContainer"
                    class="flex overflow-x-auto scrollbar-hide scroll-smooth gap-4 pb-4 -mx-2 px-2"
                    @scroll="handleScroll">
                    <div v-for="product in products" :key="product.id" class="flex-shrink-0">
                        <ProductCard :product="product" size="small" class="product-card" />
                    </div>
                </div>

                <button v-if="showRightArrow" @click="scrollRight"
                    class="scroll-button right-0 -right-2 bg-gradient-to-l from-white to-transparent">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                    </svg>
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { useI18n } from 'vue-i18n';
import ProductCard from '@/components/product/ProductCard.vue';
import historyService from '@/services/api/historyService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'RecentlyViewedProducts',
    components: {
        ProductCard
    },
    setup() {
        const { t } = useI18n();
        const products = ref([]);
        const scrollContainer = ref(null);
        const showLeftArrow = ref(false);
        const showRightArrow = ref(false);

        const { loading, error, execute } = useApiRequest();

        const loadRecentlyViewed = async () => {
            await execute(async () => {
                return await historyService.getRecentlyViewed();
            }, {
                onSuccess: (data) => {
                    if (data) {
                        products.value = data;

                        setTimeout(checkArrows, 100);
                    }
                }
            });
        };

        const scrollLeft = () => {
            if (!scrollContainer.value) return;

            const container = scrollContainer.value;
            const scrollAmount = container.clientWidth * 0.75;
            container.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
        };

        const scrollRight = () => {
            if (!scrollContainer.value) return;

            const container = scrollContainer.value;
            const scrollAmount = container.clientWidth * 0.75;
            container.scrollBy({ left: scrollAmount, behavior: 'smooth' });
        };

        const checkArrows = () => {
            if (!scrollContainer.value) return;

            const container = scrollContainer.value;
            showLeftArrow.value = container.scrollLeft > 0;
            showRightArrow.value = container.scrollWidth > container.clientWidth &&
                container.scrollLeft < container.scrollWidth - container.clientWidth;
        };

        const handleScroll = () => {
            checkArrows();
        };

        const handleResize = () => {
            checkArrows();
        };

        onMounted(() => {
            loadRecentlyViewed();
            window.addEventListener('resize', handleResize);

            setTimeout(checkArrows, 500);
        });

        onBeforeUnmount(() => {
            window.removeEventListener('resize', handleResize);
        });

        return {
            products,
            loading,
            error,
            scrollContainer,
            showLeftArrow,
            showRightArrow,
            scrollLeft,
            scrollRight,
            handleScroll
        };
    }
};
</script>

<style scoped>
.scrollbar-hide {
    -ms-overflow-style: none;
    scrollbar-width: none;
}

.scrollbar-hide::-webkit-scrollbar {
    display: none;
}

.scroll-button {
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    width: 24px;
    height: 24px;
    border-radius: 50%;
    background-color: rgba(255, 255, 255, 0.8);
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 10;
    cursor: pointer;
    color: #4f46e5;
    transition: all 0.2s ease-in-out;
}

.scroll-button:hover {
    background-color: rgba(255, 255, 255, 1);
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.15);
}

.recently-viewed-section {
    position: relative;
    margin-top: auto;
    background-color: #f9fafb;
    border-top: 1px solid #e5e7eb;
    padding-bottom: 1rem;
}

.product-card {
    width: 180px !important;
    height: auto !important;
    margin: 0 auto;
}

:deep(.flex-shrink-0 > div) {
    height: auto !important;
    min-height: 100% !important;
    display: flex !important;
    flex-direction: column !important;
}
</style>
