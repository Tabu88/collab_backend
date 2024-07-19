using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using collab_api.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using SystemTask = System.Threading.Tasks.Task;

namespace collab_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase 
    {
        private readonly IConfiguration _configuration;
        

        public TasksController(IConfiguration configuration) 
        {
          _configuration = configuration;
           
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            List<Models.Task> tasks = new List<Models.Task>();
            try
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "SELECT * FROM tasks";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        using (var reader = command.ExecuteReader()) 
                        {
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
                ModelState.AddModelError("Tasks", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);
            }
            return Ok(tasks);
            
        }


        [HttpGet("{userId}")]
        public IActionResult GetTask(string userId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            Models.Task task = new Models.Task();
            try
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "SELECT * FROM tasks WHERE userId=@userId";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (var reader = command.ExecuteReader()) 
                        {
                            if (reader.Read())
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
                            else 
                            {
                                return NotFound();
                            }
                        }
                    
                    
                    }
                
                }

            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("Tasks", $"Sorry we have an exception{ex}");
                return BadRequest(ModelState);
            
            }

            return Ok(task);

        }


        [HttpPost]
        public IActionResult CreateTask(TaskDTO taskDto)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

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

                        command.ExecuteNonQuery();
                    }

                }

            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("Tasks", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);
            }
            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id) 
        {

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "DELETE FROM tasks WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();

                    }

                }
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("Tasks", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }
            return Ok();
               
        
        }

        [HttpPut]
        public IActionResult UpdateTask(int id, TaskDTO taskDto) 
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();

                    string sql = "UPDATE tasks SET title=@title, description=@description, alert=@alert, status=@status, category=@category, deadline=@deadline, userId=@userId WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@title", taskDto.Title);
                        command.Parameters.AddWithValue("@description", taskDto.Description);
                        command.Parameters.AddWithValue("@alert", taskDto.Alert);
                        command.Parameters.AddWithValue("@status", taskDto.Status);
                        command.Parameters.AddWithValue("@category", taskDto.Category);
                        command.Parameters.AddWithValue("@deadline", taskDto.Deadline);
                        command.Parameters.AddWithValue("@userId", taskDto.UsersId);

                        command.ExecuteNonQuery();                    
                    }                
                }  
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Tasks", $"Sorry but we have an exception{ex}");
                return BadRequest(ModelState);

            }
            return Ok();
        
        }
    }
}
