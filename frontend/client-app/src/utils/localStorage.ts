import axios from 'axios';

const TOKEN_KEY = 'auth_token';
const USER_ROLE_KEY = 'user_role';

export const setToken = (token: string): void => {
  localStorage.setItem(TOKEN_KEY, token);
};

export const getToken = (): string | null => {
  return localStorage.getItem(TOKEN_KEY);
};

export const removeToken = (): void => {
  localStorage.removeItem(TOKEN_KEY);
};

export const setUserRole = (role: string): void => {
  localStorage.setItem(USER_ROLE_KEY, role);
};

export const getUserRole = (): string | null => {
  return localStorage.getItem(USER_ROLE_KEY);
};

export const removeUserRole = (): void => {
  localStorage.removeItem(USER_ROLE_KEY);
};

export const setAuthToken = (token: string | null): void => {
  if (token) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete axios.defaults.headers.common['Authorization'];
  }
};

// Инициализация токена при импорте модуля
const savedToken = getToken();
if (savedToken) {
  setAuthToken(savedToken);
}
