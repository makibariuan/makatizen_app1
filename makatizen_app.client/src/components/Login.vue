<template>
  <div class="login-container">
    <div class="login-box">
      <h2 class="title">Makatizen App Portal 🏙️</h2>
      <p class="subtitle">Secure Login</p>

      <form @submit.prevent="handleLogin">

        <!-- Username Input -->
        <div class="input-group">
          <label for="username">Username</label>
          <input type="text"
                 id="username"
                 v-model="username"
                 required
                 :disabled="loading" />
        </div>

        <!-- Password Input -->
        <div class="input-group">
          <label for="password">Password</label>
          <input type="password"
                 id="password"
                 v-model="password"
                 required
                 :disabled="loading" />
        </div>

        <!-- Feedback Message -->
        <p v-if="error" class="error-message">{{ error }}</p>

        <!-- Submit Button -->
        <button type="submit"
                :disabled="loading || !username || !password"
                class="login-button">
          {{ loading ? 'Authenticating...' : 'Log In' }}
        </button>
      </form>

      <p class="mt-4 text-sm text-gray-500">
        Forgot your password? (Feature coming soon)
      </p>
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
    };
  },
  methods: {
    async handleLogin() {
      this.error = '';
      this.loading = true;

      try {
        const response = await AuthService.login(this.username, this.password);

        // --- Successful Login Handled by AuthService ---

        if (response.mustResetPassword) {
            // User successfully logged in but MUST reset password
            this.$router.push({ name: 'PasswordReset' });
        } else {
            // User successfully logged in and does NOT need to reset password
            const userRole = AuthService.getUserRole();

            // Redirect based on role (matching the router guard logic)
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
        // Display user-friendly error from the API or fetch failure
        this.error = err.response?.data?.message || err.message || 'Login failed. Please check your credentials.';
      } finally {
        this.loading = false;
        this.password = ''; // Clear password field for security
      }
    }
  }
};
</script>

<style scoped>
  /* Tailwind-like utility styling using plain CSS for Vue component */
  .login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background-color: #e2e8f0; /* Light gray background */
  }

  .login-box {
    background: white;
    padding: 40px;
    border-radius: 12px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
    width: 100%;
    max-width: 400px;
    text-align: center;
  }

  .title {
    margin-bottom: 5px;
    font-size: 1.8rem;
    font-weight: 700;
    color: #1a202c; /* Dark text */
  }

  .subtitle {
    margin-bottom: 30px;
    color: #4a5568; /* Subdued text */
    font-size: 1rem;
  }

  .input-group {
    margin-bottom: 20px;
    text-align: left;
  }

    .input-group label {
      display: block;
      margin-bottom: 8px;
      font-weight: 600;
      color: #2d3748;
    }

    .input-group input {
      width: 100%;
      padding: 12px;
      border: 1px solid #cbd5e0;
      border-radius: 6px;
      box-sizing: border-box;
      transition: border-color 0.2s, box-shadow 0.2s;
    }

      .input-group input:focus {
        border-color: #3182ce; /* Blue focus ring */
        box-shadow: 0 0 0 3px rgba(49, 130, 206, 0.2);
        outline: none;
      }

  .login-button {
    width: 100%;
    padding: 12px;
    background-color: #3182ce; /* Blue primary color */
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 16px;
    font-weight: 700;
    transition: background-color 0.3s, opacity 0.3s;
  }

    .login-button:hover:not(:disabled) {
      background-color: #2c5282;
    }

    .login-button:disabled {
      background-color: #90cdf4;
      cursor: not-allowed;
      opacity: 0.7;
    }

  .error-message {
    color: #e53e3e; /* Red error message */
    margin-bottom: 15px;
    font-weight: 500;
    background-color: #fff5f5;
    padding: 10px;
    border-radius: 4px;
    border: 1px solid #fed7d7;
  }

  .mt-4 {
    margin-top: 1rem;
  }

  .text-sm {
    font-size: 0.875rem;
  }

  .text-gray-500 {
    color: #a0aec0;
  }
</style>
