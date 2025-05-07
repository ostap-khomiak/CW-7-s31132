using System.ComponentModel.DataAnnotations;

namespace APBDCW7.Models.DTOs;

public class ClientTripDTO
{
    [Required]
    public int IdTrip { get; set; }
    [MaxLength(120)]
    [Required]
    public string Name { get; set; }
    [MaxLength(220)]
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime DateFrom { get; set; }
    [Required]
    public DateTime DateTo { get; set; }
    [Required]
    public  int MaxPeople { get; set; }
    public int? PaymentDate { get; set; }
    [Required]
    public required int RegisteredAt { get; set; }
}