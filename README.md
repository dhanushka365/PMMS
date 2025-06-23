# Property Maintenance Management System (PMMS)

A comprehensive web application for managing property maintenance requests with role-based access control, built with Angular frontend and .NET Core backend.

## 📸 UI Screenshots & Application Workflow

### 🏠 Property Manager Interface

#### Dashboard Overview
![Property Manager Dashboard](./UI-Screenshots/property%20Manager%20dashbaord%20view.png)
*Property Manager dashboard with role indicator, search/filter functionality, and maintenance request cards*

#### Creating New Requests
![Create Maintenance Request](./UI-Screenshots/property%20manager%20create%20a%20maintenance%20request.png)
*Comprehensive form for creating maintenance requests with image upload, validation, and rich text descriptions*

#### Request Management
![New Request Listed](./UI-Screenshots/After%20create%20the%20maintenace%20request%20newly%20create%20job%20will%20listed%20in%20main%20dashboard.%20still%20property%20manager%20can%20delete%20or%20edit%20it%20untill%20admin%20or%20accept%20or%20reject%20it..png)
*Newly created requests appear immediately on dashboard - Property Manager maintains full control until admin processing*

![New Status Indicator](./UI-Screenshots/newly%20create%20job%20is%20still%20in%20new%20status.png)
*Clear status indicators and action buttons - delete option available for "New" status requests*

### 👨‍💼 Admin Interface

#### Admin Dashboard
![Admin Dashboard](./UI-Screenshots/Admin%20dashboard%20view.png)
*Admin dashboard with quick action buttons for efficient request processing - no "Add Request" button for admins*

#### Request Processing
![Admin Accept Request](./UI-Screenshots/admin%20accept%20the%20job.png)
*Quick action buttons allow admins to Accept or Reject requests directly from the dashboard*

#### Admin Edit Capabilities
![Admin Edit Details](./UI-Screenshots/admin%20can%20still%20correct%20the%20request%20details%20if%20he%20need.png)
*Admins retain full edit access to all requests regardless of status for corrections and updates*

### 🔒 Access Control Demonstration

#### View-Only Access for Processed Requests
![Property Manager View Only](./UI-Screenshots/once%20admin%20accept%20the%20job%20property%20manger%20can%20only%20view%20it.png)
*Once admin processes a request, Property Manager gets read-only access with "👁️ View Only" indicator*

### 🔄 Complete Workflow Summary

1. **Property Manager Creates Request** → Form with validation and image upload
2. **Request Appears on Dashboard** → "New" status with edit/delete options
3. **Admin Reviews & Processes** → Quick Accept/Reject buttons
4. **Status Changes to Accepted/Rejected** → Property Manager loses edit access
5. **View-Only Mode Activated** → Complete details visible but not editable

### 🎨 UI/UX Features Demonstrated

- **Role-Based Navigation**: Different interfaces for Property Manager vs Admin
- **Status-Driven Actions**: Buttons and options change based on request status
- **Visual Indicators**: Clear status badges, role indicators, and action cues
- **Professional Design**: Navy header, green gradient buttons, modern card layout
- **Responsive Layout**: Clean, organized interface that works on all screen sizes
- **Access Control Visualization**: "View Only" badges and disabled states

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- Git

### Running the Application
1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd PMMS
   ```

2. **Start the application**
   ```bash
   docker-compose up -d
   ```

3. **Access the application**
   - **Frontend**: http://localhost:4200
   - **Backend API**: http://localhost:5110
   - **Swagger Documentation**: http://localhost:5110/swagger (Interactive API testing)
   - **API Health Check**: http://localhost:5110/api/maintenancerequests

4. **Stop the application**
   ```bash
   docker-compose down
   ```

## ✨ Key Features

### 🎭 **Role-Based Access Control**
- **Property Manager Role**: Create, edit (New status only), delete (New status only)
- **Admin Role**: Process requests, full edit access, status management
- **Visual Role Indicators**: Clear role display with easy switching

### 🎨 **Modern User Interface**
- **Professional Design**: Navy blue header with green gradient action buttons
- **Responsive Layout**: Works seamlessly on desktop, tablet, and mobile
- **Intuitive Navigation**: Role-specific interfaces and contextual actions
- **Visual Status Indicators**: Color-coded status badges and "View Only" markers

### 🔄 **Smart Workflow Management**
- **Request Lifecycle**: New → Accepted/Rejected with proper state transitions
- **Dynamic Actions**: Buttons and options change based on user role and request status
- **Quick Actions**: One-click Accept/Reject buttons for efficient admin processing
- **View-Only Access**: Property managers can view but not edit processed requests

### 📝 **Rich Form Capabilities**
- **Comprehensive Forms**: Detailed maintenance request creation with validation
- **Image Upload**: Attach photos to maintenance requests
- **Real-time Validation**: Client-side form validation with helpful error messages
- **Auto-save Draft**: Form state preservation during navigation

### 🔍 **Advanced Search & Filtering**
- **Real-time Search**: Instant filtering across request names, properties, and descriptions
- **Status Filtering**: Filter by New, Accepted, or Rejected status
- **Combined Filtering**: Search and status filters work together

### 🛡️ **Security & Data Protection**
- **Role Validation**: Server-side permission checking
- **Input Sanitization**: Protection against XSS and injection attacks
- **Audit Trail**: Complete history of who created and modified requests
- **Secure Communication**: HTTPS support and CORS protection

## 🏗️ Architecture Overview

### Clean Architecture Implementation

The Property Maintenance Management System follows **Clean Architecture** principles with clear separation of concerns across four distinct layers. The architecture is documented with a comprehensive PlantUML diagram showing layer relationships and dependencies.

#### Architecture Diagram
![Backend Clean Architecture](./UI-Screenshots/backend%20clean%20architecture.png)
*Backend architecture following Clean Architecture principles with clear layer separation and dependency inversion*

The system architecture demonstrates:
- **Layer Boundaries**: Clear separation between API, Application, Domain, and Infrastructure
- **Dependency Flow**: Dependencies point inward following the Dependency Rule
- **Component Relationships**: Interfaces, implementations, and their interactions
- **Business Logic Isolation**: Core domain logic independent of external concerns

### Layer Responsibilities

#### 🎯 **API Layer (Controllers)**
- **Purpose**: Entry point for HTTP requests
- **Responsibilities**:
  - HTTP request/response handling
  - Input validation and model binding
  - Route mapping and parameter extraction
  - Response formatting (JSON serialization)
  - HTTP status code management
- **Dependencies**: Only depends on Application layer interfaces
- **Example**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class MaintenanceRequestsController : ControllerBase
{
    private readonly IMaintenanceRequestService _service;
    
    public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetAllMaintenanceRequests(
        [FromQuery] string? searchTerm = null,
        [FromQuery] MaintenanceStatus? status = null)
    {
        var requests = await _service.GetAllMaintenanceRequestsAsync();
        return Ok(requests);
    }
}
```

#### 🔧 **Application Layer (Services)**
- **Purpose**: Orchestrates business workflows
- **Responsibilities**:
  - Business logic implementation
  - Role-based authorization
  - Data transformation (Entity ↔ DTO)
  - Cross-cutting concerns (logging, validation)
  - Use case coordination
- **Dependencies**: Depends on Domain entities and Infrastructure interfaces
- **Example**:
```csharp
public class MaintenanceRequestService : IMaintenanceRequestService
{
    public async Task<MaintenanceRequestDto> UpdateMaintenanceRequestAsync(
        int id, UpdateMaintenanceRequestDto dto, UserRole userRole)
    {
        var existingRequest = await _repository.GetByIdAsync(id);
        
        // Business rule: Only Admin can update status
        if (userRole == UserRole.Admin && dto.Status.HasValue)
        {
            existingRequest.Status = dto.Status.Value;
        }
        
        existingRequest.UpdatedDate = DateTime.UtcNow;
        var updatedRequest = await _repository.UpdateAsync(existingRequest);
        
        return _mapper.Map<MaintenanceRequestDto>(updatedRequest);
    }
}
```

#### 🏛️ **Domain Layer (Entities)**
- **Purpose**: Core business entities and rules
- **Responsibilities**:
  - Business entities definition
  - Domain logic and invariants
  - Business rules enforcement
  - Value objects and enums
  - Domain events (if applicable)
- **Dependencies**: No dependencies on other layers (pure business logic)
- **Example**:
```csharp
public class MaintenanceRequest
{
    public int Id { get; set; }
    public string MaintenanceEventName { get; set; } = string.Empty;
    public MaintenanceStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    
    // Domain method example
    public bool CanBeEditedBy(UserRole userRole)
    {
        return userRole == UserRole.Admin || Status == MaintenanceStatus.New;
    }
    
    public bool CanBeDeletedBy(UserRole userRole)
    {
        return userRole == UserRole.PropertyManager && Status == MaintenanceStatus.New;
    }
}
```

#### 🗄️ **Infrastructure Layer (Data Access)**
- **Purpose**: External system integration
- **Responsibilities**:
  - Database operations (Entity Framework)
  - External API integrations
  - File system operations
  - Email services
  - Caching implementation
- **Dependencies**: Depends on Domain entities, implements Application interfaces
- **Example**:
```csharp
public class MaintenanceRequestRepository : IMaintenanceRequestRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<IEnumerable<MaintenanceRequest>> SearchAsync(string searchTerm)
    {
        return await _context.MaintenanceRequests
            .Where(x => x.MaintenanceEventName.Contains(searchTerm) ||
                       x.PropertyName.Contains(searchTerm) ||
                       x.Description.Contains(searchTerm))
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }
}
```

### 🔄 Dependency Flow (Dependency Inversion Principle)

```
API Layer          →    Application Layer    →    Domain Layer
     ↓                        ↓                       ↑
Infrastructure Layer    ←    Repository Interface    ←
```

**Key Principles:**
- **Dependency Rule**: Dependencies point inward toward the Domain
- **Interface Segregation**: Each layer depends on interfaces, not implementations
- **Inversion of Control**: High-level modules don't depend on low-level modules
- **Single Responsibility**: Each layer has one reason to change

### 🎯 Benefits of This Architecture

#### **1. Testability**
- Business logic isolated in Application layer
- Easy to mock dependencies with interfaces
- Domain logic testable without external dependencies

#### **2. Maintainability**
- Clear separation of concerns
- Changes in one layer don't affect others
- Easy to locate and modify specific functionality

#### **3. Flexibility**
- Can swap out Infrastructure implementations
- Business logic independent of UI and database
- Easy to add new features without breaking existing code

#### **4. Scalability**
- Layers can be scaled independently
- Clean interfaces enable distributed deployment
- Caching and optimization can be added transparently

## 👥 Role Management & Access Control

### Role System
The application implements a two-tier role system:

#### 1. **Property Manager**
- **Permissions**:
  - ✅ Create new maintenance requests
  - ✅ Edit requests with "New" status only
  - ✅ Delete requests with "New" status only
  - ✅ View all requests (read-only for processed ones)
- **Restrictions**:
  - ❌ Cannot edit/delete requests processed by admins
  - ❌ Cannot change request status

#### 2. **Admin**
- **Permissions**:
  - ✅ View all maintenance requests
  - ✅ Edit any request regardless of status
  - ✅ Accept/Reject requests (status management)
  - ✅ Full access to all system features
- **Restrictions**:
  - ❌ Cannot create new requests (management role)
  - ❌ Cannot delete requests (audit trail preservation)

### Role Implementation
- **Frontend**: Role service with reactive state management
- **Backend**: UserRole enum passed with API requests
- **Validation**: Server-side role validation for status updates
- **UI**: Dynamic component rendering based on current role

## 📊 Data Flow & State Management

### Request Lifecycle
1. **Creation** (Property Manager) → Status: "New"
2. **Review** (Admin) → Status: "Accepted" or "Rejected"  
3. **Final State** → Read-only for Property Manager

### State Management
- **Frontend**: Reactive forms with real-time validation
- **Backend**: Entity Framework with change tracking
- **Persistence**: In-memory database for development
- **History**: Audit fields (CreatedBy, UpdatedBy, timestamps)

## 🔧 Development Setup

### Backend Development
1. **Prerequisites**: .NET 8 SDK
2. **Navigate to backend directory**:
   ```bash
   cd backend/backend/backend
   ```
3. **Restore packages**:
   ```bash
   dotnet restore
   ```
4. **Run the application**:
   ```bash
   dotnet run --launch-profile https
   ```
5. **Access**:
   - **API**: https://localhost:7127
   - **Swagger UI**: https://localhost:7127/swagger (Interactive API documentation)
   - **Health Check**: https://localhost:7127/api/maintenancerequests

### Frontend Development
1. **Prerequisites**: Node.js 18+, npm
2. **Navigate to frontend directory**:
   ```bash
   cd frontend
   ```
3. **Install dependencies**:
   ```bash
   npm install
   ```
4. **Start development server**:
   ```bash
   npm start
   ```
5. **Access**: http://localhost:4200

### Development API Configuration
Update `frontend/src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7127/api'  // Development backend
};
```

**Role-Based Authorization Tests**
```csharp
// Example: Testing role-based business rules
[Theory]
[InlineData(UserRole.Admin, MaintenanceStatus.Accepted, true)]
[InlineData(UserRole.PropertyManager, MaintenanceStatus.Accepted, false)]
public async Task UpdateMaintenanceRequestAsync_ShouldRespectRoleBasedStatusChangeRules(
    UserRole userRole, MaintenanceStatus requestedStatus, bool shouldChangeStatus)
{
    // Test verifies that only admins can change request status
    var result = await _service.UpdateMaintenanceRequestAsync(requestId, updateDto, userRole);
    
    if (shouldChangeStatus)
        result.Status.Should().Be(requestedStatus);
    else
        result.Status.Should().Be(MaintenanceStatus.New); // Unchanged
}
```

**Business Logic Validation**
- Status change permissions (Admin vs Property Manager)
- Data integrity and audit trail maintenance
- Image handling and preservation
- Error handling for invalid scenarios

**Edge Cases & Error Handling**
- Non-existent entity handling
- Null data scenarios
- Invalid role permissions
- Concurrent modification handling

##### **3. Test Structure**

**Unit Tests** (`MaintenanceRequestServiceTests.cs`)
- Mock all dependencies using Moq
- Test individual methods in isolation
- Verify business logic without external dependencies
- Fast execution, suitable for TDD

**Integration Tests** (`MaintenanceRequestServiceIntegrationTests.cs`)
- Use real AutoMapper configuration
- Test service layer with actual mapping logic
- Verify end-to-end data transformation
- Ensure proper integration between components

**Test Helpers** (`TestDataHelper.cs`)
- Centralized test data creation
- Consistent test entity generation
- Reduces test code duplication
- Maintains test data consistency

#### Running Tests

##### **Local Development**
```bash
# Navigate to test project
cd backend/backend/backend.Tests

# Restore packages
dotnet restore

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter MaintenanceRequestServiceTests

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

##### **Docker Environment**
```bash
# Build test image
docker build -f backend/backend/backend/Dockerfile.test -t pmms-tests .

# Run tests in container
docker run --rm pmms-tests dotnet test
```
### Test Explore Result
![Visul studio Test Explore final result ](./UI-Screenshots/Test%20execution%20results.jpg)

### Manual Testing Workflow
Follow this complete workflow to test all application features:

#### 1. **Property Manager Workflow**
1. **Start as Property Manager** (default role)
2. **Create a New Request**:
   - Click the green "➕ Add Maintenance Request" button
   - Fill in all required fields (Event Name, Property, Description)
   - Optionally upload an image
   - Submit the form
3. **Verify Dashboard Updates**:
   - New request appears with "New" status badge
   - 🗑️ Delete button visible in top-right corner
   - Request card is clickable for editing

#### 2. **Admin Processing Workflow**
1. **Switch to Admin Role**:
   - Click "Switch to Admin" button (header turns navy blue)
   - Notice "Add Maintenance Request" button disappears
2. **Process the Request**:
   - Locate the request with "New" status
   - Use green "✓ Accept" or red "✗ Reject" quick action buttons
   - Verify status badge updates immediately

#### 3. **View-Only Access Verification**
1. **Switch back to Property Manager**
2. **Attempt to Access Processed Request**:
   - Click on the processed request card
   - Verify "👁️ View Only" indicator in top-right corner
   - Confirm form opens in read-only mode with informational message
   - All fields should be grayed out and non-editable
   - Only "Close" button available (no Save/Update)

### Key Test Scenarios
- **Role Switching**: Verify UI changes between Property Manager and Admin views
- **Permission Enforcement**: Confirm edit/delete restrictions work properly
- **Status Workflow**: Test complete request lifecycle from creation to processing
- **Responsive Design**: Test on different screen sizes and devices
- **Form Validation**: Verify required field validation and error messages
- **Image Upload**: Test image attachment and display functionality

### UI/UX Testing Checklist
- ✅ **Role Indicators**: Clear role display in header
- ✅ **Visual Feedback**: Hover effects, button states, loading indicators
- ✅ **Status Badges**: Color-coded status indicators (New, Accepted, Rejected)
- ✅ **Action Buttons**: Context-sensitive buttons based on role and status
- ✅ **Responsive Layout**: Mobile, tablet, and desktop compatibility
- ✅ **Accessibility**: Proper labels, keyboard navigation, screen reader support

## 🚀 Deployment

### Production Deployment
The application is containerized and ready for production deployment:

```bash
# Build and start all services
docker-compose up -d --build

# Scale services (if needed)
docker-compose up -d --scale frontend=2

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### Environment Configuration
- **Development**: Local development with hot reload
- **Docker**: Containerized deployment with nginx
- **Production**: Optimized builds with health checks

## 📚 API Documentation

### 🔗 Swagger UI Interface
![Swagger API Documentation](./UI-Screenshots/backend%20swager%20UI%20Api%20documentaion.png)
*Comprehensive API documentation with interactive testing capabilities*

### 🌐 Accessing API Documentation
- **Local Development**: https://localhost:7127/swagger
- **Docker Deployment**: http://localhost:5110/swagger
- **Interactive Testing**: All endpoints can be tested directly from the Swagger UI

### 📋 Available Endpoints

#### **Maintenance Requests API**
```
GET    /api/MaintenanceRequests              # Get all maintenance requests
GET    /api/MaintenanceRequests?searchTerm={term}  # Search by text
GET    /api/MaintenanceRequests?status={0|1|2}     # Filter by status (0=New, 1=Accepted, 2=Rejected)
GET    /api/MaintenanceRequests/{id}         # Get specific request by ID
POST   /api/MaintenanceRequests              # Create new maintenance request
PUT    /api/MaintenanceRequests/{id}         # Update existing request
DELETE /api/MaintenanceRequests/{id}         # Delete maintenance request
```

### 📝 Request/Response Examples

#### **Create Maintenance Request**
```http
POST /api/MaintenanceRequests
Content-Type: application/json

{
  "maintenanceEventName": "Broken Window",
  "propertyName": "Apartment 205",
  "description": "Bedroom window glass is cracked and needs replacement",
  "imageFileName": "broken_window.jpg",
  "imageData": "base64_encoded_image_data...",
  "createdBy": "property.manager@company.com"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "maintenanceEventName": "Broken Window",
  "propertyName": "Apartment 205",
  "description": "Bedroom window glass is cracked and needs replacement",
  "status": 0,
  "imageFileName": "broken_window.jpg",
  "imageData": "base64_encoded_image_data...",
  "createdDate": "2024-01-15T10:30:00Z",
  "updatedDate": null,
  "createdBy": "property.manager@company.com",
  "updatedBy": null
}
```

#### **Update Request Status (Admin Only)**
```http
PUT /api/MaintenanceRequests/1?userRole=1
Content-Type: application/json

{
  "maintenanceEventName": "Broken Window",
  "propertyName": "Apartment 205", 
  "description": "Bedroom window glass is cracked and needs replacement",
  "status": 1,
  "updatedBy": "admin@company.com"
}
```

#### **Search and Filter Examples**
```http
# Search by text
GET /api/MaintenanceRequests?searchTerm=window

# Filter by status (New requests only)
GET /api/MaintenanceRequests?status=0

# Combined search and filter
GET /api/MaintenanceRequests?searchTerm=plumbing&status=1
```

### 🔐 Authentication & Authorization

#### **User Role Parameter**
Most endpoints accept an optional `userRole` query parameter:
- `0` = Property Manager (default)
- `1` = Admin

```http
# Property Manager update (can only edit basic fields)
PUT /api/MaintenanceRequests/1?userRole=0

# Admin update (can change status and all fields)
PUT /api/MaintenanceRequests/1?userRole=1
```

#### **Role-Based Permissions**
| Endpoint | Property Manager | Admin |
|----------|------------------|-------|
| GET (all) | ✅ View all | ✅ View all |
| GET (single) | ✅ View details | ✅ View details |
| POST (create) | ✅ Create new | ❌ Cannot create |
| PUT (update) | ✅ Edit New status only | ✅ Edit all |
| PUT (status change) | ❌ Cannot change status | ✅ Accept/Reject |
| DELETE | ✅ Delete New status only | ❌ Cannot delete |

### 📊 Status Codes & Error Handling

#### **HTTP Status Codes**
- `200 OK` - Successful GET, PUT operations
- `201 Created` - Successful POST operation
- `204 No Content` - Successful DELETE operation
- `400 Bad Request` - Validation errors, invalid input
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server-side errors

#### **Error Response Format**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "MaintenanceEventName": ["The MaintenanceEventName field is required."],
    "PropertyName": ["The PropertyName field is required."]
  }
}
```

### 🧪 Testing with Swagger UI

#### **Interactive Testing Features**
1. **Try It Out**: Click any endpoint to test with real data
2. **Parameter Input**: Fill in required and optional parameters
3. **Request Body**: JSON editor with syntax highlighting
4. **Response Preview**: See actual API responses
5. **Authentication**: Test with different user roles
6. **Schema Documentation**: Detailed model definitions

#### **Common Test Scenarios**
1. **Create Request**: Use POST endpoint with sample data
2. **List Requests**: GET all requests to see created data
3. **Filter by Status**: Test status filtering (0, 1, 2)
4. **Search Functionality**: Test text search across fields
5. **Role-Based Updates**: Compare Property Manager vs Admin permissions
6. **Error Handling**: Test with invalid data to see error responses

### 🔧 API Configuration

#### **CORS Settings**
The API is configured to accept requests from:
- `http://localhost:4200` (Angular dev server)
- `http://localhost:*` (Any localhost port for development)
- Frontend container in Docker network

#### **Content Types**
- **Request**: `application/json`
- **Response**: `application/json`
- **Image Data**: Base64 encoded strings within JSON

#### **Validation Rules**
- **MaintenanceEventName**: Required, max 200 characters
- **PropertyName**: Required, max 200 characters  
- **Description**: Required, max 1000 characters
- **ImageData**: Optional, Base64 encoded
- **Status**: Enum (0=New, 1=Accepted, 2=Rejected)

### 📖 API Design Principles

#### **RESTful Design**
- Resource-based URLs (`/api/MaintenanceRequests`)
- HTTP verbs for actions (GET, POST, PUT, DELETE)
- Proper status codes for different outcomes
- Consistent JSON request/response format

#### **Role-Based Security**
- Server-side permission validation
- Role parameter for context-aware operations
- Status-based access control (e.g., only New requests can be deleted)

#### **Developer Experience**
- Comprehensive Swagger documentation
- Interactive API testing
- Clear error messages with validation details
- Consistent naming conventions and structure

The Swagger UI provides a complete, interactive documentation experience that allows developers to understand, test, and integrate with the PMMS API efficiently.

## 🛠️ Future Improvements

With more time, the following enhancements could be implemented:

### 1. **Authentication & Security**
- **JWT Authentication**: Replace role simulation with proper auth
- **OAuth Integration**: Support for Google/Microsoft login
- **Permission System**: Granular permissions beyond basic roles
- **Audit Logging**: Comprehensive activity tracking
- **Input Validation**: Enhanced server-side validation

### 2. **Database & Persistence**
- **SQL Server/PostgreSQL**: Replace in-memory database
- **Database Migrations**: Structured schema versioning
- **Data Seeding**: Sample data for development/testing
- **Backup Strategy**: Automated backup and recovery
- **Connection Pooling**: Optimized database connections

### 3. **Advanced Features**
- **File Management**: Dedicated file storage service (AWS S3/Azure Blob)
- **Email Notifications**: Automated status change notifications
- **Advanced Search**: Full-text search with filters
- **Dashboard Analytics**: Charts and reporting features
- **Mobile App**: React Native or Flutter mobile client
- **Real-time Updates**: SignalR for live status updates

### 4. **Performance & Scalability**
- **Caching Strategy**: Redis for session and data caching
- **API Pagination**: Efficient data loading for large datasets
- **CDN Integration**: Static asset optimization
- **Load Balancing**: Multi-instance deployment
- **Monitoring**: Application performance monitoring (APM)

### 5. **Testing & Quality**
- **Unit Tests**: Comprehensive backend and frontend testing
- **Integration Tests**: End-to-end API testing
- **E2E Tests**: Automated UI testing with Cypress/Playwright
- **Performance Tests**: Load testing and optimization
- **Code Quality**: SonarQube integration and code analysis

### 6. **DevOps & Deployment**
- **CI/CD Pipeline**: Azure DevOps/GitHub Actions
- **Infrastructure as Code**: Terraform/ARM templates
- **Container Orchestration**: Kubernetes deployment
- **Environment Management**: Staging and production environments
- **Monitoring & Alerting**: Comprehensive logging and metrics

### 7. **User Experience**
- **Internationalization**: Multi-language support
- **Progressive Web App**: Offline capabilities
- **Advanced UI Components**: Rich text editor, drag-and-drop
- **Accessibility**: WCAG compliance
- **Custom Themes**: Configurable branding

### 8. **Business Features**
- **Workflow Engine**: Complex approval processes
- **Integration APIs**: Third-party service connections
- **Reporting Module**: Custom report generation
- **Multi-tenant Support**: Organization-based isolation
- **Advanced Role Management**: Custom role creation

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 📸 Screenshot Reference

All UI screenshots are located in the `UI-Screenshots/` directory and demonstrate:

### **Application Interface**
- **property Manager dashbaord view.png**: Main dashboard with role switching and request overview
- **property manager create a maintenance request.png**: Comprehensive form with validation and image upload
- **After create the maintenace request newly create job will listed in main dashboard...png**: Request lifecycle and status management
- **newly create job is still in new status.png**: Status indicators and available actions
- **Admin dashboard view.png**: Admin interface with processing capabilities
- **admin accept the job.png**: Quick action buttons for efficient request processing
- **admin can still correct the request details if he need.png**: Admin edit capabilities
- **once admin accept the job property manger can only view it.png**: Role-based access control in action

### **API Documentation**
- **backend swager UI Api documentaion.png**: Interactive Swagger UI showing all available endpoints, request/response schemas, and testing capabilities

---

**Built with ❤️ using Angular and .NET Core**