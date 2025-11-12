<template>
  <div class="not-found-container">
    <div class="not-found-content">
      <h1 class="error-code">404</h1>
      <h2 class="error-message">Page Not Found</h2>
      <p class="error-detail">
        Oops! We couldn't find the page you were looking for.
        It might have been moved, deleted, or you might have mistyped the address.
      </p>
      <button @click="$router.push('/')" class="home-button">
        Go to Dashboard
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'NotFoundView'
};
</script>

<style scoped>
  .not-found-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 80vh;
    text-align: center;
    background-color: #f8f9fa;
    padding: 20px;
  }

  .not-found-content {
    background: white;
    padding: 40px;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    max-width: 450px;
    width: 100%;
  }

  .error-code {
    font-size: 6rem;
    font-weight: 900;
    color: #dc3545; /* Red for error */
    margin: 0;
    line-height: 1;
  }

  .error-message {
    font-size: 2rem;
    color: #343a40;
    margin: 10px 0 20px 0;
  }

  .error-detail {
    font-size: 1rem;
    color: #6c757d;
    margin-bottom: 30px;
  }

  .home-button {
    padding: 12px 25px;
    border: none;
    border-radius: 8px;
    background-color: #007bff;
    color: white;
    font-weight: 600;
    cursor: pointer;
    transition: background-color 0.3s, transform 0.1s;
  }

    .home-button:hover {
      background-color: #0056b3;
    }
</style>
