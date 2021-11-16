using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<ActivityDto>>>{}

        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper){
                this._context = context;
                this._mapper = mapper;
            }
            

            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // to get the related entities, ( the attendee ) ef is not going to do it automaticall
                var activities =  await _context.Activities
                    /* 1   .Include(a => a.Attendees)// this is the join table 
                    .ThenInclude(u => u.AppUser)// this is going to take the actual user */
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                // we can create an automapper to go from activities to activities dto
                // 1 var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities );
                return Result<List<ActivityDto>>.Success(activities);
            }
        }


    }
}