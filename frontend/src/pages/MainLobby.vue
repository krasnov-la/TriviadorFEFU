<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { QTableProps } from 'quasar';
import ProfilePopup from 'src/components/ProfilePopup.vue';
import * as signalR from '@microsoft/signalr';
import apiConfig from 'src/ApiConfig';
import { useAuthStore } from 'src/stores/auth';

const state = reactive({
  rows: [
    {
      school: 'ИМКТ',
      score: '9999',
    },
    {
      school: 'ВИ-ШРМИ',
      score: '3000',
    },
    {
      school: 'ШЭМ',
      score: '400',
    },
    {
      school: 'ПИ',
      score: '5000',
    },
  ],
  groupList: [
    {
      displayName: 'Name1',
      img: '',
    },
    {
      displayName: 'Name2',
      img: '',
    },
  ],
});

const tColumns: QTableProps = {
  columns: [
    {
      name: 'school',
      align: 'center',
      label: 'School',
      field: 'school',
      //headerStyle: headerStyle
    },
    {
      name: 'score',
      align: 'center',
      label: 'Score',
      field: 'score',
      //headerStyle: headerStyle
    },
  ],
};

const authStore = useAuthStore();

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

start();

connection.on('LobbyTerminated', () => {
  hostRegimeActive.value = false;
});

connection.on('JoinLobby', (logins) => {
  hostRegimeActive.value = true;
  joiningToGameLoadBarActive.value = false;
  inputIdActive.value = false;
  console.log('JoinLobby');
});

connection.on('LobbyNotFound', () => {
  joiningToGameLoadBarActive.value = false;
});

const sortedRows = computed(() => {
  return state.rows
    .slice()
    .sort((b, a) => parseFloat(a.score) - parseFloat(b.score));
});

const createLobby = () => {
  connection.send('CreateLobby');
  console.log('CreateLobby');
};

const joinLobby = () => {
  if (gameIdToConnect.value == null)
    throw new Error('gameIdToConnect.value is null');
  connection.send('JoinLobby', gameIdToConnect.value);
};

const profileActive = ref(false);
const hostRegimeActive = ref(false);
const inputIdActive = ref(false);
const joiningToGameLoadBarActive = ref(false);

const gameIdToConnect = ref<string | null>(null);

const showProfile = () => {
  profileActive.value = true;
};

const closeProfile = () => {
  profileActive.value = false;
};

const createGame = () => {
  hostRegimeActive.value = true;
};

const endGame = () => {
  hostRegimeActive.value = false;
};
</script>

<template>
  <q-page class="row justify-around items-center">
    <!--Table-->
    <div style="height: 600px; width: 600px">
      <q-table
        style="width: 600px; height: 600px; border-radius: 5%"
        separator="horizontal"
        :columns="tColumns.columns"
        :rows="sortedRows"
        row-key="name"
        class="no-shadow row"
        :rows-per-page-options="[10]"
        bordered
      />
    </div>
    <!--Lobby-->
    <div
      class="column justify-center"
      style="
        height: 600px;
        width: 600px;
        border-radius: 5%;
        background-color: white;
      "
    >
      <div
        class="q-my-xs row justify-around"
        style="font-size: 20px"
        :class="{ invisible: !hostRegimeActive }"
      >
        Players: {{ state.groupList.length + 1 }}/4
      </div>
      <!--Player profile-->
      <div class="col-3 q-ml-xs q-mr-xs" @click="showProfile">
        <div class="row" style="border: 1px solid #7c7c7c; border-radius: 15px">
          <div class="col-3">
            <q-img
              style="width: 100px; border-radius: 100px; height: 100px"
              src="cat.jpg"
            />
          </div>
          <span
            class="column justify-center"
            style="font-size: 25px; text-decoration: none; color: black"
          >
            Profile
          </span>
        </div>
      </div>
      <!--Lobby members-->
      <div
        class="col q-ml-xs q-mr-xs"
        :class="{ invisible: !hostRegimeActive }"
      >
        <div
          class="row q-py-xs q-mb-md"
          style="border: 1px solid #7c7c7c; border-radius: 15px"
          v-for="member in state.groupList"
          :key="member"
        >
          <div class="col-2 q-ml-xs">
            <q-img
              style="width: 50px; border-radius: 100px; height: 50px"
              src="cat.jpg"
            />
          </div>
          <div class="column justify-center" href="#" style="font-size: 20px">
            {{ member.displayName }}
          </div>
          <!--<q-btn class="column justify-center q-ml-xl" size="10px" icon="delete"/>-->
        </div>
      </div>
      <div
        class="row col-4 justify-evenly"
        :class="{ hidden: hostRegimeActive }"
      >
        <q-btn
          class="col-4"
          style="max-height: 150px"
          size="25px"
          color="primary"
          @click="createLobby"
        >
          Create game
        </q-btn>
        <q-btn
          class="col-4"
          style="max-height: 150px"
          size="25px"
          color="primary"
          @click="
            () => {
              inputIdActive = true;
            }
          "
        >
          Join game
        </q-btn>
      </div>
      <div
        class="column col-4 justify-evenly"
        :class="{ hidden: !hostRegimeActive }"
      >
        <div class="row row-4 justify-center">
          <q-input
            class="column col-5 q-pr-sm"
            v-model="gameIdToConnect"
            readonly
            label="Game ID"
            stack-label
          />
          <q-btn
            class="column col-5 q-pl-sm"
            color="primary"
            @click="endGame"
            disable
          >
            Leave room
          </q-btn>
        </div>
      </div>
    </div>
  </q-page>
  <ProfilePopup v-model:active="profileActive" @close="closeProfile" />
  <q-dialog
    :model-value="inputIdActive"
    @hide="
      () => {
        inputIdActive = false;
      }
    "
  >
    <q-card class="q-px-sm q-py-sm">
      <div class="row">
        <q-input class="q-pr-sm" v-model="gameIdToConnect" label="Game ID" />
        <q-btn label="Join" color="primary" @click="joinLobby" />
      </div>
    </q-card>
  </q-dialog>
  <q-dialog :model-value="joiningToGameLoadBarActive" persistent>
    <q-card
      style="height: 80px; width: 80px"
      class="row items-center justify-center"
    >
      <q-circular-progress
        indeterminate
        rounded
        style="height: 50px; width: 50px"
        color="primary"
      />
    </q-card>
  </q-dialog>
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
