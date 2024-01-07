<script setup lang="ts">
import {ref, reactive, computed} from 'vue';
import { QTableProps } from 'quasar';

const state = reactive({
  rows: [
    {
    school : 'ИМКТ',
    score : '9999'
    },
    {
      school : 'ВИ-ШРМИ',
    score : '3000'
    },
    {
      school : 'ШЭМ',
    score : '400'
    },
    {
      school : 'ПИ',
    score : '5000'
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
  ]
})

const tColumns : QTableProps ={
  columns : [
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
]}

const sortedRows = computed(() => {
  return state.rows.slice().sort((
    (b, a) => parseFloat(a.score) - parseFloat(b.score)));
})
</script>

<template>
  <q-page class='row justify-around items-center'>
<!--    table-->
    <div style="height: 600px; width: 600px">
      <q-table style="width: 600px; height: 600px;  border-radius: 5%"
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
    <div class="column justify-center"
         style="height: 600px;
         width: 600px; border-radius: 5%; background-color: white">
      <div class="q-my-xs row justify-around" style="font-size: 20px">
        Players: {{state.groupList.length + 1}}/4
      </div>

<!--      Player profile-->
      <div class="col-3 q-ml-xs q-mr-xs">
        <div class="row" style="border:1px solid #7c7c7c; border-radius: 15px">
          <div class="col-3">
            <q-img style="width: 100px; border-radius: 100px; height: 100px" src="cat.jpg"/>
          </div>
          <a class="column justify-center" href="#"
             style="font-size: 25px; text-decoration: none; color: black">
            Profile
          </a>
        </div>
      </div>

<!--      Lobby members-->
      <div class="col q-ml-xs q-mr-xs">
        <div class="row q-py-xs q-mb-md" style="border:1px solid #7c7c7c; border-radius: 15px"
             v-for="member in state.groupList" :key="member">
          <div class="col-2 q-ml-xs">
            <q-img style="width: 50px; border-radius: 100px; height: 50px" src="cat.jpg"/>
          </div>
            <div class="column justify-center" href="#" style="font-size: 20px;">
              {{member.displayName}}
            </div>
<!--            <q-btn class="column justify-center q-ml-xl" size="10px" icon="delete"/>-->
        </div>
      </div>

      <div class="column col-4 justify-evenly">
        <div class="row justify-center q-mb-lg"
        >
            <q-btn style="width: 300px" size="25px" :disabled="state.groupList.length === 3">Invite</q-btn>
        </div>
        <div class="row justify-center">
            <q-btn style="width: 370px" size="27px">Find game</q-btn>
        </div>
      </div>
    </div>
  </q-page>
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
