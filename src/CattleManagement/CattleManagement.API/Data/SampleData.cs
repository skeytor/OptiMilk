using CattleManagement.API.Models;

namespace CattleManagement.API.Data;

internal static class SampleData
{
    internal static readonly Cattle[] Cattles =
    [
        new() {
            Id = Guid.Parse("019a5b08-9dcb-784d-8107-923f2907dde5"),
            TagNumber = "A123",
            Breed = "Angus",
            DateOfBirth = new DateOnly(2020, 5, 15)
        },
        new() {
            Id = Guid.Parse("e49e5c17-c74c-43c2-bc96-55a3bae5712b"),
            TagNumber = "B456",
            Breed = "Hereford",
            DateOfBirth = new DateOnly(2019, 8, 22)
        },
        new() {
            Id = Guid.Parse("069f9cc8-d3bd-47b3-8dc2-7e343c90fe30"),
            TagNumber = "C789",
            Breed = "Holstein",
            DateOfBirth = new DateOnly(2021, 3, 10)
        }
    ];
}
