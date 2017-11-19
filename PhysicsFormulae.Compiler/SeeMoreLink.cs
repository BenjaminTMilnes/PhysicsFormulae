namespace PhysicsFormulae.Compiler
{
    public class SeeMoreLink
    {
        public string Name { get; set; }
        public string URL { get; set; }

        public SeeMoreLink() { }

        public SeeMoreLink(string name, string url)
        {
            Name = name;
            URL = url;
        }
    }
}
