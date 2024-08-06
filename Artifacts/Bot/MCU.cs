using Artifacts.Monsters;
using Artifacts.Maps;
using Artifacts.MyCharacters;
using Artifacts.Status;
using Artifacts.Utilities.ConsoleManager;
using Artifacts.Items;
using static Artifacts.Resources.ResourcesResponse;
using Artifacts.Resources;
using Artifacts.Bot.Personnage;
using Artifacts.Utilities;
using static Artifacts.Utilities.Enums;
using Artifacts.MyAccount;
using System.Diagnostics;

namespace Artifacts.Bot
{
    internal class MCU //Main Control Unit
    {
        public bool Stop = false;
        private List<Personnage.Personnage> ListePersonnages { get; set; }
        public List<Tile> Map { get; private set; }
        public List<Monster> MonsterList { get; private set; }
        private List<Thread> ListeThreads = new List<Thread>();
        public List<Resource> ResourceList { get; private set; }
        private List<Item> BankItemList { get; set; }
        public List<Objet> ObjetList { get; private set; }

        private List<WorkOrder> ListeWorkorderAttenteRessource = new List<WorkOrder>();
        private List<WorkOrder> ListeWorkorderAttenteCraft = new List<WorkOrder>();
        //private List<Tuple<string, int>> ListeDesResources = new List<Tuple<string, int>>(); //contient le code du drop à utiliser dans GetAllResources et la quantité requise
        private ConsoleColor originalConsoleColor = Console.ForegroundColor;
        private List<Tuple<string, int>> ItemsDeEquipe = new List<Tuple<string, int>>(); //code, quantité

        public void endMCU()
        {
            SaveAndClose();
            ConsoleManager.SetColor(originalConsoleColor);
        }

        public MCU()
        {
            //changement de couleur de la console (petit kiff)
            ConsoleManager.SetColor(ConsoleManager.defaultConsoleColor);
        }

        public void Init()
        {
            BankItemList = new List<Item>();

            ListePersonnages = new List<Personnage.Personnage>();
            GetServerStatus();
            ConsoleManager.Write("");

            ConsoleManager.Write("Récupération des personnages ");
            Character[] CharactersArray = MyCharactersEndPoint.GetMyCharacters();

            Personnage.Personnage socks = new Gatherer(GathererTypeEnum.Bucheron, this);
            socks.FeuillePerso = CharactersArray[0];
            socks.metier = Constantes.woodcutting;
            ListePersonnages.Add(socks);
            
            Personnage.Personnage pi = new Gatherer(GathererTypeEnum.Bucheron, this);
            pi.FeuillePerso = CharactersArray[1];
            pi.metier = Constantes.mining;
            ListePersonnages.Add(pi);

            Personnage.Personnage atri = new Gatherer(GathererTypeEnum.Bucheron, this);
            atri.FeuillePerso = CharactersArray[2];
            atri.metier = Constantes.fishing;
            ListePersonnages.Add(atri);


            Personnage.Personnage phage = new Guerrier(this);
            phage.FeuillePerso = CharactersArray[3];
            ListePersonnages.Add(phage);

            ConsoleManager.Write("");

            ConsoleManager.Write("Récupération de la Banque");
            ChargerBanque();
            ConsoleManager.Write("");

            ConsoleManager.Write("Création de la liste ItemsDeEquipe");
            ChargerItemsDeEquipe();
            ConsoleManager.Write("");

            ConsoleManager.Write("Récupération de la carte");
            AllMapResponse allMapResponse = MapsEndPoint.GetAllMaps(string.Empty, string.Empty, 1, 100);
            Map = allMapResponse.data;
            for (int i = 2; i <= allMapResponse.pages; i++)
            {
                Map.AddRange(MapsEndPoint.GetAllMaps(string.Empty, string.Empty, i, 100).data);
            }
            ConsoleManager.Write("");

            ConsoleManager.Write("Récupération des Monstres");
            AllMonstersResponse allMonstersResponse = MonstersEndPoint.GetAllMonsters(null, null, null, 1, 100);
            MonsterList = allMonstersResponse.data;
            for (int i = 2; i <= allMonstersResponse.pages; i++)
            {
                MonsterList.AddRange(MonstersEndPoint.GetAllMonsters(null, null, null, i, 100).data);
            }
            ConsoleManager.Write("");

            ConsoleManager.Write("Récupération des Items");
            AllItemsResponse allItemsResponse = ItemsEndpoint.GetAllItems(null, null, null, null, null, null, 1, 100);
            ObjetList = allItemsResponse.data;
            for (int i = 2; i <= allItemsResponse.pages; i++)
            {
                ObjetList.AddRange(ItemsEndpoint.GetAllItems(null, null, null, null, null, null, i, 100).data);
            }
            ConsoleManager.Write("");

            ConsoleManager.Write("Récupération des Resources");
            AllResourcesResponse allIResourcesResponse = ResourcesEndPoint.GetAllResources(null, null, null, null, 1, 100);
            ResourceList = allIResourcesResponse.data;
            for (int i = 2; i <= allItemsResponse.pages; i++)
            {
                ResourceList.AddRange(ResourcesEndPoint.GetAllResources(null, null, null, null, i, 100).data);
            }
            ConsoleManager.Write("");
        }

        private void ChargerBanque()
        {
            lock (BankItemList)
            {
                if (BankItemList != null)
                {
                    BankItemList.Clear();
                }
                BankItemResponse bankItemResponse = MyAccountEndpoint.GetBankItems(null, 1, 100);
                BankItemList = bankItemResponse.data;
                for (int i = 2; i <= bankItemResponse.pages; i++)
                {
                    BankItemList.AddRange(MyAccountEndpoint.GetBankItems(null, 1, 100).data);
                }
            }
        }

        public void Run()
        {

            ConsoleManager.Write("RUNNING !");
            WorkOrder WorkOrderEnCoursdeCollecte = null;
            WorkOrder wo = new WorkOrder();
            wo.Quantité = 10;
            wo.Demandeur = ListePersonnages.First();
            wo.Code = "cooked_chicken";
            ListeWorkorderAttenteRessource.Add(wo);

            foreach (Personnage.Personnage p in ListePersonnages)
            {
                Thread t = new Thread(p.QueFaire);
                t.Start();
                ListeThreads.Add(t);
            }
            while (!Stop)
            {
                //Dummy test
                /*List<Tuple<string,int>> lt = DecomposerObjet("feather_coat", 1);
                Tuple<string, int> feather = new Tuple<string, int>("feather", 10);
                ItemsDeEquipe.Add(feather);
                Tuple<string, int> gold = new Tuple<string, int>("gold", 1);
                ItemsDeEquipe.Add(gold);
                List<Tuple<string, int>> lt2 = GenererListeDesDrops("feather_coat", 1, ItemsDeEquipe);
                bool b = isCraftable(lt2);*/
                
                if (WorkOrderEnCoursdeCollecte == null)
                {
                    int i = 0;
                    while (WorkOrderEnCoursdeCollecte == null && i < ListeWorkorderAttenteRessource.Count)
                    {
                        List<Tuple<string, int>> listeResources = GenererListeDesDrops(ListeWorkorderAttenteRessource[i].Code, ListeWorkorderAttenteRessource[i].Quantité, ItemsDeEquipe);
                        if (isCraftable(listeResources))
                        {
                            WorkOrderEnCoursdeCollecte = ListeWorkorderAttenteRessource[i];
                            WorkOrderEnCoursdeCollecte.ListeDesResources = listeResources;
                            ListeWorkorderAttenteRessource[i].ListeDesResources = listeResources;
                            foreach(Personnage.Personnage p in ListePersonnages)
                            {
                                p.NouveauWorkOrder = true;
                            }
                        }
                        i++;
                    }
                }
                else
                {
                    if (GenererListeDesDrops(WorkOrderEnCoursdeCollecte.Code, WorkOrderEnCoursdeCollecte.Quantité, ItemsDeEquipe).Count == 0) //ça veut dire que mon équipe a loot toutes les resources pour faire le boulot
                    {
                        lock (ListeWorkorderAttenteRessource)
                        {
                            ListeWorkorderAttenteRessource.Remove(WorkOrderEnCoursdeCollecte);
                        }
                            foreach (Personnage.Personnage p in ListePersonnages)
                        {
                            p.RamenerDrops(WorkOrderEnCoursdeCollecte.ListeDesResources);
                        }
                        if (ToutEstEnBanque(WorkOrderEnCoursdeCollecte))
                        {
                            lock (ListeWorkorderAttenteCraft)
                            {
                                ListeWorkorderAttenteCraft.Add(WorkOrderEnCoursdeCollecte);
                            }
                            WorkOrderEnCoursdeCollecte = null;
                        }
                    }
                }
                
            }
        }

        private void SaveAndClose()
        {
            ConsoleManager.Write("Save and Close",ConsoleManager.errorConsoleColor);
            foreach (Personnage.Personnage p in ListePersonnages)
            {
                p.termine = true;
            }
        }

        private void GetServerStatus()
        {
            StatusResponse response = Status.StatusEndPoint.GetStatus();
            if (response == null)
            {
                ConsoleManager.Write("Erreur lors de la récupération du status du serveur", ConsoleManager.errorConsoleColor);
                throw new Exception();
            }
            else
            {
                ConsoleManager.Write("Server status : " + response.data.status);
                ConsoleManager.Write("Server version : " + response.data.version);
                ConsoleManager.Write("Characters online : " + response.data.characters_online);
                ConsoleManager.Write("Last wipe : " + response.data.last_wipe);
                ConsoleManager.Write("Next wipe : " + response.data.next_wipe);
            }
        }

        public List<Tuple<string, int>> GenererListeDesDrops(string codeItem,int quantite, List<Tuple<string, int>> _ItemsDeEquipe)
        {
            lock (_ItemsDeEquipe)
            {
                List<Tuple<string, int>> listeDesResources = new List<Tuple<string, int>>();
                Objet objet = ObjetList.Where(x => x.code.Equals(codeItem)).FirstOrDefault();
                foreach (Item item in objet.craft.items)
                {
                    Tuple<string, int> itemDeEquipe = _ItemsDeEquipe.Where(x => x.Item1.Equals(item.code)).FirstOrDefault();
                    int nb_existant = 0;
                    if (itemDeEquipe != null)
                    {
                        nb_existant = itemDeEquipe.Item2;
                    }
                    int NbRequises = Math.Max(0, Int32.Parse(item.quantity) - nb_existant);
                    if (NbRequises > 0)
                    {
                        //maintenant on doit regarder si c'est une ressource, un drop ou un Objet
                        Objet obj = ObjetList.Where(x => x.code.Equals(item.code)).First();
                        if (obj.type == "resource")
                        {
                            listeDesResources.Add(new Tuple<string, int>(item.code, NbRequises * quantite));
                        }
                        else
                        {
                            listeDesResources.AddRange(DecomposerObjet(item.code, NbRequises * quantite));
                        }
                    }
                }

                return listeDesResources;
            }
        }

        public List<Tuple<string,int>> GetItemsDeEquipe()
        {
            return ItemsDeEquipe;
        }

        private List<Tuple<string, int>> DecomposerObjet(string objetCode, int quantite)
        {
            List<Tuple<string, int>> liste = new List<Tuple<string, int>>();

            Objet obj = ObjetList.Where(x => x.code.Equals(objetCode)).First();
            if (obj.craft != null)
            {
                foreach (Item item in obj.craft.items)
                {
                        liste.AddRange(DecomposerObjet(item.code, Int32.Parse(item.quantity)*quantite));
                }
            }
            else
            {
                Tuple<string, int> t = new Tuple<string, int>(obj.code, quantite);
                liste.Add(t);
            }

            //on va refaire du tri pour n'avoir qu'un unique tuple si on a plusieurs tuples avec le même code
            List<string> liste_codes = new List<string>();
            foreach(Tuple<string, int> t in liste)
            {
                if (!liste_codes.Contains(t.Item1))
                {
                    liste_codes.Add(t.Item1);
                }
            }
            List<Tuple<string,int>> liste_triee = new List<Tuple<string, int>> ();
            foreach (string code in liste_codes)
            {
                int qte = 0;
                foreach (Tuple<string,int> t in liste)
                {
                    if (t.Item1.Equals(code))
                    {
                        qte += t.Item2;
                    }
                }
                liste_triee.Add(new Tuple<string, int> ( code, qte ));
            }
            return liste_triee;
        }

        public bool isCraftable(List<Tuple<string, int>> listeDrops)
        {
            bool craftable = true;

            foreach (Tuple<string, int> tuple in listeDrops)
            {
                foreach (Resource r in ResourceList)
                {
                    if(r.drops.Where(x => x.code.Equals(tuple.Item1)).Any())
                    {
                        string skill = r.skill;
                        int lvlMaxMineur = 0, lvlMaxBucheron = 0, lvlMaxFishing = 0;

                        foreach(Personnage.Personnage p in ListePersonnages)
                        {
                            lvlMaxBucheron = Math.Max(lvlMaxBucheron, p.FeuillePerso.woodcutting_level);
                            lvlMaxBucheron = Math.Max(lvlMaxFishing, p.FeuillePerso.fishing_level);
                            lvlMaxBucheron = Math.Max(lvlMaxMineur, p.FeuillePerso.mining_level);
                        }

                        if (skill == Constantes.mining && lvlMaxMineur < r.level)
                        {
                            return false;
                        }

                        if (skill == Constantes.woodcutting && lvlMaxBucheron < r.level)
                        {
                            return false;
                        }

                        if (skill == Constantes.fishing && lvlMaxFishing < r.level)
                        {
                            return false;
                        }
                    }
                }

                foreach(Monster m in MonsterList.Where(x => x.drops.Where(y => y.code == tuple.Item1).Any()))
                {
                    bool tuable = false;
                    foreach (Personnage.Personnage p in ListePersonnages.Where(x => x.metier == Constantes.combat))
                    {
                        if (CSPCombat(p,m).Count > 0)
                        {
                            tuable = true; break;
                        }
                    }
                    return tuable;
                }
            }

            return craftable;
        }

        public List<string> CSPCombat(Personnage.Personnage p, Monster m)
        {
            CSP_Fight cSP_Fight = new CSP_Fight(p, m, this.ObjetList);
            return cSP_Fight.GenererListePossibilites(this.ConsulterBanque());
        }

        public List<WorkOrder> GetListeWorkorderAttenteRessource()
        {
            lock (ListeWorkorderAttenteRessource)
            {
                return ListeWorkorderAttenteRessource;
            }
        }

        private bool PeutGagner(Personnage.Personnage p,  Monster m)
        {
            bool victoire = false, finCombat = false;
            int i = 1;
            int hp_monstre = m.hp;
            int hp_joueur = p.FeuillePerso.hp;
            Objet conso1 = ObjetList.Where(x => x.type == "consumable" && x.code == p.FeuillePerso.consumable1_slot).FirstOrDefault();
            Objet conso2 = ObjetList.Where(x => x.type == "consumable" && x.code == p.FeuillePerso.consumable2_slot).FirstOrDefault();
            int boost_fire_dmg = 0, boost_water_dmg = 0, boost_earth_dmg = 0, boost_air_dmg = 0;
            bool restore_utilise = false;
            if (conso1 != null)
            {
                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_air")).Any())
                {
                    boost_air_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_air")).First().value;
                }
                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_fire")).Any())
                {
                    boost_fire_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_fire")).First().value;
                }

                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_earth")).Any())
                {
                    boost_earth_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_earth")).First().value;
                }

                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_water")).Any())
                {
                    boost_water_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_water")).First().value;
                }
            }
            //
            if (conso2 != null)
            {
                if (conso2.effects.Where(x => x.name.Equals("boost_dmg_air")).Any())
                {
                    boost_air_dmg += conso2.effects.Where(x => x.name.Equals("boost_dmg_air")).First().value;
                }
                if (conso2.effects.Where(x => x.name.Equals("boost_dmg_fire")).Any())
                {
                    boost_fire_dmg += conso2.effects.Where(x => x.name.Equals("boost_dmg_fire")).First().value;
                }

                if (conso2.effects.Where(x => x.name.Equals("boost_dmg_earth")).Any())
                {
                    boost_earth_dmg += conso2.effects.Where(x => x.name.Equals("boost_dmg_earth")).First().value;
                }

                if (conso2.effects.Where(x => x.name.Equals("boost_dmg_water")).Any())
                {
                    boost_water_dmg += conso2.effects.Where(x => x.name.Equals("boost_dmg_water")).First().value;
                }
            }

            while (i <=50 && !finCombat)
            {
                int dégat_feu = (int)(p.FeuillePerso.attack_fire * (1 + (double)((p.FeuillePerso.dmg_fire + boost_fire_dmg) / 100)) * (1 - ((double)m.res_fire / 100)));
                int dégat_eau = (int)(p.FeuillePerso.attack_water * (1 + (double)((p.FeuillePerso.dmg_water + boost_water_dmg) / 100)) * (1 - ((double)m.res_water / 100)));
                int dégat_terre =(int)( p.FeuillePerso.attack_earth * (1 + (double)((p.FeuillePerso.dmg_earth + boost_earth_dmg) / 100)) * (1 - ((double)m.res_earth / 100)));
                int dégat_air = (int)(p.FeuillePerso.attack_air * (1 + (double)((p.FeuillePerso.dmg_air + boost_air_dmg) / 100)) * (1 - ((double)m.res_air / 100)));

                hp_monstre = hp_monstre - dégat_feu - dégat_eau - dégat_terre - dégat_air;

                if (hp_monstre <= 0)
                {
                    victoire = true;
                    finCombat = true;
                    continue;
                }


                dégat_feu = (int)(m.attack_fire * (1) * (1 - ((double)p.FeuillePerso.res_fire / 100)));
                dégat_eau = (int)(m.attack_water * (1) * (1 - ((double)p.FeuillePerso.res_water / 100)));
                dégat_terre = (int)(m.attack_earth * (1) * (1 - ((double)p.FeuillePerso.res_earth / 100)));
                dégat_air = (int)(m.attack_air * (1) * (1 - ((double)p.FeuillePerso.res_air / 100)));

                hp_joueur = hp_joueur - dégat_feu - dégat_eau - dégat_terre - dégat_air;



                if (hp_joueur <= 0)
                {
                    victoire = false;
                    finCombat = true;
                    continue;
                }

                if (hp_joueur <= (p.FeuillePerso.hp /2) && !restore_utilise)
                {
                    restore_utilise = true;
                    if (conso1 != null && conso1.effects.Where(x => x.name == "restore").Any())
                    {
                        hp_joueur += conso1.effects.Where(x => x.name == "restore").First().value;
                    }
                    if (conso2 != null && conso2.effects.Where(x => x.name == "restore").Any())
                    {
                        hp_joueur += conso2.effects.Where(x => x.name == "restore").First().value;
                    }
                }

                i++;
            }
            return victoire;
        }

        private bool ToutEstEnBanque(WorkOrder workOrder)
        {
            List<Tuple<string,int>> listeBanque = new List<Tuple<string,int>>();
            foreach (Item item in BankItemList)
            {
                Tuple<string,int> t = new Tuple<string,int>(item.code,Int32.Parse(item.quantity));
                listeBanque.Add(t);
            }
            List<Tuple<string, int>> liste_compos = GenererListeDesDrops(workOrder.Code, workOrder.Quantité, listeBanque);
            if (liste_compos.Count == 0)
            {
                return true;
            }
            return false;
        }

        public List<Item> ConsulterBanque()
        {
            lock (BankItemList)
            { 
            ChargerBanque();
            return new List<Item>(BankItemList);
            }
        }

        public List<Objet> getObjetList()
        {
            return this.ObjetList;
        }

        internal Character DeposerBanque(string name, string code, int quantity)
        {
            Character c = MyCharactersEndPoint.DepositBank(name, code, quantity);
            ChargerBanque();
            ChargerItemsDeEquipe();
            return c;
        }

        public Character RecupererDeBanque(string name, string code, int quantite)
        {
            Character c = MyCharactersEndPoint.WithdrawBank(name,code, quantite);
            ChargerBanque();
            ChargerItemsDeEquipe();
            return c;
        }

        public void AjouterWorkOrder(WorkOrder workOrder)
        {
            lock (ListeWorkorderAttenteRessource)
            {
                ListeWorkorderAttenteRessource.Add(workOrder);
                foreach (Personnage.Personnage p in ListePersonnages)
                {
                    p.NouveauWorkOrder = true;
                }
            }
        }

        public void SupprimerCommandeLivree(WorkOrder wo)
        {
            lock (ListeWorkorderAttenteRessource)
            {
                ListeWorkorderAttenteCraft.Remove(wo);
            }
        }

        public void ActualiserResources()
        {
            ChargerItemsDeEquipe();
        }

        private void ChargerItemsDeEquipe()
        {
            lock(ItemsDeEquipe)
            {
                ItemsDeEquipe.Clear();
                foreach (Personnage.Personnage p in ListePersonnages)
                {
                    foreach (Inventory inventory in p.FeuillePerso.inventory)
                    {
                        Tuple<string, int> t = new Tuple<string, int>(inventory.code, inventory.quantity);
                        ItemsDeEquipe.Add(t);
                    }
                }
                lock(BankItemList)
                {
                    foreach(Item item in BankItemList)
                    {
                        Tuple<string, int> t = new Tuple<string, int>(item.code, Int32.Parse(item.quantity));
                        ItemsDeEquipe.Add(t);
                    }
                }
            }
        }
    }
}
