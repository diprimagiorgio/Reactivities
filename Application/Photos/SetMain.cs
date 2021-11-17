using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>{
            public string Id { get; set; }

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
                var user = await _context.Users.Include(u => u.Photos)
                    .FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername() );
                
                if( user == null ) return null;

                var oldMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
                if( oldMainPhoto == null) return null;// no photo in the collection
                oldMainPhoto.IsMain = false;
                var newMianPhoto = user.Photos.FirstOrDefault(p => p.Id == request.Id);
                if( newMianPhoto == null) return null;// no photo in the collection
                newMianPhoto.IsMain = true;
                var res = await _context.SaveChangesAsync() > 0;
                if(!res) return Result<Unit>.Failure("fail to update the main photo");
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}