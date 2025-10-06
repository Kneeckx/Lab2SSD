using System.ComponentModel.DataAnnotations;

namespace Lab1SecureSoftware.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int YearsInBusiness { get; set; }

        [Required]
        public string Website { get; set; }

        public string Province { get; set; }

    }
}
