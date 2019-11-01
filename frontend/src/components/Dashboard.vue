<template>
  <v-container fluid>
    <v-snackbar
      v-model="snackbar"
      :color="colorSnackBar"
      :timeout="snackBarTimeout"
      left
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
        >{{account.balance | currency}}</p>
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
            <div v-if="transactions.length === 0 && operation.type === 'statement'">
              <div class="text-center">
                <h1>Ops!</h1>
                <br />
                <p>Nenhuma transação para esta conta ainda :(</p>
              </div>
            </div>
            <div v-else>
              <v-card-title class="headline" color="primary">{{ operation.name }}</v-card-title>
              <v-form v-if="operation.type !== 'statement'">
                <div v-if="operation.type === 'transfer'">
                  <v-text-field
                    type="number"
                    min="1"
                    max="9999"
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
                    label="Conta"
                    v-model="targetAccountNumber"
                    :error-messages="targetAccountNumberErrors"
                    @input="$v.targetAccountNumber.$touch"
                    @blur="$v.targetAccountNumber.$touch"
                  ></v-text-field>
                </div>
                <v-text-field
                  v-model="value"
                  prefix="R$ "
                  v-money="money"
                  label="Valor"
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
                      <th class="text-left subtitle-2">valor</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="transaction in transactions" :key="transaction.transactionId">
                      <td>{{ transaction.date }}</td>
                      <td>
                        {{ transaction.transactionType }}
                        <template
                          v-if="transaction.transactionType === 'Transferência'"
                        >
                          <v-tooltip top>
                            <template v-slot:activator="{ on }">
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
                      >{{ transaction.value | currency }}</td>
                    </tr>
                  </tbody>
                </template>
              </v-simple-table>
            </div>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                v-if="operation.type !== 'statement'"
                color="green darken-1"
                text
                @click="submitOperation(operation.type)"
                :disabled="$v.value.$invalid"
              >{{ operation.buttonText }}</v-btn>
              <v-btn color="red darken-1" text @click="dialog = false;">Fechar</v-btn>
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
import { required, maxLength } from "vuelidate/lib/validators";
import { mdiAccount } from "@mdi/js";
import { VMoney } from "v-money";

// const validValue = value => {
//   return value > 0.0 && value <= 1000000.0;
// };

export default {
  directives: { money: VMoney },
  data: () => ({
    snackbar: false,
    colorSnackBar: "",
    textSnackBar: "",
    snackBarTimeout: 6000,

    price: 123.45,
    money: {
      decimal: ",",
      thousands: ".",
      //prefix: "R$ ",
      suffix: "",
      precision: 2,
      masked: false
    },
    svgPath: mdiAccount,

    userId: localStorage.getItem("userId"),
    value: "",
    targetAccountNumber: 0,
    targetAccountAgency: 0,
    account: {},
    transactions: [],
    dialog: false,
    operation: {
      name: ""
    }
  }),
  mixins: [validationMixin],
  computed: {
    valueConverted() {
      let valueWithoutDot = this.value.replace(/[.]/g, "");
      let valueWithoutComma = valueWithoutDot.replace(/[,]/, ".");
      return valueWithoutComma === "" ? "" : parseFloat(valueWithoutComma);
    },
    valueErrors() {
      let errors = [];
      if (!this.$v.value.$dirty) return errors;
      !this.$v.value.required && errors.push("Valor é obrigatório");
      // !this.$v.value.validValue &&
      //   errors.push("Digite um valor entre 0 e 1000000");
      return errors;
    },
    targetAccountNumberErrors() {
      let errors = [];
      if (!this.$v.targetAccountNumber.$dirty) return errors;
      !this.$v.targetAccountNumber.required &&
        errors.push("Valor é obrigatório");
      // !this.$v.targetAccountNumber.validValue &&
      //   errors.push("Digite um valor válido");
      !this.$v.targetAccountNumber.maxLength &&
        errors.push("Deve ter no máximo 6 dígitos");
      return errors;
    },
    targetAccountAgencyErrors() {
      let errors = [];
      if (!this.$v.targetAccountAgency.$dirty) return errors;
      !this.$v.targetAccountAgency.required &&
        errors.push("Valor é obrigatório");
      // !this.$v.targetAccountAgency.validValue &&
      //   errors.push("Digite um valor válido");
      !this.$v.targetAccountAgency.maxLength &&
        errors.push("Deve ter no máximo 4 dígitos");
      return errors;
    }
  },
  validations: {
    value: { required },
    targetAccountNumber: {
      required,
      //validValue,
      maxLength: maxLength(6)
    },
    targetAccountAgency: {
      required,
      //validValue,
      maxLength: maxLength(4)
    }
  },
  async mounted() {
    await this.getAccount();
  },
  methods: {
    logOff() {
      localStorage.removeItem("token");
      localStorage.removeItem("userId");
      localStorage.removeItem("email");
      this.$router.push("/");
    },

    cleanModel() {
      this.value = "";
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
        debugger;
        let data = {
          accountNumber: this.account.accountNumber,
          agency: this.account.agency,
          value: parseFloat(this.valueConverted)
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
            value: parseFloat(this.valueConverted)
          };
          url = "/accounts/transfer";
        }
        await api.post(url, data, { headers });

        await this.getAccount();

        this.dialog = false;
        this.snackbar = true;
        this.colorSnackBar = "success";
        this.textSnackBar = "Operação realizada com sucesso";
        debugger;
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
            type: "transfer"
          };
          break;
        case "statement":
          this.operation = {
            name: "Extrato",
            type: "statement"
          };
          await this.getTransactions();
          this.dialog = true;
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
        console.log(error);
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