import axios from 'axios';
import AuthService from './AuthService';

// IMPORTANT: Define the base URL for your Admin endpoints
const API_BASE_URL = 'https://localhost:7132/api/admin/users';

class AdminUserService {

  // --- Helper to get authorization header ---
  getAuthHeader() {
    const token = AuthService.getToken();
    if (!token) {
      // Force logout if the token is missing when needed
      AuthService.logout();
      throw new Error("Authentication token missing. Logging out.");
    }
    return {
      headers: { 'Authorization': `Bearer ${token}` }
    };
  }

  // =========================================================
  //                    SYSTEM USER CRUD (UserType 1 & 2)
  // =========================================================

  /**
   * READ: Fetches a list of all System Users and Super Admins.
   */
  async getSystemUsers() {
    try {
      const response = await axios.get(`${API_BASE_URL}/system`, this.getAuthHeader());
      return response.data;
    } catch (error) {
      console.error("API Error fetching System Users:", error);
      throw error;
    }
  }

  /**
   * CREATE: Creates a new System User (UserType 1 or 2).
   * @param {Object} userData - User details (Username, Email, Pass, Name, etc.)
   */
  async createSystemUser(userData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/system`, userData, this.getAuthHeader());
      return response.data; // Should return success message or created user object
    } catch (error) {
      console.error("API Error creating System User:", error);
      throw error;
    }
  }

  /**
   * UPDATE: Updates an existing System User's details or role.
   * @param {number} userId - ID of the user to update
   * @param {Object} userData - Updated fields (Email, FirstName, LastName, UserType, NewPassword, etc.)
   */
  async updateSystemUser(userId, userData) {
    try {
      const response = await axios.put(`${API_BASE_URL}/system/${userId}`, userData, this.getAuthHeader());
      return response.data;
    } catch (error) {
      console.error(`API Error updating System User ${userId}:`, error);
      throw error;
    }
  }

  /**
   * DELETE: Deletes a System User by ID.
   * @param {number} userId - ID of the user to delete
   */
  async deleteSystemUser(userId) {
    try {
      const response = await axios.delete(`${API_BASE_URL}/system/${userId}`, this.getAuthHeader());
      return response.data; // Should return success message
    } catch (error) {
      console.error(`API Error deleting System User ${userId}:`, error);
      throw error;
    }
  }

  // =========================================================
  //                    KIT USER CRUD (UserType 3)
  // =========================================================

  /**
   * READ: Fetches a list of all Kit Users.
   */
  async getKitUsers() {
    try {
      const response = await axios.get(`${API_BASE_URL}/kit`, this.getAuthHeader());
      return response.data;
    } catch (error) {
      console.error("API Error fetching Kit Users:", error);
      throw error;
    }
  }

  /**
   * CREATE: Creates a new Kit User (UserType 3).
   * @param {Object} userData - User details (Username, Email, Pass, Name, etc.)
   */
  async createKitUser(userData) { // <-- NEW METHOD ADDED HERE
    try {
      // Assuming your backend API uses POST /api/admin/users/kit for creation
      const response = await axios.post(`${API_BASE_URL}/kit`, userData, this.getAuthHeader());
      return response.data;
    } catch (error) {
      console.error("API Error creating Kit User:", error);
      throw error;
    }
  }

  /**
   * UPDATE: Updates an existing Kit User's details.
   * @param {number} userId - ID of the user to update
   * @param {Object} userData - Updated fields (Email, FirstName, LastName, NewPassword, etc.)
   */
  async updateKitUser(userId, userData) {
    try {
      const response = await axios.put(`${API_BASE_URL}/kit/${userId}`, userData, this.getAuthHeader());
      return response.data;
    } catch (error) {
      console.error(`API Error updating Kit User ${userId}:`, error);
      throw error;
    }
  }

  /**
   * DELETE: Deletes a Kit User by ID.
   * @param {number} userId - ID of the user to delete
   */
  async deleteKitUser(userId) {
    try {
      const response = await axios.delete(`${API_BASE_URL}/kit/${userId}`, this.getAuthHeader());
      return response.data;
    } catch (error) {
      console.error(`API Error deleting Kit User ${userId}:`, error);
      throw error;
    }
  }
}

export default new AdminUserService();
