using System;

namespace SearchLibrary
{
    /// <summary>
    /// Binary Search implementation over a sorted Order array.
    ///
    /// Algorithm:
    ///   Repeatedly halves the search interval. Compares the target OrderId
    ///   with the middle element and adjusts the bounds accordingly.
    ///
    /// Time Complexity  : O(log n) – best/average/worst (ignoring early exit)
    /// Space Complexity : O(1)     – iterative, no recursion stack
    ///
    /// Pre-condition    : orders[] must be sorted ascending by OrderId.
    /// </summary>
    public class BinarySearch : ISearchAlgorithm
    {
        /// <inheritdoc />
        public string AlgorithmName => "Binary Search";

        /// <summary>
        /// Searches a sorted Order array for the element whose OrderId equals
        /// <paramref name="targetOrderId"/>.
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

            // ── Edge cases ───────────────────────────────────────────────
            if (orders.Length == 0)
                return -1;   // empty array → not found

            // ── Binary search loop ───────────────────────────────────────
            int bottom = 0;                    // Node N1 – initialise bounds
            int top    = orders.Length - 1;
            int index  = -1;
            bool found = false;

            // N2 – loop condition: bottom <= top AND not yet found
            while (bottom <= top && !found)
            {
                int mid = (bottom + top) / 2;  // N3 – compute midpoint

                // N4 – check equality
                if (orders[mid].OrderId == targetOrderId)
                {
                    index = mid;
                    found = true;              // N5 – record result, exit
                }
                else if (orders[mid].OrderId < targetOrderId)
                {
                    bottom = mid + 1;          // N6 – search right half
                }
                else
                {
                    top = mid - 1;             // N7 – search left half
                }
            }

            return index;                      // N8 – return found index or -1
        }
    }
}
