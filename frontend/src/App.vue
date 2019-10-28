<template>
  <v-app class="d-flex justify-center align-center flex-column">
    <v-container>
      <router-view></router-view>
    </v-container>
  </v-app>
</template>

<script>
//import Login from "./components/Login.vue";

export default {
  name: "App",
  components: {}
};
</script>
<style>
form {
  padding: 16px;
}
.v-card {
  padding: 10px 0 10px 0;
}
</style>
