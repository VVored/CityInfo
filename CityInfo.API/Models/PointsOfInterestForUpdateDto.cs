using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointsOfInterestForUpdateDto
    {
        [Required(ErrorMessage = "You should provide name value.")]
        [MaxLength(50, ErrorMessage = "Name value should be shorter.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Description value should be shorter.")]
        public string? Description { get; set; }
    }
}
