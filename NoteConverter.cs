// Decompiled with JetBrains decompiler
// Type: NMPB.Client.NoteConverter
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

namespace NMPB.Client
{
  public static class NoteConverter
  {
    public static string[] Notes;
    private static readonly string[] Letters = new string[12]
    {
      "c",
      "cs",
      "d",
      "ds",
      "e",
      "f",
      "fs",
      "g",
      "gs",
      "a",
      "as",
      "b"
    };

    static NoteConverter()
    {
      NoteConverter.Notes = new string[128];
      for (int index1 = 0; index1 < 11; ++index1)
      {
        for (int index2 = 0; index2 < 12; ++index2)
        {
          if (12 * index1 + index2 >= 128)
            return;
          NoteConverter.Notes[12 * index1 + index2] = NoteConverter.Letters[index2] + (object) (index1 - 2);
        }
      }
    }
  }
}
