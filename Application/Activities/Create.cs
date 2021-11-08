using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest{
            public Activity Activity {get; set;}
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                this._context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // here we add our activity inside our context in memory, but we need to save in db
                _context.Activities.Add(request.Activity);

                // here we actualy save
                await _context.SaveChangesAsync();
                
                return Unit.Value;// this is equal to nothing, it's just to say to the controller that we have done

            }
        }
    }
}