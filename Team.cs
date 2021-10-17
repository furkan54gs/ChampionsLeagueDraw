using System;


namespace ChampionsLeagueDraw
{
    public class Team
    {
        public string TeamName { get; set; }
        public string Nation { get; set; }
        public int Point { get; set; }
        public int Scored { get; set; }
        public int UnScored { get; set; }

        public override string ToString()
        {
            return this.TeamName+" "+Nation;
        }
    }
}