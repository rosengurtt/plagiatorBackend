namespace Plagiator.Music.Models
{
    public partial class Song
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long BandId { get; set; }
        public Band Band { get; set; }
        public long StyleId { get; set; }
        public Style Style { get; set; }

      //  public int? TicksPerQuarterNote { get; set; }
        public string OriginalMidiBase64Encoded { get; set; }

        public string ProcessedMidiBase64Encoded { get; set; }

        public long TimeSignatureId { get; set; }
        public TimeSignature TimeSignature { get; set; }
        public int? TempoInMicrosecondsPerBeat { get; set; }
        public int? TempoInBeatsPerMinute { get; set; }
        public int? NumberTracks { get; set; }
        public int? NumberOfTicks { get; set; }
        public int? DurationInSeconds { get; set; }
        public int? NumberBars { get; set; }
        public int? TotalEvents { get; set; }
        public int? TotalNoteEvents { get; set; }
        public int? TotalPitchBendEvents { get; set; }
        public int? TotalControlChangeEvents { get; set; }

        public int? TotalSustainPedalEvents { get; set; }
        public int? TotalChannels { get; set; }
        public int? TotalInstruments { get; set; }
        public int? TotalPercussionInstruments { get; set; }
        public int? TotalTempoChanges { get; set; }
        /// <summary>
        /// C1 and C2 count as different pitches
        /// </summary>
        public int? TotalDifferentPitches { get; set; }
        /// <summary>
        /// C1 and C2 count as same pitch
        /// </summary>
        public int? TotalUniquePitches { get; set; }
        public int? HighestPitch { get; set; }
        public int? LowestPitch { get; set; }
        public int? TotalChannelIndependentEvents { get; set; }
        public int? TotalProgramChangeEvents { get; set; }
        public int? TotalChunks { get; set; }
        public int? TotalMelodicChunks { get; set; }
        public int? TotalChordChunks { get; set; }
        public bool? HasMoreThanOneChannelPerChunk { get; set; }
        public bool? HasMoreThanOneInstrumentPerChunk { get; set; }
        public bool? HasPercusion { get; set; }

        public void InitializeStats()
        {
            TempoInMicrosecondsPerBeat = null;
            NumberTracks = 0;
            NumberOfTicks = 0;
            DurationInSeconds = 0;
            TempoInMicrosecondsPerBeat = 0;
            TempoInBeatsPerMinute = 0;
            NumberBars = 0;
            TotalChunks = 0;
            TotalEvents = 0;
            TotalNoteEvents = 0;
            TotalPitchBendEvents = 0;
            TotalControlChangeEvents = 0;
            TotalChannels = 0;
            TotalInstruments = 0;
            TotalPercussionInstruments = 0;
            TotalTempoChanges = 0;
            TotalProgramChangeEvents = 0;
            TotalDifferentPitches = 0;
            TotalChannelIndependentEvents = 0;
            TotalSustainPedalEvents = 0;
            TotalMelodicChunks = 0;
            TotalChordChunks = 0;
            HasMoreThanOneChannelPerChunk = false;
            HasMoreThanOneInstrumentPerChunk = false;
            HasPercusion = false;
        }
    }
}
