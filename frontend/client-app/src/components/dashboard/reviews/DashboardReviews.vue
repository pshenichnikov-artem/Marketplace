<template>
    <div class="space-y-6">
        <!-- Компонент AdminPanel для управления отзывами -->
        <AdminPanel :title="$t('dashboard.reviews.title')" :columns="columns" :items="reviews.items"
            :total-items="reviews.totalItems" :current-page="currentPage" :page-size="pageSize"
            :no-data-text="$t('dashboard.reviews.noReviews')"
            :no-data-subtext="$t('dashboard.reviews.noReviewsSubtext')" @apply-filters="applyFilters"
            @reset-filters="resetFilters" @pagination-change="handlePaginationChange" @edit="openReviewDetails"
            @delete="confirmDeleteReview">

            <!-- Слот для фильтров -->
            <template #filters>
                <div class="form-group">
                    <BaseFilterField :label="$t('dashboard.reviews.filters.search')" type="text"
                        v-model="filters.search" />
                </div>

                <div class="form-group">
                    <BaseFilterField :label="$t('dashboard.reviews.filters.userId')" type="number"
                        v-model="filters.userId" :placeholder="$t('dashboard.reviews.filters.userIdPlaceholder')" />
                </div>

                <div class="form-group">
                    <BaseFilterField :label="$t('dashboard.reviews.filters.productId')" type="number"
                        v-model="filters.productId"
                        :placeholder="$t('dashboard.reviews.filters.productIdPlaceholder')" />
                </div>

                <div class="form-group">
                    <BaseFilterField :label="$t('dashboard.reviews.filters.sortBy')" type="select" v-model="sortOption"
                        :options="sortOptions" @change="handleSortChange" />
                </div>
            </template>

            <!-- Кастомные слоты для ячеек таблицы -->
            <template #cell-rating="{ value }">
                <div class="flex items-center">
                    <div class="flex items-center">
                        <svg v-for="i in 5" :key="i" :class="i <= value ? 'text-yellow-400' : 'text-gray-300'"
                            class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                            <path
                                d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                        </svg>
                    </div>
                    <span class="ml-2 text-sm text-gray-600">{{ value }}</span>
                </div>
            </template>

            <template #cell-comment="{ value }">
                <div class="truncate max-w-xs" :title="value">{{ value }}</div>
            </template>

            <template #cell-userName="{ row }">
                <span>{{ row.firstName }} {{ row.lastName }}</span>
            </template>
        </AdminPanel>

        <!-- Модальное окно подтверждения удаления -->
        <ConfirmModal v-if="showDeleteModal" :title="$t('dashboard.reviews.deleteTitle')"
            :message="$t('dashboard.reviews.deleteMessage')" :confirm-text="$t('dashboard.reviews.confirmDelete')"
            :cancel-text="$t('dashboard.reviews.cancelDelete')" @confirm="deleteReview"
            @cancel="showDeleteModal = false" />

        <!-- Уведомления -->
        <Notification ref="toast" />

        <!-- Модальное окно просмотра/редактирования отзыва -->
        <div v-if="showReviewModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div class="bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4">
                <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
                    <h3 class="text-lg font-medium text-gray-900">{{ $t('dashboard.reviews.reviewDetails') }}</h3>
                    <button @click="closeReviewModal" class="text-gray-400 hover:text-gray-600">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
                <div class="p-6">
                    <div class="mb-4">
                        <div class="flex items-center justify-between">
                            <div>
                                <h4 class="font-medium text-gray-900">{{ selectedReview?.productName || `Product
                                    #${selectedReview?.productId}` }}</h4>
                                <p class="text-sm text-gray-500">{{ formatDate(selectedReview?.createdAt) }}</p>
                                <p class="text-sm text-gray-700">
                                    {{ $t('dashboard.reviews.columns.user') }}: {{ selectedReview?.firstName }} {{
                                        selectedReview?.lastName }}
                                </p>
                            </div>
                            <div class="flex items-center">
                                <div class="flex items-center">
                                    <svg v-for="i in 5" :key="i"
                                        :class="i <= selectedReview?.rating ? 'text-yellow-400' : 'text-gray-300'"
                                        class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                                        <path
                                            d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                                    </svg>
                                </div>
                                <span class="ml-2 font-medium">{{ selectedReview?.rating }}</span>
                            </div>
                        </div>
                    </div>
                    <div class="mb-4 bg-gray-50 p-4 rounded-lg">
                        <p class="text-gray-700">{{ selectedReview?.comment }}</p>
                    </div>
                </div>
                <div class="px-6 py-4 border-t border-gray-200 flex justify-end space-x-3">
                    <button @click="closeReviewModal"
                        class="px-4 py-2 text-gray-700 border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                        {{ $t('dashboard.reviews.cancel') }}
                    </button>
                    <button @click="confirmDeleteCurrentReview"
                        class="px-4 py-2 bg-red-600 text-white rounded-md shadow-sm hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">
                        {{ $t('dashboard.reviews.confirmDelete') }}
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import AdminPanel from '@/components/admin/AdminPanel.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import BaseFilterField from '@/components/ui/filters/BaseFilterField.vue';
import useApiRequest from '@/hooks/useApiRequest';
import reviewService from '@/services/api/reviewService';

export default {
    name: 'DashboardReviews',
    components: {
        AdminPanel,
        ConfirmModal,
        Notification,
        BaseFilterField
    },
    setup() {
        const { t, locale } = useI18n();
        const toast = ref(null);

        // Состояние таблицы
        const reviews = ref({ items: [], totalItems: 0 });
        const currentPage = ref(0); // API использует нумерацию с 0
        const pageSize = ref(20);
        const sortBy = ref('createdAt');
        const sortDirection = ref('desc');

        // Фильтры
        const filters = reactive({
            search: '',
            userId: '',
            productId: ''
        });

        // Определение колонок таблицы
        const columns = computed(() => [
            { key: 'id', label: 'ID', width: 'w-16' },
            { key: 'productId', label: t('dashboard.reviews.columns.product'), width: 'w-24' },
            { key: 'userName', label: t('dashboard.reviews.columns.user'), sortable: false, width: 'w-48' },
            { key: 'rating', label: t('dashboard.reviews.columns.rating'), width: 'w-24' },
            { key: 'comment', label: t('dashboard.reviews.columns.content'), width: 'w-full' },
            { key: 'createdAt', label: t('dashboard.reviews.columns.date'), width: 'w-44' }
        ]);

        // Сортировка
        const sortKey = ref('createdAt');
        const sortOrder = ref('desc');
        const sortOption = ref('createdAt_desc');

        // Опции для сортировки
        const sortOptions = [
            { label: t('dashboard.reviews.filters.sortOptions.dateDesc'), value: 'createdAt_desc' },
            { label: t('dashboard.reviews.filters.sortOptions.dateAsc'), value: 'createdAt_asc' },
            { label: t('dashboard.reviews.filters.sortOptions.ratingDesc'), value: 'rating_desc' },
            { label: t('dashboard.reviews.filters.sortOptions.ratingAsc'), value: 'rating_asc' }
        ];

        // Методы для модального окна редактирования
        const openReviewDetails = (review) => {
            selectedReview.value = { ...review };
            showReviewModal.value = true;
        };

        const closeReviewModal = () => {
            showReviewModal.value = false;
            selectedReview.value = {};
        };

        const confirmDeleteCurrentReview = () => {
            reviewToDelete.value = selectedReview.value;
            showDeleteModal.value = true;
            closeReviewModal();
        };

        // Удаление отзыва
        const confirmDeleteReview = (review) => {
            reviewToDelete.value = review;
            showDeleteModal.value = true;
        };

        const deleteReview = async () => {
            if (!reviewToDelete.value) return;

            await executeDelete(async () => {
                return await reviewService.deleteReviewById(reviewToDelete.value.id);
            }, {
                onSuccess: (data) => {
                    toast.value.success(t('dashboard.reviews.reviewDeleted'));
                    fetchReviews();
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
            reviewToDelete.value = null;
        };

        // API хуки
        const { loading: reviewsLoading, execute: executeReviewsFetch } = useApiRequest();
        const { loading: deleteLoading, execute: executeDelete } = useApiRequest();

        // Загрузка списка отзывов
        const fetchReviews = async () => {
            await executeReviewsFetch(async () => {
                return await reviewService.getReviews(
                    filters.productId || null,
                    filters.userId || null,
                    filters.search || null,
                    currentPage.value,
                    pageSize.value,
                    sortKey.value,
                    sortOrder.value
                );
            }, {
                onSuccess: (data) => {
                    if (data) {
                        reviews.value = {
                            items: data.comments || [],
                            totalItems: data.totalCount || 0
                        };
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        // Функция инициализации данных
        const initializeData = () => {
            fetchReviews();
        };

        // Загрузка данных при монтировании компонента
        onMounted(() => {
            initializeData();
        });

        // Обработчики событий
        const applyFilters = () => {
            currentPage.value = 0; // Сбрасываем страницу при применении фильтров
            fetchReviews();
        };

        const resetFilters = () => {
            Object.keys(filters).forEach(key => {
                filters[key] = '';
            });
            currentPage.value = 0;
            fetchReviews();
        };

        const handlePaginationChange = (event) => {
            currentPage.value = event.page; // Адаптируем к API, которое использует нумерацию с 0
            pageSize.value = event.pageSize;
            fetchReviews();
        };

        // Обработчик изменения сортировки
        const handleSortChange = () => {
            if (sortOption.value) {
                const [key, order] = sortOption.value.split('_');
                sortKey.value = key;
                sortOrder.value = order;
                fetchReviews();
            }
        };

        // Форматирование даты
        const formatDate = (dateString) => {
            if (!dateString) return '';
            const date = new Date(dateString);
            return new Intl.DateTimeFormat(locale.value, {
                year: 'numeric',
                month: 'short',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            }).format(date);
        };

        // Добавляем отсутствующие состояния для модальных окон
        const showDeleteModal = ref(false);
        const showReviewModal = ref(false);
        const reviewToDelete = ref(null);
        const selectedReview = ref({});

        return {
            reviews,
            columns,
            currentPage,
            pageSize,
            filters,
            showDeleteModal, // Добавляем в возвращаемый объект
            showReviewModal,
            reviewToDelete,
            selectedReview,
            reviewsLoading,
            deleteLoading,
            sortKey,
            sortOrder,
            sortOption,
            sortOptions,
            applyFilters,
            resetFilters,
            handlePaginationChange,
            openReviewDetails,
            closeReviewModal,
            confirmDeleteCurrentReview,
            confirmDeleteReview,
            deleteReview,
            formatDate,
            handleSortChange
        };
    }
};
</script>

<style scoped>
/* Дополнительные стили при необходимости */
</style>
