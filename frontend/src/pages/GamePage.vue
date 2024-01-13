<template>
  <q-page class="row justify-around items-center">
    <q-card @click="send" fit style="width: 80%; height: 600px" class="column">
      <q-table
        flat
        bordered
        grid
        title="KARTA DVFU"
        :rows="rows"
        :columns="columns"
        row-key="name"
        hide-header
      >
        <template v-slot:item="props">
          <div
            class="q-pa-xs col-xs-12 col-sm-6 col-md-4 col-lg-3 grid-style-transition"
          >
            <q-card
              bordered
              flat
              @click="
                () => {
                  console.log(123);
                }
              "
            >
              <q-list dense>
                <q-item
                  v-for="col in props.cols.filter((col) => col.name !== 'desc')"
                  :key="col.name"
                >
                  <q-item-section>
                    <q-item-label>{{ col.label }}</q-item-label>
                  </q-item-section>
                  <q-item-section side>
                    <q-item-label caption>{{ col.value }}</q-item-label>
                  </q-item-section>
                </q-item>
              </q-list>
            </q-card>
          </div>
        </template>
      </q-table>
    </q-card>
    <QuestionPopup
      v-model:active="active"
      :avatarSrcPlayer1="avatarSrcPlayer1"
      :avatarSrcPlayer2="avatarSrcPlayer2"
      :ratingPlayer1="ratingPlayer1"
      :ratingPlayer2="ratingPlayer2"
      :question="question"
      :ans1="ans1"
      :ans2="ans2"
      :ans3="ans3"
      :ans4="ans4"
    />
  </q-page>
</template>

<script setup lang="ts">
import * as signalR from '@microsoft/signalr';
import QuestionPopup from 'src/components/QuestionPopup.vue';
import { ref } from 'vue';
import apiConfig from 'src/ApiConfig';
import { useAuthStore } from 'src/stores/auth';

const getAccessToken = async () => {
  await authStore.updateTokensByServer();
  const accessToken = authStore.getTokens.accessToken;
  if (accessToken == null) throw new Error('accessToken is null');
  return accessToken;
};

const connection = new signalR.HubConnectionBuilder()
  .withUrl(apiConfig.baseUrl + '/Game', {
    accessTokenFactory: getAccessToken,
  })
  .configureLogging(signalR.LogLevel.Information)
  .build();

const authStore = useAuthStore();

const active = ref(false);
const avatarSrcPlayer1 = ref('https://cdn.quasar.dev/img/avatar2.jpg');
const avatarSrcPlayer2 = ref('https://cdn.quasar.dev/img/avatar2.jpg');
const ratingPlayer1 = ref(123);
const ratingPlayer2 = ref(123);
const question = ref('123');
const ans1 = ref('123');
const ans2 = ref('123');
const ans3 = ref('123');
const ans4 = ref('123');

async function start() {
  try {
    await connection.start();
    console.log('SignalR Connected.');
  } catch (err) {
    console.log(err);
    setTimeout(start, 5000);
  }
}

connection.onclose(async () => {
  await start();
});

connection.on('receiveMethod', (msg) => console.log(msg));

// Start the connection.
start();

function send() {
  connection.send('Echo', 'message');
}

const columns = [
  {
    name: 'number',
    label: 'Number',
    field: 'number',
  },
];

const mapPartsCount = 20;
const rows = Array.from(Array(mapPartsCount).keys()).map((val) => ({
  number: val,
}));
</script>
