using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TimetableLib.Changes;

namespace TimeTableProcessor
{
    public class Processor
    {
        public async Task<List<TeacherChange>> ChangesProcessor(Stream RawChanges)
        {
            var stmReader = new StreamReader(RawChanges);
            var rawHtml = await stmReader.ReadToEndAsync();
            rawHtml = rawHtml.ToLower();
            var tbody = rawHtml[rawHtml.IndexOf("<body>")..rawHtml.IndexOf("</body>")];

            return new List<TeacherChange>();
        }
        public async Task<List<ClassChange>> TimetableProcessor(Stream RawTimetable)
        {
            throw new NotImplementedException();
        }
    }
}
