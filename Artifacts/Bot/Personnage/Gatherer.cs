using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artifacts.Items;
using Artifacts.Monsters;
using Artifacts.MyAccount;
using Artifacts.MyCharacters;
using Artifacts.Utilities;
using Artifacts.Utilities.ConsoleManager;
using static Artifacts.Resources.ResourcesResponse;

namespace Artifacts.Bot.Personnage
{
    internal class Gatherer : Personnage
    {
        internal Enums.GathererTypeEnum gathererTypeEnum;
        public Gatherer(Enums.GathererTypeEnum type, MCU mCU)
        {
            gathererTypeEnum = type;
            mcu = mCU;
            WorkOrderList = new List<WorkOrder>();
        }

        internal override void QueFaire()
        {
            while (!termine)
            {
                if (IsInventairePlein() || doitViderEnBanque)
                {
                    Aller_Banque();
                    Vider_Inventaire();
                }
                if (CommandeEnAttente)
                {
                    Aller_Banque();
                    Vider_Inventaire();
                    RecupererCommande();
                    CommandeEnAttente = false;
                }

                if (NiveauMetierGagne && WorkOrderList.Count == 0)
                {
                    int niveauMetier = GetNiveauMetier();
                    List<Objet> ListeBest_tool = mcu.getObjetList().Where(x => x.type == Constantes.weapon && x.subtype == Constantes.tool && x.level <= niveauMetier && x.effects.Where(y => y.name == metier).Any()).ToList();
                    Objet best_tool = null;
                    if (ListeBest_tool.Count > 0)
                    {
                        best_tool = ListeBest_tool.OrderByDescending(x => x.level).FirstOrDefault();
                    }
                    if (best_tool != null && FeuillePerso.weapon_slot != best_tool.code)
                    {
                        WorkOrder wo = new WorkOrder();
                        wo.Quantité = 1;
                        wo.Code = best_tool.code;
                        wo.Demandeur = this;
                        mcu.AjouterWorkOrder(wo);
                        WorkOrderList.Add(wo);
                    }
                    NiveauMetierGagne = false;
                    //finir de passer la commande au MCU
                }

                Resource resourceARecuperer = null;
                WorkOrder wo_pris = null;
                //traiter les workOrder
                foreach (WorkOrder wo in mcu.GetListeWorkorderAttenteRessource())
                {
                    if (resourceARecuperer != null)
                    {
                        break;
                    }
                    //on va convertir le code de la ressource en Objet
                    List<Tuple<string, int>> listeTuplesCodeQuantite = wo.ListeDesResources;
                    if (listeTuplesCodeQuantite != null)
                    {
                        foreach (Tuple<string, int> t in listeTuplesCodeQuantite)
                        {
                            Resource TupleAsResource = mcu.ResourceList.Where(x => x.code == t.Item1).FirstOrDefault();
                            if (TupleAsResource != null)
                            {
                                if (TupleAsResource.skill == Constantes.fishing && FeuillePerso.fishing_level >= TupleAsResource.level)
                                {
                                    resourceARecuperer = TupleAsResource;
                                    wo_pris = wo;
                                    break;
                                }
                                if (TupleAsResource.skill == Constantes.mining && FeuillePerso.mining_level >= TupleAsResource.level)
                                {
                                    resourceARecuperer = TupleAsResource;
                                    wo_pris = wo;
                                    break;
                                }
                                if (TupleAsResource.skill == Constantes.woodcutting && FeuillePerso.woodcutting_level >= TupleAsResource.level)
                                {
                                    resourceARecuperer = TupleAsResource;
                                    wo_pris = wo;
                                    break;
                                }
                            }
                        }
                    }
                }
                NouveauWorkOrder = false;

                if (wo_pris != null)
                {
                    //aller sur la bonne tuile
                    AllerSurTuile(resourceARecuperer);
                    int niveau_actuel = GetNiveauMetier();
                    Character c = MyCharactersEndPoint.Gathering(FeuillePerso.name);
                    if (c != null)
                    {
                        FeuillePerso = c;
                    }
                    if (GetNiveauMetier() != niveau_actuel)
                    {
                        NiveauMetierGagne = true;
                    }
                }
                else
                {
                    while (!NiveauMetierGagne && !IsInventairePlein() && !NouveauWorkOrder && !termine)
                    {
                        int niveauMetier = GetNiveauMetier();
                        
                        FarmerMeilleureRessource(metier);
                    }
                }
            }
            MyCharactersEndPoint.Move(FeuillePerso.name, 0, 0, false);
            ConsoleManager.Write(FeuillePerso.name + "-> Fin", ConsoleColor.Gray);
        }
        internal void Vider_Inventaire()
        {
            List<Tuple<string,int>> aDeposer = new List<Tuple<string,int>>();
            foreach(Inventory inventory in FeuillePerso.inventory)
            {
                if (!String.IsNullOrEmpty(inventory.code))
                {
                    aDeposer.Add(new Tuple<string, int>(inventory.code, inventory.quantity));
                }
            }
            base.Vider_Inventaire(aDeposer);
        }
    }
}
