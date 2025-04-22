using Api.Infrastructure.Models;
using BussinesLogic.Services;
using GeneratedDtos;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers;

[Route("api/charger-group")]
public class ChargerGroupController : BaseApiController<ChargerGroup, ChargerGroupReadDto, ChargerGroupReadDetailDto, ChargerGroupCreateDto, ChargerGroupUpdateDto>
{
    public ChargerGroupController(IService<ChargerGroup, ChargerGroupReadDto, ChargerGroupReadDetailDto, ChargerGroupCreateDto, ChargerGroupUpdateDto> service) : base(service)
    {
    }


}