export interface CreateJobResponseDto {
    message: string;
    data: {
      id: string;
      serviceProviderId: string;
      jobNumber: number;
      name: string;
      description: string;
    };
  }
  