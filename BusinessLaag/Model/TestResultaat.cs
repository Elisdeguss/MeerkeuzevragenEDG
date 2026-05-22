using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLaag.Model
{
    public class TestResultaat
    {
        public TestResultaat(int id, Student student, Test test, int score, DateTime testMoment)
        {
            Id = id;
            Student = student;
            Test = test;
            Score = score;
            TestMoment = testMoment;
            TotaalVragen = test?.VragenInTest?.Count ?? 0;
        }

        public int Id { get; set; }
        public Student Student{ get; set; }
        public Test Test{ get; set; }
        public int Score { get; set; } 
        public DateTime TestMoment { get; set; }

        
        public int TotaalVragen { get; init; }


        public override string ToString()
        {
            return $"Behaalde score: {Score}/{TotaalVragen}";
        }

    }
}
