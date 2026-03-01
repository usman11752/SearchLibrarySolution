using System;
using Xunit;
using SearchLibrary;

namespace SearchLibrary.Tests
{
    /// <summary>
    /// xUnit test suite for <see cref="InterpolationSearch"/>.
    ///
    /// Covers:
    ///   - Positive (normal found) cases
    ///   - Negative (not found) cases
    ///   - Boundary cases (first/last element, single element)
    ///   - Edge cases (empty array, all-same keys, skewed distribution)
    ///   - MC/DC driven cases for the three-condition loop guard
    ///   - Exception cases (null input)
    /// </summary>
    public class InterpolationSearchTests
    {
        // ─────────────────────────────────────────────
        // Shared helper
        // ─────────────────────────────────────────────

        private static Order[] BuildOrderArray(params int[] ids)
        {
            var orders = new Order[ids.Length];
            for (int i = 0; i < ids.Length; i++)
                orders[i] = new Order(ids[i], $"Customer_{ids[i]}", 50m * ids[i], DateTime.Today);
            return orders;
        }

        private readonly InterpolationSearch _sut = new InterpolationSearch();

        // ─────────────────────────────────────────────
        // 1. Algorithm Name
        // ─────────────────────────────────────────────

        [Fact]
        public void AlgorithmName_ShouldBeInterpolationSearch()
        {
            Assert.Equal("Interpolation Search", _sut.AlgorithmName);
        }

        // ─────────────────────────────────────────────
        // 2. Positive Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_TargetAtMiddle_ReturnsCorrectIndex()
        {
            Order[] orders = BuildOrderArray(10, 20, 30, 40, 50);
            Assert.Equal(2, _sut.Search(orders, 30));
        }

        [Fact]
        public void Search_TargetAtFirstPosition_ReturnsZero()
        {
            Order[] orders = BuildOrderArray(5, 15, 25, 35);
            Assert.Equal(0, _sut.Search(orders, 5));
        }

        [Fact]
        public void Search_TargetAtLastPosition_ReturnsLastIndex()
        {
            Order[] orders = BuildOrderArray(5, 15, 25, 35);
            Assert.Equal(3, _sut.Search(orders, 35));
        }

        [Fact]
        public void Search_UniformDistribution_CorrectIndex()
        {
            // Uniform interval → interpolation excels
            Order[] orders = BuildOrderArray(100, 200, 300, 400, 500, 600, 700, 800, 900, 1000);
            Assert.Equal(6, _sut.Search(orders, 700));
        }

        // ─────────────────────────────────────────────
        // 3. Negative Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_TargetBelowRange_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 5));
        }

        [Fact]
        public void Search_TargetAboveRange_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 999));
        }

        [Fact]
        public void Search_TargetBetweenElements_ReturnsNegativeOne()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 15));
        }

        // ─────────────────────────────────────────────
        // 4. Boundary Cases
        // ─────────────────────────────────────────────

        [Fact]
        public void Search_SingleElementMatch()
        {
            Order[] orders = BuildOrderArray(7);
            Assert.Equal(0, _sut.Search(orders, 7));
        }

        [Fact]
        public void Search_SingleElementNoMatch()
        {
            Order[] orders = BuildOrderArray(7);
            Assert.Equal(-1, _sut.Search(orders, 99));
        }

        [Fact]
        public void Search_TwoElements_FindFirst()
        {
            Order[] orders = BuildOrderArray(1, 2);
            Assert.Equal(0, _sut.Search(orders, 1));
        }

        [Fact]
        public void Search_TwoElements_FindSecond()
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
            Assert.Equal(-1, _sut.Search(Array.Empty<Order>(), 10));
        }

        [Fact]
        public void Search_SkewedDistribution_StillCorrect()
        {
            // Non-uniform: 1, 2, 3, 1000 → probe may be wrong but algorithm still correct
            Order[] orders = BuildOrderArray(1, 2, 3, 1000);
            Assert.Equal(3, _sut.Search(orders, 1000));
        }

        [Fact]
        public void Search_LargeUniformArray()
        {
            var ids = new int[500];
            for (int i = 0; i < 500; i++) ids[i] = (i + 1) * 10;
            Order[] orders = BuildOrderArray(ids);
            Assert.Equal(249, _sut.Search(orders, 2500));
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
        // Loop guard: C1(low<=high) AND C2(target>=orders[low]) AND C3(target<=orders[high])

        // TC-MCDC-IS-1: C1=F, C2=T, C3=T → loop never entered (exhausted bounds)
        [Fact]
        public void MCDC_IS1_LowExceedsHigh_LoopNotEntered()
        {
            // Searching a non-existent value that causes bounds to cross
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 15));
        }

        // TC-MCDC-IS-2: C1=T, C2=F, C3=T → target below low
        [Fact]
        public void MCDC_IS2_TargetBelowLow_LoopNotEntered()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 5));
        }

        // TC-MCDC-IS-3: C1=T, C2=T, C3=F → target above high
        [Fact]
        public void MCDC_IS3_TargetAboveHigh_LoopNotEntered()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(-1, _sut.Search(orders, 35));
        }

        // TC-MCDC-IS-4: All three conditions true → loop entered, target found
        [Fact]
        public void MCDC_IS4_AllConditionsTrue_TargetFound()
        {
            Order[] orders = BuildOrderArray(10, 20, 30);
            Assert.Equal(1, _sut.Search(orders, 20));
        }

        // TC-MCDC-IS-5: low == high degenerate case → single-element block match
        [Fact]
        public void MCDC_IS5_LowEqualsHigh_Match()
        {
            Order[] orders = BuildOrderArray(42);
            Assert.Equal(0, _sut.Search(orders, 42));
        }

        // TC-MCDC-IS-6: low == high degenerate case → single-element block no match
        [Fact]
        public void MCDC_IS6_LowEqualsHigh_NoMatch()
        {
            Order[] orders = BuildOrderArray(42);
            Assert.Equal(-1, _sut.Search(orders, 99));
        }
    }
}
