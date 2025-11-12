<template>
  <div class="kit-dashboard-container">
    <h1 class="welcome-title">
      Enrollment Kit Portal 📍
    </h1>
    <p class="role-info">
      Welcome, {{ userRole }}. Your unit ID is <strong>{{ metrics.kitUnitId }}</strong>. Focus on citizen enrollment and maintaining data sync.
    </p>

    <div class="kit-grid">

      <!-- Card 1: Daily Performance -->
      <div class="card card-today">
        <h2 class="card-title">Enrollments Today</h2>
        <p class="metric-number">{{ metrics.enrollmentsToday }}</p>
        <p class="card-description">
          Successful citizen enrollments completed since midnight.
        </p>
        <div class="card-actions">
          <button class="action-button success" disabled>Start New Enrollment</button>
        </div>
      </div>

      <!-- Card 2: Total Performance -->
      <div class="card card-total">
        <h2 class="card-title">Total Citizens Enrolled</h2>
        <p class="metric-number">{{ metrics.totalCitizensEnrolled }}</p>
        <p class="card-description">
          Cumulative number of records successfully registered by this unit.
        </p>
        <div class="card-actions">
          <button class="action-button primary" disabled>View Local History</button>
        </div>
      </div>

      <!-- Card 3: Sync Status -->
      <div class="card card-sync">
        <h2 class="card-title">Sync Status: {{ metrics.unitStatus }}</h2>
        <p class="metric-number pending-count">{{ metrics.pendingUploads }}</p>
        <p class="card-description">
          Records waiting to be uploaded and synchronized with the central server.
        </p>
        <div class="card-actions">
          <button class="action-button warning"
                  :disabled="metrics.pendingUploads === 0"
                  @click="syncData">
            Manual Sync ({{ metrics.pendingUploads }})
          </button>
        </div>
        <p class="last-sync-detail">
          Last Successful Sync: {{ metrics.lastSyncTime || 'N/A' }}
        </p>
      </div>
    </div>

    <!-- Loading/Error Indicator -->
    <div v-if="loading" class="loading-overlay">
      <p>Loading enrollment kit metrics...</p>
    </div>
    <div v-if="error" class="error-box">
      <p>{{ error }}</p>
      <button v-if="error.includes('Logging out')" @click="$router.push({ name: 'Login' })">Go to Login</button>
    </div>
  </div>
</template>

<script>
import AuthService from '@/services/AuthService';
import DashboardService from '@/services/DashboardService';
import { useRouter } from 'vue-router';

export default {
  name: 'DashboardKitView',
  setup() {
    const router = useRouter();
    return { router };
  },
  data() {
    return {
      userRole: AuthService.getUserRole() || 'Kit User',
      loading: true,
      error: null,
      metrics: {
        kitUnitId: 'KIT-Loading',
        totalCitizensEnrolled: 0,
        enrollmentsToday: 0,
        pendingUploads: 0,
        lastSyncTime: 'Loading...',
        unitStatus: 'Loading...'
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
            const data = await DashboardService.getKitMetrics();

            this.metrics = {
                ...this.metrics,
                ...data,
                // Format sync date for display if available
                lastSyncTime: data.lastSyncTime
                    ? new Date(data.lastSyncTime).toLocaleString()
                    : 'Never'
            };

        } catch (err) {
            this.error = err.message || 'An unknown error occurred while fetching data.';
            if (this.error.includes('Logging out')) {
                this.router.push({ name: 'Login' });
            }
        } finally {
            this.loading = false;
        }
    },
    syncData() {
        // Placeholder for actual sync logic (e.g., calling a sync API endpoint)
        console.log('Initiating manual data synchronization...');
        // In a real app, this would trigger an API call to upload pending data
        // and then refetchMetrics() on success.
        alert("Manual Sync started. Check logs for progress. (API not implemented yet)");
    }
  }
};
</script>

<style scoped>
  /* Kit Dashboard Styles */
  .kit-dashboard-container {
    max-width: 1100px;
    margin: 0 auto;
    padding: 30px;
    background-color: #f8f9fa;
    min-height: 85vh;
    border-radius: 8px;
    position: relative;
  }

  .welcome-title {
    font-size: 2.2rem;
    font-weight: 800;
    color: #008080; /* Teal/cyan color theme for Kit user focus */
    margin-bottom: 5px;
  }

  .role-info {
    font-size: 1rem;
    color: #6c757d;
    margin-bottom: 40px;
    border-bottom: 2px solid #e9ecef;
    padding-bottom: 15px;
  }

  .kit-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
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
    font-size: 1.5rem;
    font-weight: 700;
    margin-bottom: 10px;
    color: #343a40;
  }

  .metric-number {
    font-size: 3rem;
    font-weight: 900;
    margin: 10px 0 15px 0;
    color: #008080; /* Teal highlight */
  }

  .pending-count {
    color: #ff4500; /* Orange-red for urgency */
  }

  .card-description {
    color: #606c78;
    margin-bottom: 25px;
    min-height: 40px;
    line-height: 1.5;
  }

  .last-sync-detail {
    font-size: 0.85rem;
    color: #888;
    margin-top: 15px;
    padding-top: 10px;
    border-top: 1px solid #eee;
  }

  .card-actions {
    margin-top: auto;
    padding-top: 15px;
  }

  /* Card Specific Styling */
  .card-today {
    border-left: 5px solid #008080;
  }

  .card-total {
    border-left: 5px solid #32cd32; /* Lime green */
  }

  .card-sync {
    border-left: 5px solid #ff4500;
  }

  /* Action Buttons */
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
      opacity: 0.5;
      cursor: not-allowed;
    }

    .action-button.success {
      background-color: #32cd32;
      color: white;
    }

      .action-button.success:hover:not(:disabled) {
        background-color: #2e8b57;
      }

    .action-button.primary {
      background-color: #008080;
      color: white;
    }

      .action-button.primary:hover:not(:disabled) {
        background-color: #006666;
      }

    .action-button.warning {
      background-color: #ff4500;
      color: white;
    }

      .action-button.warning:hover:not(:disabled) {
        background-color: #cc3700;
      }

  /* Loading and Error styles (same as admin) */
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
