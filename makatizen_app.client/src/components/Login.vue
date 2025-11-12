<template>
  <div class="login-wrapper">
    <div class="login-card">
      <!-- Left Section -->
      <div class="left-section">
        <img src="@/assets/lungsod_ng_makati_logo.png" alt="Makati Logo" class="logo" />
        <h1>Makati Senior Citizen System</h1>
      </div>

      <!-- Right Section -->
      <div class="right-section">
        <img src="@/assets/lungsod_ng_makati_logo.png" alt="Makati Logo" class="mobile-logo" />
        <h2>Log In</h2>

        <form @submit.prevent="handleLogin" class="login-form">
          <!-- Username -->
          <div class="input-group">
            <input v-model="username"
                   type="text"
                   placeholder="Username"
                   required
                   :disabled="loading" />
          </div>

          <!-- Password -->
          <div class="input-group">
            <input :type="showPassword ? 'text' : 'password'"
                   v-model="password"
                   placeholder="Password"
                   required
                   autocomplete="current-password"
                   :disabled="loading" />
            <span class="toggle-password" @click="showPassword = !showPassword">
              <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </span>
          </div>

          <!-- Error Message -->
          <p v-if="error" class="error-message">{{ error }}</p>

          <!-- Forgot Password -->
          <div class="forgot-password">
            <router-link to="/forgot-password" class="link">Forgot Password?</router-link>
          </div>

          <!-- Login Button -->
          <button type="submit"
                  class="login-btn"
                  :disabled="loading || !username || !password">
            {{ loading ? "Authenticating..." : "Login" }}
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
  import AuthService from '@/services/AuthService';

  export default {
    name: 'LoginView',
    data() {
      return {
        username: '',
        password: '',
        error: '',
        loading: false,
        showPassword: false,
      };
    },
    methods: {
      async handleLogin() {
        this.error = '';
        this.loading = true;

        try {
          const response = await AuthService.login(this.username, this.password);

          if (response.mustResetPassword) {
            this.$router.push({ name: 'PasswordReset' });
          } else {
            const userRole = AuthService.getUserRole();

            if (userRole === 'Kit User') {
              this.$router.push({ name: 'DashboardKit' });
            } else if (userRole === 'Super Admin' || userRole === 'System User') {
              this.$router.push({ name: 'DashboardAdmin' });
            } else {
              this.error = 'Login successful, but role is unrecognized.';
              AuthService.logout();
            }
          }
        } catch (err) {
          this.error =
            err.response?.data?.message ||
            err.message ||
            'Login failed. Please check your credentials.';
        } finally {
          this.loading = false;
          this.password = '';
        }
      },
    },
  };
</script>

<style scoped>
  @import "@/assets/css/auth.css";
</style>
