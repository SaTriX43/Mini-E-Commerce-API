using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Common.Errors;
using Mini_E_Commerce_API.Common.Responses;
using Mini_E_Commerce_API.Common.Results;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected bool TryGetUserId(out int userId, out IActionResult errorResult)
        {
            userId = 0;
            errorResult = null!;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out userId))
            {
                errorResult = BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Error = new ApiErrorDto
                    {
                        Code = "Auth.InvalidUserId",
                        Message = "Su usuarioId debe ser un número"
                    }
                });

                return false;
            }

            return true;
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok(new ApiResponseDto<object> { Success = true });
            }

            return MapErrorToResponse(result.Error!);
        }

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(new ApiResponseDto<T>
                {
                    Success = true,
                    Value = result.Value
                });
            }

            return MapErrorToResponse(result.Error!);
        }

        private IActionResult MapErrorToResponse(Error error)
        {
            var apiError = new ApiResponseDto<object>
            {
                Success = false,
                Error = new ApiErrorDto
                {
                    Code = error.Code,
                    Message = error.Message
                }
            };

            return error.Type switch
            {
                ErrorType.Validation => BadRequest(apiError),
                ErrorType.NotFound => NotFound(apiError),
                ErrorType.Conflict => Conflict(apiError),
                ErrorType.Unauthorized => Unauthorized(apiError),
                ErrorType.Forbidden => Forbid(), 
                _ => StatusCode(500, apiError)
            };
        }
    }
}
