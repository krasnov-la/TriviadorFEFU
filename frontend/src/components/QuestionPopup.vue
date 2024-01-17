<script setup lang="ts">
import { ref } from 'vue';

const props = defineProps<{
  active: boolean;
  avatarSrcPlayer1: string;
  avatarSrcPlayer2: string;
  ratingPlayer1: number;
  ratingPlayer2: number;
  question: string;
  ans1: string;
  ans2: string;
  ans3: string;
  ans4: string;
}>();

const emit = defineEmits<{
  (e: 'answer', ansId: number): void;
}>();

const timerCounter = ref<number>(5);
</script>

<template>
  <q-dialog :model-value="active" persistent>
    <q-card class="column justify-center" style="width: 700px; height: 400px">
      <q-card-section class="row text-center justify-center items-center row-1">
        <span class="col-3">
          <q-avatar style="height: 50px; width: 50px">
            <img :src="avatarSrcPlayer1" />
          </q-avatar>
        </span>
        <span class="col-6">{{ ratingPlayer1 }} / {{ ratingPlayer2 }}</span>
        <span class="col-3">
          <q-avatar style="height: 50px; width: 50px">
            <img :src="avatarSrcPlayer2" />
          </q-avatar>
        </span>
      </q-card-section>
      <q-card-section class="row text-center justify-center row-1">
        <span>{{ timerCounter }}</span>
      </q-card-section>
      <q-card-section class="row text-center justify-center row-2">
        <span>{{ question }}</span>
      </q-card-section>
      <q-card-section class="row text-center justify-center row-6">
        <div class="column">
          <div class="row col-5">
            <q-btn
              color="primary q-my-sm q-mr-sm"
              style="width: 200px"
              @click="emit('answer', 1)"
            >
              {{ ans1 }}
            </q-btn>
            <q-btn
              color="primary q-my-sm"
              style="width: 200px"
              @click="emit('answer', 3)"
            >
              {{ ans3 }}
            </q-btn>
          </div>
          <div class="row col-5">
            <q-btn
              color="primary q-my-sm q-mr-sm"
              style="width: 200px"
              @click="emit('answer', 2)"
            >
              {{ ans2 }}
            </q-btn>
            <q-btn
              color="primary q-my-sm"
              style="width: 200px"
              @click="emit('answer', 4)"
            >
              {{ ans4 }}
            </q-btn>
          </div>
        </div>
      </q-card-section>
    </q-card>
  </q-dialog>
</template>
