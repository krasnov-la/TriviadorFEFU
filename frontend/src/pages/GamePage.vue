<script setup lang="ts">
import { ref } from 'vue';
import { establishConnection, startConnection } from 'src/SignalRUtils';
import { useUserDataStore } from 'src/stores/user-data';

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

const connection = establishConnection();
startConnection(connection);

connection.on('StartTurnInit', (login) => {
  const myLogin = userDataStore.getUserData.login;
  if (myLogin == null) throw new Error('login = null');
  if (myLogin == login) myInitTurnStarted();
  else myInitTurnEnded();
});

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

const cells: { [val: string]: Element } = {};
const setCell = (element: Element, value: string) => {
  cells[value] = element;
};
const cellsIsActive = ref(false);

const myInitTurnStarted = () => {
  cellsIsActive.value = true;
};

const myInitTurnEnded = () => {
  cellsIsActive.value = true;
};
</script>

<template>
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
            @click="
              () => {
                console.log(typeof cells[col.value]);
                console.log(col.value);
              }
            "
            :ref="
              (element) => {
                setCell(element as Element, col.value);
              }
            "
            :style="{ color: cellsIsActive ? 'inherit' : 'red' }"
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
</template>
