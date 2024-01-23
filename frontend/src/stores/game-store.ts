import { LocalStorage } from 'quasar';
import { defineStore } from 'pinia';

import * as LSPath from 'src/LocalStoragePaths';

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

interface IGameStore {
  inGame: boolean;
  gamePhase: GamePhases;
  playerTurnLogin: string | null;
  playersAreas: { [login: string]: Array<number> };
  playersColors: { [login: string]: PlayerColors };
  gameId: string | null;
}

function GetFromLS(): IGameStore {
  const inGame = LocalStorage.getItem(LSPath.inGame);
  const gamePhase = LocalStorage.getItem(LSPath.gamePhase);
  const playerTurnLogin = LocalStorage.getItem(LSPath.playerTurnLogin);
  const playersAreas = LocalStorage.getItem(LSPath.playersAreas);
  const playersColors = LocalStorage.getItem(LSPath.playersColors);
  const gameId = LocalStorage.getItem(LSPath.gameId);

  if (
    inGame == null ||
    gamePhase == null ||
    playerTurnLogin == null ||
    playersAreas == null ||
    playersColors == null ||
    gameId == null
  ) {
    UpdateLS({
      inGame: false,
      gamePhase: GamePhases.Init,
      playerTurnLogin: null,
      playersAreas: {},
      playersColors: {},
      gameId: null,
    });
  }

  return {
    inGame: inGame?.valueOf() as boolean,
    gamePhase: gamePhase?.valueOf() as GamePhases,
    playerTurnLogin: playerTurnLogin?.valueOf() as string,
    playersAreas: playersAreas?.valueOf() as { [login: string]: Array<number> },
    playersColors: playersColors?.valueOf() as {
      [login: string]: PlayerColors;
    },
    gameId: gameId?.valueOf() as string,
  };
}

function UpdateLS(gameStore: IGameStore): void {
  LocalStorage.set(LSPath.inGame, gameStore.inGame);
  LocalStorage.set(LSPath.gamePhase, gameStore.gamePhase);
  LocalStorage.set(LSPath.playerTurnLogin, gameStore.playerTurnLogin);
  LocalStorage.set(LSPath.playersAreas, gameStore.playersAreas);
  LocalStorage.set(LSPath.playersColors, gameStore.playersColors);
  LocalStorage.set(LSPath.gameId, gameStore.gameId);
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

      this.playersColors[login] = chosenColor;

      UpdateLS(this.$state);
    },

    clear(): void {
      this.inGame = false;
      this.gamePhase = GamePhases.Init;
      this.playerTurnLogin = null;
      this.playersAreas = {};
      this.playersColors = {};
      this.gameId = null;
      UpdateLS(this.$state);
    },
  },
});
