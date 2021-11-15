using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [AllowAnonymous]
        public class ActivitiesController : baseApiController{  
        [HttpGet]
        public async Task<IActionResult> GetActivities(){
            return HandleResult(await Mediator.Send(new List.Query()));
        }
        [HttpGet("{id}")]//in this way we are going to recive also the id from the url
        public async Task<IActionResult> GetActivity(Guid id){
            return HandleResult(await Mediator.Send(new Details.Query{Id = id}));
        }
        // this time is not important what we are going to return, but want to provide some informaion such as response type
        // we can add [fromBody] Activity activity
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity){
            return HandleResult(
                await Mediator.Send(new Create.Command{Activity= activity})
            );
        }
        // here we are going to get the id from the url and the activity from the body
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity){
            activity.Id = id;
            return HandleResult( await Mediator.Send(new Edit.Command{Activity = activity}));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveActivity(Guid id){
            return HandleResult(await Mediator.Send(new Delete.Command{Id = id}));
        }

    }
}