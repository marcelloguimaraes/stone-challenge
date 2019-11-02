<template>
  <v-container fluid>
    <v-snackbar v-model="snackbar" :color="colorSnackBar" :timeout="snackBarTimeout" left>
      <p v-html="textSnackBar"></p>
    </v-snackbar>
    <v-card class="mx-auto" max-width="500">
      <v-card-title class="text-center">
        <router-link to="/" title="Voltar">
          <v-icon large class="float-left">keyboard_arrow_left</v-icon>
        </router-link>Abrir conta
      </v-card-title>
      <v-divider></v-divider>
      <v-form ref="form">
        <v-row>
          <v-col cols="12">
            <v-text-field
              :error-messages="nameErrors"
              v-model="name"
              label="Nome Completo"
              @blur="$v.name.$touch"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field :error-messages="emailErrors" v-model="email" label="E-mail" type="email"></v-text-field>
            <v-text-field
              :error-messages="agencyErrors"
              v-model="agency"
              label="Agência"
              type="number"
            ></v-text-field>
          </v-col>
          <v-col cols="6">
            <v-text-field
              :error-messages="passwordErrors"
              v-model="password"
              label="Senha"
              type="password"
            ></v-text-field>
            <v-text-field :error-messages="cpfErrors" v-model="cpf" v-mask="mask" label="CPF"></v-text-field>
          </v-col>
        </v-row>
        <!-- <v-date-picker v-model="birthDate" color="green lighten-1" header-color="primary"></v-date-picker> -->
        <v-menu
          ref="menu"
          v-model="menu"
          :close-on-content-click="false"
          transition="scale-transition"
          offset-y
          max-width="290px"
          min-width="290px"
        >
          <template v-slot:activator="{ on }">
            <v-text-field
              v-model="birthDate"
              v-mask="dateMask"
              :error-messages="birthDateErrors"
              label="Data de Nascimento"
              persistent-hint
              @blur="date = parseDate(birthdate)"
              v-on="on"
            ></v-text-field>
          </template>
          <v-date-picker
            v-model="date"
            no-title
            @input="menu = false"
            :first-day-of-week="0"
            locale="pt"
          ></v-date-picker>
        </v-menu>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click="submit" :disabled="$v.$anyError">Enviar</v-btn>
          <v-btn color="red" text @click="reset">Limpar</v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-container>
</template>
<script>
import { validationMixin } from "vuelidate";
import { required, email } from "vuelidate/lib/validators";
import validateCpf from "../plugins/cpf";
import { mask } from "vue-the-mask";
import api from "../services/api";

const validValue = value => value > 0;

export default {
  directives: { mask },
  mounted() {
    this.$v.$touch();
  },
  data: vm => ({
    date: new Date().toISOString().substr(0, 10),
    dateFormatted: vm.formatDate(new Date().toISOString().substr(0, 10)),
    snackbar: false,
    colorSnackBar: "",
    textSnackBar: "",
    snackBarTimeout: 6000,
    menu: false,
    email: "",
    password: "",
    agency: 0,
    cpf: "",
    mask: "###.###.###-##",
    dateMask: "##/##/####",
    name: "",
    birthDate: vm.formatDate(new Date().toISOString().substr(0, 10))
  }),

  validations: {
    email: { required, email },
    password: { required },
    agency: { required, validValue },
    cpf: { required, validateCpf },
    name: { required },
    birthDate: { required }
  },

  computed: {
    computedDateFormatted() {
      return this.formatDate(this.date);
    },
    nameErrors() {
      let errors = [];
      if (!this.$v.name.$dirty) return errors;
      !this.$v.name.required && errors.push("Valor é obrigatório");
      return errors;
    },
    emailErrors() {
      let errors = [];
      if (!this.$v.email.$dirty) return errors;
      !this.$v.email.email && errors.push("Digite um e-mail válido");
      !this.$v.email.required && errors.push("Valor é obrigatório");
      return errors;
    },
    passwordErrors() {
      let errors = [];
      if (!this.$v.password.$dirty) return errors;
      !this.$v.password.required && errors.push("Valor é obrigatório");
      return errors;
    },
    agencyErrors() {
      let errors = [];
      if (!this.$v.agency.$dirty) return errors;
      !this.$v.agency.required && errors.push("Valor é obrigatório");
      !this.$v.agency.validValue && errors.push("Valor inválido");

      return errors;
    },
    cpfErrors() {
      let errors = [];
      if (!this.$v.cpf.$dirty) return errors;
      !this.$v.cpf.required && errors.push("Valor é obrigatório");
      !this.$v.cpf.validateCpf && errors.push("Cpf inválido");

      return errors;
    },
    birthDateErrors() {
      let errors = [];
      if (!this.$v.birthDate.$dirty) return errors;
      !this.$v.birthDate.required && errors.push("Valor é obrigatório");
      return errors;
    }
  },

  watch: {
    date(val) {
      this.birthDate = this.formatDate(this.date);
    }
  },

  methods: {
    async submit() {
      this.$v.$touch();
      if (this.$v.$invalid) {
        return false;
      }

      try {
        let { email, password, agency, cpf, name, birthDate } = this;

        cpf = cpf.replace(/[-.]/g, "");
        birthDate = this.parseDate(birthDate);

        const response = await api.post("auth/open-account", {
          email,
          password,
          agency,
          customer: {
            cpf,
            name,
            birthDate
          }
        });

        this.$router.push({
          name: "account-created",
          params: { accountNumber: response.data.accountNumber }
        });
      } catch (error) {
        this.snackbar = true;
        this.colorSnackBar = "red";
        if (error.response) {
          let msg = "";
          let data = error.response.data;
          this.textSnackBar = data;
        } else {
          this.textSnackBar = error;
        }
      }
    },
    reset() {
      this.$refs.form.reset();
    },
    formatDate(date) {
      if (!date) return null;

      const [year, month, day] = date.split("-");
      return `${day}/${month}/${year}`;
    },
    parseDate(date) {
      if (!date) return null;

      const [day, month, year] = date.split("/");
      return `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}`;
    }
  }
};
</script>
<style scoped>
</style>