﻿using CommonLibrary.Entities;

namespace Inventory.Data.Entities;

public class CatalogItem : IEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}
