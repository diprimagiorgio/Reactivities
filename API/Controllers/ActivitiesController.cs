using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : baseApiController
    {
        private readonly DataContext _context;
        
        public ActivitiesController(DataContext context){
            this._context = context;
            
        }
        
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities(){
            return await _context.Activities.ToListAsync();
        }
        [HttpGet("{id}")]//in this way we are going to recive also the id from the url
        public async Task<ActionResult<Activity>> GetActivity(Guid id){
            return await _context.Activities.FindAsync(id);
        }
    }
}