using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController: baseApiController
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username){
            return HandleResult(await Mediator.Send(new Details.Query{username= username}));
        }
        [HttpGet("{username}/activities")]
        public async Task<IActionResult> GetProfile(string username, [FromQuery] string predicate){
            return HandleResult(await Mediator.Send(new ListActivities.Query{Username = username, Predicate = predicate}));
        }
    }
}