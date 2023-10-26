// Decompiled with JetBrains decompiler
// Type: NMPB.Client.UserBase
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Newtonsoft.Json;

namespace NMPB.Client
{
  public class UserBase
  {
    [JsonProperty("_id")]
    public string Auid;
    [JsonProperty("id")]
    public string Id;
    [JsonProperty("name")]
    public string Name;
    [JsonProperty("color")]
    public string Color;
    [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    private double? _x;
    [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    private double? _y;
    public const string DefaultName = "Anonymous";
    public const string DefaultColor = "#ffff00";
    public const string DefaultId = "No";

    [JsonIgnore]
    public double X
    {
      get => this._x ?? 0.0;
      set => this._x = new double?(value);
    }

    [JsonIgnore]
    public double Y
    {
      get => this._y ?? 0.0;
      set => this._y = new double?(value);
    }

    public UserBase(string auid = "No", string id = "No", string name = "Anonymous", string color = "#ffff00")
    {
      this.Auid = auid ?? "No";
      this.Id = id ?? "No";
      this.Name = name ?? "Anonymous";
      this.Color = color ?? "#ffff00";
    }
  }
}
