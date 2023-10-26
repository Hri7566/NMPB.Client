// Decompiled with JetBrains decompiler
// Type: NMPB.Client.DebugMessageEventArgs
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System;

namespace NMPB.Client
{
  public class DebugMessageEventArgs : EventArgs
  {
    public string Message { get; private set; }

    public DebugMessageEventArgs(string message) => this.Message = message;
  }
}
