import axios from 'axios';
import AuthService from './AuthService'; // To get the token

// Adjust the port if necessary
const API_URL = 'https://localhost:7122/api/dashboard';

const DashboardService = {
  /**
   * Fetches key metrics for the administrative dashboard.
   * Requires an authenticated admin token.
   */
  async getAdminMetrics() {
    const token = AuthService.getAuthToken();
    if (!token) {
      throw new Error('User not authenticated.');
    }

    try {
      const response = await axios.get(`${API_URL}/admin-metrics`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching admin metrics:', error);
      // This is critical: if the token is invalid (401), force logout
      if (error.response && error.response.status === 401) {
        AuthService.logout();
        // We'll let the component handle the redirect
        throw new Error('Session expired or unauthorized. Logging out.');
      }
      throw new Error(error.response?.data?.message || 'Failed to retrieve dashboard data.');
    }
  }
};

export default DashboardService;
