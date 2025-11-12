<template>
  <!-- Modal Overlay -->
  <div v-if="isVisible" class="modal-overlay" @click.self="closeModal">
    <!-- Modal Content -->
    <div class="modal-content">
      <div class="modal-header">
        <h2 class="modal-title">{{ isEdit ? 'Update Kit User' : 'Add New Kit User' }}</h2>
        <button class="close-button" @click="closeModal">&times;</button>
      </div>

      <form @submit.prevent="handleSubmit" class="form-container">

        <!-- ID (Display only for edit) -->
        <div v-if="isEdit" class="form-group">
          <label for="userId" class="form-label">User ID</label>
          <input type="text" id="userId" :value="formData.id" disabled class="form-input disabled-input">
        </div>

        <!-- First Name -->
        <div class="form-group">
          <label for="firstName" class="form-label">First Name *</label>
          <input type="text" id="firstName" v-model="formData.firstName" required class="form-input">
        </div>

        <!-- Last Name -->
        <div class="form-group">
          <label for="lastName" class="form-label">Last Name *</label>
          <input type="text" id="lastName" v-model="formData.lastName" required class="form-input">
        </div>

        <!-- Email -->
        <div class="form-group">
          <label for="email" class="form-label">Email *</label>
          <input type="email" id="email" v-model="formData.email" required class="form-input">
        </div>

        <!-- Kit ID -->
        <div class="form-group">
          <label for="kitId" class="form-label">Kit ID *</label>
          <input type="text" id="kitId" v-model="formData.kitId" required class="form-input" placeholder="e.g., KIT-A45">
        </div>

        <!-- Assigned Site -->
        <div class="form-group">
          <label for="site" class="form-label">Assigned Site *</label>
          <select id="site" v-model="formData.site" required class="form-input select-input">
            <option disabled value="">Select a site</option>
            <option>East District HQ</option>
            <option>Central Control</option>
            <option>North Sector</option>
            <option>South Hub</option>
          </select>
        </div>

        <!-- Status (Only editable) -->
        <div v-if="isEdit" class="form-group">
          <label for="status" class="form-label">Status *</label>
          <select id="status" v-model="formData.status" required class="form-input select-input">
            <option>Active</option>
            <option>Inactive</option>
            <option>Pending</option>
          </select>
        </div>

        <!-- Password Field (Conditional v-model fix applied here) -->
        <div class="form-group">
          <label :for="isEdit ? 'newPassword' : 'password'" class="form-label">
            {{ isEdit ? 'New Password' : 'Password' }} {{ !isEdit ? '*' : '' }}
          </label>
          <input type="password"
                 :id="isEdit ? 'newPassword' : 'password'"
                 v-model="passwordModel" <!-- NOW USES COMPUTED PROPERTY -->
          :required="!isEdit"
          :placeholder="isEdit ? 'Leave blank to keep current password' : 'Enter password'"
          class="form-input">
        </div>

        <!-- Action Button -->
        <div class="modal-footer">
          <button type="button" class="action-button secondary" @click="closeModal">Cancel</button>
          <button type="submit" class="action-button primary" :disabled="isSaving">
            <span v-if="isSaving">Saving...</span>
            <span v-else>{{ isEdit ? 'Save Changes (U)' : 'Add User (C)' }}</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
  import { reactive, watch, computed, ref } from 'vue';

  const props = defineProps({
    isVisible: Boolean,
    isEdit: Boolean,
    initialData: Object, // Data to pre-fill for editing
  });

  const emit = defineEmits(['close', 'submit']);

  const isSaving = ref(false);

  const initialFormData = {
    id: null,
    firstName: '',
    lastName: '',
    email: '',
    kitId: '',
    site: '',
    status: 'Active',
    password: '',     // For new user creation
    newPassword: '',  // For password change during edit
  };

  const formData = reactive({ ...initialFormData });

  // --- THE FIX ---
  // This computed property acts as the assignable reference for v-model.
  const passwordModel = computed({
    get() {
      // In edit mode, we bind to newPassword. In create mode, we bind to password.
      return props.isEdit ? formData.newPassword : formData.password;
    },
    set(newValue) {
      // When the input changes, update the correct property.
      if (props.isEdit) {
        formData.newPassword = newValue;
      } else {
        formData.password = newValue;
      }
    }
  });
  // --- END OF FIX ---


  // Watch for changes in visibility or edit mode to reset/load data
  watch(() => props.isVisible, (newVal) => {
    if (newVal) {
      if (props.isEdit && props.initialData) {
        // Load data for editing, reset passwords
        Object.assign(formData, props.initialData);
        formData.password = '';
        formData.newPassword = '';
      } else {
        // Reset for creation mode
        Object.assign(formData, initialFormData);
      }
      isSaving.value = false;
    }
  }, { immediate: true });

  const handleSubmit = () => {
    isSaving.value = true;
    // Deep clone and omit empty password fields before submitting
    const dataToSend = JSON.parse(JSON.stringify(formData));

    if (props.isEdit) {
      // If updating, only send newPassword if it was actually entered
      if (!dataToSend.newPassword) {
        delete dataToSend.newPassword;
      }
      delete dataToSend.password; // Never send the 'create' password field on update
    } else {
      // If creating, only send the 'create' password field
      delete dataToSend.newPassword;
    }

    // Simulate save delay for better UX
    setTimeout(() => {
      emit('submit', dataToSend);
      isSaving.value = false;
    }, 500);
  };

  const closeModal = () => {
    emit('close');
  };
</script>

<style scoped>
  /* Modal Structure and Layout */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .modal-content {
    background: white;
    border-radius: 12px;
    width: 90%;
    max-width: 550px;
    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.2);
    overflow: hidden;
    animation: fadeIn 0.3s ease-out;
  }

  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px 30px;
    background-color: #f1f3f5;
    border-bottom: 1px solid #e9ecef;
  }

  .modal-title {
    font-size: 1.5rem;
    color: #004d99;
    font-weight: 700;
  }

  .close-button {
    background: none;
    border: none;
    font-size: 2rem;
    cursor: pointer;
    color: #6c757d;
    transition: color 0.2s;
  }

    .close-button:hover {
      color: #343a40;
    }

  /* Form Styles */
  .form-container {
    padding: 30px;
    display: grid;
    gap: 15px;
    grid-template-columns: 1fr 1fr;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    /* Span full width for specific fields */
    grid-column: span 2;
  }

    .form-group:nth-child(2),
    .form-group:nth-child(3) {
      /* First name and last name side-by-side */
      grid-column: span 1;
    }

  .form-label {
    font-weight: 600;
    margin-bottom: 6px;
    color: #495057;
    font-size: 0.95rem;
  }

  .form-input {
    padding: 10px 15px;
    border: 1px solid #ced4da;
    border-radius: 8px;
    font-size: 1rem;
    transition: border-color 0.2s, box-shadow 0.2s;
    background-color: #ffffff;
  }

    .form-input:focus {
      border-color: #007bff;
      box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.1);
      outline: none;
    }

  .disabled-input {
    background-color: #e9ecef;
    cursor: not-allowed;
    color: #6c757d;
  }

  .select-input {
    appearance: none;
    background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='%23343a40' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m2 5 6 6 6-6'/%3e%3c/svg%3e");
    background-repeat: no-repeat;
    background-position: right 15px center;
    background-size: 12px;
    padding-right: 35px;
  }

  /* Footer and Buttons */
  .modal-footer {
    grid-column: span 2;
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    padding-top: 10px;
    margin-top: 15px;
    border-top: 1px solid #e9ecef;
  }

  .action-button {
    padding: 10px 20px;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-weight: 600;
    transition: background-color 0.2s, opacity 0.2s;
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

      .action-button.secondary:hover {
        background-color: #5a6268;
      }

    .action-button:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

  /* Animation */
  @keyframes fadeIn {
    from {
      opacity: 0;
      transform: translateY(-20px);
    }

    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  /* Responsive adjustments */
  @media (max-width: 500px) {
    .form-container {
      grid-template-columns: 1fr;
    }

    .form-group {
      grid-column: span 1 !important;
    }
  }
</style>
