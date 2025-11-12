import { createRouter, createWebHistory } from 'vue-router';
import AuthService from '@/services/AuthService'; // Import the service

// --- Import ACTUAL Views ---
import LoginView from '@/components/Login.vue';
import PasswordResetView from '@/components/PasswordReset.vue';
import DashboardAdmin from '@/components/DashboardAdmin.vue';
import DashboardKit from '@/components/DashboardKit.vue';
import KitUsersManagement from '@/components/KitUsersView.vue'; // <-- NEW IMPORT
import NotFound from '@/components/NotFound.vue'; // Catch-all 404
import KitUsersViewVue from '../components/KitUsersView.vue';

const routes = [
  {
    path: '/',
    name: 'Home',
    // Redirects authenticated users to their dashboard.
    redirect: (to) => {
      const role = AuthService.getUserRole();
      const isAuth = AuthService.isAuthenticated();
      const mustReset = AuthService.mustResetPassword();

      if (!isAuth) {
        return { name: 'Login' };
      }

      if (mustReset) {
        return { name: 'PasswordReset' };
      }

      if (role === 'Super Admin' || role === 'System User') {
        return { name: 'DashboardAdmin' };
      } else if (role === 'Kit User') {
        return { name: 'DashboardKit' };
      }

      // Default to login if role is unknown/expired
      return { name: 'Login' };
    }
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: { requiresAuth: false }
  },
  {
    path: '/reset-password',
    name: 'PasswordReset',
    component: PasswordResetView,
    // Note: It still requires authentication (a valid token) to access,
    // but the global guard handles the force-redirect logic.
    meta: { requiresAuth: true }
  },
  // --- PROTECTED DASHBOARD ROUTES ---
  {
    path: '/admin/dashboard',
    name: 'DashboardAdmin',
    component: DashboardAdmin,
    meta: {
      requiresAuth: true,
      roles: ['Super Admin', 'System User'] // Matching roles from AuthService
    }
  },
 
  {
    path: '/admin/kit-users',
    name: 'KitUsersManagement',
    component: KitUsersViewVue,
    meta: {
      requiresAuth: true,
      roles: ['Super Admin', 'System User'] // Only Super Admin and System User can manage kit users
    }
  },
  {
    path: '/kit/dashboard',
    name: 'DashboardKit',
    component: DashboardKit,
    meta: {
      requiresAuth: true,
      roles: ['Kit User'] // Matching roles from AuthService
    }
  },
  // Catch-all 404 route
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFound
  }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

// --- GLOBAL NAVIGATION GUARD ---
router.beforeEach((to, from, next) => {
  const isAuthenticated = AuthService.isAuthenticated();
  const isResetRequired = AuthService.mustResetPassword();
  const userRole = AuthService.getUserRole();

  // 1. If authentication is required for the route
  if (to.meta.requiresAuth) {
    if (!isAuthenticated) {
      // Not authenticated, redirect to login
      return next({ name: 'Login' });
    }

    // 2. Check for mandatory password reset
    if (isResetRequired && to.name !== 'PasswordReset') {
      // Authenticated but must reset password, force redirect to reset page
      return next({ name: 'PasswordReset' });
    }

    // If user is on PasswordReset and it is no longer required (shouldn't happen, but good check)
    if (to.name === 'PasswordReset' && !isResetRequired) {
      // Redirect them to their proper dashboard instead of letting them sit on reset page
      if (userRole === 'Kit User') {
        return next({ name: 'DashboardKit' });
      } else if (userRole === 'Super Admin' || userRole === 'System User') {
        return next({ name: 'DashboardAdmin' });
      }
      return next({ name: 'Home' });
    }

    // 3. Check role authorization (if roles are defined for the route)
    if (to.meta.roles && !to.meta.roles.includes(userRole)) {
      // Authorized but trying to access the wrong dashboard
      console.warn(`User role "${userRole}" denied access to route: ${to.name}`);

      // Redirect to their default dashboard
      if (userRole === 'Kit User') {
        return next({ name: 'DashboardKit' });
      } else if (userRole === 'Super Admin' || userRole === 'System User') {
        return next({ name: 'DashboardAdmin' });
      } else {
        // Unknown role, kick to login
        AuthService.logout();
        return next({ name: 'Login' });
      }
    }

    // All checks passed
    next();
  }
  // 4. If user is authenticated and trying to access Login
  else if (isAuthenticated && to.name === 'Login') {
    if (!isResetRequired) {
      // Logged in and not forced to reset, redirect to their dashboard
      if (userRole === 'Kit User') {
        return next({ name: 'DashboardKit' });
      } else if (userRole === 'Super Admin' || userRole === 'System User') {
        return next({ name: 'DashboardAdmin' });
      }
    }
    // If logged in but password reset is required, they stay on the Login page (which will redirect to PasswordReset via the 'Home' redirect logic)
    next();
  }
  // 5. Default: proceed
  else {
    next();
  }
});

export default router;
