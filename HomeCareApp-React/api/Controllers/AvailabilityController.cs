using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HomeCareApp.DTOs;
using HomeCareApp.Models;
using HomeCareApp.Service.Availabilitys;
using AppUser = HomeCareApp.Models.User;

namespace HomeCareApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // kan snevres inn senere
    public class AvailabilityApiController : ControllerBase   // legg merke til ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly ILogger<AvailabilityApiController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AvailabilityApiController(
            IAvailabilityService availabilityService,
            ILogger<AvailabilityApiController> logger,
            UserManager<AppUser> userManager)
        {
            _availabilityService = availabilityService;
            _logger = logger;
            _userManager = userManager;
        }

        // ⬇⬇ DENNE delen tilsvarer gule boksen i sliden ⬇⬇
        [HttpGet("list")]
        [Authorize(Roles = "Personnel,Admin")]
        public async Task<IActionResult> AvailabilityList()
        {
            var availabilities = await _availabilityService.GetAllAsync();

            if (availabilities == null || !availabilities.Any())
            {
                _logger.LogError("[AvailabilityApiController] Availability list not found while executing GetAllAsync()");
                return NotFound("Availability list not found");
            }

            var availabilityDtos = availabilities.Select(a => new AvailabilityDto
            {
                Id = a.Id,
                PersonnelId = a.PersonnelId,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Notes = a.Notes
            });

            return Ok(availabilityDtos);
        }
    }
}
