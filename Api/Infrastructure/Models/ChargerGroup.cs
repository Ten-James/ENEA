using Microsoft.EntityFrameworkCore;
using Api.Infrastructure.Models.Configuration;

namespace Api.Infrastructure.Models;

[EntityTypeConfiguration(typeof(ChargerGroupConfiguration))]
public class ChargerGroup: EntityBase
{

}