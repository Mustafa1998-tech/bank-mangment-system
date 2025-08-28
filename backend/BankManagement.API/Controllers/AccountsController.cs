using Microsoft.AspNetCore.Mvc;
using BankManagement.API.DTOs;
using BankManagement.API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BankManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// الحصول على جميع الحسابات مع إمكانية البحث والفلترة
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AccountDto>>>> GetAccounts([FromQuery] AccountSearchDto searchDto)
        {
            try
            {
                var result = await _accountService.GetPagedAccountsAsync(searchDto);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<PagedResult<AccountDto>>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<PagedResult<AccountDto>>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting accounts");
                return StatusCode(500, ApiResponse<PagedResult<AccountDto>>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على حساب محدد بالمعرف
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AccountDto>>> GetAccount(int id)
        {
            try
            {
                var result = await _accountService.GetAccountByIdAsync(id);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<AccountDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<AccountDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account {AccountId}", id);
                return StatusCode(500, ApiResponse<AccountDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على حساب محدد برقم الحساب
        /// </summary>
        [HttpGet("by-number/{accountNumber}")]
        public async Task<ActionResult<ApiResponse<AccountDto>>> GetAccountByNumber(string accountNumber)
        {
            try
            {
                var result = await _accountService.GetAccountByNumberAsync(accountNumber);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<AccountDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<AccountDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account by number {AccountNumber}", accountNumber);
                return StatusCode(500, ApiResponse<AccountDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// إنشاء حساب جديد
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AccountDto>>> CreateAccount([FromBody] CreateAccountDto createAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<AccountDto>.ErrorResponse("بيانات غير صحيحة", errors));
                }

                var result = await _accountService.CreateAccountAsync(createAccountDto);
                
                if (result.IsSuccess)
                {
                    return CreatedAtAction(nameof(GetAccount), new { id = result.Data!.Id }, 
                        ApiResponse<AccountDto>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<AccountDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating account for {OwnerName}", createAccountDto.OwnerName);
                return StatusCode(500, ApiResponse<AccountDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// تحديث بيانات الحساب
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AccountDto>>> UpdateAccount(int id, [FromBody] UpdateAccountDto updateAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<AccountDto>.ErrorResponse("بيانات غير صحيحة", errors));
                }

                var result = await _accountService.UpdateAccountAsync(id, updateAccountDto);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<AccountDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<AccountDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating account {AccountId}", id);
                return StatusCode(500, ApiResponse<AccountDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// حذف حساب
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAccount(int id)
        {
            try
            {
                var result = await _accountService.DeleteAccountAsync(id);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<bool>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting account {AccountId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// تعليق حساب
        /// </summary>
        [HttpPost("{id:int}/suspend")]
        public async Task<ActionResult<ApiResponse<bool>>> SuspendAccount(int id, [FromBody] SuspendAccountDto suspendDto)
        {
            try
            {
                var result = await _accountService.SuspendAccountAsync(id, suspendDto.Reason);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<bool>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while suspending account {AccountId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// تنشيط حساب
        /// </summary>
        [HttpPost("{id:int}/activate")]
        public async Task<ActionResult<ApiResponse<bool>>> ActivateAccount(int id)
        {
            try
            {
                var result = await _accountService.ActivateAccountAsync(id);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<bool>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while activating account {AccountId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// إغلاق حساب
        /// </summary>
        [HttpPost("{id:int}/close")]
        public async Task<ActionResult<ApiResponse<bool>>> CloseAccount(int id, [FromBody] CloseAccountDto closeDto)
        {
            try
            {
                var result = await _accountService.CloseAccountAsync(id, closeDto.Reason);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<bool>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while closing account {AccountId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على رصيد الحساب
        /// </summary>
        [HttpGet("{id:int}/balance")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetAccountBalance(int id)
        {
            try
            {
                var result = await _accountService.GetAccountBalanceAsync(id);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<decimal>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<decimal>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account balance {AccountId}", id);
                return StatusCode(500, ApiResponse<decimal>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// البحث في الحسابات
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AccountDto>>>> SearchAccounts([FromQuery, Required] string searchTerm)
        {
            try
            {
                var result = await _accountService.SearchAccountsAsync(searchTerm);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<IEnumerable<AccountDto>>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<IEnumerable<AccountDto>>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching accounts with term {SearchTerm}", searchTerm);
                return StatusCode(500, ApiResponse<IEnumerable<AccountDto>>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على إحصائيات الحسابات
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<AccountStatisticsDto>>> GetAccountStatistics()
        {
            try
            {
                var result = await _accountService.GetAccountStatisticsAsync();
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<AccountStatisticsDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<AccountStatisticsDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting account statistics");
                return StatusCode(500, ApiResponse<AccountStatisticsDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }
    }

    // Additional DTOs for controller actions
    public class SuspendAccountDto
    {
        [Required(ErrorMessage = "سبب التعليق مطلوب")]
        [StringLength(500, ErrorMessage = "سبب التعليق يجب أن يكون أقل من 500 حرف")]
        public string Reason { get; set; } = string.Empty;
    }

    public class CloseAccountDto
    {
        [Required(ErrorMessage = "سبب الإغلاق مطلوب")]
        [StringLength(500, ErrorMessage = "سبب الإغلاق يجب أن يكون أقل من 500 حرف")]
        public string Reason { get; set; } = string.Empty;
    }
}

