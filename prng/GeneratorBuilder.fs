namespace Prng

open System

// Bind  M<'T> * ('T -> M<'U>) -> M<'U>
// Return  'T -> M<'T>

type GeneratorBuilder () =
  member this.Bind (m : Generator<'t>, f : 't -> Generator<'u>) : Generator<'u> =
    (fun seed ->
      seq {
        let r = Random seed

        let xs = m (r.Next ())

        for x in xs do
          let seed = r.Next ()

          yield
            f x seed
            |> Seq.head
      })

  member this.Return x =
    Generator.constant x

[<AutoOpen>]
module ComputationExpression =

  let generator = GeneratorBuilder ()
