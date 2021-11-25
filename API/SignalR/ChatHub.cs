using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            this._mediator = mediator;
        }
        public async Task SendComment(Create.Command cmd){
            // we save the comment in our db, now we are going to recive back a CommentDto
            var comment = await _mediator.Send(cmd);

            // we want to send back the comment to all the people of the group
            await Clients.Group(cmd.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);
        }
        // when a user conect for the first time he is going to recive all the comments sent for that activity
        public override async Task OnConnectedAsync(){
            // we want to join the user to the group
            // we use query string paramiter to send additional info, no url paramiter
            var httpContext =Context.GetHttpContext();
            var activityId = httpContext.Request.Query["activityId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            // we are going to send the list of comment of the group
            var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId)});
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}