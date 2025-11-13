<template>
  <div class="dashboard-wrapper">
    <button class="sidebar-toggle" @click="toggleSidebar" aria-label="Toggle sidebar">
      <span class="bar"></span>
      <span class="bar"></span>
      <span class="bar"></span>
    </button>

    <Sidebar :activeView="activeView" :sidebarOpen="sidebarOpen" @switch-view="switchView" />

    <main class="dashboard-container">
      <h1 class="welcome-title">Administrator Dashboard 🛡️</h1>
      <p class="role-info">
        Welcome back, {{ getRoleName(userRole) }}. Use the sections below to manage users and citizen data.
      </p>

      <section v-if="activeView === 'kitUsers' || activeView === 'systemUsers'">
        <h2 class="section-title">{{ activeView === 'kitUsers' ? 'Kit Users' : 'System Users' }}</h2>
        <button class="add-btn" @click="openAddForm">+ Add User</button>

        <table class="user-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Username</th>
              <th>Role</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in userList" :key="user.id">
              <td>{{ user.id }}</td>
              <td>{{ user.username }}</td>
              <td>{{ getRoleName(user.userType) }}</td>
              <td>
                <button class="edit-btn" @click="editUser(user)">Edit</button>
                <button class="delete-btn" @click="deleteUser(user.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </section>

      <section v-else-if="activeView === 'citizenRecords'">
        <h2 class="section-title">Citizen Records</h2>
        <p class="metric-number">{{ metrics.citizenCount }}</p>
        <p>Total enrolled citizens with biometric and personal records.</p>
        <p class="detail">Last Enrollment: {{ metrics.lastEnrollmentDate || 'N/A' }}</p>
        <button class="action-button success">View Citizen Records</button>
      </section>

      <div v-if="showForm" class="form-overlay">
        <div class="form-popup">
          <h3>{{ editingUser ? 'Edit User' : 'Create New User' }}</h3>
          <form @submit.prevent="saveUser">
            <label>Username</label>
            <input v-model="form.username" required />

            <label>Password</label>
            <input v-model="form.password" type="password" :required="!editingUser" />

            <label>Role</label>
            <select v-model.number="form.userType" required>
              <option :value="1">Super Admin</option>
              <option :value="2">System User</option>
              <option v-if="activeView === 'kitUsers' || form.userType === 3" :value="3">Kit User</option>
            </select>

            <div class="form-actions">
              <button type="submit" class="save-btn">Save</button>
              <button type="button" class="cancel-btn" @click="closeForm">Cancel</button>
            </div>
          </form>
        </div>
      </div>

      <div v-if="loading" class="loading-overlay">
        <p>Loading data...</p>
      </div>

      <div v-if="error" class="error-box">
        <p>{{ error }}</p>
      </div>
    </main>
  </div>
</template>

<script>
  import Sidebar from '@/components/Sidebar.vue';
  import AuthService from '@/services/AuthService';
  import DashboardService from '@/services/DashboardService';

  // Helper mapping to convert int roles to display strings
  const ROLE_MAP = {
    1: 'Super Admin',
    2: 'System User',
    3: 'Kit User'
  };

  export default {
    name: 'DashboardAdminView',
    components: { Sidebar },
    data() {
      // Get the role from AuthService, convert it to an integer, or default to 1 (Super Admin)
      const rawUserRole = AuthService.getUserRole();
      const userRoleInt = parseInt(rawUserRole, 10);

      return {
        // 🚀 DATA FIX 1: Store role as INT
        userRole: isNaN(userRoleInt) ? 1 : userRoleInt,
        loading: false,
        error: null,
        metrics: {
          systemUserCount: 0,
          kitUserCount: 0,
          citizenCount: 0,
          lastEnrollmentDate: null
        },
        activeView: 'kitUsers',
        userList: [],
        showForm: false,
        editingUser: null,
        form: {
          username: '',
          password: '',
          // 🚀 DATA FIX 2: Initialize role as a number
          userType: 2 // Default to System User (2) for new system users, or Kit User (3) for kit
        },
        sidebarOpen: true
      };
    },
    async mounted() {
      await this.fetchMetrics();
      await this.fetchUsers(this.activeView);
    },
    methods: {
      getRoleName(userType) {
        // Helper to convert the integer role to a readable string for the template
        return ROLE_MAP[userType] || 'Unknown';
      },
      toggleSidebar() {
        this.sidebarOpen = !this.sidebarOpen;
      },
      switchView(view) {
        this.activeView = view;
        this.showForm = false;
        this.editingUser = null;
        if (view === 'kitUsers' || view === 'systemUsers') {
          this.fetchUsers(view);
        }
      },
      openAddForm() {
        this.editingUser = null;
        // 🚀 METHOD FIX 1: Set default role using the integer
        this.form = {
          username: '',
          password: '',
          userType: this.activeView === 'kitUsers' ? 3 : 2 // 3 for KitUser, 2 for SystemUser
        };
        this.showForm = true;
      },
      editUser(user) {
        this.editingUser = user;
        // User list data should now contain userType (int)
        this.form = {
          username: user.username,
          // 🚀 METHOD FIX 2: Populate form with the integer userType from the user object
          userType: user.userType,
          password: ''
        };
        this.showForm = true;
      },
      closeForm() {
        this.showForm = false;
      },

      // --- API Calls ---
      async fetchMetrics() {
        this.loading = true;
        this.error = null;
        try {
          const data = await DashboardService.getAdminMetrics();
          this.metrics = { ...this.metrics, ...data };
        } catch (err) {
          this.error = err.message || 'Failed to load dashboard metrics.';
        } finally {
          this.loading = false;
        }
      },

      async fetchUsers(view) {
        this.loading = true;
        this.error = null;
        try {
          const token = localStorage.getItem('jwt_token');
          let url = '';

          if (view === 'kitUsers') {
            url = 'https://localhost:7122/api/admin/users/kit'; // Assuming this uses the AdminUsersController path
          } else if (view === 'systemUsers') {
            url = 'https://localhost:7122/api/admin/users/system'; // Assuming this uses the AdminUsersController path
          }

          const response = await fetch(url, {
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          });

          if (!response.ok) {
            throw new Error(`Failed to load ${view}. Status: ${response.status}`);
          }

          const data = await response.json();
          this.userList = data.map(user => ({
            // This is the CRITICAL change: Ensure you receive/store the userType (int)
            id: user.id, // Assuming 'id' is consistent
            username: user.username,
            userType: user.userType, // 🚀 API FIX: Storing the INT UserType from the API response
            role: this.getRoleName(user.userType) // Keeping this for backward compatibility in the template, though not needed now
          }));

        } catch (err) {
          this.error = err.message || 'Failed to load user list.';
        } finally {
          this.loading = false;
        }
      },

      async saveUser() {
        this.loading = true;
        this.error = null;
        const isCreating = !this.editingUser;
        const method = isCreating ? 'POST' : 'PUT';

        // 🚀 METHOD FIX 3: Determine endpoint based on the numeric userType
        const roleEndpoint = this.form.userType === 3 ? 'KitUser' : 'SystemUser';

        const baseUrl = `https://localhost:7122/api/admin/users/${roleEndpoint.toLowerCase()}`;
        const url = isCreating ? baseUrl : `${baseUrl}/${this.editingUser.id}`;

        try {
          const token = localStorage.getItem('jwt_token');

          // Prepare the DTO body
          const body = {
            username: this.form.username,
            password: this.form.password,
            // 🚀 METHOD FIX 4: Send the userType (int) in the DTO
            userType: this.form.userType
            // Note: If your DTOs require other fields like Email, FirstName, etc.,
            // you must add them here and to the form data.
          };

          // If editing and password is empty, do not send the password property
          if (!isCreating && !body.password) {
            delete body.password;
          }

          const response = await fetch(url, {
            method,
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            },
            body: JSON.stringify(body),
          });

          if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Failed to save user: ${errorText || response.statusText}`);
          }

          await this.fetchUsers(this.activeView);
          this.showForm = false;
          this.editingUser = null;
          alert('✅ User saved successfully!');
        } catch (err) {
          this.error = err.message || 'Failed to save user.';
        } finally {
          this.loading = false;
        }
      },

      async deleteUser(id) {
        if (!confirm('Are you sure you want to delete this user?')) {
          return;
        }

        this.loading = true;
        this.error = null;

        try {
          const token = localStorage.getItem('jwt_token');
          let roleEndpoint = this.activeView === 'kitUsers' ? 'kit' : 'system';

          // Use the correct AdminUsersController path
          const baseUrl = `https://localhost:7122/api/admin/users/${roleEndpoint}`;

          const response = await fetch(`${baseUrl}/${id}`, {
            method: 'DELETE',
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          });

          if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Failed to delete user.');
          }

          await this.fetchUsers(this.activeView);
          alert('🗑️ User deleted successfully!');
        } catch (err) {
          this.error = err.message || 'Failed to delete user.';
        } finally {
          this.loading = false;
        }
      }
    }
  };
</script>

<style scoped>
  /* Blue Color Palette Definitions (Used Tailwind CSS values as a guide) */
  /* Primary Blue: #007bff (A vibrant blue) */
  /* Dark Blue: #0056b3 (For hover states) */
  /* Very Dark Blue/Navy: #002e4d (For titles) */
  /* Light Blue Background: #f0f7ff (or similar light wash) */

  /* ------------------- MAIN LAYOUT ------------------- */
  .dashboard-wrapper {
    display: flex;
    min-height: 100vh;
    width: 100vw;
    margin: 0;
    padding: 0;
    /* 🔵 CHANGE: Light blue/white gradient */
    background: linear-gradient(135deg, #eaf3ff, #f9ffff);
    overflow-x: hidden;
  }

  .dashboard-container {
    flex-grow: 1;
    padding: 40px 35px;
    overflow-y: auto;
    font-family: 'Inter', sans-serif;
    color: #2d3748;
  }

  /* ------------------- HEADER ------------------- */
  .welcome-title {
    font-size: 2.8rem;
    font-weight: 800;
    margin-bottom: 10px;
    /* 🔵 CHANGE: Dark Navy Blue */
    color: #002e4d;
  }

  .role-info {
    font-size: 1.15rem;
    color: #4a5568;
    margin-bottom: 40px;
    border-bottom: 2px solid #e9ecef;
    padding-bottom: 20px;
  }

  /* ------------------- TABLE ------------------- */
  .section-title {
    font-weight: 700;
    color: #2d3748;
    font-size: 1.6rem;
    margin-bottom: 20px;
  }

  .add-btn {
    /* 🔵 CHANGE: Primary Blue */
    background: #007bff;
    color: #fff;
    padding: 12px 20px;
    border: none;
    border-radius: 10px;
    cursor: pointer;
    font-weight: 700;
    /* 🔵 CHANGE: Blue shadow */
    box-shadow: 0 4px 15px rgba(0, 123, 255, 0.25);
    transition: all 0.3s ease;
  }

    .add-btn:hover {
      /* 🔵 CHANGE: Darker Blue on hover */
      background: #0056b3;
      transform: translateY(-2px);
    }

  .user-table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 0 10px;
    margin-top: 20px;
  }

    .user-table th {
      text-align: left;
      padding: 12px;
      color: #475569;
    }

    .user-table td {
      background: white;
      padding: 15px;
      border-radius: 8px;
      box-shadow: 0 3px 6px rgba(0, 0, 0, 0.08);
    }

  /* Edit/Delete Buttons (Kept Yellow/Red as they are standard indicators) */
  .edit-btn, .delete-btn {
    padding: 8px 15px;
    margin-right: 5px;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: background-color 0.2s;
  }

  .edit-btn {
    background-color: #fcd34d; /* Yellow-400 */
    color: #78350f; /* Amber-900 */
  }

    .edit-btn:hover {
      background-color: #fbbd23;
    }

  .delete-btn {
    background-color: #f87171; /* Red-400 */
    color: #7f1d1d; /* Red-900 */
  }

    .delete-btn:hover {
      background-color: #ef4444;
    }

  /* Citizen Records Section Styling */
  .metric-number {
    font-size: 3rem;
    font-weight: 800;
    /* 🔵 CHANGE: Primary Blue */
    color: #007bff;
    margin-bottom: 10px;
  }

  .action-button {
    padding: 12px 20px;
    border: none;
    border-radius: 10px;
    font-weight: 700;
    cursor: pointer;
    margin-top: 20px;
    transition: all 0.3s ease;
  }

    .action-button.success {
      /* 🔵 CHANGE: Secondary Blue (used for 'success' action) */
      background-color: #1e87f0;
      color: white;
    }

      .action-button.success:hover {
        /* 🔵 CHANGE: Darker Secondary Blue */
        background-color: #0f62c6;
      }

  /* ------------------- POPUP MODAL ------------------- */
  .form-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(10, 20, 25, 0.4);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 2000;
    backdrop-filter: blur(6px);
    opacity: 1;
    transition: opacity 0.25s ease;
  }

    .form-overlay.fade-out {
      opacity: 0;
      pointer-events: none;
    }

  .form-popup {
    background: rgba(255, 255, 255, 0.9);
    backdrop-filter: blur(12px);
    padding: 35px 40px;
    border-radius: 20px;
    width: 90%;
    max-width: 450px;
    /* 🔵 CHANGE: Blue shadow */
    box-shadow: 0 10px 30px rgba(0, 123, 255, 0.25);
    animation: popupIn 0.25s ease forwards;
    /* 🔵 CHANGE: Light blue border */
    border: 2px solid rgba(0, 123, 255, 0.15);
  }


  @keyframes popupIn {
    from {
      transform: translateY(-20px);
      opacity: 0;
    }

    to {
      transform: translateY(0);
      opacity: 1;
    }
  }

  .form-popup h3 {
    text-align: center;
    margin-bottom: 25px;
    /* 🔵 CHANGE: Dark Navy Blue */
    color: #002e4d;
    font-weight: 700;
  }

  .form-popup label {
    display: block;
    font-weight: 600;
    margin-top: 10px;
    color: #334155;
  }

  .form-popup input,
  .form-popup select {
    width: 100%;
    padding: 12px 15px;
    border-radius: 10px;
    border: 1.5px solid #cbd5e1;
    font-size: 1rem;
    margin-top: 6px;
    transition: border-color 0.3s ease;
  }

    .form-popup input:focus,
    .form-popup select:focus {
      outline: none;
      /* 🔵 CHANGE: Primary Blue focus */
      border-color: #007bff;
      /* 🔵 CHANGE: Primary Blue shadow */
      box-shadow: 0 0 8px rgba(0, 123, 255, 0.3);
    }

  .form-actions {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 25px;
  }

  /* ------------------- BUTTONS ------------------- */
  /* Kept save/cancel as standard success/neutral colors */
  .save-btn {
    background: #22c55e;
    color: #fff;
    border: none;
    padding: 10px 20px;
    border-radius: 10px;
    font-weight: 700;
    cursor: pointer;
    transition: all 0.3s ease;
  }

    .save-btn:hover {
      background: #16a34a;
      transform: scale(1.05);
    }

  .cancel-btn {
    background: #94a3b8;
    color: #fff;
    border: none;
    padding: 10px 20px;
    border-radius: 10px;
    font-weight: 700;
    cursor: pointer;
    transition: all 0.3s ease;
  }

    .cancel-btn:hover {
      background: #64748b;
      transform: scale(1.05);
    }

  /* ------------------- ERROR / LOADING ------------------- */
  .loading-overlay p {
    /* 🔵 CHANGE: Primary Blue loading text */
    color: #007bff;
    font-size: 1.2rem;
    font-weight: 700;
  }

  .error-box {
    background: #fee2e2;
    color: #991b1b;
    padding: 14px 20px;
    border-radius: 8px;
    margin-top: 10px;
    font-weight: 600;
    border: 1px solid #fca5a5;
    text-align: center;
  }
</style>
