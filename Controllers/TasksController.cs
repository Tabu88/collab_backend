using collab_api2.Models;
using collab_api2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


using SystemTask = System.Threading.Tasks.Task;

namespace collab_api2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TasksService _tasksService;


        public TasksController(IConfiguration configuration, TasksService tasksService)
        {
            _configuration = configuration;
            _tasksService = tasksService;


        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTasks()
        {
            TaskResponseModel responseModel = new TaskResponseModel();
            try
            {
                (bool success, List<Models.Task> tasks) result = await _tasksService.GetAllTasks();
                if (result.success) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Tasks retrieved successfully";
                    responseModel.Tasks = result.tasks;
                    return Ok(responseModel);              
                
                } else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Failed service";
                    return BadRequest(responseModel);                
                
                }
            } catch(Exception ex) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);
            }
            

        }


        [HttpGet("GetUserTasks/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTask(string userId)
        {
            TaskResponseModel responseModel = new TaskResponseModel();
            try 
            {
                (bool success,List<Models.Task> tasks) result = await _tasksService.GetUserTasks(userId);
                if (result.success == true) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Task retrieved successfully";
                    responseModel.Tasks = result.tasks;
                   

                }
                else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Failed service";
                    
                
                
                }
            
            
            } catch (Exception ex) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);
            
            
            }
            return Ok(responseModel);


        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTask(TaskDTO taskDto)
        {
            UserResponseModel responseModel = new UserResponseModel();
            if(taskDto == null) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = "Task data cannot be empty";
                return BadRequest(responseModel);
            
            }
            try 
            {
                var created = await _tasksService.CreateTask(taskDto);
                if(created) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Tasks created successfully";
                    return Ok(responseModel);
                
                } else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Service failed";
                    return BadRequest(responseModel);
                
                
                }
            
            
            }catch (Exception ex) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);
            
            
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            TaskResponseModel responseModel = new TaskResponseModel();
            try 
            {
                var deleted = await _tasksService.RemoveTask(id);
                if (deleted) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Task removed successfully";
                    return Ok(responseModel);
                }
                else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Failed service";
                    return BadRequest(responseModel);
                
                
                }
            
            
            
            }catch(Exception ex) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);           
            
            }

           


        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTask(int id, TaskDTO taskDto)
        {
            TaskResponseModel responseModel = new TaskResponseModel();
            try 
            {
                var updated = await _tasksService.UpdateTask(id, taskDto);
                if (updated) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Task updated successfully";
                    return Ok(responseModel);

                }
                else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Failed Service";
                    return BadRequest(responseModel);
                
                }           
            }  catch(Exception ex) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);
            
            
            }

        }
    }
}
