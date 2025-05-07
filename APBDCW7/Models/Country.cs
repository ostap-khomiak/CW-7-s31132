using System.ComponentModel.DataAnnotations;

namespace APBDCW7.Models;

public class Country
{
    public int IdCountry { get; set; }
    [MaxLength(120)]
    public string? Name { get; set; }
}