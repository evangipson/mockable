using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Mockable.Attributes;
using Mockable.Middleware;
using Mockable.Models;

namespace Mockable.Extensions
{
    /// <summary>
    /// A collection of extensions for adding Mockable to any <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="IMockableMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> for the <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> with an <see cref="IMockableMiddleware"/> singleton registered.
        /// </returns>
        public static IServiceCollection AddMockable(this IServiceCollection services)
        {
            return services.AddSingleton<IMockableMiddleware, MockableMiddleware>();
        }

        /// <summary>
        /// Adds <see cref="IMockableMiddleware"/> to the <see cref="IApplicationBuilder"/> request pipeline.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IApplicationBuilder"/> for the application.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationBuilder"/> with an <see cref="IMockableMiddleware"/> added to the request pipeline.
        /// </returns>
        public static IApplicationBuilder AddMockableMiddleware(this IApplicationBuilder builder)
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            var controllers = currentAssembly.ExportedTypes.Where(type => type.GetCustomAttributes<ControllerAttribute>().Any());
            var controllerMethods = controllers.SelectMany(type => type.GetMethods());

            var mockableMethods = controllerMethods.Where(mockableMethod => mockableMethod.IsPublic)
                .Where(mockableMethod => !mockableMethod.IsConstructor)
                .Where(mockableMethod => mockableMethod.GetCustomAttribute<MockableAttribute>() != null);

            var middlewareService = builder.ApplicationServices.GetRequiredService<IMockableMiddleware>();
            foreach (var mockableMethod in mockableMethods)
            {
                var mockableAttribute = mockableMethod.GetCustomAttribute<MockableAttribute>()!;
                middlewareService.AddContext(mockableAttribute, mockableMethod);
            }

            builder.UseMiddleware<IMockableMiddleware>();

            return builder;
        }

        /// <summary>
        /// Adds a route to match, and a mock to use for <see cref="IMockableMiddleware"/>.
        /// </summary>
        /// <param name="middleware">
        /// The <see cref="IMockableMiddleware"/> to add <see cref="MockableAttribute"/> information to.
        /// </param>
        /// <param name="mockableAttribute">
        /// The <see cref="MockableAttribute"/> to be hydrated.
        /// </param>
        /// <param name="mockableMethod">
        /// A <see cref="MethodInfo"/> containing attributes of method that has been decorated with a <see cref="MockableAttribute"/>.
        /// </param>
        private static void AddContext(this IMockableMiddleware middleware, MockableAttribute mockableAttribute, MethodInfo mockableMethod)
        {
            var mockReturnObject = GetEndpointReturnMock(mockableMethod);
            mockableAttribute.SetMock(mockReturnObject);

            middleware.AddContext(new MockableContext
            {
                Attribute = mockableAttribute,
                Action = mockableMethod.Name,
                Controller = mockableMethod.DeclaringType?.Name.Replace("controller", "", StringComparison.OrdinalIgnoreCase) ?? string.Empty
            });
        }

        /// <summary>
        /// Attempts to get the return type of the method that has a <see cref="MockableAttribute"/>.
        /// </summary>
        /// <param name="mockableMethod">
        /// A <see cref="MethodInfo"/> containing attributes of method that has been decorated with a <see cref="MockableAttribute"/>.
        /// </param>
        /// <returns>
        /// The <paramref name="mockableMethod"/> return type.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static object? GetEndpointReturnMock(MethodInfo mockableMethod)
        {
            object? mockReturnObject;
            try
            {
                /* handle public parameterless constructors */
                mockReturnObject = Activator.CreateInstance(mockableMethod.ReturnType, BindingFlags.Public);
            }
            catch (MissingMethodException)
            {
                /* handle non-public (implicit) parameterless constructors */
                mockReturnObject = Activator.CreateInstance(mockableMethod.ReturnType, true);
            }

            /* if there are no constructors that are parameterless, throw */
            if(mockReturnObject == null)
            {
                throw new InvalidOperationException($"{nameof(Mockable)}: Could not find parameterless constructor for endpoint result.");
            }

            return mockReturnObject.GetType().IsGenericType ? GetGenericTypeMock(mockReturnObject) : mockReturnObject;
        }

        /// <summary>
        /// Attempts to get the return type of a generic type, or the generic type itself.
        /// </summary>
        /// <param name="genericTypedObject">
        /// An generically-typed object.
        /// </param>
        /// <returns>
        /// The generic return type of <paramref name="genericTypedObject"/>, or the generic type itself.
        /// </returns>
        private static object? GetGenericTypeMock(object genericTypedObject)
        {
            Type? taskReturnType = null;

            /* determine if the generic type is a Task */
            var mockIsGenericTask = genericTypedObject?.GetType().GetGenericTypeDefinition() == typeof(Task<>);
            if (mockIsGenericTask)
            {
                taskReturnType = genericTypedObject!.GetType().GetGenericArguments().FirstOrDefault();
            }

            /* if the type has no generic arguments, return the generic type itself. */
            if (taskReturnType == null)
            {
                return genericTypedObject?.GetType().GetGenericTypeDefinition();
            }

            /* return the generic argument of the Task */
            return Activator.CreateInstance(taskReturnType);
        }
    }
}
