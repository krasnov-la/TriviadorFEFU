import { LocalStorage } from 'quasar';
import { defineStore } from 'pinia';

import axios from 'axios';
import apiConfig from 'src/ApiConfig';

import * as LSPath from 'src/LocalStoragePaths';

interface ITokenList {
  accessToken: string | null;
  refreshToken: string | null;
}

const updateLocalStorage = () => {
  const tokens = useAuthStore().getTokens;

  LocalStorage.set(LSPath.accessToken, tokens.accessToken);
  LocalStorage.set(LSPath.refreshToken, tokens.refreshToken);
};

const getFromLocalStorage = () => {
  const accessTokenItem = LocalStorage.getItem(LSPath.accessToken);
  const refreshTokenItem = LocalStorage.getItem(LSPath.refreshToken);

  if (accessTokenItem == null || refreshTokenItem == null) {
    return {
      accessToken: null,
      refreshToken: null,
    } as ITokenList;
  }

  return {
    accessToken: accessTokenItem.toString(),
    refreshToken: refreshTokenItem.toString(),
  } as ITokenList;
};

export const useAuthStore = defineStore('auth', {
  state: getFromLocalStorage,

  actions: {
    updateTokensByServer(): Promise<ITokenList> {
      return new Promise((resolve, reject) => {
        axios
          .post(apiConfig.baseUrl + '/Auth/Refresh', {
            accessToken: this.accessToken,
            refreshToken: this.refreshToken,
          })
          .then((response) => {
            if (response.status == 200) {
              const data = response.data;
              this.accessToken = data.accessToken;
              this.refreshToken = data.refreshToken;
              updateLocalStorage();
              resolve({
                accessToken: this.accessToken,
                refreshToken: this.refreshToken,
              });
            } else {
              console.log(response);
              reject(response);
            }
          })
          .catch((reason) => {
            console.log(reason);
            reject(reason);
          });
      });
    },

    updateTokensManually(tokens: ITokenList): void {
      this.accessToken = tokens.accessToken;
      this.refreshToken = tokens.refreshToken;
      //updateLocalStorage(); - nado ili net?
    },

    clear(): void {
      this.accessToken = null;
      this.refreshToken = null;
      updateLocalStorage();
    },
  },

  getters: {
    getTokens(): ITokenList {
      return {
        accessToken: this.accessToken,
        refreshToken: this.refreshToken,
      };
    },

    isTokens(): boolean {
      return this.accessToken == null || this.refreshToken == null;
    },
  },
});
