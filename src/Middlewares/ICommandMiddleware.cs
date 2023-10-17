namespace Tgstation.Server.CommandLineInterface.Middlewares;

using JetBrains.Annotations;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface ICommandMiddleware
{
    ValueTask HandleCommandAsync(IMiddlewareContext context, PipelineNext nextStep);
}

public delegate ValueTask PipelineNext();
