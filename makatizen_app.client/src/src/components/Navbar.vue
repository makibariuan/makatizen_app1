<template>
  <nav class="navbar">
    <div class="navbar-brand">
      <router-link :to="{ name: 'Home' }" class="app-title">
        Makatizen App Portal
      </router-link>
    </div>

    <div v-if="isAuthenticated" class="user-info">
      <span class="logged-in-as">
        Logged in as: <strong>{{ userRole }}</strong>
      </span>
      <button @click="handleLogout" class="logout-button">
        Logout
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="icon">
          <path fill-rule="evenodd" d="M7.5 3.75A1.5 1.5 0 0 0 6 5.25v13.5a1.5 1.5 0 0 0 1.5 1.5h6a1.5 1.5 0 0 0 1.5-1.5V15a.75.75 0 0 1 1.5 0v3.75a3 3 3 0 0 1-3 3h-6a3 3 0 0 1-3-3V5.25a3 3 0 0 1 3-3h6a3 3 0 0 1 3 3V9a.75.75 0 0 1-1.5 0V5.25a1.5 1.5 0 0 0-1.5-1.5h-6Zm10.662 10.375a.75.75 0 1 0 1.06 1.06l3-3a.75.75 0 0 0 0-1.06l-3-3a.75.75 0 1 0-1.06 1.06l1.72 1.72H13.5a.75.75 0 0 0 0 1.5h5.882l-1.72 1.72Z" clip-rule="evenodd" />
        </svg>
      </button>
    </div>
  </nav>
</template>

<script>import AuthService from '@/services/AuthService';
import { useRouter } from 'vue-router';
import { ref, computed } from 'vue';

export default {
  name: 'AppNavbar',
  setup() {
    const router = useRouter();
    // Use ref to track the user's role (will be static unless we add Vuex later)
    const userRole = ref(AuthService.getUserRole());

    // Check authentication status based on the presence of a token
    const isAuthenticated = computed(() => AuthService.isAuthenticated());

    const handleLogout = () => {
      AuthService.logout();
      router.push({ name: 'Login' });
    };

    return {
      userRole,
      isAuthenticated,
      handleLogout
    };
  }
};</script>

<style scoped>
  .navbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 15px 30px;
    background-color: #006666;
    color: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.15);
    position: sticky;
    top: 0;
    z-index: 1000;
  }

  .navbar-brand .app-title {
    font-size: 1.5rem;
    font-weight: 700;
    color: white;
    text-decoration: none;
  }

  .user-info {
    display: flex;
    align-items: center;
    gap: 20px;
  }

  .logged-in-as {
    font-size: 0.95rem;
    font-weight: 300;
  }

  .logout-button {
    display: flex;
    align-items: center;
    padding: 8px 15px;
    background-color: #27ae60;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: 600;
    transition: background-color 0.3s;
  }

    .logout-button:hover {
      background-color: #1e874b;
    }

  .icon {
    width: 18px;
    height: 18px;
    margin-left: 8px;
    fill: white; /* ensures logout icon is white */
  }
</style>
