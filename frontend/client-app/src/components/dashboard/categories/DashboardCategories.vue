<template>
    <div class="space-y-6">
        <router-view v-if="isCategoryEditorRoute"></router-view>

        <template v-else>
            <!-- Компонент AdminPanel для управления категориями -->
            <AdminPanel :title="$t('dashboard.categories.title')" :columns="columns" :items="categories.items"
                :total-items="categories.totalItems" :current-page="currentPage" :page-size="pageSize"
                :no-data-text="$t('dashboard.categories.noCategories')"
                :no-data-subtext="$t('dashboard.categories.noCategoriesSubtext')" @add-new="openAddCategoryModal"
                @edit="openEditCategoryModal" @delete="confirmDeleteCategory" @apply-filters="applyFilters"
                @reset-filters="resetFilters" @pagination-change="handlePaginationChange">
                <!-- Слот для фильтров -->
                <template #filters>
                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.categories.filters.search')" type="text"
                            v-model="filters.search" />
                    </div>

                    <div class="form-group">
                        <!-- Фильтр по родительской категории -->
                        <FilterFieldWithDropdown :label="$t('dashboard.categories.filters.parentCategory')"
                            icon="category" v-model="filters.parentCategoryId" :options="categoryOptions"
                            :placeholder="$t('dashboard.categories.filters.allCategories')" :select-only-leaf="false"
                            :no-results-text="$t('dashboard.categories.filters.noResults')"
                            @select="onCategorySelected" />
                    </div>
                </template>
            </AdminPanel>

            <!-- Модальное окно подтверждения удаления -->
            <ConfirmModal v-if="showDeleteModal" :title="$t('dashboard.categories.deleteTitle')"
                :message="$t('dashboard.categories.deleteMessage', { name: categoryToDelete?.categoryName || 'category' })"
                :confirm-text="$t('dashboard.categories.confirmDelete')"
                :cancel-text="$t('dashboard.categories.cancelDelete')" @confirm="deleteCategory"
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
import categoryService from '@/services/api/categoryService';
import useApiRequest from '@/hooks/useApiRequest';
import { prepareCategoryOptions } from '@/utils/categoryUtils';

export default {
    name: 'DashboardCategories',
    components: {
        AdminPanel,
        ConfirmModal,
        Notification,
        BaseFilterField,
        FilterFieldWithDropdown
    },
    setup() {
        const { t } = useI18n();
        const toast = ref(null);
        const router = useRouter();
        const route = useRoute();

        const isCategoryEditorRoute = computed(() => {
            return route.path.includes('/dashboard/categories/edit/') ||
                route.path.includes('/dashboard/categories/create');
        });

        const categories = ref({ items: [], totalItems: 0 });
        const allCategories = ref([]);
        const currentPage = ref(1);
        const pageSize = ref(20);
        const sortKey = ref('id');
        const sortOrder = ref('desc');

        const filters = reactive({
            search: '',
            parentCategoryId: ''
        });

        const showDeleteModal = ref(false);
        const categoryToDelete = ref(null);

        const categoryOptions = computed(() => {
            return prepareCategoryOptions(allCategories.value, t('dashboard.categories.filters.allCategories'));
        });

        const columns = computed(() => [
            { key: 'id', label: 'ID', width: 'w-16' },
            { key: 'categoryName', label: t('dashboard.categories.columns.name') },
            { key: 'parentCategoryName', label: t('dashboard.categories.columns.parentCategory') },
        ]);

        const { loading: categoriesLoading, execute: executeCategoriesFetch } = useApiRequest();
        const { loading: allCategoriesLoading, execute: executeAllCategoriesFetch } = useApiRequest();
        const { loading: deleteLoading, execute: executeDelete } = useApiRequest();

        const fetchAllCategories = async () => {
            await executeAllCategoriesFetch(async () => {
                return await categoryService.getAll();
            }, {
                onSuccess: (data) => {
                    if (data) {
                        allCategories.value = data;
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const fetchCategories = async () => {
            await executeCategoriesFetch(async () => {
                return await categoryService.getAll(filters.search, filters.parentCategoryId);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        const flattenCategories = [];

                        const findParentCategoryName = (parentId) => {
                            if (!parentId) return '-';

                            const findInCategories = (categories, id) => {
                                for (const category of categories) {
                                    if (category.id === id) {
                                        return category.categoryName;
                                    }

                                    if (category.subCategories && category.subCategories.length > 0) {
                                        const found = findInCategories(category.subCategories, id);
                                        if (found) return found;
                                    }
                                }
                                return null;
                            };

                            return findInCategories(data, parentId) ||
                                findInCategories(allCategories.value, parentId) ||
                                '-';
                        };

                        const processCategories = (items) => {
                            items.forEach(item => {
                                const categoryWithParent = {
                                    ...item,
                                    parentCategoryName: findParentCategoryName(item.parentCategoryId),
                                };
                                flattenCategories.push(categoryWithParent);

                                if (item.subCategories && item.subCategories.length > 0) {
                                    processCategories(item.subCategories);
                                }
                            });
                        };

                        processCategories(data);

                        categories.value = {
                            items: flattenCategories,
                            totalItems: flattenCategories.length
                        };
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const initializeData = () => {
            if (!isCategoryEditorRoute.value) {
                fetchAllCategories();
                fetchCategories();
            }
        };

        watch(() => route.path, (newPath, oldPath) => {
            if (
                (oldPath.includes('/dashboard/categories/edit/') || oldPath.includes('/dashboard/categories/create')) &&
                newPath === '/dashboard/categories'
            ) {
                fetchAllCategories();
                fetchCategories();
            }
        });

        onMounted(() => {
            initializeData();
        });

        const onCategorySelected = (option) => {
            filters.parentCategoryId = option.value || option.id || '';
        };

        const applyFilters = () => {
            currentPage.value = 1;
            fetchCategories();
        };

        const resetFilters = () => {
            Object.keys(filters).forEach(key => {
                filters[key] = '';
            });
            currentPage.value = 1;
            fetchCategories();
        };

        const handlePaginationChange = (event) => {
            currentPage.value = event.page;
            pageSize.value = event.pageSize;
            fetchCategories();
        };

        const openAddCategoryModal = () => {
            router.push('/dashboard/categories/create');
        };

        const openEditCategoryModal = (category) => {
            router.push(`/dashboard/categories/edit/${category.id}`);
        };

        const confirmDeleteCategory = (category) => {
            categoryToDelete.value = category;
            showDeleteModal.value = true;
        };

        const deleteCategory = async () => {
            if (!categoryToDelete.value) return;

            await executeDelete(async () => {
                return await categoryService.deleteCategory(categoryToDelete.value.id);
            }, {
                onSuccess: () => {
                    toast.value.success(t('common.messages.deleteSuccess'));
                    fetchAllCategories();
                    fetchCategories();
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
            categoryToDelete.value = null;
        };

        const translateFunction = (key) => {
            return t(key);
        };

        return {
            categories,
            columns,
            categoryOptions,
            currentPage,
            pageSize,
            sortKey,
            sortOrder,
            filters,
            showDeleteModal,
            categoryToDelete,
            toast,
            categoriesLoading,
            allCategoriesLoading,
            deleteLoading,
            onCategorySelected,
            applyFilters,
            resetFilters,
            handlePaginationChange,
            openAddCategoryModal,
            openEditCategoryModal,
            confirmDeleteCategory,
            deleteCategory,
            translateFunction,
            isCategoryEditorRoute
        };
    }
};
</script>
