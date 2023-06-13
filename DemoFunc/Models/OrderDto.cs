using System;

namespace DemoFunc.Models
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }

        public DateTime TimePlaced { get; set; }

        public int OrderStatus { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
