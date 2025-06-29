using AutoMapper;
using Marketplace.API;
using Marketplace.Core.DTOs.Payment;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Core.Response;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Service.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PaymentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<PaymentResponse>>> GetPaymentAsync()
        {
            var payments = await _context.Payments.ToListAsync();
            var mapped = _mapper.Map<IEnumerable<PaymentResponse>>(payments);
            return ServiceResult<IEnumerable<PaymentResponse>>.Success(mapped);
        }

        public async Task<ServiceResult<PaymentResponse>> GetPaymentByOrderId(long id)
        {
            var payments = await _context.Payments
                .Where(x => x.OrderId == id)
                .ToListAsync();

            if (!payments.Any())
                return ServiceResult<PaymentResponse>.Failure("Payment not found", 404);

            var mapped = _mapper.Map<PaymentResponse>(payments);
            return ServiceResult<PaymentResponse>.Success(mapped);
        }

        public async Task<ServiceResult<bool>> AddPayment(PaymentCreateRequest paymentRequest)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == paymentRequest.OrderId);
            if (order == null)
                return ServiceResult<bool>.Failure("Order not found", 404);

            var payment = _mapper.Map<Payment>(paymentRequest);

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 201);
        }

        public async Task<ServiceResult<bool>> DeletePayment(long id)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
            if (payment == null)
                return ServiceResult<bool>.Failure("Payment not found", 404);

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }
    }
}
