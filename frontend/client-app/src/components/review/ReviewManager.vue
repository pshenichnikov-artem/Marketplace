<template>
    <div class="mb-8">
        <div class="bg-white rounded-lg shadow-sm border border-gray-100 overflow-hidden">
            <div
                class="flex justify-between items-center bg-gradient-to-r from-indigo-50 to-blue-50 px-6 py-4 border-b border-gray-100">
                <h3 class="font-semibold text-gray-800">{{ $t('profile.reviewManager.title') }}</h3>
                <span class="text-sm text-gray-500">{{ $t('profile.reviewManager.availableReviews', {
                    count: availableProducts.length
                }) }}</span>
            </div>

            <div v-if="loading" class="p-6 flex justify-center">
                <div class="w-8 h-8 border-t-2 border-b-2 border-indigo-500 rounded-full animate-spin"></div>
            </div>

            <div v-else-if="availableProducts.length === 0" class="p-6 text-center">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 mx-auto mb-3 text-gray-400" fill="none"
                    viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                <p class="text-gray-600">{{ $t('profile.reviewManager.noAvailableProducts') }}</p>
            </div>

            <div v-else class="p-6">
                <p class="text-gray-600 mb-4">{{ $t('profile.reviewManager.description') }}</p>

                <!-- Горизонтальный скролл с товарами (одна линия) -->
                <div class="overflow-x-auto horizontal-scroll">
                    <div class="flex gap-4 pb-2 inline-flex">
                        <div v-for="product in availableProducts" :key="product.id"
                            class="w-64 flex-shrink-0 bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md transition-shadow p-4">
                            <div class="space-y-2">
                                <h4 class="font-medium text-gray-800 line-clamp-2">{{ product.name }}</h4>
                                <p class="text-sm text-gray-500">{{ formatDate(product.purchaseDate) }}</p>
                                <button @click="openReviewModal(product)"
                                    class="w-full py-2 px-4 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg transition-colors text-sm font-medium flex items-center justify-center mt-2">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none"
                                        viewBox="0 0 24 24" stroke="currentColor">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                    </svg>
                                    {{ $t('profile.reviewManager.writeReview') }}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Модальное окно для добавления отзыва -->
        <div v-if="showReviewModal"
            class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
            <div class="bg-white rounded-lg shadow-xl w-full max-w-lg overflow-hidden" @click.stop>
                <div class="flex justify-between items-center px-6 py-4 bg-indigo-50 border-b border-gray-200">
                    <h3 class="font-semibold text-gray-800">{{ $t('profile.reviewManager.addReview') }}</h3>
                    <button @click="closeReviewModal" class="text-gray-500 hover:text-gray-700">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                <div class="p-6">
                    <div v-if="selectedProduct" class="mb-6">
                        <h4 class="font-medium text-gray-800">{{ selectedProduct.name }}</h4>
                        <p class="text-sm text-gray-500">{{ formatDate(selectedProduct.purchaseDate) }}</p>
                    </div>

                    <form @submit.prevent="submitReview">
                        <div class="mb-4">
                            <label class="block text-sm font-medium text-gray-700 mb-1">{{
                                $t('profile.reviewManager.rating') }}</label>
                            <div class="flex items-center">
                                <template v-for="i in 5" :key="i">
                                    <button type="button" @click="rating = i" class="text-2xl focus:outline-none"
                                        :class="i <= rating ? 'text-yellow-400' : 'text-gray-300'">
                                        ★
                                    </button>
                                </template>
                            </div>
                        </div>

                        <div class="mb-6">
                            <label class="block text-sm font-medium text-gray-700 mb-1">{{
                                $t('profile.reviewManager.comment') }}</label>
                            <textarea v-model="comment" @input="validateComment"
                                class="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:border-indigo-500"
                                :class="commentError ? 'border-red-500 focus:ring-red-500' : 'border-gray-300 focus:ring-indigo-500'"
                                rows="4" :placeholder="$t('profile.reviewManager.commentPlaceholder')"></textarea>
                            <p v-if="commentError" class="mt-1 text-sm text-red-600">{{ commentError }}</p>
                        </div>

                        <div class="flex justify-end gap-3">
                            <button type="button" @click="closeReviewModal"
                                class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 bg-white hover:bg-gray-50">
                                {{ $t('profile.cancelButton') }}
                            </button>
                            <button type="submit"
                                class="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 flex items-center"
                                :disabled="submitLoading">
                                <span v-if="submitLoading"
                                    class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></span>
                                {{ $t('profile.reviewManager.submitReview') }}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import orderService from '@/services/api/orderService';
import reviewService from '@/services/api/reviewService';
import productService from '@/services/api/productService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'ReviewManager',
    emits: ['review-added'],
    setup(props, { emit }) {
        const { t } = useI18n();
        const { loading: ordersLoading, execute: executeOrders } = useApiRequest();
        const { loading: reviewsLoading, execute: executeReviews } = useApiRequest();
        const { loading: submitLoading, execute: executeSubmit } = useApiRequest();

        const orders = ref([]);
        const reviews = ref([]);
        const productsMap = ref({});

        const loading = computed(() => ordersLoading.value || reviewsLoading.value);
        const showReviewModal = ref(false);
        const selectedProduct = ref(null);
        const rating = ref(5);
        const comment = ref('');
        const commentError = ref(''); // Новое состояние для отображения ошибки комментария

        // Получение списка продуктов, для которых можно оставить отзыв
        const availableProducts = computed(() => {

            if (!orders.value.length) return [];

            // Собираем все купленные продукты из заказов
            const purchasedProductsMap = {};
            orders.value.forEach(order => {
                if (order.status !== 'canceled') {
                    order.items.forEach(item => {
                        if (item.productId && productsMap.value[item.productId]) {
                            const product = productsMap.value[item.productId];

                            // Используем Map для хранения уникальных продуктов
                            purchasedProductsMap[item.productId] = {
                                id: item.productId,
                                name: product.name,
                                price: item.price,
                                purchaseDate: order.orderDate
                            };
                        }
                    });
                }
            });

            // Фильтруем продукты, исключая те, на которые уже оставлен отзыв
            const reviewedProductIds = new Set(reviews.value.map(review => review.productId));

            // Возвращаем массив доступных для отзывов продуктов
            return Object.values(purchasedProductsMap).filter(product =>
                !reviewedProductIds.has(product.id)
            );
        });

        // Загрузка данных
        const fetchData = async () => {
            await Promise.all([
                fetchOrders(),
                fetchReviews()
            ]);
        };

        // Загрузка заказов
        const fetchOrders = async () => {
            await executeOrders(async () => {
                return await orderService.getMyOrders();
            }, {
                onSuccess: async (data) => {
                    orders.value = data || [];

                    // Если есть заказы, загружаем информацию о продуктах
                    if (orders.value.length) {
                        await fetchProductsInfo();
                    }
                },
                showErrorNotification: false
            });
        };

        // Загрузка отзывов
        const fetchReviews = async () => {
            await executeReviews(async () => {
                return await reviewService.getMyReview();
            }, {
                onSuccess: (data) => {
                    reviews.value = data || [];
                },
                showErrorNotification: false
            });
        };

        // Загрузка детальной информации о продуктах
        const fetchProductsInfo = async () => {
            // Собираем все уникальные идентификаторы продуктов из заказов
            const productIds = new Set();
            orders.value.forEach(order => {
                order.items?.forEach(item => {
                    if (item.productId) productIds.add(item.productId);
                });
            });

            // Загружаем информацию о продуктах батчами
            const batchSize = 5;
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
                        productsMap.value[productId] = response.data;
                    }
                }));
            }
        };

        // Форматирование даты заказа
        const formatDate = (dateString) => {
            if (!dateString) return '';
            const date = new Date(dateString);
            return new Intl.DateTimeFormat(navigator.language || 'ru-RU', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric'
            }).format(date);
        };

        // Открытие модального окна для добавления отзыва
        const openReviewModal = (product) => {
            selectedProduct.value = product;
            rating.value = 5;
            comment.value = '';
            commentError.value = ''; // Сбрасываем ошибку при открытии модального окна
            showReviewModal.value = true;
        };

        // Закрытие модального окна
        const closeReviewModal = () => {
            showReviewModal.value = false;
            selectedProduct.value = null;
            commentError.value = ''; // Сбрасываем ошибку при закрытии модального окна
        };

        // Валидация комментария
        const validateComment = () => {
            if (!comment.value.trim()) {
                commentError.value = t('profile.reviewManager.emptyCommentError');
                return false;
            }
            commentError.value = '';
            return true;
        };

        // Отправка отзыва
        const submitReview = async () => {
            // Проверка на наличие выбранного продукта и рейтинга
            if (!selectedProduct.value || !rating.value) return;

            // Проверка на пустой комментарий
            if (!validateComment()) {
                return; // Прерываем отправку, если комментарий не прошел валидацию
            }

            const reviewData = {
                productId: selectedProduct.value.id,
                rating: rating.value,
                comment: comment.value.trim()
            };

            await executeSubmit(async () => {
                return await reviewService.createReview(reviewData);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        // Добавляем новый отзыв в список
                        // Создаем полноценную структуру отзыва даже если сервер вернул только ID
                        const newReview = {
                            id: data.id || data,
                            productId: selectedProduct.value.id,
                            rating: rating.value,
                            comment: comment.value.trim() || ""
                        };
                        reviews.value.push(newReview);

                        // Принудительно обновляем список доступных товаров
                        // Обновляем заказы и отзывы, чтобы обновить список доступных товаров
                        fetchReviews();
                    }

                    // Закрываем модальное окно
                    closeReviewModal();

                    // Уведомляем родительский компонент о добавлении отзыва
                    emit('review-added', data);
                },
                showErrorNotification: true
            });
        };

        onMounted(() => {
            fetchData();
        });

        return {
            loading,
            availableProducts,
            showReviewModal,
            selectedProduct,
            rating,
            comment,
            commentError, // Добавляем в возвращаемые значения
            submitLoading,
            formatDate,
            openReviewModal,
            closeReviewModal,
            submitReview,
            validateComment // Добавляем в возвращаемые значения
        };
    }
};
</script>

<style scoped>
.line-clamp-1 {
    display: -webkit-box;
    -webkit-line-clamp: 1;
    -webkit-box-orient: vertical;
    overflow: hidden;
}

.line-clamp-2 {
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
}

/* Анимация для звёзд рейтинга */
button.text-yellow-400 {
    transform: scale(1.05);
    transition: transform 0.2s ease;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}

.animate-spin {
    animation: spin 1s linear infinite;
}

/* Стили для горизонтального скролла */
.horizontal-scroll {
    scrollbar-width: thin;
    scrollbar-color: #d1d5db transparent;
    padding-bottom: 8px;
    max-width: 100%;
    overflow-x: auto;
    white-space: nowrap;
}

.horizontal-scroll::-webkit-scrollbar {
    height: 6px;
}

.horizontal-scroll::-webkit-scrollbar-track {
    background: transparent;
}

.horizontal-scroll::-webkit-scrollbar-thumb {
    background-color: #d1d5db;
    border-radius: 20px;
}

.horizontal-scroll::-webkit-scrollbar-thumb:hover {
    background-color: #9ca3af;
}

/* Ограничение ширины для контейнера, чтобы показывать максимум 5 товаров */
@media (min-width: 1280px) {
    .horizontal-scroll {
        max-width: calc(64rem + 4rem);
        /* 5 карточек по 256px (w-64) + 4 промежутка по 1rem (gap-4) */
    }
}

@media (min-width: 1024px) and (max-width: 1279px) {
    .horizontal-scroll {
        max-width: calc(51.2rem + 3rem);
        /* 4 карточки по 256px + 3 промежутка по 1rem */
    }
}

@media (min-width: 768px) and (max-width: 1023px) {
    .horizontal-scroll {
        max-width: calc(38.4rem + 2rem);
        /* 3 карточки по 256px + 2 промежутка по 1rem */
    }
}

@media (min-width: 640px) and (max-width: 767px) {
    .horizontal-scroll {
        max-width: calc(25.6rem + 1rem);
        /* 2 карточки по 256px + 1 промежуток по 1rem */
    }
}

@media (max-width: 639px) {
    .horizontal-scroll {
        /* 1 карточка на мобильных устройствах */
        max-width: 100%;
    }
}
</style>
