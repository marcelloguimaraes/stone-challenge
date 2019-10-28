<template>
  <v-container fluid>
    <v-snackbar
      v-model="snackbar"
      :color="colorSnackBar"
      :timeout="snackBarTimeout"
      bottom
    >{{ textSnackBar }}</v-snackbar>
    <v-card class="mx-auto" max-width="400">
      <v-card-title>
        Bem-vindo, {{ account.customerName }}
        <v-btn text color="red" @click="logOff">Sair</v-btn>
      </v-card-title>
      <v-card-text>
        <p class="subtitle-1 font-regular">
          saldo em conta
          <a
            class="text-center"
            text
            color="primary"
            small
            @click="configureModal('statement'); dialog = true"
          >ver extrato</a>
        </p>
        <span
          class="subtitle-1"
          :class="account.balance >= 0 ? 'green--text' : 'red--text'"
        >R$ {{balance}}</span>
      </v-card-text>
      <v-row justify="center">
        <v-dialog v-model="dialog" max-width="500">
          <template v-slot:activator="{ on }">
            <v-card-actions>
              <v-btn
                text
                color="primary"
                x-large
                v-on="on"
                @click="configureModal('withdraw')"
              >Sacar</v-btn>
              <v-btn
                text
                color="primary"
                x-large
                v-on="on"
                @click="configureModal('deposit')"
              >Depositar</v-btn>
              <v-btn
                text
                color="primary"
                x-large
                v-on="on"
                @click="configureModal('transfer')"
              >Transferir</v-btn>
            </v-card-actions>
          </template>
          <v-card>
            <v-card-title class="headline">{{ operation.name }}</v-card-title>
            <v-form v-show="operation.type !== 'statement'">
              <div v-show="operation.type === 'transfer'">
                <v-text-field
                  type="number"
                  min="1"
                  max="9999"
                  maxlength="4"
                  label="Agência"
                  v-model="targetAccount.agency"
                  :error-messages="targetAccountErrors"
                  @input="$v.value.$touch()"
                  @blur="$v.value.$touch()"
                ></v-text-field>
                <v-text-field
                  type="number"
                  min="1"
                  max="999999"
                  maxlength="6"
                  label="Conta"
                  v-model="targetAccount.accountNumber"
                  :error-messages="targetAccountErrors"
                  @input="$v.value.$touch()"
                  @blur="$v.value.$touch()"
                ></v-text-field>
              </div>
              <v-text-field
                type="number"
                v-model="value"
                label="Valor"
                prefix="R$"
                :error-messages="valueErrors"
                @input="$v.value.$touch()"
                @blur="$v.value.$touch()"
              ></v-text-field>
            </v-form>
            <v-simple-table height="400px" fixed-header v-show="operation.type === 'statement'">
              <template v-slot:default>
                <thead>
                  <tr>
                    <th class="text-left subtitle-2">Data Transação</th>
                    <th class="text-left subtitle-2">Tipo</th>
                    <th class="text-left subtitle-2">Valor</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="transaction in transactions" :key="transaction.transactionId">
                    <td>{{ transaction.date }}</td>
                    <td>{{ transaction.transactionType }}</td>
                    <td
                      :class="transaction.value >= 0 ? 'green--text' : 'red--text'"
                    >R$ {{ transaction.value }}</td>
                  </tr>
                </tbody>
              </template>
            </v-simple-table>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                v-show="operation.type !== 'statement'"
                color="green darken-1"
                text
                @click="submitOperation(operation.type)"
                :disabled="$v.$invalid"
              >{{ operation.buttonText }}</v-btn>
              <v-btn color="red darken-1" text @click="dialog = false">Fechar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-row>
    </v-card>
  </v-container>
</template>
<script>
import api from "../services/api";
import { validationMixin } from "vuelidate";
import { required } from "vuelidate/lib/validators";

const validValue = value => value > 0;

export default {
  mixins: [validationMixin],
  computed: {
    balance() {
      return this.account.balance.toFixed(2);
    },
    valueErrors() {
      let errors = [];
      if (!this.$v.value.$dirty) return errors;
      // !this.$v.value.value && errors.push("Digite um valor válido");
      !this.$v.value.required && errors.push("Valor é obrigatório");
      !this.$v.value.validValue && errors.push("Digite um valor válido");
      return errors;
    },
    targetAccountErrors() {
      let errors = [];
      if (!this.$v.value.$dirty) return errors;
      // !this.$v.value.value && errors.push("Digite um valor válido");
      !this.$v.targetAccount.required && errors.push("Valor é obrigatório");
      !this.$v.targetAccount.validValue &&
        errors.push("Digite um valor válido");
      return errors;
    }
  },
  validations: {
    value: { required, validValue },
    targetAccount: { required, validValue }
  },
  data: () => ({
    snackbar: false,
    colorSnackBar: "",
    textSnackBar: "",
    snackBarTimeout: 3000,

    userId: localStorage.getItem("userId"),
    value: 0.0,
    targetAccount: { accountNumber: 0, agency: 0 },
    account: {},
    transactions: [],
    dialog: false,
    operation: {
      name: ""
    }
  }),
  async mounted() {
    await this.getAccount();
    await this.getTransactions();
  },
  methods: {
    logOff() {
      localStorage.removeItem("token");
      localStorage.removeItem("userId");
      localStorage.removeItem("email");
      this.$router.push("/");
    },

    cleanModel() {
      this.value = 0;
      this.targetAccount.accountNumber = 0;
      this.targetAccount.agency = 0;
    },

    getHeaders() {
      return {
        "content-type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`
      };
    },

    async submitOperation(operation) {
      try {
        const headers = this.getHeaders();
        let url = "";

        let data = {
          accountNumber: this.account.accountNumber,
          agency: this.account.agency,
          value: parseFloat(this.value)
        };

        if (operation === "withdraw") {
          url = "/accounts/withdraw";
        } else if (operation === "deposit") {
          url = "/accounts/deposit";
        } else {
          data = {
            sourceAccount: {
              accountNumber: this.account.accountNumber,
              agency: this.account.agency
            },
            targetAccount: {
              accountNumber: this.targetAccount.accountNumber,
              agency: this.targetAccount.agency
            },
            value: this.value
          };
          url = "/accounts/transfer";
        }
        await api.post(url, data, { headers });

        await this.getAccount();

        this.cleanModel();
        this.dialog = false;
        this.snackbar = true;
        this.colorSnackBar = "success";
        this.textSnackBar = "Operação realizada com sucesso";
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
    async configureModal(operation) {
      switch (operation) {
        case "withdraw":
          this.operation = {
            name: "Saque",
            buttonText: "Sacar",
            type: "withdraw"
          };
          break;
        case "deposit":
          this.operation = {
            name: "Depósito",
            buttonText: "Depositar",
            type: "deposit"
          };
          break;
        case "transfer":
          this.operation = {
            name: "transferência",
            buttonText: "Transferir",
            type: "transfer",
            value: this.value
          };
          break;
        case "statement":
          this.operation = {
            name: "Extrato",
            type: "statement"
          };
          await this.getTransactions();
          break;
      }
    },

    async getAccount() {
      try {
        const response = await api.get(`accounts/users/${this.userId}`, {
          headers: {
            "content-type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`
          }
        });
        this.account = response.data;
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
    async getTransactions() {
      try {
        this.transactions = [];
        const response = await api.post(
          "accounts/transactions",
          {
            accountNumber: this.account.accountNumber,
            agency: this.account.agency
          },
          {
            headers: {
              "content-type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("token")}`
            }
          }
        );

        if (response.data != null) {
          response.data.forEach(t => {
            this.transactions.push({
              transactionType: t.transactionType,
              date: t.dateFormatted,
              value: t.value
            });
          });
        }
      } catch (error) {
        this.snackbar = true;
        this.colorSnackBar = "red";
        if (error.response) {
          this.textSnackBar = error.response.data;
        } else {
          this.textSnackBar = error;
        }
      }
    }
  }
};
</script>
<style scoped>
form {
  padding: 0 24px 20px;
}
</style>