<template>
    <div class="space-y-6">
        <!-- Проверяем, не находимся ли мы на странице редактирования или детализации заказа -->
        <router-view v-if="isOrderDetailRoute"></router-view>

        <!-- Если не находимся на странице детализации, показываем список заказов -->
        <template v-else>
            <!-- Компонент AdminPanel для управления заказами -->
            <AdminPanel :title="$t('dashboard.orders.title')" :columns="columns" :items="orders.items"
                :total-items="orders.totalItems" :current-page="currentPage" :page-size="pageSize"
                :no-data-text="$t('dashboard.orders.noOrders')"
                :no-data-subtext="$t('dashboard.orders.noOrdersSubtext')" @edit="openOrderDetails"
                @apply-filters="applyFilters" @reset-filters="resetFilters" @pagination-change="handlePaginationChange"
                :show-add-button="false" :show-delete-button="false">
                <!-- Слот для фильтров -->
                <template #filters>
                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.orders.filters.status')" type="select"
                            v-model="filters.status" :options="statusOptions" />
                    </div>

                    <!-- Фильтр по ID продукта -->
                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.orders.filters.productId')" type="number"
                            v-model="filters.productId" />
                    </div>

                    <!-- Фильтр по ID продавца (только для админа) -->
                    <div v-if="isAdmin" class="form-group">
                        <BaseFilterField :label="$t('dashboard.orders.filters.sellerId')" type="number"
                            v-model="filters.sellerId" />
                    </div>

                    <!-- Фильтр по ID покупателя (только для админа) -->
                    <div v-if="isAdmin" class="form-group">
                        <BaseFilterField :label="$t('dashboard.orders.filters.userId')" type="number"
                            v-model="filters.userId" />
                    </div>

                    <!-- Сортировка по дате -->
                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.orders.filters.sortBy')" type="select"
                            v-model="filters.orderBy" :options="sortOptions" icon="sort" />
                    </div>

                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.orders.filters.dateFrom')" type="date"
                            v-model="filters.dateFrom" />
                    </div>

                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.orders.filters.dateTo')" type="date"
                            v-model="filters.dateTo" />
                    </div>
                </template>

                <!-- Кастомная ячейка для продавца -->
                <template #cell-sellerInfo="{ value }">
                    <div v-if="value">
                        <div class="text-sm font-medium">{{ value.name }}</div>
                        <div class="text-xs text-gray-500">ID: {{ value.id }}</div>
                    </div>
                    <div v-else class="text-sm text-gray-500">{{ $t('dashboard.orders.unknownSeller') }}</div>
                </template>

                <!-- Кастомная ячейка для покупателя -->
                <template #cell-customerInfo="{ value }">
                    <div v-if="value">
                        <div class="text-sm font-medium">{{ value.name }}</div>
                        <div class="text-xs text-gray-500">ID: {{ value.id }}</div>
                    </div>
                    <div v-else class="text-sm text-gray-500">{{ $t('dashboard.orders.unknownUser') }}</div>
                </template>
            </AdminPanel>

            <!-- Уведомления -->
            <Notification ref="toast" />
        </template>
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter, useRoute } from 'vue-router';
import AdminPanel from '@/components/admin/AdminPanel.vue';
import Notification from '@/components/ui/Notification.vue';
import BaseFilterField from '@/components/ui/filters/BaseFilterField.vue';
import orderService from '@/services/api/orderService';
import useApiRequest from '@/hooks/useApiRequest';
import { getUserRole } from '@/utils/localStorage';

export default {
    name: 'DashboardOrders',
    components: {
        AdminPanel,
        Notification,
        BaseFilterField
    },
    setup() {
        const { t, locale } = useI18n();
        const toast = ref(null);
        const router = useRouter();
        const route = useRoute();

        // Определяем, находимся ли мы на странице детализации заказа
        const isOrderDetailRoute = computed(() => {
            return route.path.includes('/dashboard/orders/edit/');
        });

        // Состояние таблицы
        const orders = ref({ items: [], totalItems: 0 });
        const currentPage = ref(1);
        const pageSize = ref(20);

        // Фильтры - добавляем productId
        const filters = reactive({
            status: '',
            dateFrom: '',
            dateTo: '',
            orderBy: 'date_desc',
            sellerId: '',
            userId: '',
            productId: '' // Новый фильтр для ID продукта
        });

        // Определяем роль пользователя
        const isAdmin = computed(() => getUserRole() === 'admin');
        const isSeller = computed(() => getUserRole() === 'seller');

        // Опции для фильтра статуса
        const statusOptions = computed(() => [
            { label: t('dashboard.orders.filters.allStatuses'), value: '' },
            { label: t('dashboard.orders.status.pending'), value: 'pending' },
            { label: t('dashboard.orders.status.processing'), value: 'processing' },
            { label: t('dashboard.orders.status.shipped'), value: 'shipped' },
            { label: t('dashboard.orders.status.delivered'), value: 'delivered' },
            { label: t('dashboard.orders.status.completed'), value: 'completed' },
            { label: t('dashboard.orders.status.cancelled'), value: 'cancelled' }
        ]);

        // Опции сортировки
        const sortOptions = computed(() => [

            { label: t('dashboard.orders.filters.sortNewest'), value: 'date_desc' },
            { label: t('dashboard.orders.filters.sortOldest'), value: 'date_asc' }
        ]);

        // Определение колонок таблицы
        const columns = computed(() => {
            const baseColumns = [
                { key: 'id', label: 'ID', width: 'w-16' },
                { key: 'orderDate', label: t('dashboard.orders.columns.date'), width: 'w-40', type: 'date' },
                { key: 'status', label: t('dashboard.orders.columns.status'), width: 'w-28', type: 'status', statusContext: 'orders' },
            ];

            // Для админа добавляем информацию о продавце и покупателе
            const adminColumns = isAdmin.value ? [
                { key: 'sellerInfo', label: t('dashboard.orders.columns.seller'), width: 'w-40' },
                { key: 'customerInfo', label: t('dashboard.orders.columns.customer'), width: 'w-40' },
            ] : [];

            // Общие колонки в конце
            const endColumns = [
                { key: 'total', label: t('dashboard.orders.columns.total'), width: 'w-20', type: 'currency' },
                { key: 'items.length', label: t('dashboard.orders.columns.items'), width: 'w-20' }
            ];

            return [...baseColumns, ...adminColumns, ...endColumns];
        });

        // API хуки
        const { loading: ordersLoading, execute: executeOrdersFetch } = useApiRequest();

        // Загрузка списка заказов
        const fetchOrders = async () => {
            await executeOrdersFetch(async () => {
                // Подготовка параметров запроса
                const userId = isAdmin.value && filters.userId ? parseInt(filters.userId) : undefined;
                const productId = filters.productId ? parseInt(filters.productId) : undefined;
                const sellerId = isAdmin.value && filters.sellerId ? parseInt(filters.sellerId) :
                    (isSeller.value ? "my" : undefined); // для продавца передаем null, для админа undefined или id

                // Формируем параметры запроса с учетом обновленных параметров в API
                return await orderService.getAll(
                    currentPage.value,
                    pageSize.value,
                    userId,
                    productId, // Теперь используем productId для фильтрации
                    filters.status || undefined,
                    filters.orderBy || undefined,
                    filters.dateTo || undefined,
                    filters.dateFrom || undefined,
                    sellerId
                );
            }, {
                onSuccess: (response) => {
                    if (response) {
                        // Обрабатываем новую структуру данных
                        const processedOrders = (response.orders || []).map(order => {
                            // Информация о продавце собирается из товаров заказа
                            let sellerInfo = null;
                            if (order.items && order.items.length > 0 && order.items[0].product && order.items[0].product.seller) {
                                const seller = order.items[0].product.seller;
                                sellerInfo = {
                                    id: seller.id,
                                    name: `${seller.firstName} ${seller.lastName}`
                                };
                            }

                            // Информация о покупателе
                            let customerInfo = null;
                            if (order.customer) {
                                customerInfo = {
                                    id: order.customer.id,
                                    name: `${order.customer.firstName} ${order.customer.lastName}`
                                };
                            }

                            // Расчет общей суммы заказа
                            const total = calculateTotal(order.items);

                            const result = {
                                ...order,
                                sellerInfo,
                                customerInfo,
                                total
                            };
                            console.log('Processed Order:', result);
                            return result;

                        });

                        orders.value = {
                            items: processedOrders,
                            totalItems: response.totalItem || processedOrders.length
                        };
                    } else {
                        orders.value = { items: [], totalItems: 0 };
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        // Расчет общей суммы заказа
        const calculateTotal = (items) => {

            if (!items || !items.length) return 0;
            const result = items.reduce((total, item) => total + (item.price * item.quantity), 0);

            return result;
        };



        // Получение класса для бейджа статуса
        const getStatusBadgeClass = (status) => {
            const classes = {
                'pending': 'bg-yellow-100 text-yellow-800',
                'processing': 'bg-blue-100 text-blue-800',
                'shipped': 'bg-indigo-100 text-indigo-800',
                'delivered': 'bg-green-100 text-green-800',
                'cancelled': 'bg-red-100 text-red-800',
                'completed': 'bg-green-100 text-green-800',
                // Добавляем соответствие для русских статусов
                'Ждет обработки': 'bg-yellow-100 text-yellow-800',
                'В обработке': 'bg-blue-100 text-blue-800',
                'Отправлен': 'bg-indigo-100 text-indigo-800',
                'Доставлен': 'bg-green-100 text-green-800',
                'Отменен': 'bg-red-100 text-red-800',
                'Завершен': 'bg-green-100 text-green-800'
            };
            return classes[status] || 'bg-gray-100 text-gray-800';
        };

        // Функция инициализации данных
        const initializeData = () => {
            if (!isOrderDetailRoute.value) {
                fetchOrders();
            }
        };

        // Наблюдение за изменениями маршрута
        watch(() => route.path, (newPath, oldPath) => {
            // Если мы возвращаемся из детализации на список заказов
            if (oldPath.includes('/dashboard/orders/edit/') &&
                newPath === '/dashboard/orders') {
                initializeData();
            }
        });

        // Загрузка данных при монтировании компонента
        onMounted(() => {
            initializeData();
        });

        // Обработчики событий
        const applyFilters = () => {
            currentPage.value = 1; // Сбрасываем страницу при применении фильтров
            fetchOrders();
        };

        const resetFilters = () => {
            Object.keys(filters).forEach(key => {
                filters[key] = '';
            });
            currentPage.value = 1;
            fetchOrders();
        };

        const handlePaginationChange = (event) => {
            currentPage.value = event.page;
            pageSize.value = event.pageSize;
            fetchOrders();
        };

        // Открытие детальной информации о заказе
        const openOrderDetails = (order) => {
            router.push(`/dashboard/orders/edit/${order.id}`);
        };

        return {
            orders,
            columns,
            currentPage,
            pageSize,
            filters,
            statusOptions,
            sortOptions,
            isAdmin,
            isSeller,
            toast,
            ordersLoading,
            applyFilters,
            resetFilters,
            handlePaginationChange,
            openOrderDetails,
            calculateTotal,
            getStatusBadgeClass,
            isOrderDetailRoute
        };
    }
};
</script>

<style scoped>
/* Дополнительные стили при необходимости */
</style>
