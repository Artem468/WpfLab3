namespace Wpflab3.Models;

public class RegistrationProduct
{
    public long RegistrationProductId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string BuilderLastname { get; set; }
    public long ProductCode { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
}