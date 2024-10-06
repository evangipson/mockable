using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using Mockable.Attributes;
using Mockable.Models;

namespace Mockable.Middleware
{
    /// <inheritdoc cref="IMockableMiddleware"/>
    public class MockableMiddleware(ILogger<MockableMiddleware> logger) : IMockableMiddleware
    {
        /// <summary>
        /// A list of <see cref="MockableContext"/> that is used to associate routes to mocks.
        /// </summary>
        private readonly List<MockableContext> _mockContexts = [];

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            logger.LogInformation($"{nameof(MockableMiddleware)}: Mockable Middleware invoked.");

            var routeActionKey = context.GetRouteData().Values["action"]?.ToString() ?? string.Empty;
            var routeControllerKey = context.GetRouteData().Values["controller"]?.ToString() ?? string.Empty;

            var contextHasMatchedRoute = _mockContexts.Exists(context => context.Action == routeActionKey && context.Controller == routeControllerKey);
            if (!contextHasMatchedRoute)
            {
                logger.LogInformation($"{nameof(MockableMiddleware)}: No matching route found.");
                return next.Invoke(context);
            }

            var mockableContext = _mockContexts.Where(context => context.Action == routeActionKey && context.Controller == routeControllerKey).First();
            if(mockableContext.Attribute.Condition == MockableCondition.Never)
            {
                logger.LogInformation($"{nameof(MockableMiddleware)}: {nameof(MockableCondition)} set to '{nameof(MockableCondition.Never)}' for matched route, continuing request pipeline.");
                return next.Invoke(context);
            }
            logger.LogInformation($"{nameof(MockableMiddleware)}: Mockable condition is set, providing a mock if the condition is met.");

            var serializedMock = JsonSerializer.Serialize(mockableContext.Attribute.GetMock());
            return context.Response.WriteAsync(serializedMock);
        }

        public void AddContext(MockableContext newContext) => _mockContexts.Add(newContext);
    }
}
