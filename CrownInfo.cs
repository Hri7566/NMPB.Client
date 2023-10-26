﻿// Decompiled with JetBrains decompiler
// Type: NMPB.Client.CrownInfo
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Newtonsoft.Json;

namespace NMPB.Client
{
  public class CrownInfo
  {
    [JsonProperty("participantId")]
    public string ParticipantId;
    [JsonProperty("userId")]
    public string UserId;
    [JsonProperty("time", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public long Time;
    [JsonProperty("startPos")]
    public CrownPoint StartPos;
    [JsonProperty("endPos")]
    public CrownPoint EndPos;

    public CrownInfo(
      string participantId,
      string userId,
      long time,
      CrownPoint startPos,
      CrownPoint endPos)
    {
      this.ParticipantId = participantId;
      this.UserId = userId;
      this.Time = time;
      this.StartPos = startPos;
      this.EndPos = endPos;
    }
  }
}
