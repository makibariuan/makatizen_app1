<template>
  <div class="dashboard-container">
    <h1 class="welcome-title">
      Administrator Dashboard 🛡️
    </h1>
    <p class="role-info">
      Welcome back, {{ userRole }}. Use the sections below to manage system users, kit users, and citizen data.
    </p>

    <div class="admin-grid">

      <!-- Card 1: User Management -->
      <div class="card card-users">
        <h2 class="card-title">System & Kit Users</h2>
        <!-- Display Total Users -->
        <p class="metric-number">{{ metrics.totalUsers }}</p>
        <p class="card-description">
          Total active users in the system (System Admins, Super Admins, and Kit Users).
          <span class="detail">System Admins: {{ metrics.systemUsers }} | Kit Users: {{ metrics.kitUsers }}</span>
        </p>
        <div class="card-actions">
          <!-- Navigation to Kit Users List - CORRECTED PATH -->
          <button class="action-button primary" @click="router.push('/admin/kit-users')">Manage Kit Users</button>
          <!-- Navigation to System Users List - Assuming a future path -->
          <button class="action-button secondary" disabled>Manage System Users</button>
        </div>
      </div>

      <!-- Card 2: Citizen Data Management -->
      <div class="card card-citizens">
        <h2 class="card-title">Citizen Records</h2>
        <!-- Display Total Citizens -->
        <p class="metric-number">{{ metrics.totalCitizens }}</p>
        <p class="card-description">
          Total enrolled citizens with biometric and personal records.
          <span class="detail">Users requiring password reset: {{ metrics.usersPendingReset }}</span>
        </p>
        <div class="card-actions">
          <!-- Placeholder for Citizen Records -->
          <button class="action-button success" disabled>View Citizen Records</button>
        </div>
      </div>

      <!-- Card 3: System Monitoring & Logs -->
      <div class="card card-logs">
        <h2 class="card-title">Logs and Audit Trail</h2>
        <p class="metric-number">{{ metrics.unresolvedLogs }}</p>
        <p class="card-description">
          Unresolved critical system logs and errors requiring review.
          <span class="detail">Last Error: {{ metrics.lastLogError || 'None' }}</span>
        </p>
        <div class="card-actions">
          <button class="action-button warning" disabled>View System Logs</button>
          <button class="action-button info" disabled>Email History</button>

        </div>
      </div>
    </div>

    <!-- Loading/Error Indicator -->
    <div v-if="loading" class="loading-overlay">
      <p>Loading dashboard metrics...</p>
    </div>
    <div v-if="error" class="error-box">
      <p>{{ error }}</p>
      <button v-if="error.includes('Logging out')" @click="router.push({ name: 'Login' })">Go to Login</button>
    </div>
  </div>
</template>

<script>
  import AuthService from '@/services/AuthService';
  import DashboardService from '@/services/DashboardService';
  import { useRouter } from 'vue-router';

  export default {
    name: 'DashboardAdminView',
    setup() {
      // Access router in setup
      const router = useRouter();
      return { router };
    },
    data() {
      return {
        userRole: AuthService.getUserRole() || 'Admin User',
        loading: true,
        error: null,
        metrics: {
          totalUsers: 0,
          systemUsers: 0,
          kitUsers: 0,
          totalCitizens: 0,
          usersPendingReset: 0,
          // Keeping placeholders for data not yet provided by backend:
          unresolvedLogs: 0,
          lastLogError: 'None',
        }
      };
    },
    async mounted() {
      await this.fetchMetrics();
    },
    methods: {
      async fetchMetrics() {
        this.loading = true;
        this.error = null;
        try {
          const data = await DashboardService.getAdminMetrics();

          // Mapping properties from the C# backend response (which is usually PascalCase,
          // but often automatically converted to camelCase in Axios/JSON deserialization).
          // We use the exact names from the backend response structure.
          this.metrics.totalUsers = data.totalUsers || 0;
          this.metrics.systemUsers = data.systemUsers || 0; // Assuming this includes SuperAdmins (UserType 1 & 2)
          this.metrics.kitUsers = data.kitUsers || 0;
          this.metrics.totalCitizens = data.totalCitizens || 0;
          this.metrics.usersPendingReset = data.usersPendingReset || 0;

          // These are placeholder values as the backend does not yet provide them:
          this.metrics.unresolvedLogs = data.unresolvedLogs || 0;
          this.metrics.lastLogError = data.lastLogError || 'None';

        } catch (err) {
          console.error(err);
          // Display a user-friendly error message
          this.error = 'Could not load data. Check console for details or verify authentication.';
        } finally {
          this.loading = false;
        }
      }
    }
  };
</script>

<style scoped>

  /* Scoped styles are largely from before, with additions for metrics and loading states */
    .dashboard-container {
        max-width: 1300px;
        margin: 0 auto;
        padding: 30px;
        background-color: #f8f9fa;
        min-height: 85vh;
        border-radius: 8px;
        position: relative; /* Needed for loading overlay */

  }

    .welcome-title {
        font-size: 2.2rem;
        font-weight: 800;
        color: #004d99;
        margin-bottom: 5px;

  }

    .role-info {
        font-size: 1rem;
        color: #6c757d;
        margin-bottom: 40px;
        border-bottom: 2px solid #e9ecef;
        padding-bottom: 15px;

  }

    .admin-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
        gap: 30px;

  }

    .card {
        background: white;
        padding: 30px;
        border-radius: 12px;
        box-shadow: 0 4px 18px rgba(0, 0, 0, 0.08);
        transition: transform 0.3s, box-shadow 0.3s;
        display: flex;
        flex-direction: column;

  }

      .card:hover {
          transform: translateY(-5px);
          box-shadow: 0 8px 25px rgba(0, 0, 0, 0.12);

  }

    .card-title {
        font-size: 1.6rem;
        font-weight: 700;
        margin-bottom: 10px;
        color: #343a40;

  }

    .metric-number {
        font-size: 3rem;
        font-weight: 900;
        margin: 10px 0 15px 0;
        color: #007bff; /* Highlight color for metrics */

  }

    .card-description {
        color: #606c78;
        margin-bottom: 25px;
        min-height: 40px;
        line-height: 1.5;

  }

    .detail {
        display: block;
        font-size: 0.85rem;
        color: #888;
        margin-top: 5px;

  }

    .card-actions {
        margin-top: auto; /* Pushes buttons to the bottom of the flex container */
        display: flex;
        flex-direction: column;
        gap: 10px;
        padding-top: 15px;

  }


  /* Card Specific Styling (Color coding based on function) */
    .card-users {
        border-left: 5px solid #007bff;

  }

    .card-citizens {
        border-left: 5px solid #28a745;

  }

    .card-logs {
        border-left: 5px solid #ffc107;

  }


  /* ... (action-button styles remain the same) ... */
    .action-button {
        width: 100%;
        padding: 12px;
        border: none;
        border-radius: 6px;
        cursor: pointer;
        font-weight: 600;
        transition: background-color 0.3s, opacity 0.3s;
        text-align: center;

  }

        .action-button:disabled {
            opacity: 0.6;
            cursor: not-allowed;

    }

        .action-button.primary {
            background-color: #007bff;
            color: white;

    }

        .action-button.primary:hover:not(:disabled) {
            background-color: #0056b3;

  }

      .action-button.secondary {
          background-color: #6c757d;
          color: white;

  }

        .action-button.secondary:hover:not(:disabled) {
            background-color: #5a6268;

  }

      .action-button.success {
          background-color: #28a745;
          color: white;

  }

        .action-button.success:hover:not(:disabled) {
            background-color: #1e7e34;

  }

      .action-button.warning {
          background-color: #ffc107;
          color: #343a40;

  }

        .action-button.warning:hover:not(:disabled) {
            background-color: #e0a800;

  }

      .action-button.info {
          background-color: #17a2b8;
          color: white;

  }

        .action-button.info:hover:not(:disabled) {
            background-color: #117a8b;

  }


  /* Loading and Error styles */
    .loading-overlay {
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(255, 255, 255, 0.8);
        display: flex;
        justify-content: center;
        align-items: center;
        font-size: 1.2rem;
        color: #007bff;
        border-radius: 8px;

  }

    .error-box {
        margin-top: 20px;
        padding: 15px;
        background-color: #f8d7da;
        color: #721c24;
        border: 1px solid #f5c6cb;
        border-radius: 4px;
        text-align: center;

  }
</style>
