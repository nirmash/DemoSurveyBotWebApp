﻿using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// Capability for a specific channel
    /// </summary>
    public interface IChannelCapability
    {
        /// <summary>
        /// True if the channel support buttons, false otherwise.
        /// </summary>
        bool SupportButtons { get; }
    }

    /// <summary>
    /// Channel capability detector.
    /// </summary>
    public interface IDetectChannelCapability
    {
        /// <summary>
        /// Detects channel capabilities.
        /// </summary>
        /// <returns>
        /// Capabilities of a channel.
        /// </returns>
        IChannelCapability Detect();
    }

    public sealed class DetectChannelCapability : IDetectChannelCapability
    {
        private readonly string channelId;
        private readonly string serviceUrl;


        public DetectChannelCapability(IMessageActivity message)
        {
            SetField.CheckNull(nameof(message), message);
            SetField.NotNull(out channelId, nameof(channelId), message.ChannelId);
            SetField.NotNull(out serviceUrl, nameof(serviceUrl), message.ServiceUrl);
        }

        public IChannelCapability Detect()
        {
            var isEmulator = channelId?.Equals("emulator", StringComparison.OrdinalIgnoreCase);
            var capability = new ChannelCapability(!(isEmulator.HasValue && isEmulator.Value));
            return capability;
        }
    }

    public sealed class ChannelCapability : IChannelCapability
    {
        private readonly bool supportButtons;

        public ChannelCapability(bool supportButtons = true)
        {
            this.supportButtons = supportButtons;
        }

        public bool SupportButtons
        {
            get
            {
                return supportButtons;
            }
        }
    }
}
