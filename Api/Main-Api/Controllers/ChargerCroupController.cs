using Api.Infrastructure.Models;
using BussinesLogic.Services;
using GeneratedDtos;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers;

[Route("/charger-group")]
public class ChargerCroupController : BaseApiController<ChargerGroup, ChargerGroupReadDto, ChargerGroupReadDetailDto, ChargerGroupCreateDto, ChargerGroupUpdateDto>
{
    public ChargerCroupController(IService<ChargerGroup, ChargerGroupReadDto, ChargerGroupReadDetailDto, ChargerGroupCreateDto, ChargerGroupUpdateDto> service) : base(service)
    {
    }
}