using System;

namespace SearchLibrary
{
    /// <summary>
    /// Contract that every search algorithm must fulfil.
    /// Follows the Interface Segregation Principle (ISP) and the
    /// Dependency Inversion Principle (DIP) from SOLID.
    /// </summary>
    public interface ISearchAlgorithm
    {
        /// <summary>
        /// Searches a sorted array of Orders for the given key (OrderId).
        /// </summary>
        /// <param name="orders">Sorted (ascending by OrderId) array of Order objects.</param>
        /// <param name="targetOrderId">The OrderId to search for.</param>
        /// <returns>
        /// Zero-based index of the matching Order, or -1 if not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="orders"/> is null.</exception>
        int Search(Order[] orders, int targetOrderId);

        /// <summary>Human-readable name of the algorithm (e.g., "Binary Search").</summary>
        string AlgorithmName { get; }
    }
}
