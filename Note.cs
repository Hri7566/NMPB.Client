// Decompiled with JetBrains decompiler
// Type: NMPB.Client.Note
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Newtonsoft.Json;

namespace NMPB.Client
{
  public class Note
  {
    [JsonProperty("n")]
    public string Value;
    [JsonProperty("v", NullValueHandling = NullValueHandling.Ignore)]
    public double Velocity;
    [JsonProperty("d", NullValueHandling = NullValueHandling.Ignore)]
    public long Delay;
    [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
    public int Stop;

    public Note()
    {
      this.Value = "a0";
      this.Velocity = 0.5;
      this.Delay = 0L;
      this.Stop = 0;
    }
  }
}
