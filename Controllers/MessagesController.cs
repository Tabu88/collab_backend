using collab_api2.Models;
using collab_api2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace collab_api2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly MessagesService _messagesService;

        public MessagesController(IConfiguration configuration, MessagesService messagesService)
        {
            _configuration = configuration;
            _messagesService = messagesService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMessage(MessagesDTO messagesDto)
        {
           MessagesResponseModel responseModel = new MessagesResponseModel();
            try 
            {
                var created = await _messagesService.CreateMessage(messagesDto);
                if (created) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Message created successfully";
                    return Ok(responseModel);              
                
                } else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Service failed";
                    return BadRequest(responseModel);
                
                }           
            } catch (Exception ex) 
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
        public async Task<IActionResult> DeleteMessage(int id)
        {
            MessagesResponseModel responseModel = new MessagesResponseModel();
            try 
            {
                var deleted = await _messagesService.DeletedMessage(id);
                if (deleted) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Message deleted successfully";
                    return Ok(responseModel);
                
                } else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Failed service";
                    return BadRequest(responseModel);
                
                }
            } catch (Exception ex) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);           
            
            }
        }

        [HttpGet("to")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReceivedMessages(string to)
        {
            MessagesResponseModel responseModel = new MessagesResponseModel();
            try 
            {
                (bool success, List<Messages> messages) result = await _messagesService.GetReceivedMessages(to);
                if (result.success) 
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Messages retrieved successfully";
                    responseModel.Messages = result.messages;
                    return Ok(responseModel);                 
                } else 
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Service failed";
                    return BadRequest(responseModel);               
                
                }
            } catch(Exception ex) 
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);
            
            }
        }

        [HttpGet("from")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSentMessages(string from)
        {
            MessagesResponseModel responseModel = new MessagesResponseModel();
            try
            {
                (bool success, List<Messages> messages) result = await _messagesService.GetSentMessages(from);
                if (result.success)
                {
                    responseModel.Status = "Success";
                    responseModel.Message = "Messages retrieved successfully";
                    responseModel.Messages = result.messages;
                    return Ok(responseModel);
                }
                else
                {
                    responseModel.Status = "Failed";
                    responseModel.Message = "Service failed";
                    return BadRequest(responseModel);

                }
            }
            catch (Exception ex)
            {
                responseModel.Status = "Failed";
                responseModel.Message = ex.Message;
                return BadRequest(responseModel);

            }

        }

    }
}
