export interface JobTypeDto {
    id: string;
    name: string;
  }
  
  export interface JobTypeListResponseDto {
    message: string;
    data: JobTypeDto[];
  }
  