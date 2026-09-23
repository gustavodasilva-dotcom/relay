namespace Relay.SharedKernel;

public sealed record Error(
    string Code,
    string Description,
    ErrorType Type);
