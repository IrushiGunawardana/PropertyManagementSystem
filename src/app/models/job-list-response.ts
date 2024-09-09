export interface JobDto {
    id: string;
    jobNumber: number;
    description: string;
    postedDate: string;
  }
  
  export interface JobsListResponseDto {
    message: string;
    data: JobDto[];
  }