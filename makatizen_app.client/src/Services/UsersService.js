// src/Services/UsersService.js
const API_URL = 'https://localhost:5001/api/Users'; // adjust port if needed

export default {
  // ----- SYSTEM USERS -----
  async getSystemUsers() {
    const res = await fetch(`${API_URL}/system-users`);
    if (!res.ok) throw new Error('Failed to fetch system users');
    return await res.json();
  },

  async createSystemUser(user) {
    const res = await fetch(`${API_URL}/system-users`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(user)
    });
    if (!res.ok) throw new Error('Failed to create system user');
    return await res.json();
  },

  async updateSystemUser(id, user) {
    const res = await fetch(`${API_URL}/system-users/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(user)
    });
    if (!res.ok) throw new Error('Failed to update system user');
    return await res.json();
  },

  async deleteSystemUser(id) {
    const res = await fetch(`${API_URL}/system-users/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error('Failed to delete system user');
  },

  // ----- KIT USERS -----
  async getKitUsers() {
    const res = await fetch(`${API_URL}/kit-users`);
    if (!res.ok) throw new Error('Failed to fetch kit users');
    return await res.json();
  },

  async createKitUser(user) {
    const res = await fetch(`${API_URL}/kit-users`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(user)
    });
    if (!res.ok) throw new Error('Failed to create kit user');
    return await res.json();
  },

  async updateKitUser(id, user) {
    const res = await fetch(`${API_URL}/kit-users/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(user)
    });
    if (!res.ok) throw new Error('Failed to update kit user');
    return await res.json();
  },

  async deleteKitUser(id) {
    const res = await fetch(`${API_URL}/kit-users/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error('Failed to delete kit user');
  }
};
