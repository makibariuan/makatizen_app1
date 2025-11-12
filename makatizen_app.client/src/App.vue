<template>
  <div id="app">
    <AppNavbar v-if="isAuthenticated && $route.name !== 'Login'" />

    <main :class="{'full-screen-main': $route.name === 'Login' || $route.name === 'PasswordReset'}">
      <router-view />
    </main>
  </div>
</template>

<script>
  import AppNavbar from '@/components/Navbar.vue';
  import AuthService from '@/services/AuthService';
  import { ref, watch } from 'vue';
  import { useRoute } from 'vue-router';

  export default {
    name: 'App',
    components: {
      AppNavbar
    },
    setup() {
      const route = useRoute();
      const isAuthenticated = ref(AuthService.isAuthenticated());

      // Watch for route changes or token changes to update authentication status
      // (This is a simplified way; a proper Vuex/Pinia store would be better)
      watch(() => route.path, () => {
        isAuthenticated.value = AuthService.isAuthenticated();
      });

      // Also re-check auth on component creation
      isAuthenticated.value = AuthService.isAuthenticated();


      return {
        isAuthenticated
      };
    }
  };
</script>

<style>
  /* Global Styles (Not scoped) */
  html, body {
    margin: 0;
    padding: 0;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background-color: #e9ecef; /* Light gray background for the overall app */
    min-height: 100vh;
  }

  #app {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
  }

  main {
    flex-grow: 1; /* Allows the main content area to fill the remaining space */
    padding: 20px 0; /* Padding for content below the navbar */
  }

  /* Specific style for login/reset pages to use the full screen layout */
  .full-screen-main {
    padding: 0;
    display: flex;
    justify-content: center;
    align-items: center;
  }
</style>
