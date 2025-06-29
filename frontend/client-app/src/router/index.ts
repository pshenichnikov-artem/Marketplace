import { createRouter, createWebHistory } from 'vue-router'
import LoginPage from '@/views/LoginPage.vue'
import RegisterPage from '@/views/RegisterPage.vue'
import HomePage from '@/views/HomePage.vue'
import AdminAndSellerDashboard from '@/views/Dashboard.vue'
import ProfilePage from '@/views/ProfilePage.vue'
import CatalogPage from '@/views/CatalogPage.vue'
import ProductPage from '@/views/ProductPage.vue'
import CartPage from '@/views/CartPage.vue'
import { getToken, getUserRole } from '@/utils/localStorage'
import Dashboard from '@/views/Dashboard.vue'
import DashboardProducts from '@/components/dashboard/products/DashboardProducts.vue'
import ProductEditor from '@/components/product/ProductEditor.vue'
import DashboardUsers from '@/components/dashboard/users/DashboardUsers.vue'
import UserEditor from '@/components/user/UserEditor.vue'
import DashboardCategories from '@/components/dashboard/categories/DashboardCategories.vue'
import CategoryEditor from '@/components/category/CategoryEditor.vue'
import OrderEditor from '@/components/dashboard/orders/OrderEditor.vue'
import DashboardOrders from '@/components/dashboard/orders/DashboardOrders.vue'
import DashboardReviews from '@/components/dashboard/reviews/DashboardReviews.vue'
import BannerEditor from '@/components/dashboard/banners/BannerEditor.vue'
import DashboardPages from '@/components/dashboard/pages/DashboardPages.vue'
import PageEditor from '@/components/dashboard/pages/PageEditor.vue'
import PageView from '@/views/PageView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomePage,
      meta: {
        requiresAuth: true,
        title: 'Home',
      },
    },
    {
      path: '/login',
      name: 'login',
      component: LoginPage,
      meta: {
        requiresAuth: false,
        title: 'Login',
      },
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterPage,
      meta: {
        requiresAuth: false,
        title: 'Register',
      },
    },
    {
      path: '/unauthorized',
      name: 'unauthorized',
      component: () => import('@/views/UnauthorizedPage.vue'),
      meta: {
        requiresAuth: false,
        title: 'Unauthorized',
      },
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: AdminAndSellerDashboard,
      meta: {
        roles: ['admin', 'seller'],
        title: 'Панель управления',
      },
      children: [
        {
          path: 'products',
          name: 'dashboard-products',
          component: DashboardProducts,
          meta: {
            view: 'products',
          },
          children: [
            {
              path: 'create',
              name: 'product-create',
              component: ProductEditor,
              meta: {
                view: 'product-create',
              },
            },
            {
              path: 'edit/:id',
              name: 'product-edit',
              component: ProductEditor,
              meta: {
                view: 'product-edit',
              },
              props: true,
            },
          ],
        },
        {
          path: 'users',
          name: 'dashboard-users',
          component: DashboardUsers,
          meta: {
            view: 'users',
            roles: ['admin'], // Только для администраторов
          },
          children: [
            {
              path: 'create',
              name: 'user-create',
              component: UserEditor,
              meta: {
                view: 'user-create',
                roles: ['admin'],
              },
            },
            {
              path: 'edit/:id',
              name: 'user-edit',
              component: UserEditor,
              meta: {
                view: 'user-edit',
                roles: ['admin'],
              },
              props: true,
            },
          ],
        },
        {
          path: 'categories',
          name: 'dashboard-categories',
          component: DashboardCategories,
          meta: {
            view: 'categories',
            roles: ['admin'], // Только для администраторов
          },
          children: [
            {
              path: 'create',
              name: 'category-create',
              component: CategoryEditor,
              meta: {
                view: 'category-create',
                roles: ['admin'],
              },
            },
            {
              path: 'edit/:id',
              name: 'category-edit',
              component: CategoryEditor,
              meta: {
                view: 'category-edit',
                roles: ['admin'],
              },
              props: true,
            },
          ],
        },
        {
          path: 'orders',
          name: 'dashboard-orders',
          component: DashboardOrders,
          meta: {
            view: 'orders',
            roles: ['admin', 'seller'],
          },
          children: [
            {
              path: 'edit/:id',
              name: 'order-edit',
              component: OrderEditor,
              meta: {
                view: 'order-edit',
                roles: ['admin', 'seller'],
              },
              props: true,
            },
          ],
        },
        {
          path: 'reviews',
          name: 'dashboard-reviews',
          component: DashboardReviews,
          meta: {
            view: 'reviews',
            roles: ['admin'], // Только для администраторов
          },
        },
        {
          path: 'banners',
          name: 'dashboard-banners',
          component: BannerEditor,
          meta: {
            view: 'banners',
            roles: ['admin'], // Только для администраторов
          },
        },
        {
          path: 'pages',
          name: 'dashboard-pages',
          component: DashboardPages,
          meta: {
            view: 'pages',
            roles: ['admin'], // Только для администраторов
          },
          children: [
            {
              path: 'create',
              name: 'page-create',
              component: PageEditor,
              meta: {
                view: 'page-create',
                roles: ['admin'],
              },
            },
            {
              path: 'edit/:pageKey/:language',
              name: 'page-edit',
              component: PageEditor,
              meta: {
                view: 'page-edit',
                roles: ['admin'],
              },
              props: true,
            },
          ],
        },
      ],
    },
    {
      path: '/profile',
      name: 'profile',
      component: ProfilePage,
      meta: {
        requiresAuth: true,
        title: 'Профиль',
      },
    },
    {
      path: '/catalog',
      name: 'catalog',
      component: CatalogPage,
      meta: {
        requiresAuth: true,
        title: 'Каталог',
      },
    },
    {
      path: '/product/:id',
      name: 'product',
      component: ProductPage,
      meta: {
        requiresAuth: true,
        title: 'Товар',
      },
    },
    {
      path: '/cart',
      name: 'cart',
      component: CartPage,
      meta: {
        requiresAuth: true,
        title: 'Корзина',
      },
    },
    {
      path: '/:pageKey(.*)*',
      name: 'dynamic-page',
      component: PageView,
      meta: {
        requiresAuth: false,
        title: 'Page',
        allowUnauthenticated: true,
      },
    },
  ],
})

router.beforeEach((to, from, next) => {
  // Установка заголовка документа
  document.title = to.meta.title ? `${to.meta.title} | MyApp` : 'MyApp'

  const isAuthenticated = !!getToken()
  const userRole = getUserRole()
  const allowedRoles = Array.isArray(to.meta.roles) ? to.meta.roles : []
  const allowUnauthenticated = to.meta.allowUnauthenticated === true

  if (to.meta.requiresAuth && !allowUnauthenticated) {
    if (!isAuthenticated) {
      next('/login')
    } else if (
      allowedRoles.length &&
      (!userRole || !allowedRoles.some((role) => role === userRole))
    ) {
      next('/unauthorized')
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router
