
using collab_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
namespace collab_api.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public MessagesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult CreateMessage(MessagesDTO messagesDto) 
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "INSERT INTO messages " +
                        "([from], [to], message) VALUES " +
                        "(@from, @to, @message)";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@from", messagesDto.From);
                        command.Parameters.AddWithValue("@to", messagesDto.To);
                        command.Parameters.AddWithValue("@message", messagesDto.Message);

                        command.ExecuteNonQuery();
                    }                               
                }
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("Messages", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }
            return Ok();      
        
        }

        [HttpDelete("{id}")]                
        public IActionResult DeleteMessage(int id)
        {

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "DELETE FROM messages WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Messages", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }
            return Ok();
        }

        [HttpGet("to")]
        public IActionResult GetReceivedMessages(string to) 
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            List<Models.Messages> messages = new List<Models.Messages>();
            try
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    string sql = "SELECT * FROM messages WHERE [to]=@to";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@to", to);
                        using (var reader = command.ExecuteReader()) 
                        {
                            while (reader.Read()) 
                            {
                                Models.Messages message = new Models.Messages();

                                message.Id = reader.GetInt32(0);
                                message.From = reader.GetString(1);
                                message.To = reader.GetString(2);
                                message.Message = reader.GetString(3);
                                message.SentAt = reader.GetDateTime(4);
                                
                                messages.Add(message);
                            
                            }                     
                        
                        }                  
                    }      
                }
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("Messages", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);
            }
            return Ok(messages);
        }

        [HttpGet("from")]
        public IActionResult GetSentMessages(string from)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            List<Models.Messages> messages = new List<Models.Messages>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM messages WHERE [from]=@from";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@from", from);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Models.Messages message = new Models.Messages();

                                message.Id = reader.GetInt32(0);
                                message.From = reader.GetString(1);
                                message.To = reader.GetString(2);
                                message.Message = reader.GetString(3);
                                message.SentAt = reader.GetDateTime(4);

                                messages.Add(message);

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Messages", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);
            }
            return Ok(messages);
        }

    }
}
