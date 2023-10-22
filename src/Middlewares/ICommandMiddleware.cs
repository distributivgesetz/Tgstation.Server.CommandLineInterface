namespace Tgstation.Server.CommandLineInterface.Middlewares;

using JetBrains.Annotations;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface ICommandMiddleware
{
    ValueTask HandleCommandAsync(ICommandContext context, PipelineNext nextStep);
}

public delegate ValueTask PipelineNext();
