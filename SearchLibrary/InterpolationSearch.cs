using System;

namespace SearchLibrary
{
    /// <summary>
    /// Interpolation Search implementation over a sorted Order array.
    ///
    /// Algorithm:
    ///   Improves upon Binary Search for uniformly distributed data by using
    ///   a probing formula to estimate where the target is likely to be:
    ///
    ///     probe = low + ((target - arr[low]) * (high - low)) / (arr[high] - arr[low])
    ///
    ///   If the distribution is NOT uniform, the probe may be a poor guess,
    ///   but the algorithm remains correct.
    ///
    /// Time Complexity  : O(log log n) – average case with uniform distribution
    ///                    O(n)         – worst case (skewed distribution)
    /// Space Complexity : O(1)        – iterative, constant extra memory
    ///
    /// Pre-condition    : orders[] must be sorted ascending by OrderId.
    /// </summary>
    public class InterpolationSearch : ISearchAlgorithm
    {
        /// <inheritdoc />
        public string AlgorithmName => "Interpolation Search";

        /// <summary>
        /// Searches a sorted Order array for the element whose OrderId equals
        /// <paramref name="targetOrderId"/> using the interpolation formula.
        /// </summary>
        /// <param name="orders">Sorted (ascending) array of Order objects. Must not be null.</param>
        /// <param name="targetOrderId">The OrderId to locate.</param>
        /// <returns>Zero-based index of the found Order, or -1 if absent.</returns>
        /// <exception cref="ArgumentNullException">Thrown when orders is null.</exception>
        public int Search(Order[] orders, int targetOrderId)
        {
            // ── Guard clause ─────────────────────────────────────────────
            if (orders is null)
                throw new ArgumentNullException(nameof(orders), "Orders array must not be null.");

            // ── Edge case: empty array ────────────────────────────────────
            if (orders.Length == 0)
                return -1;

            // ── Search bounds ─────────────────────────────────────────────
            int low  = 0;                           // N1 – initialise bounds
            int high = orders.Length - 1;

            // N2 – loop guard:
            //   C1: low <= high                (range is valid)
            //   C2: targetOrderId >= orders[low].OrderId  (target in range – lower)
            //   C3: targetOrderId <= orders[high].OrderId (target in range – upper)
            while (low <= high
                   && targetOrderId >= orders[low].OrderId
                   && targetOrderId <= orders[high].OrderId)
            {
                // N3 – handle degenerate case: single element or equal bounds
                if (low == high)
                {
                    // C4: orders[low].OrderId == targetOrderId
                    return (orders[low].OrderId == targetOrderId) ? low : -1; // N4
                }

                // N5 – compute interpolated probe index
                int rangeDenominator = orders[high].OrderId - orders[low].OrderId;
                int probe = low + ((targetOrderId - orders[low].OrderId) * (high - low))
                                  / rangeDenominator;

                // N6 – check exact match
                if (orders[probe].OrderId == targetOrderId)
                    return probe;                   // N7 – found

                // N8 – narrow the search window
                if (orders[probe].OrderId < targetOrderId)
                    low = probe + 1;                // N9 – move right
                else
                    high = probe - 1;               // N10 – move left
            }

            return -1;                              // N11 – not found
        }
    }
}
