using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Interfaces;

namespace SystemChecker.Model.Checkers.Serialisation
{
    public static class CheckUnpacker
    {
        public static ISystemCheck Unpack(CheckToPerform check, IRepositoryFactory repoFactory)
        {
            var checkType = repoFactory.GetCheckTypeRepository().GetById(check.CheckTypeId);
            // todo: cache check types?

            var checker = (ISystemCheck)Activator.CreateInstance(checkType.CheckAssembly, checkType.CheckTypeName).Unwrap();

            checker.CheckToPerformId = check.CheckId;

            if (!string.IsNullOrWhiteSpace(check.Settings))
            {

                var settingsProperty = checker.GetType().GetProperty("Settings");

                var settingsObject = JsonConvert.DeserializeObject(check.Settings, settingsProperty.PropertyType);

                settingsProperty.SetValue(checker, settingsObject);
            }

            if (!string.IsNullOrWhiteSpace(check.Outcomes))
            {
                var outcomesList = JsonConvert.DeserializeObject<List<Outcome>>(check.Outcomes);

                checker.Outcomes = outcomesList;
            }

            return checker;
        }
    }
}