using Mockable.Attributes;

namespace Mockable.Models
{
    /// <summary>
    /// A context for Mockable that contains routing information and a <see cref="MockableAttribute"/>.
    /// </summary>
    /// <param name="attribute">
    /// The <see cref="MockableAttribute"/> for this route.
    /// </param>
    /// <param name="action">
    /// The method name for the route.
    /// </param>
    /// <param name="controller">
    /// The controller name for the route.
    /// </param>
    public struct MockableContext(MockableAttribute attribute, string action, string controller)
    {
        /// <summary>
        /// A <see cref="MockableAttribute"/>, which contains the mock for a route.
        /// </summary>
        public MockableAttribute Attribute = attribute;

        /// <summary>
        /// The method name for the route.
        /// </summary>
        public string Action = action;

        /// <summary>
        /// The controller name for the route.
        /// </summary>
        public string Controller = controller;
    }
}
