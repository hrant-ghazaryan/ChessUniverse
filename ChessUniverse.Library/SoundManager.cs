using NAudio.Wave;
namespace ChessUniverse.Library;


public static class SoundManager
{
    private static readonly Dictionary<string, string> cache = new();

    public static void Load(string key, string path)
        => cache.Add(key, path);

    public static void Play(string key)
    {
        if (!cache.ContainsKey(key)) return;

        Task.Run(() =>
        {
            var reader = new AudioFileReader(cache[key]);

            var waveOut = new WaveOutEvent();

            waveOut.Init(reader);

            waveOut.Play();

            waveOut.PlaybackStopped += (s, e) =>
            {
                waveOut.Dispose();
                reader.Dispose();
            };
        });
    }
}
