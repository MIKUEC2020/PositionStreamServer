using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommonLibrary;
using CommonLibrary.ExtensionMethod;
using DebugPrintLibrary;
using StreamServer.Data;

namespace StreamServer
{
    public static class PacketSender
    {
        public static async Task Send(User user, List<MinimumAvatarPacket> packets, UdpClient udp)
        {
            var packetCopy = packets.ToList();
            if (user.CurrentPacket != null)
            {
                var selfPosition = user.CurrentPacket.Position;

                packetCopy.HeapSort((a, b) =>
                {
                    var aSquare = Vector3.Square(a.Position, selfPosition);
                    var bSquare = Vector3.Square(b.Position, selfPosition);
                    var comp = aSquare < bSquare ? -1 : 1;
                    return comp;
                });
            }

            if (packetCopy.Count > 100)
                packetCopy = packetCopy.GetRange(0, 100);
            var buffs = Utility.PacketsToBuffers(ref packetCopy);
            foreach (var buf in buffs)
            {
                await udp.SendAsync(buf, buf.Length, user.RemoteEndPoint);
            }
        }
    }
}
