using BugFixer.Domain.Entities.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.Interfaces
{
    public interface IStateRepository
    {
        Task<List<State>> GetAllStates(long? stateId = null);
    }
}
