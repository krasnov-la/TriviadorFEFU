<script setup lang="ts">
import {ref, reactive, computed} from 'vue';
import {establishConnection, startConnection} from 'src/SignalRUtils';
import {useUserDataStore} from 'src/stores/user-data';
import {useGameStore, GamePhases, PlayerColors} from 'src/stores/game-store';

import QuestionPopup from 'src/components/QuestionPopup.vue';
import {api} from 'src/boot/axios';
import {QTableProps} from "quasar";
import {displayName} from "../LocalStoragePaths";

const activeQuestion = ref(false);
const avatarSrcPlayer1 = ref('https://cdn.quasar.dev/img/avatar2.jpg');
const avatarSrcPlayer2 = ref('https://cdn.quasar.dev/img/avatar2.jpg');
const ratingPlayer1 = ref(123);
const ratingPlayer2 = ref(123);
const question = ref('123');
const ans1 = ref('123');
const ans2 = ref('123');
const ans3 = ref('123');
const ans4 = ref('123');
const correctOfAnswers = reactive<{ [ansNum: number]: boolean }>({});
const questionAnswered = ref<number | null>(null);

const userDataStore = useUserDataStore();
const gameStore = useGameStore();


const tColumns: QTableProps = {
  columns: [
    {
      name: 'displayName',
      align: 'center',
      label: 'Display name',
      field: 'displayName',
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


const state = reactive({
  players: [
    {
      displayName: 'bobik',
      score: '123',
      playerMove: false,
      color: 'red'
    },
    {
      displayName: 'clop',
      score: '100',
      playerMove: true,
      color: 'blue'
    },
    {
      displayName: 'paskuda',
      score: '140',
      playerMove: false,
      color: 'yellow'
    },
    {
      displayName: 'gena',
      score: '110',
      playerMove: false,
      color: 'green'
    },
  ]
});

const sortedPlayers = computed(() => {
  return state.players
    .slice()
    .sort((b, a) => parseFloat(a.score) - parseFloat(b.score));
});

const connection = establishConnection();
startConnection(connection);

const cells: { [val: string]: Element } = {};
const setCell = (element: Element, value: string) => {
  cells[value] = element;
};
const cellsStyle = [
  {
    top: 50, //1
    margin_left: 250,
  },
  {
    top: 80, //2
    margin_left: 100,
  },
  {
    top: 190, //3
    margin_left: 155,
  },
  {
    top: 290, //4
    margin_left: 130,
  },
  {
    top: 270,
    margin_left: 255,
  },
  {
    top: 300,
    margin_left: 330,
  },
  {
    top: 390,
    margin_left: 150,
  },
  {
    top: 400,
    margin_left: 270,
  },
  {
    top: 360,
    margin_left: 420,
  },
  {
    top: 420,
    margin_left: 390,
  },
  {
    top: 470,
    margin_left: 170,
  },
  {
    top: 480,
    margin_left: 350,
  },
  {
    top: 530,
    margin_left: 250,
  },
  {
    top: 570,
    margin_left: 130,
  },
  {
    top: 650,
    margin_left: 280,
  },
  {
    top: 580,
    margin_left: 380,
  },
  {
    top: 600,
    margin_left: 470,
  },
  {
    top: 450,
    margin_left: 450,
  },
  {
    top: 410,
    margin_left: 550,
  },
  {
    top: 530,
    margin_left: 570,
  },
];

const cellIsActive = reactive<{ [areaId: string]: boolean }>({});
const cellOccupied = reactive<{ [areaId: string]: string | null }>({});
for (let i = 0; i < 20; i++) {
  cellIsActive[i] = false;
  cellOccupied[i] = null;
}
let colorCounter = 1;

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
  gameStore.playersColors[login] = PlayerColors.Red + colorCounter++;
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
  activeQuestion.value = false;

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
  cellOccupied[areaId - 1] = login;
  activeQuestion.value = false;

  console.log(`Obtain: ${login} - ${areaId}`);
});

connection.on('WrongOrderMove', (expected, actual) => {
  console.log(`WrongOrderMove: ${expected} - ${actual}`);
});

connection.on('ExpandChoise', (login, areaId) => {
  console.log(`ExpandChoise: ${login} - ${areaId}`);
});

connection.on('AskQuestion', async (guid) => {
  getQuestionData(guid);
  activeQuestion.value = true;

  console.log(`AskQuestion: ${guid}`);
});

connection.on('ExpandChoisesDrop', () => {
  console.log('ExpandChoisesDrop');
});

interface IOption {
  id: string;
  text: string;
  correct: boolean;
  questionId: string;
}

interface IQuestion {
  id: string;
  text: string;
  options: IOption[];
}

function getQuestionData(guid: string) {
  api
    .get(`/Question/${guid}`)
    .then((response) => {
      const questionData = response.data as IQuestion;
      question.value = questionData.text;
      ans1.value = questionData.options[0].text;
      ans2.value = questionData.options[1].text;
      ans3.value = questionData.options[2].text;
      ans4.value = questionData.options[3].text;
      correctOfAnswers[0] = questionData.options[0].correct;
      correctOfAnswers[1] = questionData.options[1].correct;
      correctOfAnswers[2] = questionData.options[2].correct;
      correctOfAnswers[3] = questionData.options[3].correct;
    })
    .catch((reason) => {
      console.log(reason);
    });
}

function askQuestion(result: boolean) {
  connection
    .send('AnswerQuestion', Boolean(result))
    .then((val) => {
      console.log(val);
    })
    .catch((reason) => {
      console.log(reason);
    });
}

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

function returnFlag(areaId: number) {
  if (cellOccupied[areaId] == null) {
    return '/f_white.png';
  } else {
    switch (gameStore.playersColors[cellOccupied[areaId] as string]) {
      case PlayerColors.Red:
        return '/f_red.png';
      case PlayerColors.Blue:
        return '/f_blue.png';
      case PlayerColors.Yellow:
        return '/f_yellow.png';
      case PlayerColors.Green:
        return '/f_green.png';
    }
  }
}
</script>

<template>
  <q-page class="row justify-around items-center">

    <div>
      <q-table
        style="width: 400px; height: 300px; border-radius: 5%;"
        separator="horizontal"
        :columns="tColumns.columns"
        :rows="sortedPlayers"
        row-key="name"
        class="no-shadow row custom-table"
        :rows-per-page-options="[10]"
        bordered
        hide-pagination
      >
        <template v-slot:body-cell-displayName="props">
          <q-td :props="props">
            <div>
              <q-badge
                rounded
                :color=" props.row.color"
                :class="{ 'highlight-player': props.row.playerMove }"
                style="color: #121212; font-size: 20px; font-weight: bold"
                :label="(props.row.playerMove ? '-> ': '')"
              />
              {{ props.row.displayName }}
            </div>
          </q-td>
        </template>
      </q-table>
    </div>

    <div>
      <q-img
        src="/public/game-map.png"
        width="700px"
        style="user-focus: none ;user-select: none"
      >
        <q-img
          v-for="i in 20"
          :key="i"
          :src="returnFlag(i - 1)"
          width="40px"
          style="background: none"
          :style="{
            //opacity: cellIsActive[0] ? 1 : 0.6,
            position: 'absolute',
            top: `${cellsStyle[i - 1].top}px`,
            'margin-left': `${cellsStyle[i - 1].margin_left}px`,
            'user-select': 'none',
          }"
          @click="selectArea(`${i}`)"
          :ref="
            (element) => {
              setCell(element as Element, `${i - 1}`);
            }
          "
        />
      </q-img>
    </div>
    <QuestionPopup
      @answer="
        (ansId) => {
          askQuestion(correctOfAnswers[ansId]);
          console.log(ansId);
        }
      "
      v-model:active="activeQuestion"
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
<style lang="sass">
body
  background-color: #4481eb
  background-image: linear-gradient(to top, #4481eb 0%, #04befe 100%)

table
  margin: 0 auto
  border-collapse: collapse
  table-layout: fixed
  width: 300px
  height: 270px

th, td
  padding: 8px
  text-align: left
  border-bottom: 1px solid #ddd

th
  background-color: #f2f2f2
  color: #004080

tr
  &:hover
    background-color: #f5f5f5
    color: black

.custom
  border: 2px solid black
  border-radius: 15px
  overflow: hidden
  background-color: white

.q-table thead th
  font-size: 25px

.q-table tbody td
  font-size: 25px

</style>
