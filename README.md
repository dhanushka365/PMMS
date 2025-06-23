# Property Maintenance Management System (PMMS)

A comprehensive web application for managing property maintenance requests with role-based access control, built with Angular frontend and .NET Core backend.

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
   - Frontend: http://localhost:4200
   - Backend API: http://localhost:5110
   - Swagger Documentation: http://localhost:5110/swagger

4. **Stop the application**
   ```bash
   docker-compose down
   ```

## 🏗️ Architecture Overview

### System Architecture
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Angular SPA   │    │   .NET Core     │    │   In-Memory     │
│   (Frontend)    │◄──►│   Web API       │◄──►│   Database      │
│   Port: 4200    │    │   Port: 5110    │    │   Entity        │
└─────────────────┘    └─────────────────┘    │   Framework     │
                                              └─────────────────┘
```

### Technology Stack
- **Frontend**: Angular 17+ with TypeScript, Angular Material, Reactive Forms
- **Backend**: .NET 8 Web API with Entity Framework Core
- **Database**: In-Memory Database (EF Core)
- **Authentication**: Role-based simulation (Admin/Property Manager)
- **Containerization**: Docker with multi-stage builds
- **Reverse Proxy**: Nginx for frontend routing and API proxying

### Project Structure
```
PMMS/
├── frontend/                    # Angular application
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/     # UI components
│   │   │   ├── services/       # HTTP and role services
│   │   │   ├── models/         # TypeScript interfaces
│   │   │   └── environments/   # Configuration files
│   │   └── ...
│   ├── Dockerfile
│   └── nginx.conf
├── backend/backend/backend/     # .NET Core API
│   ├── Controllers/            # API endpoints
│   ├── Application/
│   │   ├── Services/          # Business logic
│   │   ├── DTOs/              # Data transfer objects
│   │   └── Interfaces/        # Service contracts
│   ├── Domain/
│   │   ├── Entities/          # Data models
│   │   └── Enums/             # Status and role enums
│   ├── Infrastructure/
│   │   ├── Data/              # Database context
│   │   └── Repositories/      # Data access layer
│   └── Dockerfile
├── docker-compose.yml          # Container orchestration
└── README.md
```

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
   - API: https://localhost:7127
   - Swagger: https://localhost:7127/swagger

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

## 🧪 Testing

### Manual Testing Workflow
1. **Switch to Property Manager role**
2. **Create a maintenance request**
3. **Switch to Admin role**
4. **Accept or reject the request**
5. **Switch back to Property Manager**
6. **Verify read-only access to processed request**

### Key Test Scenarios
- Role switching and permissions
- CRUD operations with role restrictions
- Form validation and error handling
- Responsive design on different screen sizes
- Docker container deployment

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

### Main Endpoints
- `GET /api/maintenancerequests` - List all requests
- `GET /api/maintenancerequests/{id}` - Get specific request
- `POST /api/maintenancerequests` - Create new request
- `PUT /api/maintenancerequests/{id}` - Update request
- `DELETE /api/maintenancerequests/{id}` - Delete request

### Query Parameters
- `searchTerm` - Filter by text search
- `status` - Filter by status (0=New, 1=Accepted, 2=Rejected)
- `userRole` - Specify user role for permission validation

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

---

**Built with ❤️ using Angular and .NET Core**