using System.ComponentModel.DataAnnotations;

namespace CattleManagement.API.DTOs;

public sealed record CreateCattleRequest(
    [Required]
    string Breed,

    [Required]
    string TagNumber,
    
    [Required]
    [DataType(DataType.Date)]
    DateOnly DateOfBirth);
