<template>
    <div
        class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden transition-all duration-300 hover:shadow-md">
        <!-- Шапка отзыва -->
        <div class="bg-gradient-to-r from-indigo-50 to-blue-50 px-5 py-4 border-b border-gray-200">
            <div class="flex flex-wrap justify-between items-center gap-3">
                <div class="flex items-center">
                    <router-link :to="`/product/${review.productId}`"
                        class="font-medium text-indigo-600 hover:text-indigo-800 hover:underline">
                        {{ productName || `${t('profile.product')} #${review.productId}` }}
                    </router-link>
                    <span class="mx-2 text-gray-400">•</span>
                    <span class="text-sm text-gray-600">{{ formatDate(review.createdAt) }}</span>
                </div>
                <div class="flex items-center">
                    <StarRating :rating="review.rating" :size="18" />
                </div>
            </div>
        </div>

        <!-- Содержимое отзыва -->
        <div class="p-5">
            <!-- Комментарий -->
            <div class="text-gray-700 mb-4 whitespace-pre-line">{{ review.comment }}</div>

            <!-- Действия с отзывом -->
            <div class="flex justify-between items-center mt-4 pt-3 border-t border-gray-100">
                <!-- Кнопка для удаления отзыва -->
                <button class="text-red-600 hover:text-red-800 text-sm font-medium flex items-center transition-colors"
                    @click="confirmDelete">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    {{ t('profile.deleteReview') }}
                </button>

                <!-- Кнопка для перехода к продукту -->
                <router-link :to="`/product/${review.productId}`"
                    class="text-indigo-600 hover:text-indigo-800 text-sm font-medium flex items-center transition-colors">
                    {{ t('profile.viewProduct') }}
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 ml-1" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                    </svg>
                </router-link>
            </div>
        </div>
    </div>

    <!-- Модальное окно подтверждения удаления -->
    <ConfirmModal v-if="showDeleteModal" :title="t('profile.confirmDeleteTitle')"
        :message="t('profile.confirmDeleteMessage')" :confirm-text="t('profile.deleteButton')"
        :cancel-text="t('profile.cancelButton')" @confirm="deleteReview" @cancel="cancelDelete" />

    <!-- Компонент уведомлений -->
    <Notification ref="toast" />
</template>

<script>
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import StarRating from '@/components/ui/StarRating.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import reviewService from '@/services/api/reviewService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'ReviewItem',
    components: {
        StarRating,
        ConfirmModal,
        Notification
    },
    props: {
        review: {
            type: Object,
            required: true
        },
        productName: {
            type: String,
            default: undefined
        }
    },
    emits: ['delete'],

    setup(props, { emit }) {
        const { t } = useI18n();
        const toast = ref(null);
        const showDeleteModal = ref(false);
        const { loading: deleteLoading, execute: executeDelete } = useApiRequest();

        // Форматирование даты
        const formatDate = (dateString) => {
            if (!dateString) return '';
            const date = new Date(dateString);
            return new Intl.DateTimeFormat('ru-RU', {
                day: 'numeric',
                month: 'long',
                year: 'numeric'
            }).format(date);
        };

        // Показываем модальное окно подтверждения удаления
        const confirmDelete = () => {
            showDeleteModal.value = true;
        };

        // Скрываем модальное окно
        const cancelDelete = () => {
            showDeleteModal.value = false;
        };

        // Удаление отзыва
        const deleteReview = async () => {
            await executeDelete(async () => {
                return await reviewService.deleteReviewById(props.review.id);
            }, {
                onSuccess: () => {
                    console.log('Отзыв успешно удален', toast.value);
                    toast.value.success(t('profile.reviewDeleted'));

                    emit('delete', props.review.id);
                    showDeleteModal.value = false;
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        return {
            t,
            toast,
            showDeleteModal,
            deleteLoading,
            formatDate,
            confirmDelete,
            cancelDelete,
            deleteReview
        };
    }
};
</script>
