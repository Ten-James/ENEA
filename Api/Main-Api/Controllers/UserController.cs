using Api.Infrastructure.Models;
using BussinesLogic.Services;
using GeneratedDtos;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers;

[Route("/user")]
public class UserController : BaseApiController<User, UserReadDto, UserReadDetailDto, UserCreateDto, UserUpdateDto>
{
    public UserController(IService<User, UserReadDto, UserReadDetailDto, UserCreateDto, UserUpdateDto> service) : base(service)
    {
    }
}