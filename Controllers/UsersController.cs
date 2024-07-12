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
        public IActionResult CreateUser(UserDTO userDto)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
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
        public IActionResult GetUsers() 
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
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

        [HttpGet("{id}")]
        public IActionResult GetUser(int id) 
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
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
        public IActionResult UpdateUserProfile(int id, UserDTO userDTO) 
        {

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
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
        public IActionResult DeleteUser(int id) 
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
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
