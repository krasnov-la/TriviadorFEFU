<script setup lang='ts'>
import { ref } from 'vue';
import { useQuasar } from 'quasar';
import { Router } from "src/router";

const email = ref('');
const password = ref('');

const isPwd = ref(true);

const error = ref('');

const usernameRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter username'
];
const passwordRules = [
  (val?: string) => (val && val.length > 0) || 'Please enter password'
];

const closeBanner = () => {
  error.value = '';
  return;
};

function exit() {
  window.close();
  return;
}

function toReg() {
  Router.push('/reg');
  return;
}
</script>

<template>
  <q-page class='column justify-center items-center'>
    <div style="font-size: 40px;" class="q-pb-lg">TriviadorFEFU</div>
    <q-card
      class='q-px-lg q-pt-lg q-mb-lg full-width column justify-center items-center'
      style='max-width: 25%'
    >
      <q-form class='fit column'>
        <q-card-section class='q-pb-none'>
          <q-input
            label='Email'
            v-model='email'
            outlined
            dense
            lazy-rules
            :rules='usernameRules'
          />
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
        </q-card-section>
        <q-card-section class='q-py-none'>
          <div>
            <q-banner
              v-if='error !== ""'
              class='relative fit bg-red-2 text-negative row'
              style='border-radius: 10px'
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
          <div class='row full-width justify-around q-pb-md'>
            <q-btn class='col-12' color='primary' label='Log in' type='submit' />
          </div>
        </q-card-section>
      </q-form>
    </q-card>
    <q-card
      class='q-px-lg q-pb-lg q-pt-md full-width column justify-center items-center'
      style='max-width: 25%'
    >
      <div class='row full-width justify-around q-pb-md'>
        <div class="q-pb-sm">Have not registered yet?</div>
        <q-btn @click='toReg' class='col-11' color='primary' label='Register'/>
      </div>
    </q-card>
  </q-page>
</template>

<style scoped lang='sass'></style>
