using collab_api2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace collab_api2.Services
{
    public class MessagesService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MessagesService> _logger;

        public MessagesService(IConfiguration config, ILogger<MessagesService> logger)
        {
            _config = config;
            _logger = logger;
            
        }
        public async Task<bool> CreateMessage(MessagesDTO messagesDto) 
        {
            string connectionString = _config.GetConnectionString("HostedConnection");
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
               _logger.LogInformation($"An exception occurred: {ex}");
                return false;

            }
            return true;



        }

        public async Task<bool> DeletedMessage(int id) 
        {
            string connectionString = _config.GetConnectionString("HostedConnection");
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
                _logger.LogInformation($"An exception occurred: {ex}");
                return false;

            }
            return true;




        }

        public async Task<(bool, List<Messages>)> GetReceivedMessages(string to) 
        {

            string connectionString = _config.GetConnectionString("HostedConnection");
            List<Messages> messages = new List<Models.Messages>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "SELECT * FROM messages WHERE [to]=@to";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@to", to);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Messages message = new Messages();

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
                _logger.LogInformation($"Sorry but we have an exception{ex}");
                return (false, null);
            }
            return (true, messages);


        }

        public async Task<(bool, List<Messages>)> GetSentMessages(string from) 
        {
            string connectionString = _config.GetConnectionString("HostedConnection");
            List<Messages> messages = new List<Messages>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "SELECT * FROM messages WHERE [from]=@from";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@from", from);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Messages message = new Messages();

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
                _logger.LogInformation($"An exception occurred: {ex}");
                return (false, null);
            }
            return (true, messages);
        }
    }
}
