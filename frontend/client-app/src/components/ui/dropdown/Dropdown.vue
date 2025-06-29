<template>
  <Teleport to="body">
    <Transition name="dropdown">
      <div class="fixed z-50 w-auto overflow-y-auto dropdown-container" :style="dropdownStyle" ref="dropdown"
        v-if="isOpenComputed" role="listbox" :aria-expanded="isOpenComputed.toString()"
        :aria-activedescendant="highlightedIndex >= 0 ? `option-${highlightedIndex}` : undefined">
        <ul class="py-1" v-if="filteredOptions.length > 0">
          <DropdownOption v-for="(option, idx) in filteredOptions" :key="option.id ?? option.value ?? idx"
            :option="option" :highlighted="highlightedIndex === idx" :select-only-leaf="selectOnlyLeaf"
            :option-id="`option-${idx}`" @select="selectOption" @hover="highlightedIndex = idx"
            :aria-selected="highlightedIndex === idx" />
        </ul>
        <div v-else class="py-4 px-3 text-sm text-gray-500 text-center empty-results">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 mx-auto mb-2 text-gray-400" fill="none"
            viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          {{ noResultsText }}
        </div>
      </div>
    </Transition>
  </Teleport>
  <!-- Невидимый элемент для позиционирования -->
  <div ref="anchor" style="display: none;"></div>
</template>

<script>
import { ref, computed, watch, onMounted, onBeforeUnmount, nextTick } from 'vue';
import DropdownOption from './DropdownOption.vue';
import { useI18n } from 'vue-i18n';

export default {
  components: { DropdownOption },
  props: {
    options: {
      type: Array,
      required: true
    },
    modelValue: {
      type: [String, Number, Object, null],
      default: null
    },
    search: {
      type: String,
      default: ''
    },
    selectOnlyLeaf: {
      type: Boolean,
      default: false
    },
    isOpen: {
      type: Boolean,
      default: false
    },
    noResultsText: {
      type: String,
      default: null
    }
  },
  emits: ['update:modelValue', 'select', 'update:isOpen'],
  setup(props, { emit }) {
    const { t } = useI18n();
    const dropdown = ref(null);
    const anchor = ref(null);
    const dropdownPosition = ref({ top: 0, left: 0, width: 0 });
    const highlightedIndex = ref(0);
    const isOpenInternal = ref(false);
    let dropdownTimer = null;
    let closeTimer = null;

    // Используем вычисляемое свойство для объединения внутреннего состояния и пропса
    const isOpenComputed = computed(() => props.isOpen || isOpenInternal.value);

    const filteredOptions = computed(() => {
      if (!props.search?.trim()) return props.options || [];

      const filterRecursive = (opts) =>
        opts
          .map((opt) => {
            if (opt.children?.length) {
              const filteredChildren = filterRecursive(opt.children);
              if (filteredChildren.length)
                return { ...opt, children: filteredChildren };
            }
            if (opt.label.toLowerCase().includes(props.search.toLowerCase())) {
              return { ...opt, children: opt.children || [] };
            }
            return null;
          })
          .filter(Boolean);

      return filterRecursive(props.options || []);
    });

    const noResultsMessage = computed(() =>
      props.noResultsText || t('productFilter.noResults')
    );

    function open() {
      isOpenInternal.value = true;
      emit('update:isOpen', true);
      highlightedIndex.value = 0;

      // Обновляем позицию при открытии
      nextTick(() => {
        updateDropdownPosition();
      });
    }

    function close() {
      isOpenInternal.value = false;
      emit('update:isOpen', false);
    }

    function moveDown() {
      if (highlightedIndex.value < filteredOptions.value.length - 1) highlightedIndex.value++;
    }

    function moveUp() {
      if (highlightedIndex.value > 0) highlightedIndex.value--;
    }

    function selectHighlighted() {
      const option = filteredOptions.value[highlightedIndex.value];
      if (option) selectOption(option);
    }

    function selectOption(option) {
      if (props.selectOnlyLeaf && option.children?.length) return;
      // Исправляем передачу значения при выборе опции
      const value = option.id !== undefined ? option.id : (option.value !== undefined ? option.value : null);
      emit('update:modelValue', value);
      emit('select', option);
      close();
    }

    function onClickOutside(event) {
      // Отменяем предыдущий таймер, если он был установлен
      clearTimeout(closeTimer);

      // Проверяем, содержится ли клик в основном дропдауне
      const isInMainDropdown = dropdown.value && dropdown.value.contains(event.target);

      // Проверяем, содержится ли клик в каком-либо подменю
      const isInSubmenu = document.querySelector('.fixed.z-50')?.contains(event.target);

      // Проверяем, является ли элемент, по которому кликнули, частью DropdownOption
      const isInDropdownOption = event.target.closest('.dropdown-option');

      if (!isInMainDropdown && !isInSubmenu && !isInDropdownOption) {
        // Устанавливаем небольшую задержку перед закрытием
        closeTimer = setTimeout(() => {
          close();
        }, 50);
      }
    }

    // Функция для обновления позиции выпадающего списка
    function updateDropdownPosition() {
      if (!anchor.value) return;

      const anchorRect = anchor.value.getBoundingClientRect();
      const parentElement = anchor.value.parentElement;
      const parentRect = parentElement.getBoundingClientRect();

      dropdownPosition.value = {
        top: parentRect.bottom + window.scrollY,
        left: parentRect.left + window.scrollX,
        width: parentRect.width
      };
    }

    // Вычисляемое свойство для стилей выпадающего списка
    const dropdownStyle = computed(() => {
      return {
        top: `${dropdownPosition.value.top}px`,
        left: `${dropdownPosition.value.left}px`,
        width: `${dropdownPosition.value.width}px`,
        maxHeight: '300px',
        backgroundColor: 'white',
        borderRadius: '0.5rem',
        boxShadow: '0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
        border: '1px solid rgba(229, 231, 235, 0.8)'
      };
    });

    // Слушаем события прокрутки и изменения размеров
    function setupEventListeners() {
      window.addEventListener('scroll', updateDropdownPosition);
      window.addEventListener('resize', updateDropdownPosition);
    }

    function cleanupEventListeners() {
      window.removeEventListener('scroll', updateDropdownPosition);
      window.removeEventListener('resize', updateDropdownPosition);
    }

    onMounted(() => {
      document.addEventListener('mousedown', onClickOutside);
      setupEventListeners();
      if (props.isOpen) {
        open();
      }
    });

    onBeforeUnmount(() => {
      document.removeEventListener('mousedown', onClickOutside);
      cleanupEventListeners();
      clearTimeout(dropdownTimer);
      clearTimeout(closeTimer);
    });

    // Следим за изменениями пропса isOpen
    watch(() => props.isOpen, (newVal) => {
      if (newVal) {
        open();
      } else {
        close();
      }
    });

    return {
      dropdown,
      anchor,
      dropdownStyle,
      highlightedIndex,
      isOpenComputed,
      filteredOptions,
      noResultsText: noResultsMessage,
      open,
      close,
      moveDown,
      moveUp,
      selectHighlighted,
      selectOption
    };
  }
};
</script>

<style scoped>
ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.dropdown-container {
  backdrop-filter: blur(8px);
  scrollbar-width: thin;
  scrollbar-color: #d1d5db #f3f4f6;
}

.dropdown-container::-webkit-scrollbar {
  width: 6px;
}

.dropdown-container::-webkit-scrollbar-track {
  background: #f3f4f6;
  border-radius: 4px;
}

.dropdown-container::-webkit-scrollbar-thumb {
  background-color: #d1d5db;
  border-radius: 4px;
}

/* Анимация появления/исчезновения */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: opacity 0.2s, transform 0.2s;
  transform-origin: top;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px) scaleY(0.95);
}

.empty-results {
  padding: 1.5rem 1rem;
  color: #6b7280;
  font-style: italic;
}
</style>
