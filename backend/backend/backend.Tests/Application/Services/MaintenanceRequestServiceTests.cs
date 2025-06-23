using AutoMapper;
using backend.Application.DTOs;
using backend.Application.Interfaces;
using backend.Application.Services;
using backend.Domain.Entities;
using backend.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace backend.Tests.Application.Services
{
    public class MaintenanceRequestServiceTests
    {
        private readonly Mock<IMaintenanceRequestRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MaintenanceRequestService _service;

        public MaintenanceRequestServiceTests()
        {
            _mockRepository = new Mock<IMaintenanceRequestRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new MaintenanceRequestService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task SearchMaintenanceRequestsAsyncShouldReturnEmptyListWhenNoMatchesFound()
        {
            // Arrange
            var searchTerm = "nonexistent";
            var entities = new List<MaintenanceRequest>();
            var dtos = new List<MaintenanceRequestDto>();

            _mockRepository.Setup(r => r.SearchAsync(searchTerm)).ReturnsAsync(entities);
            _mockMapper.Setup(m => m.Map<IEnumerable<MaintenanceRequestDto>>(entities)).Returns(dtos);

            // Act
            var result = await _service.SearchMaintenanceRequestsAsync(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(r => r.SearchAsync(searchTerm), Times.Once);
        }

        [Fact]
        public async Task CreateMaintenanceRequestAsync_ShouldSetCorrectDefaults()
        {
            // Arrange
            var createDto = new CreateMaintenanceRequestDto
            {
                MaintenanceEventName = "Test Request",
                PropertyName = "Test Property",
                Description = "Test Description",
                CreatedBy = "test@example.com"
            };

            var mappedEntity = new MaintenanceRequest
            {
                MaintenanceEventName = createDto.MaintenanceEventName,
                PropertyName = createDto.PropertyName,
                Description = createDto.Description,
                CreatedBy = createDto.CreatedBy
            };

            var createdEntity = new MaintenanceRequest
            {
                Id = 1,
                MaintenanceEventName = createDto.MaintenanceEventName,
                PropertyName = createDto.PropertyName,
                Description = createDto.Description,
                CreatedBy = createDto.CreatedBy,
                Status = MaintenanceStatus.New,
                CreatedDate = DateTime.UtcNow
            };

            var resultDto = new MaintenanceRequestDto { Id = 1, Status = MaintenanceStatus.New };

            _mockMapper.Setup(m => m.Map<MaintenanceRequest>(createDto)).Returns(mappedEntity);
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<MaintenanceRequest>())).ReturnsAsync(createdEntity);
            _mockMapper.Setup(m => m.Map<MaintenanceRequestDto>(createdEntity)).Returns(resultDto);

            // Act
            var result = await _service.CreateMaintenanceRequestAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(MaintenanceStatus.New);

            // Verify that the service sets the correct defaults
            _mockRepository.Verify(r => r.CreateAsync(It.Is<MaintenanceRequest>(x =>
                x.Status == MaintenanceStatus.New &&
                x.CreatedDate > DateTime.UtcNow.AddMinutes(-1))), Times.Once);
        }

        [Fact]
        public async Task UpdateMaintenanceRequestAsync_ShouldUpdateImageData_WhenProvided()
        {
            // Arrange
            var requestId = 1;
            var updateDto = new UpdateMaintenanceRequestDto
            {
                MaintenanceEventName = "Updated Request",
                PropertyName = "Updated Property",
                Description = "Updated Description",
                ImageFileName = "new_image.jpg",
                ImageData = "new_base64_data",
                UpdatedBy = "admin@example.com"
            };

            var existingEntity = new MaintenanceRequest
            {
                Id = requestId,
                MaintenanceEventName = "Original Request",
                PropertyName = "Original Property",
                Description = "Original Description",
                Status = MaintenanceStatus.New,
                ImageFileName = "old_image.jpg",
                ImageData = "old_base64_data",
                CreatedBy = "user@example.com",
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            };

            var updatedEntity = new MaintenanceRequest
            {
                Id = requestId,
                ImageFileName = updateDto.ImageFileName,
                ImageData = updateDto.ImageData
            };

            var resultDto = new MaintenanceRequestDto { Id = requestId };

            _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(existingEntity);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MaintenanceRequest>())).ReturnsAsync(updatedEntity);
            _mockMapper.Setup(m => m.Map<MaintenanceRequestDto>(updatedEntity)).Returns(resultDto);

            // Act
            var result = await _service.UpdateMaintenanceRequestAsync(requestId, updateDto, UserRole.Admin);

            // Assert
            result.Should().NotBeNull();

            // Verify that new image data is set when provided
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<MaintenanceRequest>(x =>
                x.ImageFileName == "new_image.jpg" &&
                x.ImageData == "new_base64_data")), Times.Once);
        }

        [Fact]
        public async Task UpdateMaintenanceRequestAsync_ShouldPreserveImageData_WhenNotProvided()
        {
            // Arrange
            var requestId = 1;
            var updateDto = new UpdateMaintenanceRequestDto
            {
                MaintenanceEventName = "Updated Request",
                PropertyName = "Updated Property",
                Description = "Updated Description",
                UpdatedBy = "admin@example.com"
                // Note: No image data provided
            };

            var existingEntity = new MaintenanceRequest
            {
                Id = requestId,
                MaintenanceEventName = "Original Request",
                PropertyName = "Original Property",
                Description = "Original Description",
                Status = MaintenanceStatus.New,
                ImageFileName = "existing_image.jpg",
                ImageData = "existing_base64_data",
                CreatedBy = "user@example.com",
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            };

            var updatedEntity = new MaintenanceRequest
            {
                Id = requestId,
                MaintenanceEventName = updateDto.MaintenanceEventName,
                PropertyName = updateDto.PropertyName,
                Description = updateDto.Description,
                ImageFileName = "existing_image.jpg", // Should be preserved
                ImageData = "existing_base64_data", // Should be preserved
                UpdatedBy = updateDto.UpdatedBy
            };

            var resultDto = new MaintenanceRequestDto { Id = requestId };

            _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(existingEntity);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MaintenanceRequest>())).ReturnsAsync(updatedEntity);
            _mockMapper.Setup(m => m.Map<MaintenanceRequestDto>(updatedEntity)).Returns(resultDto);

            // Act
            var result = await _service.UpdateMaintenanceRequestAsync(requestId, updateDto, UserRole.Admin);

            // Assert
            result.Should().NotBeNull();

            // Verify that existing image data is preserved when not provided in update
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<MaintenanceRequest>(x =>
                x.ImageFileName == "existing_image.jpg" &&
                x.ImageData == "existing_base64_data")), Times.Once);
        }

        [Theory]
        [InlineData(UserRole.Admin, MaintenanceStatus.New, MaintenanceStatus.Accepted, true)]
        [InlineData(UserRole.Admin, MaintenanceStatus.New, MaintenanceStatus.Rejected, true)]
        [InlineData(UserRole.PropertyManager, MaintenanceStatus.New, MaintenanceStatus.Accepted, false)]
        [InlineData(UserRole.PropertyManager, MaintenanceStatus.Accepted, MaintenanceStatus.Rejected, false)]
        public async Task UpdateMaintenanceRequestAsync_ShouldRespectRoleBasedStatusChangeRules(
    UserRole userRole,
    MaintenanceStatus initialStatus,
    MaintenanceStatus requestedStatus,
    bool shouldChangeStatus)
        {
            // Arrange
            var requestId = 1;
            var updateDto = new UpdateMaintenanceRequestDto
            {
                MaintenanceEventName = "Test Request",
                PropertyName = "Test Property",
                Description = "Test Description",
                Status = requestedStatus,
                UpdatedBy = "test@example.com"
            };

            var existingEntity = new MaintenanceRequest
            {
                Id = requestId,
                MaintenanceEventName = "Original Request",
                PropertyName = "Original Property",
                Description = "Original Description",
                Status = initialStatus,
                CreatedBy = "creator@example.com",
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            };

            var expectedStatus = shouldChangeStatus ? requestedStatus : initialStatus;
            var updatedEntity = new MaintenanceRequest
            {
                Id = requestId,
                Status = expectedStatus,
                MaintenanceEventName = updateDto.MaintenanceEventName,
                PropertyName = updateDto.PropertyName,
                Description = updateDto.Description,
                UpdatedBy = updateDto.UpdatedBy
            };

            var resultDto = new MaintenanceRequestDto
            {
                Id = requestId,
                Status = expectedStatus
            };

            _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(existingEntity);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MaintenanceRequest>())).ReturnsAsync(updatedEntity);
            _mockMapper.Setup(m => m.Map<MaintenanceRequestDto>(updatedEntity)).Returns(resultDto);

            // Act
            var result = await _service.UpdateMaintenanceRequestAsync(requestId, updateDto, userRole);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(expectedStatus);

            if (shouldChangeStatus)
            {
                _mockRepository.Verify(r => r.UpdateAsync(It.Is<MaintenanceRequest>(x =>
                    x.Status == requestedStatus)), Times.Once);
            }
            else
            {
                _mockRepository.Verify(r => r.UpdateAsync(It.Is<MaintenanceRequest>(x =>
                    x.Status == initialStatus)), Times.Once);
            }
        }


    }
}