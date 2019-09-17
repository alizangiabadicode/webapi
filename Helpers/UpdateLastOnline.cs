using System;
using System.Threading.Tasks;
using datingapp.api.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace datingapp.api.Helpers
{
    public class UpdateLastOnline : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var c =  await next();
            var id = int.Parse(c.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var rep= c.HttpContext.RequestServices.GetService(typeof(IDatingRepository)) as DatingRepository;
            var user = await rep.GetUser(id);
            user.LastActive = DateTime.Now;
            await rep.SaveAll();
        }
    }
}