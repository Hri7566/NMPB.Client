// Decompiled with JetBrains decompiler
// Type: NMPB.Client.NoteQuota
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;

namespace NMPB.Client
{
  public class NoteQuota : IDisposable
  {
    public int Allowance = 400;
    public int Max = 1200;
    public int HistLen = 3;
    private object _paramsNormal = (object) new
    {
      allowance = 400,
      max = 1200
    };
    private readonly Timer _timer;
    private readonly LinkedList<int> _history;

    public int Points { get; private set; }

    public NoteQuota(bool timer = true)
    {
      this.Points = this.Max;
      this._history = new LinkedList<int>((IEnumerable<int>) new int[1]
      {
        this.Max
      });
      if (!timer)
        return;
      this._timer = new Timer(2000.0);
      this._timer.Elapsed += new ElapsedEventHandler(this.Tick);
      this._timer.Start();
    }

    public bool SetParams(object param)
    {
      lock (this)
      {
        param = param ?? this._paramsNormal;
        // ISSUE: reference to a compiler-generated field
        if (NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (int), typeof (NoteQuota)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, int> target1 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, int>> p1 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "allowance", typeof (NoteQuota), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj1 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__0, param) ?? (object) this.Allowance;
        int num1 = target1((CallSite) p1, obj1);
        // ISSUE: reference to a compiler-generated field
        if (NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (int), typeof (NoteQuota)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, int> target2 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, int>> p3 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__3;
        // ISSUE: reference to a compiler-generated field
        if (NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "max", typeof (NoteQuota), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__2.Target((CallSite) NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__2, param) ?? (object) this.Max;
        int num2 = target2((CallSite) p3, obj2);
        // ISSUE: reference to a compiler-generated field
        if (NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (int), typeof (NoteQuota)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, int> target3 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, int>> p5 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__5;
        // ISSUE: reference to a compiler-generated field
        if (NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "histLen", typeof (NoteQuota), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__4.Target((CallSite) NoteQuota.\u003C\u003Eo__11.\u003C\u003Ep__4, param) ?? (object) this.HistLen;
        int num3 = target3((CallSite) p5, obj3);
        if (num1 == this.Allowance && num2 == this.Max && num3 == this.HistLen)
          return false;
        this.Allowance = num1;
        this.Max = num2;
        this.HistLen = num3;
        this.ResetPoints();
        return true;
      }
    }

    private void ResetPoints()
    {
      lock (this)
      {
        this.Points = this.Max;
        this._history.Clear();
        for (int index = 0; index < this.HistLen; ++index)
          this._history.AddFirst(this.Points);
      }
    }

    public void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
    {
      lock (this)
      {
        this._history.AddFirst(this.Points);
        while (this._history.Count > this.HistLen)
          this._history.RemoveLast();
        if (this.Points >= this.Max)
          return;
        this.Points += this.Allowance;
        if (this.Points <= this.Max)
          return;
        this.Points = this.Max;
      }
    }

    public bool CanSpend(int needed)
    {
      lock (this)
      {
        if (this._history.Sum() <= 0)
          needed *= this.Allowance;
        return this.Points >= needed;
      }
    }

    public bool Spend(int needed)
    {
      lock (this)
      {
        if (this._history.Sum() <= 0)
          needed *= this.Allowance;
        if (this.Points < needed)
          return false;
        this.Points -= needed;
        return true;
      }
    }

    public bool CanSafeSpend(int needed)
    {
      lock (this)
        return this.Points >= needed;
    }

    public bool SafeSpend(int needed)
    {
      lock (this)
      {
        if (this.Points < needed)
          return false;
        this.Points -= needed;
        return true;
      }
    }

    public void Dispose()
    {
      if (this._timer == null)
        return;
      this._timer.Stop();
      this._timer.Dispose();
    }
  }
}
