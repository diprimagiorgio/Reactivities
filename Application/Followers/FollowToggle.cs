using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>{
            public string TargetUsername { get; set; }
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

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var observer = await _context.Users.FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername());
                var target = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.TargetUsername);
                if(target == null) return null;

// check if there is a record in the UserFollowing table
                var follwing = await _context.UserFollowings.FindAsync(observer.Id, target.Id);
                if(follwing == null){
                    follwing = new UserFollowing{
                        Observer = observer,
                        Target =  target
                        
                    };
                    _context.UserFollowings.Add(follwing);
                }else{
                    _context.UserFollowings.Remove(follwing);
                }

                var success = await _context.SaveChangesAsync() > 0;
                if ( success ) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Fail to update following");


            }
        }
    }
}