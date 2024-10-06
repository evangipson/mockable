namespace Mockable.Attributes
{
    /// <summary>
    /// A list of potential mocking conditions, to be used on a <see cref="MockableAttribute"/>.
    /// </summary>
    public enum MockableCondition
    {
        /// <summary>
        /// Determines the mock should never be presented.
        /// </summary>
        Never,
        /// <summary>
        /// Determines the mock should only be presented when the method is unable to connect.
        /// </summary>
        UnableToConnect,
        /// <summary>
        /// Determines the mock should only be presented when the method gets a bad request.
        /// </summary>
        BadRequest,
        /// <summary>
        /// Determines the mock should be presented upon any exception in the method.
        /// </summary>
        Exception,
        /// <summary>
        /// Determines the mock should always be presented.
        /// </summary>
        Always
    };
}
