using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]    //action filter will apply to all method calls inside UserController
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase{
        readonly IDatingRepository _repo;
        readonly IMapper _mapper;

        public MessagesController(IDatingRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
             //check to see if user is the current user thats passed token up to server attempting to access this route
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))    
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null)
            {
                return NotFound();
            }

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            //check to see if user is the current user thats passed token up to server attempting to access this route
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))    
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            //check to see if user is the current user thats passed token up to server attempting to access this route
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))    
            {
                return Unauthorized();
            }

            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
             //NOTE** automapper will take the sender var from memory and use it to automap into message, even though we didnt add it ourselves to the message!
            var sender = await _repo.GetUser(userId);
            //check to see if user is the current user thats passed token up to server attempting to access this route
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))    
            {
                return Unauthorized();
            }

            messageForCreationDto.SenderId = userId;

            //NOTE** automapper will take the recipient var from memory and use it to automap into message, even though we didnt add it ourselves to the message!
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null)
            {
                return BadRequest("Error: Could not find User!");
            }

            var message = _mapper.Map<Message>(messageForCreationDto); 
            _repo.Add(message);
            
            if(await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);  //NOTE** Autmapper will attempt to map the recipient details since they are in memory, even though we didnt add them manually!
                return CreatedAtRoute("GetMessage", 
                    new {userId, id = message.Id}, messageToReturn);
            }

            throw new Exception("Creating Message failed on save!");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
             //check to see if user is the current user thats passed token up to server attempting to access this route
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))    
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }

            if(messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if(messageFromRepo.RecipientDeleted && messageFromRepo.SenderDeleted)
            {
                _repo.Delete(messageFromRepo);
            }

            if(await _repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Error deleting message!");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id) {
             //check to see if user is the current user thats passed token up to server attempting to access this route
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))    
            {
                return Unauthorized();
            }

            var message = await _repo.GetMessage(id);

            if(message.RecipientId != userId){
                return Unauthorized();
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _repo.SaveAll();
            return NoContent();
        }
    }
}