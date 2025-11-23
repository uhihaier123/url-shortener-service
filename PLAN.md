# URL Shortener Project Plan

## Team Information
- **Team Name:** [Your Team Name]
- **Team Members:**
  - Member 1: [Your Name] - Role: [Backend/Frontend/DevOps]
  - Member 2: [Name] - Role: [Role]
  - Member 3: [Name] - Role: [Role]

## Project Overview
Building a URL shortener service using .NET Core with automated CI/CD deployment.

## Project Timeline (12 Days)
- **Days 1-4:** Backend Development (.NET API)
- **Days 5-6:** Frontend Development (React)
- **Days 7:** Dockerization
- **Days 8-9:** CI/CD Pipeline Setup
- **Days 10:** Deployment & Testing
- **Days 11-12:** Documentation & Presentation

## Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0 Web API
- **Database:** SQL Server 2022
- **ORM:** Entity Framework Core
- **Testing:** xUnit

### Frontend
- **Framework:** React 18
- **HTTP Client:** Axios
- **Styling:** CSS/Tailwind CSS

### DevOps
- **Version Control:** Git & GitHub
- **Containerization:** Docker
- **CI/CD:** GitHub Actions
- **Container Registry:** Docker Hub
- **Hosting:** Render.com (PaaS)
- **API Testing:** Postman

## Database Design

### URLs Table Schema
```sql
CREATE TABLE Urls (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OriginalUrl NVARCHAR(2048) NOT NULL,
    ShortCode NVARCHAR(10) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    ClickCount INT NOT NULL DEFAULT 0,
    INDEX IX_ShortCode (ShortCode)
);
```

**Column Descriptions:**
- `Id`: Auto-incrementing primary key
- `OriginalUrl`: The long URL to shorten (max 2048 characters)
- `ShortCode`: Unique 6-8 character code (e.g., "abc123")
- `CreatedAt`: Timestamp when URL was created
- `ClickCount`: Number of times the short URL was accessed

## API Endpoints Design

### 1. POST /api/urls/shorten
**Purpose:** Create a shortened URL

**Request Body:**
```json
{
  "url": "https://www.example.com/very/long/url/path"
}
```

**Success Response (201 Created):**
```json
{
  "id": 1,
  "originalUrl": "https://www.example.com/very/long/url/path",
  "shortCode": "abc123",
  "shortUrl": "http://localhost:5000/abc123",
  "createdAt": "2025-11-23T10:30:00Z",
  "clickCount": 0
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Invalid URL format"
}
```

### 2. GET /{shortCode}
**Purpose:** Redirect to original URL

**Example:** `GET /abc123`

**Success Response:** HTTP 302 Redirect to original URL

**Error Response (404 Not Found):**
```json
{
  "error": "Short URL not found"
}
```

### 3. GET /api/urls/{shortCode}/stats (Optional)
**Purpose:** Get statistics for a short URL

**Success Response (200 OK):**
```json
{
  "shortCode": "abc123",
  "originalUrl": "https://www.example.com/very/long/url/path",
  "clickCount": 42,
  "createdAt": "2025-11-23T10:30:00Z"
}
```

## Short Code Generation Algorithm
- Use Base62 encoding (a-z, A-Z, 0-9)
- Generate 6-8 character codes
- Check database for uniqueness
- Retry if collision occurs

## Input Validation Rules
- URL must start with http:// or https://
- URL must be valid format
- URL length: 10-2048 characters
- Reject malicious URLs (optional: blacklist check)

## Error Handling Strategy
- Use global exception middleware
- Return consistent error format
- Log errors for debugging
- User-friendly error messages

## Unit Testing Plan
- Test short code generation
- Test URL validation
- Test database operations
- Test API endpoints
- Aim for >80% code coverage

## Docker Strategy
- Multi-stage Dockerfile
- Separate images for dev and production
- Use official Microsoft .NET images
- Keep image size minimal

## CI/CD Pipeline Stages
1. **Trigger:** Push to main branch
2. **Build:** Restore packages & compile
3. **Test:** Run unit tests
4. **Docker Build:** Create container image
5. **Push:** Upload to Docker Hub
6. **Deploy:** Trigger Render deployment

## Features Checklist (Pass Level)
- [ ] Generate unique short codes
- [ ] Store URLs in SQL Server database
- [ ] Redirect short URLs to original URLs
- [ ] Input validation (valid URL format)
- [ ] Error handling with appropriate messages
- [ ] RESTful API with proper HTTP methods
- [ ] Simple React frontend UI
- [ ] Unit tests with xUnit
- [ ] Dockerfile for containerization
- [ ] GitHub Actions CI/CD pipeline
- [ ] Automated deployment to Render
- [ ] Live demonstration

## Risks & Mitigation
- **Risk:** Database connection issues
  - *Mitigation:* Test connection strings early
- **Risk:** Docker build failures
  - *Mitigation:* Test locally before CI/CD
- **Risk:** Deployment problems
  - *Mitigation:* Set up Render early, test manually first

## Next Steps
1. ✅ Install all required tools
2. ✅ Create GitHub repository
3. ✅ Create project plan
4. ⬜ Create .NET Web API project
5. ⬜ Set up database connection
6. ⬜ Implement core features
7. ⬜ Add unit tests
8. ⬜ Build React frontend
9. ⬜ Create Dockerfile
10. ⬜ Set up CI/CD pipeline
11. ⬜ Deploy to production
12. ⬜ Prepare presentation