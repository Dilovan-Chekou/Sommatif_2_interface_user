using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatriceAchatVoiture.Model
{
    public class Client
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public ObservableCollection<Simulation> Simulations { get; set; }

        public Client(string nom, string prenom)
        {
            Nom = nom;
            Prenom = prenom;
            Simulations = new ObservableCollection<Simulation>();
        }
    }
}
