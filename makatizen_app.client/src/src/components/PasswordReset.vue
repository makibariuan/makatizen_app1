<template>
  <div class="reset-container">
    <div class="reset-box">
      <h2 class="title">Security Required 🛡️</h2>
      <p class="subtitle">
        You must reset your password before continuing. Please choose a strong new password.
      </p>

      <form @submit.prevent="handleReset">

        <!-- New Password Input -->
        <div class="input-group">
          <label for="newPassword">New Password</label>
          <input type="password"
                 id="newPassword"
                 v-model="newPassword"
                 required
                 :disabled="loading" />
        </div>

        <!-- Confirm Password Input -->
        <div class="input-group">
          <label for="confirmPassword">Confirm Password</label>
          <input type="password"
                 id="confirmPassword"
                 v-model="confirmPassword"
                 required
                 :disabled="loading" />
        </div>

        <!-- Feedback Messages -->
        <p v-if="passwordMismatch" class="error-message">Passwords do not match.</p>
        <p v-if="error && !passwordMismatch" class="error-message">{{ error }}</p>
        <p v-if="successMessage" class="success-message">{{ successMessage }}</p>

        <!-- Submit Button -->
        <button type="submit"
                :disabled="loading || passwordMismatch || !newPassword"
                class="reset-button">
          {{ loading ? 'Resetting...' : 'Set New Password' }}
        </button>
      </form>
    </div>
  </div>
</template>



<script>
import AuthService from '@/services/AuthService';

export default {
  name: 'PasswordResetView',
  data() {
    return {
      newPassword: '',
      confirmPassword: '',
      error: '',
      successMessage: '',
      loading: false,
    };
  },
  computed: {
    passwordMismatch() {
      return this.newPassword && this.confirmPassword && this.newPassword !== this.confirmPassword;
    }
  },
  methods: {
    async handleReset() {
      this.error = '';
      this.successMessage = '';

      if (this.passwordMismatch) return;

      const token = AuthService.getAuthToken();
      if (!token) {
        this.error = 'User not authenticated. Please log in again.';
        AuthService.logout();
        this.$router.push({ name: 'Login' });
        return;
      }

      this.loading = true;

      try {
        await AuthService.resetPassword(this.newPassword, token);

        this.successMessage = 'Password successfully reset! Redirecting to dashboard...';

        // Clear local storage and log the user in again with the new password
        AuthService.logout();

        // After successful reset, re-login the user (optional, but cleaner flow)
        // We need the original username to perform the relogin, but since we
        // don't store it, we'll just redirect to login for simplicity and security.
        setTimeout(() => {
          this.$router.push({ name: 'Login' });
        }, 3000);

      } catch (err) {
        this.error = err.toString().includes('failed') ? err : 'An unexpected error occurred during password reset.';
        // If the token is invalid (e.g., expired), force logout
        if (err.response && err.response.status === 401) {
             AuthService.logout();
             this.$router.push({ name: 'Login' });
        }
      } finally {
        this.loading = false;
        this.newPassword = '';
        this.confirmPassword = '';
      }
    }
  },
  created() {
    // If the user isn't logged in, they shouldn't be here
    if (!AuthService.isAuthenticated()) {
      this.$router.push({ name: 'Login' });
    }
  }
};
</script>

<style scoped>
  .reset-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background-color: #f4f6f9;
  }

  .reset-box {
    background: white;
    padding: 40px;
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    width: 100%;
    max-width: 450px;
    text-align: center;
  }

  .title {
    margin-bottom: 10px;
    color: #007bff;
  }

  .subtitle {
    margin-bottom: 30px;
    color: #6c757d;
  }

  .input-group {
    margin-bottom: 20px;
    text-align: left;
  }

    .input-group label {
      display: block;
      margin-bottom: 8px;
      font-weight: bold;
      color: #333;
    }

    .input-group input {
      width: 100%;
      padding: 10px;
      border: 1px solid #ccc;
      border-radius: 4px;
      box-sizing: border-box;
    }

  .reset-button {
    width: 100%;
    padding: 12px;
    background-color: #28a745; /* Success Green */
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 16px;
    transition: background-color 0.3s;
  }

    .reset-button:hover:not(:disabled) {
      background-color: #1e7e34;
    }

    .reset-button:disabled {
      background-color: #90ee90;
      cursor: not-allowed;
    }

  .error-message {
    color: #dc3545;
    margin-bottom: 15px;
    font-weight: 500;
  }

  .success-message {
    color: #28a745;
    margin-bottom: 15px;
    font-weight: 500;
  }
</style>
