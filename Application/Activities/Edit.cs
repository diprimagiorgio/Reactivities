using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command: IRequest<Result<Unit>>{
            public Activity Activity{get; set;}
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
            private readonly IMapper _mapper;
            private readonly DataContext _context;

            public Handler(DataContext context,IMapper mapper){
                this._mapper = mapper;
                this._context = context;
            }
            
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync( request.Activity.Id);
                if ( activity == null) return null;    

                _mapper.Map(request.Activity, activity);
                var res = await _context.SaveChangesAsync() > 0;
                if(!res) return Result<Unit>.Failure("fail to update the activity");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}