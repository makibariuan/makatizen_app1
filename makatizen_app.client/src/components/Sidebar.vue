<template>
  <aside :class="['sidebar', { 'sidebar-closed': !sidebarOpen }]">
    <nav>
      <ul>
        <li :class="{ active: activeView === 'kitUsers' }" @click="$emit('switch-view', 'kitUsers')">
          Manage Kit Users
        </li>
        <li :class="{ active: activeView === 'systemUsers' }" @click="$emit('switch-view', 'systemUsers')">
          Manage System Users
        </li>
        <li :class="{ active: activeView === 'citizenRecords' }" @click="$emit('switch-view', 'citizenRecords')">
          View Citizen Records
        </li>
      </ul>
    </nav>
  </aside>
</template>

<script>
export default {
  name: 'Sidebar',
  props: {
    activeView: {
      type: String,
      required: true
    },
    sidebarOpen: {
      type: Boolean,
      required: true
    }
  }
};
</script>

<style scoped>
  .sidebar {
    width: 220px;
    background-color: #006666;
    color: white;
    padding: 25px 20px;
    border-top-left-radius: 10px;
    border-bottom-left-radius: 10px;
    box-sizing: border-box;
    position: sticky;
    top: 0;
    height: calc(100vh - 40px);
    user-select: none;
    transition: width 0.3s ease, padding 0.3s ease;
  }

  .sidebar-closed {
    width: 0;
    padding: 0 !important;
    overflow: hidden;
  }

  .sidebar ul {
    list-style: none;
    padding-left: 0;
  }

  .sidebar li {
    padding: 15px 18px;
    margin-bottom: 14px;
    cursor: pointer;
    border-radius: 10px;
    font-weight: 600;
    font-size: 1rem;
    transition: background-color 0.25s ease, box-shadow 0.25s ease;
  }

    .sidebar li.active,
    .sidebar li:hover {
      background-color: #004d4d;
      box-shadow: inset 3px 0 0 0 #00bfa5;
    }
</style>
