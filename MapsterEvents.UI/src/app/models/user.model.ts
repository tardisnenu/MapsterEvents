/**
 * Kullanıcı model arayüzü
 */
export interface User {
  id: number;
  fullName: string;
  email: string;
  createdAt: Date;
}

/**
 * Kullanıcı giriş model arayüzü
 */
export interface UserLogin {
  email: string;
  password: string;
}

/**
 * Kullanıcı kayıt model arayüzü
 */
export interface UserRegister {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

/**
 * Giriş yanıtı model arayüzü
 */
export interface LoginResponse {
  message: string;
  token: string;
  user: User;
}

/**
 * Kayıt yanıtı model arayüzü
 */
export interface RegisterResponse {
  message: string;
  user: User;
}