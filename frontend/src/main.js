import Vue from "vue";
import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import Vuelidate from "vuelidate";
import VueRouter from "vue-router";
import router from "./router";
import VuePageTransition from "vue-page-transition";
import VueCurrencyFilter from "vue-currency-filter";
import money from "v-money";

Vue.config.productionTip = false;

Vue.use(money, { precision: 4 });
Vue.use(VuePageTransition);
Vue.use(Vuelidate);
Vue.use(VueRouter);
Vue.use(VueCurrencyFilter, {
  symbol: "R$",
  thousandsSeparator: ".",
  fractionCount: 2,
  fractionSeparator: ",",
  symbolPosition: "front",
  symbolSpacing: true
});

new Vue({
  vuetify,
  router,
  render: h => h(App)
}).$mount("#app");
