﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.JsonRpc.Messages;
using Griffin.Networking.Messages;

namespace Griffin.Networking.JsonRpc.Handlers
{
    class HeaderDecoder : IUpstreamHandler
    {
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as Received;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            // byte + int
            if (msg.BufferSlice.RemainingLength < 5)
            {
                return;
            }

            var header = new SimpleHeader
                             {
                                 Version = msg.BufferSlice.Buffer[msg.BufferSlice.Position++],
                                 Length = BitConverter.ToInt32(msg.BufferSlice.Buffer, msg.BufferSlice.Position)
                             };
            msg.BufferSlice.Position += 4;
            context.SendUpstream(new ReceivedHeader(header));

            if (msg.BufferSlice.RemainingLength > 0)
                context.SendUpstream(msg);
        }
    }
}
