using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query: IRequest<Result<List<UserActivityDto>>>{
            public string Username { get; set; }
            public string Predicate { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
            this._mapper = mapper;
            this._context = context;
            }

            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.ActivitiesAttendees
                    .Where( a => a.AppUser.UserName == request.Username )
                    .OrderBy(a => a.Activity.Date)
                    .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();
                switch(request.Predicate){
                    case "past":
                        query = query.Where(a => a.Date < DateTime.UtcNow );
                    break;
                    case "hosting":
                        query = query.Where(a => a.HostUsername == request.Username );
                    break;
                    default:
                        query = query.Where(a => a.Date >= DateTime.UtcNow );
                    break;
                }
                var res = await query.ToListAsync();
                return Result<List<UserActivityDto>>.Success(res);
            }
        }
    }
}