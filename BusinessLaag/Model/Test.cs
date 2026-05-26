namespace BusinessLaag.Model
{
    public class Test
    {
        public Test(int id, string code, DateTime aanmaakDatum, List<Vraag> vragenInTest, List<string> onderwerpen)
        {
            Id = id;
            Code = code;
            AanmaakDatum = aanmaakDatum;
            VragenInTest = vragenInTest;
            Onderwerpen = onderwerpen;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime AanmaakDatum { get; set; }

        public List<Vraag> VragenInTest { get; set; }
        public List<string> Onderwerpen { get; set; }


        public List<TestResultaat> DeelnemendeStudentenResultaten { get; set; } = new List<TestResultaat>();


        public override string ToString()
        {
            return Code;
        }
    }
}
