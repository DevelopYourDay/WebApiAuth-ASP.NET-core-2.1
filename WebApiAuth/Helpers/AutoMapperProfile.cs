using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebApiAuth.Dtos.UsersDtos;
using WebApiAuth.Models;


namespace WebApiAuth.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, GetUserDto>();
            CreateMap<GetUserDto, User>();

            CreateMap<User, CreateUserDto>();
            CreateMap<CreateUserDto, User>();

            CreateMap<User, AuthenticateDto>();
            CreateMap<AuthenticateDto, User>();

            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>();

            CreateMap<User, UpdateUserPasswordDto>();
            CreateMap<UpdateUserPasswordDto, User>();
        }
    }
}
