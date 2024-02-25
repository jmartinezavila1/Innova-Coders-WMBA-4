﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMBA_4.Models
{
    public class Inning
    {
        public int ID { get; set; }

        [Display(Name = "Inning Number")]
        public int InningNumber { get; set; }
        public int TeamID { get; set; }
        public Team Team { get; set; }
        public int GameID { get; set; }
        public Game Game { get; set; }

        public ICollection<Inplay> Inplays { get; set; } = new HashSet<Inplay>();

    }
}
