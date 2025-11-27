using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HomeCareApp.Models;
using HomeCareApp.Service.Availabilitys;
using AppUser = HomeCareApp.Models.User;
using HomeCareApp.DTOs;
using Microsoft.VisualBasic;


namespace HomeCareApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvailabilityController : Controller
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly ILogger<AvailabilityController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AvailabilityController(
            IAvailabilityService availabilityService,
            ILogger<AvailabilityController> logger,
            UserManager<AppUser> userManager)
        {
            _availabilityService = availabilityService;
            _logger = logger;
            _userManager = userManager;
        }

        // -----------------------------
        // INDEX - show all availabilities
        // -----------------------------
        [Authorize(Roles = "Personnel, Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Availability.Index called by {User}", User.Identity?.Name);

                var availabilities = await _availabilityService.GetAllAsync() ?? new List<Availability>();
                _logger.LogInformation("Loaded {Count} availabilities", availabilities.Count);

                return View(availabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Availability.Index");
                TempData["Error"] = "Unexpected error occurred.";
                return RedirectToAction("Error", "Home");
            }
        }

        // API endpoint - list all availabilities
        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            try
            {
                _logger.LogInformation("Availability.List called");
                var availabilities = await _availabilityService.GetAllAsync() ?? new List<Availability>();
                return Ok(availabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Availability.List");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AvailabilityDto availabilityDto)
        {
           if(availabilityDto == null)
           {
            return BadRequest("Availability data can't be null.");
           }
              var availability = new Availability
              {
                PersonnelId = availabilityDto.PersonnelId,
                Date = availabilityDto.Date,
                StartTime = availabilityDto.StartTime,
                EndTime = availabilityDto.EndTime,
                Notes = availabilityDto.Notes
              };

            bool retunOk = await _availabilityService.AddAsync(availability);
            if(retunOk)
            return CreatedAtAction(nameof(Create), new { id = availability.Id }, availability);
            
            _logger.LogWarning("[AvailabilityController] Create: Unable to create availability {@availability}", availability);
            return StatusCode(500, "A problem in the internal server occurred.");

               
             
        }

        // -----------------------------
        // CREATE (GET) - show form
        // -----------------------------
        [Authorize(Roles = "Personnel, Admin")]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("Availability.Create(GET) opened by {User}", User.Identity?.Name);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Availability.Create(GET)");
                TempData["Error"] = "Unexpected error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        // -----------------------------
        // CREATE (POST) - create new availability
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Personnel,Admin")]
        public async Task<IActionResult> Create(Availability availability)
        {
            try
            {
                _logger.LogInformation("Availability.Create(POST) called by {User}", User.Identity?.Name);

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Create failed: No logged-in user");
                    ModelState.AddModelError("", "Ingen innlogget bruker.");
                    return View(availability);
                }

                availability.PersonnelId = userId;
                ModelState.Remove(nameof(availability.PersonnelId));

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Create model invalid: {Errors}",
                        string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)));

                    return View(availability);
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Create failed: user {UserId} not found", userId);
                    ModelState.AddModelError("", $"Innlogget bruker finnes ikke (Id={userId}).");
                    return View(availability);
                }

                await _availabilityService.AddAsync(availability);
                _logger.LogInformation("Availability {Id} created by {User}", availability.Id, userId);

                TempData["Success"] = "Availability created.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Availability");
                TempData["Error"] = "Could not create availability.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailability(int id)
        {
            var availability = await _availabilityService.GetByIdAsync(id);
            if (availability == null)
            {
                _logger.LogError(
                    "[AvailabilityController] Availability not found for Id {Id:0000}", 
                    id
                );
                return NotFound("Availability not found for the given Id");
            }

            return Ok(availability);
        }

        // API endpoint - update availability
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AvailabilityDto availabilityDto)
        {
            if (availabilityDto == null)
            {
                return BadRequest("Availability data can't be null.");
            }

            var availability = await _availabilityService.GetByIdAsync(id);
            if (availability == null)
            {
                _logger.LogError("Availability not found for Id {Id}", id);
                return NotFound("Availability not found");
            }

            availability.Date = availabilityDto.Date;
            availability.StartTime = availabilityDto.StartTime;
            availability.EndTime = availabilityDto.EndTime;
            availability.Notes = availabilityDto.Notes;

            bool success = await _availabilityService.UpdateAsync(availability);
            if (success)
            {
                _logger.LogInformation("Availability {Id} updated", id);
                return Ok(availability);
            }

            _logger.LogWarning("[AvailabilityController] Update: Unable to update availability Id {Id}", id);
            return StatusCode(500, "A problem in the internal server happened.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool returnOk = await _availabilityService.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[AvailabilityController] Availability deletion failed for the AvailabilityId {AvailabilityId:0000}", id);
                return BadRequest("Availability deletion failed");
            }
            return NoContent(); 
        }

        


        // -----------------------------
        // EDIT (GET)
        // -----------------------------
        [Authorize(Roles = "Personnel,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                _logger.LogInformation("Availability.Edit(GET) for Id {Id} by {User}", id, User.Identity?.Name);

                var entity = await _availabilityService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Availability.Edit(GET): NotFound Id {Id}", id);
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && entity.PersonnelId != userId)
                {
                    _logger.LogWarning("User {User} attempted to edit availability they do not own: Id {Id}", userId, id);
                    return Forbid();
                }

                if (entity.Appointment != null)
                {
                    _logger.LogWarning("Attempt to edit booked availability {Id}", id);
                    TempData["Error"] = "This availability is already booked and cannot be edited.";
                    return RedirectToAction(nameof(Index));
                }

                return View(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Availability.Edit(GET) Id {Id}", id);
                TempData["Error"] = "Could not load edit page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // -----------------------------
        // EDIT (POST)
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Personnel,Admin")]
        public async Task<IActionResult> Edit(int id, Availability model)
        {
            try
            {
                _logger.LogInformation("Availability.Edit(POST) Id {Id} by {User}", id, User.Identity?.Name);

                if (id != model.Id)
                {
                    _logger.LogWarning("BadRequest: URL Id {Id} != model.Id {ModelId}", id, model.Id);
                    return BadRequest();
                }

                var entity = await _availabilityService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Availability.Edit(POST): NotFound Id {Id}", id);
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && entity.PersonnelId != userId)
                {
                    _logger.LogWarning("User {User} tried editing availability Id {Id} they do not own", userId, id);
                    return Forbid();
                }

                if (entity.Appointment != null)
                {
                    _logger.LogWarning("Attempt to edit booked availability Id {Id}", id);
                    TempData["Error"] = "This availability is already booked and cannot be edited.";
                    return RedirectToAction(nameof(Index));
                }

                if (model.StartTime >= model.EndTime)
                {
                    _logger.LogWarning("Invalid time range on Edit for Id {Id}", id);
                    ModelState.AddModelError(nameof(model.EndTime), "End time must be after start time.");
                    return View(model);
                }

                _logger.LogInformation(
                    "Edit model received for Id {Id}: Date={Date}, Start={Start}, End={End}, Notes={Notes}",
                    id, model.Date, model.StartTime, model.EndTime, model.Notes
                );

                ModelState.Remove(nameof(model.PersonnelId));

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validation failed on Edit for Id {Id}: {Errors}", id, errors);

                    return View(model);
                }

                entity.Date = model.Date;
                entity.StartTime = model.StartTime;
                entity.EndTime = model.EndTime;
                entity.Notes = model.Notes;

                await _availabilityService.UpdateAsync(entity);

                _logger.LogInformation("Availability {Id} updated successfully by {User}", entity.Id, userId);

                TempData["Success"] = "Availability updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Availability Id {Id}", id);
                TempData["Error"] = "Could not update availability.";
                return RedirectToAction(nameof(Index));
            }
        }

        // -----------------------------
        // DELETE (GET)
        // -----------------------------
        [Authorize(Roles = "Personnel,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Availability.Delete(GET) Id {Id} by {User}", id, User.Identity?.Name);

                var entity = await _availabilityService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Availability.Delete(GET): NotFound Id {Id}", id);
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && entity.PersonnelId != userId)
                {
                    _logger.LogWarning("User {User} attempted to delete availability Id {Id} they do not own", userId, id);
                    return Forbid();
                }

                if (entity.Appointment != null)
                {
                    _logger.LogWarning("Attempt to delete booked availability Id {Id}", id);
                    TempData["Error"] = "This availability is already booked and cannot be deleted.";
                    return RedirectToAction(nameof(Index));
                }

                return View(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Availability.Delete(GET) Id {Id}", id);
                TempData["Error"] = "Could not load delete page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // -----------------------------
        // DELETE (POST)
        // -----------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Personnel,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation("Availability.DeleteConfirmed(POST) Id {Id} by {User}", id, User.Identity?.Name);

                var entity = await _availabilityService.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Availability.DeleteConfirmed: NotFound Id {Id}", id);
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && entity.PersonnelId != userId)
                {
                    _logger.LogWarning("User {User} tried to delete availability Id {Id} they do not own", userId, id);
                    return Forbid();
                }

                if (entity.Appointment != null)
                {
                    _logger.LogWarning("Attempted to delete booked availability Id {Id}", id);
                    TempData["Error"] = "This availability is already booked and cannot be deleted.";
                    return RedirectToAction(nameof(Index));
                }

                await _availabilityService.DeleteAsync(id);
                _logger.LogInformation("Availability {Id} deleted by {User}", id, userId);

                TempData["Success"] = "Availability deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Availability Id {Id}", id);
                TempData["Error"] = "Could not delete availability.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
