namespace sakanat.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApartmentImage
{
    public int Id { get; set; }

    [Required, StringLength(500)]
    public string ImageUrl { get; set; }

    // Foreign key
    public int ApartmentId { get; set; }

    [ForeignKey("ApartmentId")]
    public Apartment Apartment { get; set; }
}
