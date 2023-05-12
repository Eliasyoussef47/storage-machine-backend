/// This module exposes use-cases of the Stock component as an HTTP Web service using Giraffe.
module StorageMachine.Stock.Stock

open Giraffe
open Microsoft.AspNetCore.Http
open Thoth.Json.Net
open Thoth.Json.Giraffe
open Stock

/// An overview of all bins currently stored in the Storage Machine.
let binOverview (next: HttpFunc) (ctx: HttpContext) =
    task {
        let dataAccess = ctx.GetService<IStockDataAccess> ()
        let bins = Stock.binOverview dataAccess
        return! ThothSerializer.RespondJsonSeq bins Serialization.encoderBin next ctx 
    }

/// An overview of actual stock currently stored in the Storage Machine. Actual stock is defined as all non-empty bins.
let stockOverview (next: HttpFunc) (ctx: HttpContext) =
    task {
        let dataAccess = ctx.GetService<IStockDataAccess> ()
        let bins = Stock.stockOverview dataAccess
        return! ThothSerializer.RespondJsonSeq bins Serialization.encoderBin next ctx 
    }

/// An overview of all products stored in the Storage Machine, regardless what bins contain them.
let productsInStock (next: HttpFunc) (ctx: HttpContext) =
    task {
        let productsOverview = Stock.productsInStock <| ctx.GetService<IStockDataAccess>()
        return! ThothSerializer.RespondJson productsOverview Serialization.encoderProductsOverview next ctx 
    }

let insertBin (next: HttpFunc) (ctx: HttpContext) =
    task {
        let! inputBinBody = ThothSerializer.ReadBody ctx Serialization.decoderBin
        match inputBinBody with
        | Error e -> return! RequestErrors.badRequest (text e) earlyReturn ctx
        | Ok inputBin ->
            let dataAccess = ctx.GetService<IStockDataAccess> ()
            let newBinResult = Stock.addBin dataAccess inputBin.Identifier inputBin.Content
            match newBinResult with
            | Error addError ->
                match addError with
                | BinAlreadyStored -> return! RequestErrors.badRequest (text "Bin already exists.") earlyReturn ctx
            | Ok newBin -> return! ThothSerializer.RespondJson newBin Serialization.encoderBin next ctx 
            
    }

/// Defines URLs for functionality of the Stock component and dispatches HTTP requests to those URLs.
let handlers : HttpHandler =
    choose [
        GET >=> route "/bins" >=> binOverview
        POST >=> route "/bins" >=> insertBin
        GET >=> route "/stock" >=> stockOverview
        GET >=> route "/stock/products" >=> productsInStock
    ]