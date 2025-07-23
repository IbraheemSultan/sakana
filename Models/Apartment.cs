namespace sakanat.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Apartment
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required, StringLength(200)]
    public string Address { get; set; }

    [Required, Range(1, 20)]
    public int RoomCount { get; set; }

    [Required]
    public string PhoneNumper { get; set; }
    [Required]
    public GenderType GenderType { get; set; } // فلتر بنات/ولاد

    public DateTime CreatedAt { get; set; }

    // Navigation
    public List<ApartmentImage> Images { get; set; }

    public Apartment()
    {
        CreatedAt = DateTime.Now;
        Images = new List<ApartmentImage>();
    }
}
