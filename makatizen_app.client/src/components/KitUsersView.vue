<template>
  <div class="user-management-container">
    <div class="header-bar">
      <h1 class="title">Kit User Management 📱</h1>
      <button class="action-button primary-btn"
                     @click="openCreateModal">
        + Create New Kit User
      </button>
    </div>

    <div v-if="loading" class="loading-state">
      <p>Loading Kit Users...</p>
    </div>
    <div v-else-if="error" class="error-box">
      <p>{{ error }}</p>
    </div>
    <div v-else class="table-wrapper">
      <div v-if="successMessage" class="success-alert">{{ successMessage }}</div>

      <table class="user-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Username</th>
            <th>Email</th>
            <th>Name</th>
            <th>Status</th>
            <th>Created At</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in kitUsers" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.username }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.firstName }} {{ user.lastName }}</td>
            <td>
              <span :class="getStatusClass(user.mustResetPassword)">
                {{ user.mustResetPassword ? 'Reset Required' : 'Active' }}
              </span>
            </td>
            <td>{{ formatDate(user.createdAt) }}</td>
            <td class="action-cell">
              <button class="action-icon edit-btn" @click="openUpdateModal(user)">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                  <path d="M15.526 2.544a.2.2 0 0 1 .305.153l-.841 4.205a.2.2 0 0 1-.153.153L7.7 8.526l-.412 1.646a.5.5 0 0 1-.6.375l-1.646-.412-4.205.841a.2.2 0 0 1-.153-.305L3.197 6.13a.2.2 0 0 1 .153-.153l4.205-.841 1.646-.412a.5.5 0 0 1 .375-.6l-.412-1.646a.2.2 0 0 1 .305-.153l4.205.841a.2.2 0 0 1 .153.305l-1.646.412 1.646-.412z" />
                </svg>
              </button>
              <button class="action-icon delete-btn" @click="openDeleteModal(user)">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                  <path d="M6.5 1h3a.5.5 0 0 1 .5.5v1H6v-1a.5.5 0 0 1 .5-.5ZM11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3A1.5 1.5 0 0 0 5 1.5v1H2.5a.5.5 0 0 0 0 1h.5v9a2 2 0 0 0 2 2h7a2 2 0 0 0 2-2v-9h.5a.5.5 0 0 0 0-1H11ZM8 4.5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5ZM6 4.5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5ZM10 4.5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5Z" />
                </svg>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <UserFormModal :isVisible="isCreateModalOpen"
                             :isEdit="false"
                             :initialData="{ userType: 3 }"
                             @close="closeModal"
                             @save="handleCreateUser"
                             title="Create New Kit User" />

    <UserFormModal :isVisible="isUpdateModalOpen"
                             :isEdit="true"
                             :initialData="selectedUser"
                             @close="closeModal"
                             @save="handleUpdateUser"
                             title="Update Kit User" />

    <ConfirmDeleteModal :isVisible="isDeleteModalOpen"
                                    :userName="selectedUser.username"
                                    @close="closeModal"
                                    @confirm="handleDeleteUser" />
  </div>
</template>

<script>
  // Assuming you have a service dedicated to Kit User administration
  // If not, this is a placeholder you need to implement.
  import AdminUserService from '@/services/AdminUserService';
  import UserFormModal from '@/components/UserFormModal.vue';
  import ConfirmDeleteModal from '@/components/ConfirmDeleteModal.vue';

  export default {
    name: 'KitUsersView',
    components: {
      UserFormModal,
      ConfirmDeleteModal
    },
    data() {
      return {
        loading: true,
        error: null,
        successMessage: '',
        kitUsers: [],

        // Modal State
        isCreateModalOpen: false,
        isUpdateModalOpen: false,
        isDeleteModalOpen: false,
        selectedUser: {} // Holds the data for the user currently being edited/deleted
      };
    },
    async mounted() {
      await this.fetchUsers();
    },
    methods: {
      // --- Data Fetching ---
      async fetchUsers() {
        this.loading = true;
        this.error = null;
        try {
          // Assume AdminUserService has a specific method for Kit Users
          this.kitUsers = await AdminUserService.getKitUsers();
        } catch (err) {
          this.error = 'Failed to load Kit User data. Please check the API status.';
          console.error(err);
        } finally {
          this.loading = false;
        }
      },

      // --- Modals ---
      openCreateModal() {
        this.isCreateModalOpen = true;
        // Note: userType is set to 3 for Kit Users
        this.selectedUser = { userType: 3, username: '', email: '', password: '', firstName: '', lastName: '', birthDate: '' };
      },
      openUpdateModal(user) {
        this.selectedUser = { ...user };
        // Kit users don't typically have birthdate, but include for consistency if DTO requires
        if (this.selectedUser.birthDate) {
          this.selectedUser.birthDate = new Date(this.selectedUser.birthDate).toISOString().split('T')[0];
        }
        this.isUpdateModalOpen = true;
      },
      openDeleteModal(user) {
        this.selectedUser = { ...user };
        this.isDeleteModalOpen = true;
      },
      closeModal() {
        this.isCreateModalOpen = false;
        this.isUpdateModalOpen = false;
        this.isDeleteModalOpen = false;
        this.selectedUser = {};
      },

      // --- CRUD Handlers ---
      async handleCreateUser(formData) {
        this.clearStatus();
        try {
          // FIX: Map camelCase formData to PascalCase payload for C# backend DTO
          const payload = {
            Username: formData.username,
            Email: formData.email,
            Password: formData.password,
            FirstName: formData.firstName,
            LastName: formData.lastName,
            UserType: 3 // Explicitly set UserType 3 for Kit Users
          };

          await AdminUserService.createKitUser(payload);
          this.successMessage = `Kit User ${payload.Username} created successfully!`;
          await this.fetchUsers(); // Refresh the list
          this.closeModal();
        } catch (err) {
          console.error('Create User Error:', err);
          this.error = `Creation failed: ${err.response?.data?.message || err.message || 'Unknown error'}`;
        }
      },

      async handleUpdateUser(formData) {
        this.clearStatus();
        try {
          // FIX: Map camelCase formData to PascalCase payload for C# backend DTO
          const payload = {
            Email: formData.email,
            FirstName: formData.firstName,
            LastName: formData.lastName,
            MustResetPassword: formData.mustResetPassword, // Corrected casing
            // Only include NewPassword if it exists and is not empty
            ...(formData.newPassword && { NewPassword: formData.newPassword })
          };

          await AdminUserService.updateKitUser(formData.id, payload);
          this.successMessage = `Kit User ${formData.username} updated successfully!`;
          await this.fetchUsers(); // Refresh the list
          this.closeModal();
        } catch (err) {
          console.error('Update User Error:', err);
          this.error = `Update failed: ${err.response?.data?.message || err.message || 'Unknown error'}`;
        }
      },

      async handleDeleteUser() {
        this.clearStatus();
        try {
          const userId = this.selectedUser.id;
          await AdminUserService.deleteKitUser(userId);
          this.successMessage = `Kit User ${this.selectedUser.username} deleted successfully!`;
          await this.fetchUsers(); // Refresh the list
          this.closeModal();
        } catch (err) {
          console.error('Delete User Error:', err);
          this.error = `Deletion failed: ${err.response?.data?.message || err.message || 'Unknown error'}`;
        }
      },

      // --- Utilities ---
      getStatusClass(mustResetPassword) {
        return {
          'status-reset-required': mustResetPassword,
          'status-active': !mustResetPassword
        };
      },
      formatDate(dateString) {
        return new Date(dateString).toLocaleDateString('en-US', {
          year: 'numeric', month: 'short', day: 'numeric'
        });
      },
      clearStatus() {
        this.error = null;
        this.successMessage = '';
      }
    }
  };
</script>

<style scoped>

  /* Main Container and Header - Reusing styles from SystemUsersView */
    .user-management-container {
        max-width: 1400px;
        margin: 0 auto;
        padding: 30px;

  }

    .header-bar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 30px;
        border-bottom: 3px solid #e9ecef;
        padding-bottom: 20px;

  }

    .title {
        font-size: 2rem;
        font-weight: 800;
        color: #004d99;

  }


  /* Status Messages */
    .loading-state {
        text-align: center;
        padding: 50px;
        font-size: 1.2rem;
        color: #007bff;

  }

    .error-box, .success-alert {
        padding: 15px;
        border-radius: 8px;
        margin-bottom: 20px;
        font-weight: 600;

  }

    .error-box {
        background-color: #f8d7da;
        color: #721c24;
        border: 1px solid #f5c6cb;

  }

    .success-alert {
        background-color: #d4edda;
        color: #155724;
        border: 1px solid #c3e6cb;

  }



  /* Table Styles */
    .table-wrapper {
        background: white;
        border-radius: 12px;
        box-shadow: 0 4px 18px rgba(0, 0, 0, 0.05);
        overflow-x: auto;

  }

    .user-table {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0;

  }

      .user-table th, .user-table td {
          padding: 15px 20px;
          text-align: left;
          border-bottom: 1px solid #f1f1f1;

  }

      .user-table th {
          background-color: #f8f9fa;
          color: #495057;
          font-weight: 700;
          text-transform: uppercase;
          font-size: 0.85rem;

  }

      .user-table tr:hover {
          background-color: #eaf6ff;

  }


  /* Status Badges */
    .status-active, .status-reset-required {
        display: inline-block;
        padding: 5px 10px;
        border-radius: 20px;
        font-size: 0.8rem;
        font-weight: 700;

  }

    .status-active {
        background-color: #28a745; /* Green */
        color: white;

  }

    .status-reset-required {
        background-color: #ffc107; /* Orange/Yellow */
        color: #343a40;

  }


  /* Action Buttons */
    .action-cell {
        width: 120px;
        white-space: nowrap;

  }

    .action-icon {
        background: none;
        border: none;
        cursor: pointer;
        margin-right: 10px;
        transition: transform 0.2s;
        padding: 5px;
        border-radius: 4px;

  }

      .action-icon:hover {
          transform: scale(1.1);

  }

    .edit-btn svg {
        color: #007bff; /* Blue */

  }

    .delete-btn svg {
        color: #dc3545; /* Red */

  }


  /* Primary Button Style (for Create) */
    .primary-btn {
        background-color: #007bff; /* Blue for Kit Users */
        color: white;
        padding: 10px 20px;
        border: none;
        border-radius: 8px;
        font-weight: 600;
        cursor: pointer;
        box-shadow: 0 4px 10px rgba(0, 123, 255, 0.2);
        transition: background-color 0.3s;

  }

        .primary-btn:hover {
            background-color: #0056b3;

    }
</style>
