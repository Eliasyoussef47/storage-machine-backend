/// Data access implementation of the Stock component.
module StorageMachine.Stock.Stock

open StorageMachine
open Bin
open Stock

/// Data access operations of the Stock component implemented using the simulated in-memory DB.
let stockPersistence = { new IStockDataAccess with
    member this.CreateBin(arg1: Common.BinIdentifier) (arg2: Option<Common.PartNumber>): Result<Bin, StorageMachineError> =
        let newOne: Bin = {Identifier = arg1; Content= arg2;}
        let result = SimulatedDatabase.storeBin newOne
        match result with
        | Error e ->  match e with
                        | SimulatedDatabase.BinAlreadyStored -> Error BinAlreadyStored
        | Ok r -> Ok newOne
    
    member this.RetrieveAllBins () =
        SimulatedDatabase.retrieveBins ()
        |> Set.map (fun binIdentifier ->
            {
                Identifier = binIdentifier
                Content = SimulatedDatabase.retrieveStock () |> Map.tryFind binIdentifier
            }
        )
        |> Set.toList
}