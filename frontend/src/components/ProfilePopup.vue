<script setup lang='ts'>
  import { ref } from "vue";
  import ConfirmProfilePopup from 'components/ConfirmProfilePopup.vue';
  import { QDialog } from 'quasar';

  const props = defineProps<{
    active: boolean,
  }>();

  const emit = defineEmits<{
    (e: "close"): void,
  }>();

  const rating = ref(0);
  const displayName = ref("");
  const login = ref("");
  const selectedSchool = ref(null);
  const availableSchools = ["IMKT", "IP"];

  const activeConfirm = ref(false);

  const dialogRef = ref<QDialog>();

  const hide = () => {
    if(dialogRef.value !== undefined) dialogRef.value.hide();
  }

  const backToProfile = () => {
    console.log("backToProfile");
    activeConfirm.value = false;
  };

  const saveProfile = () => {
    console.log("saveProfile");
    activeConfirm.value = false;
    emit("close");
    hide();
  };

  const forgetProfileChanges = () => {
    console.log("forgetProfileChanges");
    activeConfirm.value = false;
    emit("close");
    hide();
  }
</script>

<template>
  <q-dialog 
    :model-value='active'
    @hide='() => { activeConfirm = true; }'
    ref="dialogRef"
  >
    <q-card 
      class='column items-center no-wrap' 
      style='min-width: 25%'
    >
      <q-card-section class="fit row items-center justify-center">
        <q-btn round>
          <q-avatar size="120px">
            <img src="https://cdn.quasar.dev/img/avatar2.jpg">
          </q-avatar>
        </q-btn>
      </q-card-section>
      <q-card-section class="fit row items-center justify-center">
        <span>Rating: {{ rating }}</span>
      </q-card-section>
      <q-card-section class="fit column q-pb-md">
        <q-input 
          v-model="displayName" 
          label="Display name" 
          stack-label
        />
        <q-input 
          v-model="login"
          label="Login" 
          stack-label
        />
        <q-select 
          v-model="selectedSchool" 
          :options="availableSchools" 
          label="School" 
          stack-label
        />
      </q-card-section>
      <q-card-section class="row full-width">
        <q-btn 
          label="Save and quit" 
          color="primary col-12"
          @click="activeConfirm = true"
        />
      </q-card-section>
    </q-card>
    <ConfirmProfilePopup
      v-model:active="activeConfirm"
      @back="backToProfile"
      @save="saveProfile"
      @forget="forgetProfileChanges"
    />
  </q-dialog>
</template>

<style scoped lang='sass'>

</style>