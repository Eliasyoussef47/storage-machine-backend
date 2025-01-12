﻿/// Provides a model of stock focused on products stored in the Storage Machine.
module StorageMachine.Stock.Stock

open StorageMachine

open Common
open Bin

/// For the purposes of basic stock bookkeeping, a product is represented only by its 'PartNumber' and does not have any
/// other properties. This means that individual products do not have an "identity". Indeed, current software of the
/// Storage Machine does not (yet?) support serial numbers for products.
type Product = Product of PartNumber
let (|ProductPartNumber|) (product: Product) =
    let party = match product with
                | Product(p) -> p

    match party with
    | PartNumber(pn) -> pn
//let (|ProductPartNumber|) (Product product) = let (PartNumber partNumber) = product in partNumber

/// All products in the Storage Machine are counted by piece.
type Quantity = int

/// All products in the given bins.
let allProducts (bins: seq<Bin>) : List<Product> =
    bins
    |> Seq.distinctBy (fun x -> x.Identifier)
    |> Seq.choose (fun x -> x.Content)
    |> Seq.map Product
    |> Seq.toList

/// Total quantity of each of the provided products.
let totalQuantity (products: seq<Product>) : Map<Product, Quantity> =
    products
    |> Seq.countBy (fun x -> x)
    |> Map.ofSeq