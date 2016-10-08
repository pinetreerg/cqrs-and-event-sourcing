﻿using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Types;
using System;

namespace PinetreeShop.Domain.Baskets.Events
{
    public class BasketCreated : EventBase
    {
        public BasketCreated(Guid basketId) : base(basketId)
        {
        }
    }

    public class BasketAddItemTried : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }

        public BasketAddItemTried(Guid basketId, Guid productId, string productName, decimal price, uint quantity) : base(basketId)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }
    }

    public class BasketAddItemConfirmed : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public BasketAddItemConfirmed(Guid basketId, Guid productId, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class BasketAddItemReverted : EventFailedBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }
       
        public BasketAddItemReverted(Guid basketId, Guid productId, uint quantity, string reason) : base(basketId, reason)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class BasketItemRemoved : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public BasketItemRemoved(Guid basketId, Guid productId, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
    
    public class BasketCancelled : EventBase
    {
        public BasketCancelled(Guid basketId) : base(basketId)
        {
        }
    }

    public class BasketCheckedOut : EventBase
    {
        public Address ShippintAddress { get; private set; }

        public BasketCheckedOut(Guid basketId, Address shippingAddress) : base(basketId)
        {
            ShippintAddress = shippingAddress;
        }
    }
}
