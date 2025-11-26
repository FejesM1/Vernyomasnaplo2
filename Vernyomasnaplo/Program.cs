using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Configuration;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Vernyomasnaplo
{
    internal class Program
    {

        static DateTime szul_datum;
        static bool fut = true;
        static string bejelentkezettFelhasznalo = "";
        
        static bool bejelentkezve = false;
        static bool kivalasztva = false;
        static string[] menupontok = { "Regisztrálás", "Bejelentkezés", "Adatok hozzáadása", "Adatok módosítása", "Adatok megjelenítése", "Adat törlése", "Beállítások", "Kilépés", "Felhasználók listázása (Admin)", "Összes felhasználó adatai (Admin)" };
        static Action[] fuggvenyek = { Regisztral, Bejelentkezes, AdatokHozzaadasa, Modosit, Megjelenit, Torol, Beallit, Kilep, AdminFelhasznalok, OsszesFelhasznaloAdatai };
        static int aktualis_menu_pont = 0;
        static int menupontok_szama = menupontok.Length;

        static ConsoleColor[] szinek = {
            ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow,
            ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Gray, ConsoleColor.Black
        };

        static string[] szinek_neve = {
            "Zöld","Piros","Kék","Sárga","Cian","Magenta","Szürke","Fekete"
        };



        static int szinek_szama = szinek.Length;
        static int alapszin = 0;
        static int alaphatter = 7;

        // 🔹 Fájlok elérési útja a projekt gyökerében
        static readonly string adatokFile = "Adatok.txt";
        static readonly string felhasznalokFile = "Felhaszn.txt";

        static List<string> adatok = new List<string>();


        static void Main(string[] args)
        {

            try
            {

                // 🔹 Fájlok létrehozása, ha nem léteznek
                if (!File.Exists(adatokFile)) File.Create(adatokFile).Close();
                if (!File.Exists(felhasznalokFile)) File.Create(felhasznalokFile).Close();
                bool adminLetezik = false;
                foreach (var sor in File.ReadAllLines(felhasznalokFile))
                {
                    var adatok = sor.Split(';');
                    if (adatok.Length >= 2 && adatok[0] == "admin")
                    {
                        adminLetezik = true;
                        break;
                    }
                }

                if (!adminLetezik)
                {
                    File.AppendAllText(felhasznalokFile, $"admin;1234;2000.01.01{Environment.NewLine}");
                    // Az adatok fájlba is felvehetjük üresen az adminhoz
                    File.AppendAllText(adatokFile, "admin(" + Environment.NewLine);
                }


                // 🔹 Adatok beolvasása
                adatok.Clear();
                foreach (var sor in File.ReadAllLines(adatokFile))
                {
                    if (!string.IsNullOrWhiteSpace(sor))
                        adatok.Add(sor);
                }
                Console.BackgroundColor = szinek[alaphatter];
                while (fut)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = szinek[alaphatter];

                    Console.Clear();


                    int startIndex, endIndex;




                    if (!bejelentkezve)
                    {
                        startIndex = 0;
                        endIndex = 2; // Regisztrálás, Bejelentkezés
                    }
                    else if (bejelentkezettFelhasznalo == "admin")
                    {
                        // Admin csak a Kilépés és az admin menüpontokat lássa
                        startIndex = 7; // Kilépés indexe
                        endIndex = menupontok.Length; // 10
                    }
                    else
                    {
                        // Normál felhasználó
                        startIndex = 2;
                        endIndex = menupontok.Length - 2; // kihagyjuk az admin menüpontokat
                    }


                    // Menü kiírása
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (aktualis_menu_pont < startIndex || aktualis_menu_pont >= endIndex)
                            aktualis_menu_pont = startIndex; // index biztonság

                        if (aktualis_menu_pont == i)
                        {
                            Console.ForegroundColor = szinek[alapszin];
                            Console.WriteLine(menupontok[i]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.WriteLine(menupontok[i]);
                        }
                    }



                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Enter:

                            if (aktualis_menu_pont >= startIndex && aktualis_menu_pont < endIndex)
                                fuggvenyek[aktualis_menu_pont]();


                            if (bejelentkezve && aktualis_menu_pont < 2)
                                aktualis_menu_pont = 2;
                            break;

                        case ConsoleKey.UpArrow:
                            if (aktualis_menu_pont > startIndex)
                                aktualis_menu_pont--;
                            else
                            {
                                aktualis_menu_pont = endIndex - 1;
                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (aktualis_menu_pont < endIndex - 1)
                                aktualis_menu_pont++;
                            else
                            {
                                aktualis_menu_pont = startIndex;
                            }
                            break;

                        default:
                            Console.Beep();
                            break;
                    }


                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Megkísérli az adminisztrátor hitelesítését a megadott felhasználónév és jelszó alapján.
        /// </summary>
        /// <remarks>Ez a metódus egy előre meghatározott fájlból olvassa a felhasználói hitelesítő adatokat.
        /// Minden sort ellenőriz a megadott felhasználónév és jelszó egyezésére. A felhasználónév összehasonlítása
        /// nem érzékeny a kis- és nagybetűkre, míg a jelszó összehasonlítása érzékeny a kis- és nagybetűkre.</remarks>
        /// <param name="nev">A hitelesítendő felhasználónév. Ez a paraméter nem érzékeny a kis- és nagybetűkre.</param>
        /// <param name="jelszo">A felhasználónévhez tartozó jelszó. Ez a paraméter érzékeny a kis- és nagybetűkre.</param>
        /// <returns><see langword="true"/> ha a felhasználónév és jelszó egyezik egy adminisztrátori fiókkal; egyébként <see langword="false"/>.</returns>

        static bool AdminBelépés(string nev, string jelszo)
        {
            foreach (var sor in File.ReadAllLines(felhasznalokFile))
            {
                var adatok = sor.Split(';');
                if (adatok.Length >= 2 &&
                    adatok[0].Trim().Equals("admin", StringComparison.OrdinalIgnoreCase) &&
                    adatok[1].Trim() == jelszo.Trim())
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Megállapítja, hogy létezik-e a megadott nevű felhasználó a felhasználói fájlban.
        /// </summary>
        /// <remarks>Ez a metódus beolvassa a felhasználói adatokat tartalmazó fájlt, és ellenőrzi,
        /// hogy van-e olyan bejegyzés, amely megfelel a megadott névnek. Ha hiba történik a fájl
        /// olvasása során, a metódus <see langword="false"/> értéket ad vissza.</remarks>
        /// <param name="nev">A keresett felhasználó neve.</param>
        /// <returns><see langword="true"/> ha létezik a megadott nevű felhasználó; egyébként <see langword="false"/>.</returns>

        static bool FelhasznaloLetezik(string nev)
        {
            try
            {
                foreach (var sor in File.ReadAllLines(felhasznalokFile))
                {
                    var adatok = sor.Split(';');
                    if (adatok.Length > 0 && adatok[0] == nev)
                        return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return false;
            }

        }



        /// <summary>
        /// Konzolos regisztrációs folyamatot hajt végre:
        /// bekéri a felhasználónevét, születési dátumát és jelszavát,
        /// ellenőrzi az érvényességet és az egyediséget,
        /// majd elmenti az adatokat a megfelelő fájlokba.
        /// Hiba esetén opcionálisan kiírja a kivétel részleteit.
        /// </summary>
        static void Regisztral()
        {

            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║      Regisztrálás      ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                
                Console.Write("Felhasználónév: ");
                string nev = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(nev))
                {
                    Console.WriteLine("A felhasználónév nem lehet üres!");
                    Console.ReadLine();
                    return;
                }

                if (FelhasznaloLetezik(nev))
                {
                    Console.WriteLine("Ez a felhasználónév már létezik!");
                    Console.ReadLine();
                    return;
                }
                Console.Write("Születési dátum: ");
                szul_datum = DateTime.Parse(Console.ReadLine());


                Console.Write("Jelszó: ");
                string jelszo = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(jelszo))
                {
                    Console.WriteLine("A jelszó nem lehet üres!");
                    Console.ReadLine();
                    return;
                }
                File.AppendAllText(felhasznalokFile, $"{nev};{jelszo};{szul_datum}{Environment.NewLine}");
                File.AppendAllText(adatokFile, $"{nev}({Environment.NewLine}");
                adatok.Add($"{nev}(");
                Console.WriteLine("Sikeres regisztráció!");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Konzolos bejelentkezést végez: bekéri a felhasználónevet és jelszót,
        /// ellenőrzi azokat a felhasználói adatfájlban,
        /// siker esetén beállítja a bejelentkezett állapotot és a kapcsolódó változókat,
        /// sikertelenség esetén hibaüzenetet ír ki.
        /// Kivételkor opcionálisan megjeleníti a hiba részleteit.
        /// </summary>
        static void Bejelentkezes()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║      Bejelentkezés     ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                Console.Write("Felhasználónév: ");
                string nev = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(nev))
                {
                    Console.WriteLine("A felhasználónév nem lehet üres!");
                    Console.ReadLine();
                    return;
                }

                Console.Write("Jelszó: ");
                string jelszo = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(jelszo))
                {
                    Console.WriteLine("A jelszó nem lehet üres!");
                    Console.ReadLine();
                    return;
                }

                bool sikeres = false;
                foreach (var sor in File.ReadAllLines(felhasznalokFile))
                {
                    var adatokSor = sor.Split(';');
                    if (adatokSor.Length >= 2 && adatokSor[0].Trim() == nev && adatokSor[1].Trim() == jelszo)
                    {
                        if (adatokSor.Length >= 3)
                            szul_datum = Convert.ToDateTime(adatokSor[2]);
                        else
                            szul_datum = DateTime.MinValue; // adminhoz nincs dátum

                        sikeres = true;
                        break;
                    }
                }

                if (sikeres)
                {
                    bejelentkezettFelhasznalo = nev;
                    
                    bejelentkezve = true;

                    if (AdminBelépés(nev, jelszo))
                    {
                        Console.WriteLine("Admin bejelentkezve!");
                        aktualis_menu_pont = menupontok.Length - 1; // admin menüpont index
                    }
                    else
                    {
                        Console.WriteLine("Sikeres bejelentkezés!");
                        aktualis_menu_pont = 2; // normál felhasználó
                    }
                }
                else
                {
                    Console.WriteLine("Hibás felhasználónév vagy jelszó!");
                }


                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Felhasználók listáját jeleníti meg az előre meghatározott fájl beolvasásával, és kiírja az adataikat a konzolra.
        /// </summary>
        /// <remarks>Ez a metódus a <c>felhasznalokFile</c> változóval megadott fájlból olvassa be a felhasználói adatokat.
        /// A fájl minden sora várhatóan felhasználói információt tartalmaz pontosvesszővel elválasztott formátumban.
        /// A metódus kiírja a felhasználónevet, jelszót és születési dátumot minden felhasználóhoz a konzolra.
        /// Ha hiba történik a fájl olvasása során, hibaüzenet jelenik meg.</remarks>

        static void AdminFelhasznalok()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║     Felhasználók       ║");
                Console.WriteLine("╚════════════════════════╝\n");

                foreach (var sor in File.ReadAllLines(felhasznalokFile))
                {
                    var adatok = sor.Split(';');
                    if (adatok.Length >= 3)
                    {
                        Console.WriteLine($"Felhasználó: {adatok[0]}, Jelszó: {adatok[1]}, Születési dátum: {adatok[2]}");
                    }
                }

                Console.WriteLine("\nNyomjon Entert a visszatéréshez.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hiba történt: {e.Message}");
            }
        }
        /// <summary>
        /// Minden felhasználó adatait, beleértve a méréseiket is, kiírja a konzolra.
        /// </summary>
        /// <remarks>Ez a metódus törli a konzolt, majd kiírja minden felhasználó nevét, és ha rendelkezésre állnak, a méréseiket is.
        /// A mérések tartalmazzák a szisztolés és diasztolés vérnyomást, a pulzust és a dátumot.
        /// A felhasználót megkérdezi, hogy nyomjon Entert a visszatéréshez az adatok megtekintése után.</remarks>

        static void OsszesFelhasznaloAdatai()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║  Összes felhasználó    ║");
                Console.WriteLine("╚════════════════════════╝\n");

                foreach (var sor in adatok)
                {
                    string[] resz = sor.Split('(');
                    string nev = resz[0];
                    string meresek = resz.Length > 1 ? resz[1] : "";

                    Console.WriteLine($"Felhasználó: {nev}");
                    if (!string.IsNullOrEmpty(meresek))
                    {
                        string[] m = meresek.Split('|');
                        for (int i = 0; i < m.Length; i++)
                        {
                            string[] adat = m[i].Split(';');
                            if (adat.Length >= 4)
                            {
                                Console.WriteLine($"  {i + 1}. mérés: Szisztolés: {adat[0]}, Diasztolés: {adat[1]}, Pulzus: {adat[2]}, Dátum: {adat[3]}");
                            }
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Nyomjon Entert a visszatéréshez.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt: " + e.Message);
            }
        }

        /// <summary>
        /// Lehetővé teszi a felhasználó számára, hogy új méréseket adjon hozzá.
        /// Bekéri a szisztolés és diasztolés értékeket, valamint a pulzust,
        /// majd az aktuális idővel együtt elmenti azokat a felhasználó adataihoz
        /// és frissíti az Adatok.txt fájlt. Hibák esetén opcionálisan kiírja a kivételt.
        /// </summary>
        static void AdatokHozzaadasa()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║   Adatok hozzáadása    ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                bool talalat = false;
                string nev = "";
                int index = 0;
                DateTime most = DateTime.Now;

                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        nev = adatok[i].Split('(')[0];
                        talalat = true;
                        index = i;
                        break;
                    }
                }
                if (talalat == true)
                {
                    Console.Write("Adja meg a szisztolést: ");
                    int szisztoles = int.Parse(Console.ReadLine());

                    Console.Write("Adja meg a diasztolést: ");
                    int diasztoles = int.Parse(Console.ReadLine());


                    Console.Write("Adja meg a pulzusát: ");
                    int pulzus = int.Parse(Console.ReadLine());


                    string keszadat = $"{szisztoles};{diasztoles};{pulzus};{most}";

                    if (adatok[index].Split('(')[1] != "")
                    {
                        adatok[index] += "|";
                        adatok[index] += keszadat;
                    }
                    else
                    {
                        adatok[index] += keszadat;
                    }
                }
                else
                {
                    Console.WriteLine("Az Adatok.txt be nem került bele a felhasználó neved!");
                    Console.ReadLine();
                }
                File.WriteAllText(adatokFile, "");
                for (int i = 0; i < adatok.Count; i++)
                {
                    File.AppendAllText(adatokFile, adatok[i] + Environment.NewLine);
                }
                Console.WriteLine("Adat hozzáadva.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }

        }

        /// <summary>
        /// Lehetővé teszi a felhasználó számára, hogy meglévő mérési adatait módosítsa.
        /// Megjeleníti a felhasználó összes mérését, majd bekéri, melyik mérés adatait kívánja módosítani,
        /// és az új szisztolés, diasztolés és pulzus értékeket elmenti a felhasználó adataihoz.
        /// A változtatások frissítik az Adatok.txt fájlt. Hibák esetén opcionálisan kiírja a kivételt.
        /// </summary>
        static void Modosit()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║   Adatok módosítása    ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                bool talalat = false;
                int index = 0;
                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        talalat = true;
                        index = i;
                        break;
                    }
                }

                

                int darab = adatok[index].Split('(')[1].Split('|').Count();
                int törles;
                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;

                if (talalat == true)
                {
                    Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]} ({eletkor} éves)\n");
                    if (adatok[index].Split('(')[1] != "")
                    {
                        for (int i = 0; i < darab; i++)
                        {
                            Console.Write($"A(z) {i + 1}. mérés eredménye {adatok[index].Split('(')[1].Split('|')[i].Split(';')[3]} szisztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]} diasztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]} pulzus: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[2]}\n");
                        }
                    }
                }

                Console.Write("\nAdja meg a módosítandó mérés sorszámát: ");
                törles = int.Parse(Console.ReadLine());
                Console.Write("Adja meg a szisztolést: ");
                int szisztoles = int.Parse(Console.ReadLine());
                Console.Write("Adja meg a diasztolést: ");
                int diasztoles = int.Parse(Console.ReadLine());
                Console.Write("Adja meg a pulzusát: ");
                int pulzus = int.Parse(Console.ReadLine());

                var modositas = $"{szisztoles};{diasztoles};{pulzus};{adatok[index].Split('(')[1].Split('|')[törles - 1].Split(';')[3]}";

                var start = adatok[index].IndexOf(adatok[index].Split('(')[1].Split('|')[törles - 1]);
                var vege = adatok[index].Split('(')[1].Split('|')[törles - 1].Length;

                adatok[index] = adatok[index].Remove(start, vege);

                adatok[index] = adatok[index].Insert(start, modositas);

                File.WriteAllText(adatokFile, "");
                for (int i = 0; i < adatok.Count; i++)
                {
                    File.AppendAllText(adatokFile, adatok[i] + Environment.NewLine);
                }
                Console.WriteLine("Adat módosítva.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Megjeleníti a bejelentkezett felhasználó összes mérési adatait (szisztolés, diasztolés, pulzus),
        /// kiszámítja és kiírja a legnagyobb és legkisebb értékeket, valamint a diagnózisokat.
        /// A felhasználó megadhat egy-egy értéket, és a program kiszámítja, hogy a mérési adatok hány százaléka haladja meg azt az értéket.
        /// Hibák esetén opcionálisan kiírja a kivétel részleteit.
        /// </summary>
        static void Megjelenit()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║  Adatok megjelenítése  ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");

                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;
                bool talalat = false;
                int index = 0;
                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        talalat = true;
                        index = i;
                        break;
                    }
                }
               

                if (talalat == true)
                {
                    Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]} ({eletkor} éves)\n");
                    if (adatok[index].Split('(')[1] != "")
                    {
                        int darab = adatok[index].Split('(')[1].Split('|').Count();
                        if (darab > 0)
                        {
                            // Legnagyobbak
                            int leg_szisz = int.Parse(adatok[index].Split('(')[1].Split('|')[0].Split(';')[0]);
                            int leg_diasz = int.Parse(adatok[index].Split('(')[1].Split('|')[0].Split(';')[1]);
                            int leg_pul = int.Parse(adatok[index].Split('(')[1].Split('|')[0].Split(';')[2]);

                            // Legkisebbek
                            int legk_szisz = leg_szisz;
                            int legk_diasz = leg_diasz;
                            int legk_pul = leg_pul;

                            for (int i = 0; i < darab; i++)
                            {
                                int curr_szisz = int.Parse(adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]);
                                int curr_diasz = int.Parse(adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]);
                                int curr_pul = int.Parse(adatok[index].Split('(')[1].Split('|')[i].Split(';')[2]);

                                // Legnagyobbak
                                if (leg_szisz < curr_szisz) leg_szisz = curr_szisz;
                                if (leg_diasz < curr_diasz) leg_diasz = curr_diasz;
                                if (leg_pul < curr_pul) leg_pul = curr_pul;

                                // Legkisebbek
                                if (legk_szisz > curr_szisz) legk_szisz = curr_szisz;
                                if (legk_diasz > curr_diasz) legk_diasz = curr_diasz;
                                if (legk_pul > curr_pul) legk_pul = curr_pul;

                                Console.Write($"A(z) {i + 1}. mérés eredménye:\nDátum: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[3]}\nAdatok: szisztolés: {curr_szisz}, diasztolés: {curr_diasz}, pulzus: {curr_pul}\n" +
                                    $"Diagnózis: {Vernyomas(curr_szisz, curr_diasz)}  {pulzusfigyelo(curr_pul)}\n\n");
                            }

                            Console.WriteLine();
                            
                            Console.WriteLine($"Legnagyobb szisztolés: {leg_szisz}");
                            Console.WriteLine($"Legnagyobb diasztolés: {leg_diasz}");
                            Console.WriteLine($"Legnagyobb pulzus: {leg_pul}");
                            Console.WriteLine($"Legkisebb szisztolés: {legk_szisz}");
                            Console.WriteLine($"Legkisebb diasztolés: {legk_diasz}");
                            Console.WriteLine($"Legkisebb pulzus: {legk_pul}");
                        }


                        Console.WriteLine();

                    }
                }



                List<int> szisz = new List<int>();
                List<int> diasz = new List<int>();
                List<int> pulzus = new List<int>();
                for (int i = 0; i < adatok.Count; i++)
                {
                    for (int b = 0; b < adatok[i].Split('(')[1].Split('|').Count();b++)
                    {

                        int leg_szisz = int.Parse(adatok[i].Split('(')[1].Split('|')[b].Split(';')[0]);
                        int leg_diasz = int.Parse(adatok[i].Split('(')[1].Split('|')[b].Split(';')[1]);
                        int leg_pul = int.Parse(adatok[i].Split('(')[1].Split('|')[b].Split(';')[2]);

                        szisz.Add(leg_szisz);   
                        diasz.Add(leg_diasz);
                        pulzus.Add(leg_pul);
                       
                    }

                }

                Console.WriteLine("Szisztolés értékek:");
                foreach (int s in szisz)
                {
                    Console.Write($"{s} ");
                }

                Console.WriteLine("\nDiasztolés értékek:");
                foreach (int d in diasz)
                {
                    Console.Write($"{d} ");
                }

                Console.WriteLine("\nPulzus értékek:");
                foreach (int p in pulzus)
                {
                    Console.Write($"{p} ");
                }

                Console.WriteLine();

                // Szisztolés
                Console.Write("Adj meg egy számot a szisztoléshez: ");
                int sziszSzam = int.Parse(Console.ReadLine());

                int sziszFelott = 0;
                for (int i = 0; i < szisz.Count; i++)
                {
                    if (szisz[i] > sziszSzam)
                        sziszFelott++;
                }

                double sziszSzazalek = (double)sziszFelott / szisz.Count * 100;
                Console.WriteLine($"A szisztolés értékek {sziszSzazalek.ToString("F2")}% -a nagyobb, mint {sziszSzam}.");

                // Diasztolés
                Console.Write("Adj meg egy számot a diasztoléshez: ");
                int diaszSzam = int.Parse(Console.ReadLine());

                int diaszFelott = 0;
                for (int i = 0; i < diasz.Count; i++)
                {
                    if (diasz[i] > diaszSzam)
                        diaszFelott++;
                }

                double diaszSzazalek = (double)diaszFelott / diasz.Count * 100;
                Console.WriteLine($"A diasztolés értékek {diaszSzazalek.ToString("F2")}% -a nagyobb, mint {diaszSzam}.");

                // Pulzus
                Console.Write("Adj meg egy számot a pulzushoz: ");
                int pulzusSzam = int.Parse(Console.ReadLine());

                int pulzusFelott = 0;
                for (int i = 0; i < pulzus.Count; i++)
                {
                    if (pulzus[i] > pulzusSzam)
                        pulzusFelott++;
                }

                double pulzusSzazalek = (double)pulzusFelott / pulzus.Count * 100;
                Console.WriteLine($"A pulzus értékek {pulzusSzazalek.ToString("F2")}% -a nagyobb, mint {pulzusSzam}.");





                Console.WriteLine("\nNyomjon Entert a visszatéréshez.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.Write($"{e} ");
                }
            }
        }

        /// <summary>
        /// Meghatározza a vérnyomás kategóriáját a megadott szisztolés és diasztolés értékek alapján.
        /// </summary>
        /// <param name="szisztoles">A szisztolés vérnyomás értéke.</param>
        /// <param name="diasztoles">A diasztolés vérnyomás értéke.</param>
        /// <returns>Szöveges leírás a vérnyomás állapotáról (pl. normális, alacsony, hipertónia fokozata).</returns>
        static string Vernyomas(int szisztoles, int diasztoles)
        {
            try
            {
                if (szisztoles <= 90 && diasztoles <= 60) { return "Figyelem: a vérnyomás értéke alacsony."; }
                else if (szisztoles <= 120 && diasztoles <= 80) { return "A vérnyomása normális."; }
                else if (szisztoles <= 140 && diasztoles <= 90) { return "Figyelem: a vérnyomása emelkedett."; }
                else if (szisztoles <= 160 && diasztoles <= 100) { return "Figyelem: a vérnyomás értéke magas! Ez 1. fokú hipertónia."; }
                else if (szisztoles <= 180 && diasztoles <= 110) { return "Figyelem: a vérnyomás értéke magas! Ez 2. fokú hipertónia."; }
                else { return "Figyelem: a vérnyomás értéke magas! Ez 3. fokú hipertónia."; }
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return "hiba a Vernyomásnál";
            }
        }

        /// <summary>
        /// Törli a bejelentkezett felhasználó kiválasztott mérését az adatokból és frissíti a fájlt.
        /// </summary>
        static void Torol()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                Console.Clear();
                Console.WriteLine("╔════════════════════════╗");
                Console.WriteLine("║                        ║");
                Console.WriteLine("║     Adatok törlése     ║");
                Console.WriteLine("║                        ║");
                Console.WriteLine("╚════════════════════════╝");


                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;
                bool talalat = false;
                int index = 0;
                for (int i = 0; i < adatok.Count; i++)
                {
                    if (adatok[i].Split('(')[0] == bejelentkezettFelhasznalo)
                    {
                        talalat = true;
                        index = i;
                        break;
                    }
                }
                Console.WriteLine("Adatok megjelenítése:\n");
                int darab = 0;
                if (talalat == true)
                {
                    darab = adatok[index].Split('(')[1].Split('|').Count();
                    Console.WriteLine($"A felhasználó neve: {adatok[index].Split('(')[0]} ({eletkor} éves)\n");
                    if (adatok[index].Split('(')[1] != "")
                    {
                        for (int i = 0; i < darab; i++)
                        {

                            Console.Write($"A(z) {i + 1}. mérés eredménye:\nDátum: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[3]}\nAdatok: szisztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[0]}, diasztolés: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[1]}, pulzus: {adatok[index].Split('(')[1].Split('|')[i].Split(';')[2]}\n");
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("Adja meg melyiket kívánja törölni?");

                    Console.WriteLine(adatok[index]);
                    int torol_index = int.Parse(Console.ReadLine()) - 1;





                    string torolni = $"{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[0]};{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[1]};{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[2]};{adatok[index].Split('(')[1].Split('|')[torol_index].Split(';')[3]}";

                    if (torol_index > 0 && darab > 1)
                    {
                        torolni = "|" + torolni;

                    }
                    else if (darab > 1)
                    {
                        torolni += "|";
                    }
                    Console.WriteLine(torolni);
                    string modositott = adatok[index].Replace(torolni, "");
                    Console.WriteLine(modositott);

                    string[] sorok = File.ReadAllLines("Adatok.txt");
                    sorok[index] = modositott;

                    File.WriteAllLines("Adatok.txt", sorok);
                    adatok[index] = modositott;


                }

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Kiértékeli a bejelentkezett felhasználó pulzusát az életkora alapján.
        /// Visszaadja, hogy a pulzus alacsony, normális vagy magas.
        /// </summary>
        /// <param name="pulzus">A felhasználó aktuális pulzusértéke.</param>
        /// <returns>Szöveges értékelés a pulzusról.</returns>
        static string pulzusfigyelo(int pulzus)
        {
            try
            {

                DateTime most = DateTime.Now;
                int eletkor = most.Year - szul_datum.Year;

                if (eletkor >= 0 && eletkor <= 1) // Csecsemő
                {
                    if (pulzus < 120) return "Alacsony a pulzusod.";
                    else if (pulzus > 160) return "Magas a pulzusod.";
                }
                else if (eletkor > 1 && eletkor <= 2) // Kisgyermek
                {
                    if (pulzus < 110) return "Alacsony a pulzusod.";
                    else if (pulzus > 150) return "Magas a pulzusod.";
                }
                else if (eletkor > 2 && eletkor <= 5) // Óvodás
                {
                    if (pulzus < 80) return "Alacsony a pulzusod.";
                    else if (pulzus > 120) return "Magas a pulzusod.";
                }
                else if (eletkor > 5 && eletkor <= 12) // Iskolás
                {
                    if (pulzus < 70) return "Alacsony a pulzusod.";
                    else if (pulzus > 110) return "Magas a pulzusod.";
                }
                else if (eletkor > 12 && eletkor <= 18) // Serdülő
                {
                    if (pulzus < 60) return "Alacsony a pulzusod.";
                    else if (pulzus > 100) return "Magas a pulzusod.";
                }
                else if (eletkor >= 19) // Felnőttek (19 év felett)
                {
                    if (pulzus < 60) return "Alacsony a pulzusod.";
                    else if (pulzus > 100) return "Magas a pulzusod.";
                }

                return "Normális a pulzusod.";
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return "hiba a pulzus figyeloben";
            }
        }

        /// <summary>
        /// Megjeleníti a felhasználó számára a színek beállítási menüjét,
        /// lehetővé téve a betű- és háttérszín kiválasztását.
        /// </summary>
        static void Beallit()
        {
            try
            {
                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];
                string[] kiirni = { "Betű", "Háttér" };
                bool megy = true;
                int aktualis = 0;
                while (megy)
                {
                    Console.Clear();

                    for (int i = 0; i < kiirni.Length; i++)
                    {
                        if (aktualis == i)
                        {
                            Console.ForegroundColor = szinek[alapszin];
                            Console.WriteLine(kiirni[i]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.WriteLine(kiirni[i]);
                        }
                    }

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Enter:
                            int szin = Szinvalaszto();
                            if (szin == -1)
                            {
                                break;
                            }
                            if (alaphatter != szin && alapszin != szin)
                            {
                                megy = false;
                                if (aktualis == 0)
                                    alapszin = szin;
                                else
                                    alaphatter = szin;
                            }

                            break;

                        case ConsoleKey.DownArrow:
                            if (aktualis < kiirni.Length - 1)
                                aktualis++;
                            else
                            {
                                aktualis = 0;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (aktualis > 0)
                                aktualis--;
                            else
                            {
                                aktualis = kiirni.Length - 1;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            megy = false;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Lehetővé teszi a felhasználó számára egy szín kiválasztását a rendelkezésre álló színek közül,
        /// kizárva a már beállított betű- és háttérszínt. 
        /// A felhasználó a nyilakkal navigálhat, Enterrel választ, vagy a bal nyíllal megszakíthatja a folyamatot.
        /// </summary>
        /// <returns>
        /// A kiválasztott szín indexét adja vissza a színek tömbből, vagy -1-et, ha a felhasználó megszakította a választást.
        /// </returns>
        static int Szinvalaszto()
        {
            try
            {


                Console.BackgroundColor = szinek[alaphatter];
                Console.ForegroundColor = szinek[alapszin];


                ConsoleColor[] szinek_kicsi = new ConsoleColor[szinek.Length - 1];
                string[] szinek_neve_kicsi = new string[szinek_neve.Length - 1];
                int jo_index = 0;
                for (int i = 0; i < szinek.Length; i++)
                {
                    if (i == alapszin || i == alaphatter)
                        continue; // ezt a színt kihagyjuk

                    szinek_kicsi[jo_index] = szinek[i];
                    szinek_neve_kicsi[jo_index] = szinek_neve[i];
                    jo_index++;
                }


                bool kivalaszt = true;
                int akt_szin_szama = 0;

                while (kivalaszt)
                {
                    Console.Clear();

                    for (int i = 0; i < szinek_kicsi.Length; i++)
                    {



                        if (i == akt_szin_szama)
                        {

                            Console.ForegroundColor = szinek_kicsi[i];
                            Console.WriteLine(szinek_neve_kicsi[i]);
                            Console.ForegroundColor = ConsoleColor.White;


                        }
                        else
                        {
                            Console.WriteLine(szinek_neve_kicsi[i]);
                        }

                    }

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Enter:
                            kivalaszt = false;
                            break;
                        case ConsoleKey.UpArrow:

                            if (akt_szin_szama > 0)
                                akt_szin_szama--;
                            else
                            {
                                akt_szin_szama = szinek_szama - 3;
                            }
                            break;
                        case ConsoleKey.DownArrow:

                            if (akt_szin_szama < szinek_szama - 3)
                                akt_szin_szama++;
                            else
                            {
                                akt_szin_szama = 0;
                            }
                            break;
                        default:
                            Console.Beep();
                            break;
                        case ConsoleKey.LeftArrow:
                            kivalaszt = false;
                            return -1;

                    }
                }

                int vissza = Array.IndexOf(szinek, szinek_kicsi[akt_szin_szama]);
                return vissza;
            }
            catch (Exception e)
            {
                Console.WriteLine("Hiba történt szeretné látni a hibát?");
                if (Console.ReadLine() == "igen")
                {

                    Console.WriteLine(e);
                }
                return 0;
            }
        }

        /// <summary>
        /// Leállítja a program futását a fő ciklusban.
        /// </summary>
        static void Kilep()
        {
            fut = false;
        }
    }
}