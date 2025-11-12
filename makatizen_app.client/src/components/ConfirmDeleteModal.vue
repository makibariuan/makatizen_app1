<template>
  <div v-if="isVisible" class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <h2 class="modal-title delete-title">Confirm Deletion</h2>
      <p class="delete-message">
        Are you sure you want to permanently delete the user:
        <strong>{{ userName }}</strong>?
      </p>
      <p class="delete-warning">
        This action cannot be undone.
      </p>

      <div class="modal-actions">
        <button type="button" class="cancel-btn" @click="$emit('close')">
          Cancel
        </button>
        <button type="button" class="confirm-btn" @click="$emit('confirm')">
          Delete User
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ConfirmDeleteModal',
  props: {
    isVisible: Boolean,
    userName: String
  },
  emits: ['close', 'confirm']
};
</script>

<style scoped>
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .modal-content {
    background: white;
    padding: 30px;
    border-radius: 12px;
    width: 90%;
    max-width: 400px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
    text-align: center;
  }

  .modal-title {
    font-size: 1.6rem;
    color: #dc3545; /* Red for emphasis */
    margin-bottom: 20px;
  }

  .delete-message {
    font-size: 1.1rem;
    margin-bottom: 10px;
    color: #343a40;
  }

  .delete-warning {
    font-size: 0.9rem;
    color: #ffc107;
    font-weight: 600;
    margin-bottom: 30px;
  }

  .modal-actions {
    display: flex;
    justify-content: center;
    gap: 15px;
  }

  .confirm-btn, .cancel-btn {
    padding: 10px 20px;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-weight: 600;
    transition: background-color 0.3s;
    width: 45%;
  }

  .confirm-btn {
    background-color: #dc3545; /* Red */
    color: white;
  }

    .confirm-btn:hover {
      background-color: #c82333;
    }

  .cancel-btn {
    background-color: #6c757d;
    color: white;
  }

    .cancel-btn:hover {
      background-color: #5a6268;
    }
</style>
