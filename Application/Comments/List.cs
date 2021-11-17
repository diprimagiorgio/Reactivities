using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class List
    {
        public class Query : IRequest<Result<List<CommentDto>>>{
            public Guid ActivityId { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<CommentDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this._mapper = mapper;
                this._context = context;
            }

            public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                /*  
                    HIS version
                var comments = await _context.Comments
                    .Where(x => x.Activity.Id == request.ActivityId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Result<List<CommentDto>>.Success(comments);
                 */
                var comments =await  _context.Comments
                    .Include(p => p.Author)
                    .Where(m => m.Activity.Id == request.ActivityId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
                    ;
                
                var commentsToReturn = _mapper.Map<List<CommentDto>>(comments);
                return Result<List<CommentDto>>.Success(commentsToReturn);
            }
        }
    }
}