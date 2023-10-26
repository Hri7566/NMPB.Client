// Decompiled with JetBrains decompiler
// Type: NMPB.Client.CrownPoint
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Newtonsoft.Json;

namespace NMPB.Client
{
  public class CrownPoint
  {
    [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public double X;
    [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public double Y;

    public CrownPoint(double x, double y)
    {
      this.X = x;
      this.Y = y;
    }
  }
}
