using System;
using FluentAssertions;
using NesEmulator.Memory;
using Xunit;

namespace NesEmulator.UnitTests.MemoryTests
{
    [Trait("Category", "Unit")]
    public class MemoryBlockTests
    {
        private ushort _start;
        private ushort _end;

        public MemoryBlockTests()
        {
            _start = 0x00;
            _end = 0x100;
        }

        private MemoryBlock CreateSut()
        {
            var range = new MemoryRange(_start, _end);
            return new MemoryBlock(range);
        }

        [Fact]
        public void ctor_CalledWithEndAddressBeforeStartAddress_WillThrow()
        {
            _end = 0;
            _start = 1;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Read_ToAddressLessThanStart_WillThrowIndexOutOfRange()
        {
            _start = 0x100;
            _end = 0x200;
            ushort address = 0x00;
            
            var sut = CreateSut();

            Action action = () => sut.Read(address);

            action.Should().Throw<IndexOutOfRangeException>();
        }
        
        [Fact]
        public void Write_ToAddressLessThanStart_WillThrowIndexOutOfRange()
        {
            _start = 0x100;
            _end = 0x200;
            ushort address = 0xFF;
            
            var sut = CreateSut();

            Action action = () => sut.Write(address, 0x00);

            action.Should().Throw<IndexOutOfRangeException>();
        }
        
        [Fact]
        public void Peek_ToAddressLessThanStart_WillThrowIndexOutOfRange()
        {
            _start = 0x100;
            _end = 0x200;
            ushort address = 0x00;
            
            var sut = CreateSut();

            Action action = () => sut.Peek(address);

            action.Should().Throw<IndexOutOfRangeException>();
        }
        
        [Fact]
        public void Read_ToAddressGreaterThanEnd_WillThrowIndexOutOfRange()
        {
            _start = 0x100;
            _end = 0x200;
            ushort address = 0x201;
            
            var sut = CreateSut();

            Action action = () => sut.Read(address);

            action.Should().Throw<IndexOutOfRangeException>();
        }
        
        [Fact]
        public void Write_ToAddressGreaterThanEnd_WillThrowIndexOutOfRange()
        {
            _start = 0x100;
            _end = 0x200;
            ushort address = 0x201;
            
            var sut = CreateSut();

            Action action = () => sut.Write(address, 0xFF);

            action.Should().Throw<IndexOutOfRangeException>();
        }
        
        [Fact]
        public void Peek_ToAddressGreaterThanEnd_WillThrowIndexOutOfRange()
        {
            _start = 0x100;
            _end = 0x200;
            ushort address = 0x201;
            
            var sut = CreateSut();

            Action action = () => sut.Peek(address);

            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void Read_ToUnwrittenAddress_ReturnsZero()
        {
            var sut = CreateSut();

            var value = sut.Read(0x00AF);

            value.Should().Be(0x00);
        }

        [Fact]
        public void Peek_ToUnwrittenAddress_ReturnsZero()
        {
            var sut = CreateSut();

            var value = sut.Peek(0x003C);

            value.Should().Be(0x00);
        }

        [Fact]
        public void ReadAddress_AfterWriteToSameAddress_ReturnsPreviousValue()
        {
            ushort address1 = 0x01;
            ushort address2 = 0x13;
            ushort address3 = 0xF2;

            byte value1 = 0x35;
            byte value2 = 0x62;
            byte value3 = 0xF9;

            var sut = CreateSut();
            
            sut.Write(address1, value1);
            sut.Write(address2, value2);
            sut.Write(address3, value3);

            sut.Read(address1).Should().Be(value1);
            sut.Read(address2).Should().Be(value2);
            sut.Read(address3).Should().Be(value3);
        }
        
        [Fact]
        public void PeekAddress_AfterWriteToSameAddress_ReturnsPreviousValue()
        {
            ushort address1 = 0x01;
            ushort address2 = 0x13;
            ushort address3 = 0xF2;

            byte value1 = 0x35;
            byte value2 = 0x62;
            byte value3 = 0xF9;

            var sut = CreateSut();
            
            sut.Write(address1, value1);
            sut.Write(address2, value2);
            sut.Write(address3, value3);

            sut.Peek(address1).Should().Be(value1);
            sut.Peek(address2).Should().Be(value2);
            sut.Peek(address3).Should().Be(value3);
        }
    }
}