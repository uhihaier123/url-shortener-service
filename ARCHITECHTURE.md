# System Architecture Documentation

## High-Level Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                         USER'S BROWSER                       │
│                  (Chrome, Firefox, Edge, etc.)               │
└────────────────┬───────────────────────┬────────────────────┘
                 │                       │
                 │ 1. Submit URL         │ 2. Visit short URL
                 │                       │
                 ▼                       ▼
┌─────────────────────────────────────────────────────────────┐
│                    REACT FRONTEND                            │
│                  (User Interface Layer)                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  - Input field for long URL                          │  │
│  │  - Submit button                                     │  │
│  │  - Display shortened URL result                     │  │
│  │  - Copy to clipboard feature                        │  │
│  └──────────────────────────────────────────────────────┘  │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        │ HTTP POST/GET Requests
                        │ (JSON data)
                        ▼
┌─────────────────────────────────────────────────────────────┐
│              .NET CORE WEB API (Backend)                     │
│                  (Business Logic Layer)                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Controllers:                                        │  │
│  │    - UrlController (handles requests)                │  │
│  │                                                      │  │
│  │  Services:                                           │  │
│  │    - UrlService (business logic)                     │  │
│  │    - ShortCodeGenerator (creates unique codes)       │  │
│  │                                                      │  │
│  │  Data Layer:                                         │  │
│  │    - DbContext (Entity Framework)                    │  │
│  │    - Url Model (data structure)                      │  │
│  └──────────────────────────────────────────────────────┘  │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        │ SQL Queries (via Entity Framework)
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                   SQL SERVER 2022                            │
│                   (Data Storage Layer)                       │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Urls Table:                                         │  │
│  │    ┌────┬──────────┬───────────┬──────────┬────┐   │  │
│  │    │ Id │ Original │ ShortCode │ Created  │ ... │   │  │
│  │    ├────┼──────────┼───────────┼──────────┼────┤   │  │
│  │    │ 1  │ http://..│ abc123    │ 2025-... │ 0  │   │  │
│  │    │ 2  │ http://..│ def456    │ 2025-... │ 5  │   │  │
│  │    └────┴──────────┴───────────┴──────────┴────┘   │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Request Flow Diagrams

### Flow 1: Creating a Shortened URL
```
User                Frontend            Backend API         Database
  |                    |                    |                  |
  |  1. Enters URL     |                    |                  |
  |------------------->|                    |                  |
  |                    |                    |                  |
  |                    | 2. POST /api/urls  |                  |
  |                    |------------------->|                  |
  |                    |    /shorten        |                  |
  |                    |    {url: "..."}    |                  |
  |                    |                    |                  |
  |                    |                    | 3. Validate URL  |
  |                    |                    |                  |
  |                    |                    | 4. Generate code |
  |                    |                    |    (e.g. abc123) |
  |                    |                    |                  |
  |                    |                    | 5. INSERT INTO   |
  |                    |                    |    Urls table    |
  |                    |                    |----------------->|
  |                    |                    |                  |
  |                    |                    |    6. Success    |
  |                    |                    |<-----------------|
  |                    |                    |                  |
  |                    | 7. Return result   |                  |
  |                    |<-------------------|                  |
  |                    |    {shortUrl:...}  |                  |
  |                    |                    |                  |
  |  8. Show result    |                    |                  |
  |<-------------------|                    |                  |
  |  "Your short URL:  |                    |                  |
  |   abc123"          |                    |                  |
```

### Flow 2: Using a Shortened URL (Redirect)
```
User                Browser             Backend API         Database
  |                    |                    |                  |
  |  1. Clicks link    |                    |                  |
  |  /abc123           |                    |                  |
  |------------------->|                    |                  |
  |                    |                    |                  |
  |                    | 2. GET /abc123     |                  |
  |                    |------------------->|                  |
  |                    |                    |                  |
  |                    |                    | 3. SELECT FROM   |
  |                    |                    |    Urls WHERE    |
  |                    |                    |    ShortCode =   |
  |                    |                    |    'abc123'      |
  |                    |                    |----------------->|
  |                    |                    |                  |
  |                    |                    | 4. Return record |
  |                    |                    |<-----------------|
  |                    |                    |                  |
  |                    |                    | 5. Increment     |
  |                    |                    |    ClickCount    |
  |                    |                    |----------------->|
  |                    |                    |                  |
  |                    | 6. HTTP 302        |                  |
  |                    |    Redirect to     |                  |
  |                    |    original URL    |                  |
  |                    |<-------------------|                  |
  |                    |                    |                  |
  |  7. Load original  |                    |                  |
  |     website        |                    |                  |
  |<-------------------|                    |                  |
```

## Component Details

### 1. Frontend (React)
- **Purpose:** User interface for creating short URLs
- **Technology:** React 18 with Hooks
- **Key Files:**
  - `App.jsx` - Main component
  - `UrlShortener.jsx` - Form component
  - `ResultDisplay.jsx` - Shows shortened URL
- **Responsibilities:**
  - Collect user input
  - Send API requests
  - Display results
  - Handle client-side validation

### 2. Backend API (.NET Core)
- **Purpose:** Business logic and data management
- **Technology:** ASP.NET Core 8.0 Web API
- **Key Components:**
  - **Controllers:** Handle HTTP requests
  - **Services:** Contain business logic
  - **Models:** Define data structures
  - **DbContext:** Database connection
- **Responsibilities:**
  - Validate URLs
  - Generate unique short codes
  - Store/retrieve data
  - Handle redirects
  - Manage errors

### 3. Database (SQL Server)
- **Purpose:** Persistent data storage
- **Technology:** SQL Server 2022
- **Structure:** Single table (for Pass level)
- **Responsibilities:**
  - Store URL mappings
  - Ensure data integrity
  - Track usage statistics

## Technology Choices & Justification

### Why .NET Core?
- ✅ Modern, cross-platform framework
- ✅ High performance
- ✅ Excellent Visual Studio integration
- ✅ Built-in dependency injection
- ✅ Easy to containerize

### Why SQL Server?
- ✅ Reliable and mature
- ✅ Excellent Visual Studio integration
- ✅ Good for relational data
- ✅ ACID compliance ensures data integrity
- ✅ Free Developer edition

### Why React?
- ✅ Popular and well-documented
- ✅ Component-based architecture
- ✅ Large community support
- ✅ Easy to learn basics

### Why Docker?
- ✅ Consistent environments (dev = production)
- ✅ Easy deployment
- ✅ Isolates dependencies
- ✅ Required by assignment

### Why GitHub Actions?
- ✅ Free for public repositories
- ✅ Integrated with GitHub
- ✅ Easy to configure
- ✅ Good documentation

## Deployment Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                         DEVELOPER                            │
│                     (Your Computer)                          │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        │ git push
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                         GITHUB                               │
│                   (Code Repository)                          │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        │ Triggers
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                    GITHUB ACTIONS                            │
│                   (CI/CD Pipeline)                           │
│  1. Checkout code                                            │
│  2. Run tests                                                │
│  3. Build Docker image                                       │
│  4. Push to Docker Hub                                       │
│  5. Trigger deployment                                       │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        │ Deploy
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                      RENDER.COM                              │
│                   (Cloud Platform)                           │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Running Docker Container:                            │  │
│  │    - .NET API                                         │  │
│  │    - React Frontend                                   │  │
│  │    - Connected to database                            │  │
│  └───────────────────────────────────────────────────────┘  │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        │ Access
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                          USERS                               │
│                  (Public Internet)                           │
└─────────────────────────────────────────────────────────────┘
```

## Security Considerations
- Input validation to prevent SQL injection
- HTTPS for all communications
- CORS configuration for API access
- No sensitive data in URLs
- Rate limiting (future enhancement)

## Performance Considerations
- Database indexing on ShortCode column
- Efficient short code generation
- Minimal API response payload
- Frontend optimization (code splitting)

## Future Enhancements (Merit/Distinction)
- Redis caching for frequently accessed URLs
- Microservices architecture
- Custom domain support
- Analytics dashboard
- User authentication
- URL expiration dates
```

**Save the file:** Press `Ctrl + S`

---

## Part 5: Save Your Work to GitHub

Now let's save (commit and push) your planning documents to GitHub!

### 5.1 Using Visual Studio's Git Integration

**Steps:**

1. In Visual Studio, look at the bottom-right corner
2. You'll see a status bar with **"0↑ 0↓"** or similar - this shows Git changes
3. Click on the **"0 ↑"** or look for the **Git Changes** window
   - If you don't see it: Go to **View** → **Git Changes**

4. You should see your new files listed:
   - PLAN.md
   - ARCHITECTURE.md

5. In the **Git Changes** window:
   - **Message box at top:** Type a descriptive message:
```
     Add project planning and architecture documents