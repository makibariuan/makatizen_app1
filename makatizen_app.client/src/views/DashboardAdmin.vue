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

      <!-- ========== USER TABLES (Kit/System Users) ========== -->
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
      </section>

      <!-- ========== CITIZEN RECORDS ========== -->
      <section v-else-if="activeView === 'citizenRecords'">
        <h2 class="section-title">Citizen Records</h2>
        <p class="metric-number">{{ metrics.citizenCount }}</p>
        <p>Total enrolled citizens with biometric and personal records.</p>
        <p class="detail">Last Enrollment: {{ metrics.lastEnrollmentDate || 'N/A' }}</p>
        <button class="action-button success">View Citizen Records</button>
      </section>

      <!-- ========== MODERN ADD/EDIT USER MODAL ========== -->
      <div v-if="showForm" class="form-overlay">
        <div class="form-popup">
          <h3>{{ editingUser ? 'Edit User' : 'Create New User' }}</h3>
          <form @submit.prevent="saveUser">
            <label>Username</label>
            <input v-model="form.username" required />

            <label>Password</label>
            <input v-model="form.password" type="password" :required="!editingUser" />

            <label>Role</label>
            <select v-model="form.role" required>
              <option value="SystemUser">System User</option>
              <option value="KitUser">Kit User</option>
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
          const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(this.form)
          });

          if (!response.ok) throw new Error('Failed to save user');

          await this.fetchUsers(this.activeView);
          this.showForm = false;
          this.editingUser = null;

          alert('✅ User saved successfully!');
        } catch (err) {
          console.error(err);
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
        const overlay = document.querySelector('.form-overlay');
        if (overlay) {
          overlay.classList.add('fade-out');
          setTimeout(() => {
            this.showForm = false;
          }, 250); // matches CSS transition
        } else {
          this.showForm = false;
        }
      },
      toggleSidebar() {
        this.sidebarOpen = !this.sidebarOpen;
      }
    }
  };
</script>

<style scoped>
  /* ------------------- MAIN LAYOUT ------------------- */
  .dashboard-wrapper {
    display: flex;
    min-height: 100vh;
    width: 100vw;
    margin: 0;
    padding: 0;
    background: linear-gradient(135deg, #e6f4f1, #f9ffff);
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
    color: #004d4d;
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
    background: #008080;
    color: #fff;
    padding: 12px 20px;
    border: none;
    border-radius: 10px;
    cursor: pointer;
    font-weight: 700;
    box-shadow: 0 4px 15px rgba(0, 128, 128, 0.25);
    transition: all 0.3s ease;
  }

    .add-btn:hover {
      background: #006666;
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
    box-shadow: 0 10px 30px rgba(0, 128, 128, 0.25);
    animation: popupIn 0.25s ease forwards;
    border: 2px solid rgba(0, 128, 128, 0.15);
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
    color: #004d4d;
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
      border-color: #008080;
      box-shadow: 0 0 8px rgba(0, 128, 128, 0.3);
    }

  .form-actions {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 25px;
  }

  /* ------------------- BUTTONS ------------------- */
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
</style>
