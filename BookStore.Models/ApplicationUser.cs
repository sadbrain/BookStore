using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;


namespace BookStore.Models;

public class ApplicationUser : IdentityUser
{

    [Required]
    public string Name { get; set; }
    public string? StreetAddress { get; set; }
    public string? StrictAddress { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public int? CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    [ValidateNever]
    public Company Company { get; set; }

}
