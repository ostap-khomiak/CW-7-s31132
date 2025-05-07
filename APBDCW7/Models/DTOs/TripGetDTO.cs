using System.ComponentModel.DataAnnotations;

namespace APBDCW7.Models.DTOs;

public class TripGetDTO
{
    public required int IdTrip { get; set; }
    [MaxLength(120)]
    public required string Name { get; set; }
    [MaxLength(220)]
    public required string Description { get; set; }
    public required DateTime DateFrom { get; set; }
    public required DateTime DateTo { get; set; }
    public required int MaxPeople { get; set; }
    public required List<Country> Countries { get; set; }
}