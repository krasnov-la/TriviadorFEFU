<script setup lang='ts'>
  import { ref } from 'vue';
  import { QDialog } from 'quasar';

  defineProps<{
    active: boolean,
  }>();

  const emit = defineEmits<{
    (e: 'ok', payload?: string): void,
    (e: 'hide'): void,
    (e: 'back'): void,
    (e: 'save'): void,
    (e: 'forget'): void,
  }>();

  const dialogRef = ref<QDialog>();

  const hide = () => {
    if(dialogRef.value !== undefined) dialogRef.value.hide();
  }

  const onDialogHide = () => {
    emit('hide')
  }

  const back = () => {
    emit('back');
    hide();
  };

  const save = () => {
    emit('save');
    hide();
  };

  const forget = () => {
    emit('forget');
    hide();
  };

</script>

<template>
  <q-dialog 
    :model-value='active'
    ref='dialogRef'
    @hide='onDialogHide'
  >
    <q-card 
      class='column full-width' 
      style='min-width: 50%;'
    >
      <q-card-section class='row text-h4 justify-center'>
        <span>Save?</span>
      </q-card-section>
      <q-card-section class='row justify-around'>
        <q-btn 
          label='Back to profile' 
          class='col-3' 
          color='primary' 
          @click='back'
        />
        <q-btn 
          label='Save' 
          class='col-3' 
          color='primary' 
          @click='save'
        />
        <q-btn 
          label='Forget changes' 
          class='col-3' 
          color='primary' 
          @click='forget'
        />
      </q-card-section>
    </q-card>
  </q-dialog>
</template>