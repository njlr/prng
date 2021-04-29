# prng

Experiments with pseudo-random number generators and computation expressions, inspired by Elm.

## Development

To build:

```bash
dotnet tool restore
dotnet paket restore
dotnet build
```

To test:

```bash
dotnet test
```

To run the demos:

```bash
dotnet fsi ./demos/CoinFlips.fsx
```

## The Idea

We define a `Generator<'t>` as a function from a seed to a sequence of `'t`:

```fsharp
type Generator<'t> = Seed -> seq<'t>
```

A well-defined sequence is deterministic and infinite.

A seed is just an integer:

```fsharp
type Seed = int
```

The simplest valid generator always yields the same value:

```fsharp
let alwaysSix : Generator<int> =
  (fun _ ->
    seq {
      while true do
        yield 6
    })
```

Internally, we can use `System.Random`:

```fsharp
let int : Generator<int> =
  (fun seed ->
    seq {
      let r = System.Random seed

      while true do
        r.Next ()
    })
```

This also works in Fable.

From this simple definition, we can start transforming and combining generators:

```fsharp
let d6 = Generator.between 1 6

let twoD6 =
  d6
  |> Generator.map2 d6 (fun x y -> x + y)
```

And we are not limited to numbers either:

```fsharp
type Bread =
  | WholeWheat
  | Sourdough
  | WhiteLoaf

let bread = Generator.uniform WholeWheat [ Sourdough; WhiteLoaf ]
```

Using computation expressions, we can make complex bindings easy to read:

```fsharp
type Sandwich =
  {
    Bread : Bread
    Salad : Salad
    Filling : Filling
    Dressing : Dressing
  }

let sandwich =
  generator {
    let! b = bread
    let! s = salad
    let! f = filling
    let! d = dressing

    return
      {
        Bread = b
        Salad = s
        Filling = f
        Dressing = d
      }
  }
```
