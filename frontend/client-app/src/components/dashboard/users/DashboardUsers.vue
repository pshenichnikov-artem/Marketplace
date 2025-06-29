<template>
    <div class="space-y-6">
        <router-view v-if="isUserEditorRoute"></router-view>

        <template v-else>
            <AdminPanel :title="$t('dashboard.users.title')" :columns="columns" :items="users.items"
                :total-items="users.totalItems" :current-page="currentPage" :page-size="pageSize" :sort-key="sortKey"
                :sort-order="sortOrder" :t-function="translateFunction" :no-data-text="$t('dashboard.users.noUsers')"
                :no-data-subtext="$t('dashboard.users.noUsersSubtext')" @add-new="openAddUserModal"
                @edit="openEditUserModal" @delete="confirmDeleteUser" @apply-filters="applyFilters"
                @reset-filters="resetFilters" @pagination-change="handlePaginationChange" @sort="handleSort">
                <!-- Слот для фильтров -->
                <template #filters>
                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.users.filters.search')" type="text"
                            v-model="filters.search" />
                    </div>

                    <div class="form-group">
                        <BaseFilterField :label="$t('dashboard.users.filters.role')" type="select"
                            v-model="filters.role" :options="roleOptions" />
                    </div>
                </template>

                <template #cell-role="{ value }">
                    <span class="px-2 py-1 text-xs font-semibold rounded-full" :class="getRoleBadgeClass(value)">
                        {{ $t(`dashboard.users.roles.${value}`) }}
                    </span>
                </template>
            </AdminPanel>

            <!-- Модальное окно подтверждения удаления -->
            <ConfirmModal v-if="showDeleteModal" :title="$t('dashboard.users.deleteTitle')"
                :message="$t('dashboard.users.deleteMessage', { name: userToDelete?.firstName + ' ' + userToDelete?.lastName || 'user' })"
                :confirm-text="$t('dashboard.users.confirmDelete')" :cancel-text="$t('dashboard.users.cancelDelete')"
                @confirm="deleteUser" @cancel="showDeleteModal = false" />

            <Notification ref="toast" />
        </template>
    </div>
</template>

<script>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter, useRoute } from 'vue-router';
import AdminPanel from '@/components/admin/AdminPanel.vue';
import ConfirmModal from '@/components/ui/ConfirmModal.vue';
import Notification from '@/components/ui/Notification.vue';
import BaseFilterField from '@/components/ui/filters/BaseFilterField.vue';
import userService from '@/services/api/userService';
import useApiRequest from '@/hooks/useApiRequest';

export default {
    name: 'DashboardUsers',
    components: {
        AdminPanel,
        ConfirmModal,
        Notification,
        BaseFilterField
    },
    setup() {
        const { t, locale } = useI18n();
        const toast = ref(null);
        const router = useRouter();
        const route = useRoute();

        const isUserEditorRoute = computed(() => {
            return route.path.includes('/dashboard/users/edit/') ||
                route.path.includes('/dashboard/users/create');
        });

        const users = ref({ items: [], totalItems: 0 });
        const currentPage = ref(1);
        const pageSize = ref(20);
        const sortKey = ref('id');
        const sortOrder = ref('desc');

        const filters = reactive({
            search: '',
            role: ''
        });

        const showDeleteModal = ref(false);
        const userToDelete = ref(null);

        const roleOptions = computed(() => [
            { label: t('dashboard.users.filters.allRoles'), value: '' },
            { label: t('dashboard.users.roles.admin'), value: 'admin' },
            { label: t('dashboard.users.roles.seller'), value: 'seller' },
            { label: t('dashboard.users.roles.user'), value: 'user' }
        ]);

        const columns = computed(() => [
            { key: 'id', label: 'ID', sortable: true, width: 'w-16' },
            { key: 'firstName', label: t('dashboard.users.columns.firstName'), sortable: true },
            { key: 'lastName', label: t('dashboard.users.columns.lastName'), sortable: true },
            { key: 'email', label: t('dashboard.users.columns.email'), sortable: true },
            { key: 'role', label: t('dashboard.users.columns.role'), sortable: true, width: 'w-32' },
            { key: 'phone', label: t('dashboard.users.columns.phone'), sortable: true, width: 'w-40' }
        ]);

        const { loading: usersLoading, execute: executeUsersFetch } = useApiRequest();
        const { loading: deleteLoading, execute: executeDelete } = useApiRequest();

        const getRoleBadgeClass = (role) => {
            switch (role) {
                case 'admin':
                    return 'bg-purple-100 text-purple-800';
                case 'seller':
                    return 'bg-blue-100 text-blue-800';
                default:
                    return 'bg-green-100 text-green-800';
            }
        };

        const fetchUsers = async () => {
            await executeUsersFetch(async () => {
                const params = {
                    page: currentPage.value,
                    pageSize: pageSize.value,
                    search: filters.search || undefined,
                    role: filters.role || undefined,
                    orderBy: sortKey.value ? `${sortKey.value},${sortOrder.value}` : undefined
                };

                return await userService.getUsers(params.page, params.search, params.role);
            }, {
                onSuccess: (data) => {
                    if (data) {
                        users.value = {
                            items: data || [],
                            totalItems: 10
                        };
                    }
                },
                showErrorNotification: true,
                notificationRef: toast
            });
        };

        const initializeData = () => {
            if (!isUserEditorRoute.value) {
                fetchUsers();
            }
        };

        watch(() => route.path, (newPath, oldPath) => {
            if (
                (oldPath.includes('/dashboard/users/edit/') || oldPath.includes('/dashboard/users/create')) &&
                newPath === '/dashboard/users'
            ) {
                initializeData();
            }
        });
        onMounted(() => {
            initializeData();
        });

        const applyFilters = () => {
            currentPage.value = 1;
            fetchUsers();
        };

        const resetFilters = () => {
            Object.keys(filters).forEach(key => {
                filters[key] = '';
            });
            currentPage.value = 1;
            fetchUsers();
        };

        const handlePaginationChange = (event) => {
            currentPage.value = event.page;
            pageSize.value = event.pageSize;
            fetchUsers();
        };

        const handleSort = (key) => {
            if (sortKey.value === key) {
                sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc';
            } else {
                sortKey.value = key;
                sortOrder.value = 'asc';
            }
            fetchUsers();
        };

        const openAddUserModal = () => {
            router.push('/dashboard/users/create');
        };

        const openEditUserModal = (user) => {
            router.push(`/dashboard/users/edit/${user.id}`);
        };

        const confirmDeleteUser = (user) => {
            userToDelete.value = user;
            showDeleteModal.value = true;
        };

        const deleteUser = async () => {
            if (!userToDelete.value) return;

            await executeDelete(async () => {
                return await userService.deleteUser(userToDelete.value.id);
            }, {
                onSuccess: () => {
                    toast.value.success(t('common.messages.deleteSuccess'));
                    fetchUsers();
                },
                showErrorNotification: true,
                notificationRef: toast
            });

            showDeleteModal.value = false;
            userToDelete.value = null;
        };

        const translateFunction = (key) => {
            return t(key);
        };

        return {
            users,
            columns,
            currentPage,
            pageSize,
            sortKey,
            sortOrder,
            filters,
            roleOptions,
            showDeleteModal,
            userToDelete,
            toast,
            usersLoading,
            deleteLoading,
            applyFilters,
            resetFilters,
            handlePaginationChange,
            handleSort,
            openAddUserModal,
            openEditUserModal,
            confirmDeleteUser,
            deleteUser,
            translateFunction,
            isUserEditorRoute,
            getRoleBadgeClass
        };
    }
};
</script>
