namespace BusinessLaag.Model
{
    public class TestResultaat
    {

        public TestResultaat(int id, Student student, Test test, int score, DateTime testMoment, int totaalVragen)
        {
            Id = id;
            Student = student;
            Test = test;
            Score = score;
            TestMoment = testMoment;
            TotaalVragen = totaalVragen;
        }

        public int Id { get; set; }
        public Student Student { get; set; }
        public Test Test { get; set; }
        public int Score { get; set; }
        public DateTime TestMoment { get; set; }
        public int TotaalVragen { get; set; }




        public override string ToString()
        {
            return $"Behaalde score: {Score}/{TotaalVragen}";
        }

    }
}
