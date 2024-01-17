<script setup lang="ts">
import { ref, reactive } from 'vue';
import { establishConnection, startConnection } from 'src/SignalRUtils';
import { useUserDataStore } from 'src/stores/user-data';
import { useGameStore, GamePhases } from 'src/stores/game-store';

import QuestionPopup from 'src/components/QuestionPopup.vue';

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

const userDataStore = useUserDataStore();
const gameStore = useGameStore();

const connection = establishConnection();
startConnection(connection);

const cells: { [val: string]: Element } = {};
const setCell = (element: Element, value: string) => {
  cells[value] = element;
};
const cellsStyle = [
  {
    top: 50,
    margin_right: 120,
  },
  {
    top: 80,
    margin_right: 340,
  },
  {
    top: 180,
    margin_right: 340,
  },
  {
    top: 250,
    margin_right: 150,
  },
  {
    top: 260,
    margin_right: 380,
  },
  {
    top: 360,
    margin_right: 380,
  },
  {
    top: 380,
    margin_right: 110,
  },
  {
    top: 290,
    margin_right: 0,
  },
  {
    top: 330,
    margin_right: -140,
  },
  {
    top: 385,
    margin_right: -93,
  },
  {
    top: 435,
    margin_right: -15,
  },
  {
    top: 430,
    margin_right: 290,
  },
  {
    top: 500,
    margin_right: 360,
  },
  {
    top: 470,
    margin_right: 140,
  },
  {
    top: 570,
    margin_right: 110,
  },
  {
    top: 510,
    margin_right: -60,
  },
  {
    top: 400,
    margin_right: -200,
  },
  {
    top: 350,
    margin_right: -370,
  },
  {
    top: 480,
    margin_right: -370,
  },
  {
    top: 550,
    margin_right: -200,
  },
];
const cellIsActive = reactive<{ [areaNum: string]: boolean }>({});
for (let i = 0; i < 20; i++) {
  cellIsActive[i] = false;
}

const myInitTurnStarted = () => {
  showAllAreas();
};

const myInitTurnEnded = () => {
  hideAllAreas();
};

const myExpandTurnStarted = () => {
  showMyAreas();
};

const myExpandTurnEnded = () => {
  hideAllAreas();
};

if (gameStore.playerTurnLogin == userDataStore.login) {
  myInitTurnStarted();
}

connection.on('StartTurnInit', (login) => {
  gameStore.playerTurnLogin = login;
  gameStore.gamePhase = GamePhases.Init;
  gameStore.playersAreas[login] = [];
  gameStore.updateLS();

  if (login == userDataStore.login) {
    myInitTurnStarted();
  } else {
    myInitTurnEnded();
  }

  console.log(`StartTurnInit: ${login}`);
});

connection.on('StartTurnExpand', (login) => {
  gameStore.playerTurnLogin = login;
  gameStore.gamePhase = GamePhases.Expand;
  gameStore.updateLS();

  if (login == userDataStore.login) {
    myExpandTurnStarted();
  } else {
    myExpandTurnEnded();
  }

  console.log(`StartTurnExpand: ${login}`);
});

connection.on('EndTurn', () => {
  console.log('EndTurn');
});

connection.on('Obtain', (login: string, areaId: number) => {
  gameStore.playersAreas[login].push(areaId);
  gameStore.updateLS();

  console.log('Obtain');
});

connection.on('WrongOrderMove', (expected, actual) => {
  console.log(`WrongOrderMove: ${expected} - ${actual}`);
});

connection.on('ExpandChoise', (login, areaId) => {
  console.log(`ExpandChoise: ${login} - ${areaId}`);
});

function showMyAreas() {
  if (userDataStore.login == null) throw new Error('User login is null');
  const myAreas = gameStore.playersAreas[userDataStore.login];
  for (let area in myAreas) {
    cellIsActive[area] = true;
  }
}

function showAllAreas() {
  for (let key in cellIsActive) {
    cellIsActive[key] = true;
    console.log(key);
    console.log(cellIsActive[key]);
  }
}

function hideAllAreas() {
  for (let key in cellIsActive) {
    cellIsActive[key] = false;
  }
}

function selectArea(areaId: string) {
  if (gameStore.playerTurnLogin != userDataStore.login) return;
  switch (gameStore.gamePhase) {
    case GamePhases.Init:
      connection.send('ChooseInit', Number(areaId));

      console.log(`selectArea - ChooseInit: ${areaId}`);
      break;
    case GamePhases.Expand:
      connection.send('ChooseExpand', Number(areaId));

      console.log(`selectArea - ChooseExpand: ${areaId}`);
      break;
  }
}
</script>

<template>
  <q-page class="row justify-around items-center">
    <q-img
      src="/public/game-map.png"
      width="80vh"
      style="position: absolute"
    ></q-img>
    <q-img
      v-for="i in 20"
      :key="i"
      src="/public/f_white.png"
      width="40px"
      :style="{
        opacity: cellIsActive[0] ? 1 : 0.45,
        position: 'absolute',
        top: `${cellsStyle[i - 1].top}px`,
        'margin-right': `${cellsStyle[i - 1].margin_right}px`,
      }"
      @click="selectArea(`${i}`)"
      :ref="
        (element) => {
          setCell(element as Element, `${i - 1}`);
        }
      "
    ></q-img>
  </q-page>
</template>

<!-- <template>
  <q-page class="row justify-around items-center">
    <q-card fit style="width: 80%; height: 600px" class="column">
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
            v-for="col in props.cols.filter((col) => col.name !== 'desc')"
            class="q-pa-xs col-xs-12 col-sm-6 col-md-4 col-lg-3 grid-style-transition"
            :key="col.name"
            @click="selectArea(col.value)"
            :ref="
              (element) => {
                setCell(element as Element, col.value);
              }
            "
            :style="{ color: cellIsActive[col.value] ? 'inherit' : 'red' }"
          >
            <q-card bordered flat>
              <q-list dense>
                <q-item>
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
</template> -->
