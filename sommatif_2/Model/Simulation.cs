using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CalculatriceAchatVoiture.Model
{
    public class Simulation : ObservableObject
    {

        private decimal TAXES = 0.14975m;
        private decimal DEPRECIATION_ANNUELLE_1AN = 0.2m;
        private decimal DEPRECIATION_ANNUELLE_MOINS_4ANS = 0.1m;
        private decimal DEPRECIATION_ANNUELLE_PLUS_5ANS = 0.05m;

        public DateTime Date { get; private set; }

        private decimal _montantFinance;
        public decimal MontantFinance
        {
            get { return _montantFinance; }
            set
            {
                _montantFinance = value >= 0 ? value : 0;
                ActualiserResultats();
            }
        }

        private decimal _tauxInterets;
        public decimal TauxInterets
        {
            get { return _tauxInterets; }
            set
            {
                _tauxInterets = value >= 0 ? value : 0;
                ActualiserResultats();
            }
        }

        private TypeTransaction _typeDeTransaction;
        public TypeTransaction TypeDeTransaction
        {
            get { return _typeDeTransaction; }
            set
            {
                _typeDeTransaction = value;
                ActualiserResultats();
            }
        }

        private bool _avecTaxes;
        public bool AvecTaxes
        {
            get { return _avecTaxes; }
            set
            {
                _avecTaxes = value;
                ActualiserResultats();
            }
        }

        private int _dureeMois;
        public int DureeMois
        {
            get { return _dureeMois; }
            set
            {
                _dureeMois = value >= 0 ? value : 0;
                ActualiserResultats();
            }
        }

        public Simulation()
        {
            Date = DateTime.Now.Date;
        }

        /// <summary>
        /// Coût selon le type de transaction avec ou sans taxe
        ///     Comptant : Montant taxé ou non
        ///     Financement : Montant taxé ou non
        ///     Location : (Montant - valeur résiduelle à la fin du contrat) taxé ou non
        /// </summary>
        public decimal CoutTotal
        {
            get
            {
                decimal retour = MontantFinance;
                if (TypeDeTransaction == TypeTransaction.Location)
                    retour = retour - ValeurResiduelle;

                if (AvecTaxes)
                    retour = retour * (1 + TAXES);

                return Math.Round(retour, 2);
            }
        }

        /// <summary>
        /// Formule du calcul de la valeur d'un paiement mensuel :
        /// ( Prêt * (Taux annuel / 12) * (1 + Taux annuel/12)^NbMensualités ) / ( ((1 + Taux annuel/12)^NbMensualités) - 1 )
        /// </summary>
        public decimal CoutMensuel
        {
            get
            {
                decimal terme1 = (decimal)Math.Pow(1 + ((double)TauxInterets / 100) / 12, DureeMois);
                decimal terme2 = (terme1 - 1);

                // Divisions par 0
                if (terme2 == 0 || DureeMois == 0) return 0;

                if (TauxInterets == 0)
                    return Math.Round(CoutTotal / DureeMois, 2);
                else
                    return (CoutTotal * ((TauxInterets / 100) / 12) * terme1) / terme2;
            }
        }

        /// <summary>
        /// -20% après 12 mois
        /// -10%/an pour les 4 années suivantes
        /// -5%/an par la suite
        /// </summary>
        public decimal ValeurResiduelle
        {
            get
            {
                decimal valeur = MontantFinance;

                for (int ans = 1; ans <= DureeMois / 12; ans++)
                {
                    switch (ans)
                    {
                        case 1:
                            valeur -= valeur * DEPRECIATION_ANNUELLE_1AN; // -20% après 12 mois
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            valeur -= valeur * DEPRECIATION_ANNUELLE_MOINS_4ANS; // -10% pour les 4 années suivantes
                            break;
                        default:
                            valeur -= valeur * DEPRECIATION_ANNUELLE_PLUS_5ANS; // -5% pour les années restantes
                            break;
                    }
                }

                return valeur;
            }
        }

        //public event PropertyChangedEventHandler? PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        private void ActualiserResultats()
        {
            OnPropertyChanged(nameof(CoutTotal));
            OnPropertyChanged(nameof(CoutMensuel));
            OnPropertyChanged(nameof(ValeurResiduelle));
        }

    }
}
