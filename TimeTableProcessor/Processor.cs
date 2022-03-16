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
            var rawHtml = (await stmReader.ReadToEndAsync()).ToLower();
            var body = rawHtml[rawHtml.IndexOf("<body>")..rawHtml.IndexOf("</body>")];

            var tds =
                (from x in body.Split("<")
                    where x != "/td>\r\n" && x != "/tr>\r\n"              //weird but fun.
                    select x).ToList();
            List<List<string>> d = new List<List<string>>();
            List<string> p = new List<string>();
            foreach (var td in tds)
            {
                if (td != "tr>\r\n") p.Add(td);
                else
                {
                    d.Add(p);
                    p = new List<string>();
                }
            }

            return new List<TeacherChange>();
        }
        public async Task<List<ClassChange>> TimetableProcessor(Stream RawTimetable)
        {
            throw new NotImplementedException();
        }
    }
}
