/// Data access implementation of the Repacking component.
module StorageMachine.Repacking.Repacking

open StorageMachine
open Common
open BinTree
open Repacking

/// A helper for constructing an actual bin tree from the low-level encoding of bin nesting in the simulated DB.
let private fetchBinTree (binId: BinIdentifier) : Option<BinTree> =
    let bins = SimulatedDatabase.retrieveBins ()
    let binStructure = SimulatedDatabase.retrieveBinNesting ()
    let products = SimulatedDatabase.retrieveStock ()
    
    let rec go (outerBin: SimulatedDatabase.ParentBin) : BinTree =
        // Locate all inner bins of the outer bin
        let innerBins: List<BinIdentifier> =
            match binStructure |> Map.tryFind outerBin with
            | None -> []
            | Some (SimulatedDatabase.NestedBins (oneBin, more)) -> oneBin :: more
        // The outer bin may or may not contain a product
        let product: Option<BinTree> = products |> Map.tryFind outerBin |> Option.map Product
        // Combine the outer bin, its optional product and sub-trees into a tree node
        Bin (outerBin, (Option.toList product) @ (List.map go innerBins))

    if Set.contains binId bins 
        then Some (go binId) 
        else None

/// Data access operations of the Repacking component implemented using the simulated in-memory DB.
let binTreeDataAccess = { new IBinTreeDataAccess with

    member this.RetrieveBinTree binId = fetchBinTree binId
    
}