using System.Security.Claims;
using HomeCareApp.Controllers;
using HomeCareApp.DAL;
using HomeCareApp.DTOs;
using HomeCareApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Tests.Controllers;

public class AvailabilityControllerTests
{
    private readonly Mock<IAvailabilityRepository> _availabilityRepoMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILogger<AvailabilityController>> _loggerMock = new();

    // Constructor for a test version of AvailabilityController
    // Sets up mocked dependencies so the controller can be tested in isolation
    public AvailabilityControllerTests()
    {
        // Create a mock of the underlying user store.
        var store = new Mock<IUserStore<User>>();
        // Create a mock of UserManager<User>.
        _userManagerMock = new Mock<UserManager<User>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!); // only the store is relevant here, the rest can be null
    }

    // Helper method for unit tests: creates a controller instance. Attaches a fake authenticated user
    private AvailabilityController CreateControllerWithUser(string userId, string role)
    {
        // Build the controller with mocked dependencies
        var controller = new AvailabilityController(
                _availabilityRepoMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object);

        // Create a fake user identity containing the provided userId and role.
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, role)
    }, "TestAuth"));

        // Inject the fake user into the controller's HttpContext.
        // This simulates an authenticated request for unit testing.
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    /** 
    * CREATE Test
    * Test 1: Valid create operation
    * Test 2: Invalid create operation
    */

    [Fact]
    public async Task Create_ValidRequest()
    {
        // Arrange: define a test user ID and create a controller instance with a fake authenticated Personnel user.
        var userId = "user1";
        var controller = CreateControllerWithUser(userId, "Personnel");

        var appUser = new User { Id = userId, FullName = "Test User" };
        // Mock UserManager so that requesting this userId returns the fake user.
        _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(appUser);

        // Capture the Availability entity that the controller attempts to save.
        Availability? saved = null;
        // Assign ID and capture the saved object
        _availabilityRepoMock.Setup(r => r.AddAsync(It.IsAny<Availability>()))
                .Returns<Availability>(a =>
                {
                    a.Id = 1;
                    saved = a;
                    return Task.CompletedTask;
                });

        // Build a DTO representing valid availability input.
        var dto = new AvailabilityCreateDto
        {
            Date = DateTime.Today,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Notes = "Note"
        };

        // Act: call the controller method under test.
        var result = await controller.Create(dto);

        // Assert: verify that the controller returns HTTP 201 Created and that the response contains the expected AvailabilityDto.
        var created = Assert.IsType<CreatedAtActionResult>(result);
        var resultDto = Assert.IsType<AvailabilityDto>(created.Value);
        Assert.Equal(1, resultDto.Id);
        Assert.Equal(userId, resultDto.PersonnelId);
        Assert.Equal("Test User", resultDto.PersonnelName);
        Assert.NotNull(saved);
        Assert.Equal(userId, saved!.PersonnelId);
    }

    [Fact]
    public async Task Create_InvalidRequest()
    {
        // Arrange: define a user ID that does not exist in the system.
        var userId = "missing-user";
        var controller = CreateControllerWithUser(userId, "Personnel");

        // Simulate a missing or invalid user so that userId returns null
        _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

        // Build a DTO with valid availability data.
        // Even though the data is valid, the request is invalid because the user doesn't exist.
        var dto = new AvailabilityCreateDto
        {
            Date = DateTime.Today,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            Notes = "Note"
        };

        // Act: attempt to create availability for a non-existent user.
        var result = await controller.Create(dto);

        // Assert: the controller should reject the request and return a 400 BadRequest response.
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /** 
    * READ TEST
    * Test 1: Valid read operation
    * Test 2: Invalid read operation
    */

    [Fact]
    public async Task Get_ValidReturn()
    {
        // Arrange: simulate an authenticated Admin user.
        var userId = "admin1";
        var controller = CreateControllerWithUser(userId, "Admin");

        // Create a fake Availability record that the repository will return.
        var availability = new Availability
        {
            Id = 10,
            PersonnelId = "pers-1",
            Personnel = new User { Id = "pers-1", FullName = "Personnel 1" },
            Date = DateTime.Today,
            StartTime = TimeSpan.FromHours(8),
            EndTime = TimeSpan.FromHours(9),
            Notes = "Test"
        };

        // Mock repository so GetByIdAsync(10) returns this Availability.
        _availabilityRepoMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(availability);

        // Act: call the controller to retrieve availability with ID 10.
        var result = await controller.Get(10);

        // Assert: the controller should return HTTP 200 OK.
        var ok = Assert.IsType<OkObjectResult>(result.Result);

        // Extract the returned DTO and verify its mapped values.
        var dto = Assert.IsType<AvailabilityDto>(ok.Value);
        Assert.Equal(10, dto.Id);
        Assert.Equal("pers-1", dto.PersonnelId);
        Assert.Equal("Personnel 1", dto.PersonnelName);
    }

    [Fact]
    public async Task Get_InvalidReturn()
    {
        // Arrange: create an Admin user and controller instance
        var userId = "admin1";
        var controller = CreateControllerWithUser(userId, "Admin");

        // Mock repository so that requesting ID 99 returns null, simulating a non‑existent availability entry
        _availabilityRepoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Availability?)null);

        // Act: call the Get method with an ID that doesn't exist
        var result = await controller.Get(99);

        // Assert: controller should return HTTP 404 NotFound
        Assert.IsType<NotFoundResult>(result.Result);
    }

    /** 
    * UPDATE TEST
    * Test 1: Valid update operation
    * Test 2: Invalid update operation
    */

    [Fact]
    public async Task Update_Valid()
    {
        // Arrange: simulate a Personnel user updating their own availability.
        var userId = "pers1";
        var controller = CreateControllerWithUser(userId, "Personnel");

        // Create an existing Availability entry that will be fetched from the repository.
        var existing = new Availability
        {
            Id = 5,
            PersonnelId = userId,
            Date = DateTime.Today,
            StartTime = TimeSpan.FromHours(8),
            EndTime = TimeSpan.FromHours(9),
            Notes = "Existing Availability"
        };

        // Mock repository: return the existing Availability when asked for ID 5.
        _availabilityRepoMock.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(existing);

        // Build a DTO containing updated availability details.
        var dto = new AvailabilityCreateDto
        {
            Date = DateTime.Today.AddDays(1),
            StartTime = TimeSpan.FromHours(10),
            EndTime = TimeSpan.FromHours(11),
            Notes = "Updated Availability"
        };

        // Act: invoke the Update method with the provided DTO.
        var result = await controller.Update(5, dto);

        // Assert: expect a 204 NoContent response indicating success.
        Assert.IsType<NoContentResult>(result);

        // Verify that UpdateAsync was called once with the updated values.
        _availabilityRepoMock.Verify(r => r.UpdateAsync(It.Is<Availability>(a =>
                a.Id == 5 &&
                a.Date == dto.Date &&
                a.StartTime == dto.StartTime &&
                a.EndTime == dto.EndTime &&
                a.Notes == dto.Notes)), Times.Once);
    }

    [Fact]
    public async Task Update_Invalid()
    {
        // Arrange: define the owner of the availability and simulate a different logged-in user attempting to update it.
        var ownerId = "owner1";
        var otherId = "foreign1";
        var controller = CreateControllerWithUser(otherId, "Personnel");

        // Create an availability entry owned by someone else.
        var existing = new Availability
        {
            Id = 6,
            PersonnelId = ownerId,
            Date = DateTime.Today
        };

        // Mock repository: when fetching ID 6, return the entry owned by ownerId.
        _availabilityRepoMock.Setup(r => r.GetByIdAsync(6))
                .ReturnsAsync(existing);

        // Build a DTO containing the attempted update.
        var dto = new AvailabilityCreateDto
        {
            Date = DateTime.Today,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10)
        };

        // Act: try to update availability owned by another user.
        var result = await controller.Update(6, dto);

        // Assert: controller should return Forbid, blocking unauthorised updates.
        Assert.IsType<ForbidResult>(result);

        // Verify that UpdateAsync was never called since the user lacked permission.
        _availabilityRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Availability>()), Times.Never);
    }

    /** 
    * DELETE TEST
    * Test 1: Valid delete operation
    * Test 2: Invalid delete operation
    */

    [Fact]
    public async Task Delete_Valid()
    {
        // Arrange: simulate a user with ID "admin-1" who has the admin role.
        var adminId = "admin-1";

        var controller = CreateControllerWithUser(adminId, "Admin");

        // Define a fake availability record that exists in the repository.
        var existing = new Availability
        {
            Id = 7,
            PersonnelId = "pers-1"
        };

        // Mock repository: when GetByIdAsync(7) is called, it returns a fake availability record.
        _availabilityRepoMock.Setup(r => r.GetByIdAsync(7))
            .ReturnsAsync(existing);

        // Mock repository's DeleteAsync method.
        _availabilityRepoMock.Setup(r => r.DeleteAsync(7))
            .Returns(Task.CompletedTask);

        // Act: perform delete operation. Call the controller's Delete method with the ID 7.
        var result = await controller.Delete(7);


        // Assert: verify expected outcomes. Ensure the controller returned a "204 No Content" response
        Assert.IsType<NoContentResult>(result);

        // Confirm that DeleteAsync(7) was actually called exactly once
        _availabilityRepoMock.Verify(r => r.DeleteAsync(7), Times.Once);
    }

    [Fact]
    public async Task Delete_Invalid()
    {
        // Arrange: define the owner of the availability and simulate a different logged-in user attempting to delete it.

        var ownerId = "owner-1";
        var otherId = "foreign-1";

        // Simulate a user with ID "foreign-1" and role "Personnel" (not an admin). This user is not the owner.
        var controller = CreateControllerWithUser(otherId, "Personnel");

        // Create an availability entry owned by "owner-1".
        var existing = new Availability
        {
            Id = 8,
            PersonnelId = ownerId
        };
        
        // Mock repository: when fetching ID 8, return the entry.
        _availabilityRepoMock.Setup(r => r.GetByIdAsync(8))
            .ReturnsAsync(existing);

        // Act: Attempt to delete the record as "foreign-1" (not the owner).
        var result = await controller.Delete(8);

        // Assert: verify expected outcomes. Ensure the controller responded with ForbidResult (HTTP 403).
        Assert.IsType<ForbidResult>(result);

        // Confirm that DeleteAsync was NEVER called, since the user lacked permission to delete someone else's record.
        _availabilityRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}

