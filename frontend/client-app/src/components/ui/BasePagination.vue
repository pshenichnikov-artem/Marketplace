<template>
  <div class="flex flex-row items-center justify-between gap-6 mt-8 w-full">
    <div class="flex items-center gap-3">
      <span class="text-base text-gray-700 font-semibold">{{ $t('basePagination.show') }}</span>
      <select v-model="internalPageSize" class="border border-gray-300 rounded px-3 py-2 text-base">
        <option v-for="size in pageSizes" :key="size" :value="size">{{ size }}</option>
      </select>
    </div>

    <div class="flex-1 flex items-center justify-center">
      <div class="flex items-center gap-2">
        <button v-if="internalCurrentPage > 1" @click="changePage(internalCurrentPage - 1)"
          class="px-3 py-2 border rounded font-semibold text-base bg-white hover:bg-gray-100 transition">
          {{ $t('basePagination.prev') }}
        </button>

        <div class="flex items-center">
          <button @click="changePage(1)" :class="getPageButtonClass(1)">
            1
          </button>

          <span v-if="startPage > 2" class="px-3 py-2">...</span>

          <button v-for="page in visiblePages" :key="page" @click="changePage(page)" :class="getPageButtonClass(page)">
            {{ page }}
          </button>

          <span v-if="endPage < totalPages - 1" class="px-3 py-2">...</span>

          <button v-if="totalPages > 1" @click="changePage(totalPages)" :class="getPageButtonClass(totalPages)">
            {{ totalPages }}
          </button>
        </div>

        <button v-if="internalCurrentPage < totalPages" @click="changePage(internalCurrentPage + 1)"
          class="px-3 py-2 border rounded font-semibold text-base bg-white hover:bg-gray-100 transition">
          {{ $t('basePagination.next') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    totalItems: {
      type: Number,
      required: true
    },
    pageSizes: {
      type: Array,
      default: () => [20, 50, 100, 150]
    },
    maxVisiblePages: {
      type: Number,
      default: 5
    },
    currentPage: {
      type: Number,
      default: 1
    },
    pageSize: {
      type: Number,
      default: 20
    }
  },
  emits: ['pagination-change'],
  data() {
    return {
      internalCurrentPage: this.currentPage || 1,
      internalPageSize: this.pageSize || this.pageSizes[0]
    };
  },
  computed: {
    totalPages() {
      return Math.max(1, Math.ceil(this.totalItems / this.internalPageSize));
    },
    startPage() {
      if (this.internalCurrentPage <= Math.ceil(this.maxVisiblePages / 2)) {
        return 2;
      }
      if (this.internalCurrentPage > this.totalPages - Math.ceil(this.maxVisiblePages / 2)) {
        return Math.max(2, this.totalPages - this.maxVisiblePages);
      }
      return Math.max(2, this.internalCurrentPage - Math.floor(this.maxVisiblePages / 2));
    },
    endPage() {
      return Math.min(this.totalPages - 1, this.startPage + this.maxVisiblePages - 1);
    },
    visiblePages() {
      const pages = [];
      for (let i = this.startPage; i <= this.endPage; i++) {
        pages.push(i);
      }
      return pages;
    }
  },
  watch: {
    internalPageSize(newSize, oldSize) {
      if (newSize !== oldSize) {
        this.internalCurrentPage = 1;
        this.emitChange();
      }
    },
    currentPage(newVal) {

      if (newVal !== this.internalCurrentPage) {
        this.internalCurrentPage = newVal;
        this.getPageButtonClass(page)
      }
    },
    pageSize(newVal) {
      if (newVal !== this.internalPageSize) {
        this.internalPageSize = newVal;
      }
    }
  },
  methods: {
    changePageWithoutUpdate(page) {
      if (page >= 1 && page <= this.totalPages) {
        this.internalCurrentPage = page;
        this.getPageButtonClass(page)
        window.scrollTo({
          top: 0,
          behavior: 'smooth'
        });
      }
    },
    changePage(page) {
      if (page >= 1 && page <= this.totalPages) {
        this.internalCurrentPage = page;
        this.emitChange();
        window.scrollTo({
          top: 0,
          behavior: 'smooth'
        });
      }
    },
    getPageButtonClass(page) {
      return page === this.internalCurrentPage
        ? 'px-3 py-2 border rounded font-bold text-base bg-indigo-600 text-white hover:bg-indigo-700 transition'
        : 'px-3 py-2 border rounded font-semibold text-base bg-white hover:bg-gray-100 transition';
    },
    emitChange() {
      this.$emit('pagination-change', {
        page: this.internalCurrentPage,
        pageSize: this.internalPageSize
      });
    },
  }
};
</script>
