/// Provides a model of stock focused on bins stored in the Storage Machine.
module StorageMachine.Stock.Bin

open StorageMachine
open Common

/// The Storage Machine is specialized in storing plastic bins which can hold a single product. A bin may be empty.
type Bin = {
    Identifier : BinIdentifier
    Content : Option<PartNumber>
}

/// Indicates whether the given bin is empty.
let isEmpty (bin: Bin) =
    match bin with
    | { Content = None } -> true
    | _ -> false

/// Indicates whether the given bin is not empty, i.e. actually contains a product.
let isNotEmpty = not << isEmpty

let isNotEmptyOption (bin: Bin) =
    if isNotEmpty bin then
        bin.Content
    else None
