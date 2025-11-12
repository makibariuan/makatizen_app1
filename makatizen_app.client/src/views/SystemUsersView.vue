<template>
  <div class="page-container">
    <h1>Manage System Users ⚙️</h1>
    <button class="add-btn" @click="openCreateModal">➕ Add System User</button>

    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Username</th>
          <th>Password</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td>{{ user.id }}</td>
          <td>{{ user.username }}</td>
          <td>{{ user.password }}</td>
          <td>
            <button class="edit-btn" @click="openEditModal(user)">✏️ Edit</button>
            <button class="delete-btn" @click="deleteUser(user.id)">🗑️ Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Modal -->
    <div v-if="showModal" class="modal">
      <div class="modal-content">
        <h2>{{ editingUser ? 'Edit User' : 'Create User' }}</h2>
        <label>Username:</label>
        <input v-model="form.username" type="text" />
        <label>Password:</label>
        <input v-model="form.password" type="password" />

        <div class="modal-actions">
          <button class="save-btn" @click="saveUser">💾 Save</button>
          <button class="cancel-btn" @click="closeModal">❌ Cancel</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import UsersService from '@/Services/UsersService';

export default {
  name: 'SystemUsersView',
  data() {
    return {
      users: [],
      showModal: false,
      form: { username: '', password: '' },
      editingUser: null
    };
  },
  async mounted() {
    await this.fetchUsers();
  },
  methods: {
    async fetchUsers() {
      this.users = await UsersService.getSystemUsers();
    },
    openCreateModal() {
      this.editingUser = null;
      this.form = { username: '', password: '' };
      this.showModal = true;
    },
    openEditModal(user) {
      this.editingUser = user;
      this.form = { username: user.username, password: user.password };
      this.showModal = true;
    },
    async saveUser() {
      if (this.editingUser) {
        await UsersService.updateSystemUser(this.editingUser.id, this.form);
      } else {
        await UsersService.createSystemUser(this.form);
      }
      this.closeModal();
      await this.fetchUsers();
    },
    async deleteUser(id) {
      if (confirm('Delete this user?')) {
        await UsersService.deleteSystemUser(id);
        await this.fetchUsers();
      }
    },
    closeModal() {
      this.showModal = false;
    }
  }
};
</script>

<style scoped>
  /* Clean minimalist table style */
  .page-container {
    max-width: 900px;
    margin: auto;
    padding: 20px;
  }

  h1 {
    color: #004d99;
    margin-bottom: 20px;
  }

  .add-btn {
    background: #007bff;
    color: white;
    padding: 8px 16px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }

  table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
  }

  th, td {
    border: 1px solid #ccc;
    padding: 10px;
    text-align: left;
  }

  .edit-btn, .delete-btn {
    margin-right: 5px;
    padding: 5px 10px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }

  .edit-btn {
    background-color: #ffc107;
  }

  .delete-btn {
    background-color: #dc3545;
    color: white;
  }

  .modal {
    position: fixed;
    inset: 0;
    background: rgba(0,0,0,0.5);
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .modal-content {
    background: white;
    padding: 20px;
    border-radius: 8px;
    width: 400px;
  }

  .modal-actions {
    display: flex;
    justify-content: flex-end;
    margin-top: 15px;
    gap: 10px;
  }

  .save-btn {
    background-color: #28a745;
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 4px;
  }

  .cancel-btn {
    background-color: #6c757d;
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 4px;
  }
</style>
