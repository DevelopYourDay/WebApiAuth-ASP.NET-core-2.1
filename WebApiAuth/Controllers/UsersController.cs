using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApiAuth.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApiAuth.Services;
using WebApiAuth.Dtos.UsersDtos;
using WebApiAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace WebApiAuth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private UserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;


        public UsersController(
            UserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateDto authenticateDTO)
        {
            var user = _userService.Authenticate(authenticateDTO.Email, authenticateDTO.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]CreateUserDto createUserDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(createUserDto);

            try
            {
                // save 
                _userService.Create(user, createUserDto.Password);
                return Ok(new { message = "Utilizador Registado com Sucesso!" });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = _mapper.Map<IList<GetUserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<GetUserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UpdateUserDto updateUserDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(updateUserDto);
            user.Id = id;

            try
            {
                // save 
                _userService.Update(user);
                return Ok(new { message = "Dados do utilizador alterados com Sucesso!" });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("updatePassword/{id}")]
        public IActionResult UpdatePassword(int id, [FromBody]UpdateUserPasswordDto updateUserPasswordDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(updateUserPasswordDto);
            user.Id = id;

            try
            {
                // save 
                _userService.UpdatePassword(user, updateUserPasswordDto.NewPassword);
                return Ok(new { message = "Password alterada com sucesso!" });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            // a token can not be destroyed. Its duration can be reduced before it is created to expire more quickly.
            // To deny access to a token already created the only solution will be to place the 
            //token in a blackList and cause the user to generate a new token.

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            try
            {
                if (_userService.Logout(accessToken) != null)
                {
                    return Ok();
                }

            }catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            return Ok();
        }

    }
}