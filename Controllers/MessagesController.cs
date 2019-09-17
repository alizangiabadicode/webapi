using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Data;
using datingapp.api.Dtos;
using datingapp.api.Helpers;
using datingapp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingapp.api.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(UpdateLastOnline))]
    [ApiController]
    [Route("api/users/{userid}/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository rep;
        private readonly IMapper mapper;
        public MessagesController(IDatingRepository rep, IMapper mapper)
        {
            this.mapper = mapper;
            this.rep = rep;
        }
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMsg(int id, int userid)
        {
            // check konm k khode tarafe ini k request dade
            var tokenId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            if (userid != tokenId)
            {
                return Unauthorized();
            }

            var msg = await this.rep.GetMessage(id);
            if (msg == null)
            {
                return BadRequest("msg doesn't exist!");
            }

            return Ok(msg);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMsg(int userid, CreateMsgDTO createmsgdto)
        {
            //********


            // the key to note here is that auto mapper is so clever that it automatically include reciver 
            // in the msg2 because reciver is loaded before . so we do it exactly for sender!

            // ******* 


            // check konm k khode tarafe ini k request dade
            var tokenId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            if (userid != tokenId)
            {
                return Unauthorized();
            }


            // reason is mentioned above
            var sender = this.rep.GetUser(userid);

            // check if the reciver exists
            var reciver = await this.rep.GetUser(createmsgdto.ReciverId);
            if (reciver == null)
            {
                return BadRequest("there is problem finding reciver!");
            }
            // set the sender id to user id
            createmsgdto.SenderId = userid;
            // map the createmsgdto to read msg
            var msg = this.mapper.Map<Message>(createmsgdto);
            // save msg to db
            this.rep.Add(msg);
            if (await this.rep.SaveAll())
            {
                var msg2 = this.mapper.Map<MessageToReturnDTO>(msg);
                return CreatedAtRoute("GetMessage", new { id = msg.Id }, msg2);
            }
            // if code reachs here it means sth has gone wrong
            throw new Exception("sth has gone wrong at createmsg");
        }
        [HttpGet]
        public async Task<IActionResult> GetMessages(int userid, [FromQuery]MessageParams messageParams)
        {
            int tokenId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userid != tokenId)
            {
                return Unauthorized();
            }

            messageParams.UserId = userid;

            var messages = await this.rep.GetMessageForUser(messageParams);

            Response.AddPagination(messages.CurrentPage, messages.PageSize, messages.TotalPages, messages.TotalCount);

            var mappedMessages = this.mapper.Map<IEnumerable<MessageToReturnDTO>>(messages);
            return Ok(mappedMessages);

        }

        [HttpGet("thread/{reciverId}")]
        public async Task<IActionResult> GetMessageThread(int userid, int reciverId)
        {
            // authorize
            int tokenId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userid != tokenId)
            {
                return Unauthorized();
            }

            // check to see if reciver exists
            User reciver = await this.rep.GetUser(reciverId);
            if (reciver == null)
            {
                return BadRequest("the reciver doesn't exists!");
            }

            // get message thread
            IEnumerable<Message> messages = await this.rep.GetMessageThread(userid, reciverId);
            // call to read
            this.ReadMessages(messages, userid);
            // map messages to msgtoreturn
            IEnumerable<MessageToReturnDTO> msgtoreturn = this.mapper.Map<IEnumerable<MessageToReturnDTO>>(messages);

            return Ok(msgtoreturn);
        }

        [HttpPost("{msgid}")]
        public async Task<IActionResult> DeleteMessage(int msgid, int userid)
        {
            // authorize
            int tokenid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (tokenid != userid)
            {
                return Unauthorized();
            }

            // check if such a message exists
            Message msg = await this.rep.GetMessage(msgid);
            if (msg == null)
            {
                return BadRequest("this message doesn't even exist");
            }

            // check to see weather the user id sender or reciver
            if (userid == msg.SenderId)
            {
                msg.SenderDeleted = true;
            }
            if (userid == msg.ReciverId)
            {
                msg.ReciverDeleted = true;
            }
            // if both deleted the message so delete it from db cause its no longer needed
            if (msg.SenderDeleted && msg.ReciverDeleted)
            {
                this.rep.Delete(msg);
            }

            if (await this.rep.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("exception at deletemessage");
        }

        public async void ReadMessages(IEnumerable<Message> messages, int userid)
        {
            foreach (Message message in messages)
            {
                if (message.ReciverId == userid && !message.IsRead)
                {
                    message.DateRead = DateTime.Now;
                    message.IsRead = true;
                }
            }

            await this.rep.SaveAll();

        }

    }
}