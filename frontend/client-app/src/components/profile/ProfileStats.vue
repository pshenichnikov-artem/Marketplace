<template>
    <div class="bg-white rounded-lg shadow p-4">
        <div class="border-b border-gray-200 pb-3 mb-3">
            <h3 class="font-medium text-gray-700">{{ $t('profile.statistics') }}</h3>
        </div>
        <!-- Отображаем спиннер при загрузке -->
        <div v-if="loading" class="flex justify-center py-2">
            <div class="w-5 h-5 border-t-2 border-b-2 border-indigo-500 rounded-full animate-spin"></div>
        </div>
        <!-- Статистика -->
        <div v-else class="space-y-2 text-sm">
            <div class="flex justify-between">
                <span class="text-gray-600">{{ $t('profile.orderCount') }}</span>
                <span class="font-medium text-gray-800">{{ orderCount }}</span>
            </div>
            <div class="flex justify-between">
                <span class="text-gray-600">{{ $t('profile.commentCount') }}</span>
                <span class="font-medium text-gray-800">{{ commentCount }}</span>
            </div>
            <slot name="additional-stats"></slot>
        </div>
    </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import orderService from '@/services/api/orderService';
import reviewService from '@/services/api/reviewService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'ProfileStats',
    setup() {
        const orderCount = ref(0);
        const commentCount = ref(0);
        const loading = ref(true);

        const { execute: executeOrdersRequest } = useApiRequest();
        const { execute: executeReviewsRequest } = useApiRequest();

        // Загрузка статистики
        const fetchStats = async () => {
            loading.value = true;

            // Параллельная загрузка заказов и отзывов
            await Promise.all([
                fetchOrders(),
                fetchReviews()
            ]);

            loading.value = false;
        };

        // Загрузка количества заказов
        const fetchOrders = async () => {
            await executeOrdersRequest(async () => {
                return await orderService.getMyOrders();
            }, {
                onSuccess: (data) => {
                    orderCount.value = Array.isArray(data) ? data.length : 0;
                },
                showErrorNotification: false
            });
        };

        // Загрузка количества отзывов
        const fetchReviews = async () => {
            await executeReviewsRequest(async () => {
                return await reviewService.getMyReview();
            }, {
                onSuccess: (data) => {
                    commentCount.value = Array.isArray(data) ? data.length : 0;
                },
                showErrorNotification: false
            });
        };

        // Загружаем статистику при монтировании компонента
        onMounted(() => {
            fetchStats();
        });

        return {
            orderCount,
            commentCount,
            loading
        };
    }
}
</script>

<style scoped>
/* При наведении добавляем небольшой эффект подсветки */
.bg-white:hover {
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
    transition: all 0.2s ease;
}

/* Анимация при обновлении значений */
@keyframes highlight {
    0% {
        background-color: rgba(79, 70, 229, 0.1);
    }

    100% {
        background-color: transparent;
    }
}

.font-medium.text-gray-800 {
    transition: background-color 0.3s;
}

.font-medium.text-gray-800:not(:empty) {
    animation: highlight 1.5s ease-out;
}

/* Анимация спиннера */
@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}

.animate-spin {
    animation: spin 1s linear infinite;
}
</style>
