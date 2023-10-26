// Decompiled with JetBrains decompiler
// Type: NMPB.Client.Properties.Settings
// Assembly: NMPB.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CEFD25D1-888C-4863-942A-D7630CEEE864
// Assembly location: D:\Downloads\NMPB v1.2 bin\NMPB.Client.dll

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NMPB.Client.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
  public sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default => Settings.defaultInstance;

    [ApplicationScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("ws://www.multiplayerpiano.com:443")]
    public string ServerUrl => (string) this[nameof (ServerUrl)];
  }
}
