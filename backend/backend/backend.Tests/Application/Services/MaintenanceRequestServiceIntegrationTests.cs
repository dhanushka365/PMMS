using AutoMapper;
using backend.Application.DTOs;
using backend.Application.Interfaces;
using backend.Application.Mappings;
using backend.Application.Services;
using backend.Domain.Entities;
using backend.Domain.Enums;
using backend.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace backend.Tests.Application.Services
{
    /// <summary>
    /// Integration tests for MaintenanceRequestService with real AutoMapper configuration
    /// </summary>
    public class MaintenanceRequestServiceIntegrationTests
    {
        private readonly Mock<IMaintenanceRequestRepository> _mockRepository;
        private readonly IMapper _mapper;
        private readonly MaintenanceRequestService _service;

        public MaintenanceRequestServiceIntegrationTests()
        {
            _mockRepository = new Mock<IMaintenanceRequestRepository>();
            
            // Configure real AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            
            _service = new MaintenanceRequestService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task CreateMaintenanceRequestAsync_WithRealMapper_ShouldMapCorrectly()
        {
            // Arrange
            var createDto = TestDataHelper.CreateMaintenanceRequestDto(
                eventName: "Broken Window",
                propertyName: "Apartment 205",
                description: "Window needs replacement",
                createdBy: "pm@example.com",
                imageFileName: "window.jpg",
                imageData: "base64data"
            );

            var expectedEntity = TestDataHelper.CreateMaintenanceRequest(
                id: 1,
                eventName: createDto.MaintenanceEventName,
                propertyName: createDto.PropertyName,
                description: createDto.Description,
                status: MaintenanceStatus.New,
                createdBy: createDto.CreatedBy,
                imageFileName: createDto.ImageFileName,
                imageData: createDto.ImageData
            );

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<MaintenanceRequest>()))
                .ReturnsAsync(expectedEntity);

            // Act
            var result = await _service.CreateMaintenanceRequestAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedEntity.Id);
            result.MaintenanceEventName.Should().Be(createDto.MaintenanceEventName);
            result.PropertyName.Should().Be(createDto.PropertyName);
            result.Description.Should().Be(createDto.Description);
            result.Status.Should().Be(MaintenanceStatus.New);
            result.CreatedBy.Should().Be(createDto.CreatedBy);
            result.ImageFileName.Should().Be(createDto.ImageFileName);
            result.ImageData.Should().Be(createDto.ImageData);

            // Verify repository was called with correctly mapped entity
            _mockRepository.Verify(r => r.CreateAsync(It.Is<MaintenanceRequest>(entity =>
                entity.MaintenanceEventName == createDto.MaintenanceEventName &&
                entity.PropertyName == createDto.PropertyName &&
                entity.Description == createDto.Description &&
                entity.Status == MaintenanceStatus.New &&
                entity.CreatedBy == createDto.CreatedBy &&
                entity.ImageFileName == createDto.ImageFileName &&
                entity.ImageData == createDto.ImageData &&
                entity.CreatedDate > DateTime.UtcNow.AddMinutes(-1)
            )), Times.Once);
        }

        [Fact]
        public async Task GetAllMaintenanceRequestsAsync_WithRealMapper_ShouldMapCollectionCorrectly()
        {
            // Arrange
            var entities = TestDataHelper.CreateMaintenanceRequestList(3);
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllMaintenanceRequestsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            
            var resultList = result.ToList();
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var dto = resultList[i];
                
                dto.Id.Should().Be(entity.Id);
                dto.MaintenanceEventName.Should().Be(entity.MaintenanceEventName);
                dto.PropertyName.Should().Be(entity.PropertyName);
                dto.Description.Should().Be(entity.Description);
                dto.Status.Should().Be(entity.Status);
                dto.CreatedBy.Should().Be(entity.CreatedBy);
                dto.CreatedDate.Should().Be(entity.CreatedDate);
            }
        }

        //[Theory]
        //[InlineData(UserRole.Admin, MaintenanceStatus.New, MaintenanceStatus.Accepted)]
        //[InlineData(UserRole.Admin, MaintenanceStatus.New, MaintenanceStatus.Rejected)]
        //[InlineData(UserRole.PropertyManager, MaintenanceStatus.New, MaintenanceStatus.New)]
        //public async Task UpdateMaintenanceRequestAsync_WithRealMapper_ShouldRespectRoleBasedPermissions(
        //    UserRole userRole, 
        //    MaintenanceStatus initialStatus, 
        //    MaintenanceStatus expectedFinalStatus)
        //{
        //    // Arrange
        //    var requestId = 1;
        //    var originalEntity = TestDataHelper.CreateMaintenanceRequest(
        //        id: requestId,
        //        status: initialStatus,
        //        createdBy: "original@example.com",
        //        createdDate: DateTime.UtcNow.AddDays(-1)
        //    );

        //    var updateDto = TestDataHelper.CreateUpdateMaintenanceRequestDto(
        //        eventName: "Updated Event Name",
        //        propertyName: "Updated Property Name",
        //        description: "Updated Description",
        //        status: MaintenanceStatus.Accepted, // Request to accept
        //        updatedBy: "updater@example.com"
        //    );

        //    // Mock the repository to return the original entity on GetByIdAsync
        //    _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(originalEntity);
            
        //    // Mock UpdateAsync to return the entity with captured changes
        //    MaintenanceRequest? capturedEntity = null;
        //    _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MaintenanceRequest>()))
        //        .Callback<MaintenanceRequest>(entity => capturedEntity = entity)
        //        .ReturnsAsync((MaintenanceRequest entity) => entity);

        //    // Act
        //    var result = await _service.UpdateMaintenanceRequestAsync(requestId, updateDto, userRole);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Id.Should().Be(requestId);
        //    result.MaintenanceEventName.Should().Be(updateDto.MaintenanceEventName);
        //    result.PropertyName.Should().Be(updateDto.PropertyName);
        //    result.Description.Should().Be(updateDto.Description);
        //    result.Status.Should().Be(expectedFinalStatus);
        //    result.UpdatedBy.Should().Be(updateDto.UpdatedBy);

        //    // Verify the entity passed to repository had correct status based on role
        //    capturedEntity.Should().NotBeNull();
        //    capturedEntity!.Status.Should().Be(expectedFinalStatus);
            
        //    if (userRole == UserRole.Admin)
        //    {
        //        capturedEntity.Status.Should().Be(MaintenanceStatus.Accepted);
        //    }
        //    else
        //    {
        //        capturedEntity.Status.Should().Be(initialStatus); // Should remain unchanged
        //    }
        //}

        [Fact]
        public async Task UpdateMaintenanceRequestAsync_WithImageUpdate_ShouldHandleImageDataCorrectly()
        {
            // Arrange
            var requestId = 1;
            var originalEntity = TestDataHelper.CreateMaintenanceRequest(
                id: requestId,
                imageFileName: "old_image.jpg",
                imageData: "old_base64_data"
            );

            var updateDto = TestDataHelper.CreateUpdateMaintenanceRequestDto(
                imageFileName: "new_image.png",
                imageData: "new_base64_data"
            );

            _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(originalEntity);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MaintenanceRequest>()))
                .ReturnsAsync((MaintenanceRequest entity) => entity);

            // Act
            var result = await _service.UpdateMaintenanceRequestAsync(requestId, updateDto, UserRole.PropertyManager);

            // Assert
            result.Should().NotBeNull();
            result.ImageFileName.Should().Be(updateDto.ImageFileName);
            result.ImageData.Should().Be(updateDto.ImageData);

            // Verify repository was called with updated image data
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<MaintenanceRequest>(entity =>
                entity.ImageFileName == updateDto.ImageFileName &&
                entity.ImageData == updateDto.ImageData
            )), Times.Once);
        }

        [Fact]
        public async Task SearchMaintenanceRequestsAsync_WithRealMapper_ShouldReturnCorrectlyMappedResults()
        {
            // Arrange
            var searchTerm = "window";
            var matchingEntities = new List<MaintenanceRequest>
            {
                TestDataHelper.CreateMaintenanceRequest(1, "Broken Window", "Office A"),
                TestDataHelper.CreateMaintenanceRequest(2, "Door Repair", "Window Office"),
                TestDataHelper.CreateMaintenanceRequest(3, "General Maintenance", "Room 1", "Fix the window lock")
            };

            _mockRepository.Setup(r => r.SearchAsync(searchTerm)).ReturnsAsync(matchingEntities);

            // Act
            var result = await _service.SearchMaintenanceRequestsAsync(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            
            var resultList = result.ToList();
            for (int i = 0; i < matchingEntities.Count; i++)
            {
                var entity = matchingEntities[i];
                var dto = resultList[i];
                
                dto.Id.Should().Be(entity.Id);
                dto.MaintenanceEventName.Should().Be(entity.MaintenanceEventName);
                dto.PropertyName.Should().Be(entity.PropertyName);
                dto.Description.Should().Be(entity.Description);
            }
        }

        [Fact]
        public async Task GetMaintenanceRequestsByStatusAsync_WithRealMapper_ShouldFilterAndMapCorrectly()
        {
            // Arrange
            var targetStatus = MaintenanceStatus.Accepted;
            var filteredEntities = new List<MaintenanceRequest>
            {
                TestDataHelper.CreateMaintenanceRequest(1, status: MaintenanceStatus.Accepted),
                TestDataHelper.CreateMaintenanceRequest(2, status: MaintenanceStatus.Accepted),
                TestDataHelper.CreateMaintenanceRequest(3, status: MaintenanceStatus.Accepted)
            };

            _mockRepository.Setup(r => r.GetByStatusAsync(targetStatus)).ReturnsAsync(filteredEntities);

            // Act
            var result = await _service.GetMaintenanceRequestsByStatusAsync(targetStatus);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().OnlyContain(dto => dto.Status == MaintenanceStatus.Accepted);
            
            var resultList = result.ToList();
            for (int i = 0; i < filteredEntities.Count; i++)
            {
                var entity = filteredEntities[i];
                var dto = resultList[i];
                
                dto.Id.Should().Be(entity.Id);
                dto.Status.Should().Be(targetStatus);
            }
        }

        [Fact]
        public async Task Service_WithNullImageData_ShouldHandleGracefully()
        {
            // Arrange
            var createDto = TestDataHelper.CreateMaintenanceRequestDto(
                imageFileName: null,
                imageData: null
            );

            var expectedEntity = TestDataHelper.CreateMaintenanceRequest(
                imageFileName: null,
                imageData: null
            );

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<MaintenanceRequest>()))
                .ReturnsAsync(expectedEntity);

            // Act
            var result = await _service.CreateMaintenanceRequestAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.ImageFileName.Should().BeNull();
            result.ImageData.Should().BeNull();
        }
    }
}