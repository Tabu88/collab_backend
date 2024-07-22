using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using collab_api.Models;


namespace collab_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
       
        private readonly IConfiguration _configuration;
      
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
                
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateUser(UserDTO userDto)
        {
            string connectionString = _configuration.GetConnectionString("HostedConnection");
            try 
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "INSERT INTO users " +
                        "(name, email ,password ,profile_image, location) VALUES " +
                        "(@name, @email, @password, @profile_image, @location)";

                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@name", userDto.Name);
                        command.Parameters.AddWithValue("@email", userDto.Email);
                        command.Parameters.AddWithValue("@password", userDto.Password);
                        command.Parameters.AddWithValue("@profile_image", userDto.ProfileImage);
                        command.Parameters.AddWithValue("@location", userDto.Location);

                        command.ExecuteNonQuery();
                    }
                
                }


            } catch (Exception ex) 
            {
                ModelState.AddModelError("Users",$"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUsers() 
        {
            string connectionString = _configuration.GetConnectionString("HostedConnection");
            List<User> users = new List<User>();
            try 
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "SELECT * FROM users";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        using (var reader = command.ExecuteReader()) 
                        {
                            while (reader.Read()) 
                            {
                                User user = new User(); 

                                user.Id = reader.GetInt32(0);
                                user.Name = reader.GetString(1);
                                user.Email = reader.GetString(2);
                                user.Password = reader.GetString(3);
                                user.CreatedAt = reader.GetDateTime(4);
                                user.ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : (byte[])reader["profile_image"];
 
                                user.Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(6);
                                  

                                users.Add(user);
                            
                            }
                        }
                    
                    }
                }
            } catch (Exception ex) 
            {
                ModelState.AddModelError("Users", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login( LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid login request");
            }

            try
            {
                bool isAuthenticated = await AuthenticateUser(loginRequest.Email, loginRequest.Password);
                if (isAuthenticated)
                {
                    return Ok("Login successful");
                }
                else
                {
                    return Unauthorized("Invalid email or password");
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "An internal server error occurred");
            }
        }

        private async Task<bool> AuthenticateUser(string email, string password)
        {
            string connectionString = _configuration.GetConnectionString("HostedConnection");
            bool isAuthenticated = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Note: Ensure password hashing and secure comparison in production

                await connection.OpenAsync();
                int result = (int)await command.ExecuteScalarAsync();

                isAuthenticated = result > 0;
            }
            return isAuthenticated;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUser(int id) 
        {
            string connectionString = _configuration.GetConnectionString("HostedConnection");
            User user = new User();

            try 
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM users WHERE id=@id";
                    using(var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@id", id);
                    
                        using (var reader = command.ExecuteReader()) 
                        {

                            if(reader.Read()) 
                            {
                                user.Id = reader.GetInt32(0);
                                user.Name = reader.GetString(1);
                                user.Email = reader.GetString(2);
                                user.Password = reader.GetString(3);
                                user.CreatedAt = reader.GetDateTime(4);
                                user.ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : (byte[])reader["profile_image"];
                                user.Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location"));

                            } else 
                            {
                                return NotFound();
                            
                            }

                        
                        
                        }

                    }

                }
            
            } catch(Exception ex) 
            {
                ModelState.AddModelError("Users", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }

            return Ok(user);
        
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateUserProfile(int id, UserDTO userDTO) 
        {

            string connectionString = _configuration.GetConnectionString("HostedConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "UPDATE users SET name=@name, email=@email, location=@location, profile_image=@profile_image WHERE id=@id";
                        
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@name",userDTO.Name);
                        command.Parameters.AddWithValue("@email",userDTO.Email);
                        command.Parameters.AddWithValue("@location",userDTO.Location);
                        command.Parameters.AddWithValue("@profile_image",userDTO.ProfileImage);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();                
                    }             
                
                }


            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("Users", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteUser(int id) 
        {
            string connectionString = _configuration.GetConnectionString("HostedConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "DELETE FROM users WHERE id=@id";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }

                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Users", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);
            }
            return Ok();

        }
    }
}
