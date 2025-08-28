using Microsoft.AspNetCore.Mvc;
using BankManagement.API.DTOs;
using BankManagement.API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BankManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// إيداع أموال في حساب
        /// </summary>
        [HttpPost("accounts/{accountId:int}/deposit")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> Deposit(int accountId, [FromBody] DepositDto depositDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<TransactionDto>.ErrorResponse("بيانات غير صحيحة", errors));
                }

                var result = await _transactionService.DepositAsync(accountId, depositDto);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<TransactionDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<TransactionDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during deposit for account {AccountId}", accountId);
                return StatusCode(500, ApiResponse<TransactionDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// سحب أموال من حساب
        /// </summary>
        [HttpPost("accounts/{accountId:int}/withdraw")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> Withdraw(int accountId, [FromBody] WithdrawDto withdrawDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<TransactionDto>.ErrorResponse("بيانات غير صحيحة", errors));
                }

                var result = await _transactionService.WithdrawAsync(accountId, withdrawDto);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<TransactionDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<TransactionDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during withdrawal for account {AccountId}", accountId);
                return StatusCode(500, ApiResponse<TransactionDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// تحويل أموال بين الحسابات
        /// </summary>
        [HttpPost("accounts/{fromAccountId:int}/transfer")]
        public async Task<ActionResult<ApiResponse<TransferResultDto>>> Transfer(int fromAccountId, [FromBody] TransferDto transferDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<TransferResultDto>.ErrorResponse("بيانات غير صحيحة", errors));
                }

                var result = await _transactionService.TransferAsync(fromAccountId, transferDto);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<TransferResultDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<TransferResultDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during transfer from account {FromAccountId}", fromAccountId);
                return StatusCode(500, ApiResponse<TransferResultDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على معاملة محددة بالمعرف
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> GetTransaction(int id)
        {
            try
            {
                var result = await _transactionService.GetTransactionByIdAsync(id);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<TransactionDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<TransactionDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction {TransactionId}", id);
                return StatusCode(500, ApiResponse<TransactionDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على معاملة محددة برقم المعاملة
        /// </summary>
        [HttpGet("by-transaction-id/{transactionId}")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> GetTransactionByTransactionId(string transactionId)
        {
            try
            {
                var result = await _transactionService.GetTransactionByTransactionIdAsync(transactionId);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<TransactionDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<TransactionDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction by transaction id {TransactionId}", transactionId);
                return StatusCode(500, ApiResponse<TransactionDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على معاملات حساب محدد
        /// </summary>
        [HttpGet("accounts/{accountId:int}")]
        public async Task<ActionResult<ApiResponse<PagedResult<TransactionDto>>>> GetAccountTransactions(
            int accountId, [FromQuery] TransactionSearchDto searchDto)
        {
            try
            {
                var result = await _transactionService.GetPagedTransactionsByAccountAsync(accountId, searchDto);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<PagedResult<TransactionDto>>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<PagedResult<TransactionDto>>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transactions for account {AccountId}", accountId);
                return StatusCode(500, ApiResponse<PagedResult<TransactionDto>>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على المعاملات الأخيرة
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionDto>>>> GetRecentTransactions([FromQuery] int count = 10)
        {
            try
            {
                var result = await _transactionService.GetRecentTransactionsAsync(count);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<IEnumerable<TransactionDto>>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting recent transactions");
                return StatusCode(500, ApiResponse<IEnumerable<TransactionDto>>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على إحصائيات المعاملات
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<TransactionStatisticsDto>>> GetTransactionStatistics(
            [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _transactionService.GetTransactionStatisticsAsync(startDate, endDate);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<TransactionStatisticsDto>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<TransactionStatisticsDto>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transaction statistics");
                return StatusCode(500, ApiResponse<TransactionStatisticsDto>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// إلغاء معاملة
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelTransaction(int id, [FromBody] CancelTransactionDto cancelDto)
        {
            try
            {
                var result = await _transactionService.CancelTransactionAsync(id, cancelDto.Reason);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<bool>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling transaction {TransactionId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// الحصول على المعاملات المعلقة
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionDto>>>> GetPendingTransactions()
        {
            try
            {
                var result = await _transactionService.GetPendingTransactionsAsync();
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(result.Data!, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<IEnumerable<TransactionDto>>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting pending transactions");
                return StatusCode(500, ApiResponse<IEnumerable<TransactionDto>>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// معالجة معاملة معلقة
        /// </summary>
        [HttpPost("{id:int}/process")]
        public async Task<ActionResult<ApiResponse<bool>>> ProcessPendingTransaction(int id)
        {
            try
            {
                var result = await _transactionService.ProcessPendingTransactionAsync(id);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<bool>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing pending transaction {TransactionId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }

        /// <summary>
        /// حساب رسوم المعاملة
        /// </summary>
        [HttpGet("calculate-fee")]
        public async Task<ActionResult<ApiResponse<decimal>>> CalculateTransactionFee(
            [FromQuery, Required] string transactionType, [FromQuery, Required] decimal amount)
        {
            try
            {
                var result = await _transactionService.CalculateTransactionFeeAsync(transactionType, amount);
                
                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<decimal>.SuccessResponse(result.Data, result.Message));
                }

                return StatusCode(result.StatusCode, ApiResponse<decimal>.ErrorResponse(result.Message, result.Errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating transaction fee");
                return StatusCode(500, ApiResponse<decimal>.ErrorResponse("حدث خطأ داخلي في الخادم"));
            }
        }
    }

    // Additional DTO for controller actions
    public class CancelTransactionDto
    {
        [Required(ErrorMessage = "سبب الإلغاء مطلوب")]
        [StringLength(500, ErrorMessage = "سبب الإلغاء يجب أن يكون أقل من 500 حرف")]
        public string Reason { get; set; } = string.Empty;
    }
}

