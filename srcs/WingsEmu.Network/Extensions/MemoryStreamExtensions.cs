﻿// WingsEmu
// 
// Developed by NosWings Team

using System.IO;
using System.Runtime.CompilerServices;

namespace WingsEmu.Network.Extensions
{
    public static class MemoryStreamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Append(this MemoryStream stream, byte value)
        {
            stream.WriteByte(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Append(this MemoryStream stream, byte[] values)
        {
            stream.Write(values, 0, values.Length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteAt(this MemoryStream stream, byte value, int at)
        {
            stream.WriteAt(new[] { value }, at);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteAt(this MemoryStream stream, byte[] values, int at)
        {
            stream.Write(values, at, values.Length);
        }
    }
}