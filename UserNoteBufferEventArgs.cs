// Decompiled with JetBrains decompiler
// Type: NMPB.Client.UserNoteBufferEventArgs
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System.Collections.Generic;

namespace NMPB.Client
{
  public class UserNoteBufferEventArgs : UserBaseEventArgs
  {
    public long Time { get; private set; }

    public List<Note> Notes { get; private set; }

    public UserNoteBufferEventArgs(UserBase user, long time, List<Note> notes)
      : base(user)
    {
      this.Notes = notes;
      this.Time = time;
    }
  }
}
