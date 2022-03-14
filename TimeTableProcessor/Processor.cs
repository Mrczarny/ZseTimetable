using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TimetableLib.Changes;

namespace TimeTableProcessor
{
    public class Processor
    {
        private HttpClient _client;
        public Processor(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<TeacherChange>> ChangesProcessor()
        {
        
        }
        public async Task<List<ClassChange>> TimeetableProcessor()
        {

        }
    }
}
