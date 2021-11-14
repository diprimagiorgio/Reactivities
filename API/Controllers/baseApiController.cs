using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class baseApiController : ControllerBase
    {
        private IMediator _mediator;
        // ??= if null 
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult HandleResult<T>(Result<T> res){
            if(res == null) 
                return NotFound();
            if(res.IsSuccess && res.Value != null)
                return Ok(res.Value);
            if( res.IsSuccess && res.Value == null)
                return NotFound();
            return BadRequest(res.Error);
        }
        
    }
}