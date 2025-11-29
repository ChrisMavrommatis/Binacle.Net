# Config

Configuration files related to local setup

## Tmux
Tmux setup for Binacle.Net
`tmux.sh`

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
- `AlgorithmVersion`
- `MultipleBins`
- `MultipleItems`

---

## Tests
Script for running the tests for Binacle.Net

Arguments
- `lib` (`Binacle.Lib.UnitTests`)
- `api` (`Binacle.Net.IntegrationTests`)
- `api_service` (`Binacle.Net.ServiceModule.IntegrationTests`)
- `vipaq` (`Binacle.ViPaq.UnitTests`)

---

## Build
Script for building Binacle.Net and testing it
`build.sh`