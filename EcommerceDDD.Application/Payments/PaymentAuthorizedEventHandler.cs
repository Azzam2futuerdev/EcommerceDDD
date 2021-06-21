﻿using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Payments.Events;
using System.IO;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Services;
using System.Linq;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PaymentAuthorizedEventHandler : INotificationHandler<PaymentAuthorizedEvent>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly IOrderStatusWorkflow _orderStatusWorkflow;

        public PaymentAuthorizedEventHandler(
            IEcommerceUnitOfWork unitOfWork,
            IOrderStatusWorkflow orderStatusWorkflow)
        {
            _unitOfWork = unitOfWork;
            _orderStatusWorkflow = orderStatusWorkflow;
        }

        public async Task Handle(PaymentAuthorizedEvent paymentAuthorizedEvent, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.PaymentRepository.GetPaymentById(paymentAuthorizedEvent.PaymentId, cancellationToken);
            var customer = await _unitOfWork.CustomerRepository.GetCustomerById(payment.CustomerId, cancellationToken);

            var order = customer.Orders.
                Where(o => o.Id == payment.OrderId)
                .FirstOrDefault();

            if (payment == null)
                throw new InvalidDataException("Payment not found.");

            if (order == null)
                throw new InvalidDataException("Order not found.");

            // Changing order status
            _orderStatusWorkflow.CalculateOrderStatus(order, payment);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}