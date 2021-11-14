using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        // the Result<Activity> is the type that I'm going to return 
        public class Query: IRequest<Result<Activity>>{
            public Guid Id {get; set; }
        }

        // here the query is the input paramiter and the activity the return type
        public class Handler : IRequestHandler<Query, Result<Activity>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                this._context = context;
            }

            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Id);
                return Result<Activity>.Success(activity);

            }
        }
    }
}