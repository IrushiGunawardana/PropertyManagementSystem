export interface OwnerDetailsDto {
    userId: string;
    firstName: string;
    lastName: string;
  }
  
  export interface TenantDetailsDto {
    userId: string;
    firstName: string;
    lastName: string;
  }
  
  export interface PropertyDto {
    id: string;
    address: string;
    ownersDetails: OwnerDetailsDto[];
    tenantsDetails: TenantDetailsDto[];
  }
  
  export interface PropertyListResponseDto {
    message: string;
    data: PropertyDto[];
  }
  