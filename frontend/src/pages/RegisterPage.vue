<script setup lang='ts'>
import { ref } from 'vue';
import { api } from 'src/boot/axios';
import { useQuasar } from 'quasar';

import { LocalStorage } from 'quasar';
import { authGet } from 'src/utils';

import { Router } from 'src/router';

const login = ref('');
const displayName = ref('');
const password = ref('');
const passwordRepeat = ref('');



const isPwd = ref(true);
const school = ref('');

const error = ref('');

const $q = useQuasar();

const stringOptions = [
  'ИМКТ', 'ВИ-ШРМИ', 'Политех', 'Мед', 'ШМИ'
]

const usernameRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter username'
];
const passwordRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter password'
];

const submitForm = () => {
  error.value = '';

  const formData = {
    login: login.value,
    password: password.value,
    displayName: displayName.value,
    school: school.value,
  };

  if (login.value === '' || password.value === '') {
    return;
  }

  api.post('/Auth/Reg', formData) //TODO: complete with api
    .then(response => {

      const msg = response.data.msg;


      const accessToken = response.data.data.accessToken;
      const refreshToken = response.data.data.refreshToken;

      LocalStorage.set('accessToken', accessToken);
      LocalStorage.set('refreshToken', refreshToken);
      LocalStorage.set('email', login.value);

      api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;

      $q.notify({
        message: 'Successful log in!',
        color: 'green-4',
        textColor: 'white',
        position: 'top',
        timeout: 1000
      });
      authGet('/Auth/GetMyself')
        .then(response => {
          const role = response.data.data.user.roleName;
          console.log(role);

          if (role === 'Administrator')
            Router.push({ path: '/admin' });
          if (role === 'User')
            Router.push({ path: '/user' });

          LocalStorage.set('roleName', role);
        });
    });
};

const closeBanner = () => {
  error.value = '';
  return;
};

function exit() {
  Router.push('/');
  return;
}
</script>

<template>
  <q-page class='column justify-center items-center'>
    <q-card
      class='q-px-lg q-pt-lg full-width column justify-center items-center'
      style='max-width: 25%'
    >
      <q-form class='fit column' @submit='submitForm'>
        <q-card-section class='q-pb-none'>

          <q-input
            label='Login'
            v-model='login'
            outlined
            dense
            lazy-rules
            :rules='usernameRules'
          />
          <q-input
            label='Display Name'
            v-model='displayName'
            outlined
            dense
            lazy-rules
            :rules='usernameRules'
          />
          <div class="q-gutter-md row q-mb-md">
            <q-select
              :dense="true"
              outlined
              v-model="school"
              label="School"
              :options="stringOptions"
              style="width: 300px"
              behavior="dialog"
            />
          </div>
          <q-input
            v-model='password'
            dense
            outlined
            lazy-rules
            :rules='passwordRules'
            :type="isPwd ? 'password' : 'text'"
            label='Password'
          >
            <template v-slot:append>
              <q-icon
                :name="isPwd ? 'visibility_off' : 'visibility'"
                class='cursor-pointer'
                @click='isPwd = !isPwd'
              />
            </template>
          </q-input>
           <q-input
            dense
            outlined
            :rules='passwordRules'
            v-model="passwordRepeat"
            :type="isPwd ? 'password' : 'text'"
            label="Repeat password"
           >
             <template v-slot:append>
              <q-icon
                :name="isPwd ? 'visibility_off' : 'visibility'"
                class='cursor-pointer'
                @click='isPwd = !isPwd'
              />
            </template>
           </q-input>
        </q-card-section>

        <q-card-section class='q-py-none'>
          <div>
            <q-banner
              v-if='error !== ""'
              class='relative fit bg-red-2 text-negative row'
              style='border-radius: 10px'
              @submit.prevent='submitForm'
            >
              <div class='row justify-center'>
                {{ error }}
              </div>
              <q-btn round flat size='8px'
                     @click='closeBanner'
                     class='q-mt-sm q-mr-lg absolute-top-right'
                     text-color='negative'
                     icon='close'
              />
            </q-banner>
          </div>
          <div class='row full-width justify-between q-pt-md q-px-md'>
            <q-btn class='col-4' color='primary' label='Reg in' type='submit' />
            <q-btn @click='exit' class='col-4' color='primary' label='Back' />
          </div>
        </q-card-section>
      </q-form>
    </q-card>
  </q-page>
</template>

<style scoped lang='sass'></style>
