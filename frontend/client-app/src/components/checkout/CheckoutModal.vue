<template>
    <div class="fixed inset-0 z-50 flex items-center justify-center">
        <!-- Затемнение фона -->
        <div class="absolute inset-0 bg-black opacity-50" @click="closeModal"></div>

        <!-- Содержимое модального окна -->
        <div class="bg-white rounded-xl shadow-xl w-full max-w-3xl mx-4 z-10 overflow-auto max-h-[90vh]">
            <!-- Заголовок -->
            <div class="bg-indigo-600 text-white px-6 py-4 flex justify-between items-center sticky top-0 z-10">
                <h2 class="text-xl font-bold">{{ $t('checkout.title') }}</h2>
                <button @click="closeModal" class="text-white hover:text-gray-200 transition-colors">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </button>
            </div>

            <!-- Форма оформления заказа -->
            <div class="p-6">
                <form @submit.prevent="submitOrder">
                    <!-- Адрес доставки -->
                    <div class="mb-6">
                        <h3 class="text-lg font-semibold text-gray-800 mb-3">{{ $t('checkout.deliveryAddress') }}</h3>

                        <!-- Поле поиска адреса -->
                        <div class="mb-4 relative">
                            <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('checkout.searchAddress')
                                }}</label>
                            <input type="text" ref="addressInput" v-model="searchQuery"
                                :placeholder="$t('checkout.enterAddress')"
                                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
                                @input="handleSearchInput" @keydown.down.prevent="navigateSuggestion(1)"
                                @keydown.up.prevent="navigateSuggestion(-1)" @keydown.enter.prevent="selectSuggestion"
                                @blur="handleBlur" @focus="handleFocus" autocomplete="off">

                            <!-- Индикатор загрузки подсказок -->
                            <div v-if="isLoadingSuggestions" class="absolute right-3 top-8 flex items-center">
                                <div
                                    class="w-5 h-5 border-2 border-gray-300 border-t-indigo-600 rounded-full animate-spin">
                                </div>
                            </div>

                            <!-- Выпадающий список с подсказками адресов -->
                            <div v-if="showSuggestions && suggestions.length > 0"
                                class="absolute left-0 right-0 mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-60 overflow-y-auto z-20">
                                <div v-for="(suggestion, index) in suggestions" :key="index"
                                    :class="['px-3 py-2 cursor-pointer hover:bg-gray-100', { 'bg-indigo-50': index === selectedSuggestionIndex }]"
                                    @mousedown.prevent="selectAddress(suggestion)"
                                    @mouseover="selectedSuggestionIndex = index">
                                    {{ suggestion.title }}
                                </div>
                            </div>
                        </div>

                        <!-- Карта для выбора места доставки -->
                        <div class="w-full h-64 bg-gray-200 rounded-lg mb-4 relative overflow-hidden">
                            <div id="map-container" class="w-full h-full">
                                <component v-if="mapComponentsLoaded" :is="mapComponents.YMap" :location="mapLocation"
                                    :margin="mapMargin" ref="mapRef">
                                    <component :is="mapComponents.YMapDefaultSchemeLayer" />
                                    <component :is="mapComponents.YMapDefaultFeaturesLayer" />

                                    <!-- Маркер по умолчанию отображаем всегда -->
                                    <component v-if="markerCoordinates" :is="mapComponents.YMapDefaultMarker"
                                        :coordinates="markerCoordinates" title="Адрес доставки" :draggable="true"
                                        @dragend="onMarkerDragEnd" />
                                </component>
                            </div>

                            <!-- Индикатор загрузки карты -->
                            <div v-if="isMapLoading"
                                class="absolute inset-0 flex items-center justify-center bg-white bg-opacity-80">
                                <div
                                    class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500">
                                </div>
                            </div>
                        </div>

                        <!-- Выбранный адрес -->
                        <div v-if="orderData.address" class="p-3 bg-indigo-50 rounded-lg border border-indigo-100">
                            <p class="font-medium text-indigo-800">{{ orderData.address }}</p>
                            <p class="text-sm text-gray-600"
                                v-if="orderData.coordinates && orderData.coordinates.length === 2">
                                {{ $t('checkout.coordinates') }}: {{ orderData.coordinates[0].toFixed(6) }}, {{
                                    orderData.coordinates[1].toFixed(6) }}
                            </p>
                        </div>

                        <!-- Сообщение об ошибке адреса -->
                        <div v-if="addressError" class="mt-2 text-red-600 text-sm">
                            {{ addressError }}
                        </div>
                    </div>

                    <!-- Комментарий к заказу -->
                    <div class="mb-6">
                        <label class="block text-sm font-medium text-gray-700 mb-1">{{ $t('checkout.comment') }}</label>
                        <textarea v-model="orderData.comment" :placeholder="$t('checkout.enterComment')"
                            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 min-h-[80px]"></textarea>
                    </div>

                    <!-- Кнопки управления -->
                    <div class="flex justify-end space-x-3 mt-8">
                        <button type="button" @click="closeModal"
                            class="px-4 py-2 bg-gray-200 text-gray-800 rounded-md hover:bg-gray-300 transition-colors">
                            {{ $t('common.cancel') }}
                        </button>
                        <button type="submit"
                            class="px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 transition-colors flex items-center"
                            :disabled="isSubmitting">
                            <span v-if="isSubmitting"
                                class="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></span>
                            {{ $t('checkout.placeOrder') }}
                        </button>
                    </div>
                </form>
            </div>

            <!-- Добавляем компонент уведомлений -->
            <ToastNotification ref="toast" />
        </div>
    </div>
</template>

<script>
import { ymaps3Service } from './lib/ymaps3';
import { ref, onMounted, onBeforeUnmount } from 'vue';
import Notification from '@/components/ui/Notification.vue';

export default {
    name: 'CheckoutModal',
    components: {
        Notification
    },
    props: {
        totalAmount: {
            type: Number,
            required: true
        },
        items: {
            type: Array,
            required: true
        }
    },
    emits: ['close', 'order-submitted'],
    data() {
        return {
            // Данные для заказа
            orderData: {
                address: '',
                coordinates: [],
                comment: '',
                addressDetails: null
            },
            searchQuery: '',
            suggestions: [],
            selectedSuggestionIndex: -1,
            showSuggestions: false,
            isLoadingSuggestions: false,
            suggestionsTimeout: null,
            isMapLoading: true,
            isSubmitting: false,
            mapComponentsLoaded: false,

            // Данные для карты - изменяем координаты на координаты Москвы
            mapLocation: {
                center: [37.618423, 55.751244], // Москва (долгота, широта)
                zoom: 10,
                duration: 400
            },
            mapMargin: [20, 20, 20, 20],
            markerCoordinates: [37.618423, 55.751244], // Координаты Москвы
            mapInstance: null,

            // Новое поле для ошибки адреса
            addressError: '',
        };
    },
    computed: {
        mapComponents() {
            return ymaps3Service.vueComponents || {};
        }
    },
    mounted() {
        this.initializeMap();
        document.addEventListener('click', this.handleClickOutside);

        // Добавляем слушатель клика на карту для установки маркера
        document.addEventListener('map-click', this.handleMapClick);
    },
    beforeUnmount() {
        document.removeEventListener('click', this.handleClickOutside);
        document.removeEventListener('map-click', this.handleMapClick);
        if (this.suggestionsTimeout) {
            clearTimeout(this.suggestionsTimeout);
        }
    },
    methods: {
        async initializeMap() {
            try {
                this.isMapLoading = true;

                await ymaps3Service.loadApi();
                this.mapComponentsLoaded = true;

                // Устанавливаем задержку для гарантии, что карта загрузится
                setTimeout(() => {
                    this.setupMapClick();
                    this.isMapLoading = false;
                }, 500);
            } catch (error) {
                console.error('Ошибка при инициализации карты:', error);
                this.isMapLoading = false;
            }
        },

        setupMapClick() {
            const mapContainer = document.getElementById('map-container');
            if (!mapContainer) return;

            mapContainer.addEventListener('click', (event) => {
                if (this.$refs.mapRef && this.$refs.mapRef.entity) {
                    const map = this.$refs.mapRef.entity;

                    // Определяем координаты клика и проверяем, что это действительно клик по карте
                    const rect = mapContainer.getBoundingClientRect();
                    const x = event.clientX - rect.left;
                    const y = event.clientY - rect.top;

                    // Если клик внутри контейнера карты
                    if (x >= 0 && x <= rect.width && y >= 0 && y <= rect.height) {
                        // Создаем пользовательское событие для обработки клика
                        const mapClickEvent = new CustomEvent('map-click', {
                            detail: { x, y }
                        });
                        document.dispatchEvent(mapClickEvent);
                    }
                }
            });
        },

        async handleMapClick(event) {
            if (!this.$refs.mapRef || !this.$refs.mapRef.entity) return;

            try {
                const map = this.$refs.mapRef.entity;

                // Получаем текущие координаты центра карты как приблизительные координаты клика
                const center = map.center;
                if (center && center.length === 2) {
                    // Обновляем координаты маркера
                    this.markerCoordinates = center;
                    this.orderData.coordinates = center;

                    // Выполняем обратное геокодирование для получения адреса
                    await this.getAddressByCoordinates(center);
                }
            } catch (error) {
                console.error('Ошибка при обработке клика по карте:', error);
            }
        },

        async onMarkerDragEnd(event) {
            try {
                // Получаем новые координаты маркера после перетаскивания
                if (event && event.target && event.target._coordinates) {
                    const coordinates = event.target._coordinates;
                    this.markerCoordinates = coordinates;
                    this.orderData.coordinates = coordinates;

                    // Выполняем обратное геокодирование для обновления адреса
                    await this.getAddressByCoordinates(coordinates);
                }
            } catch (error) {
                console.error('Ошибка при обработке перетаскивания маркера:', error);
            }
        },

        async getAddressByCoordinates(coordinates) {
            try {
                const result = await ymaps3Service.reverseGeocode(coordinates);
                if (result) {
                    // Обновляем адрес в обоих местах
                    this.orderData.address = result.address;
                    this.searchQuery = result.address;

                    // Сохраняем детали адреса
                    this.orderData.addressDetails = result.details;
                }
            } catch (error) {
                console.error('Ошибка при получении адреса по координатам:', error);
            }
        },

        // Обработчик ввода в поле поиска
        handleSearchInput() {
            if (this.suggestionsTimeout) {
                clearTimeout(this.suggestionsTimeout);
            }

            this.showSuggestions = true;
            this.selectedSuggestionIndex = -1;

            if (!this.searchQuery.trim()) {
                this.suggestions = [];
                this.isLoadingSuggestions = false;
                return;
            }

            this.isLoadingSuggestions = true;
            this.suggestionsTimeout = setTimeout(() => {
                this.fetchSuggestions();
            }, 300);
        },

        // Получение подсказок для адреса
        async fetchSuggestions() {
            try {
                const results = await ymaps3Service.getSuggestions(this.searchQuery);
                this.suggestions = results;
                this.selectedSuggestionIndex = this.suggestions.length > 0 ? 0 : -1;
            } catch (error) {
                console.error('Ошибка при получении подсказок адресов:', error);
                this.suggestions = [];
            } finally {
                this.isLoadingSuggestions = false;
            }
        },

        // Навигация по подсказкам клавишами вверх/вниз
        navigateSuggestion(direction) {
            if (this.suggestions.length === 0) return;

            const newIndex = this.selectedSuggestionIndex + direction;
            if (newIndex >= -1 && newIndex < this.suggestions.length) {
                this.selectedSuggestionIndex = newIndex;
            }
        },

        // Выбор подсказки по нажатию Enter
        selectSuggestion() {
            if (this.selectedSuggestionIndex >= 0 && this.suggestions[this.selectedSuggestionIndex]) {
                this.selectAddress(this.suggestions[this.selectedSuggestionIndex]);
            }
        },

        // Обработчики для скрытия/показа списка подсказок
        handleClickOutside(event) {
            const addressInput = this.$refs.addressInput;
            if (addressInput && !addressInput.contains(event.target)) {
                this.showSuggestions = false;
            }
        },

        handleBlur() {
            setTimeout(() => {
                this.showSuggestions = false;
            }, 200);
        },

        handleFocus() {
            if (this.searchQuery && this.suggestions.length > 0) {
                this.showSuggestions = true;
            }
        },

        // Выбор адреса из подсказок
        selectAddress(suggestion) {
            if (!suggestion || !suggestion.title) return;

            // Обновляем значения поля ввода и адрес в данных заказа
            this.searchQuery = suggestion.title;
            this.orderData.address = suggestion.title;
            this.showSuggestions = false;
            this.suggestions = [];

            // Если в подсказке есть координаты, используем их
            if (suggestion.coordinates) {
                const coords = suggestion.coordinates;
                this.orderData.coordinates = coords;
                this.markerCoordinates = coords;

                // Обновляем местоположение карты
                this.mapLocation = {
                    center: coords,
                    zoom: 15,
                    duration: 400
                };
            } else {
                // Иначе ищем координаты по адресу через геокодер
                this.findCoordinatesByAddress(suggestion.title);
            }
        },

        // Поиск координат по адресу
        async findCoordinatesByAddress(address) {
            try {
                const coords = await ymaps3Service.geocode(address);
                if (coords) {
                    this.orderData.coordinates = coords;
                    this.markerCoordinates = coords;

                    // Обновляем местоположение карты
                    this.mapLocation = {
                        center: coords,
                        zoom: 15,
                        duration: 400
                    };
                }
            } catch (error) {
                console.error('Ошибка при поиске координат по адресу:', error);
            }
        },

        // Отправка заказа
        submitOrder() {
            this.addressError = ''; // Сбрасываем ошибку адреса

            if (!this.orderData.address) {
                this.addressError = this.$t('checkout.pleaseSpecifyAddress');
                // Показываем ошибку через отдельный компонент
                this.$refs.toast.error(this.$t('checkout.pleaseSpecifyAddress'));
                return;
            }

            // Проверяем валидность координат
            if (!this.orderData.coordinates || !Array.isArray(this.orderData.coordinates) || this.orderData.coordinates.length !== 2) {
                this.addressError = this.$t('checkout.invalidCoordinates');
                // Показываем ошибку через отдельный компонент
                this.$refs.toast.error(this.$t('checkout.invalidCoordinates'));
                return;
            }

            this.isSubmitting = true;

            // Собираем данные для отправки
            const orderDetails = {
                address: this.orderData.address,
                coordinates: this.orderData.coordinates,
                comment: this.orderData.comment || '',
                addressDetails: this.orderData.addressDetails || {},
                items: this.items,
                totalAmount: this.totalAmount
            };

            // Эмулируем отправку заказа с таймаутом
            setTimeout(() => {
                this.isSubmitting = false;
                this.$emit('order-submitted', orderDetails);
            }, 1000);
        },

        closeModal() {
            this.$emit('close');
        }
    }
};
</script>

<style scoped>
/* Стили для анимации */
@keyframes spin {
    0% {
        transform: rotate(0deg);
    }

    100% {
        transform: rotate(360deg);
    }
}

/* Стили для выпадающего списка с подсказками */
.absolute.left-0.right-0 {
    max-height: 200px;
    overflow-y: auto;
    z-index: 50;
}

.absolute.left-0.right-0 div {
    transition: background-color 0.15s ease;
}

/* Фиксы для корректной работы карты */
#map-container {
    width: 100%;
    height: 100%;
    overflow: hidden;
    position: relative;
}
</style>
