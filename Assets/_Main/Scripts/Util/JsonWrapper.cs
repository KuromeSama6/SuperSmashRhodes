using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SuperSmashRhodes.Util {
public class JsonWrapper {
    public JObject json { get; private set; }
    private readonly JsonSerializer _serializer;

    public JsonWrapper(JToken ele) {
        if (ele.Type != JTokenType.Object)
            throw new ArgumentException("Argument must be a JObject!");

        json = (JObject)ele;

        var settings = new JsonSerializerSettings();
        _serializer = JsonSerializer.Create(settings);
    }

    public JsonWrapper() : this(new JObject()) { }
    public JsonWrapper(string str) : this(JObject.Parse(str)) { }

    public string GetString(string path, string def = null) {
        JToken ret = ResolvePath(path);
        return ret != null && ret.Type == JTokenType.String ? ret.Value<string>() : def;
    }

    public T GetEnum<T>(string path, T def = default) where T : struct {
        string str = GetString(path, def.ToString());
        if (str == null) return def;

        if (Enum.TryParse<T>(str, out T result))
            return result;

        return def;
    }

    public int GetInt(string path, int def = 0) {
        JToken ret = ResolvePath(path);
        return ret != null && ret.Type == JTokenType.Integer ? ret.Value<int>() : def;
    }

    public long GetLong(string path, long def = 0) {
        JToken ret = ResolvePath(path);
        return ret != null && ret.Type == JTokenType.Integer ? ret.Value<long>() : def;
    }

    public double GetDouble(string path, double def = 0.0) {
        JToken ret = ResolvePath(path);
        return ret != null && ret.Type == JTokenType.Float ? ret.Value<double>() : def;
    }

    public bool GetBool(string path, bool def = false) {
        JToken ret = ResolvePath(path);
        return ret != null && ret.Type == JTokenType.Boolean ? ret.Value<bool>() : def;
    }

    public Guid? GetUuid(string path) {
        string ret = GetString(path);
        return ret == null ? (Guid?)null : Guid.Parse(ret);
    }

    public List<T> GetList<T>(string path) {
        List<T> ret = new List<T>();
        JToken ele = ResolvePath(path);
        if (ele == null || ele.Type != JTokenType.Array) return ret;

        foreach (var jsonElement in ele) {
            try {
                ret.Add(jsonElement.ToObject<T>(_serializer));
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        return ret;
    }

    public JArray GetList(string path) {
        JToken ele = ResolvePath(path);
        return ele != null && ele.Type == JTokenType.Array ? (JArray)ele : new JArray();
    }

    public T GetObject<T>(string path) {
        JToken ret = ResolvePath(path);
        if (ret == null || ret.Type == JTokenType.Null) return default(T);

        return ret.ToObject<T>(_serializer);
    }

    public T ToObject<T>() {
        return GetObject<T>(string.Empty);
    }

    public JsonWrapper GetObject(string path) {
        JToken ret = ResolvePath(path);
        if (ret == null || ret.Type != JTokenType.Object) return new JsonWrapper();
        return new JsonWrapper((JObject)ret);
    }

    public JToken Get(string path) {
        return ResolvePath(path);
    }

    public List<string> GetKeys() {
        return json.Properties().Select(p => p.Name).ToList();
    }

    public bool Has(string path) {
        return ResolvePath(path) != null;
    }

    public Stream ToStream() {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        writer.Write(ToString());
        writer.Flush();
        memoryStream.Position = 0;
        return memoryStream;
    }

    public JsonWrapper Set(string path, object obj) {
        JObject parent = json;

        var args = path.Split('.');
        if (args.Length == 0)
            throw new ArgumentException("Setting parent node not permitted.");

        string property = args[^1];

        for (int i = 0; i < args.Length - 1; i++) {
            string name = args[i];
            if (!parent.ContainsKey(name) || parent[name].Type != JTokenType.Object)
                parent[name] = new JObject();

            parent = (JObject)parent[name];
        }

        Set(property, parent, obj);
        return this;
    }

    public JsonWrapper SetObject(string path, object obj) {
        return Set(path, JToken.FromObject(obj, _serializer));
    }

    public void Save(FileInfo file) {
        file.Directory.Create();
        using var writer = new StreamWriter(file.FullName);
        var jsonWriter = new JsonTextWriter(writer) {
            Formatting = Formatting.Indented
        };
        _serializer.Serialize(jsonWriter, json);
    }

    private void Set(string name, JObject parent, object obj) {
        if (obj == null) {
            parent.Remove(name);
        } else if (obj is JToken token) {
            parent[name] = token;
        } else if (obj is IDictionary<string, object> dict) {
            var data = new JObject();
            foreach (var entry in dict)
                Set(entry.Key, data, entry.Value);

            parent[name] = data;
        } else if (obj is IEnumerable<object> list) {
            var array = new JArray();
            foreach (var item in list)
                array.Add(JToken.FromObject(item, _serializer));

            parent[name] = array;
        } else {
            parent[name] = JToken.FromObject(obj, _serializer);
        }
    }

    private JToken ResolvePath(string path) {
        var args = path.Split('.');
        if (path == string.Empty || args.Length == 0) return json;

        JObject ret = json;
        foreach (var name in args) {
            if (ret.ContainsKey(name)) {
                var ele = ret[name];
                if (ele.Type == JTokenType.Object)
                    ret = (JObject)ele;
                else
                    return ele;
            } else
                return null;
        }

        return ret;
    }

    public override string ToString() {
        return json.ToString();
    }
}

}
