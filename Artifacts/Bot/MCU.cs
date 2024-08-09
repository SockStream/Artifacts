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
using Artifacts.Characters;
using Artifacts.GrandExchange;

namespace Artifacts.Bot
{
    internal class MCU //Main Control Unit
    {
        public bool Stop = false;
        private List<Personnage.Personnage> ListePersonnages { get; set; }
        public List<Tile> Map { get; private set; }
        public List<Monster> MonsterList { get; private set; }
        private List<Thread> ListeThreads = new List<Thread>();
        private List<Resource> ResourceList { get; set; }
        private List<Item> BankItemList { get; set; }
        public List<Objet> ObjetList { get; private set; }

        private List<WorkOrder> ListeWorkorderAttenteRessource = new List<WorkOrder>();
        private WorkOrder WorkorderAttenteCraft = null;
        private List<Tuple<string, int>> ListeDesResourcesPourTousLesCrafts = new List<Tuple<string, int>>(); //code,quantité
        private ConsoleColor originalConsoleColor = Console.ForegroundColor;

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

            ConsoleManager.Write("Récupération de la Banque");
            ChargerBanque();
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

            ConsoleManager.Write("Récupération des personnages ");
            Character[] CharactersArray = MyCharactersEndPoint.GetMyCharacters();

            foreach (Character character in CharactersArray)
            {
                //CharactersEndPoint.DeleteCharacter(character.name);
                //CharactersEndPoint.Create(character.name, character.skin);
            }
            CharactersArray = MyCharactersEndPoint.GetMyCharacters();

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
            ((Guerrier)phage).rattrapage_stuff();


            Personnage.Personnage Celia = new Crafteur(this);
            Celia.FeuillePerso = CharactersArray[4];
            ListePersonnages.Add(Celia);

            /*Celia.Aller_Banque();
            while (ConsulterBanque().Count > 0)
            {
                Item item = ConsulterBanque().First();
                RecupererDeBanque(Celia.FeuillePerso.name, item.code, Math.Min(Int32.Parse(item.quantity),50));
                Celia.Aller_GrandExchange();
                GrandExchange.GrandExchange exchange = GrandExchangeEndPoint.GetItem(item.code);
                Character c = MyCharactersEndPoint.GeSellItem(Celia.FeuillePerso.name, item.code, Math.Min(Int32.Parse(item.quantity), 50), exchange.sell_price);

                Celia.Aller_Banque();
                MyCharactersEndPoint.DepositBankGold(Celia.FeuillePerso.name, Celia.FeuillePerso.gold);
                ChargerBanque();
            }*/

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

            foreach (Personnage.Personnage p in ListePersonnages)
            {
                //p.doitViderEnBanque = true;
                Thread t = new Thread(p.QueFaire);
                t.Name = p.FeuillePerso.name;
                t.Start();
                ListeThreads.Add(t);
            }
            while (!Stop)
            {
                List<Tuple<string, int>> listeGlobaleResources = new List<Tuple<string, int>>();
                List<WorkOrder> workOrders = new List<WorkOrder>();
                lock (ListeWorkorderAttenteRessource)
                {
                    workOrders = new List<WorkOrder>(ListeWorkorderAttenteRessource);
                }
                foreach (WorkOrder wo in workOrders)
                {
                    List<Tuple<string, int>> resources_du_workorder = GenererListeDesDrops(wo.Code, wo.Quantité, CalculerItemsDeEquipeSaufCrafteur());
                    foreach(Tuple<string, int> t in resources_du_workorder)
                    {
                        if(listeGlobaleResources.Where(x => x.Item1 == t.Item1).Any())
                        {
                            Tuple<string,int> tupleResourceExistant = listeGlobaleResources.Where(x => x.Item1 == t.Item1).First();
                            Tuple<string, int> nouveauTuple = new Tuple<string, int>(t.Item1, t.Item2 + tupleResourceExistant.Item2);
                            listeGlobaleResources.Remove(tupleResourceExistant);
                            listeGlobaleResources.Add(nouveauTuple);
                        }
                        else
                        {
                            listeGlobaleResources.Add(t);
                        }
                    }
                }
                lock(ListeDesResourcesPourTousLesCrafts)
                {
                    ListeDesResourcesPourTousLesCrafts = listeGlobaleResources;
                }

                if (WorkorderAttenteCraft == null)
                {
                    //maintenant on regarde si un WorkOrder est craftable ==> sa liste est vide
                    List<WorkOrder> ListeWorkorderAttenteRessource = GetListeWorkorderAttenteRessource();
                    foreach (WorkOrder wo in ListeWorkorderAttenteRessource)
                    {
                        /*if (wo.Demandeur.GetType() == typeof(Crafteur))
                        {
                            continue;
                        }*/
                        List<Tuple<string, int>>  listeDrops = GenererListeDesDrops(wo.Code, wo.Quantité, CalculerItemsDeEquipeSaufCrafteur());
                        bool aLeNiveauPourCrafter = false;
                        foreach (Crafteur p in ListePersonnages.Where(x => x.GetType() == typeof(Crafteur)))
                        {
                            if (p.PeutCrafter(wo.Code))
                            {
                                aLeNiveauPourCrafter = true;
                            }
                        }
                        if (listeDrops.Count == 0 && aLeNiveauPourCrafter)
                        {
                            listeDrops = GenererListeDesDrops(wo.Code, wo.Quantité, new List<Tuple<string, int>>());
                            foreach (Personnage.Personnage p in ListePersonnages)
                            {
                                if (p.GetType() != typeof(Crafteur))
                                {
                                    foreach(Inventory inv in p.FeuillePerso.inventory)
                                    {
                                        foreach (Tuple<string, int>  t in listeDrops)
                                        {
                                            if (t.Item1 == inv.code)
                                            {
                                                p.doitViderEnBanque = true;
                                            }
                                        }
                                    }
                                    //p.doitViderEnBanque = true; //Todo : optimiser pour ne laisser que ceux qui ont du stuff utile pour le craft
                                }
                            }
                            lock( ListeWorkorderAttenteRessource)
                            {
                                ListeWorkorderAttenteRessource.Remove(wo);
                                this.ListeWorkorderAttenteRessource = ListeWorkorderAttenteRessource;
                            }

                            WorkorderAttenteCraft = wo; break;
                        }
                    }
                }
                else
                {
                    lock (WorkorderAttenteCraft)
                    {
                        if (ToutEstEnBanque(WorkorderAttenteCraft))
                        {
                            Crafteur crafteur = (Crafteur)ListePersonnages.Where(x => x.GetType() == typeof(Crafteur)).FirstOrDefault();
                            if (crafteur != null)
                            {
                                crafteur.CrafterWorkOrder = true;
                            }
                        }
                    }
                }

                //code mort
                /*if (WorkOrderEnCoursdeCollecte == null)
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
                }*/
                
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
                ConsoleManager.Write("Server time : " + response.data.server_time);
                foreach (Announcement announcement in response.data.announcements)
                {
                    ConsoleManager.Write(announcement.message);
                    ConsoleManager.Write(announcement.created_at.ToString("dd/MM/yyyy"));
                }
                ConsoleManager.Write("Last wipe : " + response.data.last_wipe);
                ConsoleManager.Write("Next wipe : " + response.data.next_wipe);
            }
        }

        internal void SupprimerCommandeLivree(WorkOrder wo)
        {
            if (WorkorderAttenteCraft == wo)
            {
                WorkorderAttenteCraft = null;
            }
            else
            {
                throw new Exception("on essaie de me faire supprimer une commande qui n'est pas celle attendue");
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
                    int NbRequises = Int32.Parse(item.quantity)*quantite - nb_existant;
                    if (NbRequises > 0)
                    {
                        //maintenant on doit regarder si c'est une ressource, un drop ou un Objet
                        Objet obj = ObjetList.Where(x => x.code.Equals(item.code)).First();
                        if (obj.type == "resource" ||obj.code == "wooden_stick")
                        {
                            listeDesResources.Add(new Tuple<string, int>(item.code, NbRequises));
                        }
                        else
                        {
                            GenererListeDesDrops(item.code, NbRequises, _ItemsDeEquipe);
                        }
                    }
                }

                return listeDesResources;
            }
        }

        public WorkOrder GetWorkorderAttenteCraft()
        {
            if (WorkorderAttenteCraft == null)
            {
                return null;
            }
            lock (WorkorderAttenteCraft)
            {
                return WorkorderAttenteCraft;
            }
        }

        private List<Tuple<string, int>> DecomposerObjet(string objetCode, int quantite)
        {
            List<Tuple<string, int>> liste = new List<Tuple<string, int>>();

            Objet obj = ObjetList.Where(x => x.code.Equals(objetCode)).First();
            if (obj.type == "resource")
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

        /*public bool isCraftable(List<Tuple<string, int>> listeDrops)
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
        }*/

        public List<Tuple<Enums.EmplacementPieceStuff, string>> CSPCombat(Personnage.Personnage p, Monster m)
        {
            CSP_Fight cSP_Fight = new CSP_Fight(p, m, this.ObjetList);
            return cSP_Fight.GenererListePossibilites();
        }

        public List<WorkOrder> GetListeWorkorderAttenteRessource()
        {
            lock (ListeWorkorderAttenteRessource)
            {
                return new List<WorkOrder>(ListeWorkorderAttenteRessource);
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
            lock (BankItemList)
            {
                ChargerBanque();
                foreach (Item item in ConsulterBanque())
                {
                    Tuple<string, int> t = new Tuple<string, int>(item.code, Int32.Parse(item.quantity));
                    listeBanque.Add(t);
                }
                List<Tuple<string, int>> liste_compos = GenererListeDesDrops(workOrder.Code, workOrder.Quantité, listeBanque);
                if (liste_compos.Count == 0)
                {
                    return true;
                }
                return false;
            }
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
            lock (BankItemList)
            {
                Character c = MyCharactersEndPoint.DepositBank(name, code, quantity);
                ChargerBanque();
                CalculerItemsDeEquipeSaufCrafteur();
                return c;
            }
        }

        internal Character DeposerGold(string name, int amount)
        {
            Character c = MyCharactersEndPoint.DepositBankGold(name, amount);
            return c;
        }

        public Character RecupererDeBanque(string name, string code, int quantite)
        {
            if (quantite == 0)
            {
                return null;
            }

            lock (BankItemList)
            {
                Character c = MyCharactersEndPoint.WithdrawBank(name,code, quantite);
                ChargerBanque();
                CalculerItemsDeEquipeSaufCrafteur();
                return c;
            }
        }

        public void AjouterWorkOrder(WorkOrder workOrder)
        {
            lock (ListeWorkorderAttenteRessource)
            {
                ListeWorkorderAttenteRessource.Add(workOrder);
            }
        }

        public List<Tuple<string,int>> GetListeDesResourcesPourTousLesCrafts()
        {
            lock(ListeDesResourcesPourTousLesCrafts)
            {
                return ListeDesResourcesPourTousLesCrafts;
            }
        }


        private List<Tuple<string, int>> CalculerItemsDeEquipeSaufCrafteur()
        {
            List<Tuple<string, int>> ItemsDeEquipe = new List<Tuple<string, int>>();
            ItemsDeEquipe.Clear();
            foreach (Personnage.Personnage p in ListePersonnages)
            {
                if (p.GetType() != typeof(Crafteur))
                {
                    foreach (Inventory inventory in p.FeuillePerso.inventory)
                    {
                        if (!string.IsNullOrEmpty(inventory.code))
                        {
                            Tuple<string, int> t = new Tuple<string, int>(inventory.code, inventory.quantity);
                            ItemsDeEquipe.Add(t);
                        }
                    }
                }
            }
            foreach(Item item in ConsulterBanque())
            {
                Tuple<string, int> t = new Tuple<string, int>(item.code, Int32.Parse(item.quantity));
                ItemsDeEquipe.Add(t);
            }
            return ItemsDeEquipe;
        }

        internal List<Resource> GetResourceList()
        {
            lock (ResourceList)
            {
                return ResourceList;
            }
        }
    }
}
