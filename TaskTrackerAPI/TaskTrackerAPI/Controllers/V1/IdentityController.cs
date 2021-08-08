using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Registers new user in the system
        /// </summary>
        /// <response code="200">Registers new user in the system</response>
        /// <response code="400">Unable to register due to validation error</response>
        [HttpPost(ApiRoutes.Identity.Register)]
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        /// <summary>
        /// Login into the system
        /// </summary>
        /// <response code="200">Login into the system</response>
        /// <response code="400">Unable to login into the system due to validation error</response>
        [HttpPost(ApiRoutes.Identity.Login)]
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        /// <summary>
        /// Refresh user validation credentials
        /// </summary>
        /// <response code="200">Refresh user validation credentials</response>
        /// <response code="400">Unable to refresh user credentials due to validation error</response>
        [HttpPost(ApiRoutes.Identity.Refresh)]
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
        public async Task<IActionResult> LoginAsync([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}
