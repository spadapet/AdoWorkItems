using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Diagnostics;

namespace WorkItems.Model;

[DebuggerDisplay("State={State}")]
public sealed class AppModel : PropertyNotifier, IDisposable
{
    public void Dispose()
    {
        this.adoModel.Dispose();
    }

    private void EnsureValid()
    {
        this.adoModel.EnsureValid();
    }

    public Settings Settings { get; } = new();

    [JsonIgnore]
    public InfoBar InfoBar { get; } = new();

    [JsonIgnore]
    public ProgressBar ProgressBar { get; } = new();

    private AppState state = AppState.Loading;
    [JsonIgnore]
    public AppState State
    {
        get => this.state;
        set => this.SetProperty(ref this.state, value);
    }

    private AdoModel adoModel = new();
    public AdoModel AdoModel
    {
        get => this.adoModel;
        set => this.SetProperty(ref this.adoModel, value);
    }

    private static JsonSerializerSettings JsonSerializerSettings => new()
    {
        Formatting = Formatting.Indented,
        Converters =
        {
            new StringEnumConverter()
        }
    };

    public string Serialize()
    {
        this.EnsureValid();
        return JsonConvert.SerializeObject(this, AppModel.JsonSerializerSettings);
    }

    public static AppModel Deserialize(string json)
    {
        AppModel model = JsonConvert.DeserializeObject<AppModel>(json, AppModel.JsonSerializerSettings);
        model.EnsureValid();
        return model;
    }
}
