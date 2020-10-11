using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ExecutionService.Hubs;
using Microsoft.AspNetCore.SignalR;
using ExecutionService.Exceptions;

namespace ExecutionService.Middlewares
{
    public class SignalRExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private HttpContext _context;
        private readonly IHubContext<ExecutionHub> _hubContext;

        public SignalRExceptionHandlerMiddleware(RequestDelegate next, IHubContext<ExecutionHub> hubContext)
        {
            _next = next;
            _hubContext = hubContext;
        }

        public async Task Invoke(HttpContext context)
        {
            _context = context;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
            }
        }

        private async Task HandleExceptionAsync(Exception exception)
        {
            switch (exception)
            {                
                //case ClientTriggerException e:
                //    await _hubContext.Clients.User(e.UserId).SendAsync(e.TriggerType.ToString(), e.TriggerData);                 
                //    break;
                               
                default:
                    
                    break;
            }            
        }        
    }
    
}

