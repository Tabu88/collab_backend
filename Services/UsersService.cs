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
            string connectionString = _config.GetConnectionString("HostedConnection");
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "INSERT INTO users " +
                        "(name, email ,password ,profile_image, location) VALUES " +
                        "(@name, @email, @password, @profile_image, @location)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        _logger.LogInformation("Adding values");
                        command.Parameters.AddWithValue("@name", userDto.Name);
                        command.Parameters.AddWithValue("@email", userDto.Email);
                        command.Parameters.AddWithValue("@password", passwordHash);
                        command.Parameters.AddWithValue("@profile_image", userDto.ProfileImage);
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
            string connectionString = _config.GetConnectionString("HostedConnection");
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
                                user.CreatedAt = reader.GetDateTime(4);
                                user.ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : (byte[])reader["profile_image"];

                                user.Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(6);


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

        public async Task<(int, string)> AuthenticateUser(string email, string password) 
        {
            string connectionString = _config.GetConnectionString("HostedConnection");
           
            int result;
            User user = new User();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password); // Note: Ensure password hashing and secure comparison in production

                await connection.OpenAsync();
                 result = (int)await command.ExecuteScalarAsync();

            }
             user = await GetUser(email);

            if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            { 
                result = 2;
                
            }
            string token = CreateToken(user);

            var tuple = (result,token);
            return (result, token);


        }

        private string CreateToken(User user) 
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                (_config.GetSection("AppSettings:Token").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        

        }

        public async Task<User> GetUser(string email) 
        {
            string connectionString = _config.GetConnectionString("HostedConnection");
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
                                user.CreatedAt = reader.GetDateTime(4);
                                user.ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : (byte[])reader["profile_image"];
                                user.Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location"));

                               

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
            string connectionString = _config.GetConnectionString("HostedConnection");
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
            string connectionString = _config.GetConnectionString("HostedConnection");
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
