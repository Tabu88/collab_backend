using collab_api2.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace collab_api2.Services
{
    public class UsersService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<UsersService> _logger;

        public UsersService(IConfiguration config, ILogger<UsersService> logger)
        {
            _config = config;
            _logger = logger;
        }
        public async System.Threading.Tasks.Task CreateUser(UserDTO userDto) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "INSERT INTO users " +
                        "(name, email ,password , gender, profileImage, location) VALUES " +
                        "(@name, @email, @password, @gender, @profileImage, @location)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        _logger.LogInformation("Adding values");
                        command.Parameters.AddWithValue("@name", userDto.Name);
                        command.Parameters.AddWithValue("@email", userDto.Email);
                        command.Parameters.AddWithValue("@password", passwordHash);
                        command.Parameters.AddWithValue("@gender", userDto.Gender);
                        command.Parameters.AddWithValue("@profileImage", userDto.ProfileImage);
                        command.Parameters.AddWithValue("@location", userDto.Location);
                        _logger.LogInformation("Added values");

                        command.ExecuteNonQuery();

                    }
                    _logger.LogInformation("User created");

                }


            }
            catch (Exception ex)
            {

                _logger.LogError($"Error creating user {ex}");
                throw;

            }

        }

        public  List<User> GetUsers() 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            List<User> users = new List<User>();
            try 
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");    

                    string sql = "SELECT * FROM users";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            _logger.LogInformation("Reader executed");
                            while (reader.Read())
                            {
                                User user = new User();

                                user.Id = reader.GetInt32(0);
                                user.Name = reader.GetString(1);
                                user.Email = reader.GetString(2);
                                user.PasswordHash = reader.GetString(3);
                                user.Gender = reader.GetString(4);
                                user.CreatedAt = reader.GetDateTime(5);
                                user.ProfileImage = reader.GetString(6);
                                user.Location = reader.GetString(7);


                                users.Add(user);
                                _logger.LogInformation("Reader adding users");

                            }
                        }
                       
                        _logger.LogInformation("Reader added users");

                    }
                }
            } catch (Exception ex) 
            {

                _logger.LogInformation($"Error getting users: {ex}");
               

            }
            return users;
 
        
        
        }

        public async Task<(int, string)> AuthenticateUser(LoginRequest loginReq) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
           
            int result;
            User user = new User();
            _logger.LogInformation("initialized variables");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE email = @Email ";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", loginReq.Email);
                //command.Parameters.AddWithValue("@Password", loginReq.Password); // Note: Ensure password hashing and secure comparison in production

                await connection.OpenAsync();
                 result = (int)await command.ExecuteScalarAsync();

            }
            if(result == 1)
            {
                _logger.LogInformation("getting user");
                user = await GetUser(loginReq.Email);

                if (!BCrypt.Net.BCrypt.Verify(loginReq.Password, user.PasswordHash))
                {
                    result = 2;

                }
               
            }
            _logger.LogInformation("creating token");
            string token = CreateToken(user);


            return (result, token);


        }

        private string CreateToken(User user) 
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
            };
            _logger.LogInformation("claiming");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                (_config.GetSection("AppSettings:Token").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature );
            _logger.LogInformation("signing credentials");
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );
            _logger.LogInformation("writing token");
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("returning token");
            return jwt;
        

        }

        public async Task<User> GetUser(string email) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            User user = new User();

            try 
            {

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM users WHERE email=@email";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        using (var reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                user.Id = reader.GetInt32(0);
                                user.Name = reader.GetString(1);
                                user.Email = reader.GetString(2);
                                user.PasswordHash = reader.GetString(3);
                                user.Gender = reader.GetString(4);
                                user.CreatedAt = reader.GetDateTime(5);
                                user.ProfileImage = reader.GetString(6);
                                user.Location = reader.GetString(7);

                               

                            }
                        }

                    }

                }

            } catch (Exception ex) 
            {
                _logger.LogInformation($"An exception occurred: {ex}");
                throw;
            }
            return user;
            
        
        
        
        }

        public async Task<bool> UpdateUserProfile(int id, UserDTO userDto) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE users SET name=@name, email=@email, location=@location, gender=@gender, profile_image=@profile_image WHERE id=@id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", userDto.Name);
                        command.Parameters.AddWithValue("@email", userDto.Email);
                        command.Parameters.AddWithValue("@location", userDto.Location);
                        command.Parameters.AddWithValue("@gender", userDto.Gender);
                        command.Parameters.AddWithValue("@profile_image", userDto.ProfileImage);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }

                }


            }
            catch (Exception ex)
            {
                 _logger.LogInformation($"An exception occurred: {ex}");
             
                return false;
            }
            return true;


        }

        public async Task<bool> DeleteUser(int id) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
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

            } catch(Exception ex) 
            {
                _logger.LogInformation($"An exception occurred: {ex}");
                return false;
            }
            return true;
        
        
        
        }


    }
}
