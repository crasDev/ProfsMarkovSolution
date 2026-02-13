# ProfsMarkovHub Blog Engine

## Overview
This is the Blog Engine for ProfsMarkovHub, built with ASP.NET Core MVC 9.0.

## Features
- **Public Blog**: View articles with a modern "Liquid" glassmorphism design.
- **Admin Area**: Manage articles (Create, Edit, Delete). Secured with Role-based authorization.
- **Authentication**: Identity system with Admin role.
- **Database**: SQLite for local development.

## Setup

1. **Prerequisites**:
   - .NET 9.0 SDK
   - SQLite

2. **Database Setup**:
   The application is configured to use SQLite. The database file `app.db` will be created automatically on startup or manually via EF Core tools.

   To manually update database:
   ```bash
   dotnet tool restore
   dotnet ef database update --project ProfsMarkovHub/ProfsMarkovHub.csproj
   ```

3. **Running the Application**:
   ```bash
   cd ProfsMarkovHub
   dotnet run
   ```

4. **Login**:
   - Navigate to `/Identity/Account/Login`
   - **Admin User**: `admin@profsmarkov.com`
   - **Password**: `P@ssword123!`

## Architecture
- **Models**: `Article`, `Tag`, `ArticleTag`
- **Controllers**: 
  - `AdminController` (Protected)
  - `BlogController` (Public)
- **Views**: 
  - Admin views use standard Bootstrap.
  - Blog views use custom "Liquid" theme CSS.

## Customization
- **Theme**: Modify `wwwroot/css/liquid_theme.css` for visual changes.
- **Content**: Use Markdown/HTML in the content field.
 
Blog engine added by Sorin Markov Bot 🜏
