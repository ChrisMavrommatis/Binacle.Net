# Config

Configuration files related to local setup


---
## Api
Script for running Binacle.Net
`api.sh`

Arguments:
- `Normal` (`N`)
- `WithServiceModuleOnly` (`S`)
- `WithUiModuleOnly` (`U`)
- `WithAllModules` (`A`)

---

## Benchmarks
Script  for all benchmarks

Arguments:
- `Scaling_Fitting_FFD` (`--filter *Scaling.Fitting_FFD*`)
- `Scaling_FittingComparison` (`--filter *Scaling.FittingComparison*`)
- `Scaling_Packing_FFD` (`--filter *Scaling.Packing_FFD*`)
- `Scaling_PackingComparison` (`--filter *Scaling.PackingComparison*`)
- `Combination_FittingSingleBaseLine` (`--filter *Combination.FittingSingleBaseLine*`)
- `Combination_PackingSingleBaseLine` (`--filter *Combination.PackingSingleBaseLine*`)
- `Combination_FittingMultipleBins` (`--filter *Combination.FittingMultipleBins*`)
- `Combination_PackingMultipleBins` (`--filter *Combination.PackingMultipleBins*`)

---

## Tests
Script for running the tests for Binacle.Net

Arguments
- `lib` (`Binacle.Lib.UnitTests`)
- `api` (`Binacle.Net.IntegrationTests`)
- `api_service` (`Binacle.Net.ServiceModule.IntegrationTests`)
- `vipaq` (`Binacle.ViPaq.UnitTests`)

---