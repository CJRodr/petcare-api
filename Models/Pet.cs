namespace PetcareApi.Models;

public class Pet
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Species { get; set; } = "";
    public int? OwnerId { get; set; }
    public Owner? Owner { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
}
