using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.Payment;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PaymentsController : CustomControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var result = await _paymentService.GetPaymentAsync();
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("order/{id:long}")]
        public async Task<IActionResult> GetPaymentByOrderId(long id)
        {
            var result = await _paymentService.GetPaymentByOrderId(id);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateRequest request)
        {
            var result = await _paymentService.AddPayment(request);
            return result.IsSuccess
                ? Created("", null)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(long id)
        {
            var result = await _paymentService.DeletePayment(id);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }
    }

}
