import Login from "./components/Login.vue";
import Dashboard from "./components/Dashboard.vue";
import OpenAccount from "./components/OpenAccount.vue";
import AccountCreated from "./components/AccountCreated.vue";
import VueRouter from "vue-router";

const routes = [
  { path: "/", name: "login", component: Login, meta: { guest: true } },
  {
    path: "/dashboard",
    name: "dashboard",
    component: Dashboard,
    meta: { requiresAuth: true, transition: "fade-in-down" }
  },
  {
    path: "/open-account",
    name: "open-account",
    component: OpenAccount,
    meta: { guest: true }
  },
  {
    path: "/account-created",
    name: "account-created",
    component: AccountCreated,
    meta: { guest: true, transition: "fade-in-up" },
    props: true,
    beforeEnter: (to, from, next) => {
      if (Object.keys(to.params).length === 0) {
        next(false);
      } else {
        next();
      }
    }
  }
];

const router = new VueRouter({ routes });

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("token");
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (token == null) {
      next({
        path: "/",
        params: { nextUrl: to.fullPath }
      });
    } else {
      next();
    }
  } else if (to.matched.some(record => record.meta.guest)) {
    if (token == null) {
      next();
    } else {
      next({ name: "dashboard" });
    }
  } else {
    // rota não existe, retorna pro login
    next({ name: "login" });
  }
});

export default router;
