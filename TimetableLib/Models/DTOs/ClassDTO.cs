﻿using TimetableLib.Models.DBModels;

namespace TimetableLib.Models.DTOs
{
    public class ClassDTO
    {
        public ClassDTO(ClassDB cl)
        {
            //Id = (long) cl.Id;
            Name = cl.Name;
            Timetable = new TimetableDTO(cl.Timetable);
        }

        //public long Id { get; set; }
        public string Name { get; set; }
        public TimetableDTO Timetable { get; set; }
    }
}