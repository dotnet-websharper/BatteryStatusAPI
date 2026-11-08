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

    /// Formats a duration in seconds as minutes/hours, or "N/A" when the value is
    /// not finite (the API reports Infinity when the estimate does not apply).
    let formatDuration (seconds: float) =
        if seconds > 0.0 && seconds < 1e12 then
            let minutes = seconds / 60.0
            if minutes >= 60.0 then sprintf "%.1fh" (minutes / 60.0)
            else sprintf "%.0fm" minutes
        else "N/A"

    let updateBatteryInfo (battery: BatteryManager) =
        let chargingText = if battery.Charging then "Yes ⚡" else "No"
        batteryStatus.Value <-
            sprintf "Battery Level: %d%% — Charging: %s — Time to Full Charge: %s — Time to Empty: %s"
                (int (battery.Level * 100.0))
                chargingText
                (formatDuration battery.ChargingTime)
                (formatDuration battery.DischargingTime)

    let initializeBattery() =
        try 
            let batteryPromise = JS.Window.Navigator.GetBattery()
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
