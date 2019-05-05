using System;
using System.Collections.Generic;
using System.IO;

namespace NesEmulator.RomMappers
{
    internal abstract class ROM : IReadWrite, IReadWriteChr, IDisposable
    {
        private static readonly Dictionary<int, Func<RomHeader, Memory<byte>, ROM>> _mapperFactories = new Dictionary<int, Func<RomHeader, Memory<byte>, ROM>>()
        {
            { 0, (header, memory) => new NROM(header, memory) },
        };
        
        public static ROM Create(Stream stream)
        {
            Memory<byte> romContent = new Memory<byte>(ReadFully(stream));
            RomHeader header = new RomHeader(romContent.Slice(0, 16).ToArray());

            if (!_mapperFactories.TryGetValue(header.MapperNumber, out var factoryFunc))
                throw new NotSupportedException($"Mapper number {header.MapperNumber} is not implemented");

            return factoryFunc(header, romContent);
        }
        
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public abstract byte Read(ushort address);
        public abstract byte Peek(ushort address);
        public abstract void Write(ushort address, byte value);
        public abstract byte ReadChr(ushort address);
        public abstract byte PeekChr(ushort address);
        public abstract void WriteChr(ushort address, byte value);

        protected ROM()
        {
            // For Mocking framework, not explicit use
        }
        
        protected ROM(RomHeader header, Memory<byte> data)
        {
            Header = header;
            Content = data;
        }

        protected RomHeader Header { get; }
        protected Memory<byte> Content { get; }
        
        public abstract void Dispose();
        
        protected enum PrgBank
        {
            Low, High
        }
        
        protected readonly struct BankAddress
        {
            public BankAddress(ushort address)
            {
                Bank = address >= 0xC000 ? PrgBank.High : PrgBank.Low;
                Offset = address - (Bank == PrgBank.High ?  0xC000 : 0x8000);
            }

            public PrgBank Bank { get; }
            public int Offset { get; }
        }
    }
}