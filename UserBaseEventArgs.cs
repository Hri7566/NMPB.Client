// Decompiled with JetBrains decompiler
// Type: NMPB.Client.UserBaseEventArgs
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System;

namespace NMPB.Client
{
  public class UserBaseEventArgs : EventArgs
  {
    public UserBase User { get; private set; }

    public UserBaseEventArgs(UserBase user) => this.User = user;
  }
}
