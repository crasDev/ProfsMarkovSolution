# ProfsMarkovHub

## Project Setup Instructions

### Prerequisites
- .NET 9.0 SDK
- Node.js (latest recommended version)

### Installation Steps

1. **Clone the Repository:**
   ```bash
   git clone https://<YOUR_REPO_URL>
   cd ProfsMarkovSolution
   git checkout feature/liquid-blog-hub
   ```

2. **Navigate to Project Directory:**
   ```bash
   cd ProfsMarkovHub
   ```

3. **Restore Dependencies:**
   ```bash
   dotnet restore
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```

### Features and Folder Structure

#### Areas
- `Blog` - Contains controllers, views, and other resources for the blog features.
- `Backoffice` - Administration area protected with ASP.NET Identity.
- `Store` - Contains e-commerce features including integration with StreamElements API.

#### Models
- `Article` - Represents blog content.
- `StoreItem` - Represents store items for purchase.
- `User` - Represents identity data for authentication and authorization.

#### Services
Custom services include:
- `BlobStorageService`
- `StreamElementsService`
- `MetaTagService`

#### Branding & Style
- **Primary Color:** #0066FF
- **Accent Color:** #00CC66
- **Dark Color:** #1A1A1A
- **Glass Background Color:** rgba(232,244,255,0.1)
- **CSS Framework:** Tailwind CSS along with custom `liquid_theme.css`
- **Design style:** Glassmorphism components with rounded corners (24px+), high-diffusion shadows, and SF Pro typography.

### Setting Up Database
1. Update `appsettings.json` with your database connection string and Azure Blob Storage credentials.

2. Apply Migrations:
   ```bash
   dotnet ef database update
   ```

### Run the Application
To start the application, use:
```bash
dotnet run
```

Navigate to `http://localhost:5000` to access the application.

## Note
Ensure mock keys are correctly set for Azure Blob Storage and the StreamElements API.