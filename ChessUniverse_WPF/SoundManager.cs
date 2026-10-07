using NAudio.Wave;
using System.IO;

namespace ChessUniverse_WPF;

public static class SoundManager
{
    private static readonly Dictionary<string, string> Cache = new();

    public static void Load(string key, string path)
        => Cache[key] = path;

    public static void Play(string key)
    {
        if (!Cache.TryGetValue(key, out string? path) ||
            !File.Exists(path))
        {
            return;
        }

        Task.Run(() =>
        {
            var reader = new AudioFileReader(path);
            var waveOut = new WaveOutEvent();

            waveOut.Init(reader);
            waveOut.Play();

            waveOut.PlaybackStopped += (_, _) =>
            {
                waveOut.Dispose();
                reader.Dispose();
            };
        });
    }
}
