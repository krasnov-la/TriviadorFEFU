<script setup lang="ts">
import { ref, reactive } from 'vue';
import { establishConnection, startConnection } from 'src/SignalRUtils';
import { useUserDataStore } from 'src/stores/user-data';
import { useGameStore, GamePhases, PlayerColors } from 'src/stores/game-store';

import QuestionPopup from 'src/components/QuestionPopup.vue';
import { api } from 'src/boot/axios';

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
  connection.send(
    'AnswerQuestion',
    String(gameStore.gameId),
    String(userDataStore.login),
    Boolean(result)
  );
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
    <q-img
      src="/public/game-map.png"
      width="80vh"
      style="position: absolute"
    ></q-img>
    <q-img
      v-for="i in 20"
      :key="i"
      :src="returnFlag(i - 1)"
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
