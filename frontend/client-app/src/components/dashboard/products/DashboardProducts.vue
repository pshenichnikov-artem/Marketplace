<template>
    <div class="space-y-6">
        <router-view v-if="isProductEditorRoute"></router-view>

        <template v-else>
            <AdminPanel :title="$t('dashboard.products.title')" :columns="columns" :items="products.items"
                :total-items="products.totalItems" :current-page="currentPage" :page-size="pageSize"
                :no-data-text="$t('dashboard.products.noProducts')"
                :no-data-subtext="$t('dashboard.products.noProductsSubtext')" @add-new="openAddProductModal"
                @edit="openEditProductModal" @delete="confirmDeleteProduct" @apply-filters="applyFilters"
                @reset-filters="resetFilters" @pagination-change="handlePaginationChange">
                <template #filters>
                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.products.filters.name')" type="text"
                            v-model="filters.name" />
                    </div>

                    <div class="form-group">
                        <FilterFieldWithDropdown :label="$t('dashboard.products.filters.category')" icon="category"
                            v-model="filters.categoryId" :options="categoryOptions"
                            :placeholder="$t('dashboard.products.filters.allCategories')"
                            :no-results-text="$t('dashboard.products.filters.noResults')"
                            @select="onCategorySelected" />
                    </div>

                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.products.filters.sortBy')" type="select"
                            v-model="sortOption" :options="sortOptions" icon="sort" @change="handleSortChange" />
                    </div>

                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.products.filters.priceRange')" type="number"
                            v-model="filters.minPrice" :placeholder="$t('dashboard.products.filters.min')" />
                    </div>

                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.products.filters.priceRangeTo')" type="number"
                            v-model="filters.maxPrice" :placeholder="$t('dashboard.products.filters.max')" />
                    </div>
                </template>

                <template #cell-seller="{ value }">
                    <div v-if="value">
                        <div class="text-sm font-medium">{{ value.firstName + value.lastName }}</div>
                        <div class="text-xs text-gray-500">ID: {{ value.id }}</div>
                    </div>
                    <div v-else class="text-sm text-gray-500">{{ $t('dashboard.orders.unknownUser') }}</div>
                </template>

            </AdminPanel>

            <ConfirmModal v-if="showDeleteModal" :title="$t('dashboard.products.deleteTitle')"
                :message="$t('dashboard.products.deleteMessage', { name: productToDelete?.name || 'product' })"
                :confirm-text="$t('dashboard.products.confirmDelete')"
                :cancel-text="$t('dashboard.products.cancelDelete')" @confirm="deleteProduct"
                @cancel="showDeleteModal = false" />

            <Notification ref="toast" />
        </template>
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter, useRoute } from 'vue-router';
import AdminPanel from '@/components/admin/AdminPanel.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import BaseFilterField from '@/components/ui/filters/BaseFilterField.vue';
import FilterFieldWithDropdown from '@/components/ui/filters/FilterFieldWithDropdown.vue';
import productService from '@/services/api/productService';
import categoryService from '@/services/api/categoryService';
import useApiRequest from '@/hooks/useApiRequest';
import { getUserRole } from '@/utils/localStorage';
import { prepareCategoryOptions } from '@/utils/categoryUtils';

export default {
    name: 'DashboardProducts',
    components: {
        AdminPanel,
        ConfirmModal,
        Notification,
        BaseFilterField,
        FilterFieldWithDropdown
    },
    setup() {
        const { t, locale } = useI18n();
        const toast = ref(null);
        const router = useRouter();
        const route = useRoute();

        const isProductEditorRoute = computed(() => {
            return route.path.includes('/dashboard/products/edit/') ||
                route.path.includes('/dashboard/products/create');
        });

        const products = ref({ items: [], totalItems: 0 });
        const categories = ref([]);
        const currentPage = ref(1);
        const pageSize = ref(20);

        const sortKey = ref('id');
        const sortOrder = ref('desc');
        const sortOption = ref('id_desc');

        const isCategoryDropdownOpen = ref(false);
        const categorySearch = ref('');

        const filters = reactive({
            name: '',
            categoryId: '',
            minPrice: '',
            maxPrice: ''
        });

        const showDeleteModal = ref(false);
        const productToDelete = ref(null);

        const selectedCategoryName = computed(() => {
            if (!filters.categoryId) return '';
            const category = categories.value.find(c => c.id === parseInt(filters.categoryId));
            return category ? category.name : '';
        });

        const categoryOptions = computed(() => {
            return prepareCategoryOptions(categories.value, t('dashboard.products.filters.allCategories'));
        });

        const sortOptions = computed(() => [
            { label: t('dashboard.products.filters.sortOptions.nameAsc'), value: 'name_asc' },
            { label: t('dashboard.products.filters.sortOptions.nameDesc'), value: 'name_desc' },
            { label: t('dashboard.products.filters.sortOptions.priceAsc'), value: 'price_asc' },
            { label: t('dashboard.products.filters.sortOptions.priceDesc'), value: 'price_desc' },
            { label: t('dashboard.products.filters.sortOptions.stockAsc'), value: 'stockQuantity_asc' },
            { label: t('dashboard.products.filters.sortOptions.stockDesc'), value: 'stockQuantity_desc' },
            { label: t('dashboard.products.filters.sortOptions.dateAsc'), value: 'id_asc' },
            { label: t('dashboard.products.filters.sortOptions.dateDesc'), value: 'id_desc' }
        ]);

        const onCategorySelected = (option) => {
            filters.categoryId = option.value || option.id || '';
        };

        const columns = computed(() => [
            { key: 'id', label: 'ID', width: 'w-16', style: 'width: 60px; min-width: 60px;' },
            { key: 'name', label: t('dashboard.products.columns.name'), width: 'w-48', class: 'expandable-cell', style: 'width: 200px; min-width: 200px;' },
            { key: 'description', label: t('dashboard.products.columns.description'), width: 'w-64', class: 'expandable-cell line-clamp-2', style: 'width: 250px; min-width: 250px;' },
            { key: 'seller', label: t('dashboard.products.columns.seller'), width: 'w-50', style: 'width: 125px; min-width: 150px;' },
            { key: 'price', label: t('dashboard.products.columns.price'), type: 'currency', width: 'w-28', style: 'width: 100px; min-width: 100px;' },
            { key: 'stockQuantity', label: t('dashboard.products.columns.stock'), width: 'w-28', style: 'width: 100px; min-width: 100px;' },
            { key: 'rating', label: t('dashboard.products.columns.rating'), width: 'w-24', style: 'width: 80px; min-width: 80px;' },
            { key: 'reviewCount', label: t('dashboard.products.columns.reviews'), width: 'w-28', style: 'width: 100px; min-width: 100px;' },
            { key: 'categoryName', label: t('dashboard.products.columns.category'), width: 'w-40', class: 'expandable-cell', style: 'width: 150px; min-width: 150px;' }
        ]);

        const isAdmin = computed(() => getUserRole() === 'admin');
        const isSeller = computed(() => getUserRole() === 'seller');

        const { loading: productsLoading, execute: executeProductsFetch } = useApiRequest();
        const { loading: categoriesLoading, execute: executeCategoriesFetch } = useApiRequest();
        const { loading: deleteLoading, execute: executeDelete } = useApiRequest();

        const handleSortChange = () => {
            if (sortOption.value) {
                const [key, order] = sortOption.value.split('_');
                sortKey.value = key;
                sortOrder.value = order;
                fetchProducts();
            }
        };

        const fetchProducts = async () => {
            await executeProductsFetch(async () => {
                const sellerId = isSeller.value ? "my" : undefined;
                const params = {
                    page: currentPage.value,
                    pageSize: pageSize.value,
                    search: filters.name || undefined,
                    orderBy: sortKey.value ? `${sortKey.value},${sortOrder.value}` : undefined,
                    categoryId: filters.categoryId || undefined,
                    sellerId: sellerId,
                    minPrice: filters.minPrice || undefined,
                    maxPrice: filters.maxPrice || undefined
                };

                return await productService.getProducts(params.page, params.search, params.orderBy, params.categoryId, params.sellerId, params.pageSize, params.minPrice, params.maxPrice);
            }, {
                onSuccess: (data) => {
                    console.log('Products data:', data);
                    if (data) {
                        console.log('Fetched products:', data.products);
                        products.value = {
                            items: data.products || [],
                            totalItems: data.totalCount || 0
                        };
                    }
                    console.log('Products fetched successfully:', products.value.totalItems);
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const fetchCategories = async () => {
            await executeCategoriesFetch(async () => {
                return await categoryService.getAll();
            }, {
                onSuccess: (data) => {
                    if (data) {
                        categories.value = data || [];
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const initializeData = () => {
            if (!isProductEditorRoute.value) {
                fetchCategories();
                fetchProducts();
            }
        };

        watch(() => route.path, (newPath, oldPath) => {
            if (
                (oldPath.includes('/dashboard/products/edit/') || oldPath.includes('/dashboard/products/create')) &&
                newPath === '/dashboard/products'
            ) {
                initializeData();
            }
        });

        onMounted(() => {
            initializeData();
            sortOption.value = `${sortKey.value}_${sortOrder.value}`;
        });

        const applyFilters = () => {
            currentPage.value = 1;
            fetchProducts();
        };

        const resetFilters = () => {
            Object.keys(filters).forEach(key => {
                filters[key] = '';
            });
            sortOption.value = 'id_desc';
            sortKey.value = 'id';
            sortOrder.value = 'desc';
            currentPage.value = 1;
            fetchProducts();
        };

        const handlePaginationChange = (event) => {
            currentPage.value = event.page;
            pageSize.value = event.pageSize;
            fetchProducts();
        };

        const openAddProductModal = () => {
            router.push('/dashboard/products/create');
        };

        const openEditProductModal = (product) => {
            router.push(`/dashboard/products/edit/${product.id}`);
        };

        const confirmDeleteProduct = (product) => {
            productToDelete.value = product;
            showDeleteModal.value = true;
        };

        const deleteProduct = async () => {
            if (!productToDelete.value) return;

            await executeDelete(async () => {
                return await productService.deleteProduct(productToDelete.value.id);
            }, {
                onSuccess: () => {
                    toast.value.success(t('common.messages.deleteSuccess'));
                    fetchProducts();
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
            productToDelete.value = null;
        };

        const formatCurrency = (value) => {
            if (value === null || value === undefined) return '';
            return new Intl.NumberFormat(locale.value, {
                style: 'currency',
                currency: 'RUB',
                minimumFractionDigits: 0,
                maximumFractionDigits: 0
            }).format(value);
        };

        const translateFunction = (key) => {
            return t(key);
        };

        return {
            products,
            columns,
            categories,
            categoryOptions,
            selectedCategoryName,
            isCategoryDropdownOpen,
            categorySearch,
            currentPage,
            pageSize,
            sortKey,
            sortOrder,
            sortOption,
            sortOptions,
            filters,
            isAdmin,
            isSeller,
            showDeleteModal,
            productToDelete,
            toast,
            productsLoading,
            categoriesLoading,
            deleteLoading,
            onCategorySelected,
            handleSortChange,
            applyFilters,
            resetFilters,
            handlePaginationChange,
            openAddProductModal,
            openEditProductModal,
            confirmDeleteProduct,
            deleteProduct,
            formatCurrency,
            translateFunction,
            isProductEditorRoute
        };
    }
};
</script>

<style scoped>
:deep(.expandable-cell) {
    max-width: none;
    white-space: normal;
    word-break: break-word;
}

:deep(.line-clamp-2) {
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
}

:deep(table) {
    min-width: 1190px;
    width: 100%;
    table-layout: fixed;
}

:deep(.table-container) {
    max-width: 100%;
    overflow-x: auto;
}
</style>
