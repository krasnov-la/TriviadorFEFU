import { LocalStorage } from 'quasar';
import { defineStore } from 'pinia';

import * as LSPath from 'src/LocalStoragePaths';

const areasCount = 20;

export enum GamePhases {
  Init,
  Expand,
}

export enum PlayerColors {
  Red,
  Blue,
  Yellow,
  Green,
}

interface IArea {
  ownerLogin: string | null;
  isActive: boolean;
  shaded: boolean;
}

interface IGameStore {
  inGame: boolean;
  gamePhase: GamePhases;
  playerTurnLogin: string | null;
  playersAreas: { [login: string]: Array<string> };
  playersColors: { [login: string]: PlayerColors | null };
  areas: { [areaId: string]: IArea };
  gameId: string | null;
  expandChoiseAreaIds: string[];
  gameScore: { [login: string]: string };
}

function GetFromLS(): IGameStore {
  const inGame = LocalStorage.getItem(LSPath.inGame);
  const gamePhase = LocalStorage.getItem(LSPath.gamePhase);
  const playerTurnLogin = LocalStorage.getItem(LSPath.playerTurnLogin);
  const playersAreas = LocalStorage.getItem(LSPath.playersAreas);
  const playersColors = LocalStorage.getItem(LSPath.playersColors);
  const areas = LocalStorage.getItem(LSPath.areas);
  const gameId = LocalStorage.getItem(LSPath.gameId);
  const expandChoiseAreaIds = LocalStorage.getItem(LSPath.expandChoiseAreaIds);
  const gameScore = LocalStorage.getItem(LSPath.gameScore);

  if (
    inGame == null ||
    gamePhase == null ||
    playerTurnLogin == null ||
    playersAreas == null ||
    playersColors == null ||
    areas == null ||
    gameId == null ||
    expandChoiseAreaIds == null ||
    gameScore == null
  ) {
    const obj: IGameStore = {
      inGame: false,
      gamePhase: GamePhases.Init,
      playerTurnLogin: null,
      playersAreas: {},
      playersColors: {},
      areas: {},
      gameId: null,
      expandChoiseAreaIds: [],
      gameScore: {},
    };

    for (let i = 0; i < areasCount; i++) {
      obj.areas[i.toString()] = {
        ownerLogin: null,
        isActive: false,
        shaded: false,
      };
    }

    UpdateLS(obj);
  }

  return {
    inGame: inGame?.valueOf() as boolean,
    gamePhase: gamePhase?.valueOf() as GamePhases,
    playerTurnLogin: playerTurnLogin?.valueOf() as string,
    playersAreas: playersAreas?.valueOf() as { [login: string]: Array<string> },
    playersColors: playersColors?.valueOf() as {
      [login: string]: PlayerColors;
    },
    areas: areas?.valueOf() as { [areaId: string]: IArea },
    gameId: gameId?.valueOf() as string,
    expandChoiseAreaIds: expandChoiseAreaIds?.valueOf() as string[],
    gameScore: gameScore?.valueOf() as { [login: string]: string },
  };
}

function UpdateLS(gameStore: IGameStore): void {
  LocalStorage.set(LSPath.inGame, gameStore.inGame);
  LocalStorage.set(LSPath.gamePhase, gameStore.gamePhase);
  LocalStorage.set(LSPath.playerTurnLogin, gameStore.playerTurnLogin);
  LocalStorage.set(LSPath.playersAreas, gameStore.playersAreas);
  LocalStorage.set(LSPath.playersColors, gameStore.playersColors);
  LocalStorage.set(LSPath.areas, gameStore.areas);
  LocalStorage.set(LSPath.gameId, gameStore.gameId);
  LocalStorage.set(LSPath.expandChoiseAreaIds, gameStore.expandChoiseAreaIds);
  LocalStorage.set(LSPath.gameScore, gameStore.gameScore);
}

export const useGameStore = defineStore('game-store', {
  state: GetFromLS,

  getters: {},

  actions: {
    updateLS() {
      UpdateLS(this.$state);
    },

    setRandColor(login: string) {
      const colorArr = [
        PlayerColors.Red,
        PlayerColors.Blue,
        PlayerColors.Yellow,
        PlayerColors.Green,
      ];
      const occupiedColors = Object.values(this.playersColors);
      const difference = colorArr.filter(
        (color) => !occupiedColors.includes(color)
      );

      const getRandomInt = (max: number) => {
        return Math.floor(Math.random() * max);
      };

      const chosenColor = difference[getRandomInt(difference.length)];

      this.setPlayerColor(login, chosenColor);
    },

    setPlayerColor(login: string, color: PlayerColors) {
      this.playersColors[login] = color;

      UpdateLS(this.$state);
    },

    setPlayerArea(login: string, areaId: string) {
      this.playersAreas[login].push(areaId);
      this.areas[areaId].ownerLogin = login;

      UpdateLS(this.$state);
    },

    setAreaActivity(activity: boolean, areaId: string) {
      this.areas[areaId].isActive = activity;

      UpdateLS(this.$state);
    },

    initPlayer(login: string) {
      this.playersAreas[login] = [];
      this.playersColors[login] = null;
    },

    clear(): void {
      this.inGame = false;
      this.gamePhase = GamePhases.Init;
      this.playerTurnLogin = null;
      this.playersAreas = {};
      this.playersColors = {};
      this.areas = {};
      for (let i = 0; i < areasCount; i++) {
        this.areas[i] = {
          ownerLogin: null,
          isActive: false,
          shaded: false,
        };
      }
      this.gameId = null;
      this.expandChoiseAreaIds = [];
      this.gameScore = {};

      UpdateLS(this.$state);
    },

    clearExpandChoises(): void {
      this.expandChoiseAreaIds = [];

      UpdateLS(this.$state);
    },
  },
});
