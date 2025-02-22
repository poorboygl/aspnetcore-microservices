﻿using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using MongoDB.Driver;
using Shared.Enums.Inventory;

namespace Inventory.Product.API.Persistence;
public class InventoryDbSeed
{
    public async Task SeedDataAsync(IMongoClient mongoClient, DatabaseSettings settings) 
    {
        var databaseName = settings.DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);
        var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");
        if (await inventoryCollection.EstimatedDocumentCountAsync() == 0) 
        {
            await inventoryCollection.InsertManyAsync(GetPreconfiguredIventoryEntries());
        }
    }

    private IEnumerable<InventoryEntry> GetPreconfiguredIventoryEntries() => new List<InventoryEntry>()
    {
        new()
            {
                Quantity = 10,
                //DocumentNo = Guid.NewGuid().ToString(),
                ItemNo = "Lotus",
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                DocumentType = EDocumentType.Purchase
            },
            new()
            {
                ItemNo = "Cadillac",
                Quantity = 10,
                //DocumentNo = Guid.NewGuid().ToString(),
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                DocumentType = EDocumentType.Purchase
            },
    };

}

