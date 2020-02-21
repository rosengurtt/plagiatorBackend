namespace Plagiator.Music.Models
{
    /// <summary>
    /// The main reason for creating this class is for persistence in sql
    /// The useful classes are PitchPattern, RythmPattern and MelodyPattern
    /// </summary>
    public class Pattern
    {
        public long Id { get; set; }

        public string AsString { get; set; }

        public PatternType PatternType { get; set; }
    }
    public enum PatternType
    {
        Pitch = 1,
        Rythm = 2,
        Melody = 3
    }
}
