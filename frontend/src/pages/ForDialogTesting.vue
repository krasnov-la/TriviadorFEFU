<script setup lang='ts'>
  import { ref } from 'vue';
  import ProfilePopup from 'components/ProfilePopup.vue';
  import ConfirmProfilePopup from 'components/ConfirmProfilePopup.vue';

  const email = ref('');
  const password = ref('');

  const profileActive = ref(true);

  const close = () => {
    profileActive.value = false;
  };
    
</script>

<template>
  <q-page class='column justify-center items-center'>
    <div 
      style="font-size: 40px;" 
      class="q-pb-lg"
    >
      TriviadorFEFU
    </div>
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
          />
          <q-input
            v-model='password'
            dense
            outlined
            lazy-rules
            label='Password'
          >
          </q-input>
        </q-card-section>
        <q-card-section class='q-py-none'>
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
        <q-btn class='col-11' color='primary' label='Register' type='submit' />
      </div>
    </q-card>
    <ProfilePopup
      v-model:active="profileActive"
      @close="close"
    />
  </q-page>
</template>

<style scoped lang='sass'></style>