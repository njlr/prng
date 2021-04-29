namespace Prng

type Seed = int

type Generator<'t> = Seed -> seq<'t>

module Generator =

  open System

  let constant k : Generator<_> =
    (fun _ ->
      seq {
        while true do
          yield k
      })

  let int : Generator<int> =
    (fun seed ->
      seq {
        let r = Random seed

        while true do
          r.Next ()
      })

  let double : Generator<double> =
    (fun seed ->
      seq {
        let r = Random seed

        while true do
          r.NextDouble ()
      })

  let float : Generator<float> =
    (fun seed ->
      seq {
        let r = Random seed

        while true do
          r.NextDouble () |> float
      })

  let bool : Generator<bool> =
    (fun seed ->
      seq {
        let r = Random seed

        while true do
          r.NextDouble () < 0.5
      })

  let between (minInclusive : int) (maxInclusive : int) : Generator<int> =
    (fun seed ->
      seq {
        let r = Random seed
        let maxExclusive = maxInclusive + 1

        while true do
          r.Next (minInclusive, maxExclusive)
      })

  let uniform first others : Generator<_> =
    (fun seed ->
      seq {
        let r = Random seed
        let options = Array.ofSeq (first :: others)

        while true do
          let index = r.Next (Array.length options)

          yield options |> Array.item index
      })

  let weighted first others : Generator<_> =
    let totalWeight =
      first :: others
      |> List.sumBy (fst >> abs)

    let rec getByWeight (weight, value) others countdown =
      match others with
      | [] -> value
      | second :: otherOthers ->
        if countdown <= abs weight
        then
          value
        else
          getByWeight second otherOthers (countdown - abs weight)

    (fun seed ->
      seq {
        let r = Random seed

        while true do
          let weight = r.Next totalWeight

          yield getByWeight first others weight
      })

  let fork (g : Generator<_>) : Generator<_> * Generator<_> =
    let g1 = g

    let g2 =
      (fun seed ->
        let r = Random seed
        let seed2 = r.Next ()

        g seed2)

    g1, g2

  let map f (g : Generator<_>) : Generator<_> =
    (fun seed ->
      g seed
      |> Seq.map f)

  let map2 f (a : Generator<_>) (b : Generator<_>) : Generator<_> =
    (fun seed -> Seq.map2 f (a seed) (b seed))

  let map3 f (a : Generator<_>) (b : Generator<_>) (c : Generator<_>) : Generator<_> =
    (fun seed -> Seq.map3 f (a seed) (b seed) (c seed))

  let andThen (f : _ -> Generator<_>) (a : Generator<_>) : Generator<_> =
    (fun seed ->
      let r = Random seed

      let seed1 = r.Next ()
      let seed2 = r.Next ()

      a seed1
      |> Seq.zip (int seed2)
      |> Seq.collect (fun (seed, x) -> f x seed))

  let pair (a : Generator<_>) (b : Generator<_>) : Generator<_> =
    map2 (fun x y -> x, y) a b

  let start (seed : Seed) (g : Generator<_>) =
    g seed

  let list (n : int) (g : Generator<_>) : Generator<_ list> =
    (fun seed ->
      g seed
      |> Seq.chunkBySize n
      |> Seq.map Seq.toList)
