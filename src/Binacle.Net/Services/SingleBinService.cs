namespace Binacle.Net.v4.Services;

internal interface ISingleBinService
{
    
}

internal class SingleBinService : ISingleBinService
{
    // private readonly Channel<AlgorithmOperationLogChannelRequest>? logChannel;
    // private readonly ILogger<BinacleService> logger;
    // private readonly IBinProcessor loopBinProcessor;
    // private readonly IBinProcessor parallelBinProcessor;
    //
    // public SingleBinService(
    //     [FromKeyedServices("loop")] IBinProcessor loopBinProcessor,
    //     [FromKeyedServices("parallel")] IBinProcessor parallelBinProcessor,
    //     ILogger<BinacleService> logger,
    //     IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>> logChannel
    // )
    // {
    //     this.loopBinProcessor = loopBinProcessor;
    //     this.parallelBinProcessor = parallelBinProcessor;
    //     this.logChannel = logChannel.Value;
    //     this.logger = logger;
    // }
}
