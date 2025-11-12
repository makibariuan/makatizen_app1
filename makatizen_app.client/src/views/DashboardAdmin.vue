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
        Welcome back, {{ userRole }}. Use the sections below to manage users and citizen data.
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
              <td>{{ user.role }}</td>
              <td>
                <button class="edit-btn" @click="editUser(user)">Edit</button>
                <button class="delete-btn" @click="deleteUser(user.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Add/Edit Form -->
        <div v-if="showForm" class="form-overlay">
          <div class="form-container">
            <h3>{{ editingUser ? 'Edit User' : 'Add New User' }}</h3>
            <form @submit.prevent="saveUser">
              <label>Username</label>
              <input v-model="form.username" required />

              <label>Password</label>
              <input v-model="form.password" type="password" :required="!editingUser" />

              <button type="submit" class="save-btn">Save</button>
              <button type="button" class="cancel-btn" @click="closeForm">Cancel</button>
            </form>
          </div>
        </div>
      </section>

      <section v-else-if="activeView === 'citizenRecords'">
        <h2 class="section-title">Citizen Records</h2>
        <p class="metric-number">{{ metrics.citizenCount }}</p>
        <p>Total enrolled citizens with biometric and personal records.</p>
        <p class="detail">Last Enrollment: {{ metrics.lastEnrollmentDate || 'N/A' }}</p>
        <button class="action-button success">View Citizen Records</button>
      </section>

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

  export default {
    name: 'DashboardAdminView',
    components: { Sidebar },
    data() {
      return {
        userRole: AuthService.getUserRole() || 'Admin',
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
          role: ''
        },
        sidebarOpen: true
      };
    },
    async mounted() {
      await this.fetchMetrics();
      await this.fetchUsers('kitUsers');
    },
    methods: {
      async fetchMetrics() {
        this.loading = true;
        this.error = null;
        try {
          const data = await DashboardService.getAdminMetrics();
          this.metrics = { ...this.metrics, ...data };
        } catch (err) {
          this.error = err.message;
        } finally {
          this.loading = false;
        }
      },
      async fetchUsers(view) {
        this.loading = true;
        this.error = null;
        try {
          const endpoint = view === 'systemUsers' ? 'system-users' : 'kit-users';
          const res = await fetch(`https://localhost:7288/api/User/${endpoint}`);
          this.userList = await res.json();
        } catch (err) {
          this.error = 'Failed to load user list.';
        } finally {
          this.loading = false;
        }
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
        this.form = {
          username: '',
          password: '',
          role: this.activeView === 'kitUsers' ? 'KitUser' : 'SystemUser'
        };
        this.showForm = true;
      },
      editUser(user) {
        this.editingUser = user;
        this.form = { ...user, password: '' };
        this.showForm = true;
      },
      async saveUser() {
        const method = this.editingUser ? 'PUT' : 'POST';
        const url = this.editingUser
          ? `https://localhost:7288/api/User/${this.editingUser.id}`
          : 'https://localhost:7288/api/User';

        try {
          await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(this.form)
          });
          await this.fetchUsers(this.activeView);
          this.showForm = false;
        } catch (err) {
          this.error = 'Failed to save user.';
        }
      },
      async deleteUser(id) {
        if (confirm('Are you sure you want to delete this user?')) {
          try {
            await fetch(`https://localhost:7288/api/User/${id}`, { method: 'DELETE' });
            await this.fetchUsers(this.activeView);
          } catch {
            this.error = 'Failed to delete user.';
          }
        }
      },
      closeForm() {
        this.showForm = false;
      },
      toggleSidebar() {
        this.sidebarOpen = !this.sidebarOpen;
      }
    }
  };
</script>

<style scoped>
  .dashboard-wrapper {
    display: flex;
    min-height: 100vh;
    width: 100vw;
    margin: 0;
    padding: 0;
    background-color: #f7fafc;
    border-radius: 0;
    box-sizing: border-box;
    box-shadow: none;
    overflow-x: hidden;
    position: relative;
  }

  .sidebar-toggle {
    position: fixed;
    top: 80px;
    left: 15px;
    z-index: 1100;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    width: 28px;
    height: 22px;
    background: transparent;
    border: none;
    cursor: pointer;
    padding: 0;
  }

    .sidebar-toggle .bar {
      height: 4px;
      background-color: #fff;
      border-radius: 2px;
      transition: all 0.3s ease;
    }

  .dashboard-container {
    flex-grow: 1;
    padding: 40px 35px;
    overflow-y: auto;
    position: relative;
    font-family: 'Inter', sans-serif;
    color: #2d3748;
  }

  .welcome-title {
    font-size: 2.8rem;
    font-weight: 800;
    margin-bottom: 10px;
    color: #004d4d;
    letter-spacing: 0.5px;
  }

  .role-info {
    font-size: 1.15rem;
    color: #4a5568;
    margin-bottom: 40px;
    border-bottom: 2px solid #e9ecef;
    padding-bottom: 20px;
  }

  .section-title {
    font-weight: 700;
    color: #2d3748;
    font-size: 1.6rem;
    margin-bottom: 20px;
  }

  .add-btn,
  .save-btn,
  .cancel-btn,
  .edit-btn,
  .delete-btn,
  .action-button {
    border-radius: 8px;
    font-weight: 700;
    letter-spacing: 0.05em;
    transition: background-color 0.3s ease, box-shadow 0.3s ease;
  }

  .add-btn {
    background: #008080;
    color: #fff;
    padding: 12px 20px;
    border: none;
    cursor: pointer;
    margin-bottom: 20px;
    box-shadow: 0 4px 12px rgba(0, 128, 128, 0.3);
  }

    .add-btn:hover {
      background: #005959;
      box-shadow: 0 6px 15px rgba(0, 89, 89, 0.5);
    }

  .save-btn {
    background: #22c55e;
    color: white;
    padding: 12px 22px;
    border: none;
    cursor: pointer;
    box-shadow: 0 4px 12px rgba(34, 197, 94, 0.35);
  }

    .save-btn:hover {
      background: #16a34a;
      box-shadow: 0 6px 18px rgba(22, 163, 74, 0.5);
    }

  .cancel-btn {
    background: #94a3b8;
    color: white;
    border: none;
    cursor: pointer;
    padding: 12px 22px;
    margin-left: 15px;
  }

    .cancel-btn:hover {
      background: #64748b;
    }

  .edit-btn {
    background: #0ea5e9;
    color: white;
    padding: 8px 14px;
  }

    .edit-btn:hover {
      background: #0284c7;
    }

  .delete-btn {
    background: #ef4444;
    color: white;
    padding: 8px 14px;
  }

    .delete-btn:hover {
      background: #b91c1c;
    }

  .user-table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 0 10px;
    margin-bottom: 50px;
  }

    .user-table th {
      text-align: left;
      padding: 18px 15px;
      color: #475569;
      font-weight: 700;
    }

    .user-table td {
      background: white;
      padding: 15px;
      box-shadow: 0 4px 6px rgb(160 174 192 / 15%);
      border-radius: 8px;
      vertical-align: middle;
      color: #334155;
    }

  form input {
    border: 1.5px solid #cbd5e1;
    padding: 12px 15px;
    border-radius: 8px;
    font-size: 1rem;
    transition: border-color 0.3s ease;
    width: 100%;
    box-sizing: border-box;
    margin-top: 6px;
  }

    form input:focus {
      outline: none;
      border-color: #008080;
      box-shadow: 0 0 6px #008080aa;
    }

  .form-overlay,
  .loading-overlay {
    backdrop-filter: blur(4px);
  }

    .loading-overlay p {
      color: #008080;
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

  @media (max-width: 768px) {
    .dashboard-wrapper {
      flex-direction: column;
    }

    .sidebar {
      width: 100%;
      height: auto;
      border-radius: 0 0 10px 10px;
    }

    .dashboard-container {
      padding: 1rem 1.5rem;
    }

    .sidebar-toggle {
      top: 10px;
      left: 10px;
    }
  }
</style>
