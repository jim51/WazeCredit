using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WazeCredit.Service.LifeTimeExample
{
    public class SingletionService
    {
        private readonly Guid guid;

        public SingletionService()
        {
            guid = Guid.NewGuid();
        }

        public string GetGuid() => guid.ToString();
    }
}
