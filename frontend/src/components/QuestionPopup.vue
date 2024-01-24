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
  dis: boolean;
}>();

const emit = defineEmits<{
  (e: 'answer', ansId: number): void;
}>();
</script>

<template>
  <q-dialog :model-value="active" persistent>
    <q-card class="column justify-center" style="width: 700px; height: 400px">
      <q-card-section
        class="row text-center justify-center row-5"
        style="font-size: 20px"
      >
        <p style="width: 500px; word-wrap: break-word; white-space: pre-wrap">
          {{ question }}
        </p>
      </q-card-section>
      <q-card-section class="row text-center justify-center row-5">
        <div class="column">
          <div class="row col-5">
            <q-btn
              color="primary q-my-sm q-mr-sm"
              style="width: 200px"
              @click="emit('answer', 1)"
              :disable="dis"
            >
              {{ ans1 }}
            </q-btn>
            <q-btn
              color="primary q-my-sm"
              style="width: 200px"
              @click="emit('answer', 3)"
              :disable="dis"
            >
              {{ ans3 }}
            </q-btn>
          </div>
          <div class="row col-5">
            <q-btn
              color="primary q-my-sm q-mr-sm"
              style="width: 200px"
              @click="emit('answer', 2)"
              :disable="dis"
            >
              {{ ans2 }}
            </q-btn>
            <q-btn
              color="primary q-my-sm"
              style="width: 200px"
              @click="emit('answer', 4)"
              :disable="dis"
            >
              {{ ans4 }}
            </q-btn>
          </div>
        </div>
      </q-card-section>
    </q-card>
  </q-dialog>
</template>
