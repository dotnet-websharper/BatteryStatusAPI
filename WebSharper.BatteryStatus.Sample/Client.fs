namespace WebSharper.BatteryStatus.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.BatteryStatus

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let batteryStatus = Var.Create "Checking battery status..."
    
    let updateBatteryInfo (battery: BatteryManager) =
        let batteryText = 
            sprintf "Battery Level: %d%%<br>Charging: %s<br>Time to Full Charge: %s<br>Time to Empty: %s"
                (int (battery.Level * 100.0))
                (if battery.Charging then "Yes ⚡" else "No")
                (if battery.ChargingTime > 0.0 then sprintf "%fs" battery.ChargingTime else "N/A")
                (if battery.DischargingTime > 0.0 then sprintf "%fs" battery.DischargingTime else "N/A")

        JS.Document.GetElementById("battery-status").InnerHTML <- batteryText

    let initializeBattery() =
        try 
            let batteryPromise = As<Navigator>(JS.Window.Navigator).GetBattery()
            batteryPromise.Then(fun (battery: BatteryManager) ->
                updateBatteryInfo battery

                battery.AddEventListener("levelchange", fun (event:Dom.Event) -> updateBatteryInfo battery)
                battery.AddEventListener("chargingchange", fun (event:Dom.Event) -> updateBatteryInfo battery)
                battery.AddEventListener("chargingtimechange", fun (event:Dom.Event) -> updateBatteryInfo battery)
                battery.AddEventListener("dischargingtimechange", fun (event:Dom.Event) -> updateBatteryInfo battery)
            ) |> ignore

        with error ->
            batteryStatus.Value <- $"{error.Message}"

    [<SPAEntryPoint>]
    let Main () =

        initializeBattery()

        IndexTemplate.Main()
            .BatteryStatus(batteryStatus.V)
            .Doc()
        |> Doc.RunById "main"
