import { api } from 'boot/axios';
import { LocalStorage } from 'quasar';
import { Router } from 'src/router';

export const exit = () => {
  LocalStorage.remove('accessToken');
  LocalStorage.remove('refreshToken');
  LocalStorage.remove('email');

  Router.push('/');
};

export const getNewTokens = async () => {
  if (!LocalStorage.has('accessToken') || !LocalStorage.has('refreshToken')) {
    exit();

    console.log('Not authorized, redirecting to login page')
  }
  return await api
    .post('/Auth/Refresh', {
      accessToken: LocalStorage.getItem('accessToken'),
      refreshToken: LocalStorage.getItem('refreshToken')
    })
    .then((response) => {
      const data = response.data;

      LocalStorage.set('accessToken', data.data.accessToken);
      LocalStorage.set('refreshToken', data.data.refreshToken);
    })
    .catch((error) => {
      if (error.response.status === 401) {
        exit();
        console.log('Not authorized, redirecting to login page')
      }
    });
};
export const authPost = async (
  url: string,
  data: object,
  headers: object = {}
) => {
  const config = {
    headers: {
      Authorization: 'Bearer ' + LocalStorage.getItem('accessToken'),
      ...headers
    }
  };
  return api.post(url, data, config)
    .catch(async (error) => {
      if (error.response.status === 401) {
        await getNewTokens();
        return await api.post(url, data, config);
      } else throw `Unhandled error ${error}`;
    });
};
export const authGet = async (url: string, headers: object = {}) => {
  const config = {
    headers: {
      Authorization: 'Bearer ' + LocalStorage.getItem('accessToken'),
      ...headers
    }
  };
  return api.get(url, config)
    .catch(async (error) => {
      if (error.response.status === 401) {
        await getNewTokens();
        return await api.get(url, config);
      } else
        throw `Unhandled error ${error}`;
    });
};
