using Api.Infrastructure.Models;
using BussinesLogic.Services;
using Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers;

[Route("/user")]
public class UserController: BaseApiController<User, UserReadDto, User, UserReadDto, UserReadDto>
{
    public UserController(IService<User, UserReadDto, User, UserReadDto, UserReadDto> service) : base(service)
    {
    }
}