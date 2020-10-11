using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using WebApp.Helpers;
using WebApp.Exceptions;
using Common.Helpers;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Text;

namespace WebApp.Middlewares
{
    public class WebAppExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private HttpContext _context;
        private readonly ILogger<Program> _logger;

        public WebAppExceptionHandlerMiddleware(RequestDelegate next, ILogger<Program> logger)
        {
            _next = next;
            _logger = logger;
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
            bool acceptJson = _context.AcceptsJsonResult();

            switch (exception)
            {
                case ValidationException e:                   
                    if (acceptJson)
                    {
                        await _context.WriteStatusCodeResult(StatusCodes.Status400BadRequest, new { e.Errors });                        
                    }
                    else 
                    {
                        await _context.WriteViewResult("Errors", e.Errors, e.Model);
                    }
                    break;
                case UnauthenticatedException e:
                    await _context.ChallengeAsync();
                    break;
                case UnauthorizedException e:
                    if (acceptJson)
                    {
                        await _context.WriteStatusCodeResult(StatusCodes.Status401Unauthorized, e.Message);
                    }
                    else 
                    {
                        await _context.WriteErrorViewResult(e.Message);
                    }                    
                    break;
                case BadRequestException e:
                    if (acceptJson)
                    {
                        await _context.WriteStatusCodeResult(StatusCodes.Status400BadRequest, e.Message);
                    }
                    else
                    {
                        await _context.WriteErrorViewResult(e.Message);
                    }                    
                    break;
                case NotFoundException e:
                    if (acceptJson)
                    {
                        await _context.WriteStatusCodeResult(StatusCodes.Status404NotFound, e.Message);
                    }
                    else
                    {
                        await _context.WriteErrorViewResult(e.Message);
                    }
                    break;                
                default:
                    var message = new StringBuilder();
                    message.AppendLine("Unexpected Error occurred while processing the request.");
                    message.AppendLine("Error Date: " + DateTime.UtcNow);
                    message.AppendLine(" ");

                    message.AppendLine("==> User Info <==");
                    message.AppendLine("UserId: " + _context.User.Id());
                    message.AppendLine("Email: " + _context.User.Email());
                    message.AppendLine("IP Address: " + _context.Connection.RemoteIpAddress);                    
                    message.AppendLine(" ");
                    var r = _context.Request;

                    message.AppendLine("==> General <== ");                    
                    foreach (var key in r.RouteValues.Keys)
                    {
                        message.AppendLine(key + ": " + r.RouteValues[key]);
                    }
                    message.AppendLine("Method: " + r.Method);
                    if (r.Headers["referer"].Count > 0)
                        message.AppendLine("Referer: " + r.Headers["referer"].ToString());
                    message.AppendLine(" ");

                    if (r.Query.Keys.Count > 0)
                    {
                        message.AppendLine("==> Query Parameters <==");
                        foreach (var key in r.Query.Keys)
                        {
                            message.AppendLine(key + ": " + r.Query[key]);
                        }
                        message.AppendLine(" ");
                    }

                    if (r.HasFormContentType)
                    {
                        message.AppendLine("==> Form Data <==");
                        foreach (var key in r.Form.Keys)
                        {
                            message.AppendLine(key + ": " + r.Form[key]);
                        }
                        message.AppendLine(" ");
                    }

                    message.AppendLine("==> Exception Details <==");
                    message.AppendLine(exception.ToString());

                    _logger.LogError(message.ToString());

                    if (acceptJson)
                    {
                        await _context.WriteStatusCodeResult(StatusCodes.Status404NotFound, exception.Message);
                    }
                    else
                    {
                        await _context.WriteStatusCodeResult(StatusCodes.Status404NotFound);
                    }
                    break;
            }            
        }        
    }
    
}

