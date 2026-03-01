using System;
using Xunit;
using SearchLibrary;

namespace SearchLibrary.Tests
{
    /// <summary>
    /// xUnit test suite for <see cref="JumpSearch"/>.
    ///
    /// Covers:
    ///   - Positive (normal found) cases
    ///   - Negative (not found) cases
    ///   - Boundary cases (first/last element, single element)
    ///   - Edge cases (empty array, array size = 1 block, large array)
    ///   - MC/DC driven cases for both loop and exact-match conditions
    ///   - Exception cases (null input)
    /// </summary>
    public class JumpSearchTests
    {
        // ─────────────────────────────────────────────
        // Shared helper
        // ─────────────────────────────────────────────

        private static Order[] BuildOrderArray(params int[] ids)
        {
            var orders = new Order[ids.Length];
            for (int i = 0; i < ids.Length; i++)
                orders[i] = new Order(ids[i], $"Customer_{ids[i]}", 75m * ids[i], DateTime.Today);
            return orders;
        }

        private readonly JumpSearch _sut = new JumpSearch();

        // ─────────────────────────────────────────────
        // 1. Algorithm Name
        // ─────────────────────────────────────────────

        [Fact]
        public void AlgorithmName_ShouldBeJumpSearch()
        {
            Assert.Equal("Jump Search", _sut.AlgorithmName);
        }

        // ─────────────────────────────────────────────
        // 2. Positive Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_TargetInFirstBlock_ReturnsCorrectIndex()
        {
            // sqrt(9) = 3 → blocks: [0-2], [3-5], [6-8]
            Order[] orders = BuildOrderArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.Equal(1, _sut.Search(orders, 2));
        }

        [Fact]
        public void Search_TargetInMiddleBlock_ReturnsCorrectIndex()
        {
            Order[] orders = BuildOrderArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.Equal(4, _sut.Search(orders, 5));
        }

        [Fact]
        public void Search_TargetInLastBlock_ReturnsCorrectIndex()
        {
            Order[] orders = BuildOrderArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.Equal(8, _sut.Search(orders, 9));
        }

        [Fact]
        public void Search_TargetAtVeryFirst_ReturnsZero()
        {
            Order[] orders = BuildOrderArray(10, 20, 30, 40, 50);
            Assert.Equal(0, _sut.Search(orders, 10));
        }

        [Fact]
        public void Search_TargetAtVeryLast_ReturnsLastIndex()
        {
            Order[] orders = BuildOrderArray(10, 20, 30, 40, 50);
            Assert.Equal(4, _sut.Search(orders, 50));
        }

        // ─────────────────────────────────────────────
        // 3. Negative Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_TargetNotPresent_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(10, 20, 30, 40);
            Assert.Equal(-1, _sut.Search(orders, 25));
        }

        [Fact]
        public void Search_TargetSmallerThanAll_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 1));
        }

        [Fact]
        public void Search_TargetLargerThanAll_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 999));
        }

        // ─────────────────────────────────────────────
        // 4. Boundary Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_SingleElementArray_Match()
        {
            Order[] orders = BuildOrderArray(99);
            Assert.Equal(0, _sut.Search(orders, 99));
        }

        [Fact]
        public void Search_SingleElementArray_NoMatch()
        {
            Order[] orders = BuildOrderArray(99);
            Assert.Equal(-1, _sut.Search(orders, 50));
        }

        [Fact]
        public void Search_TwoElementArray_FindFirst()
        {
            Order[] orders = BuildOrderArray(5, 10);
            Assert.Equal(0, _sut.Search(orders, 5));
        }

        [Fact]
        public void Search_TwoElementArray_FindSecond()
        {
            Order[] orders = BuildOrderArray(5, 10);
            Assert.Equal(1, _sut.Search(orders, 10));
        }

        // ─────────────────────────────────────────────
        // 5. Edge Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_EmptyArray_ReturnsNegativeOne()
        {
            Assert.Equal(-1, _sut.Search(Array.Empty<Order>(), 10));
        }

        [Fact]
        public void Search_ArraySizeExactlyOneBlock()
        {
            // sqrt(4) = 2, all 4 elements fit in exactly 2 blocks
            Order[] orders = BuildOrderArray(2, 4, 6, 8);
            Assert.Equal(2, _sut.Search(orders, 6));
        }

        [Fact]
        public void Search_LargeArray_TargetFound()
        {
            var ids = new int[400];
            for (int i = 0; i < 400; i++) ids[i] = (i + 1) * 5;
            Order[] orders = BuildOrderArray(ids);
            // OrderId 1000 is at index 199
            Assert.Equal(199, _sut.Search(orders, 1000));
        }

        [Fact]
        public void Search_LargeArray_TargetNotFound()
        {
            var ids = new int[400];
            for (int i = 0; i < 400; i++) ids[i] = (i + 1) * 5;
            Order[] orders = BuildOrderArray(ids);
            Assert.Equal(-1, _sut.Search(orders, 1001));
        }

        // ─────────────────────────────────────────────
        // 6. Exception / Null Safety
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_NullArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Search(null!, 5));
        }

        // ─────────────────────────────────────────────
        // 7. MC/DC Cases
        // ─────────────────────────────────────────────
        // Jump loop: C1(jumpPos < n) AND C2(orders[jumpPos].OrderId < target)
        // Linear match: C3(orders[i].OrderId == target)

        // TC-MCDC-JS-1: C1=T, C2=T → jump loop executes (target found ahead)
        [Fact]
        public void MCDC_JS1_JumpLoopExecutes_TargetFound()
        {
            Order[] orders = BuildOrderArray(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.Equal(7, _sut.Search(orders, 8));
        }

        // TC-MCDC-JS-2: C1=F → jump loop exits because we're at array end (large target)
        [Fact]
        public void MCDC_JS2_JumpLoopExits_TargetLargerThanAll()
        {
            Order[] orders = BuildOrderArray(1, 2, 3, 4, 5);
            Assert.Equal(-1, _sut.Search(orders, 999));
        }

        // TC-MCDC-JS-3: C1=T, C2=F → first element already >= target (target in block 0)
        [Fact]
        public void MCDC_JS3_JumpLoopSkipped_TargetInFirstBlock()
        {
            Order[] orders = BuildOrderArray(10, 20, 30, 40, 50);
            Assert.Equal(0, _sut.Search(orders, 10));
        }

        // TC-MCDC-JS-4: C3=T → linear scan finds the element
        [Fact]
        public void MCDC_JS4_LinearScan_FindsTarget()
        {
            Order[] orders = BuildOrderArray(5, 10, 15, 20, 25);
            Assert.Equal(2, _sut.Search(orders, 15));
        }

        // TC-MCDC-JS-5: C3=F for all → linear scan exhausts block without finding
        [Fact]
        public void MCDC_JS5_LinearScan_DoesNotFindTarget()
        {
            Order[] orders = BuildOrderArray(10, 20, 30, 40);
            Assert.Equal(-1, _sut.Search(orders, 15));
        }
    }
}
