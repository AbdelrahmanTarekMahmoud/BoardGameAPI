using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MyBGList.Helpers.CustomModelState
{ 
    //All actions automatically do the modelstate validation due to the ModelStateInvalidFilter
    //So here i create a attribute
    //which the action will apply it
    //then i will loop over the filters of this action
    //and if i see a filter named ModelStateInvalidFilter i will remove it
    public class ManualValidationFilterAttribute : Attribute, IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            for (var i = 0; i < action.Filters.Count; i++)
            {
                if (action.Filters[i] is ModelStateInvalidFilter ||
                    action.Filters[i].GetType().Name == "ModelStateInvalidFilterFactory")
                {
                    action.Filters.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
