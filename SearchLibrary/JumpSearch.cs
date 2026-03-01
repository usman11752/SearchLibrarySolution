using System;

namespace SearchLibrary
{
    /// <summary>
    /// Jump Search implementation over a sorted Order array.
    ///
    /// Algorithm:
    ///   Divides the array into blocks of size √n and jumps forward block-by-block
    ///   until the block that may contain the target is found, then performs a
    ///   linear scan within that block.
    ///
    ///   Step 1 – Jumping phase:
    ///     Advance the jump pointer by √n until orders[jumpPos].OrderId >= target
    ///     or the end of the array is reached.
    ///
    ///   Step 2 – Linear scan phase:
    ///     Scan backwards from the current jump position to the previous block start
    ///     and compare each element with the target.
    ///
    /// Time Complexity  : O(√n) – both phases combined
    /// Space Complexity : O(1)  – iterative, constant extra memory
    ///
    /// Pre-condition    : orders[] must be sorted ascending by OrderId.
    /// </summary>
    public class JumpSearch : ISearchAlgorithm
    {
        /// <inheritdoc />
        public string AlgorithmName => "Jump Search";

        /// <summary>
        /// Searches a sorted Order array for the element whose OrderId equals
        /// <paramref name="targetOrderId"/> using the jump-then-linear strategy.
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
            int n = orders.Length;              // N1
            if (n == 0)
                return -1;                      // N2 – empty, exit

            // ── Compute optimal block / jump size ─────────────────────────
            int step = (int)Math.Sqrt(n);       // N3 – block size ≈ √n

            // ── Phase 1: Jumping ──────────────────────────────────────────
            int prev     = 0;
            int jumpPos  = 0;

            // N4 – jump loop:
            //   C1: jumpPos < n               (still within bounds)
            //   C2: orders[jumpPos].OrderId < targetOrderId  (target ahead)
            while (jumpPos < n && orders[jumpPos].OrderId < targetOrderId)
            {
                prev    = jumpPos;              // N5 – save previous block start
                jumpPos = Math.Min(jumpPos + step, n - 1); // N6 – advance by step
            }

            // ── Phase 2: Linear scan within the identified block ───────────
            // N7 – linear scan:
            //   C3: jumpPos >= prev           (scan has not undershot the block start)
            //   C4: orders[jumpPos].OrderId >= targetOrderId  (not yet past target)
            for (int i = prev; i <= jumpPos; i++)
            {
                // N8 – exact match check (C5)
                if (orders[i].OrderId == targetOrderId)
                    return i;                   // N9 – found, return index
            }

            return -1;                          // N10 – not found
        }
    }
}
