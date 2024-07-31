using collab_api2.Models;
using Microsoft.Data.SqlClient;

namespace collab_api2.Services
{
    public class SubtasksService
    {

        private readonly IConfiguration _config;
        private readonly ILogger<SubtasksService> _logger;

        public SubtasksService(IConfiguration config, ILogger<SubtasksService> logger)
        {
            _logger = logger;
            _config = config;
            
        }

        public async Task<bool> CreateSubtask(SubtaskDTO subtaskDTO, int taskId) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection Opened");

                    string sql = "INSERT INTO subtasks " +
                        "(subtitle, checked, userId, taskId ) VALUES " +
                        "(@subtitle, @checked, @userId, @taskId )";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@subtitle", subtaskDTO.Subtitle);
                        command.Parameters.AddWithValue("@checked", subtaskDTO.Checked);
                        command.Parameters.AddWithValue("@userId", subtaskDTO.UserId);
                        command.Parameters.AddWithValue("@taskId", taskId);
                      
                        _logger.LogInformation("Added values");

                        command.ExecuteNonQuery();
                    }
                    _logger.LogInformation("Subtask created");

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating subtask {ex}");
                return false;

            }
            return true;



        }

        public async Task<(bool,List<Subtask>)> GetSubtasks(int taskId) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            List<Subtask> subtasks = new List<Subtask>();
            try 
            {
                using( var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "SELECT * FROM subtasks WHERE taskId=@taskId";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@taskId", taskId);
                        using (var reader = command.ExecuteReader()) 
                        {
                            while (reader.Read()) 
                            {
                                Subtask subtask = new Subtask();

                                subtask.Id = reader.GetInt32(0);
                                subtask.Subtitle = reader.GetString(1);
                                subtask.Checked = reader.GetString(2);
                                subtask.UserId = reader.GetString(3);
                                subtask.TaskId = reader.GetString(4);
                                subtask.CreatedAt = reader.GetDateTime(5);

                                _logger.LogInformation("Reading values");

                                subtasks.Add(subtask);

                                _logger.LogInformation("Adding values");
                            }                       
                        }                    
                    }
                
                
                }
            
            } catch (Exception ex) 
            {
                _logger.LogInformation($"An exception occurred: {ex}");
                return (false, null);           
            
            
            }
            return (true, subtasks);
        }

        public async Task<bool> RemoveSubtask(int taskId) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try 
            {
                using( var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "DELETE FROM subtasks WHERE taskId=@taskId";
                    using( var command = new SqlCommand(sql, connection)) 
                    {
                       command.Parameters.AddWithValue("taskId", taskId);
                        command.ExecuteNonQuery();
                    
                    }
                
                }
            } 
            catch (Exception ex) 
            {
                _logger.LogError($"An exception occurred: {ex}");
                return false;
            
            
            }
            return true;
        
        
        
        
        }

        public async Task<bool> UpdateSubtask(int taskId,SubtaskDTO subtaskDTO) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try 
            {
                using( var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "UPDATE subtasks SET subtitle=@subtitle, checked=@checked, userId=@userId WHERE taskId=@taskId";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@taskId", taskId);
                        command.Parameters.AddWithValue("@subtitle", subtaskDTO.Subtitle);
                        command.Parameters.AddWithValue("@checked", subtaskDTO.Checked);
                        command.Parameters.AddWithValue("@userId", subtaskDTO.UserId); 
                        
                        command.ExecuteNonQuery();

                    }
                }
            } 
            catch (Exception ex) 
            {
                _logger.LogInformation($"An exception occurred :{ex}");
                return false;
            
            
            }
            return true;
        
        
        
        }
    }
}
