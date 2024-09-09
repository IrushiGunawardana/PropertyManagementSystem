export interface JobDetailsResponseDto {
    message: string;
    data: JobDetailsDto;
  }
  
  export interface JobDetailsDto {
    id: string;
    property: PropertyDto;
    ownerDetails: OwnerDetailsDto[];
    tenantDetails: TenantDetailsDto[];
    description: string;
    jobNumber: number;
    jobType: JobTypeDto;
    provider: ProviderDto;
  }
  
  export interface PropertyDto {
    id: string;
    address: string;
  }
  
  export interface OwnerDetailsDto {
    id: string;
    userId: string;
    firstName: string;
    lastName: string;
  }
  
  export interface TenantDetailsDto {
    id: string;
    userId: string;
    firstName: string;
    lastName: string;
  }
  
  export interface JobTypeDto {
    id: string;
    name: string;
  }
  
  export interface ProviderDto {
    id: string;
    userId: string;
    companyName: string;
    email: string;
  }
  