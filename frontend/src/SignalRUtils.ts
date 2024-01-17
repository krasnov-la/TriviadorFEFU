import { useAuthStore } from 'src/stores/auth';

import apiConfig from 'src/ApiConfig';

import * as signalR from '@microsoft/signalr';

const getAccessToken = async () => {
  const authStore = useAuthStore();
  await authStore.updateTokensByServer();
  const accessToken = authStore.getTokens.accessToken;
  if (accessToken == null) throw new Error('accessToken is null');
  return accessToken;
};

export const establishConnection = () => {
  return new signalR.HubConnectionBuilder()
    .withUrl(apiConfig.baseUrl + '/Game', {
      accessTokenFactory: getAccessToken,
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();
};

export const startConnection = (connection: signalR.HubConnection) => {
  const start = async () => {
    try {
      await connection.start();
      console.log('SignalR Connected.');
    } catch (err) {
      console.log(err);
      setTimeout(start, 5000);
    }
  };

  //TODO: solve multi-open connection problem

  // connection.onclose(async () => {
  //   await start();
  // });

  start();
};

export const stopConnection = (connection: signalR.HubConnection) => {
  connection.onclose(() => {
    console.log('OnClose method was emptied');
  });
  connection
    .stop()
    .then(() => {
      console.log('SignalR Stopped.');
    })
    .catch((reason) => {
      console.log(reason);
    });
};
