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
            "onchargingchange" =@ T<Dom.Event> ^-> T<unit>
            "onchargingtimechange" =@ T<unit> ^-> T<unit>
            "onchargingtimechange" =@ T<Dom.Event> ^-> T<unit>
            "ondischargingtimechange" =@ T<unit> ^-> T<unit>
            "ondischargingtimechange" =@ T<Dom.Event> ^-> T<unit>
            "onlevelchange" =@ T<unit> ^-> T<unit>
            "onlevelchange" =@ T<Dom.Event> ^-> T<unit>
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
