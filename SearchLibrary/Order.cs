using System;

namespace SearchLibrary
{
    /// <summary>
    /// Represents a customer order in the system.
    /// The OrderId is the primary searchable key used by all search algorithms.
    /// Implements IComparable to support sorted array operations.
    /// </summary>
    public class Order : IComparable<Order>
    {
        // ─────────────────────────────────────────────
        // Properties
        // ─────────────────────────────────────────────

        /// <summary>Gets the unique identifier for this order (searchable key).</summary>
        public int OrderId { get; }

        /// <summary>Gets the name of the customer who placed the order.</summary>
        public string CustomerName { get; }

        /// <summary>Gets the total monetary value of this order.</summary>
        public decimal TotalAmount { get; }

        /// <summary>Gets the date and time the order was placed.</summary>
        public DateTime OrderDate { get; }

        // ─────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────

        /// <summary>
        /// Initialises a new Order with all required fields.
        /// </summary>
        /// <param name="orderId">Unique positive integer identifier.</param>
        /// <param name="customerName">Non-null, non-empty customer name.</param>
        /// <param name="totalAmount">Non-negative order total.</param>
        /// <param name="orderDate">Date the order was placed.</param>
        /// <exception cref="ArgumentException">Thrown when orderId is not positive.</exception>
        /// <exception cref="ArgumentNullException">Thrown when customerName is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when totalAmount is negative.</exception>
        public Order(int orderId, string customerName, decimal totalAmount, DateTime orderDate)
        {
            if (orderId <= 0)
                throw new ArgumentException("OrderId must be a positive integer.", nameof(orderId));

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentNullException(nameof(customerName), "CustomerName cannot be null or empty.");

            if (totalAmount < 0)
                throw new ArgumentOutOfRangeException(nameof(totalAmount), "TotalAmount cannot be negative.");

            OrderId = orderId;
            CustomerName = customerName.Trim();
            TotalAmount = totalAmount;
            OrderDate = orderDate;
        }

        // ─────────────────────────────────────────────
        // IComparable Implementation
        // ─────────────────────────────────────────────

        /// <summary>
        /// Compares this order to another by OrderId.
        /// Required for sorted-array search algorithms.
        /// </summary>
        public int CompareTo(Order? other)
        {
            if (other is null) return 1;           // non-null > null
            return OrderId.CompareTo(other.OrderId);
        }

        // ─────────────────────────────────────────────
        // Overrides
        // ─────────────────────────────────────────────

        public override string ToString() =>
            $"Order[Id={OrderId}, Customer={CustomerName}, Amount={TotalAmount:C}, Date={OrderDate:dd-MMM-yyyy}]";

        public override bool Equals(object? obj) =>
            obj is Order other && OrderId == other.OrderId;

        public override int GetHashCode() => OrderId.GetHashCode();
    }
}
