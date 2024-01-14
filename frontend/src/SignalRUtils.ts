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

  connection.onclose(async () => {
    await start();
  });

  start();
};
