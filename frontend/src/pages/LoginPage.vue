<script setup lang="ts">
import { ref } from 'vue';
import { api } from 'src/boot/axios';
import { Router } from 'src/router';

import { useAuthStore } from 'src/stores/auth';
import { useUserDataStore } from 'src/stores/user-data';

const login = ref('');
const password = ref('');

const isPwd = ref(true);

const errorMes = ref('');

const usernameRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter username',
];
const passwordRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter password',
];

const submitForm = () => {
  errorMes.value = '';

  const formData = {
    login: login.value,
    password: password.value,
  };

  api
    .post('/Auth/Login', formData)
    .then((response) => {
      console.log(response);
      const accessToken = response.data.accessToken;
      const refreshToken = response.data.refreshToken;

      useAuthStore().updateTokensManually({
        accessToken,
        refreshToken,
      });

      api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;

      const userDataStore = useUserDataStore();
      userDataStore.getUserDataFromServer();

      if (response.status === 200) {
        Router.push('/lobby');
      }
    })
    .catch((error) => {
      console.log(error);
      errorMes.value = 'Wrong username or password';
    });
};

const closeBanner = () => {
  errorMes.value = '';
  return;
};

function exit() {
  window.close();
  return;
}

function toReg() {
  window.location.href = '';
  //Router.push('/reg');
  return;
}
</script>

<template>
  <q-page class="column justify-center items-center">
    <div style="font-size: 40px" class="q-pb-lg">TriviadorFEFU</div>
    <q-card
      class="q-px-lg q-pt-lg q-mb-lg full-width column justify-center items-center"
      style="max-width: 25%"
    >
      <q-form class="fit column" @submit="submitForm">
        <q-card-section class="q-pb-none">
          <q-input
            label="Email"
            v-model="login"
            outlined
            dense
            lazy-rules
            :rules="usernameRules"
          />
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
        </q-card-section>
        <q-card-section class="q-py-none">
          <div>
            <q-banner
              v-if="errorMes !== ''"
              class="relative fit bg-red-2 text-negative row q-mb-md"
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
          <div class="row full-width justify-around q-pb-md">
            <q-btn
              class="col-12"
              color="primary"
              label="Log in"
              type="submit"
            />
          </div>
        </q-card-section>
      </q-form>
    </q-card>
    <q-card
      class="q-px-lg q-pb-lg q-pt-md full-width column justify-center items-center"
      style="max-width: 25%"
    >
      <div class="row full-width justify-around q-pb-md">
        <div class="q-pb-sm">Have not registered yet?</div>
        <q-btn to="/reg" class="col-11" color="primary" label="Register" />
      </div>
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
