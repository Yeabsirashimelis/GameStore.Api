using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record UpdateGameDto(
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    string Name,

    [Required(ErrorMessage = "Genre is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Genre must be between 1 and 50 characters")]
    string Genre,

    [Range(0.01, 999.99, ErrorMessage = "Price must be between $0.01 and $999.99")]
    decimal Price,

    [Required(ErrorMessage = "Release date is required")]
    DateTime ReleaseDate
);