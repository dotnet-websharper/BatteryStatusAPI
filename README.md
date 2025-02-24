# WebSharper Battery Status API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Battery Status API](https://developer.mozilla.org/en-US/docs/Web/API/Battery_Status_API), enabling seamless access to battery information in WebSharper applications.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Battery Status API.

2. **Sample Project**:
   - Demonstrates how to use the Battery Status API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/BatteryStatusAPI/).

## Installation

To use this package in your WebSharper project, add the NuGet package:

```bash
   dotnet add package WebSharper.BatteryStatus
```

## Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/BatteryStatus.git
   cd BatteryStatus
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.BatteryStatus/WebSharper.BatteryStatus.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.BatteryStatus.Sample
   dotnet build
   dotnet run
   ```

4. Open the hosted demo to see the Sample project in action:
   [https://dotnet-websharper.github.io/BatteryStatusAPI/](https://dotnet-websharper.github.io/BatteryStatusAPI/)

## Example Usage

Below is an example of how to use the Battery Status API in a WebSharper project:

```fsharp
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

    // Function to update battery status UI.
    let updateBatteryInfo (battery: BatteryManager) =
        let batteryText =
            sprintf "Battery Level: %d%%<br>Charging: %s<br>Time to Full Charge: %s<br>Time to Empty: %s"
                (int (battery.Level * 100.0))
                (if battery.Charging then "Yes ⚡" else "No")
                (if battery.ChargingTime > 0.0 then sprintf "%fs" battery.ChargingTime else "N/A")
                (if battery.DischargingTime > 0.0 then sprintf "%fs" battery.DischargingTime else "N/A")

        JS.Document.GetElementById("battery-status").InnerHTML <- batteryText

    // Function to initialize battery API and listen for changes.
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
```

This example demonstrates how to retrieve and display battery status using the Battery Status API in a WebSharper project.

## Important Considerations

- **Limited Browser Support**: Some browsers may not fully support the Battery Status API; check [MDN Battery Status API](https://developer.mozilla.org/en-US/docs/Web/API/Battery_Status_API) for the latest compatibility information.
- **Privacy Considerations**: Some browsers restrict access to the Battery API due to privacy concerns.
