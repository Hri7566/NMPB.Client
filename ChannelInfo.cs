// Decompiled with JetBrains decompiler
// Type: NMPB.Client.ChannelInfo
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Newtonsoft.Json;

namespace NMPB.Client
{
  public class ChannelInfo
  {
    [JsonProperty("_id")]
    public string Id;
    [JsonProperty("settings")]
    public ChannelSettings Settings;
    [JsonProperty("crown")]
    public CrownInfo Crown;
    [JsonProperty("count", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int UserCount;
  }
}
