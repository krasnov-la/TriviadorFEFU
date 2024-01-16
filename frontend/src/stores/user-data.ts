import { defineStore } from 'pinia';
import { LocalStorage } from 'quasar';
import { api } from 'src/boot/axios';

import * as LSPath from 'src/LocalStoragePaths';

interface IUserData {
  login: string | null;
  displayName: string | null;
  school: string | null;
}

const updateLocalStorage = () => {
  const userData = useUserDataStore().getUserData;

  LocalStorage.set(LSPath.login, userData.login);
  LocalStorage.set(LSPath.displayName, userData.displayName);
  LocalStorage.set(LSPath.school, userData.school);
};

const getFromLocalStorage = () => {
  const loginItem = LocalStorage.getItem(LSPath.login);
  const displayNameItem = LocalStorage.getItem(LSPath.displayName);
  const schoolItem = LocalStorage.getItem(LSPath.school);

  if (loginItem == null || displayNameItem == null || schoolItem == null) {
    return {
      login: null,
      displayName: null,
      school: null,
    } as IUserData;
  }

  return {
    login: loginItem.toString(),
    displayName: displayNameItem.toString(),
    school: schoolItem.toString(),
  } as IUserData;
};

export const useUserDataStore = defineStore('user-data', {
  state: getFromLocalStorage,

  getters: {
    getUserData(): IUserData {
      return {
        login: this.login,
        displayName: this.displayName,
        school: this.school,
      };
    },
  },

  actions: {
    updateUserDataOnServer(data: IUserData): void {
      this.updateUserDataLocally(data);
      // na server otpravit
    },

    updateUserDataLocally(data: IUserData): void {
      this.login = data.login;
      this.displayName = data.displayName;
      this.school = data.school;
      updateLocalStorage();
    },

    getUserDataFromServer(): void {
      api
        .get('/UserInfo/GetMyself')
        .then((response) => {
          this.login = response.data.login;
          this.displayName = response.data.displayName;
          this.school = response.data.school;
          updateLocalStorage();
        })
        .catch((error) => {
          console.log(error);
        });
    },
  },
});
