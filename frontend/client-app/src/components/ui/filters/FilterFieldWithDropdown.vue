<template>
    <div class="w-full relative group">
        <label v-if="label" class="block text-sm font-semibold text-gray-700 mb-2 flex items-center">
            <!-- Иконка (если предоставлена) -->
            <slot name="icon">
                <svg v-if="icon === 'category'" xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-indigo-500"
                    fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                </svg>
                <svg v-else-if="icon === 'sort'" xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 text-indigo-500"
                    fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M3 4h13M3 8h9m-9 4h6m4 0l4-4m0 0l4 4m-4-4v12" />
                </svg>
            </slot>
            {{ label }}
        </label>
        <div class="relative">
            <div @click="toggleDropdown"
                class="w-full border border-gray-300 rounded-lg px-3 py-2.5 bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-400 transition-all cursor-pointer hover:bg-white hover:border-indigo-300 flex items-center justify-between group-hover:shadow-sm">
                <span class="truncate" :class="{ 'text-gray-500': !selectedValue }">
                    {{ selectedLabel }}
                </span>
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-gray-400 transition-transform duration-300"
                    :class="{ 'transform rotate-180': isDropdownOpen }" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
            </div>

            <Dropdown :options="options" :search="searchQuery" :model-value="modelValue"
                :select-only-leaf="selectOnlyLeaf" :is-open="isDropdownOpen" :no-results-text="noResultsText"
                @update:model-value="onOptionSelect" @update:is-open="isDropdownOpen = $event" ref="dropdownRef" />

        </div>
    </div>
</template>

<script>
import { ref, computed } from 'vue';
import Dropdown from '@/components/ui/dropdown/Dropdown.vue';

export default {
    name: 'FilterFieldWithDropdown',
    components: {
        Dropdown
    },
    props: {
        modelValue: {
            type: [String, Number, Object, null],
            default: null
        },
        options: {
            type: Array,
            required: true
        },
        label: {
            type: String,
            default: ''
        },
        icon: {
            type: String,
            default: ''
        },
        placeholder: {
            type: String,
            default: ''
        },
        selectOnlyLeaf: {
            type: Boolean,
            default: true
        },
        noResultsText: {
            type: String,
            default: 'No results found'
        }
    },
    emits: ['update:modelValue', 'select'],
    setup(props, { emit }) {
        const isDropdownOpen = ref(false);
        const searchQuery = ref('');
        const dropdownRef = ref(null);

        // Вычисляем текст выбранного элемента
        const selectedLabel = computed(() => {
            if (!props.modelValue) return props.placeholder || 'Select...';

            const findLabel = (options) => {
                for (const opt of options) {
                    if ((opt.id !== undefined && opt.id === props.modelValue) ||
                        (opt.value !== undefined && opt.value === props.modelValue)) {
                        return opt.label;
                    }

                    if (opt.children && opt.children.length) {
                        const childLabel = findLabel(opt.children);
                        if (childLabel) return childLabel;
                    }
                }
                return null;
            };

            return findLabel(props.options) || props.placeholder || 'Select...';
        });

        const selectedValue = computed(() => props.modelValue);

        function toggleDropdown() {
            isDropdownOpen.value = !isDropdownOpen.value;
        }

        function onOptionSelect(value) {
            const option = findOptionById(props.options, value);
            emit('update:modelValue', value);
            emit('select', option);
            isDropdownOpen.value = false;
        }

        function findOptionById(options, id) {
            for (const opt of options) {
                if ((opt.id !== undefined && opt.id === id) ||
                    (opt.value !== undefined && opt.value === id)) {
                    return opt;
                }

                if (opt.children && opt.children.length) {
                    const childOption = findOptionById(opt.children, id);
                    if (childOption) return childOption;
                }
            }
            return null;
        }

        return {
            isDropdownOpen,
            searchQuery,
            selectedLabel,
            selectedValue,
            dropdownRef,
            toggleDropdown,
            onOptionSelect
        };
    }
};
</script>

<style scoped>
.group:hover label {
    color: #4f46e5;
}

@keyframes fadeIn {
    from {
        opacity: 0;
    }

    to {
        opacity: 1;
    }
}

.opacity-100 {
    animation: fadeIn 0.3s ease-in-out;
}
</style>
