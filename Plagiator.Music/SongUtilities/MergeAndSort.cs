using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static List<MidiEvent> MergeEvents(List<MidiEvent> list1, List<MidiEvent> list2)
        {
            if (list1.Count == 0) return list2;
            if (list2.Count == 0) return list1;

            var retObj = new List<MidiEvent>();
            list1 = ConvertDeltaTimeToAccumulatedTime(list1);
            list2 = ConvertDeltaTimeToAccumulatedTime(list2);
            int i1 = 0;
            int i2 = 0;
            while (i1 < list1.Count && i2 < list2.Count)
            {
                if ((i2 == list2.Count && i1 < list1.Count) || list1[i1].DeltaTime <= list2[i2].DeltaTime)
                    retObj.Add(list1[i1++]);
                else retObj.Add(list2[i2++]);
            }
            if (i1 < list1.Count)
                for (var i = i1; i < list1.Count; i++) retObj.Add(list1[i]);
            if (i2 < list2.Count)
                for (var i = i2; i < list2.Count; i++) retObj.Add(list2[i]);
            return ConvertAccumulatedTimeToDeltaTime(retObj);
        }

        private static List<MidiEvent> GetSortedEventsList(List<MidiEvent> events)
        {
            var returnObj = new List<MidiEvent>();
            foreach (var e in events) returnObj.Add(e.Clone());
            for (int i = 0; i < returnObj.Count - 1; i++)
            {
                for (int j = i + 1; j < returnObj.Count; j++)
                {
                    if (returnObj[i].DeltaTime > returnObj[j].DeltaTime)
                    {
                        var aux = returnObj[i].Clone();
                        returnObj[i] = returnObj[j];
                        returnObj[j] = aux;
                    }
                }
            }
            return returnObj;
        }

    }
}
