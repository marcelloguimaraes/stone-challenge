<template>
  <v-container fluid>
    <v-snackbar
      v-model="snackbar"
      :color="colorSnackBar"
      :timeout="snackBarTimeout"
      bottom
    >{{ textSnackBar }}</v-snackbar>
    <v-card class="mx-auto" max-width="400">
      <v-card-title>Acesse sua conta</v-card-title>
      <v-divider></v-divider>
      <form @submit.prevent="submit">
        <v-text-field
          v-model="email"
          :error-messages="emailErrors"
          label="E-mail"
          @input="$v.email.$touch()"
          @blur="$v.email.$touch()"
        ></v-text-field>
        <v-text-field
          v-model="password"
          :error-messages="passwordErrors"
          label="Senha"
          type="password"
          @input="$v.password.$touch()"
          @blur="$v.password.$touch()"
        ></v-text-field>
        <v-btn
          type="submit"
          :disabled="$v.$invalid"
          color="success"
          class="mr-4"
          @click="submit"
        >Entrar</v-btn>
      </form>
      <p class="text-center">
        Não possui?
        <router-link to="open-account">Abra uma conta</router-link>
      </p>
    </v-card>
  </v-container>
</template>

<script>
import { validationMixin } from "vuelidate";
import { required, email } from "vuelidate/lib/validators";
import api from "../services/api";

export default {
  mixins: [validationMixin],

  validations: {
    email: { required, email },
    password: { required }
  },

  data: () => ({
    snackbar: false,
    colorSnackBar: "",
    textSnackBar: "",
    snackBarTimeout: 5000,

    email: "",
    password: ""
  }),

  computed: {
    emailErrors() {
      const errors = [];
      if (!this.$v.email.$dirty) return errors;
      !this.$v.email.email && errors.push("Digite um e-mail válido");
      !this.$v.email.required && errors.push("E-mail é obrigatório");
      return errors;
    },
    passwordErrors() {
      const errors = [];
      if (!this.$v.password.$dirty) return errors;
      !this.$v.password.required && errors.push("Senha é obrigatória.");
      return errors;
    }
  },

  methods: {
    async submit() {
      this.$v.$touch();
      if (this.$v.$invalid) {
        return false;
      }
        try {
          const response = await api.post("auth/login", {
            email: this.email,
            password: this.password
          });

          localStorage.setItem("token", response.data.token);
          localStorage.setItem("userId", response.data.user.id);
          localStorage.setItem("email", response.data.user.email);

          this.$router.push({ name: "dashboard"});
        } catch (error) {
          this.snackbar = true;
          this.colorSnackBar = "red";

          if (error.response) {
            this.textSnackBar = error.response.data;
          } else {
            this.textSnackBar = error;
          }
        }
    },
    clear() {
      this.$v.$reset();
      this.email = "";
      this.password = "";
    }
  }
};
</script>

<style scoped>
a {
  text-decoration: none;
}
</style>

