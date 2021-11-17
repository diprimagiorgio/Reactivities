using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>{
           public string Body { get; set; }
           public Guid ActivityId { get; set; }

        }
        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                this._userAccessor = userAccessor;
                this._mapper = mapper;
                this._context = context;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.ActivityId);
                var user = await _context.Users
                    .Include(p => p.Photos)
                    .FirstOrDefaultAsync( x => x.UserName == _userAccessor.GetUsername());


                var comment = new Comment{
                    Author = user,
                    Activity = activity,
                    Body = request.Body
                };
                activity.Comments.Add(comment);
                var res = await _context.SaveChangesAsync() > 0;
                if(!res) return Result<CommentDto>.Failure("fail to add comment");
                return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));

            }
        }

    }
}