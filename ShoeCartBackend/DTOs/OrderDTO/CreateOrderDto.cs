using ShoeCartBackend.Enums;

namespace ShoeCartBackend.DTOs
{
    public class CreateOrderDto
    {
        // Payment method: COD or Razorpay
        public PaymentMethod PaymentMethod { get; set; }

        // Billing address
        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCountry { get; set; }
    }
}
