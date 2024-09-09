export interface ServiceProviderDto {
    id: string;
    companyName: string;
    email: string;
    userId: string;
  }
  
  export interface ServiceProviderResponseDto {
    message: string;
    data: ServiceProviderDto[];
  }
  