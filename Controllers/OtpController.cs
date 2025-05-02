using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QtechBackend.Interfaces;
using QtechBackend.Services;
using SendingOTP.Model;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SendingOTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly userService _userService;
        private readonly IMemoryCache _memoryCache;

        public OtpController(userService objuserService, IMemoryCache memoryCache)
        {
            _userService = objuserService;
            _memoryCache = memoryCache;
        }

        [HttpPost("generate")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateOtp([FromBody] OtpGenerateRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Email and name are required");
            }

            try
            {
                MethodInfo generateMethod = typeof(userService).GetMethod("GenerateNumber", BindingFlags.NonPublic | BindingFlags.Instance);
                if (generateMethod == null)
                {
                    
                    Console.WriteLine("OTP generation method not found in service");
                    return StatusCode(500, "OTP generation method not found in service");
                }

                string otp = (string)generateMethod.Invoke(_userService, null);
                
                Console.WriteLine($"Generated OTP: {otp}");

                _memoryCache.Set(request.Email, otp, TimeSpan.FromMinutes(5));

                MethodInfo sendMethod = typeof(userService).GetMethod("SendOtpMail", BindingFlags.NonPublic | BindingFlags.Instance);
                if (sendMethod == null)
                {
                
                    Console.WriteLine("Email sending method not found in service");
                    return StatusCode(500, "Email sending method not found in service");
                }

                await (Task)sendMethod.Invoke(_userService, new object[] { request.Email, otp, request.Name });

                return Ok(new
                {
                    success = true,
                    message = "OTP sent successfully"
                });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send OTP",
                    error = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }
    }

    public class OtpGenerateRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
