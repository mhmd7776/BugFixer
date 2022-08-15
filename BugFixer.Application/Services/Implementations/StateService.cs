using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Interfaces;
using BugFixer.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Implementations
{
    public class StateService : IStateService
    {
        #region Ctor

        private readonly IStateRepository _stateRepository;

        public StateService(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        #endregion

        public async Task<List<SelectListViewModel>> GetAllStates(long? stateId = null)
        {
            var states = await _stateRepository.GetAllStates(stateId);

            return states.Select(s => new SelectListViewModel { 
                Id = s.Id,
                Title = s.Title
            }).ToList();
        }
    }
}
