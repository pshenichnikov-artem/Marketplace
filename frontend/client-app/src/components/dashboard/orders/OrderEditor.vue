<template>
    <div class="max-w-5xl mx-auto">
        <EntityEditor :title="$t('entityEditor.order.title.edit')" :back-link="'/dashboard/orders'" @back="handleBack"
            @save="saveOrder" :is-saving="isSaving">

            <template v-if="isAdmin" #actions>
                <button v-if="order.status !== 'cancelled'" @click="confirmCancelOrder"
                    class="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M6 18L18 6M6 6l12 12" />
                    </svg>
                    {{ $t('dashboard.orders.details.cancelOrder') }}
                </button>
            </template>

            <!-- Основное содержимое формы редактирования заказа -->
            <div class="space-y-6">
                <!-- Индикатор загрузки при инициализации редактора -->
                <div v-if="isLoading" class="flex justify-center items-center py-8">
                    <div class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-indigo-500"></div>
                </div>

                <!-- Форма заказа -->
                <div v-else class="flex flex-col gap-6">
                    <!-- Шапка с основной информацией о заказе -->
                    <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                        <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                viewBox="0 0 20 20" fill="currentColor">
                                <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z" />
                                <path fill-rule="evenodd"
                                    d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z"
                                    clip-rule="evenodd" />
                            </svg>
                            <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.order.orderInfo') }}</h3>
                        </div>
                        <div class="p-5">
                            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <!-- Левая колонка с информацией о заказе -->
                                <div class="space-y-4">
                                    <!-- ID заказа -->
                                    <div class="flex items-center">
                                        <div class="w-1/3 font-medium text-gray-600">
                                            {{ $t('entityEditor.order.orderId') }}:
                                        </div>
                                        <div class="w-2/3 font-bold text-gray-900">
                                            #{{ order.id }}
                                        </div>
                                    </div>

                                    <!-- Дата заказа -->
                                    <div class="flex items-center">
                                        <div class="w-1/3 font-medium text-gray-600">
                                            {{ $t('entityEditor.order.orderDate') }}:
                                        </div>
                                        <div class="w-2/3 text-gray-900">
                                            {{ formatDateSimple(order.orderDate) }}
                                        </div>
                                    </div>

                                    <div class="flex items-center">
                                        <div class="w-1/3 font-medium text-gray-600">
                                            {{ $t('entityEditor.order.status') }}:
                                        </div>
                                        <div class="w-2/3">
                                            <select v-if="isAdmin || isSeller" v-model="order.status"
                                                class="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md">
                                                <option v-for="status in availableOrderStatuses" :key="status"
                                                    :value="status">
                                                    {{ getTranslatedStatus(status) }}
                                                </option>
                                            </select>
                                            <div v-else class="px-3 py-1 text-sm font-medium rounded-full inline-block"
                                                :class="getStatusClass(order.status)">
                                                {{ getTranslatedStatus(order.status) }}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="space-y-4">
                                    <div v-if="order.user" class="flex items-center">
                                        <div class="w-1/3 font-medium text-gray-600">
                                            {{ $t('entityEditor.order.customer') }}:
                                        </div>
                                        <div class="w-2/3 text-gray-900">
                                            {{ order.user.firstName }} {{ order.user.lastName }}
                                        </div>
                                    </div>

                                    <div v-if="order.user" class="flex items-center">
                                        <div class="w-1/3 font-medium text-gray-600">
                                            {{ $t('entityEditor.order.email') }}:
                                        </div>
                                        <div class="w-2/3 text-gray-900">
                                            {{ order.user.email }}
                                        </div>
                                    </div>

                                    <div class="flex items-center">
                                        <div class="w-1/3 font-medium text-gray-600">
                                            {{ $t('entityEditor.order.payment') }}:
                                        </div>
                                        <div class="w-2/3">
                                            <select v-if="isAdmin" v-model="order.paymentStatus"
                                                class="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md">
                                                <option value="pending">{{
                                                    $t('entityEditor.order.paymentStatus.pending') }}
                                                </option>
                                                <option value="paid">{{ $t('entityEditor.order.paymentStatus.paid') }}
                                                </option>
                                                <option value="failed">{{ $t('entityEditor.order.paymentStatus.failed')
                                                    }}</option>
                                                <option value="refunded">{{
                                                    $t('entityEditor.order.paymentStatus.refunded') }}
                                                </option>
                                            </select>
                                            <span v-else-if="order.payment"
                                                class="px-3 py-1 text-sm font-medium rounded-full bg-green-100 text-green-800">
                                                {{ $t('entityEditor.order.paymentStatus.paid') }} {{
                                                    formatDate(order.payment.paymentDate) }}
                                            </span>
                                            <span v-else
                                                class="px-3 py-1 text-sm font-medium rounded-full bg-yellow-100 text-yellow-800">
                                                {{ $t('entityEditor.order.paymentStatus.pending') }}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                        <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center justify-between">
                            <div class="flex items-center">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                    viewBox="0 0 20 20" fill="currentColor">
                                    <path d="M4 3a2 2 0 100 4h12a2 2 0 100-4H4z" />
                                    <path fill-rule="evenodd"
                                        d="M3 8h14v7a2 2 0 01-2 2H5a2 2 0 01-2-2V8zm5 3a1 1 0 011-1h2a1 1 0 110 2H9a1 1 0 01-1-1z"
                                        clip-rule="evenodd" />
                                </svg>
                                <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.order.items') }}</h3>
                            </div>

                            <button v-if="isAdmin" @click="showAddProductModal = true"
                                class="text-xs bg-indigo-600 text-white px-3 py-1 rounded hover:bg-indigo-700 transition-colors">
                                {{ $t('entityEditor.order.addItem') }}
                            </button>
                        </div>
                        <div class="p-5">
                            <div class="overflow-x-auto pb-2 -mx-4 px-4">
                                <table class="min-w-full divide-y divide-gray-200">
                                    <thead class="bg-gray-50">
                                        <tr>
                                            <th scope="col"
                                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                                {{ $t('entityEditor.order.product') }}
                                            </th>
                                            <th scope="col"
                                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                                {{ $t('entityEditor.order.price') }}
                                            </th>
                                            <th scope="col"
                                                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                                {{ $t('entityEditor.order.quantity') }}
                                            </th>
                                            <th scope="col"
                                                class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                                {{ $t('entityEditor.order.subtotal') }}
                                            </th>
                                            <th v-if="isAdmin" scope="col"
                                                class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                                {{ $t('entityEditor.order.actions') }}
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody class="bg-white divide-y divide-gray-200">
                                        <tr v-for="(item, index) in orderItems" :key="index">
                                            <td class="px-6 py-4 whitespace-nowrap">
                                                <div class="flex items-center">
                                                    <div class="ml-4">
                                                        <div class="text-sm font-medium text-gray-900">
                                                            {{ item.productName || `Товар #${item.productId}` }}
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td class="px-6 py-4 whitespace-nowrap">
                                                <div v-if="isAdmin && isEditingItem === index"
                                                    class="flex items-center">
                                                    <input type="number" v-model.number="item.price" min="0" step="0.01"
                                                        class="w-24 border-gray-300 rounded-md shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-200 focus:ring-opacity-50" />
                                                </div>
                                                <div v-else class="text-sm text-gray-500">
                                                    {{ formatCurrency(item.price) }}
                                                </div>
                                            </td>
                                            <td class="px-6 py-4 whitespace-nowrap">
                                                <div v-if="isAdmin && isEditingItem === index"
                                                    class="flex items-center">
                                                    <input type="number" v-model.number="item.quantity" min="1" step="1"
                                                        class="w-16 border-gray-300 rounded-md shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-200 focus:ring-opacity-50" />
                                                </div>
                                                <div v-else class="text-sm text-gray-500">
                                                    {{ item.quantity }}
                                                </div>
                                            </td>
                                            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                                {{ formatCurrency(item.price * item.quantity) }}
                                            </td>
                                            <td v-if="isAdmin" class="px-6 py-4 whitespace-nowrap text-right text-sm">
                                                <div class="flex justify-end space-x-2">
                                                    <button v-if="isEditingItem === index" @click="saveItemEdit(index)"
                                                        class="text-indigo-600 hover:text-indigo-900">
                                                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5"
                                                            fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                            <path stroke-linecap="round" stroke-linejoin="round"
                                                                stroke-width="2" d="M5 13l4 4L19 7" />
                                                        </svg>
                                                    </button>
                                                    <button v-else @click="startEditItem(index)"
                                                        class="text-indigo-600 hover:text-indigo-900">
                                                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5"
                                                            fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                            <path stroke-linecap="round" stroke-linejoin="round"
                                                                stroke-width="2"
                                                                d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                                        </svg>
                                                    </button>
                                                    <button @click="removeOrderItem(index)"
                                                        class="text-red-600 hover:text-red-900">
                                                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5"
                                                            fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                            <path stroke-linecap="round" stroke-linejoin="round"
                                                                stroke-width="2"
                                                                d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                        </svg>
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                    <tfoot>
                                        <tr class="bg-gray-50">
                                            <td colspan="3" class="px-6 py-4 text-right font-medium text-gray-700">
                                                {{ $t('entityEditor.order.total') }}:
                                            </td>
                                            <td class="px-6 py-4 text-right font-bold text-gray-900"
                                                :colspan="isAdmin ? 2 : 1">
                                                {{ formatCurrency(orderTotal) }}
                                            </td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
                        <div class="px-5 py-3 bg-gray-50 border-b border-gray-200 flex items-center">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2 text-indigo-500"
                                viewBox="0 0 20 20" fill="currentColor">
                                <path fill-rule="evenodd"
                                    d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z"
                                    clip-rule="evenodd" />
                            </svg>
                            <h3 class="font-semibold text-gray-800">{{ $t('entityEditor.order.shippingAddress') }}</h3>
                        </div>
                        <div class="p-5">
                            <div class="grid grid-cols-1 md:grid-cols-1 gap-6">
                                <div class="space-y-4">
                                    <label class="block text-sm font-medium text-gray-700 mb-1">
                                        {{ $t('entityEditor.order.address') }}:
                                    </label>
                                    <textarea v-model="order.shippingAddress" rows="3"
                                        class="block w-full border-gray-300 rounded-md shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                                        :placeholder="$t('entityEditor.order.addressPlaceholder')"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </EntityEditor>

        <div v-if="showAddProductModal"
            class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div class="bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] flex flex-col">
                <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
                    <h3 class="text-lg font-medium text-gray-900">{{ $t('entityEditor.order.addProduct') }}</h3>
                    <button @click="showAddProductModal = false" class="text-gray-400 hover:text-gray-600">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
                <div class="p-6 flex-grow overflow-y-auto">
                    <div class="mb-4">
                        <input type="text" v-model="productSearch"
                            class="w-full border-gray-300 rounded-md shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
                            :placeholder="$t('entityEditor.order.searchProducts')" />
                    </div>
                    <div class="divide-y divide-gray-200 max-h-96 overflow-y-auto">
                        <div v-if="filteredProducts.length === 0" class="py-4 text-center text-gray-500">
                            {{ $t('entityEditor.order.noProductsFound') }}
                        </div>
                        <div v-for="product in filteredProducts" :key="product.id"
                            class="py-4 flex items-center justify-between hover:bg-gray-50 px-2 rounded cursor-pointer"
                            @click="selectProduct(product)">
                            <div class="flex items-center">
                                <div
                                    class="flex-shrink-0 h-10 w-10 bg-gray-100 rounded-md flex items-center justify-center">
                                    <img v-if="product.imageUrl" :src="product.imageUrl" :alt="product.name"
                                        class="h-10 w-10 object-cover rounded-md">
                                    <span v-else class="text-gray-500 text-xs">{{ product.id }}</span>
                                </div>
                                <div class="ml-4">
                                    <div class="text-sm font-medium text-gray-900">{{ product.name }}</div>
                                    <div class="text-sm text-gray-500">{{ formatCurrency(product.price) }}</div>
                                </div>
                            </div>
                            <div>
                                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                                    :class="product.stockQuantity > 0 ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                                    {{ product.stockQuantity > 0 ? $t('entityEditor.order.inStock', {
                                        count:
                                            product.stockQuantity
                                    }) : $t('entityEditor.order.outOfStock') }}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="px-6 py-4 border-t border-gray-200 flex justify-end">
                    <button @click="showAddProductModal = false"
                        class="px-4 py-2 text-gray-700 border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                        {{ $t('entityEditor.cancel') }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Модальное окно ввода количества товара -->
        <div v-if="showQuantityModal"
            class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div class="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
                <div class="px-6 py-4 border-b border-gray-200">
                    <h3 class="text-lg font-medium text-gray-900">{{ $t('entityEditor.order.enterQuantity') }}</h3>
                </div>
                <div class="p-6">
                    <div class="mb-4">
                        <label class="block text-sm font-medium text-gray-700 mb-1">
                            {{ $t('entityEditor.order.quantity') }}
                        </label>
                        <input type="number" v-model.number="newItemQuantity" min="1"
                            :max="selectedProduct ? selectedProduct.stockQuantity : 999"
                            class="w-full border-gray-300 rounded-md shadow-sm focus:border-indigo-500 focus:ring focus:ring-indigo-200 focus:ring-opacity-50" />
                        <p class="mt-1 text-sm text-gray-500">
                            {{ $t('entityEditor.order.maxQuantity') }}: {{ selectedProduct ?
                                selectedProduct.stockQuantity : 0
                            }}
                        </p>
                    </div>
                </div>
                <div class="px-6 py-4 border-t border-gray-200 flex justify-end space-x-3">
                    <button @click="cancelProductSelection"
                        class="px-4 py-2 text-gray-700 border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                        {{ $t('entityEditor.cancel') }}
                    </button>
                    <button @click="addProductToOrder"
                        class="px-4 py-2 bg-indigo-600 text-white rounded-md shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                        {{ $t('entityEditor.add') }}
                    </button>
                </div>
            </div>
        </div>

        <ConfirmModal v-if="showCancelModal" :title="$t('entityEditor.order.confirmCancelTitle')"
            :message="$t('entityEditor.order.confirmCancelMessage')"
            :confirm-text="$t('entityEditor.order.confirmCancel')" :cancel-text="$t('entityEditor.order.cancelAction')"
            @confirm="cancelOrder" @cancel="showCancelModal = false" />

        <Notification ref="toast" />
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import EntityEditor from '@/components/admin/EntityEditor.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import orderService from '@/services/api/orderService';
import productService from '@/services/api/productService';
import useApiRequest from '@/hooks/useApiRequest';
import { getUserRole } from '@/utils/localStorage';
import { getStatusInfo } from '@/utils/statusUtils';

export default {
    name: 'OrderEditor',
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
    emits: ['save', 'back', 'cancel'],

    setup(props) {
        const { t, locale } = useI18n();
        const toast = ref(null);
        const router = useRouter();
        const route = useRoute();

        const isAdmin = computed(() => getUserRole() === 'admin');
        const isSeller = computed(() => { return getUserRole() === 'seller' });

        const isLoading = ref(true);
        const isSaving = ref(false);
        const showCancelModal = ref(false);
        const showAddProductModal = ref(false);
        const showQuantityModal = ref(false);
        const isEditingItem = ref(null);
        const productSearch = ref('');
        const newItemQuantity = ref(1);
        const selectedProduct = ref(null);
        const allProducts = ref([]);

        const statusOptions = computed(() => [
            { label: t('dashboard.orders.filters.allStatuses'), value: '' },
            { label: t('dashboard.orders.status.pending'), value: 'pending' },
            { label: t('dashboard.orders.status.processing'), value: 'processing' },
            { label: t('dashboard.orders.status.shipped'), value: 'shipped' },
            { label: t('dashboard.orders.status.delivered'), value: 'delivered' },
            { label: t('dashboard.orders.status.completed'), value: 'completed' },
            { label: t('dashboard.orders.status.cancelled'), value: 'cancelled' }
        ]);

        const originalOrder = ref({});

        const order = reactive({
            id: null,
            orderDate: '',
            status: '',
            items: [],
            user: null,
            payment: null,
            paymentStatus: 'pending',
            shippingAddress: '',
            notes: ''
        });

        const availableOrderStatuses = ['pending', 'processing', 'shipped', 'delivered', 'completed', 'cancelled'];

        const orderItems = ref([]);

        const orderTotal = computed(() => {
            return orderItems.value.reduce((total, item) =>
                total + (item.price * item.quantity), 0);
        });

        const filteredProducts = computed(() => {

            if (!productSearch.value) return allProducts.value;

            const search = productSearch.value.toLowerCase();
            return allProducts.value.filter(product =>
                product.name.toLowerCase().includes(search) ||
                product.id.toString().includes(search)
            );
        });

        const hasChanges = computed(() => {
            if (order.status !== originalOrder.value.status) return true;
            if (order.paymentStatus !== originalOrder.value.paymentStatus) return true;
            if (order.shippingAddress !== originalOrder.value.shippingAddress) return true;
            if (order.notes !== originalOrder.value.notes) return true;

            if (orderItems.value.length !== originalOrder.value.items?.length) return true;

            for (let i = 0; i < orderItems.value.length; i++) {
                const currentItem = orderItems.value[i];
                const originalItem = originalOrder.value.items?.[i];

                if (!originalItem) return true;
                if (currentItem.productId !== originalItem.productId) return true;
                if (currentItem.quantity !== originalItem.quantity) return true;
                if (currentItem.price !== originalItem.price) return true;
            }

            return false;
        });

        const { loading, execute: executeOrderFetch } = useApiRequest();
        const { loading: savingOrder, execute: executeSaveOrder } = useApiRequest();
        const { loading: loadingProducts, execute: executeLoadProducts } = useApiRequest();
        const { loading: cancellingOrder, execute: executeCancelOrder } = useApiRequest();
        const { loading: enrichingOrder, execute: executeProductEnrichment } = useApiRequest();

        watch(savingOrder, (isSavingNow) => {
            console.log('Saving state changed:', isSavingNow);
            isSaving.value = isSavingNow;
        });

        const fetchProducts = async () => {
            if (!isAdmin.value) return;
            console.error(getUserRole())
            await executeLoadProducts(async () => {
                return await productService.getProducts(0,
                    productSearch.value || '',
                    null, null, null,
                    100,
                    null, null);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        allProducts.value = data.products || [];
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const fetchOrderDetails = async () => {
            if (!props.id) return;

            isLoading.value = true;

            await executeOrderFetch(async () => {
                return await orderService.getById(props.id);
            }, {
                onSuccess: async (data) => {
                    if (data) {
                        console.log('Order data:', data);

                        order.id = data.id;
                        order.orderDate = data.orderDate;
                        order.status = data.status || 'pending';
                        order.user = data.customer;
                        order.payment = data.payment;
                        order.paymentStatus = data.payment ? 'paid' : 'pending';
                        order.shippingAddress = data.shippingAddress || '';

                        originalOrder.value = {
                            status: data.status || 'pending',
                            paymentStatus: data.payment ? 'paid' : 'pending',
                            shippingAddress: data.shippingAddress || '',
                            notes: data.notes || '',
                            items: data.items || []
                        };


                        if (data.items && Array.isArray(data.items)) {
                            orderItems.value = data.items.map(item => ({
                                productId: item.productId,
                                orderId: item.orderId,
                                price: item.price,
                                quantity: item.quantity,
                                productName: item.product?.name || `Товар #${item.productId}`,
                                imageUrl: item.product?.imageUrl || null
                            }));


                            const needsEnrichment = orderItems.value.some(item => !item.productName || item.productName === `Товар #${item.productId}`);
                            if (needsEnrichment) {
                                await enrichOrderWithProductInfo();
                            }
                        } else {
                            orderItems.value = [];
                        }
                        isLoading.value = false;
                    }
                },
                onFinally: () => {
                    isLoading.value = false;
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const enrichOrderWithProductInfo = async () => {
            if (!orderItems.value || orderItems.value.length === 0) return;

            for (const item of orderItems.value) {
                await executeProductEnrichment(async () => {
                    return await productService.getProductById(item.productId);
                }, {
                    onSuccess: (data) => {
                        if (data) {
                            console.log(data)
                            item.productName = data.name || `Товар #${item.productId}`;
                            item.imageUrl = data.imageUrl || null;
                        }
                    },
                    onError: () => {
                        console.log(`Не удалось загрузить информацию о товаре #${item.productId}`);
                    }
                });
            }
        };

        const saveOrder = async () => {
            console.log("Is edit");
            if (isEditingItem.value !== null) {
                toast.value.info(t('entityEditor.order.finishEditingItem'));
                return;
            }

            const orderData = {
                id: order.id,
                status: order.status,
                paymentStatus: order.paymentStatus,
                shippingAddress: order.shippingAddress,
                notes: order.notes,
                items: orderItems.value.map(item => ({
                    productId: item.productId,
                    quantity: item.quantity,
                    price: item.price
                }))
            };

            await executeSaveOrder(async () => {
                return await orderService.updateOrder(order.id, orderData);
            }, {
                onSuccess: () => {
                    toast.value.success(t('entityEditor.order.orderSaved'));
                    router.push('/dashboard/orders');
                },
                onError: () => {

                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const startEditItem = (index) => {
            isEditingItem.value = index;


        };

        const saveItemEdit = (index) => {
            isEditingItem.value = null;

        };

        const removeOrderItem = (index) => {
            orderItems.value.splice(index, 1);
        };

        const selectProduct = (product) => {
            selectedProduct.value = product;
            newItemQuantity.value = 1;
            showAddProductModal.value = false;
            showQuantityModal.value = true;
        };

        const cancelProductSelection = () => {
            selectedProduct.value = null;
            showQuantityModal.value = false;
        };

        const addProductToOrder = () => {
            const product = selectedProduct.value;
            if (!product) return;

            console.log(orderItems.value)
            const existingItemIndex = orderItems.value.findIndex(item => item.productId === product.id);

            if (existingItemIndex >= 0) {
                toast.value.error(t('entityEditor.order.itemAlreadyExists'));
            } else {
                orderItems.value.push({
                    productId: product.id,
                    productName: product.name,
                    price: product.price,
                    quantity: newItemQuantity.value,
                    imageUrl: product.imageUrl
                });
                toast.value.success(t('entityEditor.order.itemAdded'));
            }

            selectedProduct.value = null;
            showQuantityModal.value = false;
        };

        const formatDate = (dateString) => {
            if (!dateString) return '';
            const date = new Date(dateString);
            return new Intl.DateTimeFormat(locale.value, {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit'
            }).format(date);
        };

        const formatDateSimple = (dateString) => {
            if (!dateString) return '';
            const date = new Date(dateString);
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            const hours = String(date.getHours()).padStart(2, '0');
            const minutes = String(date.getMinutes()).padStart(2, '0');
            const seconds = String(date.getSeconds()).padStart(2, '0');

            return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
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

        const getStatusBadgeClass = (status) => {
            const classes = {
                'pending': 'bg-yellow-100 text-yellow-800',
                'processing': 'bg-blue-100 text-blue-800',
                'shipped': 'bg-indigo-100 text-indigo-800',
                'delivered': 'bg-green-100 text-green-800',
                'cancelled': 'bg-red-100 text-red-800',
                'completed': 'bg-green-100 text-green-800'
            };
            return classes[status.toLowerCase()] || 'bg-gray-100 text-gray-800';
        };

        const getStatusClass = (status) => {
            if (!status) return 'bg-gray-100 text-gray-800';
            const statusInfo = getStatusInfo(status, 'order');
            return statusInfo.cssClass;
        };

        const getTranslatedStatus = (status) => {
            if (!status) return '';
            const statusInfo = getStatusInfo(status, 'orders');
            return t(statusInfo.translationPath);
        };

        const confirmCancelOrder = () => {
            showCancelModal.value = true;
        };

        const cancelOrder = async () => {
            await executeCancelOrder(async () => {
                return await orderService.cancelOrder(order.id);
            }, {
                onSuccess: () => {
                    order.status = 'cancelled';
                    toast.value.success(t('entityEditor.order.orderCancelled'));
                    showCancelModal.value = false;
                    router.push('/dashboard/orders');
                },
                onError: () => {
                    showCancelModal.value = false;
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const handleBack = () => {
            router.push('/dashboard/orders');
        };

        onMounted(() => {

            fetchOrderDetails();
            fetchProducts();
        });

        watch(() => props.id, (newId) => {
            if (newId) {
                fetchOrderDetails();
            }
        });

        return {
            order,
            orderItems,
            orderTotal,
            isLoading,
            isSaving,
            isAdmin,
            showCancelModal,
            showAddProductModal,
            showQuantityModal,
            availableOrderStatuses,
            isEditingItem,
            productSearch,
            newItemQuantity,
            selectedProduct,
            filteredProducts,
            hasChanges,
            toast,
            isSeller,
            formatDate,
            formatDateSimple,
            formatCurrency,
            getStatusBadgeClass,
            getStatusClass,
            getTranslatedStatus,
            startEditItem,
            saveItemEdit,
            removeOrderItem,
            selectProduct,
            cancelProductSelection,
            addProductToOrder,
            confirmCancelOrder,
            cancelOrder,
            handleBack,
            saveOrder
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
