#load "../prng/Prng.fs"

open Prng

type CoinSide =
  | Heads
  | Tails

let coinFlips = Generator.uniform Heads [ Tails ]

for coinSide in Generator.start 0 coinFlips do
  printfn "%A" coinSide
