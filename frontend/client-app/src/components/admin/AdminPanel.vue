<template>
    <div class="bg-white rounded-xl shadow-md border border-gray-200">
        <!-- Верхняя секция с фильтрами -->
        <div class="border-b border-gray-200 bg-gray-50">
            <!-- Заголовок панели -->
            <div class="px-6 py-4 bg-gradient-to-r from-indigo-50 to-blue-50 border-b border-gray-200">
                <div class="flex justify-between items-center">
                    <h2 class="text-xl font-bold text-gray-800">{{ title }}</h2>
                    <button v-if="showAddButton" @click="$emit('add-new')"
                        class="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                        </svg>
                        {{ $t('dashboard.admin.addNew') }}
                    </button>
                </div>
            </div>

            <!-- Слот для фильтров -->
            <div class="p-6">
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
                    <slot name="filters"></slot>
                </div>

                <!-- Кнопки управления фильтрами -->
                <div class="flex items-center gap-3">
                    <button @click="$emit('apply-filters')"
                        class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors duration-200 flex items-center shadow-sm hover:shadow-md">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
                        </svg>
                        {{ $t('dashboard.admin.applyFilters') }}
                    </button>
                    <button @click="$emit('reset-filters')"
                        class="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 bg-white hover:bg-gray-50 transition-colors duration-200 flex items-center">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                        </svg>
                        {{ $t('dashboard.admin.resetFilters') }}
                    </button>
                </div>
            </div>
        </div>

        <!-- Таблица данных  -->
        <div class="relative w-full">
            <div class="table-wrapper">
                <div class="max-h-[600px] overflow-y-auto table-container">
                    <table class="w-full divide-y divide-gray-200 table-fixed overflow-x-auto">
                        <!-- Заголовок таблицы -->
                        <thead class="bg-gray-50 sticky top-0 z-10">
                            <tr>
                                <!-- Заголовки столбцов -->
                                <th v-for="column in columns" :key="column.key" scope="col" :class="[
                                    'px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider',
                                    column.width || '',
                                    column.key === 'description' ? 'description-column' : '',
                                    column.class || ''
                                ]" :style="column.style || ''">
                                    <div class="flex items-center">
                                        <span>{{ column.label }}</span>
                                    </div>
                                </th>
                                <!-- Заголовок для столбца с действиями-->
                                <th scope="col"
                                    class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider actions-column sticky right-0 bg-gray-50 z-20">
                                    {{ $t('dashboard.admin.actions') }}
                                </th>
                            </tr>
                        </thead>

                        <!-- Тело таблицы -->
                        <tbody class="bg-white divide-y divide-gray-200">
                            <template v-if="items.length">
                                <tr v-for="(item, rowIndex) in items" :key="rowIndex"
                                    class="hover:bg-gray-50 transition-colors duration-150">

                                    <!-- Ячейка с данными -->
                                    <td v-for="(column, colIndex) in columns" :key="`${rowIndex}-${colIndex}`" :class="[
                                        'px-6 py-4',
                                        column.class || '',
                                        column.key === 'name' || column.key === 'description' ? 'content-cell' : ''
                                    ]" :style="column.style || ''">

                                        <slot :name="`cell-${column.key}`" :value="getItemValue(item, column.key)"
                                            :row="item">
                                            <div v-if="column.type === 'image' && getItemValue(item, column.key)">

                                                <img :src="getItemValue(item, column.key)"
                                                    class="h-10 w-10 rounded-md object-cover"
                                                    :alt="`${column.label} image`">
                                            </div>
                                            <div v-else-if="column.type === 'boolean'" class="text-center">

                                                <span v-if="getItemValue(item, column.key)"
                                                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 mr-1"
                                                        fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                        <path stroke-linecap="round" stroke-linejoin="round"
                                                            stroke-width="2" d="M5 13l4 4L19 7" />
                                                    </svg>
                                                    {{ yesText }}
                                                </span>
                                                <span v-else
                                                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 mr-1"
                                                        fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                        <path stroke-linecap="round" stroke-linejoin="round"
                                                            stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                                                    </svg>
                                                    {{ noText }}
                                                </span>
                                            </div>
                                            <div v-else-if="column.type === 'date'"
                                                :title="formatDate(getItemValue(item, column.key))">

                                                {{ formatDate(getItemValue(item, column.key)) }}
                                            </div>
                                            <div v-else-if="column.type === 'number'" class="text-right">

                                                {{ formatNumber(getItemValue(item, column.key)) }}
                                            </div>
                                            <div v-else-if="column.type === 'currency'" class="text-right">

                                                {{ formatCurrency(getItemValue(item, column.key)) }}
                                            </div>
                                            <div v-else-if="column.type === 'status'" class="text-center">

                                                <span
                                                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                                                    :class="getStatusClass(getItemValue(item, column.key), column.statusContext || 'orders')">
                                                    {{ $t(getTranslatedStatus(getItemValue(item, column.key),
                                                        column.statusContext || 'orders')) }}
                                                </span>
                                            </div>
                                            <div v-else>
                                                <div :class="column.key === 'description' ? 'line-clamp-2' : ''">
                                                    {{ getItemValue(item, column.key) }}
                                                </div>
                                            </div>
                                        </slot>
                                    </td>

                                    <!-- Столбец для действий -->
                                    <td
                                        class="px-6 py-4 text-right text-sm font-medium actions-column sticky right-0 bg-white z-20">
                                        <div class="relative inline-block text-left" ref="actionsDropdown">
                                            <button @click="toggleActions(rowIndex)" type="button"
                                                class="p-2 rounded-md hover:bg-gray-100 transition-colors">
                                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-gray-500"
                                                    viewBox="0 0 20 20" fill="currentColor">
                                                    <path
                                                        d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z" />
                                                </svg>
                                            </button>

                                            <teleport to="body">
                                                <div v-if="activeActionsRow === rowIndex"
                                                    class="fixed z-50 bg-white ring-1 ring-black ring-opacity-5 divide-y divide-gray-100 rounded-md shadow-lg focus:outline-none"
                                                    style="min-width: 12rem;" :style="getActionMenuPosition(rowIndex)"
                                                    role="menu" aria-orientation="vertical"
                                                    aria-labelledby="options-menu">
                                                    <div class="py-1">
                                                        <button @click="handleEdit(item)"
                                                            class="block w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-gray-900 text-left"
                                                            role="menuitem">
                                                            <div class="flex items-center">
                                                                <svg xmlns="http://www.w3.org/2000/svg"
                                                                    class="h-4 w-4 mr-3 text-indigo-600" fill="none"
                                                                    viewBox="0 0 24 24" stroke="currentColor">
                                                                    <path stroke-linecap="round" stroke-linejoin="round"
                                                                        stroke-width="2"
                                                                        d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                                                </svg>
                                                                {{ $t('dashboard.admin.edit') }}
                                                            </div>
                                                        </button>
                                                        <button v-if="showDeleteButton" @click="handleDelete(item)"
                                                            class="block w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 hover:text-gray-900 text-left"
                                                            role="menuitem">
                                                            <div class="flex items-center">
                                                                <svg xmlns="http://www.w3.org/2000/svg"
                                                                    class="h-4 w-4 mr-3 text-red-600" fill="none"
                                                                    viewBox="0 0 24 24" stroke="currentColor">
                                                                    <path stroke-linecap="round" stroke-linejoin="round"
                                                                        stroke-width="2"
                                                                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                                </svg>
                                                                {{ $t('dashboard.admin.delete') }}
                                                            </div>
                                                        </button>
                                                    </div>
                                                </div>
                                            </teleport>
                                        </div>
                                    </td>
                                </tr>
                            </template>

                            <!-- Состояние, когда нет данных -->
                            <template v-else>
                                <tr>
                                    <td :colspan="columns.length + 1" class="px-6 py-12 text-center">
                                        <div class="flex flex-col items-center text-gray-500">
                                            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-gray-300 mb-3"
                                                fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1"
                                                    d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                                            </svg>
                                            <p class="text-lg font-medium">{{ noDataText }}</p>
                                            <p class="text-sm mt-1">{{ noDataSubtext }}</p>
                                        </div>
                                    </td>
                                </tr>
                            </template>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Пагинация -->
        <div
            class="bg-gray-50 px-6 py-4 flex flex-col md:flex-row gap-3 justify-between items-center border-t border-gray-200">
            <BasePagination v-if="totalItems > 0" :total-items="totalItems" :current-page="currentPage"
                :page-size="pageSize" @pagination-change="$emit('pagination-change', $event)" />
        </div>
    </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useI18n } from 'vue-i18n';
import BasePagination from '@/components/ui/BasePagination.vue';
import { getStatusInfo } from '@/utils/statusUtils';

export default {
    name: 'AdminPanel',
    components: {
        BasePagination
    },
    props: {
        title: {
            type: String,
            default: 'Admin Panel'
        },
        columns: {
            type: Array,
            required: true
            // Пример структуры:
            // [
            //   { key: 'id', label: 'ID' },
            //   { key: 'name', label: 'Name' },
            //   { key: 'status', label: 'Status', type: 'status', statusContext: 'orders' },
            //   { key: 'image', label: 'Image', type: 'image' },
            //   { key: 'price', label: 'Price', type: 'currency' }
            // ]
        },
        items: {
            type: Array,
            default: () => []
        },
        totalItems: {
            type: Number,
            default: 0
        },
        currentPage: {
            type: Number,
            default: 1
        },
        pageSize: {
            type: Number,
            default: 10
        },
        noDataText: {
            type: String,
            default: 'No data available'
        },
        noDataSubtext: {
            type: String,
            default: 'Try changing your filters or add new items'
        },
        showAddButton: {
            type: Boolean,
            default: true
        },
        showDeleteButton: {
            type: Boolean,
            default: true
        }
    },
    emits: [
        'add-new',
        'edit',
        'delete',
        'apply-filters',
        'reset-filters',
        'pagination-change'
    ],
    setup(props, { emit }) {
        const { t, locale } = useI18n();
        const activeActionsRow = ref(null);
        const actionsDropdown = ref([]);

        const toggleActions = (rowIndex) => {
            activeActionsRow.value = activeActionsRow.value === rowIndex ? null : rowIndex;
        };

        const getItemValue = (item, key) => {
            if (!key || !item) return '';
            return key.split('.').reduce((obj, k) => (obj && obj[k] !== undefined ? obj[k] : ''), item);
        };

        const handleEdit = (item) => {
            emit('edit', item);
            activeActionsRow.value = null;
        };

        const handleDelete = (item) => {
            emit('delete', item);
            activeActionsRow.value = null;
        };

        function formatDate(dateValue) {
            if (!dateValue) return '';
            console.log("Изменяем дату");

            try {
                const date = new Date(dateValue);

                const pad = (n) => String(n).padStart(2, '0');

                const day = pad(date.getDate());
                const month = pad(date.getMonth() + 1);
                const year = date.getFullYear();

                const hours = pad(date.getHours());
                const minutes = pad(date.getMinutes());
                const seconds = pad(date.getSeconds());

                return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
            } catch (e) {
                return dateValue;
            }
        }

        const formatNumber = (value) => {
            if (value === null || value === undefined) return '';
            return new Intl.NumberFormat(locale.value).format(value);
        };

        const formatCurrency = (value) => {
            if (value === null || value === undefined) return '';

            try {
                const number = new Intl.NumberFormat(locale.value, {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                }).format(value);

                return `${number} ₽`;
            } catch (e) {
                return value;
            }
        };

        const getStatusClass = (status, context = 'common') => {
            if (!status) return 'bg-gray-100 text-gray-800';
            const statusInfo = getStatusInfo(status, context);
            return statusInfo.cssClass;
        };

        const getTranslatedStatus = computed(() => {

            return (status, context = 'common') => {
                console.log("Изменяем статус", status, context);
                if (!status) return '';
                const statusInfo = getStatusInfo(status, context);
                return statusInfo.translationPath;
            };
        });

        const getActionMenuPosition = (rowIndex) => {
            if (!actionsDropdown.value[rowIndex]) return { top: '0px', left: '0px' };

            const element = actionsDropdown.value[rowIndex];
            const rect = element.getBoundingClientRect();

            const viewportWidth = window.innerWidth;
            const viewportHeight = window.innerHeight;

            let top = rect.bottom + window.scrollY;
            let left = rect.right - 192;

            if (left + 192 > viewportWidth) {
                left = viewportWidth - 200;
            }

            const menuHeight = 100;
            if (top + menuHeight > viewportHeight + window.scrollY) {

                top = rect.top + window.scrollY - menuHeight;
            }

            return {
                top: `${top}px`,
                left: `${left}px`
            };
        };

        const handleClickOutside = (event) => {
            if (activeActionsRow.value !== null) {
                const currentDropdown = actionsDropdown.value[activeActionsRow.value];
                if (currentDropdown && !currentDropdown.contains(event.target)) {
                    activeActionsRow.value = null;
                }
            }
        };

        onMounted(() => {
            document.addEventListener('click', handleClickOutside);
        });

        onUnmounted(() => {
            document.removeEventListener('click', handleClickOutside);
        });

        return {
            activeActionsRow,
            actionsDropdown,
            toggleActions,
            handleEdit,
            handleDelete,
            getItemValue,
            formatDate,
            formatNumber,
            formatCurrency,
            getStatusClass,
            getTranslatedStatus,
            getActionMenuPosition,
            handleClickOutside
        };
    }
};
</script>

<style scoped>
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: scale(0.95);
    }

    to {
        opacity: 1;
        transform: scale(1);
    }
}

@keyframes fadeOut {
    from {
        opacity: 1;
        transform: scale(1);
    }

    to {
        opacity: 0;
        transform: scale(0.95);
    }
}

div[role="menu"] {
    animation: fadeIn 0.1s ease-out;
}

button svg {
    transition: transform 0.15s ease-in-out;
}

button:hover svg {
    transform: scale(1.1);
}

.hover\:scale-105:hover {
    transform: scale(1.05);
    transition: transform 0.2s ease-in-out;
}

.table-wrapper {
    width: 100%;
    overflow: hidden;
    position: relative;
    border-radius: 0.5rem;
}

.table-container {
    overflow-x: auto;
    overflow-y: auto;
    scrollbar-width: thin;
    scrollbar-color: rgba(156, 163, 175, 0.5) transparent;
    position: relative;
    width: 100%;
    max-width: 100%;
}

.table-container::-webkit-scrollbar {
    width: 8px;
    height: 8px;
}

.table-container::-webkit-scrollbar-track {
    background: rgba(243, 244, 246, 0.5);
    border-radius: 4px;
}

.table-container::-webkit-scrollbar-thumb {
    background-color: rgba(156, 163, 175, 0.5);
    border-radius: 4px;
}

table {
    width: 100%;
    table-layout: fixed;
    max-width: none;
}

th,
td {
    box-sizing: border-box;
}

thead.sticky {
    position: sticky;
    top: 0;
    z-index: 10;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.actions-column {
    position: sticky;
    right: 0;
    z-index: 20;
    width: 100px;
    min-width: 100px;
    max-width: 100px;
}

td.actions-column {
    box-shadow: -4px 0 6px -4px rgba(0, 0, 0, 0.1);
}

th.actions-column {
    box-shadow: -4px 0 6px -4px rgba(0, 0, 0, 0.1);
}

tr:hover td:not(.actions-column) {
    background-color: rgba(249, 250, 251, 1);
}

tr:hover td.actions-column {
    background-color: white;
}

table {
    table-layout: fixed;
    width: 100%;
}

table {
    min-width: 0;
}

th,
td {
    overflow: hidden;
    text-overflow: ellipsis;
}

td.content-cell {
    white-space: normal;
    max-width: 250px;
    overflow: hidden;
}

.line-clamp-2 {
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
}

.description-column {
    width: 250px;
    min-width: 250px;
    max-width: 250px;
}

.expandable-cell {
    white-space: normal;
    word-wrap: break-word;
}

:deep(th),
:deep(td) {
    white-space: normal;
}
</style>
