﻿using Binacle.Lib.Models;

namespace Binacle.Lib.Abstractions.Strategies
{
    public interface IBinFittingStrategy : IBinFittingStrategyWithBins, IBinFittingStrategyWithBinsAndItems, IBinFittingOperation
    {
        IBinFittingStrategyWithBins WithBins(IEnumerable<Item> bins);
    }

    public interface IBinFittingStrategyWithBins
    {
        IBinFittingStrategyWithBinsAndItems AndItems(IEnumerable<Item> items);
    }

    public interface IBinFittingStrategyWithBinsAndItems
    {
        IBinFittingOperation Build();
    }

    public interface IBinFittingOperation
    {
        BinFittingOperationResult Execute();
    }
}
