﻿/// Serialization and deserialization for types/values of the Stock component.
module StorageMachine.Stock.Serialization

open Thoth.Json.Net
open FsToolkit.ErrorHandling
open StorageMachine
open Common
open Bin
open Stock

/// JSON serialization of a bin.
let encoderBin : Encoder<Bin> = fun bin ->
    Encode.object [
        "binIdentifier", (let (BinIdentifier identifier) = bin.Identifier in Encode.string identifier)
        "content", (Encode.option (fun (PartNumber partNumber) -> Encode.string partNumber) bin.Content)
    ]

/// JSON deserialization of a bin identifier.
let decoderBinIdentifier : Decoder<BinIdentifier> =
    Decode.string
    |> Decode.andThen (fun s ->
        match BinIdentifier.make s with
        | Ok binIdentifier -> Decode.succeed binIdentifier
        | Error validationMessage -> Decode.fail validationMessage
    )
// EXERCISE: Is this decoder in the right place (in the architecture) here?
    
/// JSON deserialization of a part number.
let decoderPartNumber : Decoder<PartNumber> =
    Decode.string
    |> Decode.andThen (fun s ->
        match PartNumber.make s with
        | Ok partNumber -> Decode.succeed partNumber
        | Error validationMessage -> Decode.fail validationMessage
    )

let decoderBin : Decoder<Bin> =
    Decode.object (fun get ->
        {
            Identifier = get.Required.Field "identifier" decoderBinIdentifier
            Content = get.Optional.Field "content" decoderPartNumber
        }
    )

/// JSON serialization of a stock product.
// VRAAG: hoe werkt dit?
let encoderProduct : Encoder<Product> = fun product ->
    Encode.object [
        "partNumber", (let (ProductPartNumber partNumber) = product in Encode.string partNumber)
        // "partNumber", (let (Product partNumber) = product in Encode.string (PartNumber.rawValue partNumber))
    ]

/// JSON serialization of a complete products overview.
let encoderProductsOverview : Encoder<ProductsOverview> = fun productsOverview ->
    Encode.seq [
        for (product, quantity) in productsOverview do
            yield Encode.object [
                "product", encoderProduct product
                "total", Encode.int quantity
            ]
    ]