// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Runtime.CompilerServices;

namespace WingsEmu.Communication.RPC
{
    public class GRpcEndPoint : ICommunicationClientEndPoint
    {
        public string Ip { get; set; }
        public int Port { get; set; }
    }

    public static class GrpcExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static gRPCEndPointInformations ToGRpcEndPointInformations(this ICommunicationClientEndPoint endpoint) =>
            new gRPCEndPointInformations
            {
                Ip = endpoint.Ip,
                Port = endpoint.Port
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UUID ToUUID(this Guid id) =>
            new UUID
            {
                Id = id.ToString()
            };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegisteredWorldServer ToRegisteredWorldServer(this SerializableWorldServer server) =>
            new RegisteredWorldServer
            {
                WorldGroup = server.WorldGroup,
                Id = server.Id.ToUUID(),
                ChannelId = server.ChannelId,
                IpAddress = server.EndPointIp,
                Port = server.EndPointPort,
                SessionLimit = server.AccountLimit
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializableWorldServer ToSerializableWorldServer(this RegisteredWorldServer server) =>
            new SerializableWorldServer
            {
                WorldGroup = server.WorldGroup,
                Id = server.Id.ToGuid(),
                AccountLimit = server.SessionLimit,
                EndPointPort = (short)server.Port,
                EndPointIp = server.IpAddress,
                ChannelId = server.ChannelId
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ToGuid(this UUID id) => Guid.Parse(id.Id);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Long ToLong(this long value) => new Long { Id = value };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bool ToBool(this bool value) => new Bool { Boolean = value };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int ToInt(this int value) => new Int { Id = value };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Name ToName(this string str) => new Name { Str = str };
    }
}