using System;
using Xunit;
using SearchLibrary;

namespace SearchLibrary.Tests
{
    /// <summary>
    /// xUnit test suite for <see cref="BinarySearch"/>.
    ///
    /// Covers:
    ///   - Positive (normal found) cases
    ///   - Negative (not found) cases
    ///   - Boundary cases (first/last element, single element)
    ///   - Edge cases (empty array, null input)
    ///   - MC/DC driven cases (all decision outcomes exercised)
    /// </summary>
    public class BinarySearchTests
    {
        // ─────────────────────────────────────────────
        // Shared test fixture helpers
        // ─────────────────────────────────────────────

        private static Order[] BuildOrderArray(params int[] ids)
        {
            var orders = new Order[ids.Length];
            for (int i = 0; i < ids.Length; i++)
                orders[i] = new Order(ids[i], $"Customer_{ids[i]}", 100m * ids[i], DateTime.Today);
            return orders;
        }

        private readonly BinarySearch _sut = new BinarySearch();

        // ─────────────────────────────────────────────
        // 1. Algorithm Name
        // ─────────────────────────────────────────────

        [Fact]
        public void AlgorithmName_ShouldBeBinarySearch()
        {
            Assert.Equal("Binary Search", _sut.AlgorithmName);
        }

        // ─────────────────────────────────────────────
        // 2. Positive Cases – target IS in the array
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_TargetInMiddle_ReturnsCorrectIndex()
        {
            // Arrange
            Order[] orders = BuildOrderArray(10, 20, 30, 40, 50);
            // Act
            int result = _sut.Search(orders, 30);
            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Search_TargetAtFirstPosition_ReturnsZero()
        {
            Order[] orders = BuildOrderArray(5, 10, 15);
            Assert.Equal(0, _sut.Search(orders, 5));
        }

        [Fact]
        public void Search_TargetAtLastPosition_ReturnsLastIndex()
        {
            Order[] orders = BuildOrderArray(5, 10, 15);
            Assert.Equal(2, _sut.Search(orders, 15));
        }

        [Fact]
        public void Search_TargetInLeftHalf_ReturnsCorrectIndex()
        {
            Order[] orders = BuildOrderArray(1, 3, 5, 7, 9, 11, 13);
            Assert.Equal(1, _sut.Search(orders, 3));
        }

        [Fact]
        public void Search_TargetInRightHalf_ReturnsCorrectIndex()
        {
            Order[] orders = BuildOrderArray(1, 3, 5, 7, 9, 11, 13);
            Assert.Equal(5, _sut.Search(orders, 11));
        }

        // ─────────────────────────────────────────────
        // 3. Negative Cases – target is NOT in the array
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_TargetNotPresent_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
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
        public void Search_SingleElementArray_MatchFound()
        {
            Order[] orders = BuildOrderArray(42);
            Assert.Equal(0, _sut.Search(orders, 42));
        }

        [Fact]
        public void Search_SingleElementArray_NoMatch_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(42);
            Assert.Equal(-1, _sut.Search(orders, 99));
        }

        [Fact]
        public void Search_TwoElementArray_FindFirst()
        {
            Order[] orders = BuildOrderArray(1, 2);
            Assert.Equal(0, _sut.Search(orders, 1));
        }

        [Fact]
        public void Search_TwoElementArray_FindSecond()
        {
            Order[] orders = BuildOrderArray(1, 2);
            Assert.Equal(1, _sut.Search(orders, 2));
        }

        // ─────────────────────────────────────────────
        // 5. Edge Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_EmptyArray_ReturnsNegativeOne()
        {
            Order[] orders = Array.Empty<Order>();
            Assert.Equal(-1, _sut.Search(orders, 10));
        }

        [Fact]
        public void Search_LargeArray_TargetFound()
        {
            // 1000 orders with ids 1..1000
            var ids = new int[1000];
            for (int i = 0; i < 1000; i++) ids[i] = i + 1;
            Order[] orders = BuildOrderArray(ids);
            Assert.Equal(499, _sut.Search(orders, 500));
        }

        [Fact]
        public void Search_LargeArray_TargetNotFound()
        {
            var ids = new int[1000];
            for (int i = 0; i < 1000; i++) ids[i] = i + 1;
            Order[] orders = BuildOrderArray(ids);
            Assert.Equal(-1, _sut.Search(orders, 1001));
        }

        // ─────────────────────────────────────────────
        // 6. Exception / Null Safety Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_NullArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Search(null!, 10));
        }

        // ─────────────────────────────────────────────
        // 7. MC/DC Cases – independent condition toggling
        // ─────────────────────────────────────────────

        // Decision: (bottom <= top) AND (found == false)
        // TC-MCDC-BS-1: bottom > top  ⟹ loop exits (condition C1 = false, C2 = true)
        [Fact]
        public void MCDC_BS1_LoopExitsWhenBottomExceedsTop()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            // Searching for 25 will exhaust the array without finding it
            Assert.Equal(-1, _sut.Search(orders, 25));
        }

        // TC-MCDC-BS-2: found == true ⟹ loop exits (C1 = true, C2 = false)
        [Fact]
        public void MCDC_BS2_LoopExitsWhenFound()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            // 20 is the mid element – found = true on first iteration
            Assert.Equal(1, _sut.Search(orders, 20));
        }

        // Decision: mid element < target (C3 = true → bottom = mid+1)
        [Fact]
        public void MCDC_BS3_MidLessThanTarget_MovesLowerBoundUp()
        {
            Order[] orders = BuildOrderArray(1, 2, 3, 4, 5);
            Assert.Equal(3, _sut.Search(orders, 4));
        }

        // Decision: mid element > target (C3 = false, C4 = true → top = mid-1)
        [Fact]
        public void MCDC_BS4_MidGreaterThanTarget_MovesUpperBoundDown()
        {
            Order[] orders = BuildOrderArray(1, 2, 3, 4, 5);
            Assert.Equal(1, _sut.Search(orders, 2));
        }
    }
}
