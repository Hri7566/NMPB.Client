// Decompiled with JetBrains decompiler
// Type: NMPB.Client.DelayedTask
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NMPB.Client
{
  public class DelayedTask
  {
    private readonly int _delay;
    private readonly Action _action;
    private readonly CancellationTokenSource _token;

    public DelayedTask(Action action, int delay)
    {
      this._delay = delay;
      this._action = action;
      this._token = new CancellationTokenSource();
      new Task(new Action(this.WaitAndDoWork), this._token.Token).Start();
    }

    public void Cancel() => this._token.Cancel();

    private void WaitAndDoWork()
    {
      Thread.Sleep(this._delay);
      if (this._token.IsCancellationRequested)
        return;
      this._action();
    }
  }
}
