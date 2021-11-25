using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;

namespace Application.Activities
{
    // we want new paramiter for filtering
    public class ActivityParams: PagingParams
    {
        public bool isGoing { get; set; }
        public bool isHost { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }
}