// Decompiled with JetBrains decompiler
// Type: NMPB.Client.Client
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NMPB.Client.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Timers;
using WebSocket4Net;

namespace NMPB.Client
{
  public class Client : EventEmitter, IDisposable
  {
    private WebSocket _ws;
    private readonly Uri _uri;
    public ChannelInfo Channel;
    protected ChannelSettings DesiredChannelSettings;
    private string _desiredChannelId;
    public readonly List<UserBase> Users;
    public UserBase BotUser;
    private readonly Timer _noteFlushInterval;
    private readonly Timer _pingInterval;
    public string ParticipantId;
    private DateTime? _connectionTime;
    private int _connectionAttempts;
    private JArray _noteBuffer;
    private long _noteBufferTime;
    public long ServerTimeOffset;
    private bool _canConnect;
    private const int AllowedMessageLength = 8192;
    private readonly string _useragent;
    private bool _channelReceived;
    private readonly DateTime _st = new DateTime(1970, 1, 1);
    private bool _disposed;

    public event EventHandler<ConnectedEventArgs> Connected = (_param1, _param2) => { };

    public event EventHandler Disconnected = (_param1, _param2) => { };

    public event EventHandler<ErrorEventArgs> ConnectionError = (_param1, _param2) => { };

    public event EventHandler<ChannelEventArgs> ChannelUpdated = (_param1, _param2) => { };

    public event EventHandler<UserBaseEventArgs> UserEntered = (_param1, _param2) => { };

    public event EventHandler<UserBaseEventArgs> UserLeft = (_param1, _param2) => { };

    public event EventHandler<UserBaseEventArgs> UserMouseMoved = (_param1, _param2) => { };

    public event EventHandler<UserBaseEventArgs> UserNameReceived = (_param1, _param2) => { };

    public event EventHandler<UserBaseEventArgs> UserColorReceived = (_param1, _param2) => { };

    public event EventHandler<UserBaseEventArgs> UserUpdated = (_param1, _param2) => { };

    public event EventHandler<DebugMessageEventArgs> TextDebug = (_param1, _param2) => { };

    public event EventHandler<DebugMessageEventArgs> DataDebug = (_param1, _param2) => { };

    [Conditional("DEBUG")]
    private void SendDebug(string msg) => this.TextDebug((object) this, new DebugMessageEventArgs(msg));

    [Conditional("DEBUG")]
    private void LogData(string msg) => this.DataDebug((object) this, new DebugMessageEventArgs(Regex.Replace(msg, "\\s+", " ")));

    public Client(Uri uri = null, string useragent = null)
    {
      this.Users = new List<UserBase>();
      this.BotUser = new UserBase();
      Uri uri1 = uri;
      if ((object) uri1 == null)
        uri1 = new Uri(string.IsNullOrWhiteSpace(Settings.Default.ServerUrl) ? "ws://www.multiplayerpiano.com:443" : Settings.Default.ServerUrl);
      this._uri = uri1;
      this._useragent = useragent ?? "NMPB.Client";
      this.ReinitWs();
      this.BindEventListeners();
      this._pingInterval = new Timer()
      {
        Interval = 20000.0
      };
      this._pingInterval.Elapsed += (ElapsedEventHandler) ((o, eventArgs) => this.Send(string.Format("[{{\"m\": \"t\", \"e\":\"{0}\"}}]", (object) this.GetTime())));
      this._noteFlushInterval = new Timer()
      {
        Interval = 200.0
      };
      this._noteFlushInterval.Elapsed += new ElapsedEventHandler(this.OnNoteFlushIntervalElapsed);
    }

    public bool IsConnected() => !this._disposed && this._ws != null && this._ws.State == WebSocketState.Open;

    public bool IsConnecting() => this._ws != null && this._ws.State == WebSocketState.Connecting;

    public void Start()
    {
      this._canConnect = true;
      this.Connect();
    }

    public void Stop()
    {
      this._canConnect = false;
      lock (this._ws)
      {
        if (!this.IsConnected())
          return;
        this._ws.Close();
      }
    }

    private void Connect()
    {
      if (this._disposed || !this._canConnect || this.IsConnected())
        return;
      this.ReinitWs();
      lock (this._ws)
        this._ws.Open();
    }

    private void ReinitWs()
    {
      this._ws = new WebSocket(this._uri.ToString(), userAgent: this._useragent, origin: this._uri.Host)
      {
        ReceiveBufferSize = 8192
      };
      this._ws.Closed += new EventHandler(this.OnWsClosed);
      this._ws.Opened += new EventHandler(this.OnWsOpened);
      this._ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(this.OnWsMessageReceived);
      this._ws.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(this.OnWsError);
      this._ws.EnableAutoSendPing = false;
      this._ws.NoDelay = true;
    }

    private void OnWsError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) => this.ConnectionError((object) this, new ErrorEventArgs(e.Exception));

    private void OnWsMessageReceived(object sender, MessageReceivedEventArgs args)
    {
      object obj1 = (object) JArray.Parse(args.Message);
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (NMPB.Client.Client)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      foreach (object obj2 in NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__9.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__9, obj1))
      {
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target1 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p5 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__5;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__0, obj2, (object) null);
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        object obj4;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (!NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__4.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__4, obj3))
        {
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target2 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__3.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p3 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__3;
          object obj5 = obj3;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target3 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__2.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p2 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__2;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "m", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj6 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__1.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__1, obj2);
          object obj7 = target3((CallSite) p2, obj6, (object) null);
          obj4 = target2((CallSite) p3, obj5, obj7);
        }
        else
          obj4 = obj3;
        if (target1((CallSite) p5, obj4))
        {
          lock (this.Users)
          {
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__8 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__8 = CallSite<Action<CallSite, NMPB.Client.Client, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "Emit", (IEnumerable<Type>) null, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Action<CallSite, NMPB.Client.Client, string, object> target4 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__8.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Action<CallSite, NMPB.Client.Client, string, object>> p8 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__8;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__7 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, string> target5 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__7.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, string>> p7 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__7;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__6 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "m", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj8 = NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__6.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__64.\u003C\u003Ep__6, obj2);
            string str = target5((CallSite) p7, obj8);
            object obj9 = obj2;
            target4((CallSite) p8, this, str, obj9);
          }
        }
      }
    }

    private void OnWsClosed(object sender, EventArgs e)
    {
      lock (this._ws)
      {
        if (this._disposed)
          return;
        this._pingInterval.Stop();
        this._noteFlushInterval.Stop();
        this.Disconnected((object) this, new EventArgs());
        if (!this._canConnect)
          return;
        if (this._connectionTime.HasValue)
        {
          this._connectionTime = new DateTime?();
          this._connectionAttempts = 0;
        }
        else
          ++this._connectionAttempts;
        int[] numArray = new int[4]{ 50, 2950, 7000, 10000 };
        int index = this._connectionAttempts;
        if (index >= numArray.Length)
          index = numArray.Length - 1;
        DelayedTask delayedTask = new DelayedTask(new Action(this.Connect), numArray[index]);
      }
    }

    private void OnWsOpened(object sender, EventArgs e)
    {
      this._connectionTime = new DateTime?(DateTime.Now);
      this.Send("[{\"m\": \"hi\"}]");
      this._pingInterval.Start();
      this._noteBuffer = new JArray();
      this._noteBufferTime = 0L;
      this._noteFlushInterval.Start();
    }

    private void OnNoteFlushIntervalElapsed(object o, ElapsedEventArgs eventArgs)
    {
      if (this._disposed)
        return;
      lock (this._noteBuffer)
      {
        if (this._noteBufferTime <= 0L || this._noteBuffer.Count <= 0)
          return;
        this.SplitAndSendNotes(this._noteBuffer);
        this._noteBufferTime = 0L;
        this._noteBuffer.Clear();
      }
    }

    private void SplitAndSendNotes(JArray array)
    {
      if (array == null)
        return;
      string raw = JsonConvert.SerializeObject((object) new JArray((object) JObject.FromObject((object) new
      {
        m = "n",
        t = (this._noteBufferTime + this.ServerTimeOffset),
        n = array
      })));
      if (raw.Length < 8192)
      {
        this.Send(raw);
      }
      else
      {
        int count = array.Count;
        this.SplitAndSendNotes(JArray.FromObject((object) array.Take<JToken>(count / 2)));
        this.SplitAndSendNotes(JArray.FromObject((object) array.Skip<JToken>(count / 2)));
      }
    }

    private void Send(string raw)
    {
      lock (this._ws)
      {
        if (!this.IsConnected())
          return;
        this._ws.Send(raw);
      }
    }

    private void SetChannelSure()
    {
      this.SetChannel();
      DelayedTask delayedTask = new DelayedTask((Action) (() =>
      {
        if (this._channelReceived)
          return;
        this.SetChannelSure();
      }), 2000);
    }

    private void BindEventListeners()
    {
      this.OnDynamic("hi", (Action<object>) (msg =>
      {
        this._channelReceived = false;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, UserBase>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (UserBase), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, UserBase> target1 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__2.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, UserBase>> p2 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__2;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", (IEnumerable<Type>) new Type[1]
          {
            typeof (UserBase)
          }, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target2 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p1 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "u", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj1 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__0, msg);
        object obj2 = target2((CallSite) p1, obj1);
        this.BotUser = target1((CallSite) p2, obj2);
        EventHandler<ConnectedEventArgs> connected = this.Connected;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target3 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__4.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p4 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__4;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "v", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__3.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__3, msg);
        string version = target3((CallSite) p4, obj3) ?? "";
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target4 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__6.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p6 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__6;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "motd", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj4 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__5.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__5, msg);
        string motd = target4((CallSite) p6, obj4) ?? "";
        ConnectedEventArgs e = new ConnectedEventArgs(version, motd, this.BotUser);
        connected((object) this, e);
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target5 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__9.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p9 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__9;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target6 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__8.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p8 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__8;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "t", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__7.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__7, msg);
        object obj6 = target6((CallSite) p8, obj5, (object) null);
        if (target5((CallSite) p9, obj6))
        {
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__11 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, long>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (long), typeof (NMPB.Client.Client)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, long> target7 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__11.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, long>> p11 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__11;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__10 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "t", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj7 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__10.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__10, msg);
          this.ReceiveServerTime(target7((CallSite) p11, obj7));
        }
        if (this._desiredChannelId == null)
          return;
        this.SetChannelSure();
      }));
      this.OnDynamic("t", (Action<object>) (msg =>
      {
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target8 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__14.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p14 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__14;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__13 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target9 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__13.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p13 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__13;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "t", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj8 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__12.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__12, msg);
        object obj9 = target9((CallSite) p13, obj8, (object) null);
        if (!target8((CallSite) p14, obj9))
          return;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__16 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, long>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (long), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, long> target10 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__16.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, long>> p16 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__16;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "t", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj10 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__15.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__15, msg);
        this.ReceiveServerTime(target10((CallSite) p16, obj10));
      }));
      this.OnDynamic("ch", (Action<object>) (msg =>
      {
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__18 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target11 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__18.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p18 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__18;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ch", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj11 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__17.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__17, msg);
        object obj12 = target11((CallSite) p18, obj11, (object) null);
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__22 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__22.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__22, obj12))
          return;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__21 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target12 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__21.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p21 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__21;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__20 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.Or, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool, object> target13 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__20.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool, object>> p20 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__20;
        object obj13 = obj12;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__19 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ch", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        int num = !(NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__19.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__19, msg) is JObject) ? 1 : 0;
        object obj14 = target13((CallSite) p20, obj13, num != 0);
        if (target12((CallSite) p21, obj14))
          return;
        this._channelReceived = true;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__25 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target14 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__25.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p25 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__25;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__24 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "_id", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target15 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__24.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p24 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__24;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__23 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ch", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj15 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__23.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__23, msg);
        object obj16 = target15((CallSite) p24, obj15);
        this._desiredChannelId = target14((CallSite) p25, obj16);
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__28 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, ChannelInfo>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ChannelInfo), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, ChannelInfo> target16 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__28.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, ChannelInfo>> p28 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__28;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__27 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", (IEnumerable<Type>) new Type[1]
          {
            typeof (ChannelInfo)
          }, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target17 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__27.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p27 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__27;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__26 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ch", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj17 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__26.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__26, msg);
        object obj18 = target17((CallSite) p27, obj17);
        this.Channel = target16((CallSite) p28, obj18);
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__31 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target18 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__31.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p31 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__31;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__30 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target19 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__30.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p30 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__30;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__29 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj19 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__29.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__29, msg);
        object obj20 = target19((CallSite) p30, obj19, (object) null);
        if (target18((CallSite) p31, obj20))
        {
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__33 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, string> target20 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__33.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, string>> p33 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__33;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__32 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj21 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__32.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__32, msg);
          this.ParticipantId = target20((CallSite) p33, obj21);
        }
        if (this.DesiredChannelSettings != null && this.IsOwner() && !this.DesiredChannelSettings.Equals(this.Channel.Settings))
        {
          this._channelReceived = false;
          this.SetChannelSure();
        }
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__35 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", (IEnumerable<Type>) new Type[1]
          {
            typeof (List<UserBase>)
          }, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target21 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__35.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p35 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__35;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__34 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ppl", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj22 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__34.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__34, msg);
        object obj23 = target21((CallSite) p35, obj22);
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__37 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__37 = CallSite<Action<CallSite, NMPB.Client.Client, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "SetParticipants", (IEnumerable<Type>) null, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, NMPB.Client.Client, object> target22 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__37.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, NMPB.Client.Client, object>> p37 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__37;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__36 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ppl", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj24 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__36.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__36, msg);
        target22((CallSite) p37, this, obj24);
        EventHandler<ChannelEventArgs> channelUpdated = this.ChannelUpdated;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__38 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__38 = CallSite<Func<CallSite, Type, ChannelInfo, string, object, List<UserBase>, ChannelEventArgs>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ChannelEventArgs e = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__38.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__38, typeof (ChannelEventArgs), this.Channel, this.ParticipantId, obj23, this.Users);
        channelUpdated((object) this, e);
      }));
      this.OnDynamic("p", (Action<object>) (msg =>
      {
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__39 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__39 = CallSite<Action<CallSite, NMPB.Client.Client, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "ParticipantUpdate", (IEnumerable<Type>) null, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__39.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__39, this, msg);
      }));
      this.OnDynamic("m", (Action<object>) (msg =>
      {
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__41 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__41.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p41 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__41;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__40 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__40 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__40.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__40, msg);
        string id = target((CallSite) p41, obj);
        if (!this.Users.Any<UserBase>((Func<UserBase, bool>) (user => user.Id == id)))
          return;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__42 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__42 = CallSite<Action<CallSite, NMPB.Client.Client, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "ParticipantUpdate", (IEnumerable<Type>) null, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__42.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__42, this, msg);
      }));
      this.OnDynamic("bye", (Action<object>) (msg =>
      {
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__44 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__44 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__44.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p44 = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__44;
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__43 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__43 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "p", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__43.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__72.\u003C\u003Ep__43, msg);
        this.RemoveParticipant(target((CallSite) p44, obj));
      }));
    }

    private object OfflineChannelSettings() => (object) JObject.FromObject((object) new
    {
      lobby = true,
      visible = false,
      chat = false,
      crownsolo = false
    });

    private ChannelSettings GetChannelSetting()
    {
      if (this.Channel != null && this.Channel.Settings != null)
        return this.Channel.Settings;
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__74.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__74.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ChannelSettings>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ChannelSettings), typeof (NMPB.Client.Client)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return NMPB.Client.Client.\u003C\u003Eo__74.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__74.\u003C\u003Ep__0, this.OfflineChannelSettings());
    }

    public UserBase OfflineParticipant() => new UserBase("", "", "", "#777");

    public UserBase GetOwnParticipant() => this.FindParticipantById(this.ParticipantId);

    private void RemoveParticipant(string id)
    {
      UserBase user1;
      lock (this.Users)
      {
        user1 = this.Users.FirstOrDefault<UserBase>((Func<UserBase, bool>) (user => user.Id == id));
        if (user1 == null)
          return;
        this.Users.Remove(user1);
      }
      this.UserLeft((object) this, new UserBaseEventArgs(user1));
    }

    public UserBase FindParticipantById(string id) => this.Users.FirstOrDefault<UserBase>((Func<UserBase, bool>) (user => user.Id == id));

    public bool IsOwner() => this.Channel != null && this.Channel.Crown != null && this.Channel.Crown.ParticipantId == this.ParticipantId;

    public bool PreventsPlaying() => this.IsConnected() && !this.IsOwner() && this.GetChannelSetting().Crownsolo;

    private void ParticipantUpdate(object update)
    {
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p2 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target2 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p1 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__0, update);
      object obj2 = target2((CallSite) p1, obj1, (object) null);
      if (target1((CallSite) p2, obj2))
        return;
      lock (this.Users)
      {
        UserBase user1 = this.Users.FirstOrDefault<UserBase>((Func<UserBase, bool>) (user =>
        {
          string id = user.Id;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, string> target3 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__4.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, string>> p4 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__4;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj3 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__3.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__3, update);
          string str = target3((CallSite) p4, obj3);
          return id == str;
        }));
        if (user1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, UserBase>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (UserBase), typeof (NMPB.Client.Client)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, UserBase> target4 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__6.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, UserBase>> p6 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__6;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__5 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", (IEnumerable<Type>) new Type[1]
            {
              typeof (UserBase)
            }, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj4 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__5.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__5, update);
          UserBase user2 = target4((CallSite) p6, obj4);
          if (user2 == null)
            return;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__9 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target5 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__9.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p9 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__9;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__8 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target6 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__8.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p8 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__8;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__7 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "x", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj5 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__7.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__7, update);
          object obj6 = target6((CallSite) p8, obj5, (object) null);
          if (target5((CallSite) p9, obj6))
          {
            UserBase userBase = user2;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__11 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, double>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (double), typeof (NMPB.Client.Client)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, double> target7 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__11.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, double>> p11 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__11;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__10 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "x", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj7 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__10.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__10, update);
            double num = target7((CallSite) p11, obj7);
            userBase.X = num;
          }
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__14 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target8 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__14.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p14 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__14;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__13 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target9 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__13.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p13 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__13;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__12 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "y", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj8 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__12.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__12, update);
          object obj9 = target9((CallSite) p13, obj8, (object) null);
          if (target8((CallSite) p14, obj9))
          {
            UserBase userBase = user2;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__16 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, double>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (double), typeof (NMPB.Client.Client)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, double> target10 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__16.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, double>> p16 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__16;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__15 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "y", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj10 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__15.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__15, update);
            double num = target10((CallSite) p16, obj10);
            userBase.Y = num;
          }
          this.Users.Add(user2);
          this.UserEntered((object) this, new UserBaseEventArgs(user2));
          user1 = user2;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__19 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target11 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__19.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p19 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__19;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__18 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target12 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__18.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p18 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__18;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__17 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "x", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj11 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__17.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__17, update);
          object obj12 = target12((CallSite) p18, obj11, (object) null);
          if (target11((CallSite) p19, obj12))
          {
            UserBase userBase = user1;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__21 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, double>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (double), typeof (NMPB.Client.Client)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, double> target13 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__21.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, double>> p21 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__21;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__20 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "x", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj13 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__20.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__20, update);
            double num = target13((CallSite) p21, obj13);
            userBase.X = num;
          }
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__24 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target14 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__24.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p24 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__24;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__23 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target15 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__23.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p23 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__23;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__22 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "y", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj14 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__22.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__22, update);
          object obj15 = target15((CallSite) p23, obj14, (object) null);
          if (target14((CallSite) p24, obj15))
          {
            UserBase userBase = user1;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__26 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, double>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (double), typeof (NMPB.Client.Client)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, double> target16 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__26.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, double>> p26 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__26;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__25 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "y", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj16 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__25.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__25, update);
            double num = target16((CallSite) p26, obj16);
            userBase.Y = num;
          }
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__28 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target17 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__28.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p28 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__28;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__27 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "x", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj17 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__27.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__27, update);
          object obj18 = target17((CallSite) p28, obj17, (object) null);
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__33 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          if (!NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__33.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__33, obj18))
          {
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__32 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, bool> target18 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__32.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, bool>> p32 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__32;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__31 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.Or, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, object, object> target19 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__31.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, object, object>> p31 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__31;
            object obj19 = obj18;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__30 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, object, object> target20 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__30.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, object, object>> p30 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__30;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__29 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "y", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj20 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__29.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__29, update);
            object obj21 = target20((CallSite) p30, obj20, (object) null);
            object obj22 = target19((CallSite) p31, obj19, obj21);
            if (!target18((CallSite) p32, obj22))
              goto label_81;
          }
          this.UserMouseMoved((object) this, new UserBaseEventArgs(user1));
label_81:
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__36 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target21 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__36.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p36 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__36;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__35 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target22 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__35.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p35 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__35;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__34 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj23 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__34.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__34, update);
          object obj24 = target22((CallSite) p35, obj23, (object) null);
          if (target21((CallSite) p36, obj24))
          {
            UserBase userBase = user1;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__38 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__38 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, string> target23 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__38.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, string>> p38 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__38;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__37 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__37 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj25 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__37.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__37, update);
            string str = target23((CallSite) p38, obj25);
            userBase.Name = str;
            this.UserNameReceived((object) this, new UserBaseEventArgs(user1));
          }
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__41 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target24 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__41.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p41 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__41;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__40 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__40 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object, object> target25 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__40.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object, object>> p40 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__40;
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__39 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__39 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "color", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj26 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__39.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__39, update);
          object obj27 = target25((CallSite) p40, obj26, (object) null);
          if (target24((CallSite) p41, obj27))
          {
            UserBase userBase = user1;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__43 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__43 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (NMPB.Client.Client)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, string> target26 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__43.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, string>> p43 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__43;
            // ISSUE: reference to a compiler-generated field
            if (NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__42 == null)
            {
              // ISSUE: reference to a compiler-generated field
              NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__42 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "color", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj28 = NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__42.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__81.\u003C\u003Ep__42, update);
            string str = target26((CallSite) p43, obj28);
            userBase.Color = str;
            this.UserColorReceived((object) this, new UserBaseEventArgs(user1));
          }
        }
        this.UserUpdated((object) this, new UserBaseEventArgs(user1));
      }
    }

    private void SetParticipants(IEnumerable<JToken> newParticipants)
    {
      if (newParticipants == null)
        return;
      lock (this.Users)
      {
        List<UserBase> list = this.Users.Where<UserBase>((Func<UserBase, bool>) (user1 => newParticipants.All<JToken>((Func<JToken, bool>) (user2 => user1.Id != (string) user2[(object) "id"])))).ToList<UserBase>();
        for (int index = 0; index < list.Count; ++index)
          this.RemoveParticipant(list[index].Id);
        foreach (object newParticipant in newParticipants)
          this.ParticipantUpdate(newParticipant);
      }
    }

    private int CountParticipants() => this.Users.Count;

    public void SetChannel(string id = null, ChannelSettings set = null)
    {
      this._desiredChannelId = id ?? this._desiredChannelId;
      this.DesiredChannelSettings = set ?? this.DesiredChannelSettings;
      object obj1 = (object) JObject.FromObject((object) new
      {
        m = "ch",
        _id = this._desiredChannelId
      });
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, JObject, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, nameof (set), typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__0, obj1, JObject.FromObject((object) this.DesiredChannelSettings ?? new object()));
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__1 = CallSite<Func<CallSite, Type, object, JArray>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.SendArray(NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__1.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__84.\u003C\u003Ep__1, typeof (JArray), obj1));
    }

    public void SetChannelSettings(ChannelSettings set)
    {
      this.DesiredChannelSettings = set ?? this.DesiredChannelSettings;
      this.SendArray(new JArray((object) JObject.FromObject((object) new
      {
        m = "chset",
        set = set
      })));
    }

    public void SendArray(JArray jArray) => this.Send(JsonConvert.SerializeObject((object) jArray));

    private void ReceiveServerTime(long t) => this.ServerTimeOffset = t - this.GetTime();

    public long GetTime() => (long) ((DateTime.Now.ToUniversalTime() - this._st).TotalMilliseconds + 0.5);

    public long GetSTime() => this.GetTime() + this.ServerTimeOffset;

    public void OnDynamic(string evnt, Action<object> callback) => this.On(evnt, (Action<object[]>) (objects =>
    {
      // ISSUE: reference to a compiler-generated field
      if (NMPB.Client.Client.\u003C\u003Eo__91.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__91.\u003C\u003Ep__0 = CallSite<Action<CallSite, Action<object>, object>>.Create(Binder.Invoke(CSharpBinderFlags.ResultDiscarded, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      NMPB.Client.Client.\u003C\u003Eo__91.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__91.\u003C\u003Ep__0, callback, objects[0]);
    }));

    public void StartNote(string note, double vel = 0.5, long? startTime = null)
    {
      if (!this.IsConnected())
        return;
      long num = startTime ?? this.GetTime();
      string str = vel.ToString("N3", (IFormatProvider) CultureInfo.InvariantCulture);
      object obj1 = (object) JObject.FromObject((object) new
      {
        n = note,
        v = str
      });
      lock (this._noteBuffer)
      {
        if (this._noteBufferTime <= 0L)
        {
          this._noteBufferTime = num;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, long, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "d", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__0, obj1, num - this._noteBufferTime);
        }
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__1 = CallSite<Action<CallSite, JArray, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__1.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__92.\u003C\u003Ep__1, this._noteBuffer, obj1);
      }
    }

    public void StopNote(string note, long? time = null)
    {
      if (!this.IsConnected())
        return;
      time = new long?(time ?? this.GetTime());
      object obj1 = (object) JObject.FromObject((object) new
      {
        n = note,
        s = 1
      });
      lock (this._noteBuffer)
      {
        if (this._noteBufferTime <= 0L)
        {
          this._noteBufferTime = time.Value;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, long, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "d", typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__0.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__0, obj1, time.Value - this._noteBufferTime);
        }
        // ISSUE: reference to a compiler-generated field
        if (NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__1 = CallSite<Action<CallSite, JArray, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (NMPB.Client.Client), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__1.Target((CallSite) NMPB.Client.Client.\u003C\u003Eo__93.\u003C\u003Ep__1, this._noteBuffer, obj1);
      }
    }

    public void Dispose()
    {
      this._disposed = true;
      this.Stop();
      this._pingInterval.Dispose();
      this._noteFlushInterval.Dispose();
    }
  }
}
