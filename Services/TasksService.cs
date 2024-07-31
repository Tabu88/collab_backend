using collab_api2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using SystemTask = System.Threading.Tasks.Task ;

namespace collab_api2.Services
{
    public class TasksService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TasksService> _logger;
        private readonly SubtasksService _subService;

        public TasksService(IConfiguration config, ILogger<TasksService> logger, SubtasksService subService)
        {
            _config = config;
            _logger = logger;
            _subService = subService;
        }

        public async System.Threading.Tasks.Task<bool> CreateTask(TaskDTO taskDto) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection Opened");

                    string sql = "INSERT INTO tasks " +
                        "(title, description, alert, status, category, deadline, userId) VALUES " +
                        "(@title, @description, @alert, @status, @category, @deadline, @userId)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", taskDto.Title);
                        command.Parameters.AddWithValue("@description", taskDto.Description);
                        command.Parameters.AddWithValue("@alert", taskDto.Alert);
                        command.Parameters.AddWithValue("@status", taskDto.Status);
                        command.Parameters.AddWithValue("@category", taskDto.Category);
                        command.Parameters.AddWithValue("@deadline", taskDto.Deadline);
                        command.Parameters.AddWithValue("@userId", taskDto.UsersId);
                        _logger.LogInformation("Added values");

                        command.ExecuteNonQuery();
                    }
                    _logger.LogInformation("Task created");
                    // get task id
                    (bool success, int id) result = await GetTaskId(taskDto.Title);
                    if(result.success) 
                    {
                        foreach (var subtask in taskDto.Subtasks)
                        {
                            var created = await _subService.CreateSubtask(subtask, result.id);
                            if (!created)
                            {
                                _logger.LogError($"Error creating subtask {subtask.Subtitle}");
                                return false;

                            }

                        }

                    } else 
                    {
                        _logger.LogInformation("Error fetching taskId ");
                        return false;
                    
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating task {ex}");
                return false;

            }
            return true;

        }

        private async Task<(bool, int)> GetTaskId(string title) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            Models.Task task = new Models.Task();

            try 
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "SELECT * FROM tasks WHERE title=@title";
                    using( var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@title", title);
                        using (var reader = command.ExecuteReader()) 
                        {
                            if(reader.Read()) 
                            {
                                task.Id = reader.GetInt32(0);
                                task.Title = reader.GetString(1);
                                task.Description = reader.GetString(2);
                                task.Alert = reader.GetString(3);
                                task.Status = reader.GetString(4);
                                task.Category = reader.GetString(5);
                                task.Deadline = reader.GetDateTime(6);
                                task.UserId = reader.GetString(7);
                                task.CreatedAt = reader.GetDateTime(8);
                            
                            }
                        }
                    }                
                }           
            
            } catch (Exception ex) 
            {
                _logger.LogError($"An exception occurred: {ex}");
                return (false, 0);           
            }
            return (true, task.Id);


        
        
        }

        public async Task<(bool, List<Models.Task>)> GetUserTasks(string userId) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            List<Models.Task> tasks = new List<Models.Task>();
            List<String> users = new List<String>(userId.Split('|'));
            Models.Task task = new Models.Task();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                   

                    connection.Open();
                    //_logger.LogInformation("Connection opened");


                    string sql = "SELECT * FROM tasks WHERE userId=@userId";
                    foreach (var user in users)
                    {
                        using (var command = new SqlCommand(sql, connection))
                        {
                       
                            command.Parameters.AddWithValue("@userId", userId);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    

                                    task.Id = reader.GetInt32(0);
                                    task.Title = reader.GetString(1);
                                    task.Description = reader.GetString(2);
                                    task.Alert = reader.GetString(3);
                                    task.Status = reader.GetString(4);
                                    task.Category = reader.GetString(5);
                                    task.Deadline = reader.GetDateTime(6);
                                    task.UserId = reader.GetString(7);
                                    task.CreatedAt = reader.GetDateTime(8);

                                    _logger.LogInformation("Reading values");

                                    (bool success, List<Subtask> subtasks) result = await _subService.GetSubtasks(task.Id);

                                    if (!result.success)
                                    {
                                        _logger.LogInformation("Failed fetching subtasks");
                                        return (false, null);


                                    }
                                    task.Subtasks = result.subtasks;
                                    tasks.Add(task);





                                }

                            }
                            



                        }
                        
                    }
                    

                }
                

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"An exception occurred{ex}");
                return (false, null);

            }

            return (true , tasks);



        }

        public async Task<(bool, List<Models.Task>)> GetAllTasks() 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            List<Models.Task> tasks = new List<Models.Task>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "SELECT * FROM tasks";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            _logger.LogInformation("Reading values");
                            while (reader.Read())
                            {
                                Models.Task task = new Models.Task();

                                task.Id = reader.GetInt32(0);
                                task.Title = reader.GetString(1);
                                task.Description = reader.GetString(2);
                                task.Alert = reader.GetString(3);
                                task.Status = reader.GetString(4);
                                task.Category = reader.GetString(5);
                                task.Deadline = reader.GetDateTime(6);
                                task.UserId = reader.GetString(7);
                                task.CreatedAt = reader.GetDateTime(8);

                                tasks.Add(task);


                            }


                        }



                    }


                }


            }
            catch (Exception ex)
            {
                _logger.LogInformation($"An exception occurred{ex}");
                return (false, null);
            }
            return (true, tasks);

        }

        public async Task<bool> RemoveTask(int id) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Connection opened");

                    string sql = "DELETE FROM tasks WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();

                        var deleted = await _subService.RemoveSubtask(id);
                        if(!deleted) 
                        {
                            _logger.LogInformation("Failed to remove subtasks");
                            return false;
                        
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"An exception occurred:{ex}");
                return false;

            }
            return true;


        }

        public async Task<bool> UpdateTask(int id, TaskDTO taskDto) 
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE tasks SET title=@title, description=@description, alert=@alert, status=@status, category=@category, deadline=@deadline, userId=@userId WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@title", taskDto.Title);
                        command.Parameters.AddWithValue("@description", taskDto.Description);
                        command.Parameters.AddWithValue("@alert", taskDto.Alert);
                        command.Parameters.AddWithValue("@status", taskDto.Status);
                        command.Parameters.AddWithValue("@category", taskDto.Category);
                        command.Parameters.AddWithValue("@deadline", taskDto.Deadline);
                        command.Parameters.AddWithValue("@userId", taskDto.UsersId);

                        foreach(var subtask in taskDto.Subtasks) 
                        {
                            var updated = await _subService.UpdateSubtask(id, subtask);
                            if (!updated) 
                            {
                                _logger.LogError("Failed to update subtasks");
                                return false;
                            
                            }
                        
                        
                        }
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
    }
}
