using System.ComponentModel.DataAnnotations;

namespace CattleManagement.API.DTOs;

public sealed record UpdateCattleRequest(
    
    [Required]
    string Breed,

    [Required]
    string TagNumber,

    [Required]
    [DataType(DataType.Date)]
    DateOnly DateOfBirth
);