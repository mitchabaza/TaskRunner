namespace Dance
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T arg);
        T Deserialize<T>(string arg);
    }
}