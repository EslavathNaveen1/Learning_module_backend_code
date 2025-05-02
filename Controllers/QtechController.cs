using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QtechBackend.Models;
using QtechBackend.ServiceInterface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using QtechBackend.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using SendingOTP.Controllers;
using Microsoft.AspNetCore.Identity.Data;
using Org.BouncyCastle.Crypto.Generators;
using QtechBackend.Repositories;
using QtechBackend.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using QtechBackendv2.ServiceInterface;
using QtechBackendv2.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace QtechBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QtechController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IDocumentationService _documentationService;
        private readonly IPlaylistService _playlistService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<QtechController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly OtpController _otpController;
        private readonly IUserRepository _userRepository;
        private readonly IEnrolledService _enrolledService;

        public QtechController(
            IVideoService videoService,
            IDocumentationService documentationService,
            IPlaylistService playlistService,
            IUserService userService,
            IConfiguration configuration,
            ILogger<QtechController> logger,
            IMemoryCache memoryCache,
            OtpController otpController,
            IUserRepository userRepository,
            IEnrolledService enrolledService)
        {
            _videoService = videoService;
            _documentationService = documentationService;
            _playlistService = playlistService;
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
            _memoryCache = memoryCache;
            _otpController = otpController;
             _userRepository = userRepository;
            _enrolledService = enrolledService;
        }

        #region Video Endpoints
        
        [HttpGet("Videos")]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideos()
        {
            try
            {
                var videos = await _videoService.GetVideosAsync();
                return Ok(videos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving videos");
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

        
        [HttpGet("Videos/{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            try
            {
                var video = await _videoService.GetVideoByIdAsync(id);

                if (video == null)
                {
                    return NotFound(new { message = $"Video with ID {id} not found" });
                }

                return Ok(video);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving video with ID {VideoId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

        
        [HttpPost("Videos")]
        public async Task<ActionResult<Video>> PostVideo([FromBody] Video video)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdVideo = await _videoService.AddVideoAsync(video);
                return CreatedAtAction(nameof(GetVideo), new { id = createdVideo.VideoId }, createdVideo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating video");
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

       
        [HttpPut("Videos/{id}")]
        public async Task<IActionResult> PutVideo(int id, [FromBody] Video video)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingVideo = await _videoService.GetVideoByIdAsync(id);
                if (existingVideo == null)
                {
                    return NotFound(new { message = $"Video with ID {id} not found" });
                }
                existingVideo.PlaylistId = video.PlaylistId;
                existingVideo.Title = video.Title;
                existingVideo.Url = video.Url;
                existingVideo.ImageUrl = video.ImageUrl;
                existingVideo.UpdatedAt = video.UpdatedAt;

                var updatedVideo = await _videoService.UpdateVideoAsync(existingVideo);
                return Ok(updatedVideo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating video with ID {VideoId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }
        
        [HttpPatch("Videos/Edit/{id}")]
        public async Task<IActionResult> PatchVideo(int id, [FromBody] VideoEdit videoEdit)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingVideo = await _videoService.GetVideoByIdAsync(id);
                if (existingVideo == null)
                {
                    return NotFound(new { message = $"Video with ID {id} not found" });
                }
                existingVideo.Title = videoEdit.Title;
                existingVideo.Url= videoEdit.Url;
                existingVideo.ImageUrl = videoEdit.imgUrl;
                existingVideo.UpdatedAt = DateTime.Now;

                var updatedVideo = await _videoService.UpdateVideoAsync(existingVideo);
                return Ok(updatedVideo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while patching video with ID {VideoId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }
        public class VideoEdit
        {

            [Required]
            [StringLength(100)]
            public string Title { get; set; }

            [Required]
            [Url]
            public string Url { get; set; }

            public string? imgUrl { get; set; }
        }

        [HttpDelete("Videos/{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            try
            {
                var existingVideo = await _videoService.GetVideoByIdAsync(id);
                if (existingVideo == null)
                {
                    return NotFound(new { message = $"Video with ID {id} not found" });
                }

                await _videoService.DeleteVideoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting video with ID {VideoId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }
        #endregion


        [HttpGet("documents")]
        [SwaggerResponse(200, "Documents retrieved successfully")]
        [SwaggerResponse(404, "No documents found")]
        public async Task<IActionResult> GetAllDocuments()
        {
            try
            {
                var documents = await _documentationService.GetAllDocumentsAsync();

                if (documents == null || !documents.Any())
                    return NotFound("No documents found.");

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPut("update-pdf/{id}")]
        [SwaggerOperation(
            Summary = "Update PDF File",
            Description = "Updates a specific PDF document with validation")]
        [SwaggerResponse(200, "File updated successfully")]
        [SwaggerResponse(400, "Invalid file or no file uploaded")]
        [SwaggerResponse(404, "PDF file not found")]
        [SwaggerResponse(413, "File too large")]
        public async Task<IActionResult> UpdatePdf(
            int id,
            [SwaggerParameter("PDF file to upload", Required = true)] IFormFile file)

        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

           
            const long maxFileSize = 50 * 1024 * 1024;
            if (file.Length > maxFileSize)
                return StatusCode(413, "File is too large. Maximum file size is 50MB.");

            var allowedExtensions = new[] { ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType.ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension) ||
                !new[] { "application/pdf", "application/x-pdf" }.Contains(contentType))
            {
                return BadRequest("Invalid file type. Only PDF files are allowed.");
            }

            try
            {
                var existingPdf = await _documentationService.GetPdfByIdAsync(id);
                if (existingPdf == null)
                {
                    return NotFound($"PDF file with ID {id} not found.");
                }

                await _documentationService.UpdatePdfAsync(id, file);

                return Ok(new
                {
                    Message = "File updated successfully",
                    FileName = file.FileName,
                    FileSize = file.Length
                });
            }
            catch (Exception ex)
            {


                return StatusCode(500, new
                {
                    Message = "Internal server error. Please try again later.",
                    ErrorId = Guid.NewGuid() 
                });
            }
        }
     
        [HttpPost("upload")]

        [SwaggerResponse(200, "File uploaded successfully")]
        [SwaggerResponse(400, "Invalid file or missing required fields")]
        public async Task<IActionResult> UploadDocument(
           [FromForm] int playlistId,
           [FromForm] string title,
           [FromForm] string content,
           [FromForm] DateTime createdAt,
           [FromForm] DateTime updatedAt,
           [FromForm] IFormFile file
            )
        {
            
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Document title is required.");

            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Document content is required.");

            if (file.ContentType != "application/pdf")
                return BadRequest("Invalid file type. Only PDF files are allowed.");

            try
            {
                var documentDto = new Documentation
                {
                    PlaylistId = playlistId,
                    FileName = file.FileName,

                    Content = content,
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt,
                    FileContent = await GetFileBytes(file)
                };

                await _documentationService.SaveDocumentAsync(documentDto);
                return Ok("Document uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        [HttpGet("documents/{playlistId}")]

        [SwaggerResponse(200, "Documents retrieved successfully")]
        [SwaggerResponse(404, "No documents found")]
        public async Task<IActionResult> GetDocumentsByPlaylist(int playlistId)
        {
            try
            {
                var documents = await _documentationService.GetDocumentsByPlaylistIdAsync(playlistId);

                if (documents == null || !documents.Any())
                    return NotFound("No documents found for this playlist.");

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpDelete("document/{docId}")]
        [SwaggerResponse(200, "Document deleted successfully")]
        [SwaggerResponse(404, "Document not found")]
        public async Task<IActionResult> DeleteDocument(int docId)
        {
            try
            {
                await _documentationService.DeleteDocumentAsync(docId);
                return Ok("Document deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        private async Task<byte[]> GetFileBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


        #region Playlist Endpoints
        
        [HttpGet("Playlists")]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            try
            {
                var playlists = await _playlistService.GetPlaylistsAsync();
                return Ok(playlists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving playlists");
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

        [HttpGet("Playlists/{id}")]
        public async Task<ActionResult<Playlist>> GetPlaylist(int id)
        {
            try
            {
                var playlist = await _playlistService.GetPlaylistByIdAsync(id);

                if (playlist == null)
                {
                    return NotFound(new { message = $"Playlist with ID {id} not found" });
                }

                return Ok(playlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving playlist with ID {PlaylistId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

        [HttpPost("Playlists")]
        public async Task<ActionResult<Playlist>> PostPlaylist([FromBody] Playlist playlist)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdPlaylist = await _playlistService.AddPlaylistAsync(playlist);
                return CreatedAtAction(nameof(GetPlaylist), new { id = createdPlaylist.PlaylistId }, createdPlaylist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating playlist");
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

  
        [HttpPut("Playlists/{id}")]
        public async Task<IActionResult> PutPlaylist(int id, [FromBody] Playlist playlist)
        {
            try
            {
                if (id != playlist.PlaylistId)
                {
                    return BadRequest(new { message = "ID in URL does not match ID in the request body" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingPlaylist = await _playlistService.GetPlaylistByIdAsync(id);
                if (existingPlaylist == null)
                {
                    return NotFound(new { message = $"Playlist with ID {id} not found" });
                }

                var updatedPlaylist = await _playlistService.UpdatePlaylistAsync(playlist);
                return Ok(updatedPlaylist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating playlist with ID {PlaylistId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

        [HttpPatch("Playlists/Edit/{id}")]

        public async Task<IActionResult> PatchPlaylist(int id, [FromBody] PlaylistEdit playlistEdit)

        {
            try

            {
                _logger.LogInformation("Received PATCH request for Playlist ID {PlaylistId} with data: {@PlaylistEdit}",

                    id, playlistEdit);

                if (!ModelState.IsValid)

                {
                    return BadRequest(ModelState);
                }

                var existingPlaylist = await _playlistService.GetPlaylistByIdAsync(id);

                if (existingPlaylist == null)

                {
                    return NotFound(new { message = $"Playlist with ID {id} not found" });
                }

                existingPlaylist.Title = playlistEdit.Title;

                existingPlaylist.Description = playlistEdit.Description;

                existingPlaylist.ImageUrl = string.IsNullOrEmpty(playlistEdit.ImageUrl)

                    ? existingPlaylist.ImageUrl  

                    : playlistEdit.ImageUrl;     

                existingPlaylist.UpdatedAt = DateTime.UtcNow;

                await _playlistService.UpdatePlaylistAsync(existingPlaylist);

                return Ok(new

                {
                    playlistId = existingPlaylist.PlaylistId,

                    title = existingPlaylist.Title,

                    description = existingPlaylist.Description,

                    imageUrl = existingPlaylist.ImageUrl,

                    updatedAt = existingPlaylist.UpdatedAt

                });

            }

            catch (Exception ex)

            {
                _logger.LogError(ex, "Error occurred while patching playlist with ID {PlaylistId}", id);

                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });

            }

        }

        public class PlaylistEdit
        {
            [Required]
            [StringLength(100)]
            public string Title { get; set; }

            [StringLength(500)]
            public string Description { get; set; }
       
            public string ? ImageUrl { get; set; }
        }

        [HttpDelete("Playlists/{id}")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            try
            {
                var existingPlaylist = await _playlistService.GetPlaylistByIdAsync(id);
                if (existingPlaylist == null)
                {
                    return NotFound(new { message = $"Playlist with ID {id} not found" });
                }

                await _playlistService.DeletePlaylistAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting playlist with ID {PlaylistId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }
        #endregion

        #region User Endpoints
        // GET: api/Qtech/Users
        [HttpGet("Users")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving users");
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

        // GET: api/Qtech/Users/5
        [HttpGet("Users/{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<Users>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }



        //// POST: api/Qtech/Users
        //[HttpPost("Registration")]
        //// [Authorize(Policy = "AdminOnly")]
        //public async Task<ActionResult<Users>> PostUser([FromBody] RegisterOtpRequest registerOtpRequest)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        // Retrieve the stored OTP for the user's email
        //        if (_memoryCache.TryGetValue(registerOtpRequest.Email, out string otp))
        //        {
        //            if (otp == registerOtpRequest.Otp)
        //            {
        //                _memoryCache.Remove(registerOtpRequest.Email);

        //                // Retrieve the user object
        //                var user = await _userService.GetUserByEmailAsync(registerOtpRequest.Email);
        //                if (user != null)
        //                {
        //                    return BadRequest("User already exists.");
        //                }

        //                // Create a new user object
        //                user = new Users
        //                {
        //                    FirstName = registerOtpRequest.FirstName,
        //                    LastName = registerOtpRequest.LastName,
        //                    Email = registerOtpRequest.Email,
        //                    Password = HashPassword(registerOtpRequest.Password),
        //                    Role = "User",
        //                    DateJoined = DateTime.Now
        //                };

        //                var createdUser = await _userService.AddUserAsync(user);

        //                return CreatedAtAction(nameof(GetUser), new { id = createdUser.EmployeeId }, createdUser);
        //            }
        //            else
        //            {
        //                return BadRequest(new { success = false, message = "Invalid OTP." });
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest(new { success = false, message = "OTP expired or not found." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while creating user");
        //        return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
        //    }
        //}





        [HttpPost("Registration")]
        public async Task<ActionResult<Users>> PostUser([FromBody] RegisterOtpRequest registerOtpRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Retrieve the stored OTP for the user's email
                if (_memoryCache.TryGetValue(registerOtpRequest.Email, out string otp))
                {
                    if (otp == registerOtpRequest.Otp)
                    {
                        _memoryCache.Remove(registerOtpRequest.Email);

                        // Retrieve the user object
                        var user = await _userService.GetUserByEmailAsync(registerOtpRequest.Email);
                        if (user != null)
                        {
                            return BadRequest("User already exists.");
                        }

                        // Check if the database is empty
                        var userCount = await _userService.GetUserCountAsync();
                        if (userCount == 0)
                        {
                            // Assign the role of Manager to the first user
                            user = new Users
                            {
                                FirstName = registerOtpRequest.FirstName,
                                LastName = registerOtpRequest.LastName,
                                Email = registerOtpRequest.Email,
                                Password = HashPassword(registerOtpRequest.Password),
                                Role = "Manager",
                                DateJoined = DateTime.Now
                            };
                        }
                        else if (userCount == 1)
                        {
                            // Assign the role of Admin to the second user
                            user = new Users
                            {
                                FirstName = registerOtpRequest.FirstName,
                                LastName = registerOtpRequest.LastName,
                                Email = registerOtpRequest.Email,
                                Password = HashPassword(registerOtpRequest.Password),
                                Role = "Admin",
                                DateJoined = DateTime.Now
                            };
                        }
                        else
                        {
                            // Assign the role of User to subsequent users
                            user = new Users
                            {
                                FirstName = registerOtpRequest.FirstName,
                                LastName = registerOtpRequest.LastName,
                                Email = registerOtpRequest.Email,
                                Password = HashPassword(registerOtpRequest.Password),
                                Role = "User",
                                DateJoined = DateTime.Now
                            };
                        }

                        var createdUser = await _userService.AddUserAsync(user);

                        return CreatedAtAction(nameof(GetUser), new { id = createdUser.EmployeeId }, createdUser);
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Invalid OTP." });
                    }
                }
                else
                {
                    return BadRequest(new { success = false, message = "OTP expired or not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }







        [HttpPost("RegisterVerification")]
        //[AllowAnonymous]
        public async Task<IActionResult> PasswordRegister([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required");
            }

            try
            {
                var user = await _userService.GetUserByEmailAsync(request.Email);

                if (user != null)
                {
                    return BadRequest("User already exists with the provided email.");
                }

                var otpRequest = new OtpGenerateRequest
                {
                    Email = request.Email,
                    Name = request.Email // Assuming you want to use the email as the name if the user is null
                };

                var otpResult = await _otpController.GenerateOtp(otpRequest);

                if (otpResult is OkObjectResult okResult)
                {
                    _logger.LogInformation("OTP sent successfully to {Email}", request.Email);
                    return Ok(new { success = true, message = "OTP sent successfully" });
                }
                else if (otpResult is BadRequestObjectResult badRequestResult)
                {
                    _logger.LogWarning("Failed to send OTP to {Email}: {Reason}", request.Email, badRequestResult.Value);
                    return BadRequest(badRequestResult.Value);
                }
                else if (otpResult is StatusCodeResult statusCodeResult)
                {
                    _logger.LogError("Failed to send OTP to {Email}: StatusCode {StatusCode}", request.Email, statusCodeResult.StatusCode);
                    return StatusCode(statusCodeResult.StatusCode, "Failed to send OTP. Please try again.");
                }
                else
                {
                    _logger.LogError("Unexpected error occurred while sending OTP to {Email}", request.Email);
                    return StatusCode(500, "An unexpected error occurred while sending OTP.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending OTP to {Email}", request.Email);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send OTP",
                    error = ex.Message,
                });
            }
        }







        // PUT: api/Qtech/Users/5
        [HttpPut("Users/{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PutUser(int id, [FromBody] Users user)
        {
            try
            {
                if (id != user.EmployeeId)
                {
                    return BadRequest(new { message = "ID in URL does not match ID in the request body" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                var updatedUser = await _userService.UpdateUserAsync(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }


        //patch user

        [HttpPatch("Users/{id}")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] UpdateUserRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                existingUser.Role = request.Role;
                var updatedUser = await _userService.UpdateUserAsync(existingUser);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while patching user with ID {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }


        public class UpdateUserRoleRequest
        {
            public string Role { get; set; }
        }





        // DELETE: api/Qtech/Users/5
        [HttpDelete("Users/{id}")]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }
        #endregion

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.GetUserByEmailAsync(loginModel.Email);

                if (user == null || !VerifyPassword(loginModel.Password, user.Password))
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var token = GenerateJwtToken(user);
                return Ok(new { token, role = user.Role,mail = user.Email, fname = user.FirstName, lname=user.LastName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login attempt for user {Email}", loginModel.Email);
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateJwtToken(Users user)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating JWT token for user {Email}", user.Email);
                throw; 
            }
        }

        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required");
            }

            try
            {
                var user = await _userService.GetUserByEmailAsync(request.Email);

                if (user == null)
                {
                    return NotFound("User not found with the provided email.");
                }

                var otpRequest = new OtpGenerateRequest
                {
                    Email = request.Email,
                    Name = user.FirstName
                };

                var otpResult = await _otpController.GenerateOtp(otpRequest);

                if (otpResult is OkObjectResult okResult)
                {
                    return Ok(new { success = true, message = "OTP sent successfully" });
                }
                else if (otpResult is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }
                else if (otpResult is StatusCodeResult statusCodeResult)
                {
                    return StatusCode(statusCodeResult.StatusCode, "Failed to send OTP. Please try again.");
                }
                else
                {
                    return StatusCode(500, "An unexpected error occurred while sending OTP.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send OTP",
                    error = ex.Message,
                });
            }
        }

        [HttpPost("verifyotp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new { success = false, message = "Email, OTP, and new password are required." });
            }

            if (_memoryCache.TryGetValue(request.Email, out string otp))
            {
                if (otp == request.Otp)
                {
                    _memoryCache.Remove(request.Email);

                    var user = await _userService.GetUserByEmailAsync(request.Email);
                    if (user == null)
                    {
                        return NotFound(new { success = false, message = "User not found." });
                    }

                    user.Password = HashPassword(request.NewPassword); 
                    await _userRepository.UpdateUserAsync(user);
                    return Ok(new { success = true, message = "OTP verified and password updated successfully." });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Invalid OTP." });
                }
            }
            else
            {
                return BadRequest(new { success = false, message = "OTP expired or not found." });
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }
        
        public class VerifyOtpRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
            public string NewPassword { get; set; }
        }

        public class RegisterOtpRequest
        {
            public string Email { get; set; }
            
            public string FirstName {  get; set; }
            public string LastName { get; set; }
            public string Password { get; set; }
            public string Otp { get; set; }
        }



        [HttpGet("enrolleds")]
        public async Task<ActionResult<IEnumerable<UserPlaylistDto>>> GetUserPlaylists()
        {
            var userPlaylists = await _enrolledService.GetUserPlaylistsAsync();
            return Ok(userPlaylists);
        }


        [HttpGet("EnrolledPlaylistIds")]
        public async Task<ActionResult<List<int>>> GetPlaylistIdsByUserEmail([FromQuery] string userEmail)
        {
            var playlistIds = await _enrolledService.GetPlaylistIdsByUserEmailAsync(userEmail);
            return Ok(playlistIds);
        }

        [HttpPost("EnrollPlaylists")]
        public async Task<ActionResult<Enrolled>> CreateEnrolled([FromBody] Enrolled enrolled)
        {
            var createdEnrolled = await _enrolledService.CreateEnrolledAsync(enrolled);
            return CreatedAtAction(nameof(GetPlaylistIdsByUserEmail), new { userEmail = createdEnrolled.UserEmail }, createdEnrolled);
        }

        [HttpDelete("UnEnROll")]
        public async Task<IActionResult> DeleteEnrolled([FromQuery] string userEmail, [FromQuery] int playlistId)
        {
            await _enrolledService.DeleteEnrolledAsync(userEmail, playlistId);
            return NoContent();
        }

        [HttpPut("UpdateEnroll")]
        public async Task<ActionResult<Enrolled>> UpdateEnrolled([FromBody] Enrolled enrolled)
        {
            var updatedEnrolled = await _enrolledService.UpdateEnrolledAsync(enrolled);
            return Ok(updatedEnrolled);
        }


        [HttpPut("enrolleds/approveAll")]
        public async Task<IActionResult> ApproveAllEnrollments()
        {
            await _enrolledService.ApproveAllEnrollmentsAsync();
            return NoContent();
        }

        [HttpGet("enrolleds/pending")]
        public async Task<ActionResult<IEnumerable<UserPlaylistDto>>> GetPendingEnrollments()
        {
            var pendingEnrollments = await _enrolledService.GetPendingEnrollmentsAsync();
            return Ok(pendingEnrollments);
        }

        [HttpGet("enrolleds/approved")]
        public async Task<ActionResult<IEnumerable<UserPlaylistDto>>> GetApprovedEnrollments()
        {
            var approvedEnrollments = await _enrolledService.GetApprovedEnrollmentsAsync();
            return Ok(approvedEnrollments);
        }


    }
}