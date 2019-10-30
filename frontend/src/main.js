import Vue from "vue";
import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import Vuelidate from "vuelidate";
import VueRouter from "vue-router";
import router from "./router";
import "v-currency-field/dist/index.css";
import VuePageTransition from 'vue-page-transition'


Vue.config.productionTip = false;

Vue.use(VuePageTransition)
Vue.use(Vuelidate);
Vue.use(VueRouter);

new Vue({
  vuetify,
  router,
  render: h => h(App)
}).$mount("#app");
