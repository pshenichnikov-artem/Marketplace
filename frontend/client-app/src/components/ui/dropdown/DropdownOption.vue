<template>
  <li :id="optionId" :class="[
    'px-3 py-2.5 cursor-pointer select-none relative dropdown-option transition-colors duration-150 ease-in-out',
    highlighted ? 'bg-indigo-50 text-indigo-700' : 'hover:bg-gray-50',
    hasChildren ? 'has-children' : ''
  ]" role="option" @mouseenter="$emit('hover')" @click="handleClick">
    <div class="flex justify-between items-center">
      <div class="flex items-center gap-2 truncate">
        <!-- Иконка для отображения статуса выбора опции -->
        <span v-if="isSelected && !hasChildren" class="text-indigo-600 flex-shrink-0">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd"
              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
              clip-rule="evenodd" />
          </svg>
        </span>
        <span class="truncate">{{ option.label }}</span>
      </div>

      <span v-if="hasChildren" class="ml-2 text-gray-400 transition-transform duration-200 chevron-icon"
        :class="{ 'rotate-90': submenuOpen }" @click.stop="toggleSubmenu">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
        </svg>
      </span>
    </div>
  </li>

  <!-- Подменю с анимацией -->
  <Transition name="submenu">
    <teleport to="body" v-if="submenuOpen && hasChildren">
      <div class="fixed bg-white border border-gray-200 rounded-lg shadow-xl min-w-max z-50 submenu-container"
        :style="submenuPosition" ref="submenu" @click.stop>
        <ul class="py-1">
          <DropdownOption v-for="(child, i) in option.children" :key="child.id ?? child.value ?? i" :option="child"
            :highlighted="false" :select-only-leaf="selectOnlyLeaf" @select="$emit('select', $event)"
            @hover="$emit('hover')" :parent-path="currentPath" />
        </ul>
      </div>
    </teleport>
  </Transition>
</template>

<script>
export default {
  name: 'DropdownOption',
  props: {
    option: Object,
    highlighted: Boolean,
    selectOnlyLeaf: Boolean,
    optionId: String,
    parentPath: {
      type: Array,
      default: () => []
    }
  },
  emits: ['select', 'hover'],
  data() {
    return {
      submenuOpen: false,
      submenuPosition: {
        top: '0px',
        left: '0px'
      }
    };
  },
  computed: {
    hasChildren() {
      return this.option.children && this.option.children.length > 0;
    },
    isSelected() {
      // Проверка, является ли опция выбранной, для отображения галочки
      return this.$parent.modelValue === this.option.id ||
        this.$parent.modelValue === this.option.value;
    },
    // Создаем путь для текущего элемента, добавляя его id к родительскому пути
    currentPath() {
      const id = this.option.id || this.option.value;
      return [...this.parentPath, id];
    }
  },
  methods: {
    handleClick(event) {
      if (this.hasChildren) {
        // Если есть дочерние элементы, открываем/закрываем подменю
        // при клике на основную часть элемента
        if (this.selectOnlyLeaf) {
          this.toggleSubmenu(event);
        } else {
          // Если разрешено выбирать родительские элементы
          this.$emit('select', this.option);
        }
      } else {
        // Если нет дочерних элементов, просто выбираем опцию
        this.$emit('select', this.option);
      }
    },
    toggleSubmenu(event) {
      event.stopPropagation();

      // Закрываем только субменю того же уровня, но не родительские или дочерние
      this.closeAllSiblingSubmenus();

      // Переключаем состояние текущего подменю
      this.submenuOpen = !this.submenuOpen;

      if (this.submenuOpen) {
        this.$nextTick(() => {
          // Получаем размеры родительского элемента для правильного позиционирования
          const parentElement = event.currentTarget.closest('li');
          const rect = parentElement.getBoundingClientRect();

          // Устанавливаем позицию подменю относительно родительского элемента
          this.submenuPosition = {
            top: `${rect.top}px`,
            left: `${rect.right + 5}px` // 5px отступ от родительского элемента
          };
        });
      }
    },
    closeAllSiblingSubmenus() {
      // Создаем кастомное событие с передачей текущего пути и уровня
      const event = new CustomEvent('close-sibling-submenus', {
        detail: {
          path: this.currentPath,
          parentPath: this.parentPath,
          level: this.parentPath.length
        }
      });
      document.dispatchEvent(event);
    },
    // Обработчик события закрытия субменю
    handleCloseSubmenus(event) {
      const { path, parentPath, level } = event.detail;

      // Закрываем только меню того же уровня (с тем же родителем), но не если это наш предок или потомок
      if (level === this.parentPath.length && // тот же уровень
        JSON.stringify(parentPath) === JSON.stringify(this.parentPath) && // тот же родитель
        path[path.length - 1] !== (this.option.id || this.option.value)) { // не сам текущий элемент
        this.submenuOpen = false;
      }
    }
  },
  created() {
    // Слушаем событие для закрытия субменю
    this.closeSubmenuHandler = this.handleCloseSubmenus.bind(this);
    document.addEventListener('close-sibling-submenus', this.closeSubmenuHandler);
  },
  beforeUnmount() {
    document.removeEventListener('close-sibling-submenus', this.closeSubmenuHandler);
  }
};
</script>

<style scoped>
.dropdown-option {
  border-radius: 0.25rem;
  margin: 0 0.25rem;
  width: calc(100% - 0.5rem);
}

.dropdown-option:first-child {
  margin-top: 0.25rem;
}

.dropdown-option:last-child {
  margin-bottom: 0.25rem;
}

.submenu-container {
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  border-radius: 0.5rem;
  overflow: hidden;
  min-width: 200px;
}

.chevron-icon {
  opacity: 0.6;
}

.has-children:hover .chevron-icon {
  opacity: 1;
}

/* Анимация для подменю */
.submenu-enter-active,
.submenu-leave-active {
  transition: opacity 0.2s, transform 0.2s;
}

.submenu-enter-from,
.submenu-leave-to {
  opacity: 0;
  transform: translateX(-10px);
}
</style>
