#load "../prng/Prng.fs"
#load "../prng/GeneratorBuilder.fs"

type RiskCombat =
  {
    Attacker : int list
    Defender : int list
  }

type RiskCombatOutcome =
  {
    AttackerLosses : int
    DefenderLosses : int
  }

module RiskCombat =

  let outcome (x : RiskCombat) =
    x.Attacker
    |> Seq.sortDescending
    |> Seq.zip
      (
        x.Defender
        |> Seq.sortDescending
      )
    |> Seq.fold
      (fun state (attackerRoll, defenderRoll) ->
        if attackerRoll > defenderRoll
        then
          {
            state with
              AttackerLosses = state.AttackerLosses + 1
          }
        else
          {
            state with
              DefenderLosses = state.DefenderLosses + 1
          })
      {
        AttackerLosses = 0
        DefenderLosses = 0
      }

open Prng

let d6 = Generator.between 1 6

let combats =
  generator {
    let! attacker1 = d6
    let! attacker2 = d6
    let! attacker3 = d6

    let! defender1 = d6
    let! defender2 = d6

    return
      {
        Attacker = [ attacker1; attacker2; attacker3 ]
        Defender = [ defender1; defender2 ]
      }
  }

let outcomes =
  combats
  |> Generator.map RiskCombat.outcome

for outcome in Generator.start 0 outcomes do
  printfn "%A" outcome
