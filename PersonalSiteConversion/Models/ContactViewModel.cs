using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PersonalSiteConversion.Models
{
    [Keyless]
    public class ContactViewModel
    {
        [Required(ErrorMessage = "* Name is Required")] //* Field is required
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "* Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "* Message is Required")]
        [DataType(DataType.MultilineText)] // makes a "textarea" for this field.
        public string Message { get; set; } = null!;
    }
}
