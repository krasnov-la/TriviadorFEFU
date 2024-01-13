<script setup lang="ts">
import { ref } from 'vue';
import { api } from 'src/boot/axios';
import { useQuasar } from 'quasar';

import { LocalStorage } from 'quasar';
import { authGet } from 'src/utils';

import { Router } from 'src/router';

import { useAuthStore } from 'src/stores/auth';

const login = ref('');
const displayName = ref('');
const password = ref('');
const passwordRepeat = ref('');

const isPwd = ref(true);
const school = ref('');

const errorMes = ref('');

const $q = useQuasar();

const stringOptions = ['ИМКТ', 'ВИ-ШРМИ', 'Политех', 'Мед', 'ШМИ'];

const usernameRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter username',
];

const passwordRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter password',
];

const schoolRules = [
  (val?: string) => (val && val.length > 0) || 'Please choose school',
];

const nameRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter display name',
];

const submitForm = () => {
  errorMes.value = '';

  const formData = {
    login: login.value,
    displayName: displayName.value,
    password: password.value,
    school: school.value,
  };

  if (
    login.value === '' ||
    password.value === '' ||
    school.value === '' ||
    displayName.value === ''
  )
    return;

  api
    .post('/Auth/Registrate', formData)
    .then((response) => {
      const accessToken = response.data.accessToken;
      const refreshToken = response.data.refreshToken;

      useAuthStore().updateTokensManually({
        accessToken,
        refreshToken,
      });

      api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;

      if (response.status === 200) {
        Router.push('/lobby');
      }
      // authGet('/Auth/GetMyself')
      //   .then(response => {
      //     const role = response.data.data.user.roleName;
      //     console.log(role);
      //
      //     if (role === 'Administrator')
      //       Router.push({ path: '/admin' });
      //     if (role === 'User')
      //       Router.push({ path: '/user' });
      //
      //     LocalStorage.set('roleName', role);
      //   });
    })
    .catch((error) => {
      console.log(error);
      errorMes.value = 'Login already taken';
    });
};

const closeBanner = () => {
  errorMes.value = '';
  return;
};
</script>

<template>
  <q-page class="column justify-center items-center">
    <div style="font-size: 40px" class="q-pb-lg">TriviadorFEFU</div>
    <q-card
      class="q-px-lg q-pt-lg full-width column justify-center items-center"
      style="max-width: 25%"
    >
      <q-form class="fit column" @submit="submitForm">
        <q-card-section class="q-pb-none">
          <q-input
            label="Login"
            v-model="login"
            outlined
            dense
            lazy-rules
            :rules="usernameRules"
          />
          <q-input
            label="Display Name"
            v-model="displayName"
            outlined
            dense
            lazy-rules
            :rules="nameRules"
          />
          <div class="q-gutter-md row">
            <q-select
              :dense="true"
              outlined
              v-model="school"
              label="School"
              :options="stringOptions"
              :rules="schoolRules"
              style="width: 300px"
              behavior="dialog"
            />
          </div>
          <q-input
            v-model="password"
            dense
            outlined
            lazy-rules
            :rules="passwordRules"
            :type="isPwd ? 'password' : 'text'"
            label="Password"
          >
            <template v-slot:append>
              <q-icon
                :name="isPwd ? 'visibility_off' : 'visibility'"
                class="cursor-pointer"
                @click="isPwd = !isPwd"
              />
            </template>
          </q-input>
          <q-input
            dense
            outlined
            :rules="passwordRules"
            v-model="passwordRepeat"
            :type="isPwd ? 'password' : 'text'"
            label="Repeat password"
          >
            <template v-slot:append>
              <q-icon
                :name="isPwd ? 'visibility_off' : 'visibility'"
                class="cursor-pointer"
                @click="isPwd = !isPwd"
              />
            </template>
          </q-input>
        </q-card-section>

        <q-card-section class="q-py-none">
          <div>
            <q-banner
              v-if="errorMes !== ''"
              class="relative fit bg-red-2 text-negative row"
              style="border-radius: 10px"
              @submit.prevent="submitForm"
            >
              <div class="row justify-center">
                {{ errorMes }}
              </div>
              <q-btn
                round
                flat
                size="8px"
                @click="closeBanner"
                class="q-mt-sm q-mr-lg absolute-top-right"
                text-color="negative"
                icon="close"
              />
            </q-banner>
          </div>
          <div class="row full-width justify-between q-pt-md q-px-md">
            <q-btn class="col-4" color="primary" label="Reg in" type="submit" />
            <q-btn to="/" class="col-4" color="primary" label="Back" />
          </div>
        </q-card-section>
      </q-form>
    </q-card>
  </q-page>
</template>

<style lang="sass">
body
  background-color: #4481eb
  background-image: linear-gradient(to top, #4481eb 0%, #04befe 100%)

.profile
  text-decoration: none
  color: black
  display: block
  position: relative

  &:after
    position: absolute
    bottom: 0
    left: 0
    right: 0
    margin: auto
    width: 0
    content: '.'
    color: transparent
    background: black
    height: 1px
    transition: all 0.3s

  &:hover:after
    width: 100%
</style>
