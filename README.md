# Property Maintenance Management Application

## Project Overview

This web application is designed to manage property maintenance jobs, enabling users to create, view, and manage maintenance jobs for existing properties. The application is built using Angular for the front end and .NET Core Web API with Entity Framework for the back end. The application uses a SQL Server database to store job-related data, ensuring efficient and organized data management.

## Features

- **User Authentication**: Users can log in using their email address and password with ASP.NET Core Identity.
- **Job Management**: Users can post and view maintenance jobs through a user-friendly dashboard.
- **Property and Service Provider Selection**: Users can select properties and service providers from pre-populated lists.
- **Unique Job Number Generation**: Each job receives a unique six-digit job number upon posting.

## Objectives

- Develop a web application using Angular and .NET Core to allow property managers to post and view maintenance jobs.
- Implement user login functionality and ensure that the application is accessible only to authenticated users.
- Create a SQL Server database using the Entity Framework Code First approach and pre-populate tables with relevant data.
- Host the application on Azure and set up CI/CD pipelines for smooth deployment.

## User Interface and Application Workflow

### User Login

- Users can log in using their username and password.
- Upon successful login, users will be redirected to the Job Dashboard.

### Job Dashboard

- Displays a list of jobs posted by the logged-in user, including Job Number, Description, and Posted Date.
- Users can select a job from the list to view its details or click the "New Job" button to open the Job Post Wizard.

### Job Post Wizard

1. **Job Details Page**
   - **Property**: Searchable text box for selecting properties.
   - **Owner**: Read-only list populated with details from the PropertyOwners table.
   - **Tenant**: Read-only list populated with details from the PropertyTenants table.
   - **Description**: Multi-line text area for job description (mandatory).
   - **Type**: Dropdown populated with job types (mandatory).
   - **Next**: Button to navigate to the Service Provider Selection step.

2. **Service Provider Selection**
   - **Service Provider**: Searchable text box for selecting a service provider based on job type (mandatory).
   - **Post Job**: Button to save job data to the Jobs table and display a confirmation message with the generated job number.

### Job Details

- Displays all relevant information for a selected job, including Property, Owner, Tenant, Description, Type, and Provider details.

## Source Control and DevOps

- **Source Code**: The application source code is stored in [GitHub](https://github.com/IrushiGunawardana/PropertyManagementSystem.git).
- **Azure App Service**: The web application is hosted in Azure App Service.
- **Azure SQL Database**: An Azure SQL database is used for the application database.
- **CI/CD Pipelines**: Build and deployment pipelines have been set up in Azure for streamlined application deployment.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 
- [Node.js](https://nodejs.org/en/download/) 
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Angular CLI](https://angular.io/cli)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/IrushiGunawardana/PropertyManagementSystem.git
