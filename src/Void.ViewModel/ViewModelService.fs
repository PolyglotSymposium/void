namespace Void.ViewModel

open Void.Core

type ViewModelService() =
    let mutable _viewModel = ViewModel.defaultViewModel

    member x.handleCommand =
        function
        | CoreCommand.InitializeVoid ->
            VMEvent.ViewModelInitialized _viewModel :> Message
        | CoreCommand.Display _ ->
            notImplemented
        | _ ->
            noMessage

    member x.subscribe (bus : Bus) =
        bus.subscribe x.handleCommand