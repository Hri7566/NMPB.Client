// Decompiled with JetBrains decompiler
// Type: NMPB.Client.ChatMessageEventArgs
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System;

namespace NMPB.Client
{
  public class ChatMessageEventArgs : EventArgs
  {
    public string Message { get; private set; }

    public string Username { get; private set; }

    public string Color { get; private set; }

    public string Auid { get; private set; }

    public ChatMessageEventArgs(string username, string message, string color, string auid)
    {
      this.Message = message;
      this.Username = username;
      this.Color = color;
      this.Auid = auid;
    }
  }
}
