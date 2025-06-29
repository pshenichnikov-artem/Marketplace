<template>
    <teleport to="body">
        <div class="fixed top-4 right-4 z-50 w-full max-w-sm flex flex-col gap-2 pointer-events-none">
            <transition-group name="notification">
                <div v-for="notification in notifications" :key="notification.id" :class="[
                    'shadow-lg rounded-lg p-4 mb-2 transform transition-all duration-300 pointer-events-auto',
                    notificationTypeClasses[notification.type],
                ]" :style="{
                    maxHeight: notification.visible ? '200px' : '0',
                    opacity: notification.visible ? '1' : '0',
                }">
                    <div class="flex items-start">
                        <div class="flex-shrink-0">
                            <component :is="icons[notification.type]" class="h-5 w-5 text-white" />
                        </div>
                        <div class="ml-3 flex-1">
                            <p class="text-sm font-medium">{{ notification.message }}</p>
                        </div>
                        <div class="ml-auto pl-3">
                            <button @click="closeNotification(notification.id)"
                                class="inline-flex rounded-md p-1.5 focus:outline-none focus:ring-2 focus:ring-offset-2"
                                :class="buttonClass(notification.type)">
                                <span class="sr-only">Close</span>
                                <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                        d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            </button>
                        </div>
                    </div>
                    <div v-if="notification.timeout > 0" class="absolute bottom-0 left-0 h-1 bg-white bg-opacity-30"
                        :style="{
                            width: `${notification.progress}%`,
                            transitionDuration: `${notification.timeout}ms`,
                        }"></div>
                </div>
            </transition-group>
        </div>
    </teleport>
</template>

<script>
import { h } from 'vue'

export default {
    name: 'Notification',
    data() {
        return {
            notifications: [],
            notificationCounter: 0,
            notificationTypeClasses: {
                success: 'bg-green-500 text-white',
                error: 'bg-red-600 text-white',
                info: 'bg-blue-600 text-white',
                warning: 'bg-yellow-600 text-white',
            },
            icons: {
                success: () =>
                    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
                        h('path', {
                            strokeLinecap: 'round',
                            strokeLinejoin: 'round',
                            strokeWidth: 2,
                            d: 'M5 13l4 4L19 7',
                        }),
                    ]),
                error: () =>
                    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
                        h('path', {
                            strokeLinecap: 'round',
                            strokeLinejoin: 'round',
                            strokeWidth: 2,
                            d: 'M6 18L18 6M6 6l12 12',
                        }),
                    ]),
                info: () =>
                    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
                        h('path', {
                            strokeLinecap: 'round',
                            strokeLinejoin: 'round',
                            strokeWidth: 2,
                            d: 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
                        }),
                    ]),
                warning: () =>
                    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
                        h('path', {
                            strokeLinecap: 'round',
                            strokeLinejoin: 'round',
                            strokeWidth: 2,
                            d: 'M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z',
                        }),
                    ]),
            },
        }
    },
    methods: {
        show(message, options = {}) {
            console.log('show', message, options);
            const type = options.type || 'info';
            const timeout = options.timeout !== undefined ? options.timeout : 5000;
            const id = ++this.notificationCounter;

            const notification = {
                id,
                message,
                type,
                timeout,
                visible: true,
                progress: 100,
                timer: null,
                progressInterval: null,
            };

            this.notifications.unshift(notification);

            if (timeout > 0) {
                notification.progressInterval = window.setInterval(() => {
                    if (notification.progress > 0) {
                        notification.progress -= 0.5;
                    }
                }, timeout / 200);

                notification.timer = window.setTimeout(() => {
                    this.closeNotification(id);
                }, timeout);
            }

            return id;
        },

        success(message, options = {}) {
            return this.show(message, { ...options, type: 'success' });
        },

        error(message, options = {}) {
            return this.show(message, { ...options, type: 'error' });
        },

        info(message, options = {}) {
            return this.show(message, { ...options, type: 'info' });
        },

        warning(message, options = {}) {
            return this.show(message, { ...options, type: 'warning' });
        },

        closeNotification(id) {
            const index = this.notifications.findIndex(n => n.id === id);
            if (index !== -1) {
                const notification = this.notifications[index];
                if (notification.timer) clearTimeout(notification.timer);
                if (notification.progressInterval) clearInterval(notification.progressInterval);
                notification.visible = false;
                notification.progress = 0;
                setTimeout(() => {
                    this.notifications = this.notifications.filter(n => n.id !== id);
                }, 300);
            }
        },

        closeAll() {
            this.notifications.forEach(n => this.closeNotification(n.id));
        },

        buttonClass(type) {
            return {
                success: 'text-white hover:bg-green-600 focus:ring-green-500',
                error: 'text-white hover:bg-red-700 focus:ring-red-500',
                info: 'text-white hover:bg-blue-700 focus:ring-blue-500',
                warning: 'text-white hover:bg-yellow-700 focus:ring-yellow-500',
            }[type];
        },
    }
};
</script>

<style scoped>
.notification-enter-active,
.notification-leave-active {
    transition: all 0.3s ease;
}

.notification-enter-from,
.notification-leave-to {
    transform: translateX(30px);
    opacity: 0;
}
</style>
