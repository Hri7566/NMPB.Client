// Decompiled with JetBrains decompiler
// Type: NMPB.Client.Player
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Timers;

namespace NMPB.Client
{
  public class Player : NMPB.Client.Client
  {
    public NoteQuota NoteQuota;
    public double PosX;
    public double PosY;
    private Timer _sendMouseTimer;
    public Player.CursorDelegate Cursor = (Player.CursorDelegate) ((ref double _param1, ref double _param2) => { });
    public bool ObtainCrown;
    public QuotaLimitations QuotaLimitation = QuotaLimitations.Easy;
    private double _lastX;
    private double _lastY;

    public event EventHandler<UserBaseEventArgs> BotUserUpdated = (_param1, _param2) => { };

    public event EventHandler<UserNoteBufferEventArgs> NoteBufferReceived = (_param1, _param2) => { };

    public event EventHandler<ChatMessageEventArgs> ChatReceived = (_param1, _param2) => { };

    public bool ConnectedToRoom { get; private set; }

    public Player(Uri uri = null, string useragent = null)
      : base(uri, useragent)
    {
      this.InitClient();
    }

    public Player(string room, ChannelSettings settings, string useragent = null)
      : base(useragent: useragent)
    {
      this.InitClient();
      this.SetChannel(Uri.UnescapeDataString(room), settings ?? new ChannelSettings(false, true, true, false));
    }

    private void GetCrown(object sender, ChannelEventArgs args)
    {
      if (!this.ObtainCrown || args == null || args.Channel == null || args.Channel.Crown == null || this.ParticipantId == null || args.Channel.Crown.ParticipantId == this.ParticipantId)
        return;
      this.SendObject((object) new
      {
        m = "chown",
        id = this.ParticipantId
      });
      int delay = (int) (args.Channel.Crown.Time + 15000L - this.GetSTime());
      if (delay <= 0)
        return;
      DelayedTask delayedTask = new DelayedTask((Action) (() =>
      {
        this.SendObject((object) new
        {
          m = "chown",
          id = this.ParticipantId
        });
        this.SetChannelSettings(this.DesiredChannelSettings);
      }), delay);
    }

    private void InitClient()
    {
      this.NoteQuota = new NoteQuota();
      this.OnDynamic("a", new Action<object>(this.ReceiveChat));
      this.OnDynamic("n", new Action<object>(this.ReceiveNoteBuffer));
      this.OnDynamic("nq", (Action<object>) (msg =>
      {
        // ISSUE: reference to a compiler-generated field
        if (Player.\u003C\u003Eo__24.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          Player.\u003C\u003Eo__24.\u003C\u003Ep__0 = CallSite<Action<CallSite, NoteQuota, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetParams", (IEnumerable<Type>) null, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__24.\u003C\u003Ep__0.Target((CallSite) Player.\u003C\u003Eo__24.\u003C\u003Ep__0, this.NoteQuota, msg);
      }));
      this.OnDynamic("hi", (Action<object>) (msg => this.ConnectedToRoom = false));
      this.Disconnected += (EventHandler) ((sender, args) => this.ConnectedToRoom = false);
      this.UserUpdated += new EventHandler<UserBaseEventArgs>(this.UpdateBotUser);
      this.UserEntered += new EventHandler<UserBaseEventArgs>(this.UpdateBotUser);
      this.ChannelUpdated += new EventHandler<ChannelEventArgs>(this.GetCrown);
      this.ChannelUpdated += (EventHandler<ChannelEventArgs>) ((sender, args) => this.ConnectedToRoom = true);
      this._sendMouseTimer = new Timer() { Interval = 50.0 };
      this._sendMouseTimer.Elapsed += new ElapsedEventHandler(this.SendMouse);
      this._sendMouseTimer.Start();
    }

    private void ReceiveNoteBuffer(object msg)
    {
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (Player)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target1 = Player.\u003C\u003Eo__25.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p1 = Player.\u003C\u003Eo__25.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = Player.\u003C\u003Eo__25.\u003C\u003Ep__0.Target((CallSite) Player.\u003C\u003Eo__25.\u003C\u003Ep__0, msg);
      UserBase participantById = this.FindParticipantById(target1((CallSite) p1, obj1));
      bool flag = participantById == null;
      if (flag)
        return;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target2 = Player.\u003C\u003Eo__25.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p5 = Player.\u003C\u003Eo__25.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__4 = CallSite<Func<CallSite, bool, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.Or, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, bool, object, object> target3 = Player.\u003C\u003Eo__25.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, bool, object, object>> p4 = Player.\u003C\u003Eo__25.\u003C\u003Ep__4;
      int num1 = flag ? 1 : 0;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target4 = Player.\u003C\u003Eo__25.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p3 = Player.\u003C\u003Eo__25.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "t", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = Player.\u003C\u003Eo__25.\u003C\u003Ep__2.Target((CallSite) Player.\u003C\u003Eo__25.\u003C\u003Ep__2, msg);
      object obj3 = target4((CallSite) p3, obj2, (object) null);
      object obj4 = target3((CallSite) p4, num1 != 0, obj3);
      if (target2((CallSite) p5, obj4))
        return;
      EventHandler<UserNoteBufferEventArgs> noteBufferReceived = this.NoteBufferReceived;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__10 = CallSite<Func<CallSite, Type, UserBase, long, object, UserNoteBufferEventArgs>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, UserBase, long, object, UserNoteBufferEventArgs> target5 = Player.\u003C\u003Eo__25.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, UserBase, long, object, UserNoteBufferEventArgs>> p10 = Player.\u003C\u003Eo__25.\u003C\u003Ep__10;
      Type type = typeof (UserNoteBufferEventArgs);
      UserBase userBase = participantById;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, long>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (long), typeof (Player)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, long> target6 = Player.\u003C\u003Eo__25.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, long>> p7 = Player.\u003C\u003Eo__25.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "t", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = Player.\u003C\u003Eo__25.\u003C\u003Ep__6.Target((CallSite) Player.\u003C\u003Eo__25.\u003C\u003Ep__6, msg);
      long num2 = target6((CallSite) p7, obj5);
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", (IEnumerable<Type>) new Type[1]
        {
          typeof (List<Note>)
        }, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target7 = Player.\u003C\u003Eo__25.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p9 = Player.\u003C\u003Eo__25.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__25.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__25.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "n", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = Player.\u003C\u003Eo__25.\u003C\u003Ep__8.Target((CallSite) Player.\u003C\u003Eo__25.\u003C\u003Ep__8, msg);
      object obj7 = target7((CallSite) p9, obj6);
      UserNoteBufferEventArgs e = target5((CallSite) p10, type, userBase, num2, obj7);
      noteBufferReceived((object) this, e);
    }

    private void UpdateBotUser(object sender, UserBaseEventArgs e)
    {
      if (e.User.Auid != this.BotUser.Auid)
        return;
      this.BotUser = e.User;
      this.BotUserUpdated((object) this, new UserBaseEventArgs(this.BotUser));
    }

    private void SendMouse(object sender, EventArgs args)
    {
      this.Cursor(ref this.PosX, ref this.PosY);
      if (Math.Abs(this._lastX - this.PosX) < 0.01 && Math.Abs(this._lastY - this.PosY) < 0.01)
        return;
      this._lastX = this.PosX;
      this._lastY = this.PosY;
      this.SendObject((object) new
      {
        m = "m",
        x = this.PosX.ToString("N2", (IFormatProvider) CultureInfo.InvariantCulture),
        y = this.PosY.ToString("N2", (IFormatProvider) CultureInfo.InvariantCulture)
      });
    }

    public void SendObject(object content) => this.SendArray(new JArray((object) JObject.FromObject(content)));

    public new void Dispose()
    {
      base.Dispose();
      this._sendMouseTimer.Dispose();
    }

    public void Say(string message) => this.SendObject((object) new
    {
      m = "a",
      message = message
    });

    private bool SpendQuota()
    {
      if (this.QuotaLimitation == QuotaLimitations.Easy && !this.NoteQuota.SafeSpend(1) || this.QuotaLimitation == QuotaLimitations.Aggressive && !this.NoteQuota.Spend(1))
        return false;
      if (this.QuotaLimitation == QuotaLimitations.None)
        this.NoteQuota.SafeSpend(1);
      return true;
    }

    public void PlayNote(int code, int volume = 64, long? time = null)
    {
      if (!this.SpendQuota())
        return;
      this.StartNote(NoteConverter.Notes[Player.Clamp(code, 0, (int) sbyte.MaxValue)], (double) volume / 128.0, time);
    }

    public void StopNote(int code)
    {
      if (!this.SpendQuota())
        return;
      this.StopNote(NoteConverter.Notes[Player.Clamp(code, 0, (int) sbyte.MaxValue)]);
    }

    private void ReceiveChat(object msg)
    {
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = Player.\u003C\u003Eo__36.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p2 = Player.\u003C\u003Eo__36.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target2 = Player.\u003C\u003Eo__36.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p1 = Player.\u003C\u003Eo__36.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = Player.\u003C\u003Eo__36.\u003C\u003Ep__0.Target((CallSite) Player.\u003C\u003Eo__36.\u003C\u003Ep__0, msg);
      object obj2 = target2((CallSite) p1, obj1, (object) null);
      if (target1((CallSite) p2, obj2))
        return;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (Player)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = Player.\u003C\u003Eo__36.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p4 = Player.\u003C\u003Eo__36.\u003C\u003Ep__4;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "a", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = Player.\u003C\u003Eo__36.\u003C\u003Ep__3.Target((CallSite) Player.\u003C\u003Eo__36.\u003C\u003Ep__3, msg);
      string message = target3((CallSite) p4, obj3) ?? "";
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (Player)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target4 = Player.\u003C\u003Eo__36.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p7 = Player.\u003C\u003Eo__36.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target5 = Player.\u003C\u003Eo__36.\u003C\u003Ep__6.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p6 = Player.\u003C\u003Eo__36.\u003C\u003Ep__6;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = Player.\u003C\u003Eo__36.\u003C\u003Ep__5.Target((CallSite) Player.\u003C\u003Eo__36.\u003C\u003Ep__5, msg);
      object obj5 = target5((CallSite) p6, obj4);
      string username = target4((CallSite) p7, obj5) ?? "";
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (Player)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target6 = Player.\u003C\u003Eo__36.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p10 = Player.\u003C\u003Eo__36.\u003C\u003Ep__10;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "color", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target7 = Player.\u003C\u003Eo__36.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p9 = Player.\u003C\u003Eo__36.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = Player.\u003C\u003Eo__36.\u003C\u003Ep__8.Target((CallSite) Player.\u003C\u003Eo__36.\u003C\u003Ep__8, msg);
      object obj7 = target7((CallSite) p9, obj6);
      string color = target6((CallSite) p10, obj7) ?? "#ffffff";
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (Player)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target8 = Player.\u003C\u003Eo__36.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p13 = Player.\u003C\u003Eo__36.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "_id", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target9 = Player.\u003C\u003Eo__36.\u003C\u003Ep__12.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p12 = Player.\u003C\u003Eo__36.\u003C\u003Ep__12;
      // ISSUE: reference to a compiler-generated field
      if (Player.\u003C\u003Eo__36.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Player.\u003C\u003Eo__36.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (Player), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = Player.\u003C\u003Eo__36.\u003C\u003Ep__11.Target((CallSite) Player.\u003C\u003Eo__36.\u003C\u003Ep__11, msg);
      object obj9 = target9((CallSite) p12, obj8);
      string auid = target8((CallSite) p13, obj9) ?? "no";
      this.ChatReceived((object) this, new ChatMessageEventArgs(username, message, color, auid));
    }

    private static int Clamp(int val, int a, int b)
    {
      if (val < a)
        return a;
      return val <= b ? val : b;
    }

    public static string PrepareRoomName(string name)
    {
      name = Uri.UnescapeDataString(name);
      Match match = new Regex("multiplayerpiano.com\\/(.+)", RegexOptions.IgnoreCase).Match(name);
      if (match.Success)
        name = match.Groups[1].Value;
      return name;
    }

    public delegate void CursorDelegate(ref double x, ref double y);
  }
}
