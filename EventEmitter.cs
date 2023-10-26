// Decompiled with JetBrains decompiler
// Type: NMPB.Client.EventEmitter
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System;
using System.Collections.Generic;

namespace NMPB.Client
{
  public class EventEmitter
  {
    private readonly Dictionary<string, Action<object[]>> _events = new Dictionary<string, Action<object[]>>();

    public void On(string evnt, Action<object[]> fn)
    {
      if (!this._events.ContainsKey(evnt))
        this._events.Add(evnt, (Action<object[]>) (o => { }));
      this._events[evnt] += fn;
    }

    public void Off(string evnt, Action<object[]> fn)
    {
      if (!this._events.ContainsKey(evnt))
        return;
      this._events[evnt] -= fn;
    }

    public void Emit(string evnt, params object[] arguments)
    {
      if (!this._events.ContainsKey(evnt))
        return;
      this._events[evnt](arguments);
    }
  }
}
