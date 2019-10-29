<template>
  <v-container fluid>
    <v-snackbar
      v-model="snackbar"
      :color="colorSnackBar"
      :timeout="snackBarTimeout"
      bottom
    >{{ textSnackBar }}</v-snackbar>
    <v-card class="mx-auto" max-width="400">
      <v-card-title class="d-flex justify-space-between">
        Bem-vindo(a), {{ account.customerName }} :)
        <v-btn text color="red" @click="logOff">Sair</v-btn>
      </v-card-title>
      <v-card-text>
        <span class="subtitle-1 font-regular">saldo em conta</span>
        <a
          class="seeStatement subtitle-1 font-weight-bold"
          text
          color="primary"
          small
          @click="configureModal('statement');"
        >ver extrato</a>
        <p
          class="subtitle-1"
          :class="account.balance >= 0 ? 'green--text' : 'red--text'"
        >R$ {{balance}}</p>
      </v-card-text>
      <v-row justify="center">
        <v-dialog v-model="dialog" :max-width="operation.type === 'statement' ? 500 : 300">
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
            <v-form v-if="operation.type !== 'statement'">
              <div v-if="operation.type === 'transfer'">
                <v-text-field
                  type="number"
                  min="1"
                  max="9999"
                  maxlength="4"
                  label="Agência"
                  v-model="targetAccountAgency"
                  :error-messages="targetAccountAgencyErrors"
                  @input="$v.targetAccountAgency.$touch"
                  @blur="$v.targetAccountAgency.$touch"
                ></v-text-field>
                <v-text-field
                  type="number"
                  min="1"
                  max="999999"
                  maxlength="6"
                  label="Conta"
                  v-model="targetAccountNumber"
                  :error-messages="targetAccountNumberErrors"
                  @input="$v.targetAccountNumber.$touch"
                  @blur="$v.targetAccountNumber.$touch"
                ></v-text-field>
              </div>
              <v-text-field
                type="number"
                v-model="value"
                label="Valor"
                prefix="R$"
                :error-messages="valueErrors"
                @blur="$v.value.$touch"
              ></v-text-field>
            </v-form>
            <v-simple-table height="400px" fixed-header v-if="operation.type === 'statement'">
              <template v-slot:default>
                <thead>
                  <tr>
                    <th class="text-left subtitle-2">data transação</th>
                    <th class="text-left subtitle-2">tipo</th>
                    <th class="text-left subtitle-2">valor(R$)</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="transaction in transactions" :key="transaction.transactionId">
                    <td>{{ transaction.date }}</td>
                    <td>
                      {{ transaction.transactionType }}
                      <template v-if="transaction.transactionType === 'Transferência'">
                        <v-tooltip top>
                          <template v-slot:activator="{ on }" >
                            <v-btn icon v-on="on">
                              <v-icon small color="blue lighten-1">info</v-icon>
                            </v-btn>
                          </template>
                          <span>{{ transaction.note }}</span>
                        </v-tooltip>
                      </template>
                    </td>
                    <td
                      :class="transaction.value >= 0 ? 'green--text' : 'red--text'"
                    >{{ transaction.value }}</td>
                  </tr>
                </tbody>
              </template>
            </v-simple-table>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                v-if="operation.type !== 'statement'"
                color="green darken-1"
                text
                @click="submitOperation(operation.type)"
                :disabled="$v.value.$invalid"
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
      !this.$v.value.required && errors.push("Valor é obrigatório");
      !this.$v.value.validValue && errors.push("Digite um valor válido");
      return errors;
    },
    targetAccountNumberErrors() {
      let errors = [];
      if (!this.$v.targetAccountNumber.$dirty) return errors;
      !this.$v.targetAccountNumber.required &&
        errors.push("Valor é obrigatório");
      !this.$v.targetAccountNumber.validValue &&
        errors.push("Digite um valor válido");
      return errors;
    },
    targetAccountAgencyErrors() {
      let errors = [];
      if (!this.$v.targetAccountAgency.$dirty) return errors;
      !this.$v.targetAccountAgency.required &&
        errors.push("Valor é obrigatório");
      !this.$v.targetAccountAgency.validValue &&
        errors.push("Digite um valor válido");
      return errors;
    }
  },
  validations: {
    value: { required, validValue },
    targetAccountNumber: {
      required,
      validValue
    },
    targetAccountAgency: {
      required,
      validValue
    }
  },
  data: () => ({
    snackbar: false,
    colorSnackBar: "",
    textSnackBar: "",
    snackBarTimeout: 3000,

    userId: localStorage.getItem("userId"),
    value: 0.0,
    targetAccountNumber: 0,
    targetAccountAgency: 0,
    account: {},
    transactions: [],
    dialog: false,
    operation: {
      name: ""
    }
  }),
  async mounted() {
    await this.getAccount();
    //await this.getTransactions();
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
      this.targetAccountNumber = 0;
      this.targetAccountAgency = 0;
    },

    getHeaders() {
      return {
        "content-type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`
      };
    },

    async submitOperation(operation) {
      try {
        this.$v.$touch();

        if (!this.$v.value.$invalid) {
          if (
            operation === "transfer" &&
            (this.$v.targetAccountNumber.$invalid ||
              this.$v.targetAccountAgency.$invalid)
          ) {
            return false;
          }
        }

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
              accountNumber: this.targetAccountNumber,
              agency: this.targetAccountAgency
            },
            value: this.value
          };
          url = "/accounts/transfer";
        }
        await api.post(url, data, { headers });

        await this.getAccount();

        this.dialog = false;
        this.snackbar = true;
        this.colorSnackBar = "success";
        this.textSnackBar = "Operação realizada com sucesso";
        this.cleanModel();
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
      this.cleanModel();
      this.$v.$reset();
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
            name: "Transferência",
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
          if (this.transactions.length > 0) this.dialog = true;
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
              value: t.value,
              note: t.note
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
.seeStatement {
  margin-left: 50px;
}
</style>