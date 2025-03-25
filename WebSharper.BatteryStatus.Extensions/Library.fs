namespace WebSharper.BatteryStatus

open WebSharper
open WebSharper.JavaScript

[<JavaScript;AutoOpen>]
module Extensions = 
    type Navigator with
        [<Inline "$this.getBattery()">]
        member this.GetBattery():Promise<BatteryManager> = X<Promise<BatteryManager>>