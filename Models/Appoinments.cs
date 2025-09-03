namespace PetcareApi.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateTime When { get; set; }
    public string Reason { get; set; } = "";
}
