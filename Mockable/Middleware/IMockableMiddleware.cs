using Microsoft.AspNetCore.Http;

using Mockable.Models;

namespace Mockable.Middleware
{
    /// <summary>
    /// An implementation of <see cref="IMiddleware"/> that is responsible for intercepting
    /// a matching controller route and providing a mock of the expected return type.
    /// </summary>
    public interface IMockableMiddleware : IMiddleware
    {
        /// <summary>
        /// Adds routing information with a mock of the expected return type.
        /// </summary>
        /// <param name="newContext">
        /// The context to add to <see cref="IMockableMiddleware"/>.
        /// </param>
        void AddContext(MockableContext newContext);
    }
}
