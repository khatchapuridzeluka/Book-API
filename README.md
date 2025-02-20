# 📚 Book Management API

This API supports basic CRUD operations for managing books using **SQL Server** and **Entity Framework**. It uses **Swagger** for API documentation and testing.

## Features

- **Authentication**:  
  To interact with the API, login using the following credentials and get the JWT token:
  - **Username**: `Exadel`
  - **Password**: `bookAPI`

- **CRUD Operations**:
  - **Add a Book** (single/bulk)
  - **Update a Book** (single)
  - **Soft Delete a Book** (single/bulk)
  - **Retrieve Book Titles** (list)
  - **Retrieve Book Details** (returns detailed info with a popularity score calculated on the fly)

- **Validations**:
  - `bookDTO` requires:
    - `Title`
    - `PublicationYear` (Range: 1000–2025)
    - `AuthorName`
  - A book can't be added if there is an existing one with the same `Title`, `AuthorName`, and `PublicationYear`.
  - `ViewsCount` cannot be modified via the API request.

## Setup

### 1. Clone the Repository
Clone this repository to your local machine:

```bash
git clone https://github.com/khatchapuridzeluka/Book-API.git
```
# 2. Update Connection String
In the `appsettings.json` file, replace the placeholder connection string with your own SQL Server connection string.
```bash
"ConnectionStrings": {
  "DefaultSQLConnection": "your_connection_string_here"
}
```

### 3. Restore NuGet Packages
Open Package Manager Console in Visual Studio (or your preferred IDE) and run:
```bash
dotnet restore
```

### 4. Run the API
Run the project with the following command:
```bash
dotnet run
```

