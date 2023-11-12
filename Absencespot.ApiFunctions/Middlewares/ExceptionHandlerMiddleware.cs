using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Middlewares
{
    public class ExceptionHandlerMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var logger = context.GetLogger<ExceptionHandlerMiddleware>();
                using (logger.BeginScope("Downloading messages"))
                {
                    // Scope is "Checking mail" -> "Downloading messages"
                    //logger.("Connection interrupted");
                }
            }
        }
    }
}
