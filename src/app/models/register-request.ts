export interface RegisterRequest {
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  companyName?: string;
  address: string;
  password: string;
  confirmPassword?: string; 
  }