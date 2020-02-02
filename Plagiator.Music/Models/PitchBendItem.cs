namespace Plagiator.Music.Models
{
    public class PitchBendItem
    {
        public long absTime {get;set;}
        public int pitch { get; set; }
        
        public PitchBendItem Clone()
        {
            return new PitchBendItem
            {
                absTime = this.absTime,
                pitch = this.pitch
            };
        }
    }
}
