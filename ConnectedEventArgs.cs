// Decompiled with JetBrains decompiler
// Type: NMPB.Client.ConnectedEventArgs
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

namespace NMPB.Client
{
  public class ConnectedEventArgs : UserBaseEventArgs
  {
    public string Version { get; private set; }

    public string Motd { get; private set; }

    public ConnectedEventArgs(string version, string motd, UserBase user)
      : base(user)
    {
      this.Version = version;
      this.Motd = motd;
    }
  }
}
