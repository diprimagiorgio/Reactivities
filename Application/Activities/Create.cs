using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>{
            public Activity Activity {get; set;}
        } 


        public class Commandvalidator : AbstractValidator<Command>
        {
            public Commandvalidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this._userAccessor = userAccessor;
                this._context = context;
            }

            public async Task< Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                var attendee = new ActivityAttendee{
                    Activity = request.Activity,
                    AppUser = user,
                    IsHost = true
                };

                request.Activity.Attendees.Add(attendee); 

                // here we add our activity inside our context in memory, but we need to save in db
                _context.Activities.Add(request.Activity);

                // here we actualy save
                var res = await _context.SaveChangesAsync() > 0;
                if(!res) return Result<Unit>.Failure("fail to create activity");
                return Result<Unit>.Success(Unit.Value);// this is equal to nothing, it's just to say to the controller that we have done

            }
        }
    }
}