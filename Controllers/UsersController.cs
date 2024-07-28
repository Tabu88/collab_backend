using collab_api2.Models;
using collab_api2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace collab_api2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly UsersService _usersService;

        public UsersController(IConfiguration configuration, UsersService usersService)
        {
            _configuration = configuration;
            _usersService = usersService;

        }

        [HttpPost("createUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser(UserDTO userDto)
        {
            ResponseModel responseModel = new ResponseModel();
            if(userDto == null) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = "User Data cannot be empty";

                return BadRequest(responseModel);       
            }
            try
            {
                await _usersService.CreateUser(userDto);
                responseModel.Status = "Success";
                responseModel.Message = "User created successfully";
                return Ok(responseModel);


            }
            catch (Exception ex)
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;

                return BadRequest(responseModel);

            }
        }

        [HttpGet("getAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers()
        {
            ResponseModel responseModel = new ResponseModel();  
           
            try
            {
                 var users = _usersService.GetUsers();
                responseModel.Status = "Success";
                responseModel.Users = users;
                return Ok(responseModel);
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Users", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }
            
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            ResponseModel responseModel = new ResponseModel();
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                responseModel.Status = "Failed";
                responseModel.Message = "Invalid login request";

                return BadRequest(responseModel);
            }

            try
            {
                (int result, string token) auth = await _usersService.AuthenticateUser(loginRequest.Email, loginRequest.Password);
                if (auth.result == 1)
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Login successful";
                    responseModel.Token = auth.token;
                    return Ok(responseModel);
                } else if (auth.result == 0) 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "User not found";
                    return Unauthorized(responseModel);               
                
                }
                else
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Invalid email or password";
                    return Unauthorized(responseModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.Status = "Failed";
                responseModel.Message = "An internal server error occurred";
                return BadRequest(responseModel);
            }
        }
         

        [HttpGet("getUser/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser(string email)
        {
            ResponseModel responseModel = new ResponseModel();
            
            try
            {
                var user = await _usersService.GetUser(email);
               return Ok(user);
                

            }
            catch (Exception ex)
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);

            }    
        }

        [HttpPut("updateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserProfile(int id, UserDTO userDto)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                var update = _usersService.UpdateUserProfile(id, userDto);
               
            }
            catch (Exception ex)
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);
                
            }
            responseModel.Status = "Success";
            responseModel.Message = "Successfully updated user";
            return Ok(responseModel);
        }

        [HttpDelete("deleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                var deleted = await _usersService.DeleteUser(id);
                if (deleted)
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "User deleted successfully";
                    return Ok(responseModel);

                } else 
                {
                    responseModel = new ResponseModel();
                    responseModel.Status = "Failed";
                    responseModel.Message = "An error occurred";
                    return BadRequest(responseModel);
                
                
                }

            }
            catch (Exception ex)
            {
                responseModel = new ResponseModel();
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);
                
            }
            

        }
    }
}
