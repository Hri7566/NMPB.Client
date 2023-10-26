// Decompiled with JetBrains decompiler
// Type: NMPB.Client.ChannelSettings
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Newtonsoft.Json;
using System;

namespace NMPB.Client
{
  public class ChannelSettings
  {
    [JsonProperty("lobby")]
    public bool Lobby;
    [JsonProperty("visible")]
    public bool Visible;
    [JsonProperty("chat")]
    public bool Chat;
    [JsonProperty("crownsolo")]
    public bool Crownsolo;
    [JsonProperty("color")]
    public string Color;

    public ChannelSettings(bool lobby, bool visible, bool chat, bool crownsolo, string color = null)
    {
      this.Lobby = lobby;
      this.Visible = visible;
      this.Chat = chat;
      this.Crownsolo = crownsolo;
      this.Color = color;
    }

    public bool Equals(ChannelSettings b)
    {
      if (this.Lobby != b.Lobby || this.Visible != b.Visible || this.Chat != b.Chat || this.Crownsolo != b.Crownsolo)
        return false;
      return this.Color == null || b.Color == null || this.Color.Equals(b.Color, StringComparison.OrdinalIgnoreCase);
    }
  }
}
