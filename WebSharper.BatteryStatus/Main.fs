namespace WebSharper.BatteryStatus

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator
open System.IO

module Definition =
    
    let BatteryManager =
        Class "BatteryManager"
        |=> Inherits T<Dom.EventTarget>  
        |+> Instance [
            "charging" =? T<bool> 
            "chargingTime" =? T<float> 
            "dischargingTime" =? T<float> 
            "level" =? T<float> 

            "onchargingchange" =@ T<unit> ^-> T<unit>
            |> ObsoleteWithMessage "Use OnChargingChange instead"
            "onchargingchange" =@ T<Dom.Event> ^-> T<unit>
            |> WithSourceName "OnChargingChange"
            "onchargingtimechange" =@ T<unit> ^-> T<unit>
            |> ObsoleteWithMessage "Use OnChargingTimeChange instead"
            "onchargingtimechange" =@ T<Dom.Event> ^-> T<unit>
            |> WithSourceName "OnChargingTimeChange"
            "ondischargingtimechange" =@ T<unit> ^-> T<unit>
            |> ObsoleteWithMessage "Use OnDischargingTimeChange instead"
            "ondischargingtimechange" =@ T<Dom.Event> ^-> T<unit>
            |> WithSourceName "OnDischargingTimeChange"
            "onlevelchange" =@ T<unit> ^-> T<unit>
            |> ObsoleteWithMessage "Use OnLevelChange instead"
            "onlevelchange" =@ T<Dom.Event> ^-> T<unit>
            |> WithSourceName "OnLevelChange"
        ]

    let Navigator =
        Class "Navigator"
        |+> Instance [
            "getBattery" => T<unit> ^-> T<Promise<_>>[BatteryManager] 
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.BatteryStatus" [
                Navigator
                BatteryManager
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
