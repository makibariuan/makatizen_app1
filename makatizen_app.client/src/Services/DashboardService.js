import axios from 'axios';
import AuthService from './AuthService';

// IMPORTANT: Ensure this URL matches your ASP.NET Core API root
const API_BASE_URL = 'https://localhost:7132/api/admin/dashboard';

class DashboardService {
  /**
   * Fetches the summary metrics for the Admin Dashboard.
   * @returns {Promise<Object>} The dashboard summary data.
   */
  async getAdminMetrics() {
    try {
      const token = AuthService.getToken();
      if (!token) {
        throw new Error("User not authenticated. Logging out...");
      }

      const response = await axios.get(`${API_BASE_URL}/summary`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      // The API response contains the data object directly
      return response.data;

    } catch (error) {
      // Handle 401/403 errors and other network issues
      if (error.response && (error.response.status === 401 || error.response.status === 403)) {
        AuthService.logout();
        throw new Error(`Authentication expired or unauthorized access. Logging out for security.`);
      }
      // Re-throw other errors
      throw new Error(`Failed to fetch dashboard metrics: ${error.message}`);
    }
  }

  // Future methods for fetching detailed data can go here
}

export default new DashboardService();
