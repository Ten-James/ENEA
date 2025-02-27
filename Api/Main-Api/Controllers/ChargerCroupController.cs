using Api.Infrastructure.Models;
using BussinesLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers;

[Route("/charger-group")]
public class ChargerCroupController: BaseApiController<ChargerGroup, ChargerGroup, ChargerGroup, ChargerGroup, ChargerGroup>
{
    public ChargerCroupController(IService<ChargerGroup, ChargerGroup, ChargerGroup, ChargerGroup, ChargerGroup> service) : base(service)
    {}
}