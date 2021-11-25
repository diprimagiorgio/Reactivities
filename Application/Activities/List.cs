using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<ActivityDto>>>{
            public ActivityParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor ){
                this._userAccessor = userAccessor;
                this._context = context;
                this._mapper = mapper;
            }
            

            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // to get the related entities, ( the attendee ) ef is not going to do it automaticall
                var query =  _context.Activities
                .Where(d => d.Date >= request.Params.StartDate)
                    .OrderBy(d => d.Date)
                    /* 1   .Include(a => a.Attendees)// this is the join table 
                    .ThenInclude(u => u.AppUser)// this is going to take the actual user */
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider,new {currentUsername = _userAccessor.GetUsername()} )
                    .AsQueryable();

                if (request.Params.isGoing && !request.Params.isHost)
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                if (request.Params.isHost && !request.Params.isGoing)
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }


                // we can create an automapper to go from activities to activities dto
                // 1 var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities );
                return Result<PagedList<ActivityDto>>.Success(
                    await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
                );
            }
        }


    }
}