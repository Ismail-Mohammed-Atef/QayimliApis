# Rating API

## Overview
This is a .NET 8-based RESTful API designed to support a rating web application. It provides endpoints for user authentication, rating submissions, and review management. The API follows clean architecture principles and ensures security, scalability, and maintainability.

## Features
- **User Authentication & Authorization**: Secure login and role-based access control (RBAC) using JWT.
- **CRUD Operations**: Manage ratings, reviews, and users via RESTful endpoints.
- **API Security**: Implements JWT authentication and data validation.
- **Pagination & Filtering**: Retrieve data efficiently with sorting, filtering, and pagination.
- **Logging & Exception Handling**: Centralized logging and error handling for better debugging.
- **SOLID Principles & Clean Architecture**: Ensures a well-structured and maintainable codebase.

## Installation
1. Clone the repository.
2. Install dependencies:
   ```sh
   dotnet restore
   ```
3. Configure the database connection in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server;Database=your-db;User Id=your-user;Password=your-password;"
     }
   }
   ```
4. Apply migrations and update the database:
   ```sh
   dotnet ef database update
   ```
5. Run the application:
   ```sh
   dotnet run
   ```


## Requirements
- **.NET Version**: .NET 8
- **Database**: SQL Server or PostgreSQL
- **Authentication**: JWT-based authentication
- **Logging**: Serilog or built-in .NET logging

## License
This project is licensed under the MIT License.

## Contact
For support or inquiries, reach out via email at your-email@example.com.

