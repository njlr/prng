#load "../prng/Prng.fs"
#load "../prng/GeneratorBuilder.fs"

open Prng

type Suit =
  | Heart
  | Diamond
  | Club
  | Spade

type Rank =
  | Ace
  | King
  | Queen
  | Jack
  | Ten
  | Eight
  | Seven
  | Six
  | Five
  | Four
  | Three
  | Two

type Card =
  {
    Suit : Suit
    Rank : Rank
  }

module Generator =

  let suit = Generator.uniform Heart [ Diamond; Club; Spade ]

  let rank =
    Generator.uniform
      Ace
      [
        King
        Queen
        Jack
        Ten
        Eight
        Seven
        Six
        Five
        Four
        Three
        Two
     ]

  let card =
    Generator.map2
      (fun suit rank ->
        {
          Suit = suit
          Rank = rank
        })
      suit
      rank

  let cardCE =
    generator {
      let! s = suit
      let! r = rank

      return
        {
          Suit = s
          Rank = r
        }
    }

let cards =
  Generator.cardCE
  |> Generator.start 0
  |> Seq.truncate 10

for card in cards do
  printfn "%A" card
