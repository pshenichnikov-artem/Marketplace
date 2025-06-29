<template>
    <div class="max-w-5xl mx-auto">
        <!-- Используем общий компонент EntityEditor в качестве оболочки -->
        <EntityEditor
            :title="isEditMode ? $t('entityEditor.product.title.edit') : $t('entityEditor.product.title.create')"
            :is-saving="isSaving" :back-link="'/dashboard/products'" @back="handleBack" @save="saveProduct">
            <!-- Дополнительные действия в шапке -->
            <template v-if="isEditMode" #actions>
                <button @click="confirmDelete"
                    class="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    {{ $t('entityEditor.product.deleteProduct') }}
                </button>
            </template>

            <div class="space-y-6">
                <div v-if="isLoading" class="flex justify-center items-center py-8">
                    <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-indigo-500"></div>
                </div>

                <!-- Форма продукта -->
                <div v-else class="flex flex-col md:flex-row gap-6">
                    <!-- Левая колонка -->
                    <div class="md:w-7/12 space-y-6">
                        <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd"
                                        d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.product.basicInfo') }}</h3>
                            </div>
                            <div class="p-5">
                                <div class="space-y-4">
                                    <!-- Название товара -->
                                    <ValidationInput id="name" :label="$t('entityEditor.product.name')" type="text"
                                        v-model="product.name" validationRules="required" :error-messages="{
                                            required: $t('entityEditor.product.validation.nameRequired')
                                        }" :placeholder="$t('entityEditor.product.namePlaceholder')" ref="nameInput"
                                        @validateInput="updateValidationState('name', $event)" />

                                    <div v-if="isAdmin">
                                        <label class="block text-sm font-medium text-gray-700 mb-1">
                                            {{ $t('entityEditor.product.seller') }}
                                        </label>
                                        <div class="relative">
                                            <input type="text" :value="sellerName" readonly disabled
                                                class="w-full px-4 py-2 border rounded-lg bg-gray-100 text-gray-600 cursor-not-allowed" />
                                        </div>
                                    </div>

                                    <!-- Категория -->
                                    <div>
                                        <label class="block text-sm font-medium text-gray-700 mb-1">
                                            {{ $t('entityEditor.product.category') }} <span
                                                class="text-red-500">*</span>
                                        </label>
                                        <div class="relative">
                                            <FilterFieldWithDropdown :label="$t('entityEditor.product.category')"
                                                icon="category" v-model="product.categoryId" :options="categoryOptions"
                                                :placeholder="$t('entityEditor.product.selectCategory')"
                                                :no-results-text="$t('dashboard.products.filters.noResults')"
                                                @select="onCategorySelected" @blur="validateField('categoryId')" />
                                            <p v-if="errors.categoryId" class="mt-1 text-sm text-red-500">{{
                                                errors.categoryId }}</p>
                                        </div>
                                    </div>

                                    <!-- Цена и Количество в наличии -->
                                    <div class="grid grid-cols-2 gap-4">
                                        <!-- Цена -->
                                        <ValidationInput id="price" :label="$t('entityEditor.product.price')"
                                            type="number" v-model="product.price" validationRules="required"
                                            :error-messages="{
                                                required: $t('entityEditor.product.validation.priceRequired')
                                            }" :placeholder="$t('entityEditor.product.pricePlaceholder')" min="0"
                                            step="0.01" ref="priceInput"
                                            @validateInput="updateValidationState('price', $event)">
                                            <template #append>
                                                <div
                                                    class="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                                                    <span class="text-gray-500">₽</span>
                                                </div>
                                            </template>
                                        </ValidationInput>

                                        <!-- Количество в наличии -->
                                        <ValidationInput id="stockQuantity"
                                            :label="$t('entityEditor.product.stockQuantity')" type="number"
                                            v-model="product.stockQuantity" validationRules="required" :error-messages="{
                                                required: $t('entityEditor.product.validation.stockRequired')
                                            }" :placeholder="$t('entityEditor.product.stockPlaceholder')" min="0"
                                            step="1" ref="stockQuantityInput"
                                            @validateInput="updateValidationState('stockQuantity', $event)" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd"
                                        d="M4 4a2 2 0 012-2h4.586A2 2 0 0112 2.586L15.414 6A2 2 0 0116 7.414V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4zm2 6a1 1 0 011-1h6a1 1 0 110 2H7a1 1 0 01-1-1zm1 3a1 1 0 100 2h6a1 1 0 100-2H7z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.product.details') }}</h3>
                            </div>
                            <div class="p-5">
                                <ValidationInput id="description" :label="$t('entityEditor.product.description')"
                                    type="textarea" v-model="product.description" validationRules="required"
                                    :error-messages="{
                                        required: $t('entityEditor.product.validation.descriptionRequired')
                                    }" :placeholder="$t('entityEditor.product.descriptionPlaceholder')" rows="6"
                                    ref="descriptionInput"
                                    @validateInput="updateValidationState('description', $event)" />
                            </div>
                        </div>
                    </div>

                    <!-- Правая колонка -->
                    <div class="md:w-5/12">
                        <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd"
                                        d="M4 3a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V5a2 2 0 00-2-2H4zm12 12H4l4-8 3 6 2-4 3 6z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.product.images') }}</h3>
                            </div>
                            <div class="p-5">
                                <!-- Загрузка изображений -->
                                <div class="border-2 border-dashed border-gray-300 rounded-lg p-5 text-center cursor-pointer hover:border-indigo-500 transition-colors"
                                    @click="triggerFileInput" @dragover.prevent="isDragging = true"
                                    @dragleave.prevent="isDragging = false" @drop.prevent="handleFileDrop"
                                    :class="{ 'border-indigo-500 bg-indigo-50': isDragging }">
                                    <input type="file" ref="fileInput" class="hidden" accept="image/*" multiple
                                        @change="handleFileSelect" />
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10 text-gray-400 mx-auto mb-3"
                                        fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                                    </svg>
                                    <p class="text-sm text-gray-600">{{ $t('entityEditor.product.dragImagesHere') }}</p>
                                </div>

                                <!-- Отображение загруженных изображений -->
                                <div v-if="product.images && product.images.length > 0"
                                    class="mt-4 grid grid-cols-2 gap-3">
                                    <div v-for="(image, index) in product.images" :key="index"
                                        class="relative rounded-lg border overflow-hidden group h-24"
                                        :class="{ 'ring-2 ring-indigo-500': image.isPrimary }">
                                        <!-- Изображение -->
                                        <img :src="getImageUrl(image)" :alt="`Product image ${index + 1}`"
                                            class="w-full h-full object-cover" />

                                        <!-- Панель управления изображением -->
                                        <div
                                            class="absolute inset-0 bg-black bg-opacity-50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center space-x-2">
                                            <button @click="setPrimaryImage(index)"
                                                class="bg-indigo-500 text-white p-1.5 rounded hover:bg-indigo-600 transition-colors"
                                                :class="{ 'opacity-50 cursor-not-allowed': image.isPrimary }"
                                                :disabled="image.isPrimary"
                                                :title="$t('entityEditor.product.setAsMain')">
                                                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none"
                                                    viewBox="0 0 24 24" stroke="currentColor">
                                                    <path stroke-linecap="round" stroke-linejoin="round"
                                                        stroke-width="2" d="M5 13l4 4L19 7" />
                                                </svg>
                                            </button>
                                            <button @click="deleteImage(index)"
                                                class="bg-red-500 text-white p-1.5 rounded hover:bg-red-600 transition-colors"
                                                :title="$t('entityEditor.product.deleteImage')">
                                                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none"
                                                    viewBox="0 0 24 24" stroke="currentColor">
                                                    <path stroke-linecap="round" stroke-linejoin="round"
                                                        stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                                                </svg>
                                            </button>
                                        </div>

                                        <!-- Индикатор основного изображения -->
                                        <div v-if="image.isPrimary"
                                            class="absolute bottom-0 left-0 right-0 bg-indigo-500 text-white text-xs py-0.5 text-center">
                                            {{ $t('entityEditor.product.mainImage') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="mt-6 bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                            <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd"
                                        d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-11a1 1 0 10-2 0v2H7a1 1 0 100 2h2v2a1 1 0 102 0v-2h2a1 1 0 100-2h-2V7z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.product.preview') }}</h3>
                            </div>
                            <div class="p-5">
                                <div class="border rounded-lg shadow-sm overflow-hidden">
                                    <div
                                        class="aspect-w-16 aspect-h-9 bg-gray-100 flex items-center justify-center overflow-hidden">
                                        <img v-if="product.images && product.images.length > 0" :src="getPreviewImage()"
                                            alt="Product preview" class="object-contain w-full h-full" />
                                        <div v-else class="text-gray-400 flex flex-col items-center justify-center p-6">
                                            <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none"
                                                viewBox="0 0 24 24" stroke="currentColor">
                                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                    d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                            </svg>
                                            <span class="text-sm mt-2">{{ $t('entityEditor.product.noImage') }}</span>
                                        </div>
                                    </div>
                                    <div class="p-4 bg-white">
                                        <h4 class="font-medium text-gray-900 truncate">
                                            {{ product.name || $t('entityEditor.product.namePlaceholder') }}
                                        </h4>
                                        <div class="mt-1 flex items-center justify-between">
                                            <span class="text-indigo-600 font-bold">
                                                {{ product.price }}
                                            </span>
                                            <span class="text-sm text-gray-600">
                                                {{ $t('entityEditor.product.inStock', {
                                                    count: product.stockQuantity ||
                                                        0
                                                }) }}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </EntityEditor>

        <ConfirmModal v-if="showDeleteModal" :title="$t('entityEditor.product.deleteConfirm')"
            :message="$t('entityEditor.product.deleteConfirmText')"
            :confirm-text="$t('entityEditor.product.deleteConfirmButton')"
            :cancel-text="$t('entityEditor.product.deleteCancel')" @confirm="deleteProduct" @cancel="cancelDelete" />

        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, reactive, computed, watch, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import EntityEditor from '@/components/admin/EntityEditor.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import FilterFieldWithDropdown from '@/components/ui/filters/FilterFieldWithDropdown.vue';
import ValidationInput from '@/components/ui/ValidationInput.vue';
import productService from '@/services/api/productService';
import categoryService from '@/services/api/categoryService';
import userService from '@/services/api/userService';
import useApiRequest from '@/hooks/useApiRequest';
import { prepareCategoryOptions } from '@/utils/categoryUtils';
import { getUserRole } from '@/utils/localStorage';

export default {
    name: 'ProductEditor',
    components: {
        EntityEditor,
        ConfirmModal,
        Notification,
        FilterFieldWithDropdown,
        ValidationInput
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
        const fileInput = ref(null);

        const nameInput = ref(null);
        const priceInput = ref(null);
        const stockQuantityInput = ref(null);
        const descriptionInput = ref(null);

        const isLoading = ref(false);
        const isSaving = ref(false);
        const isDragging = ref(false);
        const showDeleteModal = ref(false);
        const isAdmin = computed(() => getUserRole() === 'admin');
        const sellerName = ref('Текущий продавец');
        const product = reactive({
            id: null,
            name: '',
            description: '',
            price: '',
            stockQuantity: '',
            categoryId: '',
            sellerId: null,
            images: [],
            deletedImageUrls: []
        });

        const categories = ref([]);

        const isEditMode = computed(() => !!props.id);

        const validationState = reactive({
            name: false,
            categoryId: false,
            price: false,
            stockQuantity: false,
            description: false
        });

        const errors = reactive({
            name: '',
            categoryId: '',
            price: '',
            stockQuantity: '',
            description: ''
        });

        const updateValidationState = (field, isValid) => {
            validationState[field] = isValid;
        };

        const { loading: loadingProduct, execute: executeLoadProduct } = useApiRequest();
        const { loading: loadingCategories, execute: executeLoadCategories } = useApiRequest();
        const { loading: loadingSeller, execute: executeLoadSeller } = useApiRequest();
        const { loading: savingProduct, execute: executeSaveProduct } = useApiRequest();
        const { loading: deletingProduct, execute: executeDeleteProduct } = useApiRequest();

        watch([loadingProduct, loadingCategories, loadingSeller], ([isLoadingProduct, isLoadingCategories, isLoadingSeller]) => {
            isLoading.value = isLoadingProduct || isLoadingCategories || isLoadingSeller;
        });

        watch(savingProduct, (isSavingProduct) => {
            isSaving.value = isSavingProduct;
        });

        const categoryOptions = computed(() => {
            return prepareCategoryOptions(categories.value);
        });

        const loadProduct = async () => {
            if (!props.id) return;

            await executeLoadProduct(async () => {
                return await productService.getProductById(props.id);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        product.id = data.id;
                        product.name = data.name;
                        product.description = data.description;
                        product.price = data.price;
                        product.stockQuantity = data.stockQuantity;
                        product.categoryId = data.categoryId;
                        product.sellerId = data.seller.id;
                        console.log('Seller ID:', data.seller.id);

                        if (data.seller.id) {
                            loadSellerData(data.seller.id);
                        } else {
                            sellerName.value = 'Неизвестный продавец';
                        }

                        if (data.images && data.images.length > 0) {
                            product.images = data.images.map((img, idx) => ({
                                id: img.id,
                                url: img.url,
                                isPrimary: img.isPrimary,
                                isExisting: true
                            }));
                        }

                        validateAllFields();
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const loadSellerData = async (sellerId) => {
            await executeLoadSeller(async () => {
                return await userService.getUserById(sellerId);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        const seller = data;
                        sellerName.value = `${seller.firstName} ${seller.lastName}`;
                    } else {
                        sellerName.value = 'Неизвестный продавец';
                    }
                },
                onError: () => {
                    sellerName.value = 'Неизвестный продавец';
                }
            });
        };

        const loadCategories = async () => {
            await executeLoadCategories(async () => {
                return await categoryService.getAll();
            }, {
                onSuccess: (data) => {
                    categories.value = data || [];
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const validateField = (field) => {
            switch (field) {
                case 'name':
                    errors.name = !product.name
                        ? t('entityEditor.product.validation.nameRequired')
                        : '';
                    break;

                case 'categoryId':
                    errors.categoryId = !product.categoryId
                        ? t('entityEditor.product.validation.categoryRequired')
                        : '';
                    break;

                case 'price':
                    if (!product.price) {
                        errors.price = t('entityEditor.product.validation.priceRequired');
                    } else if (Number(product.price) <= 0) {
                        errors.price = t('entityEditor.product.validation.pricePositive');
                    } else {
                        errors.price = '';
                    }
                    break;

                case 'stockQuantity':
                    if (product.stockQuantity === '') {
                        errors.stockQuantity = t('entityEditor.product.validation.stockRequired');
                    } else if (Number(product.stockQuantity) < 0) {
                        errors.stockQuantity = t('entityEditor.product.validation.stockPositive');
                    } else {
                        errors.stockQuantity = '';
                    }
                    break;

                case 'description':
                    errors.description = !product.description
                        ? t('entityEditor.product.validation.descriptionRequired')
                        : '';
                    break;
            }
        };

        const validateAllFields = () => {
            nameInput.value?.validate();
            priceInput.value?.validate();
            stockQuantityInput.value?.validate();
            descriptionInput.value?.validate();
            validateField('categoryId');
        };

        const validateForm = () => {
            validateAllFields();

            const errorMessages = [];

            if (!validationState.name) {
                errorMessages.push(t('entityEditor.product.validation.nameRequired'));
            }

            if (!validationState.categoryId && !product.categoryId) {
                errorMessages.push(t('entityEditor.product.validation.categoryRequired'));
            }

            if (!validationState.price) {
                if (!product.price) {
                    errorMessages.push(t('entityEditor.product.validation.priceRequired'));
                } else if (Number(product.price) <= 0) {
                    errorMessages.push(t('entityEditor.product.validation.pricePositive'));
                }
            }

            if (!validationState.stockQuantity) {
                if (product.stockQuantity === '') {
                    errorMessages.push(t('entityEditor.product.validation.stockRequired'));
                } else if (Number(product.stockQuantity) < 0) {
                    errorMessages.push(t('entityEditor.product.validation.stockPositive'));
                }
            }

            if (!validationState.description) {
                errorMessages.push(t('entityEditor.product.validation.descriptionRequired'));
            }

            if (errorMessages.length > 0) {
                errorMessages.forEach(message => {
                    toast.value.error(message);
                });
                return false;
            }

            return true;
        };

        const triggerFileInput = () => {
            fileInput.value.click();
        };

        const handleFileSelect = (event) => {
            const files = event.target.files;
            uploadImages(files);
        };

        const handleFileDrop = (event) => {
            isDragging.value = false;
            const files = event.dataTransfer.files;
            uploadImages(files);
        };

        const uploadImages = (files) => {
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (file.type.startsWith('image/')) {
                    const reader = new FileReader();
                    reader.onload = (e) => {
                        product.images.push({
                            file: file,
                            url: e.target.result,
                            isPrimary: product.images.length === 0,
                            isExisting: false
                        });
                    };
                    reader.readAsDataURL(file);
                }
            }
        };

        const getImageUrl = (image) => {
            return image.url;
        };

        const deleteImage = (index) => {
            const image = product.images[index];
            const wasMain = image.isPrimary;

            if (image.isExisting && image.url) {
                product.deletedImageUrls.push(image.url);
            }

            product.images.splice(index, 1);

            if (wasMain && product.images.length > 0) {
                product.images[0].isPrimary = true;
            }
        };

        const isPrimaryImage = (image) => {
            return image.isPrimary;
        };

        const setPrimaryImage = (index) => {
            product.images.forEach(img => {
                img.isPrimary = false;
            });

            // Устанавливаем флаг для выбранного изображения
            product.images[index].isPrimary = true;
        };

        const getPreviewImage = () => {
            if (!product.images || product.images.length === 0) return '';

            const primaryImage = product.images.find(img => img.isPrimary);
            if (primaryImage) return primaryImage.url;

            return product.images[0].url;
        };

        const saveProduct = async () => {
            if (!validateForm()) {
                toast.value.error(t('entityEditor.product.validation.formErrors'));
                return;
            }

            const formData =
            {
                name: product.name,
                description: product.description,
                price: product.price,
                stockQuantity: product.stockQuantity,
                categoryId: product.categoryId,
                sellerId: product.sellerId || 1,
                photos: [],
                oldPhotos: [],
                primaryPhotoIndex: 0
            };
            formData.name = product.name;
            formData.description = product.description;
            formData.price = product.price;
            formData.stockQuantity = product.stockQuantity;
            formData.categoryId = product.categoryId;

            if (product.sellerId) {
                formData.sellerId = product.sellerId;
            }
            else {
                formData.sellerId = 1;
            }

            const newImages = product.images.filter(img => !img.isExisting && img.file);
            newImages.forEach(img => {
                formData.photos.push(img.file);
            });

            const existingImages = product.images.filter(img => img.isExisting && img.url);
            existingImages.forEach(img => {
                formData.oldPhotos.push(img.url);
            });

            const allImages = [...newImages, ...existingImages];
            const primaryIndex = allImages.findIndex(img => img.isPrimary);
            formData.primaryPhotoIndex = primaryIndex >= 0 ? primaryIndex : 0;

            if (isEditMode.value) {
                formData.id = product.id;
            }
            console.log('formData', formData);
            await executeSaveProduct(async () => {
                if (isEditMode.value) {
                    return await productService.updateProduct(product.id, formData);
                } else {
                    return await productService.createProduct(formData);
                }
            }, {
                onSuccess: (response) => {
                    if (isEditMode.value) {
                        toast.value.success(t('entityEditor.product.productSaved'));
                    } else {
                        toast.value.success(t('entityEditor.product.productCreated'));
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const confirmDelete = () => {
            showDeleteModal.value = true;
        };

        const cancelDelete = () => {
            showDeleteModal.value = false;
        };

        const deleteProduct = async () => {
            await executeDeleteProduct(async () => {
                return await productService.deleteProduct(product.id);
            }, {
                onSuccess: () => {
                    toast.value.success(t('entityEditor.product.productDeleted'));
                    router.push({ name: 'dashboard-products' });
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
        };

        const onCategorySelected = (option) => {
            product.categoryId = option.value || option.id;
            validateField('categoryId');
            validationState.categoryId = !!product.categoryId;
        };

        onMounted(async () => {
            await loadCategories();

            if (isEditMode.value) {
                await loadProduct();
            }
        });

        return {
            product,
            categories,
            categoryOptions,
            isEditMode,
            isLoading,
            isSaving,
            isDragging,
            showDeleteModal,
            validationState,
            errors,
            toast,
            fileInput,
            nameInput,
            priceInput,
            stockQuantityInput,
            descriptionInput,
            triggerFileInput,
            handleFileSelect,
            handleFileDrop,
            uploadImages,
            getImageUrl,
            deleteImage,
            isPrimaryImage,
            setPrimaryImage,
            getPreviewImage,
            validateField,
            validateForm,
            updateValidationState,
            saveProduct,
            confirmDelete,
            cancelDelete,
            deleteProduct,
            onCategorySelected,
            isAdmin,
            sellerName,
            loadSellerData
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

.aspect-w-16 {
    position: relative;
    padding-bottom: 56.25%;
}

.aspect-w-16>* {
    position: absolute;
    height: 100%;
    width: 100%;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
}

.bg-white,
.rounded-lg,
.border {
    transition: all 0.2s ease-in-out;
}

.bg-white:hover {
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}
</style>
