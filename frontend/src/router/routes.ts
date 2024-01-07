import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/UnloggedLayout.vue'),
    children: [
      { 
        path: '', component: () => import('pages/LoginPage.vue')
      },
      { 
        path: 'testdialog', component: () => import('pages/ForDialogTesting.vue')
      },
      {
        path: 'reg',   component: () => import('pages/RegisterPage.vue')
      },
      {
        path: 'lobby',   component: () => import('pages/MainLobby.vue')
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
