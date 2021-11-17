using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    // our application does not have dependecies to infractructure, but we can access via interface
    public interface IUserAccessor
    {
        string GetUsername();
    }
}