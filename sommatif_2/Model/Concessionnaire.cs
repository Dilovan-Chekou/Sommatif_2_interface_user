using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sommatif_2.Model
{
    public class Concessionnaire
    {
        public ObservableCollection<Client> Clients { get; private set; }

        public Concessionnaire()
        {
            Clients = new ObservableCollection<Client>();
        }

    }
}
