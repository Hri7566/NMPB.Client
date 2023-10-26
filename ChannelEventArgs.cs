// Decompiled with JetBrains decompiler
// Type: NMPB.Client.ChannelEventArgs
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System;
using System.Collections.Generic;

namespace NMPB.Client
{
  public class ChannelEventArgs : EventArgs
  {
    public ChannelInfo Channel { get; private set; }

    public string ParticipantId { get; private set; }

    public List<UserBase> UpdatingUsers { get; private set; }

    public List<UserBase> UpdatedUsers { get; private set; }

    public ChannelEventArgs(
      ChannelInfo channel,
      string participantId,
      List<UserBase> updatingUsers,
      List<UserBase> updatedUsers)
    {
      this.Channel = channel;
      this.ParticipantId = participantId;
      this.UpdatingUsers = updatingUsers;
      this.UpdatedUsers = updatedUsers;
    }
  }
}
