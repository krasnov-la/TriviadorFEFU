import { LocalStorage } from 'quasar';
import { defineStore } from 'pinia';

import * as LSPath from 'src/LocalStoragePaths';

export enum GamePhases {
  Init,
  Expand,
}

interface IGameStore {
  inGame: boolean;
  gamePhase: GamePhases;
  playerTurnLogin: string | null;
  playersAreas: { [login: string]: Array<number> };
}

function GetFromLS(): IGameStore {
  const inGame = LocalStorage.getItem(LSPath.inGame);
  const gamePhase = LocalStorage.getItem(LSPath.gamePhase);
  const playerTurnLogin = LocalStorage.getItem(LSPath.playerTurnLogin);
  const playersAreas = LocalStorage.getItem(LSPath.playersAreas);

  if (
    inGame == null ||
    gamePhase == null ||
    playerTurnLogin == null ||
    playersAreas == null
  ) {
    UpdateLS({
      inGame: false,
      gamePhase: GamePhases.Init,
      playerTurnLogin: null,
      playersAreas: {},
    });
  }

  return {
    inGame: inGame?.valueOf() as boolean,
    gamePhase: gamePhase?.valueOf() as GamePhases,
    playerTurnLogin: playerTurnLogin?.valueOf() as string,
    playersAreas: playersAreas?.valueOf() as { [login: string]: Array<number> },
  };
}

function UpdateLS(gameStore: IGameStore): void {
  LocalStorage.set(LSPath.inGame, gameStore.inGame);
  LocalStorage.set(LSPath.gamePhase, gameStore.gamePhase);
  LocalStorage.set(LSPath.playerTurnLogin, gameStore.playerTurnLogin);
  LocalStorage.set(LSPath.playersAreas, gameStore.playersAreas);
}

export const useGameStore = defineStore('game-store', {
  state: GetFromLS,

  getters: {},

  actions: {
    updateLS() {
      UpdateLS(this.$state);
    },
  },
});
