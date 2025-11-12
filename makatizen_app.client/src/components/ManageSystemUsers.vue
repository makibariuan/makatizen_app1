<template>
  <div class="manage-users-container">
    <h1>Manage System Users ⚙️</h1>
    <button class="add-button" @click="openAddModal">+ Add New User</button>

    <!-- Table -->
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
        <tr v-for="user in systemUsers" :key="user.id">
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

    <!-- Add/Edit Modal -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal">
        <h3>{{ editingUser ? 'Edit User' : 'Add New System User' }}</h3>
        <form @submit.prevent="saveUser">
          <label>Username:</label>
          <input v-model="form.username" required />
          <label>Password:</label>
          <input v-model="form.password" type="password" required />
          <button type="submit" class="save-btn">Save</button>
          <button type="button" class="cancel-btn" @click="closeModal">Cancel</button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ManageSystemUsers',
  data() {
    return {
      systemUsers: [],
      showModal: false,
      editingUser: null,
      form: {
        username: '',
        password: '',
        role: 'SystemUser'
      }
    };
  },
  async mounted() {
    await this.fetchSystemUsers();
  },
  methods: {
    async fetchSystemUsers() {
      const res = await fetch('https://localhost:7288/api/User/system-users');
      this.systemUsers = await res.json();
    },
    openAddModal() {
      this.editingUser = null;
      this.form = { username: '', password: '', role: 'SystemUser' };
      this.showModal = true;
    },
    editUser(user) {
      this.editingUser = user;
      this.form = { ...user };
      this.showModal = true;
    },
    async saveUser() {
      const method = this.editingUser ? 'PUT' : 'POST';
      const url = this.editingUser
        ? `https://localhost:7288/api/User/${this.editingUser.id}`
        : 'https://localhost:7288/api/User';
      await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(this.form)
      });
      await this.fetchSystemUsers();
      this.closeModal();
    },
    async deleteUser(id) {
      if (confirm('Delete this user?')) {
        await fetch(`https://localhost:7288/api/User/${id}`, { method: 'DELETE' });
        await this.fetchSystemUsers();
      }
    },
    closeModal() {
      this.showModal = false;
    }
  }
};
</script>

<style scoped>
  .manage-users-container {
    padding: 30px;
  }

  h1 {
    font-size: 1.8rem;
    color: #004d99;
    margin-bottom: 20px;
  }

  .user-table {
    width: 100%;
    border-collapse: collapse;
  }

    .user-table th, .user-table td {
      border: 1px solid #ddd;
      padding: 10px;
    }

  .add-button {
    background-color: #007bff;
    color: white;
    padding: 10px 14px;
    border: none;
    border-radius: 6px;
    margin-bottom: 10px;
    cursor: pointer;
  }

  .edit-btn {
    background-color: #17a2b8;
    color: white;
    border: none;
    padding: 6px 10px;
    margin-right: 5px;
    cursor: pointer;
  }

  .delete-btn {
    background-color: #dc3545;
    color: white;
    border: none;
    padding: 6px 10px;
    cursor: pointer;
  }

  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .modal {
    background: white;
    padding: 25px;
    border-radius: 8px;
    width: 400px;
  }

  .save-btn, .cancel-btn {
    margin-top: 10px;
    padding: 8px 12px;
    border: none;
    border-radius: 5px;
  }

  .save-btn {
    background: #28a745;
    color: white;
  }

  .cancel-btn {
    background: #6c757d;
    color: white;
    margin-left: 10px;
  }
</style>
