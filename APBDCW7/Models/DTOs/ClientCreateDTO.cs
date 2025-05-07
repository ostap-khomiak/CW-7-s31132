using System.ComponentModel.DataAnnotations;

namespace APBDCW7.Models.DTOs;

public class ClientCreateDTO
{
    [MaxLength(120)]
    [Required]
    public string FirstName { get; set; }
    [MaxLength(120)]
    [Required]
    public string LastName { get; set; }
    [MaxLength(120)]
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [MaxLength(120)]
    [Phone]
    [Required]
    public string Telephone { get; set; }
    [MaxLength(120)]
    [Required]
    public string Pesel { get; set; }
    
    
    
    
}