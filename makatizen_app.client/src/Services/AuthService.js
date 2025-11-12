import axios from 'axios';
import { jwtDecode } from 'jwt-decode'; // Ensure you run 'npm install jwt-decode'

// The base URL for your ASP.NET Core backend AuthController
// Adjust the port if needed (e.g., to 7122 or 5001)
const API_URL = 'https://localhost:7122/api/auth';
const TOKEN_KEY = 'makatizen_auth_token';
const USER_ROLE_KEY = 'makatizen_user_role';
const MUST_RESET_KEY = 'makatizen_must_reset';

// Helper function to handle token storage
const setAuthData = (token, role, mustReset) => {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_ROLE_KEY, role);
  localStorage.setItem(MUST_RESET_KEY, mustReset ? 'true' : 'false');
};

const AuthService = {
  /**
   * Attempts to log in the user and stores the returned JWT, role, and reset flag.
   * @param {string} username 
   * @param {string} password 
   * @returns {object} { mustResetPassword: bool, role: string }
   */
  async login(username, password) {
    try {
      const response = await axios.post(`${API_URL}/login`, { username, password });
      const { token, mustResetPassword, role } = response.data;

      if (token) {
        setAuthData(token, role, mustResetPassword);
      }

      return { mustResetPassword, role };
    } catch (error) {
      // Re-throw specific error for the component to handle
      const message = error.response?.data?.message || 'Login failed due to network or server error.';
      throw new Error(message);
    }
  },

  /**
   * Resets the user's password using the existing token.
   * @param {string} newPassword 
   */
  async resetPassword(newPassword) {
    const token = this.getAuthToken();
    if (!token) {
      throw new Error('No authentication token found for password reset.');
    }

    try {
      // The ASP.NET Core controller expects the token in the Authorization header
      await axios.post(`${API_URL}/reset-password`, { newPassword }, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      // If reset is successful, update the reset flag in storage
      localStorage.setItem(MUST_RESET_KEY, 'false');

    } catch (error) {
      const message = error.response?.data?.message || 'Password reset failed.';
      throw new Error(message);
    }
  },

  /**
   * Clears all authentication data from local storage.
   */
  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_ROLE_KEY);
    localStorage.removeItem(MUST_RESET_KEY);
    // Also remove any existing axios default headers if used
    if (axios.defaults.headers.common['Authorization']) {
      delete axios.defaults.headers.common['Authorization'];
    }
  },

  /**
   * Checks if a token exists in local storage.
   * @returns {boolean}
   */
  isAuthenticated() {
    return !!localStorage.getItem(TOKEN_KEY);
  },

  /**
   * Retrieves the stored JWT.
   * @returns {string | null}
   */
  getAuthToken() {
    return localStorage.getItem(TOKEN_KEY);
  },

  /**
   * Retrieves the stored user role.
   * @returns {string | null}
   */
  getUserRole() {
    return localStorage.getItem(USER_ROLE_KEY);
  },

  /**
   * Checks if the mandatory password reset flag is set.
   * @returns {boolean}
   */
  mustResetPassword() {
    return localStorage.getItem(MUST_RESET_KEY) === 'true';
  },

  /**
   * Attaches the token to all subsequent Axios requests. 
   * (Optional, but useful for API calls outside of Login/Reset)
   */
  setAuthHeader() {
    const token = this.getAuthToken();
    if (token) {
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    } else {
      delete axios.defaults.headers.common['Authorization'];
    }
  }
};

export default AuthService;
