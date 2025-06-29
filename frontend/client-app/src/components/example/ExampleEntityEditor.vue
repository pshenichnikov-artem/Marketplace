<template>
    <div class="space-y-6">
        <EntityEditor :title="isEditing ? 'Редактирование товара' : 'Создание товара'" :is-saving="isSaving"
            @save="saveProduct" @back="handleBack">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Название товара -->
                <div class="form-group">
                    <label class="block text-sm font-medium text-gray-700 mb-1">Название</label>
                    <input type="text" v-model="product.name"
                        class="w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-500 focus:ring-opacity-50"
                        placeholder="Введите название товара" />
                </div>

                <!-- Цена -->
                <div class="form-group">
                    <label class="block text-sm font-medium text-gray-700 mb-1">Цена</label>
                    <input type="number" v-model="product.price"
                        class="w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-500 focus:ring-opacity-50"
                        placeholder="0" />
                </div>

                <!-- Описание -->
                <div class="form-group md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-1">Описание</label>
                    <textarea v-model="product.description" rows="4"
                        class="w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-500 focus:ring-opacity-50"
                        placeholder="Описание товара"></textarea>
                </div>

                <!-- Количество в наличии -->
                <div class="form-group">
                    <label class="block text-sm font-medium text-gray-700 mb-1">Количество в наличии</label>
                    <input type="number" v-model="product.stockQuantity"
                        class="w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-500 focus:ring-opacity-50"
                        placeholder="0" />
                </div>

                <!-- Категория -->
                <div class="form-group">
                    <label class="block text-sm font-medium text-gray-700 mb-1">Категория</label>
                    <select v-model="product.categoryId"
                        class="w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-500 focus:ring-opacity-50">
                        <option v-for="category in categories" :key="category.id" :value="category.id">
                            {{ category.name }}
                        </option>
                    </select>
                </div>
            </div>

            <!-- Дополнительные действия в заголовке -->
            <template #actions>
                <button v-if="isEditing" @click="confirmDelete"
                    class="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    Удалить
                </button>
            </template>
        </EntityEditor>

        <!-- Модальное окно подтверждения удаления -->
        <ConfirmModal v-if="showDeleteModal" title="Подтверждение удаления"
            message="Вы действительно хотите удалить этот товар? Это действие нельзя отменить." confirm-text="Удалить"
            cancel-text="Отмена" @confirm="deleteProduct" @cancel="showDeleteModal = false" />

        <!-- Уведомления -->
        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue';
import EntityEditor from '@/components/admin/EntityEditor.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'ExampleEntityEditor',
    components: {
        EntityEditor,
        ConfirmModal,
        Notification
    },
    props: {
        id: {
            type: [String, Number],
            default: null
        }
    },
    setup(props, { emit }) {
        const toast = ref(null);
        const showDeleteModal = ref(false);

        // Состояние продукта
        const product = reactive({
            id: null,
            name: '',
            description: '',
            price: 0,
            stockQuantity: 0,
            categoryId: null
        });

        // Состояние редактирования
        const isEditing = computed(() => !!props.id);

        // API запросы
        const { loading: loadingProduct, execute: executeLoadProduct } = useApiRequest();
        const { loading: savingProduct, execute: executeSaveProduct } = useApiRequest();
        const { loading: deletingProduct, execute: executeDeleteProduct } = useApiRequest();

        // Вычисляемое свойство isSaving
        const isSaving = computed(() => savingProduct.value);

        // Демо-данные
        const categories = ref([
            { id: 1, name: 'Электроника' },
            { id: 2, name: 'Одежда' },
            { id: 3, name: 'Книги' },
            { id: 4, name: 'Спорт' }
        ]);

        // Загрузка данных продукта при редактировании
        const loadProduct = async () => {
            if (!props.id) return;

            await executeLoadProduct(async () => {
                // Здесь был бы реальный API запрос
                // Например: await productService.getProductById(props.id);

                // Для примера возвращаем тестовые данные
                return {
                    status: 'success',
                    data: {
                        id: props.id,
                        name: 'Тестовый продукт',
                        description: 'Описание тестового продукта',
                        price: 1000,
                        stockQuantity: 10,
                        categoryId: 1
                    }
                };
            }, {
                onSuccess: (response) => {
                    Object.assign(product, response.data);
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        // Сохранение продукта
        const saveProduct = async () => {
            await executeSaveProduct(async () => {
                // Здесь был бы реальный API запрос
                // Например: await productService.save(product);

                // Имитация задержки запроса
                await new Promise(resolve => setTimeout(resolve, 1000));

                return {
                    status: 'success',
                    data: { ...product, id: product.id || Date.now() }
                };
            }, {
                onSuccess: (response) => {
                    product.id = response.data.id;
                    toast.value.success('Продукт успешно сохранен');
                    emit('saved', response.data);
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        // Подтверждение удаления
        const confirmDelete = () => {
            showDeleteModal.value = true;
        };

        // Удаление продукта
        const deleteProduct = async () => {
            await executeDeleteProduct(async () => {
                // Здесь был бы реальный API запрос
                // Например: await productService.delete(product.id);

                // Имитация задержки запроса
                await new Promise(resolve => setTimeout(resolve, 1000));

                return {
                    status: 'success'
                };
            }, {
                onSuccess: () => {
                    showDeleteModal.value = false;
                    toast.value.success('Продукт успешно удален');
                    emit('deleted', product.id);
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        // Обработка возврата назад
        const handleBack = (prevState) => {
            emit('cancel');
        };

        // Загрузка данных при монтировании компонента
        onMounted(() => {
            if (props.id) {
                loadProduct();
            }
        });

        return {
            product,
            categories,
            isEditing,
            isSaving,
            toast,
            showDeleteModal,
            saveProduct,
            confirmDelete,
            deleteProduct,
            handleBack
        };
    }
};
</script>
