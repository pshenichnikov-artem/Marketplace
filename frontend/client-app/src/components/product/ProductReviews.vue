<template>
    <div class="bg-white rounded-xl shadow-lg p-8 border border-gray-100">
        <StateWrapper :loading="loading" :error="error" :is-empty="!reviews || reviews.length === 0"
            :empty-message="$t('product.noReviews')">
            <!-- Заголовок с иконкой и количеством -->
            <template #default>
                <div class="flex items-center justify-between mb-6 border-b border-gray-100 pb-4">
                    <div class="flex items-center">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-indigo-600 mr-2" fill="none"
                            viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-4l-4 4z" />
                        </svg>
                        <h2 class="text-2xl font-bold text-gray-800">{{ $t('product.reviews') }}</h2>
                        <span class="ml-2 px-3 py-1 bg-indigo-100 text-indigo-800 text-sm font-medium rounded-full">
                            {{ reviews.length }}
                        </span>
                    </div>
                </div>

                <!-- Список отзывов -->
                <div class="space-y-6">
                    <div v-for="review in reviews" :key="review.id"
                        class="bg-gray-50 rounded-lg p-5 hover:shadow-md transition-shadow duration-300 border border-gray-100">
                        <div class="flex items-start">
                            <div
                                class="w-12 h-12 bg-indigo-100 rounded-full flex items-center justify-center text-indigo-600 text-lg font-bold mr-4 flex-shrink-0">
                                {{ review.firstName.charAt(0) }}{{ review.lastName.charAt(0) }}
                            </div>

                            <div class="flex-1">
                                <!-- Данные пользователя и дата -->
                                <div class="flex flex-wrap justify-between items-center mb-3">
                                    <div class="font-semibold text-gray-900 text-lg">{{ review.firstName }} {{
                                        review.lastName }}</div>
                                    <div
                                        class="text-gray-500 text-sm bg-white py-1 px-3 rounded-full shadow-sm border border-gray-100">
                                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 inline mr-1" fill="none"
                                            viewBox="0 0 24 24" stroke="currentColor">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                        </svg>
                                        {{ formatDate(review.createdAt) }}
                                    </div>
                                </div>

                                <!-- Рейтинг со стилизованным фоном -->
                                <div
                                    class="flex items-center mb-3 bg-white py-1.5 px-3 rounded-md inline-block shadow-sm border border-gray-100">
                                    <StarRating :rating="review.rating" :size="20" :show-value="true" />
                                </div>

                                <!-- Текст отзыва с улучшенным форматированием -->
                                <div
                                    class="text-gray-700 bg-white p-4 rounded-lg border border-gray-100 shadow-sm leading-relaxed">
                                    {{ review.comment }}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </template>

            <template #empty>
                <div class="text-center py-12 px-4">
                    <div
                        class="inline-flex justify-center items-center mb-4 w-16 h-16 rounded-full bg-indigo-100 text-indigo-500">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z" />
                        </svg>
                    </div>
                    <p class="text-gray-500 text-lg">{{ $t('product.noReviews') }}</p>
                    <button
                        class="mt-4 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold py-2 px-6 rounded-lg shadow-sm hover:shadow transition-all duration-200">
                        {{ $t('product.leaveReview') }}
                    </button>
                </div>
            </template>
        </StateWrapper>

        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import StarRating from '@/components/ui/StarRating.vue';
import reviewService from '@/services/api/reviewService';
import StateWrapper from '@/components/ui/StateWrapper.vue';
import Notification from '@/components/ui/Notification.vue';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'ProductReviews',
    components: {
        StarRating,
        StateWrapper,
        Notification
    },
    props: {
        productId: {
            type: Number,
            required: true
        }
    },
    setup(props) {
        const reviews = ref([]);
        const toast = ref(null);

        const { loading, error, execute } = useApiRequest();

        const fetchReviews = async () => {
            await execute(async () => {
                return await reviewService.getReviews(props.productId);
            }, {
                onSuccess: (data) => {
                    reviews.value = data.comments;
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const formatDate = (dateString) => {
            const date = new Date(dateString);
            return new Intl.DateTimeFormat('ru-RU', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            }).format(date);
        };

        onMounted(() => {
            fetchReviews();
        });

        return {
            reviews,
            loading,
            error,
            toast,
            formatDate
        };
    }
};
</script>

<style scoped>
.space-y-6>div {
    animation: fadeIn 0.5s ease-out;
    animation-fill-mode: both;
}

.space-y-6>div:nth-child(2) {
    animation-delay: 0.1s;
}

.space-y-6>div:nth-child(3) {
    animation-delay: 0.2s;
}

.space-y-6>div:nth-child(4) {
    animation-delay: 0.3s;
}

.space-y-6>div:nth-child(5) {
    animation-delay: 0.4s;
}

@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}
</style>
