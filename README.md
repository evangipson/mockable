# Mockable
A [.NET attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute?view=net-8.0) that enables controller methods to provide a mock.

## Usage
Add the `[Mockable]` attribute to any controller method.

Optionally, provide a `MockableCondition` to only mock when certain conditions are met.

If no `MockableCondition` is provided, the default behavior is to never mock.

```csharp
using Mockable.Attributes;

[ApiController]
public class SomeController : ControllerBase
{
    public string EndpointWithoutMock() => "This is not configured to mock.";

    [Mockable]
    public string EndpointToNeverMock() => "This will never be mocked.";

    [Mockable(Condition = MockableCondition.Always)]
    public string EndpointToAlwaysMock() => "This will be mocked.";

    [Mockable(Condition = MockableCondition.Always)]
    public Task<string> AsyncEndpointToAlwaysMock() => await Task.FromResult("This async result will be mocked.");
}
```

## Roadmap
- [x] Scaffold attribute
- [x] Scaffold middleware
- [x] Mock simple types
- [x] Mock `Task<>` types
- [x] Enable conditions "Never" and "Always"
- [ ] Enable "BadRequest", "Exception", and "UnableToConnect" conditions