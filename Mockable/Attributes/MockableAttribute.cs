using Mockable.Models;

namespace Mockable.Attributes
{
    /// <summary>
    /// Adds the ability to mock a response from a method, supported for controller methods.
    /// <para>
    /// Derives the return type of the method, and creates a mock of that type to use when
    /// <see cref="Condition"/> is met.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class MockableAttribute() : Attribute
    {
        private object? _mock;

        /// <summary>
        /// Determines when the endpoint will be mocked, defaults to <see cref="MockableCondition.Never"/>.
        /// </summary>
        public MockableCondition Condition { get; set; } = MockableCondition.Never;

        /// <summary>
        /// Sets the object to be returned when the decorated method is mocked.
        /// </summary>
        /// <typeparam name="TMock">
        /// The type of object to mock.
        /// </typeparam>
        /// <param name="mockObject">
        /// An object to mock.
        /// </param>
        public void SetMock<TMock>(TMock mockObject)
        {
            _mock = mockObject;
        }

        /// <summary>
        /// Gets the current mock from this <see cref="MockableAttribute"/>.
        /// </summary>
        /// <returns>
        /// The current mocked object.
        /// </returns>
        public object? GetMock() => _mock;
    }
}
