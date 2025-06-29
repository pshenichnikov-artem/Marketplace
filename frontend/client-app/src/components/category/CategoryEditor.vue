<template>
    <div class="max-w-5xl mx-auto">
        <!-- Используем общий компонент EntityEditor в качестве оболочки -->
        <EntityEditor :title="isEditMode ? $t('dashboard.categories.edit') : $t('dashboard.categories.create')"
            :is-saving="isSaving" :back-link="'/dashboard/categories'" @back="handleBack" @save="saveCategory">
            <!-- Дополнительные действия в шапке -->
            <template v-if="isEditMode" #actions>
                <button @click="confirmDelete"
                    class="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    {{ $t('dashboard.categories.deleteCategory') }}
                </button>
            </template>

            <!-- Основное содержимое формы редактирования категории -->
            <div class="space-y-6">
                <!-- Индикатор загрузки при инициализации редактора -->
                <div v-if="isLoading" class="flex justify-center items-center py-8">
                    <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-indigo-500"></div>
                </div>

                <!-- Форма категории -->
                <div v-else class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                    <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500" viewBox="0 0 20 20"
                            fill="currentColor">
                            <path fill-rule="evenodd"
                                d="M17.707 9.293a1 1 0 010 1.414l-7 7a1 1 0 01-1.414 0l-7-7A.997.997 0 012 10V5a3 3 0 013-3h5c.256 0 .512.098.707.293l7 7zM5 6a1 1 0 100-2 1 1 0 000 2z"
                                clip-rule="evenodd" />
                        </svg>
                        <h3 class="font-semibold text-gray-800">{{ $t('dashboard.categories.info') }}</h3>
                    </div>
                    <div class="p-5">
                        <div class="space-y-4">
                            <!-- Название категории -->
                            <ValidationInput id="categoryName" :label="$t('dashboard.categories.name')" type="text"
                                v-model="category.categoryName" validationRules="required" :error-messages="{
                                    required: $t('dashboard.categories.validation.nameRequired')
                                }" :placeholder="$t('dashboard.categories.namePlaceholder')" ref="categoryNameInput"
                                @validateInput="updateValidationState('categoryName', $event)" />

                            <!-- Родительская категория -->
                            <div>
                                <label class="block text-sm font-medium text-gray-700 mb-1">
                                    {{ $t('dashboard.categories.parentCategory') }}
                                </label>
                                <div class="relative">
                                    <FilterFieldWithDropdown :label="$t('dashboard.categories.parentCategory')"
                                        icon="category" v-model="category.parentCategoryId" :options="categoryOptions"
                                        :select-only-leaf="false"
                                        :placeholder="$t('dashboard.categories.selectParentCategory')"
                                        :no-results-text="$t('dashboard.categories.filters.noResults')"
                                        @select="onCategorySelected" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </EntityEditor>

        <!-- Модальное окно подтверждения удаления -->
        <ConfirmModal v-if="showDeleteModal" :title="$t('dashboard.categories.deleteConfirm')"
            :message="$t('dashboard.categories.deleteConfirmText')"
            :confirm-text="$t('dashboard.categories.deleteConfirmButton')"
            :cancel-text="$t('dashboard.categories.cancelDelete')" @confirm="deleteCategory" @cancel="cancelDelete" />

        <!-- Уведомления -->
        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import EntityEditor from '@/components/admin/EntityEditor.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import ValidationInput from '@/components/ui/ValidationInput.vue';
import FilterFieldWithDropdown from '@/components/ui/filters/FilterFieldWithDropdown.vue';
import categoryService from '@/services/api/categoryService';
import useApiRequest from '@/hooks/useApiRequest';
import { prepareCategoryOptions } from '@/utils/categoryUtils';

export default {
    name: 'CategoryEditor',
    components: {
        EntityEditor,
        ConfirmModal,
        Notification,
        ValidationInput,
        FilterFieldWithDropdown
    },
    props: {
        id: {
            type: [String, Number],
            default: null
        }
    },
    emits: ['save', 'back', 'cancel'],
    setup(props) {
        const { t } = useI18n();
        const router = useRouter();
        const route = useRoute();

        const toast = ref(null);
        const categoryNameInput = ref(null);

        // Состояния
        const isLoading = ref(false);
        const isSaving = ref(false);
        const showDeleteModal = ref(false);

        // Данные категории
        const category = reactive({
            id: null,
            categoryName: '',
            parentCategoryId: null
        });

        // Список всех категорий для выбора родительской категории
        const categories = ref([]);

        // Режим (создание или редактирование)
        const isEditMode = computed(() => !!props.id);

        // Преобразование категорий в формат для выпадающего списка
        const categoryOptions = computed(() => {
            // Фильтруем список, чтобы исключить текущую категорию и ее подкатегории
            // это предотвратит создание циклических зависимостей
            const filteredCategories = isEditMode.value
                ? filterCategoriesForEdit(categories.value, parseInt(props.id))
                : [...categories.value];

            return prepareCategoryOptions(filteredCategories, t('dashboard.categories.noParentCategory'));
        });

        // Функция для фильтрации категорий при редактировании
        function filterCategoriesForEdit(categories, currentId) {
            const result = [];

            for (const cat of categories) {
                // Пропускаем текущую категорию и все ее подкатегории
                if (cat.id === currentId) {
                    continue;
                }

                const newCat = { ...cat };

                // Рекурсивно фильтруем подкатегории
                if (cat.subCategories && cat.subCategories.length > 0) {
                    newCat.subCategories = filterCategoriesForEdit(cat.subCategories, currentId);
                }

                result.push(newCat);
            }

            return result;
        }

        // Валидация
        const validationState = reactive({
            categoryName: false
        });

        const errors = reactive({
            categoryName: ''
        });

        // Метод для обновления состояния валидации
        const updateValidationState = (field, isValid) => {
            validationState[field] = isValid;
        };

        // API хуки
        const { loading: loadingCategory, execute: executeLoadCategory } = useApiRequest();
        const { loading: loadingCategories, execute: executeLoadCategories } = useApiRequest();
        const { loading: savingCategory, execute: executeSaveCategory } = useApiRequest();
        const { loading: deletingCategory, execute: executeDeleteCategory } = useApiRequest();

        // Отслеживание состояния загрузки
        const watchLoading = () => {
            isLoading.value = loadingCategory.value || loadingCategories.value;
            isSaving.value = savingCategory.value;
        };

        // Загрузка категорий
        const loadCategories = async () => {
            await executeLoadCategories(async () => {
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

            watchLoading();
        };

        // Загрузка данных категории при редактировании
        const loadCategory = async () => {
            if (!props.id) return;

            await executeLoadCategory(async () => {
                return await categoryService.getById(parseInt(props.id));
            }, {
                onSuccess: (data) => {
                    if (data) {
                        category.id = data.id;
                        category.categoryName = data.categoryName;
                        category.parentCategoryId = data.parentCategoryId;

                        // Запускаем валидацию полей после загрузки данных
                        validateAllFields();
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            watchLoading();
        };

        // Валидация полей формы
        const validateField = (field) => {
            switch (field) {
                case 'categoryName':
                    errors.categoryName = !category.categoryName
                        ? t('dashboard.categories.validation.nameRequired')
                        : '';
                    break;
            }

            // Обновляем состояние валидации
            validationState[field] = !errors[field];
        };

        // Валидация всех полей
        const validateAllFields = () => {
            categoryNameInput.value?.validate();
        };

        const validateForm = () => {
            validateAllFields();

            // Проверяем валидность формы
            return validationState.categoryName;
        };

        // Обработчик выбора категории
        const onCategorySelected = (option) => {
            category.parentCategoryId = option.value || option.id || null;
        };

        // Методы для управления категорией
        const saveCategory = async () => {
            // Валидируем форму
            if (!validateForm()) {
                return;
            }

            // Создаем запрос
            const categoryRequest = {
                categoryName: category.categoryName,
                parentCategoryId: category.parentCategoryId
            };

            await executeSaveCategory(async () => {
                if (isEditMode.value) {
                    return await categoryService.update(category.id, categoryRequest);
                } else {
                    return await categoryService.create(categoryRequest);
                }
            }, {
                onSuccess: () => {
                    if (isEditMode.value) {
                        toast.value.success(t('dashboard.categories.categorySaved'));
                    } else {
                        toast.value.success(t('dashboard.categories.categoryCreated'));
                    }
                    router.push('/dashboard/categories');
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            watchLoading();
        };

        const confirmDelete = () => {
            showDeleteModal.value = true;
        };

        const cancelDelete = () => {
            showDeleteModal.value = false;
        };

        const deleteCategory = async () => {
            await executeDeleteCategory(async () => {
                return await categoryService.deleteCategory(category.id);
            }, {
                onSuccess: () => {
                    toast.value.success(t('dashboard.categories.categoryDeleted'));
                    router.push('/dashboard/categories');
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
            watchLoading();
        };

        const handleBack = () => {
            router.push('/dashboard/categories');
        };

        // Инициализация
        onMounted(async () => {
            // Загружаем список категорий
            await loadCategories();

            // Если это режим редактирования, загружаем данные категории
            if (isEditMode.value) {
                await loadCategory();
            }
        });

        return {
            category,
            categoryOptions,
            isEditMode,
            isLoading,
            isSaving,
            showDeleteModal,
            validationState,
            errors,
            toast,
            categoryNameInput,
            validateField,
            validateForm,
            updateValidationState,
            onCategorySelected,
            saveCategory,
            confirmDelete,
            cancelDelete,
            deleteCategory,
            handleBack
        };
    }
};
</script>

<style scoped>
.animate-spin {
    animation: spin 1s linear infinite;
}

@keyframes spin {
    from {
        transform: rotate(0deg);
    }

    to {
        transform: rotate(360deg);
    }
}
</style>
