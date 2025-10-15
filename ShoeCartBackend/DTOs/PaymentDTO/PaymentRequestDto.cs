public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public string BillingStreet { get; set; }
    public string BillingCity { get; set; }
    public string BillingState { get; set; }
    public string BillingZip { get; set; }
    public string BillingCountry { get; set; } = "India";
}