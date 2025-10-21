using ShoeCartBackend.Enums;

namespace ShoeCartBackend.DTOs
{
    public class CreateOrderDto

    {
        public int UserId { get; set; }  

        public PaymentMethod PaymentMethod { get; set; }

        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingCountry { get; set; }
    }
}
