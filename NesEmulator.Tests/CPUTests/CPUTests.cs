﻿using System;
using FakeItEasy;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    [Trait("Category", "Unit")]
    public class CPUTests
    {
        public CPUTests()
        {
            _memoryBus = A.Fake<IMemoryBus>();
        }

        private IMemoryBus _memoryBus;

        private CPU CreateSut()
        {
            return new CPU(_memoryBus);
        }

        [Fact]
        public void ctor_CalledWithNullMemoryReference_WillThrow()
        {
            _memoryBus = null;

            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }
    }
}