module Prng.Tests.Generator

open Xunit
open FsUnit.Xunit
open Prng

[<Fact>]
let ``Generator.constant works as expected`` () =

  let k = 123
  let n = 10

  let actual =
    Generator.constant k
    |> Generator.start 0
    |> Seq.truncate n
    |> Seq.toList

  let expected = List.init n (fun _ -> k)

  actual |> should equal expected

[<Fact>]
let ``Generator.list works as expected`` () =

  let k = 123
  let w = 5
  let n = 10

  let g = Generator.constant k

  let actual =
    Generator.list w g
    |> Generator.start 0
    |> Seq.truncate n
    |> Seq.toList

  let expected = List.init n (fun _ -> List.init w (fun _ -> k))

  actual |> should equal expected
